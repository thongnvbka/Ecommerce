using System;

namespace Library.DbContext.Entities
{

    // SourceDetail

    public partial class SourceDetail
    {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id đơn hàng tìm nguồn
        ///</summary>
        public long SourceId { get; set; } // SourceId

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Số lượng ít nhất của sản phẩm
        ///</summary>
        public int? BeginAmount { get; set; } // BeginAmount

        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

        ///<summary>
        /// Tỷ giá ngoại tệ
        ///</summary>
        public decimal ExchangeRate { get; set; } // ExchangeRate

        ///<summary>
        /// Giao VNĐ
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
        /// Khách hàng ghi chú cho sản phẩm
        ///</summary>
        public string Note { get; set; } // Note (length: 600)

        ///<summary>
        /// Trạng thái sản phẩm: Đã đặt, Hủy,...
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Số lượng đặt được
        ///</summary>
        public int QuantityBooked { get; set; } // QuantityBooked

        ///<summary>
        /// Thuộc tính JSON
        ///</summary>
        public string Properties { get; set; } // Properties (length: 600)

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        ///<summary>
        /// Id nhóm sản phẩm
        ///</summary>
        public int? CategoryId { get; set; } // CategoryId

        ///<summary>
        /// Tên nhóm sản phẩm
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate
        public bool IsDelete { get; set; } // IsDelete
        public string UniqueCode { get; set; } // UniqueCode

        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Màu sắc sản phẩm
        ///</summary>
        public string Color { get; set; } // Color (length: 50)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string ImagePath1 { get; set; } // ImagePath1 (length: 255)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string ImagePath2 { get; set; } // ImagePath2 (length: 255)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string ImagePath3 { get; set; } // ImagePath3 (length: 255)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string ImagePath4 { get; set; } // ImagePath4 (length: 255)

        public SourceDetail()
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
