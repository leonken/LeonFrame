using Core.Bases.Interfaces;
using Core.Bases.Repository;
using Domain.IRepository;
using Domain.Model.Users;
using Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/21 14:37:06
*描述：
*
***********************************************************/
namespace Infrastructure.Repository
{
    /// <summary>
    /// User仓储
    /// </summary>
    public class UserRepository : BaseRepository<UserInfo>, IUserRepository
    {
        private LContext _LContext;

        public UserRepository(LContext context) : base(context)
        {
            _LContext = context;
        }

       // public IUnitOfWork UnitOfWork => _LContext ?? throw new ArgumentNullException(nameof(_LContext));
    }
}
