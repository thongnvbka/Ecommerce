using System;

namespace Library.DbContext.Entities
{
    // MessageRealTime
    
    public partial class MessageRealTime
    {
        public long Id { get; set; } // Id (Primary key)
        public string FromUser { get; set; } // FromUser (length: 200)
        public long FromUserId { get; set; } // FromUserId
        public string FromAvatar { get; set; } // FromAvatar (length: 500)
        public string ToUser { get; set; } // ToUser
        public string CcToUser { get; set; } // CcToUser
        public string BccToUser { get; set; } // BccToUser
        public string Title { get; set; } // Title (length: 500)
        public string UnsignTitle { get; set; } // UnsignTitle
        public string Content { get; set; } // Content
        public short AttachmentCount { get; set; } // AttachmentCount
        public DateTime? SendTime { get; set; } // SendTime
        public DateTime LastModifiedOnDate { get; set; } // LastModifiedOnDate

        public MessageRealTime()
        {
            AttachmentCount = 0;
            SendTime = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
