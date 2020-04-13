using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class QuickAddUserMeta
    {
        [Required(ErrorMessage = "Account is required")]
        [StringLength(90, ErrorMessage = "Account must be less than 90 characters")]
        [RegularExpression(@"^(?:[a-z0-9]|([._-])){2,50}$", ErrorMessage = "Account name is not in the correct format")]
        public string UserName { get; set; } // UserName (length: 50)

        [Required(ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Password must be less than 50 characters")]
        public string Password { get; set; } // Password (length: 50)

        [Required(ErrorMessage = "Staff name is required")]
        [StringLength(30, ErrorMessage = "Staff name must be less than 30 characters")]
        public string FullName { get; set; } // FirstName (length: 30)

        [Required(ErrorMessage = "Position is mandatory")]
        public int TitleId { get; set; } // TitleId (Primary key)

        [Required(ErrorMessage = "Unit is mandatory")]
        public int OfficeId { get; set; } // OfficeId (Primary key)

        public string TitleName { get; set; } // TitleName (length: 300)

        public string OfficeName { get; set; } // OfficeName (length: 300)

        [Required(ErrorMessage = "Position type is mandatory")]
        public byte Type { get; set; } // Type

        public short? LevelId { get; set; } // LevelId
        public string LevelName { get; set; } // LevelName (length: 300)

        public int? GroupPermisionId { get; set; } // GroupPermisionId
        public string GroupPermissionName { get; set; } // GroupPermissionName (length: 300)

        public bool IsCompany { get; set; }
        [Required(ErrorMessage = "Object type required")]
        public int? TypeId { get; set; } // TypeId
    }
}
