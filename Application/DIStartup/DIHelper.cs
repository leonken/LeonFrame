using Domain.IntegrationEvents;
using EventBusRabbitMQ;
using EventBusRabbitMQ.Abstractions;
using IntegrationEventLog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Data.Common;

namespace Application.DIStartup
{
    public static class DIHelper
    {
        /// <summary>
        /// 集成相关
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCustomInterations(this IServiceCollection sc, IConfiguration configuration)
        {
            sc.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //sc.AddTransient<IIdentityService, IdentityService>();//认证服务

            sc.AddTransient<Func<DbConnection, IIntegrationEventLogService>>();//集成事件log服务的工厂

            sc.AddTransient<IIntegrationEventService, IntegrationEventService>();

            sc.AddSingleton<IRabbitMQConnection, RabbitMQConnection>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<RabbitMQConnection>>();

                //在这里配置connection的注入，并配置用户名和密码
                string mqConnection = configuration["EventBusConnection"];
                string username = configuration["EventBusUserName"];
                string pwd = configuration["EventBusPassword"];
                int retry = int.Parse(configuration["EventBusRetryCount"]);

                ConnectionFactory connectionFactory = new ConnectionFactory
                {
                    UserName = username,
                    Password = pwd,
                    HostName = mqConnection
                };

                return new RabbitMQConnection(connectionFactory, logger, retry);
            });

            return sc;
        }
    }
}
