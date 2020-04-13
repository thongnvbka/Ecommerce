using System;

namespace Library.DbContext.Entities
{
    // SendEmail
    
    public partial class SendEmail
    {
        public long Id { get; set; } // Id (Primary key)
        public int FromUserId { get; set; } // FromUserId
        public string Title { get; set; } // Title (length: 500)
        public string FromUserName { get; set; } // FromUserName (length: 100)
        public string FromUserFullName { get; set; } // FromUserFullName (length: 150)
        public string FromUserEmail { get; set; } // FromUserEmail (length: 100)
        public string To { get; set; } // To
        public string Cc { get; set; } // Cc
        public string Bcc { get; set; } // Bcc
        public string Content { get; set; } // Content
        public DateTime CreatedOnDate { get; set; } // CreatedOnDate

        ///<summary>
        /// 0: Pending 1: Scanned
        ///</summary>
        public byte Status { get; set; } // Status
        public DateTime? SendTime { get; set; } // SendTime

        ///<summary>
        /// 0: Email 1: Notification
        ///</summary>
        public byte Type { get; set; } // Type
        public string UnsignName { get; set; } // UnsignName (length: 500)
        public short? AttachmentCount { get; set; } // AttachmentCount
        public string Attachments { get; set; } // Attachments
        public bool IsLock { get; set; } // IsLock
        public byte ErrorNo { get; set; } // ErrorNo
        public DateTime? ErrorLastTime { get; set; } // ErrorLastTime

        public SendEmail()
        {
            CreatedOnDate = DateTime.Now;
            Type = 0;
            IsLock = false;
            ErrorNo = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
