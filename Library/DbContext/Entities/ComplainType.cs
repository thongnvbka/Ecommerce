using System;

namespace Library.DbContext.Entities
{
    // ComplainType

    public partial class ComplainType
    {

        ///<summary>
        /// Mã quỹ
        ///</summary>
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Đường dẫn
        ///</summary>
        public string IdPath { get; set; } // IdPath (length: 300)

        ///<summary>
        /// Tên đường dẫn
        ///</summary>
        public string NamePath { get; set; } // NamePath (length: 500)

        ///<summary>
        /// Tên quỹ
        ///</summary>
        public string Name { get; set; } // Name (length: 500)

        ///<summary>
        /// Mã cha, mặc định cha ngoài cùng có mã:0
        ///</summary>
        public int ParentId { get; set; } // ParentId

        ///<summary>
        /// Tên cha, mặc định là rỗng ""
        ///</summary>
        public string ParentName { get; set; } // ParentName (length: 300)


        ///<summary>
        /// 0: chưa xóa. 1: đã xóa
        ///</summary>
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// True là cha, fale là con
        ///</summary>
        public bool IsParent { get; set; } // IsParent

        ///<summary>
        /// Mô tả
        ///</summary>
        public string Description { get; set; } // Description (length: 500)
        ///<summary>
        /// Số thứ tự sắp xếp
        ///</summary>
        public int? Index { get; set; } // Index 

        public ComplainType()
        {
            Name = "getdate()";
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
