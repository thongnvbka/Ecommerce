using System;

namespace Library.DbContext.Results
{
    public class AppResult
    {
        public byte Id { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; } // Ngày tạo chức vụ
        public int OrderNo { get; set; }
        public string Url { get; set; }
    }
}
