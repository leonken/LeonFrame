using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 14:30:02
*描述：
*
***********************************************************/
namespace EventBus.Abstractions
{
    /// <summary>
    /// 事件总线接口
    /// 事件总线仅用于集成事件，领域事件直接发布到MediatR
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布集成事件
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IntegrationEvent @event);

        /// <summary>
        /// 订阅集成事件处理Handler
        /// </summary>
        /// <typeparam name="T">集成事件</typeparam>
        /// <typeparam name="TH">集成事件处理器</typeparam>
        void Subscribe<T, TH>() 
            where T : IntegrationEvent 
            where TH : IIntegrationEventHandler<T>;

        /// <summary>
        /// 取消订阅事件处理器
        /// </summary>
        /// <typeparam name="T">指定事件</typeparam>
        /// <typeparam name="TH">指定处理器</typeparam>
        void Unsubscribe<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        /// <summary>
        /// 订阅动态类型事件与处理器
        /// </summary>
        /// <typeparam name="TH">动态类型事件处理器</typeparam>
        /// <param name="eventName">事件名</param>
        void SubscribeDynamic<TH>(string eventName) 
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 取消订阅动态类型事件与处理器
        /// </summary>
        /// <typeparam name="TH">动态类型事件处理器</typeparam>
        /// <param name="eventName">事件名</param>
        void UnsubscribeDynamic<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;
    }
}
