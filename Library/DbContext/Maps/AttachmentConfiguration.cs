using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Attachment
    
    public partial class AttachmentConfiguration : EntityTypeConfiguration<Attachment>
    {
        public AttachmentConfiguration()
            : this("dbo")
        {
        }

        public AttachmentConfiguration(string schema)
        {
            ToTable("Attachment", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.AttachmentName).HasColumnName(@"AttachmentName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.AttachmentPath).HasColumnName(@"AttachmentPath").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Type).HasColumnName(@"Type").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.TypeEn).HasColumnName(@"TypeEn").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Extension).HasColumnName(@"Extension").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(10);
            Property(x => x.Size).HasColumnName(@"Size").IsRequired().HasColumnType("int");
            Property(x => x.SizeString).HasColumnName(@"SizeString").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.CreatedOnDate).HasColumnName(@"CreatedOnDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UploaderId).HasColumnName(@"UploaderId").IsOptional().HasColumnType("bigint");
            Property(x => x.UploaderFullName).HasColumnName(@"UploaderFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
