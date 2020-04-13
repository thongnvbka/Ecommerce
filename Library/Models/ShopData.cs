using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class ShopData
    {
        [Required(ErrorMessage = "Please enter shop name")]
        [StringLength(255, ErrorMessage = "Shop name can not be more than 255 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter your website name")]
        [StringLength(300, ErrorMessage = "Website must not more than 300 characters")]
        public string Website { get; set; }
        [Required(ErrorMessage = "Please enter a URL name")]
        [StringLength(2000, ErrorMessage = "Website must not be more than 2000 characters")]
        public string Url
        {
            get; set;
        }
        [Required(ErrorMessage = "Please enter the entire system link")]
        [StringLength(2000, ErrorMessage = "Website must not be more than 2000 characters")]
        public string HashTag
        {
            get; set;
        }
        
    }
}
