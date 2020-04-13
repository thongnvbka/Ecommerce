using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // MessageRealTime
    
    public partial class MessageRealTimeConfiguration : EntityTypeConfiguration<MessageRealTime>
    {
        public MessageRealTimeConfiguration()
            : this("dbo")
        {
        }

        public MessageRealTimeConfiguration(string schema)
        {
            ToTable("MessageRealTime", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.FromUser).HasColumnName(@"FromUser").IsRequired().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.FromUserId).HasColumnName(@"FromUserId").IsRequired().HasColumnType("bigint");
            Property(x => x.FromAvatar).HasColumnName(@"FromAvatar").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ToUser).HasColumnName(@"ToUser").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.CcToUser).HasColumnName(@"CcToUser").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.BccToUser).HasColumnName(@"BccToUser").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Title).HasColumnName(@"Title").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.UnsignTitle).HasColumnName(@"UnsignTitle").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.Content).HasColumnName(@"Content").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.AttachmentCount).HasColumnName(@"AttachmentCount").IsRequired().HasColumnType("smallint");
            Property(x => x.SendTime).HasColumnName(@"SendTime").IsOptional().HasColumnType("datetime");
            Property(x => x.LastModifiedOnDate).HasColumnName(@"LastModifiedOnDate").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
