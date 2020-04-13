using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.Models
{
    public class WalletMeta
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot be more than 500 characters")]
        public string Note { get; set; }

        [Required(ErrorMessage = "The width of the packing is required")]
        public decimal Width { get; set; }

        [Required(ErrorMessage = "The length of the packing is required")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "The height of the packing is required")]
        public decimal Height { get; set; }

        [Required(ErrorMessage = "Weight of the packing is required")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Type of the packing is required")]
        [Range(0, 1, ErrorMessage = "Incorrect envelope range ")]
        public byte Mode { get; set; }

        [RequiredIf("Mode", Operator.EqualTo, 1,ErrorMessage = "The destination is required")]
        public int? TargetWarehouseId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public byte Status { get; set; }

        [RequiredIf("Mode", Operator.EqualTo, 1, ErrorMessage = "The transshipment point of the envelope is required")]
        public int? EntrepotId { get; set; }

        public bool? IsSameWallet { get; set; }
        public bool? IsConfirm { get; set; }

        [Required(ErrorMessage = "Package is required")]
        public List<WalletDetailMeta> Packages { get; set; }

        public List<OrderServiceOtherMeta> OrderServiceOthers { get; set; }
    }

    public class WalletDetailMeta
    {
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public string PackageCode { get; set; }

        public int? WalletId { get; set; }

        public string Note { get; set; }
    }
}
