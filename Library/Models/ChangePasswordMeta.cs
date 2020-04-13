using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class ChangePasswordMeta
    {
        [StringLength(100, ErrorMessage = "Old password")]
        [Required(ErrorMessage = "You must enter your old password")]
        [MaxLength(15, ErrorMessage = "Passwords cannot be more than 12 characters")]
        [MinLength(6, ErrorMessage = "Passwords require a minimum of 6 characters")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [StringLength(100, ErrorMessage = "New password")]
        [Required(ErrorMessage = "You must enter a new password")]
        [MaxLength(15, ErrorMessage = "New password cannot be more than 12 characters")]
        [MinLength(6, ErrorMessage = "Passwords require a minimum of 6 characters")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [StringLength(100, ErrorMessage = "Enter a new password again")]
        [Required(ErrorMessage = "You must import the new password again")]
        [EqualTo("NewPassword", ErrorMessage = "Retype the new password incorrectly")]
        [MaxLength(15, ErrorMessage = "Retype the new password incorrectly")]
        [MinLength(6, ErrorMessage = "Passwords require a minimum of 6 characters")]
        public string ReNewPassword { get; set; }
    }
}