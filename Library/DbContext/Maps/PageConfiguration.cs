using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Page
    
    public partial class PageConfiguration : EntityTypeConfiguration<Page>
    {
        public PageConfiguration()
            : this("dbo")
        {
        }

        public PageConfiguration(string schema)
        {
            ToTable("Page", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("smallint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ModuleId).HasColumnName(@"ModuleId").IsRequired().HasColumnType("smallint");
            Property(x => x.ModuleName).HasColumnName(@"ModuleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AppId).HasColumnName(@"AppId").IsRequired().HasColumnType("tinyint");
            Property(x => x.ShowInMenu).HasColumnName(@"ShowInMenu").IsRequired().HasColumnType("bit");
            Property(x => x.AppName).HasColumnName(@"AppName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.OrderNo).HasColumnName(@"OrderNo").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Url).HasColumnName(@"Url").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(300);
            Property(x => x.Icon).HasColumnName(@"Icon").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
