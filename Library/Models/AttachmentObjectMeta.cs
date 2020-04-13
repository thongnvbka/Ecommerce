using System;

namespace Library.Models
{
    public class AttachmentObjectMeta
    {
        public int Id { get; set; }
        public long AttachmentId { get; set; }
        public string AttachmentName { get; set; }
        public long ObjectName { get; set; }
        public byte Type { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool IsDelete { get; set; }
        public string Extension { get; set; }
    }
}
