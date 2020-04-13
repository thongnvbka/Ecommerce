using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourcesLikeOrderThaiLan;

namespace Library.Models
{
    public class CustomerValidateMeta
    {

        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        ///
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_EmailObligatory")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailShort")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorEmailFormat")]
        public string Email { get; set; } // Email (length: 30)

      
        ///<summary>
        /// Tên đầy đủ khách hàng
        ///</summary>
        public string FullName { get; set; } // FullName (length: 90)

        ///<summary>
        /// Mật khẩu truy cập hệ thống
        ///</summary>

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassConfirm")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassShort50")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "PassForget_PassMin6")]
         
        public string Password { get; set; } // Password (length: 50)

        ///<summary>
        /// Tài khoản khách hàng từ hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Điện thoại khác hàng
        ///</summary>
        public string Phone { get; set; } // Phone (length: 50)

        ///<summary>
        /// Tên hiển thị khách hàng
        ///</summary>
        [Required(ErrorMessage = "Please enter your username")]
        [MinLength(6, ErrorMessage = "The minimum login name is 6 characters ")] 
        public string Nickname { get; set; } // Nickname (length: 150)

       
        ///<summary>
        /// Id cấp bậc khách hàng
        ///</summary>
        public byte LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp bậc khách hàng: Vip1,Vip2,..
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 150)

        ///<summary>
        /// Điểm lên cấp khách hàng
        ///</summary>
        public int Point { get; set; } // Point
  
        ///<summary>
        /// Id Quận/Huyện
        ///</summary>
        public int DistrictId { get; set; } // DistrictId
         
        ///<summary>
        /// Chi tiết địa chỉ khách hàng
        ///</summary>
        public string Address { get; set; } // Address (length: 600)
 
        ///<summary>
        /// Số dư khả dụng
        ///</summary>
        public decimal BalanceAvalible { get; set; } // BalanceAvalible
         

        ///<summary>
        /// Thời gian kích hoạt
        ///</summary>
        public DateTime? DateActive { get; set; } // DateActive
        public string CountryId { get; set; } // CountryId (length: 10)
         
    }
}

