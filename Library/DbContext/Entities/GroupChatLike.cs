using System;

namespace Library.DbContext.Entities
{
    // GroupChatLike
    
    public partial class GroupChatLike
    {
        public long Id { get; set; } // Id (Primary key)
        public long ContentId { get; set; } // ContentId
        public string GroupId { get; set; } // GroupId (length: 100)
        public int UserId { get; set; } // UserId
        public string UserName { get; set; } // UserName (length: 100)
        public string FullName { get; set; } // FullName (length: 150)
        public string Image { get; set; } // Image (length: 500)

        ///<summary>
        /// 0: Nhân viên trong công ty 1: Khách hàng
        ///</summary>
        public byte UserType { get; set; } // UserType
        public bool IsLike { get; set; } // IsLike
        public DateTime CreatedOnDate { get; set; } // CreatedOnDate

        public GroupChatLike()
        {
            CreatedOnDate = DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
