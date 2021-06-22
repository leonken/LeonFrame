using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ.Abstractions
{
    /// <summary>
    /// RMQ连接接口
    /// </summary>
    public interface IRabbitMQConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();
        /// <summary>
        /// 创建通道
        /// </summary>
        /// <returns></returns>
        IModel CreateModel();
    }
}
