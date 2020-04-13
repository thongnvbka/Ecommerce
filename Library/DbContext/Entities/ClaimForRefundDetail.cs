using System;
namespace Library.DbContext.Entities
{
    // ClaimForRefundDetail

    public partial class ClaimForRefundDetail
    {
        public int Id { get; set; } // Id (Primary key)

        public long? ProductId { get; set; } // Mã sản phẩm
        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int? Quantity { get; set; } // Quantity

        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Màu sắc sản phẩm
        ///</summary>
        public string Color { get; set; } // Color (length: 50)

        ///<summary>
        /// Đơn giá ngoại tệ
        ///</summary>
        public decimal? Price { get; set; } // Price

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Đơn Giá (Ngoại tệ)
        ///</summary>
        public decimal? TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal? TotalExchange { get; set; } // TotalExchange

        ///<summary>
        /// Id đơn hàng có sản phẩm lỗi
        ///</summary>
        public int? OrderId { get; set; } // OrderId

        ///<summary>
        /// Loại đơn hàng có sản phẩm lỗi
        ///</summary>
        public byte? OrderType { get; set; } // OrderType

        ///<summary>
        /// Số lượng sản phẩm khách khiếu nại thực tế
        ///</summary>
        public int? QuantityFailed { get; set; } // QuantityFailed

        ///<summary>
        /// Tổng tiền dự kiến hoàn tiền của sản phẩm
        ///</summary>
        public decimal? TotalQuantityFailed { get; set; } // TotalQuantityFailed

        ///<summary>
        /// Ghi chú cho sản phẩm bị đổi trả
        ///</summary>
        public string Note { get; set; } // Note

        ///<summary>
        /// Id phiếu xử lý hoàn tiền xử lý khiếu nại
        ///</summary>
        public int? ClaimId { get; set; } // ClaimId

        ///<summary>
        /// Code phiếu hoàn tiền xử lý khiếu nại
        ///</summary>
        public string ClaimCode { get; set; } // ClaimCode (length: 30)

        public ClaimForRefundDetail()
        {
            TotalExchange = 0m;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
