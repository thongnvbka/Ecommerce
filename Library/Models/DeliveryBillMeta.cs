using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class DeliveryBillMeta
    {
        [Required(ErrorMessage = "Package information is required")]
        public string PackageIds { get; set; }

        [Required(ErrorMessage = "Customer information is required")]
        public int CustomerId { get; set; }
       
        [Required(ErrorMessage = "Ship charge is required")]
        public decimal PriceShip { get; set; }

        public string Note { get; set; }
    }
}
