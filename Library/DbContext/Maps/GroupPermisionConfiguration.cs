using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupPermision
    
    public partial class GroupPermisionConfiguration : EntityTypeConfiguration<GroupPermision>
    {
        public GroupPermisionConfiguration()
            : this("dbo")
        {
        }

        public GroupPermisionConfiguration(string schema)
        {
            ToTable("GroupPermision", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("smallint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").IsRequired().HasColumnType("bit");
            Property(x => x.UserNo).HasColumnName(@"UserNo").IsRequired().HasColumnType("int");
            Property(x => x.AppNo).HasColumnName(@"AppNo").IsRequired().HasColumnType("smallint");
            Property(x => x.ModuleNo).HasColumnName(@"ModuleNo").IsRequired().HasColumnType("smallint");
            Property(x => x.PageNo).HasColumnName(@"PageNo").IsRequired().HasColumnType("smallint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDefault).HasColumnName(@"IsDefault").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
