using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // ComplainHistory

    public partial class ComplainHistoryConfiguration : EntityTypeConfiguration<ComplainHistory>
    {
        public ComplainHistoryConfiguration()
            : this("dbo")
        {
        }

        public ComplainHistoryConfiguration(string schema)
        {
            ToTable("ComplainHistory", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ComplainId).HasColumnName(@"ComplainId").IsRequired().HasColumnType("bigint");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Content).HasColumnName(@"Content").IsRequired().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.ClaimForRefundId).HasColumnName(@"ClaimForRefundId").IsOptional().HasColumnType("int");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
