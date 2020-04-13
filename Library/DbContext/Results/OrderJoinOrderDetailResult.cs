using System;

namespace Library.DbContext.Results
{
    public class OrderJoinOrderDetailResult
    {
        ///<summary>
        /// Id đơn hàng
        /// orderdetail
        ///</summary>
        public int Id { get; set; } // Id (Primary key)

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

        //Todo lấy thuộc tính của bảng order
        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Loại đơn hàng: Ký gửi, Order,..
        ///</summary>
        public byte Type { get; set; } // Type

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

        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)

        ///<summary>
        /// Trạng thái đơn hàng
        ///</summary>
        public byte Status { get; set; } // Status

        ///<summary>
        /// Thời gian tạo đơn hàng
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Tổng giá trị tiền + tiền dịch vụ và sau khi giảm giá VND
        ///</summary>
        public decimal Total { get; set; } // Total
    }
}