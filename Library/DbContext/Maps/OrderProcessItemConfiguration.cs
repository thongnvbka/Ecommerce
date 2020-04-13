using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderProcessItem
    
    public partial class OrderProcessItemConfiguration : EntityTypeConfiguration<OrderProcessItem>
    {
        public OrderProcessItemConfiguration()
            : this("dbo")
        {
        }

        public OrderProcessItemConfiguration(string schema)
        {
            ToTable("OrderProcessItem", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderProcessId).HasColumnName(@"OrderProcessId").IsOptional().HasColumnType("int");
            Property(x => x.OrderProcessCode).HasColumnName(@"OrderProcessCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsRequired().HasColumnType("int");
            Property(x => x.OrderShopCode).HasColumnName(@"OrderShopCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.OrderShopContract).HasColumnName(@"OrderShopContract").IsOptional().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.LadingCode).HasColumnName(@"LadingCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsRequired().HasColumnType("int");
            Property(x => x.CustomerCode).HasColumnName(@"CustomerCode").IsRequired().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseDesId).HasColumnName(@"WarehouseDesId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseCode).HasColumnName(@"WarehouseCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(20);
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.WarehouseAddress).HasColumnName(@"WarehouseAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsOptional().HasColumnType("datetime");
            Property(x => x.LastUpdated).HasColumnName(@"LastUpdated").IsOptional().HasColumnType("datetime");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
