using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // LogLogin
    
    public partial class LogLoginConfiguration : EntityTypeConfiguration<LogLogin>
    {
        public LogLoginConfiguration()
            : this("dbo")
        {
        }

        public LogLoginConfiguration(string schema)
        {
            ToTable("LogLogin", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.UserName).HasColumnName(@"UserName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(100);
            Property(x => x.LoginTime).HasColumnName(@"LoginTime").IsRequired().HasColumnType("datetime");
            Property(x => x.Ip).HasColumnName(@"IP").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Token).HasColumnName(@"Token").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Os).HasColumnName(@"OS").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Browser).HasColumnName(@"Browser").IsOptional().HasColumnType("nvarchar").HasMaxLength(400);
            Property(x => x.Version).HasColumnName(@"Version").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.LogoutTime).HasColumnName(@"LogoutTime").IsOptional().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
