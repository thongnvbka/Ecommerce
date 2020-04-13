using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Message_User
    
    public partial class MessageUserConfiguration : EntityTypeConfiguration<MessageUser>
    {
        public MessageUserConfiguration()
            : this("dbo")
        {
        }

        public MessageUserConfiguration(string schema)
        {
            ToTable("Message_User", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.MessageId).HasColumnName(@"MessageId").IsRequired().HasColumnType("bigint");
            Property(x => x.UserId).HasColumnName(@"UserId").IsRequired().HasColumnType("int");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("bit");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.IsTrash).HasColumnName(@"IsTrash").IsRequired().HasColumnType("bit");
            Property(x => x.Star).HasColumnName(@"Star").IsRequired().HasColumnType("bit");
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsRequired().HasColumnType("bit");
            Property(x => x.ReadTime).HasColumnName(@"ReadTime").IsOptional().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
