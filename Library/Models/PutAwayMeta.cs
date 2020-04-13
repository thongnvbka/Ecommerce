using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Foolproof;

namespace Library.Models
{
    public class PutAwayMeta
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public byte Status { get; set; }

        [Required(ErrorMessage = "Package is required")]
        public List<PutAwayDetailMeta> Packages { get; set; }
    }

    public class PutAwayDetailMeta
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Package Id is required")]
        public int PackageId { get; set; }

        [RequiredIf("IsLose", Operator.EqualTo, false, ErrorMessage = "Layout is required")]
        public int LayoutId { get; set; }

        [Required(ErrorMessage = "Package code is required")]
        public string PackageCode { get; set; }

        [Required(ErrorMessage = "The weight of the package is required")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "The length of the package is required")]
        public decimal Length { get; set; }

        [Required(ErrorMessage = "The width of the package is required")]
        public decimal Width { get; set; }

        [Required(ErrorMessage = "Height is required to enter")]
        public decimal Height { get; set; }

        public int? PutAwayId { get; set; }

        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; }

        public bool IsLose { get; set; } = false;
    }

    public class PutAwayMeta1
    {
        public int PutAwayId { get; set; }
        public List<PutAwayDetailMeta> Packages { get; set; }
    }
}
