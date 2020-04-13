using System.ComponentModel.DataAnnotations;

namespace Library.Models
{
    public class LayoutMeta
    {
        public int Id { get; set; } // Id (Primary key)
        [Required(ErrorMessage = "Please select the layout container")]
        ///<summary>
        /// Id kho chứa layout
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho chứa layout
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)
        [Required(ErrorMessage = "Please enter a layout name")]
        ///<summary>
        /// Tên layout
        ///</summary>
        public string Name { get; set; } // Name (length: 300)

        ///<summary>
        /// Loại của layout: 0: Khu vực, 1: Layout, 2: Giá kệ, 3: Hàng trong giá kệ, 4: Bin trong giá kệ
        ///</summary>
        public byte Mode { get; set; } // Mode
        [Required(ErrorMessage = "Layout code can not be empty.")]
        ///<summary>
        /// Mã của layout
        ///</summary>
        public string Code { get; set; } // Code (length: 50)
        ///<summary>
        /// Id Layout cha
        ///</summary>
        public int? ParentLayoutId { get; set; } // ParentLayoutId

        ///<summary>
        /// Tên Layout cha
        ///</summary>
        public string ParentLayoutName { get; set; } // ParentLayoutName (length: 300)

        

        ///<summary>
        /// Mô tả về layout
        ///</summary>
        public string Description { get; set; } // Description (length: 500)
        
        ///<summary>
        /// 0: Mới, 1: Đang sử dụng, 2: Cũ
        ///</summary>
        public byte Status { get; set; } // Status
        
        ///<summary>
        /// Chiều dài
        ///</summary>
        public int? Length { get; set; } // Length

        ///<summary>
        /// Chiều rộng
        ///</summary>
        public int? Width { get; set; } // Width

        ///<summary>
        /// Chiều cao
        ///</summary>
        public int? Height { get; set; } // Height
        ///<summary>
        /// Cân nặng tối đa
        ///</summary>
        public int? MaxWeight { get; set; } // MaxWeight
        
    }
}
