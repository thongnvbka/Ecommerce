using System;

namespace Library.DbContext.Entities
{

    // Notification

    public partial class Notification
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 200)

        ///<summary>
        /// Mô tả chi tiết
        ///</summary>
        public string Description { get; set; } // Description (length: 1000)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Id order gắn với thông báo
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Kiểu thông báo. 0: đơn order, 1: đơn ký gửi, 2: đơn tìm nguồn, 3: khiếu nại
        ///</summary>
        public int? OrderType { get; set; } // OrderType

        ///<summary>
        /// Mã khách hàng, mã khách hàng null nếu order type = 0
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 90)

        ///<summary>
        /// Đã đọc hay chưa
        ///</summary>
        public bool IsRead { get; set; } // IsRead

        ///<summary>
        /// Tiêu đề thông báo
        ///</summary>
        public string Title { get; set; } // Title (length: 255)

        ///<summary>
        /// Mã người tạo
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên người tạo thông báo
        ///</summary>
        public string UserName { get; set; } // UserName (length: 90)

        public Notification()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
