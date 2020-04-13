using System;

namespace Library.DbContext.Results
{
    public class AttachmentObjectGetResult
    {
        public string Extension { get; set; }
        public string AttachmentName { get; set; }
        public int Size { get; set; }
        public string SizeString { get; set; }
        public string AttachmentPath { get; set; }
        public long Id { get; set; }
        public long AttachmentId { get; set; }
        public long ObjectId { get; set; }
        public byte Type { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public bool IsDelete { get; set; }
    }
}
