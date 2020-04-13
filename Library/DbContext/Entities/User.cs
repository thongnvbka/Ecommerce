using System;

namespace Library.DbContext.Entities
{
    // User

    public partial class User
    {
        public int Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string Password { get; set; } // Password (length: 50)

        ///<summary>
        /// Tên
        ///</summary>
        public string FirstName { get; set; } // FirstName (length: 30)

        ///<summary>
        /// Tên đệm
        ///</summary>
        public string MidleName { get; set; } // MidleName (length: 30)

        ///<summary>
        /// Họ
        ///</summary>
        public string LastName { get; set; } // LastName (length: 30)

        ///<summary>
        /// Tên đầy đủ
        ///</summary>
        public string FullName { get; set; } // FullName (length: 100)
        public string UnsignName { get; set; } // UnsignName (length: 500)

        ///<summary>
        /// Giới tính: 0: Nữ, 1: Nam, 2: Bí mật
        ///</summary>
        public byte Gender { get; set; } // Gender
        public string Email { get; set; } // Email (length: 50)

        ///<summary>
        /// Mô tả về chức vụ
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Cập nhật gần nhất
        ///</summary>
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// UserId cập nhật gần nhất
        ///</summary>
        public int LastUpdateUserId { get; set; } // LastUpdateUserId

        ///<summary>
        /// Đơn vị đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// 0: Nghỉ việc, 1: Bình thường, 2: Nghỉ hưu
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Sinh nhật
        ///</summary>
        public DateTime? Birthday { get; set; } // Birthday

        ///<summary>
        /// Ngày bắt đầu làm việc
        ///</summary>
        public DateTime? StartDate { get; set; } // StartDate
        public string Avatar { get; set; } // Avatar (length: 2000)
        public bool IsLockout { get; set; } // IsLockout
        public DateTime? LastLockoutDate { get; set; } // LastLockoutDate
        public DateTime? LockoutToDate { get; set; } // LockoutToDate
        public DateTime? FirstLoginFailureDate { get; set; } // FirstLoginFailureDate
        public byte LoginFailureCount { get; set; } // LoginFailureCount
        public bool IsSystem { get; set; } // IsSystem
        ///<summary>
        /// Số đt trong công ty
        ///</summary>
        public string Phone { get; set; } // Phone (length: 20)

        ///<summary>
        /// Số đt cá nhân
        ///</summary>
        public string Mobile { get; set; } // Mobile (length: 20)

        ///<summary>
        /// Tên ngân hàng
        ///</summary>
        public string NameBank { get; set; } // NameBank (length: 50)

        ///<summary>
        /// Chi nhánh ngân hàng
        ///</summary>
        public string Department { get; set; } // Department (length: 600)

        ///<summary>
        /// Số tài khoản ngân hàng
        ///</summary>
        public string BankAccountNumber { get; set; } // BankAccountNumber (length: 50)

        /// <summary>
        /// TRUE: Tài khoản ảo của công ty FALSE: Tài khoản nhân viên
        /// </summary>
        public bool IsCompany { get; set; }
        ///<summary>
        /// UserId cập nhật gần nhất
        ///</summary>
        public int? TypeId { get; set; } // TypeId
        ///<summary>
        /// Mã định danh đối tượng trên hệ thống
        ///</summary>
        public int? TypeIdd { get; set; } // TypeIdd
        ///<summary>
        /// Tên đối tượng người dùng trên  hệ thống 
        ///</summary>
        public string TypeName { get; set; } // TypeName
        /// <summary>
        /// Website đặt hàng mà nhân viên đó phụ trách, áp dụng cho nhân viên đặt hàng
        /// </summary>
        public string Websites { get; set; }
        public string Culture { get; set; }

        public User()
        {
            UnsignName = "";
            IsDelete = false;
            IsLockout = false;
            LoginFailureCount = 0;
            IsSystem = false;
            IsCompany = false;
            Culture = "vi-VN";
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
