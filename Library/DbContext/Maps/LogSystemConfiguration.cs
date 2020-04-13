using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // LogSystem
    
    public partial class LogSystemConfiguration : EntityTypeConfiguration<LogSystem>
    {
        public LogSystemConfiguration()
            : this("dbo")
        {
        }

        public LogSystemConfiguration(string schema)
        {
            ToTable("LogSystem", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.LogType).HasColumnName(@"LogType").IsRequired().HasColumnType("tinyint");
            Property(x => x.ShortMessage).HasColumnName(@"ShortMessage").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.FullMessage).HasColumnName(@"FullMessage").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.SesstionId).HasColumnName(@"SesstionId").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.Ip).HasColumnName(@"Ip").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(20);
            Property(x => x.Os).HasColumnName(@"Os").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.Broswser).HasColumnName(@"Broswser").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Version).HasColumnName(@"Version").IsOptional().HasColumnType("int");
            Property(x => x.RequestJson).HasColumnName(@"RequestJson").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.ObjectJson).HasColumnName(@"ObjectJson").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.CreatedTime).HasColumnName(@"CreatedTime").IsRequired().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
