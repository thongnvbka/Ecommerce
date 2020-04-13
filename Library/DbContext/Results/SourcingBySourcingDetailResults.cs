using System;

namespace Library.DbContext.Results
{
    public class SourcingBySourcingDetailResults
    {   ///<summary>
        /// Mã yêu cầu tìm nguồn
        ///</summary>
        public long Id { get; set; } // Id (Primary key)

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

        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity
         
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

        public bool IsDelete { get; set; } // IsDelete 
        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Màu sắc sản phẩm
        ///</summary>
        public string Color { get; set; } // Color (length: 50)

        public string Code { get; set; } // Code (length: 30)

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Đơn hàng phát sinh từ hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 100)

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

        public string CustomerAddress { get; set; } // CustomerPhone (length: 500)
         
        ///<summary>
        /// Gói dịch vụ, 0: gói cơ bản, 1: gói vip, 2: dùng thử
        ///</summary>
        public int TypeService { get; set; } // TypeService

        public string TypeServiceName { get; set; } // TypeServiceName

        ///<summary>
        /// Tiền dịch vụ trên 1 sản phẩm tìm nguồn
        ///</summary>
        public decimal ServiceMoney { get; set; } // ServiceMoney

        ///<summary>
        /// Loại đơn hàng order, ký gửi, tìm nguồn
        ///</summary>
        public byte Type { get; set; } // Status

        ///<summary>
        /// Phục vụ tìm kiếm
        ///</summary>
        public string UnsignName { get; set; } // UnsignName (length: 500)
    }
}