using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // SendEmailResult
    
    public partial class SendEmailResultConfiguration : EntityTypeConfiguration<SendEmailResult>
    {
        public SendEmailResultConfiguration()
            : this("dbo")
        {
        }

        public SendEmailResultConfiguration(string schema)
        {
            ToTable("SendEmailResult", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SendEmailId).HasColumnName(@"Send_Email_Id").IsRequired().HasColumnType("bigint");
            Property(x => x.SendId).HasColumnName(@"SendId").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.Email).HasColumnName(@"Email").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.Message).HasColumnName(@"Message").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.ErrorCode).HasColumnName(@"ErrorCode").IsOptional().HasColumnType("int");
            Property(x => x.Time).HasColumnName(@"Time").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
