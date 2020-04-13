using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // NotifiCustomer

    public partial class NotifiCustomerConfiguration : EntityTypeConfiguration<NotifiCustomer>
    {
        public NotifiCustomerConfiguration()
            : this("dbo")
        {
        }

        public NotifiCustomerConfiguration(string schema)
        {
            ToTable("NotifiCustomer", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(200);
            Property(x => x.NotiCommonId).HasColumnName(@"NotiCommonId").IsOptional().HasColumnType("bigint");
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(90);
            Property(x => x.IsRead).HasColumnName(@"IsRead").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
