namespace Library.DbContext.Entities
{
    // GroupChatRead
    
    public partial class GroupChatRead
    {
        public long Id { get; set; } // Id (Primary key)
        public string GroupId { get; set; } // GroupId (length: 100)
        public int UserId { get; set; } // UserId
        public bool IsRead { get; set; } // IsRead
        public int? Quantity { get; set; } // Quantity
        public long? FromChatId { get; set; } // FromChatId

        ///<summary>
        /// 0: Nhân viên trong công ty 1: Khách hàng
        ///</summary>
        public byte UserType { get; set; } // UserType

        public GroupChatRead()
        {
            UserType = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
