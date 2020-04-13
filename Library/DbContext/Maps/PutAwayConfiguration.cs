using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // PutAway

    public partial class PutAwayConfiguration : EntityTypeConfiguration<PutAway>
    {
        public PutAwayConfiguration()
            : this("dbo")
        {
        }

        public PutAwayConfiguration(string schema)
        {
            ToTable("PutAway", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalActualWeight).HasColumnName(@"TotalActualWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.TotalConversionWeight).HasColumnName(@"TotalConversionWeight").IsOptional().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseIdPath).HasColumnName(@"WarehouseIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.UnsignedText).HasColumnName(@"UnsignedText").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
