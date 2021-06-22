using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    /// <summary>
    /// 订阅信息
    /// </summary>
    public class SubscriptionInfo
    {
        /// <summary>
        /// 是否为动态类型处理器
        /// </summary>
        public bool IsDynamic { get; set; }
        /// <summary>
        /// 处理器类型
        /// </summary>
        public Type HandlerType { get; set; }

        private SubscriptionInfo(bool isDynamic, Type handlerType)
        {
            IsDynamic = isDynamic;
            HandlerType = handlerType;
        }

        /// <summary>
        /// 获取Dynamic类型实例
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static SubscriptionInfo Dynamic(Type handlerType)
        {
            return new SubscriptionInfo(true, handlerType);
        }

        /// <summary>
        /// 获取Type类型实例  
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        public static SubscriptionInfo Typed(Type handlerType)
        {
            return new SubscriptionInfo(false, handlerType);
        }
    }
}
