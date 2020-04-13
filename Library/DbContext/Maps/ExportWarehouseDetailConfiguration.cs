using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ExportWarehouseDetail

    public partial class ExportWarehouseDetailConfiguration : EntityTypeConfiguration<ExportWarehouseDetail>
    {
        public ExportWarehouseDetailConfiguration()
            : this("dbo")
        {
        }

        public ExportWarehouseDetailConfiguration(string schema)
        {
            ToTable("ExportWarehouseDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ExportWarehouseId).HasColumnName(@"ExportWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.ExportWarehouseCode).HasColumnName(@"ExportWarehouseCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.PackageWeight).HasColumnName(@"PackageWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PackageWeightConverted).HasColumnName(@"PackageWeightConverted").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PackageWeightActual).HasColumnName(@"PackageWeightActual").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PackageTransportCode).HasColumnName(@"PackageTransportCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.PackageSize).HasColumnName(@"PackageSize").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.OrderWeight).HasColumnName(@"OrderWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderWeightConverted).HasColumnName(@"OrderWeightConverted").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderWeightActual).HasColumnName(@"OrderWeightActual").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderShip).HasColumnName(@"OrderShip").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderShipActual).HasColumnName(@"OrderShipActual").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OrderPackageNo).HasColumnName(@"OrderPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.OrderTotalPackageNo).HasColumnName(@"OrderTotalPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.OrderNote).HasColumnName(@"OrderNote").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerUserName).HasColumnName(@"CustomerUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerFullName).HasColumnName(@"CustomerFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerOrderNo).HasColumnName(@"CustomerOrderNo").IsRequired().HasColumnType("int");
            Property(x => x.CustomerDistance).HasColumnName(@"CustomerDistance").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CustomerWeight).HasColumnName(@"CustomerWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CustomerWeightConverted).HasColumnName(@"CustomerWeightConverted").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.CustomerWeightActual).HasColumnName(@"CustomerWeightActual").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
