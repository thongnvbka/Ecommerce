using System;

namespace Library.DbContext.Entities
{

    // NotifiCommon

    public partial class NotifiCommon
    {

        ///<summary>
        /// Id bảng thông báo chung của hệ thống. Khi insert bảng này cần insert bản notifi customer với các khách hàng cùng hệ thống (systemid)
        ///</summary>
        public long Id { get; set; } // Id (Primary key)

        ///<summary>
        /// Id hệ thống
        ///</summary>
        public int SystemId { get; set; } // SystemId

        ///<summary>
        /// Tên hệ thống
        ///</summary>
        public string SystemName { get; set; } // SystemName (length: 200)

        ///<summary>
        /// Mô tả chi tiết
        ///</summary>
        public string Description { get; set; } // Description (length: 1073741823)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate

        ///<summary>
        /// Ngày cập nhật
        ///</summary>
        public DateTime? UpdateDate { get; set; } // UpdateDate

        ///<summary>
        /// Đã đọc hay chưa
        ///</summary>
        public bool IsRead { get; set; } // IsRead

        ///<summary>
        /// Tiêu đề thông báo
        ///</summary>
        public string Title { get; set; } // Title (length: 255)

        ///<summary>
        /// Mã người tạo
        ///</summary>
        public int? UserId { get; set; } // UserId

        ///<summary>
        /// Tên người tạo thông báo
        ///</summary>
        public string UserName { get; set; } // UserName (length: 90)
        public string Url { get; set; } // Url (length: 255)

        ///<summary>
        /// Trạng thái:True: hiển thị, False: không hiển thị
        ///</summary>
        public bool Status { get; set; } // Status

        ///<summary>
        /// Ngày xuất bản
        ///</summary>
        public DateTime? PublishDate { get; set; } // PublishDate
        public string ImagePath { get; set; } // ImagePath (length: 255)
        public NotifiCommon()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
