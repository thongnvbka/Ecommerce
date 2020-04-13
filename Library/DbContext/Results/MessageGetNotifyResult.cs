using System;

namespace Library.DbContext.Results
{
    public class MessageGetNotifyResult
    {
        public long Id { get; set; } // Id (Primary key)
        public string FromUser { get; set; } // FromUser (length: 200)
        public string FromAvatar { get; set; } // FromAvatar (length: 500)
        public string Title { get; set; } // Title (length: 500)
        public DateTime? SendTime { get; set; } // SendTime
    }
}
