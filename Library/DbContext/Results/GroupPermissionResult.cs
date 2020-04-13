using System;

namespace Library.DbContext.Results
{
    public class GroupPermissionResult
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsSystem { get; set; } // Là nhóm quyền mặc định của hệ thống không thể xóa
        public short AppNo { get; set; }
        public short ModuleNo { get; set; }
        public short UserNo { get; set; }
        public short PageNo { get; set; }
        public DateTime Created { get; set; } // Ngày tạo chức vụ
        public DateTime Updated { get; set; } // Cập nhật gần nhất
    }
}
