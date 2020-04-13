using System;

namespace Library.ViewModels.Items
{
    public class DepositDetailViewItem
    {
        public DateTime CreateDate { get; set; } // CreateDate
        public DateTime UpdateDate { get; set; } // UpdateDate
        public double Weight { get; set; } // Weight

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

        ///<summary>
        /// Danh sách mã vận đơn
        ///</summary>
        public string ListCode { get; set; } // ListCode (length: 1000)

        public decimal ShipTq { get; set; }
    }
}