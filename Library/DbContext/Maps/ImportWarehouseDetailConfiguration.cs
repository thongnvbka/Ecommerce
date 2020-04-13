using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ImportWarehouseDetail

    public partial class ImportWarehouseDetailConfiguration : EntityTypeConfiguration<ImportWarehouseDetail>
    {
        public ImportWarehouseDetailConfiguration()
            : this("dbo")
        {
        }

        public ImportWarehouseDetailConfiguration(string schema)
        {
            ToTable("ImportWarehouseDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ImportWarehouseId).HasColumnName(@"ImportWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.ImportWarehouseCode).HasColumnName(@"ImportWarehouseCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerUserName).HasColumnName(@"CustomerUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"packageCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.OrderPackageNo).HasColumnName(@"OrderPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsOptional().HasColumnType("int");
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.TransportCode).HasColumnName(@"TransportCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.WarehouseIdPath).HasColumnName(@"WarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
