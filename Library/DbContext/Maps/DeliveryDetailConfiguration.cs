using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DeliveryDetail
    
    public partial class DeliveryDetailConfiguration : EntityTypeConfiguration<DeliveryDetail>
    {
        public DeliveryDetailConfiguration()
            : this("dbo")
        {
        }

        public DeliveryDetailConfiguration(string schema)
        {
            ToTable("DeliveryDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.DeliveryId).HasColumnName(@"DeliveryId").IsRequired().HasColumnType("int");
            Property(x => x.DeliveryCode).HasColumnName(@"DeliveryCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(30);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"packageCode").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.OrderPackageNo).HasColumnName(@"OrderPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
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
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightConverted).HasColumnName(@"WeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActual).HasColumnName(@"WeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Price).HasColumnName(@"Price").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceWeight).HasColumnName(@"PriceWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PricePacking).HasColumnName(@"PricePacking").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceOther).HasColumnName(@"PriceOther").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PricePayed).HasColumnName(@"PricePayed").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceStored).HasColumnName(@"PriceStored").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceOrder).HasColumnName(@"PriceOrder").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PriceShip).HasColumnName(@"PriceShip").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Debit).HasColumnName(@"Debit").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.LayoutId).HasColumnName(@"LayoutId").IsOptional().HasColumnType("int");
            Property(x => x.LayoutName).HasColumnName(@"LayoutName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WalletId).HasColumnName(@"WalletId").IsOptional().HasColumnType("int");
            Property(x => x.WalletCode).HasColumnName(@"WalletCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ShipDiscount).HasColumnName(@"ShipDiscount").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
