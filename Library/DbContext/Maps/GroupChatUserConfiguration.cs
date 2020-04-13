using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupChatUsers
    
    public partial class GroupChatUserConfiguration : EntityTypeConfiguration<GroupChatUser>
    {
        public GroupChatUserConfiguration()
            : this("dbo")
        {
        }

        public GroupChatUserConfiguration(string schema)
        {
            ToTable("GroupChatUsers", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(350);
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(350);
            Property(x => x.InvitedByUserId).HasColumnName(@"InvitedByUserId").IsRequired().HasColumnType("int");
            Property(x => x.JoinTime).HasColumnName(@"JoinTime").IsRequired().HasColumnType("datetime");
            Property(x => x.InviteStatus).HasColumnName(@"InviteStatus").IsRequired().HasColumnType("tinyint");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.NotifyUrl).HasColumnName(@"NotifyUrl").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(1000);
            Property(x => x.IsShowNotify).HasColumnName(@"IsShowNotify").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
