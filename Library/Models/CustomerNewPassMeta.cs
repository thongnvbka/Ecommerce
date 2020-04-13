using System.ComponentModel.DataAnnotations;
using ResourcesLikeOrderThaiLan;

namespace Library.Models
{
    public class CustomerNewPassMeta
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassConfirm")]  
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassShort50")]  
        [MinLength(6, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassMin6")]   
        [DataType(DataType.Password)]
        public string Password { get; set; } // Password (length: 50)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassConfirmNotTrue")] 
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PassConfirmNotTrue")]  
        public string ConfirmPassword { get; set; } // Password (length: 50)
        public string Email { get; set; }
    }
}
