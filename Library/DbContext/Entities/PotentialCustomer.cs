using System;

namespace Library.DbContext.Entities
{
    public partial class PotentialCustomer
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

        ///<summary>
        /// mã tài khoản
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string Email { get; set; } // Email (length: 30)

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string FirstName { get; set; } // FirstName (length: 30)

        ///<summary>
        /// Họ khách hàng
        ///</summary>
        public string LastName { get; set; } // LastName (length: 30)

        ///<summary>
        /// Tên đệm khách hàng
        ///</summary>
        public string MidleName { get; set; } // MidleName (length: 30)

        ///<summary>
        /// Tên đầy đủ khách hàng
        ///</summary>
        public string FullName { get; set; } // FullName (length: 90)

        ///<summary>
        /// Tài khoản khách hàng từ hệ thống
        ///</summary>
        public int? SystemId { get; set; } // SystemId
        public string SystemName { get; set; } // SystemName (length: 200)

        ///<summary>
        /// Điện thoại khác hàng
        ///</summary>
        public string Phone { get; set; } // Phone (length: 50)

        ///<summary>
        /// Ảnh đại điện
        ///</summary>
        public string Avatar { get; set; } // Avatar

        ///<summary>
        /// Tên hiển thị khách hàng
        ///</summary>
        public string Nickname { get; set; } // Nickname (length: 150)

        ///<summary>
        /// Ngày sinh khách hàng
        ///</summary>
        public DateTime? Birthday { get; set; } // Birthday

        ///<summary>
        /// Id cấp bậc khách hàng
        ///</summary>
        public byte? LevelId { get; set; } // LevelId

        ///<summary>
        /// Tên cấp bậc khách hàng: Vip1,Vip2,..
        ///</summary>
        public string LevelName { get; set; } // LevelName (length: 150)

        ///<summary>
        /// Id giới tính
        ///</summary>
        public byte? GenderId { get; set; } // GenderId

        ///<summary>
        /// Tên giói tính
        ///</summary>
        public string GenderName { get; set; } // GenderName (length: 300)

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

        ///<summary>
        /// Chi tiết địa chỉ khách hàng
        ///</summary>
        public string Address { get; set; } // Address (length: 600)

        ///<summary>
        /// Tài khoản khách hàng của nhân viên
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên quản lý khách hàng
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

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
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag
        public string CountryId { get; set; } // CountryId (length: 10)
        public bool IsDelete { get; set; } // IsDelete
        public int? CustomerTypeId { get; set; } // CustomerTypeId
        public string CustomerTypeName { get; set; } // CustomerTypeName (length: 300)

        /// <summary>
        /// Trạng thái của khách hàng tiềm năng 0: Kích hoạt 1: Chưa kích hoạt 2: Chờ khách hàng xác thực chính thức
        /// </summary>
        public byte Status { get; set; }

        public PotentialCustomer()
        {
            Created = DateTime.Now;
            Updated = DateTime.Now;
            IsDelete = false;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }
}