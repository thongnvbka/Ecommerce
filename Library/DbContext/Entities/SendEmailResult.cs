using System;

namespace Library.DbContext.Entities
{
    // SendEmailResult
    
    public partial class SendEmailResult
    {
        public int Id { get; set; } // Id (Primary key)
        public long SendEmailId { get; set; } // Send_Email_Id
        public string SendId { get; set; } // SendId (length: 150)
        public string Email { get; set; } // Email (length: 100)

        ///<summary>
        /// 0: Respone 1: Error
        ///</summary>
        public byte Type { get; set; } // Type
        public string Message { get; set; } // Message
        public int? ErrorCode { get; set; } // ErrorCode
        public DateTime Time { get; set; } // Time
        public string Status { get; set; } // Status (length: 100)

        public SendEmailResult()
        {
            Time = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
