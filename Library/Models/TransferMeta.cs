using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class TransferMeta
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Ghi chú
        ///</summary>
        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; } // Note (length: 500)
       
        ///<summary>
        /// Id đơn vị của nhân viên xác thực
        ///</summary>
        [Required(ErrorMessage = "Destination repository is required")]
        public int ToWarehouseId { get; set; } // ToWarehouseId

        public decimal? PriceShip { get; set; } // PriceShip

        [Required(ErrorMessage = "Package is required")]
        public List<TransferDetailMeta> TransferDetails { get; set; }
    }

    public class TransferDetailMeta
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id kiện hàng hoặc Id của bao hàng
        ///</summary>
        [Required]
        public int PackageId { get; set; } // PackageId

        [Required]
        public string PackageCode { get; set; } // PackageCode

        ///<summary>
        /// Ghi chú đơn hàng
        ///</summary>
        [StringLength(500, ErrorMessage = "Package notes must not be longer than 500 characters")]
        public string Note { get; set; } // Note (length: 500)
    }
}
