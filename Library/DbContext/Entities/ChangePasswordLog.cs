using System;

namespace Library.DbContext.Entities
{
    // ChangePasswordLog
    
    public partial class ChangePasswordLog
    {
        public long Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string Ip { get; set; } // IP (length: 20)
        public DateTime ChangeTime { get; set; } // ChangeTime
        public string OldPassword { get; set; } // OldPassword (length: 100)

        public ChangePasswordLog()
        {
            ChangeTime = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
