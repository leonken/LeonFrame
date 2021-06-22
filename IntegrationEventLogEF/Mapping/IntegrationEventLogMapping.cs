using System;
using System.Collections.Generic;
using System.Text;
using IntegrationEventLog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/11/25 17:36:59
*描述：
*
***********************************************************/
namespace IntegrationEventLog.Mapping
{
    public class IntegrationEventLogMapping : IEntityTypeConfiguration<IntegrationEventLogs>
    {
        public void Configure(EntityTypeBuilder<IntegrationEventLogs> builder)
        {
            builder.ToTable("IntegrationEventLog");
            builder.HasKey(r => r.EventId);
            builder.Property(r => r.EventTypeName).HasColumnType("varchar(300)");
            builder.Property(r => r.CreationTime).HasColumnType("datetime");
            builder.Property(r => r.TransactionId).HasColumnType("varchar(100)");
        }
    }
}
