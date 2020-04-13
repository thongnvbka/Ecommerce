using System;

namespace Library.DbContext.Entities
{
    // GroupChatContent
    
    public partial class GroupChatContent
    {
        public long Id { get; set; } // Id (Primary key)
        public string GroupId { get; set; } // GroupId (length: 100)
        public int UserId { get; set; } // UserId
        public string UserName { get; set; } // UserName (length: 250)
        public string FullName { get; set; } // FullName (length: 250)
        public string TitleName { get; set; } // TitleName (length: 300)
        public string OfficeName { get; set; } // OfficeName (length: 300)
        public string Image { get; set; } // Image (length: 500)
        public string Content { get; set; } // Content
        public DateTime SentTime { get; set; } // SentTime

        ///<summary>
        /// Có phải là tin nhắn của hệ thống không.
        ///</summary>
        public bool IsSystem { get; set; } // IsSystem

        ///<summary>
        /// 0: Nhân viên trong công ty 1: Khách hàng
        ///</summary>
        public byte Type { get; set; } // Type
        public bool IsDelete { get; set; } // IsDelete
        public int? AttachmentCount { get; set; } // AttachmentCount
        public int? Like { get; set; } // Like
        public int? Dislike { get; set; } // Dislike
        public int? NumberOfReplies { get; set; } // NumberOfReplies
        public long? ParentId { get; set; } // ParentId

        public GroupChatContent()
        {
            SentTime = DateTime.Now;
            IsSystem = false;
            Type = 0;
            IsDelete = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
