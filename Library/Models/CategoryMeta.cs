using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Models
{
    public class CategoryMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessage = "The category name is not null")]
        [StringLength(300, ErrorMessage = "The category name cannot be more than 300 characters")]
        public string Name { get; set; } // Name (length: 300)

        public int? ParentId { get; set; } // ParentId

        [StringLength(300, ErrorMessage = "Tên chuyên mục cha không được quá 300 ký tự")]
        public string ParentName { get; set; } // ParentName (length: 300)

        [Required(ErrorMessage = "Trạng thái là bắt buộc phải chọn")]
        public int Status { get; set; } // Status

        [StringLength(600, ErrorMessage = "Category name cannot be more than 600 characters")]
        public string Description { get; set; } // Description (length: 600)

        [Required(ErrorMessage = "HashTag is not null")]
      
        public string HashTag { get; set; } // HashTag
    }
}
