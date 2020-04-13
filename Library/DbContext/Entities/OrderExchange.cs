using System;

namespace Library.DbContext.Entities
{
    // OrderExchange
    
    public partial class OrderExchange
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại thanh toán, Đặt cọc, Đền tiền,...
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// Tên loại thanh toán
        ///</summary>
        public string ModeName { get; set; } // ModeName (length: 300)

        ///<summary>
        /// Đơn vị tiền tệ: VND, USD,...
        ///</summary>
        public string Currency { get; set; } // Currency (length: 50)

        ///<summary>
        /// Tỉ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Tổng giá trị tiền hàng chuyển đổi VND
        ///</summary>
        public decimal? TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Tổng tiền hàng theo ngoại tệ
        ///</summary>
        public decimal? TotalExchange { get; set; } // TotalExchange

        ///<summary>
        /// Trạng thái
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Nhân viên thao tác cuối cùng
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// Id tài khoản
        ///</summary>
        public int? BankId { get; set; } // BankId

        ///<summary>
        /// Loại giao dịch, Nhập, Xuất
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Ghi chú giao dịch
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Là xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Thời gian tạo
        ///</summary>
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag
        ///<summary>
        /// 0: đơn order, 1: đơn ký gửi, 2: đơn tìm nguồn
        ///</summary>
        public byte OrderType { get; set; } // OrderType

        public OrderExchange()
        {
            IsDelete = false;
            HashTag = string.Empty;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
