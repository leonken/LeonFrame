using Core.Bases.Interfaces;
using Core.Bases.Models;
using System;
using System.Collections.Generic;
using System.Text;


/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/21 16:02:09
*描述：
*
***********************************************************/
namespace Core.Bases.Repository
{
    public interface IBaseRepository<T> where T : class, IAggregateRoot
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity);

        public void AddRange(IEnumerable<T> entity);

        /// <summary>
        /// 通过主键id查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(object id);

        public void Update(T entity);

        public void Delete(T entity);

        IUnitOfWork UnitOfWork { get; }
    }
}
