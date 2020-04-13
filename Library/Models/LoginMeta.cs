using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class LoginMeta
    {
        [Required(ErrorMessage = "Login name not null")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password name not null")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
