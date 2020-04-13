using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    // Warehouse

    public class WarehouseMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Warehouse code is required")]
        [StringLength(50, ErrorMessage = "Warehouse code cannot be more than 50 characters")]
        [RegularExpression(@"^(?:[a-zA-Z0-9]|([._-])){2,50}$", ErrorMessage = "Warehouse code is not in the correct format")]
        public string Code { get; set; } // Code (length: 50)

        [Required(ErrorMessage = "Warehouse name is required")]
        [StringLength(300, ErrorMessage = "Warehouse name cannot be more than 50 characters")]
        public string Name { get; set; } // Name (length: 300)

        [Required(ErrorMessage = "Warehouse address is required")]
        [StringLength(600, ErrorMessage = "Warehouse address cannot be more than 600 characters")]
        public string Address { get; set; } // Address (length: 600)

        [Required(ErrorMessage = "Status is required")]
        public byte Status { get; set; } // Status

        [StringLength(600, ErrorMessage = "Description cannot be more than 600 characters")]
        public string Description { get; set; } // Description (length: 600)

        [Required(ErrorMessage = "Employee required to choose")]
        public int UserId { get; set; } // UserId

        [Required(ErrorMessage = "Country is required")]
        [StringLength(150, ErrorMessage = "Country cannot be more than 150 characters ")]
        public string Country { get; set; }
    }
}