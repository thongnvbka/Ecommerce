using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderPackage

    public partial class OrderPackageConfiguration : EntityTypeConfiguration<OrderPackage>
    {
        public OrderPackageConfiguration()
            : this("dbo")
        {
        }

        public OrderPackageConfiguration(string schema)
        {
            ToTable("OrderPackage", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerUserName).HasColumnName(@"CustomerUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerLevelId).HasColumnName(@"CustomerLevelId").IsRequired().HasColumnType("tinyint");
            Property(x => x.CustomerLevelName).HasColumnName(@"CustomerLevelName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerWarehouseId).HasColumnName(@"CustomerWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerWarehouseAddress).HasColumnName(@"CustomerWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerWarehouseName).HasColumnName(@"CustomerWarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerWarehouseIdPath).HasColumnName(@"CustomerWarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TransportCode).HasColumnName(@"TransportCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightConverted).HasColumnName(@"WeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActual).HasColumnName(@"WeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActualPercent).HasColumnName(@"WeightActualPercent").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.WeightWapperPercent).HasColumnName(@"WeightWapperPercent").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.WeightWapper).HasColumnName(@"WeightWapper").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalPriceWapper).HasColumnName(@"TotalPriceWapper").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Volume).HasColumnName(@"Volume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.VolumeActual).HasColumnName(@"VolumeActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.OtherService).HasColumnName(@"OtherService").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.VolumeWapperPercent).HasColumnName(@"VolumeWapperPercent").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.VolumeWapper).HasColumnName(@"VolumeWapper").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Width).HasColumnName(@"Width").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Height).HasColumnName(@"Height").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Length).HasColumnName(@"Length").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseIdPath).HasColumnName(@"WarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.ForcastDate).HasColumnName(@"ForcastDate").IsOptional().HasColumnType("datetime");
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CurrentLayoutId).HasColumnName(@"CurrentLayoutId").IsOptional().HasColumnType("int");
            Property(x => x.CurrentLayoutName).HasColumnName(@"CurrentLayoutName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentLayoutIdPath).HasColumnName(@"CurrentLayoutIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentWarehouseId).HasColumnName(@"CurrentWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.CurrentWarehouseName).HasColumnName(@"CurrentWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentWarehouseIdPath).HasColumnName(@"CurrentWarehouseIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentWarehouseAddress).HasColumnName(@"CurrentWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.OrderCodes).HasColumnName(@"OrderCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodes).HasColumnName(@"PackageCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Customers).HasColumnName(@"Customers").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.OrderCodesUnsigned).HasColumnName(@"OrderCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodesUnsigned).HasColumnName(@"PackageCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.CustomersUnsigned).HasColumnName(@"CustomersUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.Mode).HasColumnName(@"Mode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.SameCodeStatus).HasColumnName(@"SameCodeStatus").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}