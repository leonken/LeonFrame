using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Model.Users;
using PMS.Infrastruct.Mappings;

/**********************************************************

*创建人：  liuhd4
*创建时间：2020/10/19 20:30:46
*描述：
*
***********************************************************/
namespace Infrastructure.Mapping.UserInfos
{
    public class UserInfoMapping : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.ToTable("Usr_UserInfo");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.UserName).HasColumnType(CType.NVARCHAR_L(50));
            builder.Property(r => r.UserCode).HasColumnType(CType.VARCHAR_L(100));
            builder.OwnsMany(r => r.UserAddresses, r =>
            {
                r.ToTable("Usr_UserAddress");
                r.HasKey(o => o.Id);
                r.Property(r => r.Province).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("Province");
                r.Property(r => r.Street).HasColumnType(CType.NVARCHAR_L(200)).HasColumnName("Street");
                r.Property(r => r.ZIP).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("ZIP");
                r.Property(r => r.AddressTitle).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("AddressTitle");
                r.Property(r => r.Area).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("Area");
                r.Property(r => r.City).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("City");
                r.Property(r => r.Country).HasColumnType(CType.NVARCHAR_L(50)).HasColumnName("Country");
            });
        }
    }
}
