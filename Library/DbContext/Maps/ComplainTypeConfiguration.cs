using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ComplainType

    public partial class ComplainTypeConfiguration : EntityTypeConfiguration<ComplainType>
    {
        public ComplainTypeConfiguration()
            : this("dbo")
        {
        }

        public ComplainTypeConfiguration(string schema)
        {
            ToTable("ComplainType", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.IdPath).HasColumnName(@"IdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.NamePath).HasColumnName(@"NamePath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsRequired().HasColumnType("int");
            Property(x => x.ParentName).HasColumnName(@"ParentName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.IsParent).HasColumnName(@"IsParent").IsRequired().HasColumnType("bit");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Index).HasColumnName(@"Index").IsOptional().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
