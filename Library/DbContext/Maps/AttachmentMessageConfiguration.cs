using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Attachment_Message
    
    public partial class AttachmentMessageConfiguration : EntityTypeConfiguration<AttachmentMessage>
    {
        public AttachmentMessageConfiguration()
            : this("dbo")
        {
        }

        public AttachmentMessageConfiguration(string schema)
        {
            ToTable("Attachment_Message", schema);
            HasKey(x => new { x.AttachmentId, x.MessageId });

            Property(x => x.AttachmentId).HasColumnName(@"AttachmentId").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.MessageId).HasColumnName(@"MessageId").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.IsCanEdit).HasColumnName(@"IsCanEdit").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
