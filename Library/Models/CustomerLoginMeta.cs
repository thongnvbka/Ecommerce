using ResourcesLikeOrderThaiLan;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class CustomerLoginMeta
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AccoumtLogin_ErrorNullEmail")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailShort")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailFormat")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AccoumtLogin_ErrorNullPass")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "AccoumtLogin_ErrorPassMin")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}