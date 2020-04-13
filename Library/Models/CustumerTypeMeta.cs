using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class CustumerTypeMeta
    {
        public int Id { get; set; } // Id (Primary key)
        [StringLength(600, ErrorMessage = "The description cannot be more than 600 characters")]
        public string Description { get; set; } // Description (length: 600)
        [Required(ErrorMessage = "The customer type name is required")]
        [StringLength(300, ErrorMessage = "Category name cannot be more than 300 characters")]
        public string NameType { get; set; } // NameType (length: 300)
        
        public bool IsDelete { get; set; } // IsDelete
        public byte Status { get; set; } // Status
    }
}
