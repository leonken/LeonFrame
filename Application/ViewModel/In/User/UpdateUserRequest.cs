using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/20 20:04:27
*描述：
*
***********************************************************/
namespace Application.ViewModel.In.User
{
    /// <summary>
    /// 用户信息更新请求
    /// </summary>
    public class UpdateUserRequest
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

        ///// <summary>
        ///// 用户编号
        ///// </summary>
        //public string UserCode { get; set; }

        /// <summary>
        /// 用户地址
        /// </summary>
        public List<UpdateUserAddressVM> UserAddresses { get; set; }

        public class UpdateUserAddressVM
        {
            /// <summary>
            /// 地址标题
            /// </summary>
            public string AddressTitle { get; set; }

            /// <summary>
            /// 邮编
            /// </summary>
            public string ZIP { get; set; }

            /// <summary>
            /// 国家
            /// </summary>
            public string Country { get; set; }

            /// <summary>
            /// 省
            /// </summary>
            public string Province { get; set; }

            /// <summary>
            /// 市
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 区
            /// </summary>
            public string Area { get; set; }

            /// <summary>
            /// 街道
            /// </summary>
            public string Street { get; set; }
        }
    }
}
