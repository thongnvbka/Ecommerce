using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // GroupChatRead
    
    public partial class GroupChatReadConfiguration : EntityTypeConfiguration<GroupChatRead>
    {
        public GroupChatReadConfiguration()
            : this("dbo")
        {
        }

        public GroupChatReadConfiguration(string schema)
        {
            ToTable("GroupChatRead", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsRequired().HasColumnType("bit");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsOptional().HasColumnType("int");
            Property(x => x.FromChatId).HasColumnName(@"FromChatId").IsOptional().HasColumnType("bigint");
            Property(x => x.UserType).HasColumnName(@"UserType").IsRequired().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
