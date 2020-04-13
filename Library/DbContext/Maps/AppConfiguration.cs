using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // App
    
    public partial class AppConfiguration : EntityTypeConfiguration<App>
    {
        public AppConfiguration()
            : this("dbo")
        {
        }

        public AppConfiguration(string schema)
        {
            ToTable("App", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("tinyint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Icon).HasColumnName(@"Icon").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.OrderNo).HasColumnName(@"OrderNo").IsRequired().HasColumnType("int");
            Property(x => x.Url).HasColumnName(@"Url").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
