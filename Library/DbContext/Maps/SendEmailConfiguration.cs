using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // SendEmail
    
    public partial class SendEmailConfiguration : EntityTypeConfiguration<SendEmail>
    {
        public SendEmailConfiguration()
            : this("dbo")
        {
        }

        public SendEmailConfiguration(string schema)
        {
            ToTable("SendEmail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.FromUserId).HasColumnName(@"FromUserId").IsRequired().HasColumnType("int");
            Property(x => x.Title).HasColumnName(@"Title").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.FromUserName).HasColumnName(@"FromUserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.FromUserFullName).HasColumnName(@"FromUserFullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.FromUserEmail).HasColumnName(@"FromUserEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.To).HasColumnName(@"To").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Cc).HasColumnName(@"Cc").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Bcc).HasColumnName(@"Bcc").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Content).HasColumnName(@"Content").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.CreatedOnDate).HasColumnName(@"CreatedOnDate").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.SendTime).HasColumnName(@"SendTime").IsOptional().HasColumnType("datetime");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.AttachmentCount).HasColumnName(@"AttachmentCount").IsOptional().HasColumnType("smallint");
            Property(x => x.Attachments).HasColumnName(@"Attachments").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.IsLock).HasColumnName(@"IsLock").IsRequired().HasColumnType("bit");
            Property(x => x.ErrorNo).HasColumnName(@"ErrorNo").IsRequired().HasColumnType("tinyint");
            Property(x => x.ErrorLastTime).HasColumnName(@"ErrorLastTime").IsOptional().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
