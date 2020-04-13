using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // SourceServiceCustomer
    
    public partial class SourceServiceCustomerConfiguration : EntityTypeConfiguration<SourceServiceCustomer>
    {
        public SourceServiceCustomerConfiguration()
            : this("dbo")
        {
        }

        public SourceServiceCustomerConfiguration(string schema)
        {
            ToTable("SourceServiceCustomer", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.SourceServiceId).HasColumnName(@"SourceServiceId").IsRequired().HasColumnType("int");
            Property(x => x.SourceServiceName).HasColumnName(@"SourceServiceName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.StartDate).HasColumnName(@"StartDate").IsOptional().HasColumnType("datetime");
            Property(x => x.FinishDate).HasColumnName(@"FinishDate").IsOptional().HasColumnType("datetime");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsOptional().HasColumnType("datetime");
            Property(x => x.IsActive).HasColumnName(@"IsActive").IsOptional().HasColumnType("bit");
            Property(x => x.CreateId).HasColumnName(@"CreateId").IsOptional().HasColumnType("int");
            Property(x => x.CreateName).HasColumnName(@"CreateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.UpdateId).HasColumnName(@"UpdateId").IsOptional().HasColumnType("int");
            Property(x => x.UpdateName).HasColumnName(@"UpdateName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
