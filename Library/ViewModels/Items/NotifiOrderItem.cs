using System;

namespace Library.ViewModels.Items
{
    public class NotifiOrderItem
    {
        ///<summary>
        /// Mô tả chi tiết
        ///</summary>
        public string Description { get; set; } // Description (length: 1000)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime? CreateDate { get; set; } // CreateDate
        

        ///<summary>
        /// Id order gắn với thông báo
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Kiểu thông báo. 0: đơn order, 1: đơn ký gửi, 2: đơn tìm nguồn, 3: khiếu nại
        ///</summary>
        public int? OrderType { get; set; } // OrderType
        
        ///<summary>
        /// Đã đọc hay chưa
        ///</summary>
        public bool IsRead { get; set; } // IsRead

        ///<summary>
        /// Tiêu đề thông báo
        ///</summary>
        public string Title { get; set; } // Title (length: 255)
       
        ///<summary>
        /// Tên người tạo thông báo
        ///</summary>
        public string UserName { get; set; } // UserName (length: 90)

        public long Id { get; set; }
    }
}
