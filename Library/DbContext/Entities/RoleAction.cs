using System;

namespace Library.DbContext.Entities
{
    // RoleAction
    
    public partial class RoleAction
    {

        ///<summary>
        /// Khai báo các hoành động trong hệ thống như (Thêm, Xem, Sửa, Xóa, Import, Export, Upload,...)
        ///</summary>
        public byte Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 500)
        public string Description { get; set; } // Description (length: 500)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created

        public RoleAction()
        {
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
