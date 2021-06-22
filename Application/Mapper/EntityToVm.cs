using Application.ViewModel.Out.User;
using AutoMapper;
using Domain.Model.Users;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 20:00:51
*描述：
*
***********************************************************/
namespace Application.Mapper
{
    public class EntityToVm: Profile
    {
        public EntityToVm()
        {
            CreateMap<UserInfo, UserInfoVM>();
            CreateMap<UserAddress, UserAddressVM>();
        }
    }
}
