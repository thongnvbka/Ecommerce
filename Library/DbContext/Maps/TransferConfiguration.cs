using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Transfer
    
    public partial class TransferConfiguration : EntityTypeConfiguration<Transfer>
    {
        public TransferConfiguration()
            : this("dbo")
        {
        }

        public TransferConfiguration(string schema)
        {
            ToTable("Transfer", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsOptional().HasColumnType("decimal").HasPrecision(18,2);
            Property(x => x.TotalWeightConverted).HasColumnName(@"TotalWeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18,2);
            Property(x => x.TotalWeightActual).HasColumnName(@"TotalWeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18,2);
            Property(x => x.WalletNo).HasColumnName(@"WalletNo").IsRequired().HasColumnType("int");
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.FromUserId).HasColumnName(@"FromUserId").IsRequired().HasColumnType("int");
            Property(x => x.FromUserFullName).HasColumnName(@"FromUserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromUserUserName).HasColumnName(@"FromUserUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FromUserTitleId).HasColumnName(@"FromUserTitleId").IsRequired().HasColumnType("int");
            Property(x => x.FromUserTitleName).HasColumnName(@"FromUserTitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromWarehouseId).HasColumnName(@"FromWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.FromWarehouseName).HasColumnName(@"FromWarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromWarehouseIdPath).HasColumnName(@"FromWarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FromTime).HasColumnName(@"FromTime").IsRequired().HasColumnType("datetime");
            Property(x => x.ToUserId).HasColumnName(@"ToUserId").IsOptional().HasColumnType("int");
            Property(x => x.ToUserFullName).HasColumnName(@"ToUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToUserUserName).HasColumnName(@"ToUserUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ToUserTitleId).HasColumnName(@"ToUserTitleId").IsOptional().HasColumnType("int");
            Property(x => x.ToUserTitleName).HasColumnName(@"ToUserTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToWarehouseId).HasColumnName(@"ToWarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.ToWarehouseName).HasColumnName(@"ToWarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToWarehouseIdPath).HasColumnName(@"ToWarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ToTime).HasColumnName(@"ToTime").IsOptional().HasColumnType("datetime");
            Property(x => x.PriceShip).HasColumnName(@"PriceShip").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
