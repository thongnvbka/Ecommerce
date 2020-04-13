using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Warehouse
    
    public partial class WarehouseConfiguration : EntityTypeConfiguration<Warehouse>
    {
        public WarehouseConfiguration()
            : this("dbo")
        {
        }

        public WarehouseConfiguration(string schema)
        {
            ToTable("Warehouse", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Address).HasColumnName(@"Address").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Country).HasColumnName(@"Country").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Phone).HasColumnName(@"Phone").HasColumnType("nvarchar").HasMaxLength(50);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
