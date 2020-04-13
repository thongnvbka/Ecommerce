namespace Library.DbContext.Entities
{
    // UserConnection
    
    public partial class UserConnection
    {
        public string ConnectionId { get; set; } // ConnectionId (Primary key) (length: 50)
        public int UserId { get; set; } // UserId (Primary key)
        public string SessionId { get; set; } // Session_ID (length: 150)
        public string UserName { get; set; } // UserName (length: 200)
        public int? OfficeId { get; set; } // OfficeID
        public string OfficeName { get; set; } // OfficeName (length: 250)
        public string TitleName { get; set; } // TitleName
        public string FullName { get; set; } // FullName (length: 250)
        public string Image { get; set; } // Image
        public int? Platform { get; set; } // Platform
        public string UnsignName { get; set; } // UnsignName

        ///<summary>
        /// 0: Nhân viên 1: Khách hàng
        ///</summary>
        public byte? UserType { get; set; } // UserType

        public UserConnection()
        {
            UserType = 0;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
