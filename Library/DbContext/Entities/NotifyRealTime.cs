using System;

namespace Library.DbContext.Entities
{
    // NotifyRealTime
    
    public partial class NotifyRealTime
    {
        public long Id { get; set; } // Id (Primary key)
        public int FromUserId { get; set; } // FromUserId
        public int ToUserId { get; set; } // ToUserId
        public string Title { get; set; } // Title (length: 160)
        public string Avatar { get; set; } // Avatar (length: 500)
        public string Content { get; set; } // Content (length: 1073741823)
        public DateTime SendTime { get; set; } // SendTime

        ///<summary>
        /// 0: cảnh báo, 1: thông tin, 2: Cảnh báo của khách hàng, 3: Thông tin khách hàng
        ///</summary>
        public byte Type { get; set; } // Type
        public bool IsRead { get; set; } // IsRead
        public string UnsignName { get; set; } // UnsignName (length: 500)
        public string Url { get; set; } // Url (length: 500)
        public string Group { get; set; } // Group (length: 150)

        public NotifyRealTime()
        {
            FromUserId = 0;
            SendTime = DateTime.Now;
            Type = 0;
            IsRead = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
