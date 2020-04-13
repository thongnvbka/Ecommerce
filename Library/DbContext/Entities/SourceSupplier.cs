using System;

namespace Library.DbContext.Entities
{

    // SourceSupplier

    public partial class SourceSupplier
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id đơn hàng tìm nguồn
        ///</summary>
        public long? SourceId { get; set; } // SourceId

        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Tỷ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Giá VNĐ
        ///</summary>
        public decimal ExchangePrice { get; set; } // ExchangePrice

        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Thành tiền sau khi chuyển đổi VNĐ
        ///</summary>
        public decimal TotalExchange { get; set; } // TotalExchange

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity
        public string Name { get; set; } // Name (length: 255)

        ///<summary>
        /// Trạng thái báo giá của ncc
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Link tìm kiếm ncc
        ///</summary>
        public string Link { get; set; } // Link
        public string Description { get; set; } // Description

        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate
        public bool IsDelete { get; set; } // IsDelete
        public decimal? ShipMoney { get; set; } // ShipMoney
        public DateTime? ActiveDate { get; set; } // ActiveDate
        public DateTime? LimitDate { get; set; } // ActiveDate
        public SourceSupplier()
        {
            ExchangeRate = 0m;
            ExchangePrice = 0m;
            TotalExchange = 0m;
            Created = DateTime.Now;
            LastUpdate = DateTime.Now;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
