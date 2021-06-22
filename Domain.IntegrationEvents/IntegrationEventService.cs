using EventBus.Abstractions;
using EventBus.Event;
using Infrastructure.DBContext;
using IntegrationEventLog.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Threading.Tasks;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 14:26:42
*描述：
*
***********************************************************/
namespace Domain.IntegrationEvents
{
    /// <summary>
    /// 集成事件服务
    /// </summary>
    public class IntegrationEventService : IIntegrationEventService
    {
        /// <summary>
        /// 集成事件总线
        /// </summary>
        IEventBus _eventBus;
        /// <summary>
        /// 集成事件日志服务
        /// </summary>
        IIntegrationEventLogService _integrationEventLogService;
        /// <summary>
        /// LContext
        /// </summary>
        LContext _LContext;

        public IntegrationEventService(IEventBus eventBus, LContext lContext, Func<DbConnection, IIntegrationEventLogService> integrationEventLogServiceFactory)
        {
            _eventBus = eventBus;
            _LContext = lContext;
            _integrationEventLogService = integrationEventLogServiceFactory(lContext.Database.GetDbConnection());
        }

        public Task AddAndSaveIntegrationEventAsync(IntegrationEvent @event)
        {
            return _integrationEventLogService.SaveEventToDBAsync(@event, _LContext.Database.CurrentTransaction);
        }

        public async Task PublishIntegrationEventAsync(Guid transactionId)
        {
            //检索待发布事件
            var pendingIntegrationEvents = await _integrationEventLogService.RetrieveEventPendingLogs(transactionId);

            foreach (var e in pendingIntegrationEvents)
            {
                try
                {
                    await _integrationEventLogService.MarkEventAsInProgressAsync(e.EventId);
                    _eventBus.Publish(e.IntegrationEvent);//发布到消息总线
                    await _integrationEventLogService.MarkEventAsPublishedAsync(e.EventId);
                }
                catch (Exception ex)
                {
                    await _integrationEventLogService.MarkEventAsFailedAsync(e.EventId);
                }
            }
        }
    }
}
