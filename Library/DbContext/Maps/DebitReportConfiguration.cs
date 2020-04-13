using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // DebitReport
    
    public partial class DebitReportConfiguration : EntityTypeConfiguration<DebitReport>
    {
        public DebitReportConfiguration()
            : this("dbo")
        {
        }

        public DebitReportConfiguration(string schema)
        {
            ToTable("DebitReport", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.PackageId).HasColumnName(@"PackageId").IsOptional().HasColumnType("int");
            Property(x => x.PackageCode).HasColumnName(@"PackageCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ServiceId).HasColumnName(@"ServiceId").IsRequired().HasColumnType("tinyint");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
