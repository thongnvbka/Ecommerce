using Foolproof;
using System;
using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class PotentialCustomerMeta
    {
        public int Id { get; set; } // Id (Primary key)  

        ///<summary>
        /// Id đối tượng trên hệ thống
        ///</summary>
        public int? TypeId { get; set; } // TypeId

        ///<summary>
        /// Idd định danh đối tượng trên hệ thống
        ///</summary>
        public int? TypeIdd { get; set; } // TypeIdd

        ///<summary>
        /// Tên đối tượng trên hệ thống
        ///</summary>
        public string TypeName { get; set; } // TypeName (length: 100)

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email invalidate")]
        [StringLength(200, ErrorMessage = "Email can not be longer than 30 characters")]
        public string Email { get; set; } // Email (length: 30) 

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(90, ErrorMessage = "Full name can not be longer than 30 characters")]
        public string FullName { get; set; } // FullName (length: 90) 

        [Required(ErrorMessage = "System for customer use is required")]
        public int SystemId { get; set; } // SystemId 

        public string SystemName { get; set; } // SystemName 

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(50, ErrorMessage = "Phone number can not be longer than 50 characters")]
        public string Phone { get; set; } // Phone (length: 50)

        [Required(ErrorMessage = "Account name is required")]
        [StringLength(150, ErrorMessage = "Account name can not be longer than 150 characters")]
        public string Nickname { get; set; } // Nickname (length: 150)

        [Required(ErrorMessage = "Sex is required to enter")]
        public byte GenderId { get; set; } // GenderId 

        [StringLength(600, ErrorMessage = "Address can not be longer than 600 characters")]
        public string Address { get; set; } // Address (length: 600)
        public int? UserId { get; set; } // UserId 
        public string UserFullName { get; set; } // UserFullName

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        [Required(ErrorMessage = "Please select the type of customer")]
        public int CustomerTypeId { get; set; } // CustomerTypeId 

        public string CustomerTypeName { get; set; } // CustomerTypeName (length: 300)
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
        /// Id Xã/Phường
        ///</summary>
        public int? WardId { get; set; } // WardId

        ///<summary>
        /// Tên xã phường
        ///</summary>
        public string WardsName { get; set; } // WardsName (length: 300)
        public DateTime? Birthday { get; set; }

    }
}