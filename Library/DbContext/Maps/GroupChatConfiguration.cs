using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupChat
    
    public partial class GroupChatConfiguration : EntityTypeConfiguration<GroupChat>
    {
        public GroupChatConfiguration()
            : this("dbo")
        {
        }

        public GroupChatConfiguration(string schema)
        {
            ToTable("GroupChat", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.CreatorId).HasColumnName(@"CreatorId").IsRequired().HasColumnType("int");
            Property(x => x.CreatedOnDate).HasColumnName(@"CreatedOnDate").IsRequired().HasColumnType("datetime");
            Property(x => x.GroupName).HasColumnName(@"GroupName").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.CreatorFullName).HasColumnName(@"CreatorFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CreatorTitleName).HasColumnName(@"CreatorTitleName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatorOfficeName).HasColumnName(@"CreatorOfficeName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.IsSystem).HasColumnName(@"IsSystem").IsRequired().HasColumnType("bit");
            Property(x => x.UserQuantity).HasColumnName(@"UserQuantity").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
