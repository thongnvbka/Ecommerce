using System;

namespace Library.DbContext.Entities
{
    // OrderRefundDetail
    
    public partial class OrderRefundDetail
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderRefundId { get; set; } // OrderRefundId
        public int OrderDetailCountingId { get; set; } // OrderDetailCountingId

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh sản phẩm
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Số lượng đặt được
        ///</summary>
        public int? Quantity { get; set; } // Quantity

        ///<summary>
        /// Thuộc tính sản phẩm JSON
        ///</summary>
        public string Properties { get; set; } // Properties (length: 600)

        ///<summary>
        /// Số lượng link trong đơn hàng
        ///</summary>
        public int ProductNo { get; set; } // ProductNo

        ///<summary>
        /// Số lượng ít nhất của sản phẩm
        ///</summary>
        public int? BeginAmount { get; set; } // BeginAmount

        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Giao VNĐ
        ///</summary>
        public decimal ExchangePrice { get; set; } // ExchangePrice

        ///<summary>
        /// Tỷ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal TotalExchange { get; set; } // TotalExchange
        public int? QuantityLose { get; set; } // QuantityLose

        ///<summary>
        /// Ghi chú
        ///</summary>
        public string Note { get; set; } // Note
        public DateTime Created { get; set; } // Created
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPriceLose { get; set; } // TotalPriceLose

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal TotalExchangeLose { get; set; } // TotalExchangeLose

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPriceShop { get; set; } // TotalPriceShop

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal TotalExchangeShop { get; set; } // TotalExchangeShop

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPriceCustomer { get; set; } // TotalPriceCustomer
        public bool IsDelete { get; set; } // IsDelete

        public OrderRefundDetail()
        {
            ExchangePrice = 0m;
            ExchangeRate = 0m;
            TotalExchange = 0m;
            TotalExchangeLose = 0m;
            TotalExchangeShop = 0m;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
