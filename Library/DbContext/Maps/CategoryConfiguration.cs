using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Category
    
    public partial class CategoryConfiguration : EntityTypeConfiguration<Category>
    {
        public CategoryConfiguration()
            : this("dbo")
        {
        }

        public CategoryConfiguration(string schema)
        {
            ToTable("Category", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsOptional().HasColumnType("int");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("int");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsRequired().HasColumnType("datetime");
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsRequired().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
