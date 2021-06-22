using Domain.IntegrationEvents.Events;
using EventBus.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IntegrationEvents.Handlers
{
    /// <summary>
    /// 用户创建的集成事件处理程序
    /// </summary>
    public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private ILogger<UserCreatedIntegrationEventHandler> _logger;
        private IMediator _mediator;

        public UserCreatedIntegrationEventHandler(ILogger<UserCreatedIntegrationEventHandler> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(UserCreatedIntegrationEvent @event)
        {

            _logger.LogInformation("正在处理RabbitMQ推送来的UserCreatedIntegrationEvent事件....");

            //如果有业务逻辑也可以抛command,但是有个问题，command在Application中，会产生循环引用，后期解决方案：
            //不单独建该项目，而是放到Application中
        }
    }
}
