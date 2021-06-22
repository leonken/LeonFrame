using EventBus.Abstractions;
using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventBus
{
    public class EventBusSubscriptionManager : IEventBusSubscriptionManager
    { 
        /// <summary>
        /// 集成事件与处理程序的对应关系
        /// </summary>
        private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;
        /// <summary>
        /// 已注册的集成事件类型列表
        /// </summary>
        private readonly List<Type> _eventTypes;

        public bool IsEmpty => _handlers.Keys.Any();

        public event EventHandler<string> OnEventRemoved;

        public EventBusSubscriptionManager()
        {
            _handlers = new Dictionary<string, List<SubscriptionInfo>>();
            _eventTypes = new List<Type>();
        }

        public void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            DoAddSubscription(typeof(TH), eventName, true);
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            DoAddSubscription(typeof(TH), typeof(T).Name, false);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }
        }

        public void Clear() => _handlers.Clear();

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _eventTypes.FirstOrDefault(r => r.Name == eventName);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            return GetHandlersForEvent(typeof(T).Name);
        }

        public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
        {
            return _handlers[eventName];
        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            string eventName = typeof(T).Name;
            return HasSubscriptionsForEvent(eventName);
        }

        public bool HasSubscriptionsForEvent(string eventName) => _handlers.ContainsKey(eventName);

        #region Remove 

        public void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            var subscriptionToRemove = RetriveSubscriptionByEventType(eventName, typeof(TH));
            if (subscriptionToRemove != null)
                DoRemoveHandler(eventName, subscriptionToRemove);
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var subscriptionToRemove = RetriveSubscriptionByEventType(typeof(T).Name, typeof(TH));
            if (subscriptionToRemove != null)
                DoRemoveHandler(typeof(T).Name, subscriptionToRemove);
        }

        #endregion 

        #region Private Methods

        /// <summary>
        /// 通过事件名称、处理器类型获得Subscription对象
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        private SubscriptionInfo RetriveSubscriptionByEventType(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return _handlers[eventName].FirstOrDefault(r => r.HandlerType == handlerType);
        }

        /// <summary>
        /// 注册 集成事件-集成事件处理器 （1对多)
        /// </summary>
        /// <param name="handlerType"></param>
        /// <param name="eventName"></param>
        /// <param name="isDynamic"></param>
        private void DoAddSubscription(Type handlerType, string eventName, bool isDynamic)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                _handlers.Add(eventName, new List<SubscriptionInfo>());
            }

            if (_handlers[eventName].Any(r => r.HandlerType == handlerType))
            {
                throw new ArgumentException($"Handler type {handlerType.Name} 已经注册给事件{eventName}", nameof(handlerType));
            }

            if (isDynamic)
            {
                _handlers[eventName].Add(SubscriptionInfo.Dynamic(handlerType));
            }
            else
            {
                _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
            }
        }

        /// <summary>
        /// 执行移除事件处理器绑定
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="subToRemove">事件处理器</param>
        private void DoRemoveHandler(string eventName, SubscriptionInfo subToRemove)
        {
            if (subToRemove != null)
            {
                _handlers[eventName].Remove(subToRemove);
                if (!_handlers[eventName].Any())//该类型事件被清空时
                {
                    _handlers.Remove(eventName);
                    var eventType = _eventTypes.Where(r => r.Name == eventName).FirstOrDefault();
                    if (eventType != null)
                    {
                        _eventTypes.Remove(eventType);
                        RaiseOnEventRemoved(eventName);
                    }
                }
            }
        }

        /// <summary>
        /// 事件清空时触发监听事件
        /// </summary>
        /// <param name="eventName"></param>
        private void RaiseOnEventRemoved(string eventName)
        {
            OnEventRemoved?.Invoke(this, eventName);
        }

        #endregion
         
    }
}
