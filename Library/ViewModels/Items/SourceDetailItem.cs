using System;

namespace Library.ViewModels.Items
{
    public class SourceDetailItem
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public byte Type { get; set; }
        public string AnalyticSupplier { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public byte Status { get; set; }
        public decimal ServiceMoney { get; set; }
        public int TypeService { get; set; }
        public decimal ShipMoney { get; set; }
        public long SourceSupplierId { get; set;}
        ///<summary>
        /// Tên khách hàng
        ///</summary>
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Họ tên người nhận
        ///</summary>
        public string CustomerAddress { get; set; } // CustomerAddress (length: 255)
        public string TypeServiceName { get; set; } // TypeServiceName (length: 50)
    }
    public class SourceProductItem
    {
        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Tên nhóm sản phẩm
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Kích cỡ sản phẩm
        ///</summary>
        public string Size { get; set; } // Size (length: 50)

        ///<summary>
        /// Màu sắc sản phẩm
        ///</summary>
        public string Color { get; set; } // Color (length: 50)

        ///<summary>
        /// Khách hàng ghi chú cho sản phẩm
        ///</summary>
        public string Note { get; set; } // Note (length: 600)
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
    }
    public class SourceSupplierItem
    {
        public long Id { get; set; } // Id (Primary key)
        
        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

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
        public decimal? ShipMoney { get; set; } // ShipMoney
    }
}
