using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Library.Models
{
    public class ImportWarehouseMeta
    {
        public int Id { get; set; }

        [StringLength(300, ErrorMessage = "Carrier name must not longer than 300 characters")]
        public string ShipperName { get; set; }

        [StringLength(300, ErrorMessage = "The carrier's phone number must not longer than 300 characters")]
        public string ShipperPhone { get; set; }

        [StringLength(300, ErrorMessage = "The carrier's address must not longer than 300 characters")]
        public string ShipperAddress { get; set; }

        [StringLength(300, ErrorMessage = "The carrier's email must not longer than 300 characters")]
        public string ShipperEmail { get; set; }

        [StringLength(500, ErrorMessage = "Notes")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Range(0, 1, ErrorMessage = "The value range of the status is incorrect")]
        public byte Status { get; set; }

        [Required(ErrorMessage = "Package is required")]
        public List<PackageMeta> Packages { get; set; }

        public List<OrderServiceOtherMeta> OrderServiceOthers { get; set; }

        public List<string> TransportCodes { get; set; }
    }

    public class PackageMeta
    {
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public string PackageCode { get; set; }

        public int? ImportWarehouseId { get; set; }

        /// <summary>
        /// Type = 0: Kiện hàng, 1: Bao hàng
        /// </summary>
        [Required]
        public byte Type { get; set; }

        public string Note { get; set; }
    }

    public class OrderServiceOtherMeta
    {
        //[Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        public string OrderCode { get; set; } // OrderId

        [Required]
        [Min(1, ErrorMessage = "The minimum required money value is 1")]
        public decimal Value { get; set; } // Value

        [Required(ErrorMessage = "Notes are required to enter")]
        public string Note { get; set; } // Note

        [Required]
        public byte Mode { get; set; } // Mode
    }

    public class OrderServiceOtherMetaV2
    {
        [Required]
        public string OrderCode { get; set; } // OrderId

        [Required]
        public decimal Value { get; set; } // Value

        public string Note { get; set; } // Note

        [Required]
        public byte Mode { get; set; } // Mode

        [Required]
        public int ImportWarehouseId { get; set; }

        [Required]
        public string ImportWarehouseCode { get; set; } // ImportWarehouseCode
    }

    public class PackageUpdateInfoMeta
    {
        [Required]
        public int PackageId { get; set; }

        [DisplayName("Length")]
        [Required(ErrorMessage = "The length of the package is required to enter")]
        public decimal Length { get; set; }

        [DisplayName("Width")]
        [Required(ErrorMessage = "The width of the package is required to enter")]
        public decimal Width { get; set; }

        [DisplayName("Height")]
        [Required(ErrorMessage = "The height of the package is required to enter")]
        public decimal Height { get; set; }

        [DisplayName("Weight")]
        [Required(ErrorMessage = "The weight of the package is required")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Notes is required")]
        public string Note { get; set; }
    }

    public class WalletUpdateInfoMeta
    {
        [Required]
        public int WalletId { get; set; }

        [DisplayName("Length")]
        [Required(ErrorMessage = "The length of the package is required to enter")]
        public decimal Length { get; set; }

        [DisplayName("Width")]
        [Required(ErrorMessage = "The width of the package is required to enter")]
        public decimal Width { get; set; }

        [DisplayName("Height")]
        [Required(ErrorMessage = "The height of the package is required to enter")]
        public decimal Height { get; set; }

        [DisplayName("Weight")]
        [Required(ErrorMessage = "The weight of the package is required")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Notes is required")]
        public string Note { get; set; }
    }
}
