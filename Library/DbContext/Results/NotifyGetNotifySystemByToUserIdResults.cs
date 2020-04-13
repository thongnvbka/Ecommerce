using System;

namespace Library.DbContext.Results
{
    public partial class NotifyGetNotifySystemByToUserIdResults
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public byte Type { get; set; }
        public DateTime SendTime { get; set; }
        public bool IsRead { get; set; }
        public string Content { get; set; }
        public string Url { get; set; }
    }

}
