using System;

namespace Library.DbContext.Entities
{
    // LogAction
    
    public partial class LogAction
    {
        public long Id { get; set; } // Id (Primary key)
        public long RecordId { get; set; } // RecordId
        public string UserName { get; set; } // UserName (length: 50)
        public string FullName { get; set; } // FullName (length: 150)
        public string UnsignedName { get; set; } // UnsignedName (length: 500)
        public DateTime ActionTime { get; set; } // ActionTime
        public string Content { get; set; } // Content (length: 500)
        public string SessionId { get; set; } // SessionId (length: 300)
        public string Ip { get; set; } // Ip (length: 20)
        public string Os { get; set; } // Os (length: 200)
        public int? Version { get; set; } // Version
        public string TableName { get; set; } // TableName (length: 50)
        public string ActionName { get; set; } // ActionName (length: 50)
        public string OldRecord { get; set; } // OldRecord
        public string NewRecord { get; set; } // NewRecord
        public string CompareRecord { get; set; } // CompareRecord

        public LogAction()
        {
            ActionTime = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
