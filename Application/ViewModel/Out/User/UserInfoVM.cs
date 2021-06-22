using Core.Bases.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 17:54:01
*描述：
*
***********************************************************/
namespace Application.ViewModel.Out.User
{
    public class UserInfoVM : IViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 用户地址
        /// </summary>
        public List<UserAddressVM> UserAddresses { get; set; }
    }
}
