using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderRefundDetail
    
    public partial class OrderRefundDetailConfiguration : EntityTypeConfiguration<OrderRefundDetail>
    {
        public OrderRefundDetailConfiguration()
            : this("dbo")
        {
        }

        public OrderRefundDetailConfiguration(string schema)
        {
            ToTable("OrderRefundDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderRefundId).HasColumnName(@"OrderRefundId").IsRequired().HasColumnType("int");
            Property(x => x.OrderDetailCountingId).HasColumnName(@"OrderDetailCountingId").IsRequired().HasColumnType("int");
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsOptional().HasColumnType("int");
            Property(x => x.Properties).HasColumnName(@"Properties").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ProductNo).HasColumnName(@"ProductNo").IsRequired().HasColumnType("int");
            Property(x => x.BeginAmount).HasColumnName(@"BeginAmount").IsOptional().HasColumnType("int");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangePrice).HasColumnName(@"ExchangePrice").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.QuantityLose).HasColumnName(@"QuantityLose").IsOptional().HasColumnType("int");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.TotalPriceLose).HasColumnName(@"TotalPriceLose").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchangeLose).HasColumnName(@"TotalExchangeLose").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPriceShop).HasColumnName(@"TotalPriceShop").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalExchangeShop).HasColumnName(@"TotalExchangeShop").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.TotalPriceCustomer).HasColumnName(@"TotalPriceCustomer").IsRequired().HasColumnType("decimal").HasPrecision(18,4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
