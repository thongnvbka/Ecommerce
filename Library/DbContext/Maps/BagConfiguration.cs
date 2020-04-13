using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Bag
    
    public partial class BagConfiguration : EntityTypeConfiguration<Bag>
    {
        public BagConfiguration()
            : this("dbo")
        {
        }

        public BagConfiguration(string schema)
        {
            ToTable("Bag", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OdlWarehouseId).HasColumnName(@"OdlWarehouseId").IsOptional().HasColumnType("int");
            Property(x => x.OldWarehouseName).HasColumnName(@"OldWarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
