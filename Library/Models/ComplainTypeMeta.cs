using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class ComplainTypeMeta
    {
        public int Id { get; set; } // Id (Primary key)

        public string IdPath { get; set; } // IdPath (length: 300)

        public string NamePath { get; set; } // NamePath (length: 500)

        [Required(ErrorMessage = "Please enter the type of complaint name")]
        [StringLength(500, ErrorMessage = "The complaint type name cannot be more than 500 characters")]
        public string Name { get; set; } // Name (length: 500)
        [Required(ErrorMessage = "Please choose the type of complaint")]
        public int ParentId { get; set; } // ParentId
        public string ParentName { get; set; } // ParentName (length: 300)
       
        [StringLength(500, ErrorMessage = "Complaint notescannot be more than 500 characters")]
        public string Description { get; set; } // Description (length: 500)
        public int? Index { get; set; } // Index

    }
    public class ComplainLinkDetail
    {
        public int Id { get; set; } 
        public int Index { get; set; } 

        public string Note { get; set; } 

        
    }
}
