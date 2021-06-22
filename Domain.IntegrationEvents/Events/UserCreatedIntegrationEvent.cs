using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 11:59:17
*描述：
*
***********************************************************/
namespace Domain.IntegrationEvents.Events
{
    /// <summary>
    /// 用户创建-集成事件
    /// </summary>
    public class UserCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserName { get; }
        public int Age { get; }

        public UserCreatedIntegrationEvent(string username, int age)
        {
            this.UserName = username;
            this.Age = age;
        }
    }
}
