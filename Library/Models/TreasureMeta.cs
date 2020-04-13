using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class TreasureMeta
    {
        public int Id { get; set; } // Id (Primary key)
        public int? IDD { get; set; }
        [Required(ErrorMessage = "Parent code is required to select")]
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public string IdPath { get; set; }
        public string NamePath { get; set; }
        [Required(ErrorMessage = "Fund name is required to enter")]
        [StringLength(500, ErrorMessage = "Fund name must be less than 500 characters")]
        public string Name { get; set; } // Name (length: 300)
        
        public bool IsDelete { get; set; } // IsDelete
        public bool IsIdSystem { get; set; } // IsIdSystem
        public byte Status { get; set; } // Status
        [StringLength(600, ErrorMessage = "Description must be shorter than 600 characters")]
        public string Description { get; set; } // Description (length: 600)
      

       
    }
}
