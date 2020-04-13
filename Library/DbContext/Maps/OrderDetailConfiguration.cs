using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // OrderDetail

    public partial class OrderDetailConfiguration : EntityTypeConfiguration<OrderDetail>
    {
        public OrderDetailConfiguration()
            : this("dbo")
        {
        }

        public OrderDetailConfiguration(string schema)
        {
            ToTable("OrderDetail", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.OrderId).HasColumnName(@"OrderId").IsRequired().HasColumnType("int");
            Property(x => x.Name).HasColumnName(@"Name").IsRequired().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Image).HasColumnName(@"Image").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Quantity).HasColumnName(@"Quantity").IsRequired().HasColumnType("int");
            Property(x => x.BeginAmount).HasColumnName(@"BeginAmount").IsOptional().HasColumnType("int");
            Property(x => x.Price).HasColumnName(@"Price").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.AuditPrice).HasColumnName(@"AuditPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ExchangePrice).HasColumnName(@"ExchangePrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.Link).HasColumnName(@"Link").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.QuantityBooked).HasColumnName(@"QuantityBooked").IsOptional().HasColumnType("int");
            Property(x => x.QuantityRecived).HasColumnName(@"QuantityRecived").IsOptional().HasColumnType("int");
            Property(x => x.QuantityIncorrect).HasColumnName(@"QuantityIncorrect").IsOptional().HasColumnType("int");
            Property(x => x.QuantityActuallyReceived).HasColumnName(@"QuantityActuallyReceived").IsOptional().HasColumnType("int");
            Property(x => x.Properties).HasColumnName(@"Properties").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.CategoryId).HasColumnName(@"CategoryId").IsOptional().HasColumnType("int");
            Property(x => x.CategoryName).HasColumnName(@"CategoryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.UniqueCode).HasColumnName(@"UniqueCode").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.Size).HasColumnName(@"Size").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.Color).HasColumnName(@"Color").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.UserNote).HasColumnName(@"UserNote").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.CountingTime).HasColumnName(@"CountingTime").IsOptional().HasColumnType("datetime");
            Property(x => x.CountingUserId).HasColumnName(@"CountingUserId").IsOptional().HasColumnType("int");
            Property(x => x.CountingUserName).HasColumnName(@"CountingUserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.CountingFullName).HasColumnName(@"CountingFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CountingOfficeId).HasColumnName(@"CountingOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CountingOfficeName).HasColumnName(@"CountingOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CountingOfficeIdPath).HasColumnName(@"CountingOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.Min).HasColumnName(@"Min").IsRequired().HasColumnType("int");
            Property(x => x.Max).HasColumnName(@"Max").IsRequired().HasColumnType("int");
            Property(x => x.Prices).HasColumnName(@"Prices").IsOptional().HasColumnType("nvarchar").HasMaxLength(600);
            Property(x => x.ProId).HasColumnName(@"ProId").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.SkullId).HasColumnName(@"SkullId").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.WebsiteName).HasColumnName(@"WebsiteName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShopId).HasColumnName(@"ShopId").IsOptional().HasColumnType("int");
            Property(x => x.ShopName).HasColumnName(@"ShopName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ShopLink).HasColumnName(@"ShopLink").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.IsView).HasColumnName(@"IsView").IsOptional().HasColumnType("bit");
            Property(x => x.PrivateNote).HasColumnName(@"PrivateNote").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
