using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class AccountantSubjectMeta
    {
        public int Id { get; set; } // Id (Primary key)
        public int? IDD { get; set; }
        public string SubjectNote { get; set; } // Description 
        [Required(ErrorMessage = "Accountant name is required")]
        [StringLength(100, ErrorMessage = "Accountant name cannot be more than 100 characters")]
        public string SubjectName { get; set; } // NameType (length: 300)
    }
}
