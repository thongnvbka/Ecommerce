using System;

namespace Library.DbContext.Entities
{
    // LockHistory
    
    public partial class LockHistory
    {
        public int Id { get; set; } // Id (Primary key)
        public string KeyLock { get; set; } // KeyLock (length: 50)
        public DateTime UpdateTime { get; set; } // UpdateTime
        public long UserId { get; set; } // UserId
        public string UserName { get; set; } // UserName (length: 100)
        public string FullName { get; set; } // FullName (length: 150)
        public byte State { get; set; } // State

        ///<summary>
        /// Lý do unlock
        ///</summary>
        public string ReasonUnlock { get; set; } // ReasonUnlock (length: 1000)
        public int ObjectId { get; set; } // ObjectId
        public string ObjectName { get; set; } // ObjectName (length: 150)
        public bool IsLatest { get; set; } // IsLatest

        public LockHistory()
        {
            UpdateTime = DateTime.Now;
            IsLatest = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
