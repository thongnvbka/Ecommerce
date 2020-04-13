using System.ComponentModel.DataAnnotations;
using ResourcesLikeOrderThaiLan;

namespace Library.Models
{
    public class CustomerForgotMeta
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_EmailObligatory")]  
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailShort")] 
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailFormat")]  
        public string Email { get; set; }
    }
}
