using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Library.Models
{
    public class OrderAcountingMeta
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int OrderDetailId { get; set; }

        [Required]
        public byte Mode { get; set; }

        [Required(ErrorMessage = "Missing / Wrong number is required")]
        [Min(1, ErrorMessage = "Missing / wrong amount must be greater than 0")]
        public int QuantityLose { get; set; }

        [Required(ErrorMessage = "Notes are required to enter")]
        public string Note { get; set; }

        public string ImageJson { get; set; }
    }
}
