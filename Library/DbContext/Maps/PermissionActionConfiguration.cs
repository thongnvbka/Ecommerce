using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // PermissionAction
    
    public partial class PermissionActionConfiguration : EntityTypeConfiguration<PermissionAction>
    {
        public PermissionActionConfiguration()
            : this("dbo")
        {
        }

        public PermissionActionConfiguration(string schema)
        {
            ToTable("PermissionAction", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.AppId).HasColumnName(@"AppId").IsRequired().HasColumnType("tinyint");
            Property(x => x.ModuleId).HasColumnName(@"ModuleId").IsRequired().HasColumnType("smallint");
            Property(x => x.PageId).HasColumnName(@"PageId").IsRequired().HasColumnType("smallint");
            Property(x => x.GroupPermisionId).HasColumnName(@"GroupPermisionId").IsOptional().HasColumnType("smallint");
            Property(x => x.GroupPermisionName).HasColumnName(@"GroupPermisionName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.RoleActionId).HasColumnName(@"RoleActionId").IsRequired().HasColumnType("tinyint");
            Property(x => x.AppName).HasColumnName(@"AppName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ModuleName).HasColumnName(@"ModuleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.PageName).HasColumnName(@"PageName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.RoleName).HasColumnName(@"RoleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.Checked).HasColumnName(@"Checked").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
