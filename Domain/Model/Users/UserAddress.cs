using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/19 20:48:57
*描述：
*
***********************************************************/
namespace Domain.Model.Users
{
    public class UserAddress
    {
        public int Id { get; set; }
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
