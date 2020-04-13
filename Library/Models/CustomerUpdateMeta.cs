using ResourcesLikeOrderThaiLan;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.ComTypes;
using Common.Attributes;

namespace Library.Models
{
    public class CustomerUpdateMeta
    {
        public int Id { get; set; } // Id (Primary key)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PhoneNumberObligatory")]
        [StringLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_PhoneNumberShort")]
        public string Phone { get; set; } // Email (length: 50)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorAddressObligatory")]
        [StringLength(600, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorAddressShort")]
        public string Address { get; set; } // Address (length: 600)

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_FullNameObligatory")]
        [StringLength(90, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "Register_ErrorFullnameShort")]
        public string FullName { get; set; } // FullName (length: 90)

        public string CountryId { get; set; } // CountryId (length: 10)

        ///<summary>
        /// Id Quận/Huyện
        ///</summary>
        public int? DistrictId { get; set; } // DistrictId

        ///<summary>
        /// Tên quận/huyện
        ///</summary>
        public string DistrictName { get; set; } // DistrictName (length: 300)

        ///<summary>
        /// Id Tỉnh/Thành Phố
        ///</summary>
        public int? ProvinceId { get; set; } // ProvinceId

        ///<summary>
        /// Tên tỉnh/thành phố
        ///</summary>
        public string ProvinceName { get; set; } // ProvinceName (length: 300)

        ///<summary>
        /// Id giới tính
        ///</summary>
        public byte? GenderId { get; set; } // GenderId

        ///<summary>
        /// Tên giói tính
        ///</summary>
        public string GenderName { get; set; } // GenderName (length: 300)

        ///<summary>
        /// Ảnh đại điện
        ///</summary>
        public string Avatar { get; set; } // Avatar

        ///<summary>
        /// Tên hiển thị khách hàng
        ///</summary>
        public string Nickname { get; set; } // Nickname (length: 150)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string Email { get; set; } // Email (length: 30)

        ///<summary>
        /// Tên chủ tài khoản ngân hàng
        ///</summary>
        public string CardName { get; set; } // CardName (length: 50)

        ///<summary>
        /// Số tài khoản
        ///</summary>
        public string CardId { get; set; } // CardId (length: 20)

        ///<summary>
        /// Ngân hàng
        ///</summary>
        public string CardBank { get; set; } // CardBank (length: 255)

        ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string CardBranch { get; set; } // CardBranch (length: 255)

        public DateTime? Birthday { get; set; }

        public int? WarehouseId { get; set; } // WarehouseId
    }

    public enum SexCustomerInfor
    {
        [LocalizedDescription("Male", typeof(Resource))]
        ผู้ชาย,//nam

        [LocalizedDescription("Female", typeof(Resource))]
        ผู้หญิง //nu
    }
}