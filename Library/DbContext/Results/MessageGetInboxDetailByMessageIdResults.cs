using System;

namespace Library.DbContext.Results
{
    public class MessageGetInboxDetailByMessageIdResults
    {
        public long Id { get; set; }
        public string FromUser { get; set; }
        public long FromUserId { get; set; }
        public string FromAvatar { get; set; }
        public string ToUser { get; set; }
        public string CcToUser { get; set; }
        public DateTime LastModifiedOnDate { get; set; }
        public string Title { get; set; }
        public short AttachmentCount { get; set; }
        public DateTime? SendTime { get; set; }
        public bool Star { get; set; }
        public bool IsRead { get; set; }
        public bool Type { get; set; }
    }
}
