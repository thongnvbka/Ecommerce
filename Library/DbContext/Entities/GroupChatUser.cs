using System;

namespace Library.DbContext.Entities
{
    // GroupChatUsers
    
    public partial class GroupChatUser
    {
        public long Id { get; set; } // Id (Primary key)
        public string GroupId { get; set; } // GroupId (length: 100)
        public int UserId { get; set; } // UserId
        public string FullName { get; set; } // FullName (length: 150)
        public string Image { get; set; } // Image (length: 500)
        public string TitleName { get; set; } // TitleName (length: 350)
        public string OfficeName { get; set; } // OfficeName (length: 350)
        public int InvitedByUserId { get; set; } // InvitedByUserId
        public DateTime JoinTime { get; set; } // JoinTime

        ///<summary>
        /// 0: Đang mời 1: Không đồng ý 2: Đồng ý 3: bị kick khỏi nhóm
        ///</summary>
        public byte InviteStatus { get; set; } // InviteStatus

        ///<summary>
        /// 0: Nhân viên trong công ty 1: Khách hàng
        ///</summary>
        public byte Type { get; set; } // Type

        ///<summary>
        /// 0: Offline 1: Online 2: Invisible 3: Away 4: Busy
        ///</summary>
        public byte Status { get; set; } // Status
        public string NotifyUrl { get; set; } // NotifyUrl (length: 1000)
        public bool IsShowNotify { get; set; } // IsShowNotify

        public GroupChatUser()
        {
            JoinTime = DateTime.Now;
            InviteStatus = 0;
            Type = 0;
            Status = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
