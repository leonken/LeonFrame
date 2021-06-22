using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.Event;
using EventBus.Extensions;
using EventBusRabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/*
职责：
发布事件，初始化队列绑定，启动消费者，实现消费者使用相应的EventHandler处理逻辑
*/
namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private const string _BROKER_NAME = "leon_eventbus"; 
        private const string _BINGDING_KEY = "leon_bindingkey";
        private const string _AUTOFAC_SCOPE_NAME = "leon_eventbus";

        private readonly IRabbitMQConnection _rabbitMQConnection;
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly int _retryCount;

        /// <summary>
        /// 消费者的通道
        /// </summary>
        private IModel _consumerChannel;
        private string _queueName;

        public EventBusRabbitMQ(IRabbitMQConnection rabbitMQConnection, ILogger<EventBusRabbitMQ> logger
            , IEventBusSubscriptionManager eventBusSubscriptionManager
            , ILifetimeScope lifetimeScope, string queueName, int retryCount = 5)
        {
            _rabbitMQConnection = rabbitMQConnection ?? throw new ArgumentNullException(nameof(rabbitMQConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventBusSubscriptionManager = eventBusSubscriptionManager ?? new EventBusSubscriptionManager();
            _lifetimeScope = lifetimeScope;
            _retryCount = retryCount;
            _queueName = queueName;
            _consumerChannel = CreateConsumerChannel();
            _eventBusSubscriptionManager.OnEventRemoved += _eventBusSubscriptionManager_OnEventRemoved;
        }

        public void Dispose()
        {
            if (_rabbitMQConnection != null)
            {
                _rabbitMQConnection.Dispose();
            }

            _eventBusSubscriptionManager.Clear();
        }

        /// <summary>
        /// 发布集成事件
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IntegrationEvent @event)
        {
            if (!_rabbitMQConnection.IsConnected)
            {
                _rabbitMQConnection.TryConnect();
            }

            //Polly
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
               .Or<SocketException>()
               .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
               {
                   _logger.LogWarning(ex, "无法发布事件: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
               });

            string eventName = @event.GetType().Name;
            string routeKey = eventName;

            _logger.LogTrace("创建RabbitMQ通道并发布事件:{EventId}({EventName})", @event.Id, eventName);

            using (var channel = _rabbitMQConnection.CreateModel())
            {
                channel.ExchangeDeclare(_BROKER_NAME, "direct", false, false, null);//定义交换机（在CreateConsumerChannel其实已经定义过了）
                //channel.QueueDeclare(queueName, false, false, false, null); 队列不在此处声明

                //消息体
                string message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // 1-不持久化 2-持久化

                    _logger.LogTrace("发布集成事件到RabbitMQ：" + @event.Id);

                    channel.BasicPublish(_BROKER_NAME
                        , routeKey //将消息发布到bindingKey==eventName的队列上
                        , true
                        , properties
                        , body);//发布消息
                });
            }
        }

        #region 订阅&解除订阅

        /// <summary>
        /// 订阅集成事件与处理器
        /// </summary>
        /// <typeparam name="T">集成事件</typeparam>
        /// <typeparam name="TH">集成事件处理器</typeparam>
        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            string eventName = typeof(T).Name;

            BindExchangeAndQueueWithKey(eventName);//使用eventName作为BindingKey绑定队列与交换机

            _logger.LogInformation($"绑定事件{eventName}与处理器{typeof(TH).GetGenericTypeName()}");

            _eventBusSubscriptionManager.AddSubscription<T, TH>(); //事件管理器添加T与TH的绑定 (EventBus本身是singleton)

            StartBasicConsume();//启动消费者
        }

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation($"绑定dynamic事件{eventName}与处理器{typeof(TH).GetGenericTypeName()}");

            BindExchangeAndQueueWithKey(eventName);//绑定队列

            _eventBusSubscriptionManager.AddDynamicSubscription<TH>(eventName);

            StartBasicConsume();//启动消费者
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _logger.LogInformation($"解除事件绑定{typeof(T).Name}");
            _eventBusSubscriptionManager.RemoveSubscription<T, TH>();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            _logger.LogInformation($"解除dynamic事件绑定{eventName}");
            _eventBusSubscriptionManager.RemoveDynamicSubscription<TH>(eventName);
        }

        #endregion

        #region 事件

        /// <summary>
        /// 注册给订阅管理器的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventName"></param>
        private void _eventBusSubscriptionManager_OnEventRemoved(object sender, string eventName)
        {
            if (!_rabbitMQConnection.IsConnected)
            {
                _rabbitMQConnection.TryConnect();
            }

            using (var channel = _rabbitMQConnection.CreateModel())
            {
                channel.QueueUnbind(_queueName, _BROKER_NAME, eventName);//当某种类型的事件解绑了所有对应得Handler时就解绑队列

                if (_eventBusSubscriptionManager.IsEmpty)
                {
                    _queueName = "";
                    _consumerChannel.Close();//订阅管理器清空时关闭通道
                }
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 使用eventName作为BindingKey绑定队列与交换机
        /// </summary>
        /// <param name="eventName"></param>
        private void BindExchangeAndQueueWithKey(string eventName)
        {
            var containsKey = _eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!_rabbitMQConnection.IsConnected)
                {
                    _rabbitMQConnection.TryConnect();
                }

                using (var channel = _rabbitMQConnection.CreateModel())
                {
                    channel.QueueBind(_queueName
                        , _BROKER_NAME  //交换机名：Broker名
                        , eventName   //由生产端绑定交换机与队列，RoutingKey是事件的名称
                        , null);
                }
            }
        }

        /// <summary>
        /// 启动消费者
        /// </summary>
        private void StartBasicConsume()
        {
            _logger.LogTrace("正在启动RabbitMQ消费者...");

            var consume = new AsyncEventingBasicConsumer(_consumerChannel);

            consume.Received += async (sender, @event) =>
            {
                var eventName = @event.RoutingKey;//设计的时候事件名称=路由Key
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());

                try
                {
                    if (message.ToLower().Contains("throw-fake-exception"))
                    {
                        throw new InvalidOperationException($"Fake exception requested:\"{message}\"");
                    }

                    //业务逻辑
                    await ProcessEvent(eventName, message);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "处理该消息时发生错误:" + message);
                }

                _consumerChannel.BasicAck(@event.DeliveryTag, false);//回应ack消息
            };

            _consumerChannel.BasicConsume(_queueName, false, consume); //应用消费者
        }

        /// <summary>
        /// 处理事件逻辑
        /// 有一些骚操作
        /// </summary>
        /// <param name="eventName">事件名：用于查找出他的事件Handler</param>
        /// <param name="message">事件体：用于反射出事件实例，应用于Handler</param>
        private async Task ProcessEvent(string eventName, string message)
        {
            if (_eventBusSubscriptionManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope(_AUTOFAC_SCOPE_NAME))
                {
                    var subscriptionInfos = _eventBusSubscriptionManager.GetHandlersForEvent(eventName);

                    foreach (var subscription in subscriptionInfos)
                    {
                        if (subscription.IsDynamic)
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType) as IDynamicIntegrationEventHandler;

                            if (handler == null) continue;

                            var @eventData = JObject.Parse(message);

                            await Task.Yield();//一个低优先级的线程执行后面的逻辑
                            await handler.Handle(@eventData);
                        }
                        else
                        {
                            var handler = scope.ResolveOptional(subscription.HandlerType);
                            if (handler == null) continue;

                            var eventType = _eventBusSubscriptionManager.GetEventTypeByName(eventName);
                            var @eventData = JsonConvert.DeserializeObject(message, eventType);
                            var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                            await Task.Yield();
                            await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { eventData });
                        }
                    }
                }
            }
            else
            {
                _logger.LogWarning($"没有找到事件{eventName}订阅的处理器!");
            }
        }

        /// <summary>
        /// 创建消费者通道(基于同一个连接)
        /// </summary>
        /// <returns></returns>
        private IModel CreateConsumerChannel()
        {
            if (!_rabbitMQConnection.IsConnected)
            {
                _rabbitMQConnection.TryConnect();
            }

            _logger.LogTrace("创建消费者通道..");

            var channel = _rabbitMQConnection.CreateModel();

            //channel.BasicQos(,2,true)

            channel.ExchangeDeclare(_BROKER_NAME, "direct", false, false, null);

            channel.QueueDeclare(_queueName, true, false, false, null);  //声明一个队列

            /*此处不负责绑定交换机与队列，由生产端完成*/

            channel.CallbackException += (sender, e) =>
            {  //当消费者发生异常时

                _logger.LogWarning(e.Exception, "正在重新创建RabbitMQ消费者通道...");

                _consumerChannel.Dispose();

                _consumerChannel = CreateConsumerChannel();//重新建立通道

                StartBasicConsume();//重新启动消费者
            };

            return channel;
        }

        #endregion

    }
}
