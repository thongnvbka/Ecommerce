using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ChatFilesAttach
    
    public partial class ChatFilesAttachConfiguration : EntityTypeConfiguration<ChatFilesAttach>
    {
        public ChatFilesAttachConfiguration()
            : this("dbo")
        {
        }

        public ChatFilesAttachConfiguration(string schema)
        {
            ToTable("ChatFilesAttach", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.GroupId).HasColumnName(@"GroupId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.ChatId).HasColumnName(@"ChatId").IsOptional().HasColumnType("bigint");
            Property(x => x.FileName).HasColumnName(@"FileName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FileUrl).HasColumnName(@"FileUrl").IsOptional().IsUnicode(false).HasColumnType("varchar(max)");
            Property(x => x.DownloaderIds).HasColumnName(@"DownloaderIds").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.FileSize).HasColumnName(@"FileSize").IsOptional().HasColumnType("int");
            Property(x => x.FileSizeText).HasColumnName(@"FileSizeText").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(150);
            Property(x => x.ContentType).HasColumnName(@"ContentType").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.DownloadCount).HasColumnName(@"DownloadCount").IsOptional().HasColumnType("int");
            Property(x => x.IsRemove).HasColumnName(@"IsRemove").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
