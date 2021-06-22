using Core.Bases.Repository;
using Domain.Model.Users;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/21 14:36:43
*描述：
*
***********************************************************/
namespace Domain.IRepository
{
    public interface IUserRepository : IBaseRepository<UserInfo>
    {
    }
}
