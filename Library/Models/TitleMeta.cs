using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class TitleMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Position code is required")]
        [StringLength(50, ErrorMessage = "Position code can not be more than 50 characters")]
        public string Code { get; set; } // Code (length: 50)

        [Required(ErrorMessage = "The title is required")]
        [StringLength(300, ErrorMessage = "The title can not more than 300 characters")]
        public string Name { get; set; } // Name (length: 300)

        [StringLength(50, ErrorMessage = "Short names can not more than 50 characters")]
        public string ShortName { get; set; } // ShortName (length: 50)

        [Required(ErrorMessage = "Position status is required")]
        [Range(0, 2, ErrorMessage = "The state must be between 0 and 1")]
        public byte Status { get; set; } // Status

        [StringLength(500, ErrorMessage = "Description must not more than 500 characters")]
        public string Description { get; set; } // Description (length: 500)
    }
}
