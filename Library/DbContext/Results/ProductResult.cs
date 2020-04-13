using System;
using System.Collections.Generic;

namespace Library.DbContext.Results
{
    public class ProductResult
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên sản phẩm
        ///</summary>
        public string Name { get; set; } // Name (length: 600)

        ///<summary>
        /// Ảnh
        ///</summary>
        public List<string> Images { get; set; } // Image

        ///<summary>
        /// Số lượng sản phẩm
        ///</summary>
        public int Quantity { get; set; } // Quantity

        ///<summary>
        /// Link sản phẩm
        ///</summary>
        public string Link { get; set; } // Link

        ///<summary>
        /// Thuộc tính
        ///</summary>
        public List<string> Properties { get; set; } // Properties (length: 600)

        ///<summary>
        /// Thời gian tạo link sản phẩm
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Thời gian cập nhật gần đây
        ///</summary>
        public DateTime LastUpdate { get; set; } // LastUpdate

        ///<summary>
        /// Id nhóm sản phẩm
        ///</summary>
        public int? CategoryId { get; set; } // CategoryId

        ///<summary>
        /// Tên nhóm sản phẩm
        ///</summary>
        public string CategoryName { get; set; } // CategoryName
    }
}