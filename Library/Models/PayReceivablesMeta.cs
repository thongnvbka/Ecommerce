using System.ComponentModel.DataAnnotations;
namespace Library.Models
{
    public class PayReceivablesMeta
    {
        public int Id { get; set; } // Id (Primary key)
        [Required(ErrorMessage = "Parent code is required to select")]
        public int ParentId { get; set; }

        public int? IDD { get; set; }
        public string ParentName { get; set; }
        public string IdPath { get; set; }
        public string NamePath { get; set; }
        [Required(ErrorMessage = "E-wallet name is required")]
        [StringLength(500, ErrorMessage = "The name of the electronic wallet must be less than 500 characters ")]
        public string Name { get; set; } // Name (length: 300)

        public bool IsDelete { get; set; } // IsDelete
        public bool IsIdSystem { get; set; } // IsIdSystem
        public byte Status { get; set; } // Status
        [StringLength(600, ErrorMessage = "Description must be shorter than 600 characters")]
        public string Description { get; set; } // Description (length: 600)
    }
}
