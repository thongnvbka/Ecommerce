using System;

namespace Library.DbContext.Entities
{
    // Message_User
    
    public partial class MessageUser
    {
        public long Id { get; set; } // Id (Primary key)
        public long MessageId { get; set; } // MessageId
        public int UserId { get; set; } // UserId

        ///<summary>
        /// 1: Người gửi, 0: Người nhận
        ///</summary>
        public bool Type { get; set; } // Type
        public bool IsDelete { get; set; } // IsDelete
        public bool IsTrash { get; set; } // IsTrash
        public bool Star { get; set; } // Star
        public bool IsRead { get; set; } // IsRead
        public DateTime? ReadTime { get; set; } // ReadTime

        public MessageUser()
        {
            Type = false;
            Star = false;
            IsRead = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
