using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // UserPosition
    
    public partial class UserPositionConfiguration : EntityTypeConfiguration<UserPosition>
    {
        public UserPositionConfiguration()
            : this("dbo")
        {
        }

        public UserPositionConfiguration(string schema)
        {
            ToTable("UserPosition", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.TitleId).HasColumnName(@"TitleId").IsRequired().HasColumnType("int");
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsRequired().HasColumnType("int");
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDefault).HasColumnName(@"IsDefault").IsRequired().HasColumnType("bit");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OfficeNamePath).HasColumnName(@"OfficeNamePath").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.DirectUserId).HasColumnName(@"DirectUserId").IsOptional().HasColumnType("int");
            Property(x => x.DirectFullName).HasColumnName(@"DirectFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.DirectTitleId).HasColumnName(@"DirectTitleId").IsOptional().HasColumnType("int");
            Property(x => x.DirectTitleName).HasColumnName(@"DirectTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.DirectOfficeId).HasColumnName(@"DirectOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.DirectOfficeName).HasColumnName(@"DirectOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ApprovalUserId).HasColumnName(@"ApprovalUserId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovalFullName).HasColumnName(@"ApprovalFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.ApprovalTitleId).HasColumnName(@"ApprovalTitleId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovalTitleName).HasColumnName(@"ApprovalTitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ApprovalOfficeId).HasColumnName(@"ApprovalOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.ApprovalOfficeName).HasColumnName(@"ApprovalOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsOptional().HasColumnType("smallint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.GroupPermisionId).HasColumnName(@"GroupPermisionId").IsOptional().HasColumnType("smallint");
            Property(x => x.GroupPermissionName).HasColumnName(@"GroupPermissionName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
