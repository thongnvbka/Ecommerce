using System;

namespace Library.DbContext.Entities
{
    // OrderDetail

    public partial class OrderDetail
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public int OrderId { get; set; } // OrderId

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image { get; set; } // Image

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
        public decimal? AuditPrice { get; set; } // AuditPrice

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
        public int? QuantityBooked { get; set; } // QuantityBooked

        ///<summary>
        /// Số lượng kho nhận
        ///</summary>
        public int? QuantityRecived { get; set; } // QuantityRecived

        ///<summary>
        /// Số lượng kiểm đếm sai
        ///</summary>
        public int? QuantityIncorrect { get; set; } // QuantityIncorrect

        ///<summary>
        /// Khách hàng thực nhận
        ///</summary>
        public int? QuantityActuallyReceived { get; set; } // QuantityActuallyReceived

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
        /// Nhân viên ghi chú cho sản phẩm
        ///</summary>
        public string UserNote { get; set; } // UserNote (length: 600)

        public DateTime? CountingTime { get; set; } // CountingTime
        public int? CountingUserId { get; set; } // CountingUserId
        public string CountingUserName { get; set; } // CountingUserName (length: 50)
        public string CountingFullName { get; set; } // CountingFullName (length: 300)
        public int? CountingOfficeId { get; set; } // CountingOfficeId
        public string CountingOfficeName { get; set; } // CountingOfficeName (length: 300)
        public string CountingOfficeIdPath { get; set; } // CountingOfficeIdPath (length: 300)

        ///<summary>
        /// Số lượng Min của sản phẩm trong 1688
        ///</summary>
        public int Min { get; set; } // Min

        ///<summary>
        /// Số lượng max sản phẩm trong 1688
        ///</summary>
        public int Max { get; set; } // Max

        ///<summary>
        /// Khoảng giá sản phẩm trong 1688
        ///</summary>
        public string Prices { get; set; } // Prices (length: 600)

        ///<summary>
        /// Id sản phẩm trong 1688
        ///</summary>
        public string ProId { get; set; } // ProId (length: 50)

        ///<summary>
        /// Sku sản phẩm trong 1688
        ///</summary>
        public string SkullId { get; set; } // SkullId (length: 50)

        ///<summary>
        /// Nguồn Order từ website
        ///</summary>
        public string WebsiteName { get; set; } // WebsiteName (length: 300)

        ///<summary>
        /// Id Shop bán hàng
        ///</summary>
        public int? ShopId { get; set; } // ShopId

        ///<summary>
        /// Tên Shop bán hàng
        ///</summary>
        public string ShopName { get; set; } // ShopName (length: 500)

        ///<summary>
        /// Link của shop bán hàng
        ///</summary>
        public string ShopLink { get; set; } // ShopLink

        /// <summary>
        /// đã xem
        /// </summary>
        public bool? IsView { get; set; }
        public string PrivateNote { get; set; } // PrivateNote (length: 2000)
        public OrderDetail()
        {
            ExchangeRate = 0m;
            ExchangePrice = 0m;
            TotalExchange = 0m;
            Created = DateTime.Now;
            LastUpdate = DateTime.Now;
            IsView = false;
            IsDelete = false;
            Min = 0;
            Max = 0;
            Color = "";
            Size = "";
            Link = "";
            Name = " ";
            Properties = "";
            Note = "";
            UserNote = "";
            WebsiteName = "";
            ShopName = "";
            ShopLink = "";
            ShopId = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
