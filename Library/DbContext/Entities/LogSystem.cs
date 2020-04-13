using System;

namespace Library.DbContext.Entities
{
    // LogSystem
    
    public partial class LogSystem
    {
        public long Id { get; set; } // Id (Primary key)
        public byte LogType { get; set; } // LogType
        public string ShortMessage { get; set; } // ShortMessage (length: 300)
        public string FullMessage { get; set; } // FullMessage
        public string UserName { get; set; } // UserName (length: 50)
        public string FullName { get; set; } // FullName (length: 150)
        public string UnsignedName { get; set; } // UnsignedName (length: 500)
        public string SesstionId { get; set; } // SesstionId (length: 50)
        public string Ip { get; set; } // Ip (length: 20)
        public string Os { get; set; } // Os (length: 200)
        public string Broswser { get; set; } // Broswser (length: 500)
        public int? Version { get; set; } // Version
        public string RequestJson { get; set; } // RequestJson
        public string ObjectJson { get; set; } // ObjectJson
        public DateTime CreatedTime { get; set; } // CreatedTime

        public LogSystem()
        {
            CreatedTime = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
