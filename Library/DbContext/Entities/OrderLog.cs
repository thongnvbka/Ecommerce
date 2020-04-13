using System;

namespace Library.DbContext.Entities
{
    public partial class OrderLog
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Thời gian thao tác
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Loại thay đổi 0: Dữ liệu thông thường 1: Dữ liệu json 2: Thao tác
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Dữ liệu trước thay đổi
        ///</summary>
        public string DataBefore { get; set; } // DataBefore (length: 1000)

        ///<summary>
        /// Dữ liệu sau thay đổi
        ///</summary>
        public string DataAfter { get; set; } // DataAfter (length: 1000)

        ///<summary>
        /// Nội dung thay đổi
        ///</summary>
        public string Content { get; set; } // Content (length: 1000)

        ///<summary>
        /// Mã nhân viên thao tác
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên thao tác
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Mã phòng ban của nhân viên
        ///</summary>
        public int? UserOfficeId { get; set; } // UserOfficeId

        ///<summary>
        /// Tên phòng ban
        ///</summary>
        public string UserOfficeName { get; set; } // UserOfficeName (length: 300)

        ///<summary>
        /// Đường dẫn phòng ban
        ///</summary>
        public string UserOfficePath { get; set; } // UserOfficePath (length: 300)

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerFullName { get; set; } // CustomerFullName (length: 150)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Số điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        public OrderLog()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }
}
