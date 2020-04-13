using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderContractCode
    
    public partial class OrderContractCodeConfiguration : EntityTypeConfiguration<OrderContractCode>
    {
        public OrderContractCodeConfiguration()
            : this("dbo")
        {
        }

        public OrderContractCodeConfiguration(string schema)
        {
            ToTable("OrderContractCode", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.ContractCode).HasColumnName(@"ContractCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CreateDate).HasColumnName(@"CreateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.UpdateDate).HasColumnName(@"UpdateDate").IsRequired().HasColumnType("datetime");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.AccountantDate).HasColumnName(@"AccountantDate").IsOptional().HasColumnType("datetime");
            Property(x => x.AccountantId).HasColumnName(@"AccountantId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantFullName).HasColumnName(@"AccountantFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.AccountantOfficeId).HasColumnName(@"AccountantOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.AccountantOfficeName).HasColumnName(@"AccountantOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.AccountantOfficeIdPath).HasColumnName(@"AccountantOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsShip).HasColumnName(@"IsShip").IsOptional().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
