using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.DbContext.Entities
{
    // Order

    public partial class OrderCustomer
    {
        public int Id { get; set; } // Id (Primary key)

        public string Code { get; set; } // Code (length: 30)

        public byte Type { get; set; } // Type

       
        public string WebsiteName { get; set; } // WebsiteName (length: 300)

        public int? ShopId { get; set; } // ShopId

        public string ShopName { get; set; } // ShopName (length: 500)

      
        public string ShopLink { get; set; } // ShopLink

       
        public int ProductNo { get; set; } // ProductNo

        
        public int PackageNo { get; set; } // PackageNo
        public int PackageNoDelivered { get; set; } // PackageNo

        
        public string ContractCode { get; set; } // ContractCode (length: 50)

       
        public string ContractCodes { get; set; } // ContractCodes (length: 300)

       
        public byte LevelId { get; set; } // LevelId

       
        public string LevelName { get; set; } // LevelName (length: 300)

       
        public decimal TotalWeight { get; set; } // TotalWeight

        
        public byte DiscountType { get; set; } // DiscountType

        
        public decimal? DiscountValue { get; set; } // DiscountValue

       
        public string GiftCode { get; set; } // GiftCode (length: 30)

      
        public byte CreatedTool { get; set; } // CreatedTool

      
        public string Currency { get; set; } // Currency (length: 50)

      
        public decimal ExchangeRate { get; set; } // ExchangeRate

      
        public decimal TotalExchange { get; set; } // TotalExchange

        
        public decimal TotalPrice { get; set; } // TotalPrice

      
        public decimal DepositPercent { get; set; } // TotalPrice

        public decimal TotalPayed { get; set; } // Total

      
        public decimal Total { get; set; } // Total

      
        public string HashTag { get; set; } // HashTag

       
        public int WarehouseId { get; set; } // WarehouseId

        public string WarehouseName { get; set; } // WarehouseName (length: 500)

      
        public int? CustomerId { get; set; } // CustomerId

    
        public string CustomerName { get; set; } // CustomerName (length: 300)

      
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

      
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        public byte Status { get; set; } // Status

     
        public int? UserId { get; set; } // UserId

        public string UserName { get; set; } // UserName (length: 150)

      
        public string UserFullName { get; set; } // UserFullName (length: 150)

      
        public int? OfficeId { get; set; } // OfficeId

      
        public string OfficeName { get; set; } // OfficeName (length: 300)

      
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        
        public string CreatedOfficeIdPath { get; set; } // CreatedOfficeIdPath (length: 300)

       
        public int? CreatedUserId { get; set; } // CreatedUserId

       
        public string CreatedUserFullName { get; set; } // CreatedUserFullName (length: 150)

       
        public int? CreatedOfficeId { get; set; } // CreatedOfficeId

      
        public string CreatedOfficeName { get; set; } // CreatedOfficeName (length: 300)

    
        public int OrderInfoId { get; set; } // OrderInfoId

      
        public int FromAddressId { get; set; } // FromAddressId

      
        public int ToAddressId { get; set; } // ToAddressId

        
        public int SystemId { get; set; } // SystemId

        public string SystemName { get; set; } // SystemName (length: 100)

        public byte ServiceType { get; set; } // ServiceType

        
        public string Note { get; set; } // Note (length: 500)

        public string PrivateNote { get; set; } // PrivateNote (length: 500)

       
        public int LinkNo { get; set; } // LinkNo
        public bool IsDelete { get; set; } // IsDelete

     
        public DateTime Created { get; set; } // Created

      
        public DateTime LastUpdate { get; set; } // LastUpdate
        public DateTime? ExpectedDate { get; set; } // ExpectedDate
      
        public decimal? TotalPurchase { get; set; } // TotalPurchase
       
        public decimal? TotalAdvance { get; set; } // TotalAdvance
     
        public string ReasonCancel { get; set; }
      
        public decimal? PriceBargain { get; set; }
      
        public decimal? PaidShop { get; set; }
      
        public decimal? FeeShip { get; set; }
      
        public decimal? FeeShipBargain { get; set; }
       
        public bool IsPayWarehouseShip { get; set; }
    
        public string UserNote { get; set; } // UserNote (length: 500)

     
        public int? PackageNoInStock { get; set; } // PackageNoInStock

        public string UnsignName { get; set; } // UnsignName (length: 500)
   
        public int? PacketNumber { get; set; } // PacketNumber

      
        public string Description { get; set; } // Description (length: 1000)
      
        public decimal? ProvisionalMoney { get; set; } // ProvisionalMoney

      
        public int? DepositType { get; set; } // DepositType

        public int? WarehouseDeliveryId { get; set; } // WarehouseDeliveryId

     
        public string WarehouseDeliveryName { get; set; } // WarehouseDeliveryName (length: 500)

       
        public string ApprovelUnit { get; set; } // ApprovelUnit (length: 50)

     
        public decimal? ApprovelPrice { get; set; } // ApprovelPrice

     
        public string ContactName { get; set; } // ContactName (length: 100)

        public string ContactPhone { get; set; } // ContactPhone (length: 20)

       
        public string ContactAddress { get; set; } // ContactAddress (length: 255)

       
        public string ContactEmail { get; set; } // ContactEmail (length: 300)

      
        public int? CustomerCareUserId { get; set; } // CustomerCareUserId

      
        public string CustomerCareName { get; set; } // CustomerCareName (length: 150)

       
        public string CustomerCareFullName { get; set; } // CustomerCareFullName (length: 150)

        public int? CustomerCareOfficeId { get; set; } // CustomerCareOfficeId

     
        public string CustomerCareOfficeName { get; set; } // CustomerCareOfficeName (length: 300)

        public string CustomerCareOfficeIdPath { get; set; } // CustomerCareOfficeIdPath (length: 300)

        
        public byte? BargainType { get; set; }

        [NotMapped]
        public int? Chat { get; set; }
        [NotMapped]
        public string ChatContent { get; set; }

        public DateTime? LastDeliveryTime { get; set; }


        public int? CustomerUserId { get; set; } // CustomerCareUserId


        public string CustomerUserName { get; set; } // CustomerCareName (length: 150)

        /// <summary>
        /// Tiền phí dịch vụ mua hàng
        /// </summary>
        public decimal? ServiceOrder { get; set; }

        partial void InitializePartial();
    }

}

