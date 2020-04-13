using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // LogAction
    
    public partial class LogActionConfiguration : EntityTypeConfiguration<LogAction>
    {
        public LogActionConfiguration()
            : this("dbo")
        {
        }

        public LogActionConfiguration(string schema)
        {
            ToTable("LogAction", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.RecordId).HasColumnName(@"RecordId").IsRequired().HasColumnType("bigint");
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FullName).HasColumnName(@"FullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.UnsignedName).HasColumnName(@"UnsignedName").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(500);
            Property(x => x.ActionTime).HasColumnName(@"ActionTime").IsRequired().HasColumnType("datetime");
            Property(x => x.Content).HasColumnName(@"Content").IsRequired().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.SessionId).HasColumnName(@"SessionId").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Ip).HasColumnName(@"Ip").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.Os).HasColumnName(@"Os").IsOptional().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.Version).HasColumnName(@"Version").IsOptional().HasColumnType("int");
            Property(x => x.TableName).HasColumnName(@"TableName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ActionName).HasColumnName(@"ActionName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OldRecord).HasColumnName(@"OldRecord").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.NewRecord).HasColumnName(@"NewRecord").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CompareRecord).HasColumnName(@"CompareRecord").IsOptional().HasColumnType("nvarchar(max)");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
