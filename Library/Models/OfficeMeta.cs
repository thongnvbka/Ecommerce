using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class OfficeMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "Unit code is required to enter")]
        [StringLength(30, ErrorMessage = "Unit code must be shorter than 30 characters")]
        public string Code { get; set; } // Code (length: 30)

        [Required(ErrorMessage = "Unit code is required to enter")]
        [StringLength(2, ErrorMessage = "Unit code must be shorter than 2 characters")]
        public string Culture { get; set; } // Code (length: 2)

        [Required(ErrorMessage = "Unit code is required to enter")]
        [StringLength(300, ErrorMessage = "Unit code must be shorter than 300 characters")]
        public string Name { get; set; } // Name (length: 300)

        [StringLength(50, ErrorMessage = "Short name must be shorter than 50 characters")]
        public string ShortName { get; set; } // ShortName (length: 50)

        [Required(ErrorMessage = "Unit status is required to enter")]
        [Range(0, 2, ErrorMessage = "Unit state must be between 0 and 2")]
        public byte Status { get; set; } // Status

        [Required(ErrorMessage = "Types of units are required to enter")]
        [Range(0, 8, ErrorMessage = "Type of units must be between 0 and 7")]
        public byte Type { get; set; } // Status

        public int? ParentId { get; set; } // ParentId

        [StringLength(300, ErrorMessage = "Name of the father must be shorter than 300 characters")]
        public string ParentName { get; set; } // ParentName (length: 300)

        [StringLength(500, ErrorMessage = "Unit description must be 500 characters or less")]
        public string Description { get; set; } // Description (length: 500)

        [StringLength(500, ErrorMessage = "Unit addresses must be 500 characters or less")]
        public string Address { get; set; } // Description (length: 500)
    }
}
