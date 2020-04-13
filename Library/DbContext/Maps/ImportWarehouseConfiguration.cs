using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ImportWarehouse

    public partial class ImportWarehouseConfiguration : EntityTypeConfiguration<ImportWarehouse>
    {
        public ImportWarehouseConfiguration()
            : this("dbo")
        {
        }

        public ImportWarehouseConfiguration(string schema)
        {
            ToTable("ImportWarehouse", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.PackageNumber).HasColumnName(@"PackageNumber").IsOptional().HasColumnType("int");
            Property(x => x.WalletNumber).HasColumnName(@"WalletNumber").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseIdPath).HasColumnName(@"WarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperName).HasColumnName(@"ShipperName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperPhone).HasColumnName(@"ShipperPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperAddress).HasColumnName(@"ShipperAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperEmail).HasColumnName(@"ShipperEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseManagerId).HasColumnName(@"WarehouseManagerId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseManagerCode).HasColumnName(@"WarehouseManagerCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseManagerFullName).HasColumnName(@"WarehouseManagerFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAccountantId).HasColumnName(@"WarehouseAccountantId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseAccountantCode).HasColumnName(@"WarehouseAccountantCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseAccountantFullName).HasColumnName(@"WarehouseAccountantFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
