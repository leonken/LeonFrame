using EventBusRabbitMQ.Abstractions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Polly;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using System.Net.Sockets;
using RabbitMQ.Client.Exceptions;

namespace EventBusRabbitMQ
{
    /// <summary>
    /// RMQ连接类
    /// </summary>
    public class RabbitMQConnection : IRabbitMQConnection
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMQConnection> _logger;
        private IConnection _connection;
        /// <summary>
        /// 重试次数
        /// </summary>
        private readonly int _retryCount;
        private object _locker = new object();
        private bool _disposed;

        public RabbitMQConnection(IConnectionFactory connectionFactory, ILogger<RabbitMQConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
            _retryCount = retryCount;
        }

        public bool IsConnected
        {
            get
            {
                return _connection != null && _connection.IsOpen && !_disposed;
            }
        }

        /// <summary>
        /// 创建通道
        /// </summary>
        /// <returns></returns>
        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("没有可用的RabbitMQ连接");
            }
            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (_disposed) return;
            try
            {
                _connection.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.ToString());
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ客户端正在尝试连接...");

            lock (_locker)
            {
                //定义策略
                var policy = RetryPolicy.Handle<SocketException>()
                         .Or<BrokerUnreachableException>()
                         .WaitAndRetry(_retryCount, r => TimeSpan.FromSeconds(Math.Pow(2, r)), (ex, timespan) =>
                         {
                             _logger.LogWarning(ex, $"RabbitMQ在{timespan.TotalSeconds:n1}秒后无法连接.{ex.Message}", ex.Message);
                         });
                //执行
                policy.Execute(() =>
                {
                    _connection = _connectionFactory.CreateConnection();
                });

                //检查
                if (IsConnected)
                {
                    //订阅事件
                    _connection.ConnectionShutdown += _connection_ConnectionShutdown;
                    _connection.CallbackException += _connection_CallbackException;
                    _connection.ConnectionBlocked += _connection_ConnectionBlocked;

                    _logger.LogInformation("RabbitMQ Client 连接到 '{HostName}' 并订阅处理事件 ", _connection.Endpoint.HostName);

                    return true;
                }
                else
                {
                    _logger.LogCritical("严重错误：RabbitMQ无法建立连接！");
                    return false;
                }
            }
        }
        /// <summary>
        /// 连接被阻止时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _connection_ConnectionBlocked(object sender, RabbitMQ.Client.Events.ConnectionBlockedEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接关闭，正在尝试重连....");

            TryConnect();
        }
        /// <summary>
        /// 回调失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _connection_CallbackException(object sender, RabbitMQ.Client.Events.CallbackExceptionEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接出现异常，正在尝试重连....");

            TryConnect();
        }
        /// <summary>
        /// 连接关闭时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_disposed) return;

            _logger.LogWarning("RabbitMQ连接关闭，正在尝试重连....");

            TryConnect();
        }
    }
}
