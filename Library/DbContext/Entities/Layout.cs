using System;

namespace Library.DbContext.Entities
{
    // Layout

    public partial class Layout
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id kho chứa layout
        ///</summary>
        public int WarehouseId { get; set; } // WarehouseId

        ///<summary>
        /// Tên kho chứa layout
        ///</summary>
        public string WarehouseName { get; set; } // WarehouseName (length: 300)

        ///<summary>
        /// Tên layout
        ///</summary>
        public string Name { get; set; } // Name (length: 300)

        ///<summary>
        /// Loại của layout: 0: Khu vực, 1: Layout, 2: Giá kệ, 3: Hàng trong giá kệ, 4: Bin trong giá kệ
        ///</summary>
        public byte Mode { get; set; } // Mode

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
        /// IdPath của layout
        ///</summary>
        public string IdPath { get; set; } // IdPath (length: 500)

        ///<summary>
        /// Name path của layout
        ///</summary>
        public string NamePath { get; set; } // NamePath

        ///<summary>
        /// Mô tả về layout
        ///</summary>
        public string Description { get; set; } // Description (length: 500)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Cập nhật gần đây
        ///</summary>
        public DateTime Updated { get; set; } // Updated

        ///<summary>
        /// 0: Mới, 1: Đang sử dụng, 2: Cũ
        ///</summary>
        public byte Status { get; set; } // Status
        public bool IsDelete { get; set; } // IsDelete

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

        ///<summary>
        /// Số layout, Bin cấp dưới
        ///</summary>
        public int ChildNo { get; set; } // ChildNo

        ///<summary>
        /// Tên không dấu
        ///</summary>
        public string UnsignName { get; set; } // UnsignName

        public Layout()
        {
            Created = DateTime.Now;
            Updated = DateTime.Now;
            Status = 1;
            IsDelete = false;
            ChildNo = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
