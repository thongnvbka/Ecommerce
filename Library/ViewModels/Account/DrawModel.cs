using System;
using System.Collections.Generic;
using Common.Items;
using Library.ViewModels.Items;

namespace Library.ViewModels.Account
{
    public class DrawModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<DrawItem> ListItems { get; set; }
    }
}
