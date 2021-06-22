using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Commands.User;
using Domain.IntegrationEvents.User;
using Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

/**********************************************************
*公司名称：Lenovo
*创建人：  liuhd4
*创建时间：2020/11/26 18:13:40
*描述：
*
***********************************************************/
namespace Domain.CommandHandler.User
{
    public class CreateUserCmdHandler : IRequestHandler<CreateUserCommand, bool>
    {
        IUserInfoIntegrationEventService _userIntegrationEventService;
        IUserRepository _userRepository;

        public CreateUserCmdHandler(IUserInfoIntegrationEventService userIntegrationEventService, IUserRepository userRepository)
        {
            _userIntegrationEventService = userIntegrationEventService;
            _userRepository = userRepository;
        }

        public Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            //发送集成事件

            //处理业务逻辑&触发领域事件

            //提交数据库

            return true;
        }
    }
}
