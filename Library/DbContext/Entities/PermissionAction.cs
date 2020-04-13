using System;

namespace Library.DbContext.Entities
{
    // PermissionAction
    
    public partial class PermissionAction
    {

        ///<summary>
        /// Bảng liên kết Quyền trong trang và Các hành dộng cho phép
        ///</summary>
        public int Id { get; set; } // Id (Primary key)
        public byte AppId { get; set; } // AppId
        public short ModuleId { get; set; } // ModuleId
        public short PageId { get; set; } // PageId
        public short? GroupPermisionId { get; set; } // GroupPermisionId
        public string GroupPermisionName { get; set; } // GroupPermisionName (length: 300)
        public byte RoleActionId { get; set; } // RoleActionId
        public string AppName { get; set; } // AppName (length: 300)
        public string ModuleName { get; set; } // ModuleName (length: 300)
        public string PageName { get; set; } // PageName (length: 300)
        public string RoleName { get; set; } // RoleName (length: 300)
        public bool IsDelete { get; set; } // IsDelete

        ///<summary>
        /// Ngày tạo chức vụ
        ///</summary>
        public DateTime Created { get; set; } // Created

        ///<summary>
        /// Cập nhật gần nhất
        ///</summary>
        public DateTime Updated { get; set; } // Updated
        public bool Checked { get; set; } // Checked

        public PermissionAction()
        {
            IsDelete = false;
            Checked = true;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
