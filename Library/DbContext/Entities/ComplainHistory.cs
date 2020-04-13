using System;

namespace Library.DbContext.Entities
{
    // ComplainHistory

    public partial class ComplainHistory
    {
        public long Id { get; set; } // Id (Primary key)
        public long ComplainId { get; set; } // ComplainId

        ///<summary>
        /// Trạng thái mới của đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Nội dung lịch sử
        ///</summary>
        public string Content { get; set; } // Content (length: 1000)

        ///<summary>
        /// Id khách hàng thay đổi trạng thái
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Họ và tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Id nhân viên thay đổi
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Họ và tên của nhân viên
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)
        public DateTime CreateDate { get; set; } // CreateDate
        public int? ClaimForRefundId { get; set; } // ClaimForRefundId

        public ComplainHistory()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
