using System;

namespace Library.ViewModels.Items
{
    public class ShopItem
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên Shop
        ///</summary>
        public string Name { get; set; } // Name (length: 255)

        ///<summary>
        /// Shop bán hàng trên website
        ///</summary>
        public string Website { get; set; } // Website (length: 300)

        ///<summary>
        /// Chi tiết đường link đến shop
        ///</summary>
        public string Url { get; set; } // Url

        ///<summary>
        /// Id nghành hàng của shop
        ///</summary>
        public int? CategoryId { get; set; } // CategoryId

        ///<summary>
        /// Tên nghành hàng của shop
        ///</summary>
        public string CategoryName { get; set; } // CategoryName (length: 300)

        ///<summary>
        /// Điểm đánh gái shop
        ///</summary>
        public int? Vote { get; set; } // Vote

        ///<summary>
        /// Ghi chú cho shop
        ///</summary>
        public string Note { get; set; } // Note (length: 500)

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        public byte Status { get; set; } // Status

        ///<summary>
        /// Tổng số link các đơn hàng hoàn thành với Shop
        ///</summary>
        public int LinkNo { get; set; } // LinkNo

        ///<summary>
        /// Tổng số đơn hàng hoàn thành với Shop
        ///</summary>
        public int OrderNo { get; set; } // OrderNo

        ///<summary>
        /// Tổng số sản phẩm của đơn đã hoàn thành với Shop
        ///</summary>
        public int ProductNo { get; set; } // ProductNo

        ///<summary>
        /// Tổng tiền hàng của các đơn đã mua của Shop
        ///</summary>
        public decimal TotalPrice { get; set; } // TotalPrice

        ///<summary>
        /// Giá trị tiền mặc cả được cao nhất đã mặc ả của Shop
        ///</summary>
        public decimal BargainMax { get; set; } // BargainMax

        ///<summary>
        /// Giá trị tiền mặc cả thấp nhất đã mặc cả được của Shop
        ///</summary>
        public decimal BargainMin { get; set; } // BargainMin

        public bool IsDelete { get; set; } // IsDelete
        public DateTime CreateDate { get; set; } // CreateDate
        public DateTime UpdateDate { get; set; } // UpdateDate

        public int TotalOrder { get; set; }
        public decimal TotalMoney { get; set; }
        public decimal TotalBargain { get; set; }
        public decimal TotalBargainVn { get; set; }
    }
}