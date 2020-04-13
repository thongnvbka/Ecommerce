using System;

namespace Library.DbContext.Results
{
    public class PageResult
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short ModuleId { get; set; }
        public string ModuleName { get; set; }
        public byte AppId { get; set; }
        public string AppName { get; set; }
        public string Icon { get; set; }
        public DateTime Created { get; set; } // Ngày tạo chức vụ
        public int OrderNo { get; set; }
        public string Url { get; set; }
        public bool ShowInMenu { get; set; }
    }
}
