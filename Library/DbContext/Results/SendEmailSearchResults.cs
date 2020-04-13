using System;

namespace Library.DbContext.Results
{
    public class SendEmailSearchResults
    {
        public long Id { get; set; }
        public int FromUserId { get; set; }
        public string Title { get; set; }
        public string FromUserName { get; set; }
        public string FromUserFullName { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public byte Status { get; set; }
        public DateTime? SendTime { get; set; }
        public byte Type { get; set; }
        public string UnsignName { get; set; }
        public short? AttachmentCount { get; set; }
        public string Attachments { get; set; }
        public string FromUserEmail { get; set; }
    }
}
