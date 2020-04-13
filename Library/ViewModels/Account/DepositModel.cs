using Common.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class DepositModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<DepositItem> ListItems { get; set; }
        public DepositStatusItem DepositStatusItem { get; set; }
    }
}
