using System;
using System.Collections.Generic;
using System.Text;
using IntegrationEventLog.Mapping;
using IntegrationEventLog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 17:16:56
*描述：
*
***********************************************************/
namespace IntegrationEventLog
{
    /// <summary>
    /// IntegraionEventLog的上下文不使用DI生成，由LContext在使用时new
    /// </summary>
    public class IntegrationEventLogDbContext : DbContext
    {
        public IntegrationEventLogDbContext() { }

        public IntegrationEventLogDbContext(DbContextOptions<IntegrationEventLogDbContext> options) : base(options)
        { }

        public virtual DbSet<IntegrationEventLogs> IntegrationEventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new IntegrationEventLogMapping());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies(true);
            

            base.OnConfiguring(optionsBuilder);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IntegrationEventLogDesignOptions : IDesignTimeDbContextFactory<IntegrationEventLogDbContext>
    {
        public IntegrationEventLogDbContext CreateDbContext(string[] args)
        {
            return new IntegrationEventLogDbContext(
                new DbContextOptionsBuilder<IntegrationEventLogDbContext>()
                .UseSqlServer("Server=.;Database=LeonFrameDB;uid=sa;password=wanfangdata;MultipleActiveResultSets=true", optBuilder =>
                {
                    optBuilder.MigrationsAssembly("Infrastructure");
                }).Options
                );
        }
    }
}
