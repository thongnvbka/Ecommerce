using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // RoleAction
    
    public partial class RoleActionConfiguration : EntityTypeConfiguration<RoleAction>
    {
        public RoleActionConfiguration()
            : this("dbo")
        {
        }

        public RoleActionConfiguration(string schema)
        {
            ToTable("RoleAction", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("tinyint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
