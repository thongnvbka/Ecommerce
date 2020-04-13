using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // PackageTranport
    
    public partial class PackageTranportConfiguration : EntityTypeConfiguration<PackageTranport>
    {
        public PackageTranportConfiguration()
            : this("dbo")
        {
        }

        public PackageTranportConfiguration(string schema)
        {
            ToTable("PackageTranport", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsRequired().HasColumnType("int");
            Property(x => x.Time).HasColumnName(@"Time").IsRequired().HasColumnType("datetime");
            Property(x => x.StoreId).HasColumnName(@"StoreId").IsRequired().HasColumnType("int");
            Property(x => x.StoreName).HasColumnName(@"StoreName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Address).HasColumnName(@"Address").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Type).HasColumnName(@"Type").IsOptional().HasColumnType("tinyint");
            Property(x => x.TypeName).HasColumnName(@"TypeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
