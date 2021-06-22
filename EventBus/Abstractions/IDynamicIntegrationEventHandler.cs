using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 16:31:52
*描述：
*
***********************************************************/
namespace EventBus.Abstractions
{
    /// <summary>
    /// 动态事件类型处理器
    /// </summary>
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic @event);
    }
}
