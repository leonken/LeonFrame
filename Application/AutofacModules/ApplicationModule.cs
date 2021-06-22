using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.CommandHandler.User;
using Autofac;
using Core.Bases.Models;
using Domain.IntegrationEvents;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Abstractions;
using FluentValidation;
using Infrastructure.DBContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Application.AutofacModules
{
    /// <summary>
    /// 应用模块
    /// </summary>
    public class ApplicationModule : Autofac.Module
    {
        public string SqlConn { get; set; }
        public IConfiguration _configuration { get; private set; }

        public ApplicationModule(string sqlconn, IConfiguration configuration)
        {
            SqlConn = sqlconn;
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<LContext>()
            //    .AsSelf()
            //    .InstancePerLifetimeScope();

            //Validator register
            builder.RegisterAssemblyTypes(Assembly.Load("Application"))
                .Where(r => r.IsClosedTypeOf(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .InstancePerDependency();

            #region IntergrationEvent

            builder.RegisterType<IntegrationEventService>()
                .As<IIntegrationEventService>()
                .InstancePerDependency();

            #endregion

            #region EventBus

            builder.RegisterType<EventBusSubscriptionManager>().As<IEventBusSubscriptionManager>().AsSelf().SingleInstance();

            builder.Register<RabbitMQConnection>(componentContext =>
            {
                string username = _configuration["EventBusUserName"];
                string pwd = _configuration["EventBusPassword"];
                string host = _configuration["EventBusConnection"];
                string connection = _configuration["EventBusConnection"];
                int retryCount = int.Parse(_configuration["EventBusRetryCount"]);
                var logger = componentContext.Resolve<ILogger<RabbitMQConnection>>();
                var connectionFactory = new ConnectionFactory();
                connectionFactory.HostName = connection;
                connectionFactory.DispatchConsumersAsync = true;//设为true时支持AsyncEventingBasicConsumer异步消费者。默认为false。
                if (!string.IsNullOrWhiteSpace(username))
                    connectionFactory.UserName = username;
                if (!string.IsNullOrWhiteSpace(pwd))
                    connectionFactory.Password = pwd;
                return new RabbitMQConnection(connectionFactory, logger, retryCount);
            }).As<IRabbitMQConnection>().SingleInstance();

            builder.Register<EventBusRabbitMQ.EventBusRabbitMQ>(componentContext => {
                int retryCount = 5;
                string queueName = "leon_Queue";
                var connection = componentContext.Resolve<IRabbitMQConnection>();
                var logger = componentContext.Resolve<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                var lifetimescope = componentContext.Resolve<ILifetimeScope>();
                var eventbusManager = componentContext.Resolve<IEventBusSubscriptionManager>();
                return new EventBusRabbitMQ.EventBusRabbitMQ(connection, logger, eventbusManager, lifetimescope, queueName, retryCount);
            }).As<IEventBus>().SingleInstance();

            #endregion

            //Application Services
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
               .Where(r => r.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerDependency();
             
            builder.RegisterAssemblyTypes(typeof(LContext).Assembly)
                .Where(r => r.Name.EndsWith("Repository"))
                .Except<IAggregateRoot>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            //这里应该可以省略，因为使用事件总线管理订阅
            builder.RegisterAssemblyTypes(typeof(IIntegrationEventService).Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>))
                .InstancePerDependency();

            //builder.RegisterAssemblyTypes(typeof(IEventBus).Assembly)
            //    .

            base.Load(builder);
        }
    }
}
