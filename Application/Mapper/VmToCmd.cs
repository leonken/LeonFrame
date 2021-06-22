using Application.Commands.User;
using Application.ViewModel.In.User;
using AutoMapper;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 20:00:29
*描述：
*
***********************************************************/
namespace Application.Mapper
{
    public class VmToCmd : Profile
    {
        public VmToCmd()
        {
            CreateMap<UpdateUserRequest, UpdateUserCommand>();
            CreateMap<UpdateUserRequest.UpdateUserAddressVM, UpdateUserCommand.UpdateUserAddressCommand>();
        }
    }
}
