using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // WalletDetail

    public partial class WalletDetailConfiguration : EntityTypeConfiguration<WalletDetail>
    {
        public WalletDetailConfiguration()
            : this("dbo")
        {
        }

        public WalletDetailConfiguration(string schema)
        {
            ToTable("WalletDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.WalletId).HasColumnName(@"WalletId").IsRequired().HasColumnType("int");
            Property(x => x.WalletCode).HasColumnName(@"WalletCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"packageCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderPackageNo).HasColumnName(@"OrderPackageNo").IsOptional().HasColumnType("int");
            Property(x => x.Amount).HasColumnName(@"Amount").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TransportCode).HasColumnName(@"TransportCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Volume).HasColumnName(@"Volume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ConvertedWeight).HasColumnName(@"ConvertedWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.ActualWeight).HasColumnName(@"ActualWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.OrderCodes).HasColumnName(@"OrderCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodes).HasColumnName(@"PackageCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Customers).HasColumnName(@"Customers").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.OrderCodesUnsigned).HasColumnName(@"OrderCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodesUnsigned).HasColumnName(@"PackageCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.CustomersUnsigned).HasColumnName(@"CustomersUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
