using ResourcesLikeOrderThaiLan;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class CustomerRegisterMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_EmailObligatory")] 
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailShort")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailFormat")]
        public string Email { get; set; } // Email (length: 50)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassConfirm")]
        [StringLength(50,ErrorMessageResourceType = typeof(Resource),ErrorMessageResourceName = "PassForget_PassShort50")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassMin6")]
        [DataType(DataType.Password)]
        public string Password { get; set; } // Password (length: 50)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PassConfirmError")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PassConfirmNotTrue")]
        public string ConfirmPassword { get; set; } // Password (length: 50)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_FullNameObligatory")]
        [StringLength(90, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorFullnameShort")]
        public string FullName { get; set; } // FullName (length: 90)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PhoneNumberObligatory")]
        [StringLength(90, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PhoneNumberShort")]
        public string Phone { get; set; } // Phone (length: 50)

      //  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorProvinceObligatory")]
        public int ProvinceId { get; set; }

        public string ProvinceName { get; set; }

      //  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorDistricObligatory")]
        public int DistrictId { get; set; }

        public string DistrictName { get; set; }

      //  [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorWardsObligatory")]
        public int WardId { get; set; }

        public string WardsName { get; set; }

      [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorAddressObligatory")]
        [StringLength(600, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorAddressShort")]
        public string Address { get; set; }

        public DateTime? Birthday { get; set; }
    }
}