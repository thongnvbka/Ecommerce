using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.Models
{
    public class DeliveryMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Shipping note status is required")]
        public byte Status { get; set; } // Status

        [Required(ErrorMessage = "The type of transport partner is required to select")]
        public byte Type { get; set; } // Type

        //[RequiredIf("Type", Operator.EqualTo, 0, ErrorMessage = "Nhân viên vận chuyển là bắt buộc phải chọn")]
        [Required(ErrorMessage = "The transporter is required to choose")]
        public int ShipperUserId { get; set; } // ShipperUserId

        //[RequiredIf("Type", Operator.EqualTo, 1, ErrorMessage = "Đối tác vận chuyển là bắt buộc phải chọn")]
        public int? PartnerId { get; set; } // PartnerId

        [StringLength(500, ErrorMessage = "Note content can not be longer than 500 characters")]
        public string Note { get; set; } // Note

        [StringLength(10, ErrorMessage = "Car license plate can not be longer than 10 characters")]
        public string CarNumber { get; set; }

        [Required(ErrorMessage = "Spending on garage parking is compulsory to enter")]
        public decimal AmountGrage { get; set; }

        [Required(ErrorMessage = "Expenditure is required to declare")]
        public decimal AmountSpend { get; set; }

        [StringLength(500, ErrorMessage = "Note content can not be longer than 500 characters")]
        public string NoteSpend { get; set; }

        public List<DeliveryDetailMeta> Packages { get; set; }
    }

    public class DeliveryDetailMeta
    {
        public int Id { get; set; }
        public int? DeliveryId { get; set; }
        [Required]
        public int PackageId { get; set; }
        public decimal CustomerDebt { get; set; }
        public int CustomerPackageNo { get; set; }
        public string PackageCode { get; set; }
        public byte Status { get; set; }

        public string CustomerAddress { get; set; }
        public string CustomerFullName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerPhone { get; set; }
        public decimal CustomerWeight { get; set; }
        public int PackageNo { get; set; }
        public int PackageNoInDelivery { get; set; }
    }

    public class DeliveryDetailDeleteMeta
    {
        public int DeliveryId { get; set; }

        public int ObjectId { get; set; }

        /// <summary>
        /// 0: Id là của DeliveryDetail
        /// 1: Id là của đơn hàng
        /// 2: Id của khách hàng 
        /// </summary>
        public byte Mode { get; set; }
    }
}
