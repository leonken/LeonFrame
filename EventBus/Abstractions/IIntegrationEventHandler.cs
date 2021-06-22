using EventBus.Event;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 14:55:16
*描述：
*
***********************************************************/
namespace EventBus.Abstractions
{
    public interface IIntegrationEventHandler
    {
    }

    public interface IIntegrationEventHandler<in T> : IIntegrationEventHandler where T : IntegrationEvent
    {
        Task Handle(T @event);
    }
}
