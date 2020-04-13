using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderAddress
    
    public partial class OrderAddressConfiguration : EntityTypeConfiguration<OrderAddress>
    {
        public OrderAddressConfiguration()
            : this("dbo")
        {
        }

        public OrderAddressConfiguration(string schema)
        {
            ToTable("OrderAddress", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ProvinceId).HasColumnName(@"ProvinceId").IsRequired().HasColumnType("int");
            Property(x => x.DistrictId).HasColumnName(@"DistrictId").IsRequired().HasColumnType("int");
            Property(x => x.WardId).HasColumnName(@"WardId").IsOptional().HasColumnType("int");
            Property(x => x.Address).HasColumnName(@"Address").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ProvinceName).HasColumnName(@"ProvinceName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.DistrictName).HasColumnName(@"DistrictName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WardName).HasColumnName(@"WardName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Phone).HasColumnName(@"Phone").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
