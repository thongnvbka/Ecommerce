using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderDetailCounting

    public partial class OrderDetailCountingConfiguration : EntityTypeConfiguration<OrderDetailCounting>
    {
        public OrderDetailCountingConfiguration()
            : this("dbo")
        {
        }

        public OrderDetailCountingConfiguration(string schema)
        {
            ToTable("OrderDetailCounting", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.OrderCode).HasColumnName(@"OrderCode").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(50);
            Property(x => x.OrderType).HasColumnName(@"OrderType").IsRequired().HasColumnType("tinyint");
            Property(x => x.WebsiteName).HasColumnName(@"WebsiteName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShopId).HasColumnName(@"ShopId").IsOptional().HasColumnType("int");
            Property(x => x.ShopName).HasColumnName(@"ShopName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ShopLink).HasColumnName(@"ShopLink").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.OrderDetailId).HasColumnName(@"OrderDetailId").IsRequired().HasColumnType("int");
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsOptional().HasColumnType("int");
            Property(x => x.Properties).HasColumnName(@"Properties").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ProductNo).HasColumnName(@"ProductNo").IsRequired().HasColumnType("int");
            Property(x => x.BeginAmount).HasColumnName(@"BeginAmount").IsOptional().HasColumnType("int");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ExchangePrice).HasColumnName(@"ExchangePrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.QuantityLose).HasColumnName(@"QuantityLose").IsOptional().HasColumnType("int");
            Property(x => x.Mode).HasColumnName(@"Mode").IsRequired().HasColumnType("tinyint");
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.NotePrivate).HasColumnName(@"NotePrivate").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.Updated).HasColumnName(@"Updated").IsRequired().HasColumnType("datetime");
            Property(x => x.TotalPriceLose).HasColumnName(@"TotalPriceLose").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalExchangeLose).HasColumnName(@"TotalExchangeLose").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPriceShop).HasColumnName(@"TotalPriceShop").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalExchangeShop).HasColumnName(@"TotalExchangeShop").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPriceCustomer).HasColumnName(@"TotalPriceCustomer").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.CommentNo).HasColumnName(@"CommentNo").IsRequired().HasColumnType("int");
            Property(x => x.ImageJson).HasColumnName(@"ImageJson").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            InitializePartial();
        }
        partial void InitializePartial();
    }
}
