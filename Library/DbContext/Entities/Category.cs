using System;

namespace Library.DbContext.Entities
{
    // Category
    
    public partial class Category
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Tên nghành hàng
        ///</summary>
        public string Name { get; set; } // Name (length: 300)

        ///<summary>
        /// IdPath dạng: IdParen.MyId
        ///</summary>
        public string IdPath { get; set; } // IdPath (length: 600)
        public string NamePath { get; set; } // NamePath (length: 200)

        ///<summary>
        /// Id chuyên mục cha
        ///</summary>
        public int? ParentId { get; set; } // ParentId

        ///<summary>
        /// Tên chuyên mục cha
        ///</summary>
        public string ParentName { get; set; } // ParentName (length: 300)

        ///<summary>
        /// Trạng thái
        ///</summary>
        public int Status { get; set; } // Status

        ///<summary>
        /// Mô tả
        ///</summary>
        public string Description { get; set; } // Description (length: 600)

        ///<summary>
        /// Là xóa hay không
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete
        public DateTime Created { get; set; } // Created
        public DateTime LastUpdated { get; set; } // LastUpdated

        ///<summary>
        /// Tag link toàn bộ hệ thống
        ///</summary>
        public string HashTag { get; set; } // HashTag

        public Category()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
