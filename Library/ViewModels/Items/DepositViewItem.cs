using System;

namespace Library.ViewModels.Items
{
    public class DepositViewItem
    {
        ///<summary>
        /// Mã đơn ký gửi
        ///</summary>
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Mã yêu cầu
        ///</summary>
        public string Code { get; set; } // Code (length: 30
        public byte Type { get; set; } // Code (length: 30)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Họ tên người nhận
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 255)
        public string Note { get; set; } // Note (length: 500)
        public string WarehouseName { get; set; } // WarehouseName (length: 500)
        ///<summary>
        /// Tên liên lạc lấy hàng bên TQ
        ///</summary>
        public string ContactName { get; set; } // ContactName (length: 100)

        ///<summary>
        /// Điện thoại người liên lạc bên TQ
        ///</summary>
        public string ContactPhone { get; set; } // ContactPhone (length: 20)

        ///<summary>
        /// Địa chỉ người liên lạc bên TQ
        ///</summary>
        public string ContactAddress { get; set; } // ContactAddress (length: 255)

        ///<summary>
        /// Điện thoại người nhận
        ///</summary>
        public string ContactEmail { get; set; } // ContactEmail (length: 300)

        ///<summary>
        /// Số kiện
        ///</summary>
        public int? PacketNumber { get; set; } // PacketNumber

        ///<summary>
        /// 1: chờ báo giá, 2: chờ kết đơn, 3: đang giao dịch, 4: hoàn thành, 5: hủy đơn
        ///</summary>
        public byte Status { get; set; } // Status
        ///<summary>
        /// Tiền khách đặt cọc
        ///</summary>
        public decimal? TotalAdvance { get; set; } // TotalAdvance
         ///<summary>
        /// Tiền tạm tính khách phải thanh toán
        ///</summary>
        public decimal? ProvisionalMoney { get; set; } // ProvisionalMoney
        public decimal ExchangeRate { get; set; } // ExchangeRate
        public string WarehouseDeliveryName { get; set; }
        public decimal Total { get; set; } // Total
        public decimal TotalPayed { get; set; } // TotalPayed
        public decimal TotalRefunded { get; set; } // TotalRefunded
        public decimal Debt { get; set; } // Debt
    }
}
