using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Wallet

    public partial class WalletConfiguration : EntityTypeConfiguration<Wallet>
    {
        public WalletConfiguration()
            : this("dbo")
        {
        }

        public WalletConfiguration(string schema)
        {
            ToTable("Wallet", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Width).HasColumnName(@"Width").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Length).HasColumnName(@"Length").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Height).HasColumnName(@"Height").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalWeightConverted).HasColumnName(@"TotalWeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalWeightActual).HasColumnName(@"TotalWeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalVolume).HasColumnName(@"TotalVolume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Weight).HasColumnName(@"Weight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightConverted).HasColumnName(@"WeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.WeightActual).HasColumnName(@"WeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.Volume).HasColumnName(@"Volume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalValue).HasColumnName(@"TotalValue").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.CreatedWarehouseId).HasColumnName(@"CreatedWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedWarehouseIdPath).HasColumnName(@"CreatedWarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedWarehouseName).HasColumnName(@"CreatedWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedWarehouseAddress).HasColumnName(@"CreatedWarehouseAddress").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CurrentWarehouseId).HasColumnName(@"CurrentWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.CurrentWarehouseIdPath).HasColumnName(@"CurrentWarehouseIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentWarehouseName).HasColumnName(@"CurrentWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CurrentWarehouseAddress).HasColumnName(@"CurrentWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.TargetWarehouseId).HasColumnName(@"TargetWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.TargetWarehouseIdPath).HasColumnName(@"TargetWarehouseIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TargetWarehouseName).HasColumnName(@"TargetWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TargetWarehouseAddress).HasColumnName(@"TargetWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.OrderCodes).HasColumnName(@"OrderCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodes).HasColumnName(@"PackageCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Customers).HasColumnName(@"Customers").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.OrderCodesUnsigned).HasColumnName(@"OrderCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.PackageCodesUnsigned).HasColumnName(@"PackageCodesUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.CustomersUnsigned).HasColumnName(@"CustomersUnsigned").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.PartnerId).HasColumnName(@"PartnerId").IsOptional().HasColumnType("int");
            Property(x => x.PartnerName).HasColumnName(@"PartnerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.PartnerUpdate).HasColumnName(@"PartnerUpdate").IsOptional().HasColumnType("datetime");
            Property(x => x.EntrepotId).HasColumnName(@"EntrepotId").IsOptional().HasColumnType("int");
            Property(x => x.EntrepotName).HasColumnName(@"EntrepotName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderServices).HasColumnName(@"OrderServices").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.OrderServicesJson).HasColumnName(@"OrderServicesJson").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
