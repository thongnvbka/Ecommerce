using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Module
    
    public partial class ModuleConfiguration : EntityTypeConfiguration<Module>
    {
        public ModuleConfiguration()
            : this("dbo")
        {
        }

        public ModuleConfiguration(string schema)
        {
            ToTable("Module", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("smallint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Icon).HasColumnName(@"Icon").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.AppId).HasColumnName(@"AppId").IsRequired().HasColumnType("tinyint");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.OrderNo).HasColumnName(@"OrderNo").IsRequired().HasColumnType("int");
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsOptional().HasColumnType("smallint");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsOptional().HasColumnType("varchar").HasMaxLength(300);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Level).HasColumnName(@"Level").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
