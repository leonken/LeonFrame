using EventBus.Event;
using IntegrationEventLog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 18:12:46
*描述：
*
***********************************************************/
namespace IntegrationEventLog.Services
{
    public class IntegrationEventLogService : IIntegrationEventLogService
    {
        private IntegrationEventLogDbContext _db;
        /// <summary>
        /// 集成事件集合
        /// </summary>
        private List<Type> _eventTypes;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnection">由调用方传入共用连接</param>
        public IntegrationEventLogService(DbConnection dbConnection)
        {
            dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

            _db = new IntegrationEventLogDbContext(new DbContextOptionsBuilder<IntegrationEventLogDbContext>().UseSqlServer(dbConnection).Options);//基于LContext的连接创建db
            _eventTypes = Assembly.Load("Domain.IntegrationEvents").GetTypes().Where(r => r.Name.EndsWith(nameof(IntegrationEvent))).ToList();//加载集成事件类型
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEntry(eventId, EventStateEnum.PublishedFailed);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEntry(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEntry(eventId, EventStateEnum.Published);
        }

        private Task UpdateEntry(Guid eventid, EventStateEnum state)
        {
            var entry = _db.IntegrationEventLogs.Find(eventid);

            if (state == EventStateEnum.InProgress)
                entry.TimesSent++;
            entry.State = state;
            
            return _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<IntegrationEventLogs>> RetrieveEventPendingLogs(Guid transactionId)
        {
            //_db.Database.UseTransaction(transaction.GetDbTransaction());
            var result = await _db.IntegrationEventLogs.Where(r => r.TransactionId == transactionId.ToString()
                        && r.State == EventStateEnum.NotPublished).ToListAsync();

            if (result.Any())
                return result.OrderBy(r => r.CreationTime).Select(r =>
                  r.DeserializeJsonContent(_eventTypes.Where(k => k.Name == r.EventTypeShortName).FirstOrDefault())
                  );//检索符合条件的事件，并将事件内容反序列为事件对象，返回Log对象 

            return Enumerable.Empty<IntegrationEventLogs>();
        }

        public Task SaveEventToDBAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var entity = new IntegrationEventLogs(@event, transaction.TransactionId);

            _db.Database.UseTransaction(transaction.GetDbTransaction());//在MediatR的管道中启动了事务
            _db.IntegrationEventLogs.Add(entity);

            return _db.SaveChangesAsync();
        }
    }
}
