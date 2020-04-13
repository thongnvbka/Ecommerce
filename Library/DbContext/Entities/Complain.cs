using System;

namespace Library.DbContext.Entities
{

    // Complain

    public partial class Complain
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã ticket
        ///</summary>
        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Vấn đề hàng hóa.0: Sai lệch lệ phí vận chuyển, 1: Hàng hỏng, 2: hàng về sai quy cách
        ///</summary>
        public byte TypeOrder { get; set; } // TypeOrder

        ///<summary>
        /// Id loại khiếu nại khách hàng
        ///</summary>
        public int TypeService { get; set; } // TypeService

        ///<summary>
        /// Tên loại khiếu nại khách hàng chọn
        ///</summary>
        public string TypeServiceName { get; set; } // TypeServiceName (length: 500)

        ///<summary>
        /// Id khiếu nại chốt
        ///</summary>
        public int? TypeServiceClose { get; set; } // TypeServiceClose

        ///<summary>
        /// Tên loại khiếu nại chốt
        ///</summary>
        public string TypeServiceCloseName { get; set; } // TypeServiceCloseName (length: 500)

        ///<summary>
        /// Hình ảnh khiếu nại 1
        ///</summary>
        public string ImagePath1 { get; set; } // ImagePath1 (length: 255)

        ///<summary>
        /// Hình ảnh khiếu nại 2
        ///</summary>
        public string ImagePath2 { get; set; } // ImagePath2 (length: 255)

        ///<summary>
        /// Hình ảnh khiếu nại 3
        ///</summary>
        public string ImagePath3 { get; set; } // ImagePath3 (length: 255)

        ///<summary>
        /// Hình ảnh khiếu nại 4
        ///</summary>
        public string ImagePath4 { get; set; } // ImagePath4 (length: 255)

        ///<summary>
        /// Hình ảnh khiếu nại 5
        ///</summary>
        public string ImagePath5 { get; set; } // ImagePath5 (length: 255)

        ///<summary>
        /// Hình ảnh khiếu nại 6
        ///</summary>
        public string ImagePath6 { get; set; } // ImagePath6 (length: 255)

        ///<summary>
        /// Nội dung khiếu nại
        ///</summary>
        public string Content { get; set; } // Content (length: 2000)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Mã code đơn hàng
        ///</summary>
        public string OrderCode { get; set; } // OrderCode (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte? OrderType { get; set; } // OrderType

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật gần nhất
        ///</summary>
        public DateTime LastUpdateDate { get; set; } // LastUpdateDate

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int? SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

        ///<summary>
        /// Trạng thái khiếu nại: 0: chờ xử lý, 1: đang xử lý, 2: hoàn thành, 3: hủy
        ///</summary>
        public byte? Status { get; set; } // Status

        ///<summary>
        /// Comment cuối cùng
        ///</summary>
        public string LastReply { get; set; } // LastReply (length: 2000)

        ///<summary>
        /// Số tiền bồi thường
        ///</summary>
        public decimal? BigMoney { get; set; } // BigMoney

        ///<summary>
        /// Bản ghi đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Số tiền mong muốn bồi thường
        ///</summary>
        public decimal? RequestMoney { get; set; } // RequestMoney
        public string ContentInternal { get; set; } // ContentInternal
        public string ContentInternalOrder { get; set; } // ContentInternalOrder

        public Complain()
        {
            CreateDate = DateTime.Now;
            LastUpdateDate = DateTime.Now;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
