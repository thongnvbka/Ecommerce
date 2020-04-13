using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataAnnotationsExtensions;

namespace Library.Models
{
    public class PaymentRefundMeta
    {
        [DisplayName("Amount of money")]
        [Min(100, ErrorMessage = "The requested amount of money must be >= 100")]
        [Required(ErrorMessage = "Amount of money is required")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Order is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Amount of money is required enter")]
        [StringLength(500, ErrorMessage = "Note must not be longer than 500 characters")]
        public string Note { get; set; }
    }
}
