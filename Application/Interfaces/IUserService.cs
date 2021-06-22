using Application.ViewModel;
using Application.ViewModel.In.User;
using Application.ViewModel.Out.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 19:48:49
*描述：
*
***********************************************************/
namespace Application.Interfaces
{
    /// <summary>
    /// 用户服务
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 根据用户id获取用户信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        UserInfoVM GetUserInfo(string uid);

        /// <summary>
        /// 更新User
        /// </summary>
        /// <param name="user"></param>
        Task UpdateUser(UpdateUserRequest user);
    }
}
