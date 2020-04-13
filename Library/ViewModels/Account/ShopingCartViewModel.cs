using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.ViewModels.Account
{
    public class ShopingCartViewModel
    {
        public ShopingCartViewModel()
        {
            Checked = false;
            ShowDetail = false;
            Id = 0;
            Services = new List<Service>();
        }
        public int Id { get; set; }

        public bool Checked { get; set; }

        public bool ShowDetail { get; set; }

        [Required(ErrorMessage = "Shop name is required")]
        public string ShopName { get; set; }

        [Required(ErrorMessage = "Link shop is required")]
        public string ShopLink { get; set; }

        [Required(ErrorMessage = "Product is required")]
        public List<Product> Products { get; set; }

        [Range(0, 1, ErrorMessage = "Data type is invalid")]
        public byte ServiceType { get; set; }
        public string Note { get; set; }
        public string PrivateNote { get; set; }
        public List<Service> Services { get; set; }
        public int ProductNo { get; set; }
        public int LinkNo { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? TotalExchange { get; set; }
        public decimal? Total { get; set; }
    }

    public class Product
    {
        public Product()
        {
            Id = 0;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Note { get; set; }
        public string Link { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string WebsiteName { get; set; }
        public string ShopName { get; set; }
        public string ShopLink { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ExchangePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalExchange { get; set; }
        public decimal AuditPrice { get; set; }
        public int? BeginAmount { get; set; }
        public string SkullId { get; set; }
        public string ProId { get; set; }
        public int Max { get; set; }
        public int Min { get; set; }
        public List<Propety> Propeties { get; set; }
        public List<PriceMeta> Prices { get; set; }
    }

    public class PriceMeta
    {
        public int Begin { get; set; }
        public int? End { get; set; }
        public decimal Price { get; set; }
    }

    public class Propety
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
    }

    public class Service
    {
        public int ServiceId { get; set; } // ServiceId
        public string ServiceName { get; set; } // ServiceName (length: 300)
        public decimal ExchangeRate { get; set; } // ExchangeRate
        public string Currency { get; set; } // Currency (length: 50)
        public byte Type { get; set; } // Type
        public decimal Value { get; set; } // Value
        public decimal TotalPrice { get; set; } // TotalPrice
        public string Note { get; set; } // Note (length: 600)
        public byte Mode { get; set; } // Mode
        public bool Checked { get; set; }
    }


    public class UpdateProductViewModel
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderViewModel
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public string PrivateNote { get; set; }
        public byte ServiceType { get; set; }
    }

    public class UpdateServiceViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public bool Checked { get; set; }
    }

    public class OrderDepositViewModel
    {
        [Required(ErrorMessage = "Order Id is required")]
        public string OrderIds { get; set; }

        [Required]
        public bool IsOtherAddress { get; set; }

        [RequiredIfTrue("IsOtherAddress", ErrorMessage = "Full name is required")]
        [StringLength(150, ErrorMessage = "Name cannot be more than 150 characters")]
        public string FullName { get; set; }

        [RequiredIfTrue("IsOtherAddress", ErrorMessage = "The address is required")]
        [StringLength(600, ErrorMessage = "Address cannot be more than 600 characters")]
        public string Address { get; set; }

        [RequiredIfTrue("IsOtherAddress", ErrorMessage = "Phone number is required")]
        [StringLength(300, ErrorMessage = "Phone number cannot be more than 300 characters")]
        public string Phone { get; set; }

        //[RequiredIfTrue("IsOtherAddress", ErrorMessage = "Tỉnh/Thành phố là bắt buộc phải nhập")]
        public int? ProvinceId { get; set; }

        //[RequiredIfTrue("IsOtherAddress", ErrorMessage = "Quận/Huyện là bắt buộc phải nhập")]
        public int? DistrictId { get; set; }

        //[RequiredIfTrue("IsOtherAddress", ErrorMessage = "Xã/Phường là bắt buộc phải nhập")]
        public int? WardId { get; set; }

        [Required(ErrorMessage = "Please select a warehouse")]
        public int? WarehouseDeliveryId { get; set; }
        public string WarehouseDeliveryName { get; set; }
        public string IsLevel { get; set; }
        public string TotalLevel { get; set; }
        public string Total { get; set; }
        public string Percent { get; set; }
        public string TypeLevel { get; set; }
    }

    public class OrderDeleteMeta
    {
        public int Id { get; set; }

        public string ShopName { get; set; }
    }
}
