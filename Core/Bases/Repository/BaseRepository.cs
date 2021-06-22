using Core.Bases.Interfaces;
using Core.Bases.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/21 14:41:14
*描述：
*
***********************************************************/
namespace Core.Bases.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class, IAggregateRoot
    {
        protected DbContext _dbContext;
        protected DbSet<T> _set;

        public IUnitOfWork UnitOfWork { get; private set; }

        public BaseRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            UnitOfWork = dbContext as IUnitOfWork;
            _set = _dbContext.Set<T>();
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity"></param>
        public void Add(T entity)
        {
            _set.Add(entity);
        }

        public void AddRange(IEnumerable<T> entity)
        {
            _set.AddRange(entity);
        }

        /// <summary>
        /// 通过主键id查找
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindById(object id)
        {
            return _set.Find(id);
        }

        public void Update(T entity)
        {
            _set.Update(entity);
        }

        public void Delete(T entity)
        {
            _set.Remove(entity);
        }
    }
}
