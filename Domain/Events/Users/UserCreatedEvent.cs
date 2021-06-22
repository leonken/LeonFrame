using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/24 21:01:32
*描述：
*
***********************************************************/
namespace Domain.Events.Users
{
    public class UserCreatedEvent : INotification
    {
        public string UserName { get; }

        public UserCreatedEvent(string username)
        {
            this.UserName = username;
        }
    }
}
