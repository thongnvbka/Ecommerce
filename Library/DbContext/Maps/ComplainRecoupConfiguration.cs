using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{

    // ComplainRecoup

    public partial class ComplainRecoupConfiguration : EntityTypeConfiguration<ComplainRecoup>
    {
        public ComplainRecoupConfiguration()
            : this("dbo")
        {
        }

        public ComplainRecoupConfiguration(string schema)
        {
            ToTable("ComplainRecoup", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("bigint").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.ComplainId).HasColumnName(@"ComplainId").IsRequired().HasColumnType("bigint");
            Property(x => x.RecoupCause).HasColumnName(@"RecoupCause").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.RecoupSolution).HasColumnName(@"RecoupSolution").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.CompanyMoney).HasColumnName(@"CompanyMoney").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.CompanyBargain).HasColumnName(@"CompanyBargain").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.EmployeeMoney).HasColumnName(@"EmployeeMoney").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ShopMoney).HasColumnName(@"ShopMoney").IsOptional().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TypeCause).HasColumnName(@"TypeCause").IsRequired().HasColumnType("tinyint");
            Property(x => x.TypeTicket).HasColumnName(@"TypeTicket").IsOptional().HasColumnType("tinyint");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
