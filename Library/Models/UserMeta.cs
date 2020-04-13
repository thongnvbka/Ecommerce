using System;
using System.ComponentModel.DataAnnotations;
using Foolproof;
using ResourcesLikeOrderThaiLan;

namespace Library.Models
{
    public class UserMeta
    {
        public int Id { get; set; } // Id (Primary key)
        [Required(ErrorMessage = "Account is required")]
        [StringLength(50, ErrorMessage = "Account cannot be more than 50 characters")]
        [RegularExpression(@"^(?:[a-z0-9]|([._-])){2,50}$", ErrorMessage = "Account name is not in the correct format")]
        public string UserName { get; set; } // UserName (length: 50)

        [RequiredIf("Id", Operator.LessThanOrEqualTo, 0, ErrorMessage = "Password is required")]
        [StringLength(50, ErrorMessage = "Passwords cannot be more than 50 characters")]
        public string Password { get; set; } // Password (length: 50)

        [Required(ErrorMessage = "FirstName of employee is required")]
        [StringLength(30, ErrorMessage = "Lastname of employee cannot be more than 30 characters")]
        public string FirstName { get; set; } // FirstName (length: 30)

        [StringLength(30, ErrorMessage = "MidleName of employee cannot be more than 30 characters")]
        public string MidleName { get; set; } // MidleName (length: 30)

        [Required(ErrorMessage = "Lastname of employee is required")]
        [StringLength(30, ErrorMessage = "Lastname of employee cannot be more than 30 characters")]
        public string LastName { get; set; } // LastName (length: 30)

        [Required(ErrorMessage = "Gender of employee is required to select")]
        [Range(0, 2, ErrorMessage = "Invalid gender data")]
        public byte Gender { get; set; } // Gender

        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, ErrorMessage = "Email cannot be more than 50 characters")]
        [EmailAddress(ErrorMessage = "Email invalidate")]
        public string Email { get; set; } // Email (length: 50)

        [StringLength(500, ErrorMessage = "Description of employee cannot be more than 500 characters")]
        public string Description { get; set; } // Description (length: 500)

        [StringLength(20, ErrorMessage = "Phone number in the company is required")]
        public string Phone { get; set; } // Description (length: 20)

        [StringLength(20, ErrorMessage = "Phone number is required")]
        public string Mobile { get; set; } // Description (length: 20)

        [Required(ErrorMessage = "Status is required to select")]
        public byte Status { get; set; } // Status

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Birthday { get; set; } // Birthday

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? StartDate { get; set; } // StartDate

        [StringLength(2000, ErrorMessage = "Avatar of employee cannot be more than 2000 characters")]
        public string Avatar { get; set; } // Avatar (length: 2000)

        [Required(ErrorMessage = "Position is required to select")]
        public int TitleId { get; set; } // TitleId (Primary key)

        [Required(ErrorMessage = "Unit is required to select")]
        public int OfficeId { get; set; } // OfficeId (Primary key)

        public string TitleName { get; set; } // TitleName (length: 300)

        public string OfficeName { get; set; } // OfficeName (length: 300)

        [Required(ErrorMessage = "Type of position is required to select")]
        public byte Type { get; set; } // Type

        public short? LevelId { get; set; } // LevelId

        public string LevelName { get; set; } // LevelName (length: 300)

        public int? GroupPermisionId { get; set; } // GroupPermisionId

        public string GroupPermissionName { get; set; } // GroupPermissionName (length: 300)

        public bool IsCompany { get; set; }

        //[Required(ErrorMessage = "Kiểu đối tượng bắt buộc phải chọn")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "User_Object_Types_Required")]
        public int? TypeId { get; set; } // TypeId
    }
}
