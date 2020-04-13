using System;

namespace Library.DbContext.Entities
{
    // PackageHistory
    
    public partial class PackageHistory
    {
        public long Id { get; set; } // Id (Primary key)
        public int PackageId { get; set; } // PackageId
        public string PackageCode { get; set; } // PackageCode (length: 50)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode

        ///<summary>
        /// Loại đơn hàng order, ký gửi, tìm nguồn
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Trạng thái của kiện hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Nội dung lịch sử
        ///</summary>
        public string Content { get; set; } // Content (length: 1000

        ///<summary>
        /// Lưu dữ liệu Json
        ///</summary>
        public string JsonData { get; set; }

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
        public string UserName { get; set; } // UserName (length: 300)

        ///<summary>
        /// Họ và tên của nhân viên
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)
        public DateTime CreateDate { get; set; } // CreateDate

        public PackageHistory()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
