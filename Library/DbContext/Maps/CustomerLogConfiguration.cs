using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // CustomerLog
    public partial class CustomerLogConfiguration : EntityTypeConfiguration<CustomerLog>
    {
        public CustomerLogConfiguration()
            : this("dbo")
        {
        }

        public CustomerLogConfiguration(string schema)
        {
            ToTable("CustomerLog", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.Email).HasColumnName(@"Email").IsRequired().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.FullName).HasColumnName(@"FullName").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Type).HasColumnName(@"Type").IsOptional().HasColumnType("tinyint");
            Property(x => x.DataBefore).HasColumnName(@"DataBefore").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.DataAfter).HasColumnName(@"DataAfter").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.DataType).HasColumnName(@"DataType").IsOptional().HasColumnType("tinyint");
            Property(x => x.LogContent).HasColumnName(@"LogContent").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.Ip).HasColumnName(@"IP").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Token).HasColumnName(@"Token").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.Os).HasColumnName(@"OS").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Browser).HasColumnName(@"Browser").IsOptional().HasColumnType("nvarchar").HasMaxLength(400);
            Property(x => x.Version).HasColumnName(@"Version").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
