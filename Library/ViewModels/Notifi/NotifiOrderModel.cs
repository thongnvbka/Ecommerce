using System;
using System.Collections.Generic;
using Common.Items;
using Library.ViewModels.Items;

namespace Library.ViewModels.Notifi
{
    public class NotifiOrderModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<NotifiOrderItem> ListItems { get; set; }
    }
}
