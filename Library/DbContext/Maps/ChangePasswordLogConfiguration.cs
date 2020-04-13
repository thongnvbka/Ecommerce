using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ChangePasswordLog
    
    public partial class ChangePasswordLogConfiguration : EntityTypeConfiguration<ChangePasswordLog>
    {
        public ChangePasswordLogConfiguration()
            : this("dbo")
        {
        }

        public ChangePasswordLogConfiguration(string schema)
        {
            ToTable("ChangePasswordLog", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Ip).HasColumnName(@"IP").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.ChangeTime).HasColumnName(@"ChangeTime").IsRequired().HasColumnType("datetime");
            Property(x => x.OldPassword).HasColumnName(@"OldPassword").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
