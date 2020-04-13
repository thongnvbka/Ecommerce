using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Library.DbContext.Entities;

namespace Library.DbContext.Maps
{
    // Order

    public partial class OrderConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderConfiguration()
            : this("dbo")
        {
        }

        public OrderConfiguration(string schema)
        {
            ToTable("Order", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").IsRequired().HasColumnType("int").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(x => x.Code).HasColumnName(@"Code").IsRequired().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.Type).HasColumnName(@"Type").IsRequired().HasColumnType("tinyint");
            Property(x => x.WebsiteName).HasColumnName(@"WebsiteName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.ShopId).HasColumnName(@"ShopId").IsOptional().HasColumnType("int");
            Property(x => x.ShopName).HasColumnName(@"ShopName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ShopLink).HasColumnName(@"ShopLink").IsRequired().HasColumnType("nvarchar(max)");
            Property(x => x.ProductNo).HasColumnName(@"ProductNo").IsRequired().HasColumnType("int");
            Property(x => x.PackageNo).HasColumnName(@"PackageNo").IsRequired().HasColumnType("int");
            Property(x => x.PackageNoDelivered).HasColumnName(@"PackageNoDelivered").IsRequired().HasColumnType("int");
            Property(x => x.ContractCode).HasColumnName(@"ContractCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ContractCodes).HasColumnName(@"ContractCodes").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.LevelId).HasColumnName(@"LevelId").IsRequired().HasColumnType("tinyint");
            Property(x => x.LevelName).HasColumnName(@"LevelName").IsRequired().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.TotalWeight).HasColumnName(@"TotalWeight").IsRequired().HasColumnType("decimal").HasPrecision(18, 2);
            Property(x => x.DiscountType).HasColumnName(@"DiscountType").IsRequired().HasColumnType("tinyint");
            Property(x => x.DiscountValue).HasColumnName(@"DiscountValue").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.GiftCode).HasColumnName(@"GiftCode").IsOptional().HasColumnType("nvarchar").HasMaxLength(30);
            Property(x => x.CreatedTool).HasColumnName(@"CreatedTool").IsRequired().HasColumnType("tinyint");
            Property(x => x.Currency).HasColumnName(@"Currency").IsRequired().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ExchangeRate).HasColumnName(@"ExchangeRate").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalExchange).HasColumnName(@"TotalExchange").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPrice).HasColumnName(@"TotalPrice").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.DepositPercent).HasColumnName(@"DepositPercent").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalPayed).HasColumnName(@"TotalPayed").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalRefunded).HasColumnName(@"TotalRefunded").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Debt).HasColumnName(@"Debt").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.Total).HasColumnName(@"Total").IsRequired().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.HashTag).HasColumnName(@"HashTag").IsOptional().HasColumnType("nvarchar(max)");
            Property(x => x.WarehouseId).HasColumnName(@"WarehouseId").IsRequired().HasColumnType("int");
            Property(x => x.WarehouseName).HasColumnName(@"WarehouseName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.CustomerId).HasColumnName(@"CustomerId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerName).HasColumnName(@"CustomerName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerEmail).HasColumnName(@"CustomerEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerPhone).HasColumnName(@"CustomerPhone").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerAddress).HasColumnName(@"CustomerAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.Status).HasColumnName(@"Status").IsRequired().HasColumnType("tinyint");
            Property(x => x.UserId).HasColumnName(@"UserId").IsOptional().HasColumnType("int");
            Property(x => x.UserName).HasColumnName(@"UserName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.UserFullName).HasColumnName(@"UserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.OfficeId).HasColumnName(@"OfficeId").IsOptional().HasColumnType("int");
            Property(x => x.OfficeName).HasColumnName(@"OfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OfficeIdPath).HasColumnName(@"OfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedOfficeIdPath).HasColumnName(@"CreatedOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CreatedUserId).HasColumnName(@"CreatedUserId").IsOptional().HasColumnType("int");
            Property(x => x.CreatedUserFullName).HasColumnName(@"CreatedUserFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CreatedOfficeId).HasColumnName(@"CreatedOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CreatedOfficeName).HasColumnName(@"CreatedOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.OrderInfoId).HasColumnName(@"OrderInfoId").IsRequired().HasColumnType("int");
            Property(x => x.FromAddressId).HasColumnName(@"FromAddressId").IsRequired().HasColumnType("int");
            Property(x => x.ToAddressId).HasColumnName(@"ToAddressId").IsRequired().HasColumnType("int");
            Property(x => x.SystemId).HasColumnName(@"SystemId").IsRequired().HasColumnType("int");
            Property(x => x.SystemName).HasColumnName(@"SystemName").IsRequired().HasColumnType("nvarchar").HasMaxLength(100);
            Property(x => x.ServiceType).HasColumnName(@"ServiceType").IsRequired().HasColumnType("tinyint");
            Property(x => x.Note).HasColumnName(@"Note").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.PrivateNote).HasColumnName(@"PrivateNote").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.LinkNo).HasColumnName(@"LinkNo").IsRequired().HasColumnType("int");
            Property(x => x.IsDelete).HasColumnName(@"IsDelete").IsRequired().HasColumnType("bit");
            Property(x => x.Created).HasColumnName(@"Created").IsRequired().HasColumnType("datetime");
            Property(x => x.LastUpdate).HasColumnName(@"LastUpdate").IsRequired().HasColumnType("datetime");
            Property(x => x.TotalPurchase).HasColumnName(@"TotalPurchase").HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.TotalAdvance).HasColumnName(@"TotalAdvance").IsOptional().HasColumnType("money").HasPrecision(19, 4);
            Property(x => x.ReasonCancel).HasColumnName(@"ReasonCancel").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.PriceBargain).HasColumnName(@"PriceBargain").IsOptional().HasColumnType("money").HasPrecision(18, 4);
            Property(x => x.PaidShop).HasColumnName(@"PaidShop").IsOptional().HasColumnType("money").HasPrecision(18, 4);
            Property(x => x.FeeShip).HasColumnName(@"FeeShip").IsOptional().HasColumnType("money").HasPrecision(18, 4);
            Property(x => x.FeeShipBargain).HasColumnName(@"FeeShipBargain").IsOptional().HasColumnType("money").HasPrecision(18, 4);
            Property(x => x.IsPayWarehouseShip).HasColumnName(@"IsPayWarehouseShip").IsRequired().HasColumnType("bit");
            Property(x => x.UserNote).HasColumnName(@"UserNote").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.PackageNoInStock).HasColumnName(@"PackageNoInStock").IsOptional().HasColumnType("int");
            Property(x => x.UnsignName).HasColumnName(@"UnsignName").IsOptional().HasColumnType("nvarchar").HasMaxLength(2000);
            Property(x => x.PacketNumber).HasColumnName(@"PacketNumber").IsOptional().HasColumnType("int");
            Property(x => x.Description).HasColumnName(@"Description").IsOptional().HasColumnType("nvarchar").HasMaxLength(1000);
            Property(x => x.ProvisionalMoney).HasColumnName(@"ProvisionalMoney").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.DepositType).HasColumnName(@"DepositType").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseDeliveryId).HasColumnName(@"WarehouseDeliveryId").IsOptional().HasColumnType("int");
            Property(x => x.WarehouseDeliveryName).HasColumnName(@"WarehouseDeliveryName").IsOptional().HasColumnType("nvarchar").HasMaxLength(500);
            Property(x => x.ApprovelUnit).HasColumnName(@"ApprovelUnit").IsOptional().HasColumnType("nvarchar").HasMaxLength(50);
            Property(x => x.ApprovelPrice).HasColumnName(@"ApprovelPrice").IsOptional().HasColumnType("decimal").HasPrecision(18, 4);
            Property(x => x.ContactName).HasColumnName(@"ContactName").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ContactPhone).HasColumnName(@"ContactPhone").IsOptional().IsUnicode(false).HasColumnType("varchar").HasMaxLength(300);
            Property(x => x.ContactAddress).HasColumnName(@"ContactAddress").IsOptional().HasColumnType("nvarchar").HasMaxLength(255);
            Property(x => x.ContactEmail).HasColumnName(@"ContactEmail").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerCareUserId).HasColumnName(@"CustomerCareUserId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerCareName).HasColumnName(@"CustomerCareName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CustomerCareFullName).HasColumnName(@"CustomerCareFullName").IsOptional().HasColumnType("nvarchar").HasMaxLength(150);
            Property(x => x.CustomerCareOfficeId).HasColumnName(@"CustomerCareOfficeId").IsOptional().HasColumnType("int");
            Property(x => x.CustomerCareOfficeName).HasColumnName(@"CustomerCareOfficeName").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.CustomerCareOfficeIdPath).HasColumnName(@"CustomerCareOfficeIdPath").IsOptional().HasColumnType("nvarchar").HasMaxLength(300);
            Property(x => x.BargainType).HasColumnName(@"BargainType").IsOptional().HasColumnType("tinyint");
            Property(x => x.LastDeliveryTime).HasColumnName(@"LastDeliveryTime").IsOptional().HasColumnType("datetime");
            Property(x => x.IsRetail).IsRequired().HasColumnType("bit");

            InitializePartial();
        }
        partial void InitializePartial();
    }

}
