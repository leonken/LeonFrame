using EventBus.Event;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 17:23:21
*描述：
*
***********************************************************/
namespace IntegrationEventLog.Models
{
    public class IntegrationEventLogs
    {
        /// <summary>
        /// 事件日志实体
        /// </summary>
        public IntegrationEventLogs() { }
        /// <summary>
        /// 事件日志实体
        /// </summary>
        /// <param name="event"></param>
        /// <param name="transactionId"></param>
        public IntegrationEventLogs(IntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreateTime;
            EventTypeName = @event.GetType().FullName;
            Content = JsonConvert.SerializeObject(@event);
            State = EventStateEnum.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
        }
        /// <summary>
        /// 事件标识
        /// </summary>
        [Key]
        public Guid EventId { get; private set; }

        /// <summary>
        /// 事件类型
        /// </summary> 
        public string EventTypeName { get; private set; }

        /// <summary>
        /// 事件类型简称
        /// </summary>
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();

        /// <summary>
        /// 集成事件
        /// </summary>
        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        /// <summary>
        /// 事件状态
        /// </summary>
        public EventStateEnum State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TimesSent { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; private set; }
        /// <summary>
        /// 事件的序列化内容
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// 事务ID
        /// </summary>
        public string TransactionId { get; private set; }

        public IntegrationEventLogs DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonConvert.DeserializeObject(Content, type) as IntegrationEvent;
            return this;
        }
    }
}
