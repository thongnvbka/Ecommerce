using System;
using System.Collections.Generic;
using Common.Items;
using Library.ViewModels.Items;

namespace Library.ViewModels.Notifi
{
    public class NotifiCommonModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<NotifiCommonItem> ListItems { get; set; }
    }

    public class NotifiSearchModel
    {
        public string Keyword { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int SystemId { get; set; }
    }

    public class NotifiMetaModel
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
        /// Tiêu đề thông báo
        ///</summary>
        public string Title { get; set; } // Title (length: 255)

        ///<summary>
        /// Trạng thái:True: hiển thị, False: không hiển thị
        ///</summary>
        public bool Status { get; set; } // Status
    }
}
