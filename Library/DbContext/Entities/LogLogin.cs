using System;

namespace Library.DbContext.Entities
{
    // LogLogin
    
    public partial class LogLogin
    {

        ///<summary>
        /// Id tự tăng
        ///</summary>
        public int Id { get; set; } // Id (Primary key)
        public string UserName { get; set; } // UserName (length: 50)
        public string FullName { get; set; } // FullName (length: 50)
        public string UnsignName { get; set; } // UnsignName (length: 100)

        ///<summary>
        /// Thời gian login
        ///</summary>
        public DateTime LoginTime { get; set; } // LoginTime

        ///<summary>
        /// IP đăng nhập
        ///</summary>
        public string Ip { get; set; } // IP (length: 30)

        ///<summary>
        /// Token truy cập
        ///</summary>
        public string Token { get; set; } // Token (length: 1000)

        ///<summary>
        /// Hệ điều hành
        ///</summary>
        public string Os { get; set; } // OS (length: 300)

        ///<summary>
        /// Trình duyệt
        ///</summary>
        public string Browser { get; set; } // Browser (length: 400)

        ///<summary>
        /// Phiên bản trình duyệt
        ///</summary>
        public string Version { get; set; } // Version (length: 300)

        ///<summary>
        /// Thời gian đăng xuất
        ///</summary>
        public DateTime? LogoutTime { get; set; } // LogoutTime

        public LogLogin()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
