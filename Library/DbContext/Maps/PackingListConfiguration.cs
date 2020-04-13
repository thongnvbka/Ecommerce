using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // PackingList
    
    public partial class PackingListConfiguration : EntityTypeConfiguration<PackingList>
    {
        public PackingListConfiguration()
            : this("dbo")
        {
        }

        public PackingListConfiguration(string schema)
        {
            ToTable("PackingList", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.PackingListName).HasColumnName(@"PackingListName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TransportType).HasColumnName(@"TransportType").IsOptional().HasColumnType("tinyint");
            Property(x => x.PackageNumber).HasColumnName(@"PackageNumber").IsOptional().HasColumnType("int");
            Property(x => x.WalletNumber).HasColumnName(@"WalletNumber").IsOptional().HasColumnType("int");
            Property(x => x.ExportWarehouseId).HasColumnName(@"ExportWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.ExportWarehouseCode).HasColumnName(@"ExportWarehouseCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.ExportWarehouseName).HasColumnName(@"ExportWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ExportWarehouseAddress).HasColumnName(@"ExportWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TimeStart).HasColumnName(@"TimeStart").IsOptional().HasColumnType("datetime");
            Property(x => x.TimeEnd).HasColumnName(@"TimeEnd").IsOptional().HasColumnType("datetime");
            Property(x => x.WarehouseSourceId).HasColumnName(@"WarehouseSourceId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseSourceCode).HasColumnName(@"WarehouseSourceCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseSourceName).HasColumnName(@"WarehouseSourceName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseSourceAddress).HasColumnName(@"WarehouseSourceAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseDesId).HasColumnName(@"WarehouseDesId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseDesCode).HasColumnName(@"WarehouseDesCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseDesName).HasColumnName(@"WarehouseDesName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseDesAddress).HasColumnName(@"WarehouseDesAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserCode).HasColumnName(@"UserCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseManagerId).HasColumnName(@"WarehouseManagerId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseManagerCode).HasColumnName(@"WarehouseManagerCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseManagerFullName).HasColumnName(@"WarehouseManagerFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAccountantId).HasColumnName(@"WarehouseAccountantId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseAccountantCode).HasColumnName(@"WarehouseAccountantCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseAccountantFullName).HasColumnName(@"WarehouseAccountantFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsOptional().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsOptional().HasColumnType("datetime");
            Property(x => x.ShipperName).HasColumnName(@"ShipperName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperPhone).HasColumnName(@"ShipperPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ShipperEmail).HasColumnName(@"ShipperEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperAddress).HasColumnName(@"ShipperAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShipperLicensePlate).HasColumnName(@"ShipperLicensePlate").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
