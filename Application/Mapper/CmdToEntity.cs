using Application.Commands.User;
using AutoMapper;
using Domain.Model.Users;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 20:00:43
*描述：
*
***********************************************************/
namespace Application.Mapper
{
    public class CmdToEntity : Profile
    {
        public CmdToEntity()
        {
            CreateMap<UpdateUserCommand, UserInfo>()//使用构造函数映射，以触发领域事件
                    .ForCtorParam("age", s => s.MapFrom(ss => ss.Age))
                    .ForCtorParam("addresses", s => s.MapFrom(ss => ss.UserAddresses))
                    .ForCtorParam("usercode", s => s.MapFrom(ss => ss.UserCode))
                    .ForCtorParam("username", s => s.MapFrom(ss => ss.UserName));
            CreateMap<UpdateUserCommand.UpdateUserAddressCommand, UserAddress>();
        }
    }
}
