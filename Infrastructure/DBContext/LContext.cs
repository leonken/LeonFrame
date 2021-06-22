using Domain.Model.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore.Design;
using Core.Bases.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore.Storage;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/19 20:04:49
*描述：
*
***********************************************************/
namespace Infrastructure.DBContext
{
    public class LContext : DbContext, IUnitOfWork
    {
        private IConfiguration _configuration;
        private IMediator _mediator;

        public LContext() { }
        public LContext(DbContextOptions<LContext> options, IConfiguration configuration, IMediator mediator) : base(options)
        {
            _configuration = configuration;
            _mediator = mediator;
        }

        public virtual DbSet<UserInfo> UserInfos { get; set; }

        /// <summary>
        /// 当前事务
        /// </summary>
        public IDbContextTransaction CurrentTransaction { get; private set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //泛型注入
            var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
                .Where(q => q.GetInterface(typeof(IEntityTypeConfiguration<>).FullName) != null);
            foreach (var type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                modelBuilder.ApplyConfiguration(configurationInstance);
            }
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        /// <summary>
        /// 发布领域事件并保存数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // 发布领域事件到MediatR
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // 提交到数据库
            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            await this.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 开启一个事务
        /// </summary>
        /// <returns></returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (this.Database.CurrentTransaction != null)
                return this.Database.CurrentTransaction;//此处有疑义，esp中直接返回null

            this.CurrentTransaction = await this.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);
            return this.CurrentTransaction;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <param name="transaction">事务对象</param>
        /// <returns></returns>
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != this.CurrentTransaction) throw new InvalidOperationException($"事务{transaction.TransactionId}不是当前事务!");

            try
            {
                await base.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                //回滚
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                CurrentTransaction?.Rollback();
            }
            catch
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <summary>
        /// 是否存在活动的事务
        /// </summary>
        public bool HasActiveTransaction => CurrentTransaction != null;
    }

    public class LContextDesignTimeOptions : IDesignTimeDbContextFactory<LContext>
    {
        public LContext CreateDbContext(string[] args)
        {
            return new LContext((new DbContextOptionsBuilder<LContext>())
                .UseSqlServer("Server=.;Database=LeonFrameDB;uid=sa;password=wanfangdata;MultipleActiveResultSets=true").Options
                , null, new MockMediatR());
        }

        public class MockMediatR : IMediator
        {
            public Task Publish(object notification, CancellationToken cancellationToken = default)
            {
                return Task.CompletedTask;
            }

            public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
            {
                return Task.CompletedTask;
            }

            public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task<object> Send(object request, CancellationToken cancellationToken = default)
            {
                return Task.FromResult((object)null);
            }
        }
    }
}
