using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // NotifyRealTime
    
    public partial class NotifyRealTimeConfiguration : EntityTypeConfiguration<NotifyRealTime>
    {
        public NotifyRealTimeConfiguration()
            : this("dbo")
        {
        }

        public NotifyRealTimeConfiguration(string schema)
        {
            ToTable("NotifyRealTime", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.FromUserId).HasColumnName(@"FromUserId").IsRequired().HasColumnType("int");
            Property(x => x.ToUserId).HasColumnName(@"ToUserId").IsRequired().HasColumnType("int");
            Property(x => x.Title).HasColumnName(@"Title").IsRequired().HasColumnType("nvarchar").HasMaxLength(160);
            Property(x => x.Avatar).HasColumnName(@"Avatar").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Content).HasColumnName(@"Content").IsOptional().HasColumnType("ntext").IsMaxLength();
            Property(x => x.SendTime).HasColumnName(@"SendTime").IsRequired().HasColumnType("datetime");
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsRequired().HasColumnType("bit");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.Url).HasColumnName(@"Url").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.Group).HasColumnName(@"Group").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
