using System;

namespace Library.DbContext.Entities
{
    // CustomerCallHistory
    
    public partial class CustomerCallHistory
    {
        public int Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id nhân viên gọi điện
        ///</summary>
        public int UserId { get; set; } // UserId

        ///<summary>
        /// Họ và tên của nhân viên gọi điện
        ///</summary>
        public string UserFullName { get; set; } // UserFullName (length: 300)

        ///<summary>
        /// UserName nhân viên gọi điện
        ///</summary>
        public string UserName { get; set; } // UserName (length: 30)

        ///<summary>
        /// Id chức vụ nhân viên gọi
        ///</summary>
        public int TitleId { get; set; } // TitleId

        ///<summary>
        /// Tên chức vụ nhân viên gọi
        ///</summary>
        public string TitleName { get; set; } // TitleName (length: 300)

        ///<summary>
        /// Id đơn vị của nhân viên gọi
        ///</summary>
        public int OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên nhân viên gọi
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// IdPath đơn vị của nhân viên gọi
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 600)

        ///<summary>
        /// OfficeNamePath của nhân viên gọi
        ///</summary>
        public string OfficeNamePath { get; set; } // OfficeNamePath (length: 600)

        ///<summary>
        /// Id Khách hàng
        ///</summary>
        public int CustomerId { get; set; } // CustomerId

        ///<summary>
        /// Email khách hàng
        ///</summary>
        public string CustomerEmail { get; set; } // CustomerEmail (length: 300)
        public string CustomerName { get; set; } // CustomerName (length: 300)

        ///<summary>
        /// Điện thoại khách hàng
        ///</summary>
        public string CustomerPhone { get; set; } // CustomerPhone (length: 300)

        ///<summary>
        /// Id Vip của khách hàng
        ///</summary>
        public byte CustomerVipId { get; set; } // CustomerVipId

        ///<summary>
        /// Tên Vip của khách hàng
        ///</summary>
        public string CustomerVipName { get; set; } // CustomerVipName (length: 300)

        ///<summary>
        /// Loại gọi điện: 0: Gọi điện giao hàng
        ///</summary>
        public byte Mode { get; set; } // Mode

        ///<summary>
        /// Là lần gọi điện cuối cùng của khách theo mode
        ///</summary>
        public bool IsLast { get; set; } // IsLast

        ///<summary>
        /// ObjectId là Id đối tượng tùy theo mode
        ///</summary>
        public int? ObjectId { get; set; } // ObjectId

        ///<summary>
        /// Nội dung trong lần gọi điện
        ///</summary>
        public string Content { get; set; } // Content

        ///<summary>
        /// Thời gian goi cho khách hàng
        ///</summary>
        public DateTime Created { get; set; } // Created

        public CustomerCallHistory()
        {
            IsLast = true;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
