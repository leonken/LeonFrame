using Application.Commands.User;
using Application.Interfaces;
using Application.ViewModel.In.User;
using Application.ViewModel.Out.User;
using AutoMapper;
using Domain.IRepository;
using MediatR;
using System.Threading.Tasks;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 19:51:56
*描述：
*
***********************************************************/
namespace Application.Services
{
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        IMapper _mapper;
        IMediator _mediator;
        public UserService(IUserRepository userRepository, IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public UserInfoVM GetUserInfo(string uid)
        {
            return _mapper.Map<UserInfoVM>(_userRepository.FindById(uid));
        }

        public async Task UpdateUser(UpdateUserRequest user)
        {
            var command = _mapper.Map<UpdateUserCommand>(user);
            await _mediator.Send(command);
        }
    }
}
