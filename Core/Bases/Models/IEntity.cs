using System;
using System.Collections.Generic;
using System.Text;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/19 20:16:55
*描述：
*
***********************************************************/
namespace Core.Bases.Models
{
    public interface IEntity
    {
    }
    public interface IEntity<T> : IEntity
    {
        public T Id { get; set; }
    }
}
