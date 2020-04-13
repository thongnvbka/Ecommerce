using System;

namespace Library.DbContext.Entities
{
    // GroupPermision
    
    public partial class GroupPermision
    {
        public short Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 300)
        public string UnsignedName { get; set; } // UnsignedName (length: 300)
        public string Description { get; set; } // Description (length: 300)

        ///<summary>
        /// Là nhóm quyền mặc định của hệ thống không thể xóa
        ///</summary>
        public bool IsSystem { get; set; } // IsSystem
        public int UserNo { get; set; } // UserNo
        public short AppNo { get; set; } // AppNo
        public short ModuleNo { get; set; } // ModuleNo
        public short PageNo { get; set; } // PageNo
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Cập nhật gần nhất
        ///</summary>
        public DateTime Updated { get; set; } // Updated
        public bool IsDefault { get; set; } // IsDefault

        public GroupPermision()
        {
            IsSystem = false;
            UserNo = 0;
            AppNo = 0;
            ModuleNo = 0;
            PageNo = 0;
            IsDelete = false;
            IsDefault = false;
            InitializePartial();
        }
        partial void InitializePartial();
    }

}
