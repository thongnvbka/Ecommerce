using System;

namespace Library.ViewModels.Items
{
    public class NotifiCommonItem
    {
        ///<summary>
        /// Mô tả chi tiết
        ///</summary>
        public string Description { get; set; } // Description (length: 1000)

        ///<summary>
        /// Ngày tạo
        ///</summary>
        public DateTime CreateDate { get; set; } // CreateDate
        
        ///<summary>
        /// Tiêu đề thông báo
        ///</summary>
        public string Title { get; set; } // Title (length: 255)

        ///<summary>
        /// Tên người tạo thông báo
        ///</summary>
        public string UserName { get; set; } // UserName (length: 90)
        ///<summary>
        /// Url đường dẫn
        ///</summary>
        public string Url { get; set; } // Url (length: 90)
    }
}
