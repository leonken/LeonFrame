using Autofac;
using EventBus;
using EventBus.Abstractions;
using EventBus.Event;
using EventBusRabbitMQ.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/**********************************************************
*Copyright (c) 2021  All Rights Reserved.
*创建人：  liuhd4
*创建时间：2021/4/20 17:28:42
*描述：
***********************************************************/
namespace EventBusRabbitMQ
{
    public class EventBusTest : IEventBus
    {
        public IRabbitMQConnection _rabbitMQConnection;
        public ILogger<EventBusTest> _logger;
        public IEventBusSubscriptionManager _subscriptionManager;
        public ILifetimeScope _lifetimeScope;

        private string _QueueName = "LeonQueue";
        private string _ExchangeName = "LeonExchange";
        private IModel _consumerChannel;

        public EventBusTest(IRabbitMQConnection  rabbitMQConnection, ILogger<EventBusTest>  logger
            , IEventBusSubscriptionManager  subscriptionManager, ILifetimeScope  lifetimeScope)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _logger = logger;
            _subscriptionManager = subscriptionManager;
            _lifetimeScope = lifetimeScope;
        }

        private IModel InitConsumerChannel()
        {
            if (!_rabbitMQConnection.IsConnected)
            {
                _rabbitMQConnection.TryConnect();
            }

            var channel = _rabbitMQConnection.CreateModel();

            channel.ExchangeDeclare(_ExchangeName, "Direct", true, false);
            channel.QueueDeclare(_QueueName, true, false, false);

            channel.CallbackException += (sender, e) =>
            {
                _logger.LogWarning("消费者channel发生异常,正在重启通路:" + e.Exception.Message);

                _consumerChannel.Dispose();

                _consumerChannel = InitConsumerChannel();

                ///Leon
                StartConsume();
            };

            return channel;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_rabbitMQConnection.IsConnected)
                _rabbitMQConnection.TryConnect();

            using (var channel = _rabbitMQConnection.CreateModel())
            {
                string eventName = @event.GetType().Name;
                string message = Newtonsoft.Json.JsonConvert.SerializeObject(@event);
                byte[] messageByte = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // 1-不持久化 2-持久化

                channel.BasicPublish(_ExchangeName, eventName, properties, messageByte); 
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!_subscriptionManager.HasSubscriptionsForEvent<T>())
            {
                _subscriptionManager.AddSubscription<T, TH>();

                if (!_rabbitMQConnection.IsConnected)
                    _rabbitMQConnection.TryConnect();

                using (var channel = _rabbitMQConnection.CreateModel())
                {
                    string eventName = typeof(T).Name;

                    channel.QueueBind(_QueueName, _ExchangeName, eventName);
                }
            }
        }

        private void StartConsume()
        {
            var consume = new AsyncEventingBasicConsumer(_consumerChannel);

            consume.Received += async (sender, @event) =>
            {
                string eventName = @event.RoutingKey;

                //TO-DO

                _consumerChannel.BasicAck(@event.DeliveryTag, false);
            };

            _consumerChannel.BasicConsume(_QueueName, false, consume);
        }
 

        public void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler
        {
            throw new NotImplementedException();
        }
    }
}
