using Common.Items;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class RechargeModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<RechargeItem> ListItems { get; set; }
        public List<DropdownItem> Wallets { get; set; }
    }
}