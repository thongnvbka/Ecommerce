using System;

namespace Library.DbContext.Entities
{
    // CustomerLog

    public partial class CustomerLog
    {
        public int Id { get; set; } // Id (Primary key)
        public int SystemId { get; set; } // SystemId
        public string SystemName { get; set; } // SystemName (length: 100)
        public string Email { get; set; } // Email (length: 50)
        public string FullName { get; set; } // FullName (length: 50)
        public byte? Type { get; set; } // Type
        public string DataBefore { get; set; } // DataBefore
        public string DataAfter { get; set; } // DataAfter
        public byte? DataType { get; set; } // DataType
        public string LogContent { get; set; } // LogContent

        ///<summary>
        /// Thời gian login
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

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

        public CustomerLog()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
