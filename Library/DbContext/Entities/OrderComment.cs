using System;

namespace Library.DbContext.Entities
{

    // OrderComment

    public partial class OrderComment
    {

        ///<summary>
        /// Khóa chính trao đổi
        ///</summary>
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Nội dung trao đổi
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// Mã đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Mã nhân viên
        ///</summary>
        public int? UserId { get; set; } // UserId
        public DateTime? CreateDate { get; set; } // CreateDate
        public bool? IsRead { get; set; } // IsRead
        public string CustomerName { get; set; } // CustomerName (length: 50)
        public string UserName { get; set; } // UserName (length: 50)
        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)
        public byte? OrderType { get; set; } // OrderType
        /// <summary>
        /// Loại tin nhắn 0-Text, 1-Ảnh, 2-Icon
        /// </summary>
        public byte? CommentType { get; set; } //CommentType
        ///<summary>
        /// Phòng ban người dùng comment
        ///</summary>
        public string UserOffice { get; set; } // UserOffice (length: 100)
        ///<summary>
        /// Số điện thoại người dùng
        ///</summary>
        public string UserPhone { get; set; } // UserPhone (length: 100)

        /// <summary>
        /// Gộp chat
        /// </summary>
        public int? GroupId { get; set; }
        public OrderComment()
        {
            CreateDate = DateTime.Now;
            CommentType = 0;
            GroupId = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
