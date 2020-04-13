
namespace Library.ViewModels.Items
{
    public class DepositAddItem
    {
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)
        ///<summary>
        /// Họ tên người nhận
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 255) 
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
        /// Mã kho TQ khách gửi hàng
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho TQ khách gửi hàng
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 500)
    }

    public class DepositDetailAddItem
    {
        public int Index { get; set; }
        public string Weight { get; set; } // Weight
        ///<summary>
        /// Id nhóm sản phẩm
        ///</summary>
        public int CategoryId { get; set; } // CategoryId

        ///<summary>
        /// Tên nhóm sản phẩm
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string ProductName { get; set; } // ProductName (length: 300)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image { get; set; } // Image

        ///<summary>
        /// Khách hàng ghi chú cho sản phẩm
        ///</summary>
        public string Note { get; set; } // Note (length: 600)
        public int PacketNumber { get; set; } // PacketNumber
        public string PacketCode { get; set; }
        public decimal? ShipTq { get; set; }
    }
}
