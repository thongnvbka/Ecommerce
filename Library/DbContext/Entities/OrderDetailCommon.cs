using System;

namespace Library.DbContext.Entities
{
    public partial class OrderDetailCommon
        {
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id đơn hàng
        ///</summary>
        public long OrderId { get; set; } // OrderId

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh
        ///</summary>
        public string Image1 { get; set; } // Image
        public string Image2 { get; set; } // Image
        public string Image3 { get; set; } // Image
        public string Image4 { get; set; } // Image
        public string Image5 { get; set; } // Image
        public string Image6 { get; set; } // Image

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        
        ///<summary>
        /// Giá ngoại tệ
        ///</summary>
        public decimal Price { get; set; } // Price

       
        ///<summary>
        /// Thành tiền ngoại tệ số lượng * Giá (Ngoại tệ)
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link
        
        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag
        
        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate
        public bool IsDelete { get; set; } // IsDelete
        ///<summary>
        public OrderDetailCommon()
        {
            Created = DateTime.Now;
            LastUpdate = DateTime.Now;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
