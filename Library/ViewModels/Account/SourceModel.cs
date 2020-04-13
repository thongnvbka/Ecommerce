using Common.Items;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class SourceModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<SourceItem> ListItems { get; set; }
        public SourceStatusItem StatusItem { get; set; }
    }
}
