using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupChatLike
    
    public partial class GroupChatLikeConfiguration : EntityTypeConfiguration<GroupChatLike>
    {
        public GroupChatLikeConfiguration()
            : this("dbo")
        {
        }

        public GroupChatLikeConfiguration(string schema)
        {
            ToTable("GroupChatLike", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ContentId).HasColumnName(@"ContentId").IsRequired().HasColumnType("bigint");
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.UserType).HasColumnName(@"UserType").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsLike).HasColumnName(@"IsLike").IsRequired().HasColumnType("bit");
            Property(x => x.CreatedOnDate).HasColumnName(@"CreatedOnDate").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
