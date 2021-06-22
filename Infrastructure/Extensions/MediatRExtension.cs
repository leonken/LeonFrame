using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Bases.Models;
using Infrastructure.DBContext;
using MediatR;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/26 20:47:17
*描述：
*
***********************************************************/
namespace Infrastructure.Extensions
{
    /// <summary>
    /// MediatR扩展类
    /// </summary>
    public static class MediatRExtension
    {
        /// <summary>
        /// 发布所有实体内的领域事件
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, LContext context)
        {
            //所有包含领域事件的实体
            var domainEntities = context.ChangeTracker
             .Entries<Entity>()
             .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

            domainEntities.ToList().ForEach(r => r.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}
