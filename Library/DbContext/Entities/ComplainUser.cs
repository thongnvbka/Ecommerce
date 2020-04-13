using System;

namespace Library.DbContext.Entities
{
    // ComplainUser
    public partial class ComplainUser
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
        ///<summary>
        /// Chat nội bộ: 1, Chat với khách hàng: 0
        ///</summary>
        public bool? IsInHouse { get; set; } // IsInHouse
        ///<summary>
        /// Đơn vị nhân viên xử lý
        ///</summary>
        public int? OfficeId { get; set; } // OfficeId

        ///<summary>
        /// Tên đơn vị nhân viên xử lý
        ///</summary>
        public string OfficeName { get; set; } // OfficeName (length: 300)

        ///<summary>
        /// Id Path đơn vị nhân viên
        ///</summary>
        public string OfficeIdPath { get; set; } // OfficeIdPath (length: 300)
        public int? GroupId { get; set; } // GroupId
        /// <summary>
        /// Loại tin nhắn 0-Text, 1-Ảnh, 2-Icon
        /// </summary>
        public byte? CommentType { get; set; } //CommentType
                                               ///<summary>
                                               /// Id hệ thống
                                               ///</summary>
        public int? SystemId { get; set; } // SystemId

      
        public string SystemName { get; set; } // SystemName (length: 100)

        public ComplainUser()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
