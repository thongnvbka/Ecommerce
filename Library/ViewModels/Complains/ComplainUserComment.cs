using System;

namespace Library.ViewModels.Complains
{
    public class ComplainUserComment
    {
        public long Id { get; set; } // Id (Primary key)
        public long ComplainId { get; set; } // ComplainId
        public int? UserId { get; set; } // UserId
        public string Content { get; set; } // Content (length: 2000)
        public string AttachFile { get; set; } // AttachFile (length: 255)
        public DateTime? CreateDate { get; set; } // CreateDate
        public DateTime? UpdateDate { get; set; } // UpdateDate
        public int? UserRequestId { get; set; } // UserRequestId
        public string UserRequestName { get; set; } // UserRequestName (length: 255)
        public int? CustomerId { get; set; } // CustomerId
        public string CustomerName { get; set; } // CustomerName (length: 255)
        public string UserName { get; set; } // UserName (length: 255)

        ///<summary>
        /// Đã đọc hay chưa
        ///</summary>
        public bool? IsRead { get; set; } // IsRead

        ///<summary>
        /// Là người tiếp nhận xử lý chính, đầu tiên
        ///</summary>
        public bool? IsCare { get; set; } // IsCare

        public string UserFullName { get; set; } // UserFullName (length: 255)
        public string UserOffice { get; set; } // UserOffice (length: 255)
        public string UserPhone { get; set; } // UserPhone (length: 255
        public string TypeServerName { get; set; } // TypeServerName (length: 255)
        public int? GroupId { get; set; }
    }
}