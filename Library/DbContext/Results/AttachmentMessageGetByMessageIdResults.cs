using System;

namespace Library.DbContext.Results
{
    public class AttachmentMessageGetByMessageIdResults
    {
        public long Id { get; set; }
        public string AttachmentName { get; set; }
        public int Size { get; set; }
        public string SizeString { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedOnDate { get; set; }
    }
}
