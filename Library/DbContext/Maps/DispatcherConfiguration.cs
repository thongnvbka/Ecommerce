using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Dispatcher

    public partial class DispatcherConfiguration : EntityTypeConfiguration<Dispatcher>
    {
        public DispatcherConfiguration()
            : this("dbo")
        {
        }

        public DispatcherConfiguration(string schema)
        {
            ToTable("Dispatcher", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.FromWarehouseId).HasColumnName(@"FromWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.FromWarehouseIdPath).HasColumnName(@"FromWarehouseIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromWarehouseName).HasColumnName(@"FromWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FromWarehouseAddress).HasColumnName(@"FromWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Amount).HasColumnName(@"Amount").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalWeightActual).HasColumnName(@"TotalWeightActual").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalWeightConverted).HasColumnName(@"TotalWeightConverted").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalVolume).HasColumnName(@"TotalVolume").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPackageNo).HasColumnName(@"TotalPackageNo").IsRequired().HasColumnType("int");
            Property(x => x.WalletNo).HasColumnName(@"WalletNo").IsRequired().HasColumnType("int");
            Property(x => x.PriceType).HasColumnName(@"PriceType").IsRequired().HasColumnType("tinyint");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Value).HasColumnName(@"Value").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.ForcastDate).HasColumnName(@"ForcastDate").IsOptional().HasColumnType("datetime");
            Property(x => x.ToWarehouseId).HasColumnName(@"ToWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.ToWarehouseIdPath).HasColumnName(@"ToWarehouseIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToWarehouseName).HasColumnName(@"ToWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ToWarehouseAddress).HasColumnName(@"ToWarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.TransportPartnerId).HasColumnName(@"TransportPartnerId").IsRequired().HasColumnType("int");
            Property(x => x.TransportPartnerName).HasColumnName(@"TransportPartnerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TransportMethodId).HasColumnName(@"TransportMethodId").IsRequired().HasColumnType("int");
            Property(x => x.TransportMethodName).HasColumnName(@"TransportMethodName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ContactName).HasColumnName(@"ContactName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ContactPhone).HasColumnName(@"ContactPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.EntrepotId).HasColumnName(@"EntrepotId").IsOptional().HasColumnType("int");
            Property(x => x.EntrepotName).HasColumnName(@"EntrepotName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
