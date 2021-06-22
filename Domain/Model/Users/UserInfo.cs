using Core.Bases.Models;
using Domain.Events.Users;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/19 20:23:15
*描述：
*
***********************************************************/
namespace Domain.Model.Users
{
    public class UserInfo : Entity, IAggregateRoot
    {
        public UserInfo() { }
        public UserInfo(string username, int age, string usercode, List<UserAddress> addresses)
        {
            this.UserName = username;
            this.Age = age;
            this.UserCode = usercode;
            this.UserAddresses = addresses;

            AddOrderCreatedDomainEvent();
        }

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
        public virtual List<UserAddress> UserAddresses { get; set; }

        #region 此处为业务逻辑

        /// <summary>
        /// 添加用户创建事件
        /// </summary>
        private void AddOrderCreatedDomainEvent()
        {
            base.AddDomainEvent(new UserCreatedEvent(this.UserName));
        }

        #endregion
    }
}
