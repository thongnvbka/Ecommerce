using System;

namespace Library.DbContext.Results
{
    public class OrderRefundDetailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public int OrderId { get; set; } // OrderId
        public string OrderCode { get; set; } // OrderCode (length: 50)
        public byte OrderType { get; set; } // OrderType

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

        ///<summary>
        /// Id kho đến khách hàng chọn
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho đến khách hàng chọn
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 500)

        ///<summary>
        /// Id khách hàng
        ///</summary>
        public int? CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Địa chỉ khách hàng
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 500)

        public int OrderDetailId { get; set; } // OrderDetailId

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh sản phẩm
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Khách hàng ghi chú cho sản phẩm
        ///</summary>
        public string Note { get; set; } // Note (length: 600)

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

        ///<summary>
        /// Id nhân viên xử lý
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên nhân viên xử lý
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 150)

        ///<summary>
        /// Đơn vị nhân viên tạo
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)

        public int? QuantityLose { get; set; } // QuantityLose

        ///<summary>
        /// 0: Yêu cầu xử lý thiếu sản phẩm, 1: Yêu cầu sử lý sai hàng, 2: Yêu cầu xử lý đổi trả hàng
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// 0: Kho mới tạo, 1: Đặt hàng tiếp nhận, 2: Đặt hàng gửi yêu cầu Shop, 3: Đặt hàng phản hồi, 4 Gửi kế toán xác nhận, 5 Đã hoàn thành, 6 Không xử lý được
        ///</summary>
        public byte Status { get; set; } // Status

        public string NotePrivate { get; set; } // NotePrivate
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

        public int CommentNo { get; set; } // CommentNo

        #region OrderRefundDetail

        public int OrderRefundDetailId { get; set; } // Id (Primary key)
        public int OrderRefundId { get; set; } // OrderRefundId
        public int OrderDetailCountingId { get; set; } // OrderDetailCountingId

        #endregion OrderRefundDetail
    }
}