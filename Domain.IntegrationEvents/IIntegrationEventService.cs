using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 14:21:40
*描述：
*
***********************************************************/
namespace Domain.IntegrationEvents
{
    /// <summary>
    /// 基础集成事件服务接口
    /// </summary>
    public interface IIntegrationEventService
    {
        /// <summary>
        /// 添加并保存集成事件
        /// </summary>
        /// <param name="event">事件</param>
        public Task AddAndSaveIntegrationEventAsync(IntegrationEvent @event);

        /// <summary>
        /// 发布集成事件
        /// </summary>
        /// <param name="transactionId">事务ID</param>
        public Task PublishIntegrationEventAsync(Guid transactionId);
    }
}
