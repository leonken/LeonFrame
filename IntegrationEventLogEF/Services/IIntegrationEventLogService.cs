using EventBus.Event;
using IntegrationEventLog.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 17:51:45
*描述：
*
***********************************************************/
namespace IntegrationEventLog.Services
{
    public interface IIntegrationEventLogService
    {
        /// <summary>
        /// 检索所有待发布到总线的事件
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        Task<IEnumerable<IntegrationEventLogs>> RetrieveEventPendingLogs(Guid transactionId);

        /// <summary>
        /// 保存事件
        /// </summary>
        /// <param name="event"></param>
        /// <param name="transaction">db事务</param>
        /// <returns></returns>
        Task SaveEventToDBAsync(IntegrationEvent @event, IDbContextTransaction transaction);

        /// <summary>
        /// 事件标记为已发布
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task MarkEventAsPublishedAsync(Guid eventId);

        /// <summary>
        /// 事件标记为处理中
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task MarkEventAsInProgressAsync(Guid eventId);

        /// <summary>
        /// 事件标记为失败
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        Task MarkEventAsFailedAsync(Guid eventId);
    }
}
