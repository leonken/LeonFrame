using EventBus.Abstractions;
using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    /// <summary>
    /// 事件总线订阅管理器
    /// </summary>
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }
        /// <summary>
        /// 事件移除时的事件
        /// </summary>
        event EventHandler<string> OnEventRemoved;
        /// <summary>
        /// 添加动态事件处理程序的订阅
        /// </summary>
        /// <typeparam name="TH"></typeparam>
        /// <param name="eventName"></param>
        void AddDynamicSubscription<TH>(string eventName)
           where TH : IDynamicIntegrationEventHandler;
        
        /// <summary>
        /// 添加集成事件处理程序订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;
        /// <summary>
        /// 移除事件的订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TH"></typeparam>
        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;
        /// <summary>
        /// 移除动态事件的订阅
        /// </summary>
        /// <typeparam name="TH"></typeparam>
        /// <param name="eventName"></param>
        void RemoveDynamicSubscription<TH>(string eventName)
            where TH : IDynamicIntegrationEventHandler;

        /// <summary>
        /// 事件是否已经有订阅的Handler
        /// </summary>
        /// <typeparam name="T">事件类型</typeparam>
        /// <returns></returns>
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        /// <summary>
        /// 事件是否已经有订阅的Handler
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        bool HasSubscriptionsForEvent(string eventName);
        /// <summary>
        /// 根据集成事件名称获取事件(动态事件除外)
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        Type GetEventTypeByName(string eventName);
        /// <summary>
        /// 清空事件订阅关系
        /// </summary>
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
