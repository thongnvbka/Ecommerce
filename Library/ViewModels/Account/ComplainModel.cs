using System.Collections.Generic;
using Common.Items;
using Library.ViewModels.Items;

namespace Library.ViewModels.Account
{
    public class ComplainModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<ComplainItem> ListItems { get; set; }
        public ComplainStatusItem ComplainStatusItem { get; set; }
    }
}
