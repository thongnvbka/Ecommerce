using System;

namespace Library.Models
{
    public class AttachmentMeta
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Type { get; set; }
        public string TypeEn { get; set; }
        public string Size { get; set; }
        public int SizeByte { get; set; }
        public string Url { get; set; }
        public long UploaderId { get; set; }
        public string UploaderName { get; set; }
        public DateTime Created { get; set; }
        public bool IsDelete { get; set; }
    }
}
