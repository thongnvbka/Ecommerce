using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupChatContent
    
    public partial class GroupChatContentConfiguration : EntityTypeConfiguration<GroupChatContent>
    {
        public GroupChatContentConfiguration()
            : this("dbo")
        {
        }

        public GroupChatContentConfiguration(string schema)
        {
            ToTable("GroupChatContent", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(250);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(250);
            Property(x => x.TitleName).HasColumnName(@"TitleName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Content).HasColumnName(@"Content").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.SentTime).HasColumnName(@"SentTime").IsRequired().HasColumnType("datetime");
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").IsRequired().HasColumnType("bit");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.AttachmentCount).HasColumnName(@"AttachmentCount").IsOptional().HasColumnType("int");
            Property(x => x.Like).HasColumnName(@"Like").IsOptional().HasColumnType("int");
            Property(x => x.Dislike).HasColumnName(@"Dislike").IsOptional().HasColumnType("int");
            Property(x => x.NumberOfReplies).HasColumnName(@"NumberOfReplies").IsOptional().HasColumnType("int");
            Property(x => x.ParentId).HasColumnName(@"ParentId").IsOptional().HasColumnType("bigint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
