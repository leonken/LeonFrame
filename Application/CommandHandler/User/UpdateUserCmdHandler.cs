using Application.Commands.User;
using AutoMapper;
using Domain.IntegrationEvents;
using Domain.IntegrationEvents.Events;
using Domain.IRepository;
using Domain.Model.Users;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/26 18:13:40
*描述：
*
***********************************************************/
namespace Application.CommandHandler.User
{
    public class UpdateUserCmdHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        IIntegrationEventService _userIntegrationEventService;
        IUserRepository _userRepository;
        IMapper _mapper;

        public UpdateUserCmdHandler(IIntegrationEventService userIntegrationEventService
            , IUserRepository userRepository
            , IMapper mapper
            )
        {
            _userIntegrationEventService = userIntegrationEventService;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            //发送集成事件
            await _userIntegrationEventService.AddAndSaveIntegrationEventAsync(
                 new UserCreatedIntegrationEvent(request.UserName, request.Age)
                 );

            //处理业务逻辑&触发领域事件
            UserInfo userInfo = _mapper.Map<UserInfo>(request);

            _userRepository.Add(userInfo);
            //提交数据库
            await _userRepository.UnitOfWork.SaveEntitiesAsync();

            return true;
        }
    }
}
