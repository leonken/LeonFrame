using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 11:45:42
*描述：
*
***********************************************************/
namespace EventBus.Event
{
    /// <summary>
    /// 集成事件
    /// 事件是一种描述已经发生的事实，如其名称所述。
    /// 集成事件是一种会对微服务、受限上下文、外部系统造成影响的事件
    /// </summary>
    public class IntegrationEvent
    {
        public Guid Id { get; }
        public DateTime CreateTime { get; }

        public IntegrationEvent()
        {
            this.Id = Guid.NewGuid();
            this.CreateTime = DateTime.Now;
        }
    }
}
