using Common.Items;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class DebitHistoryModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<DebitItem> ListItems { get; set; }
    }
    public class DebitHistoryModelV2
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<DebitItemV2> ListItems { get; set; }
    }
}
