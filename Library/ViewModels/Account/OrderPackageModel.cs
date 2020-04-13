using Common.Items;
using Library.ViewModels.Items;
using System.Collections.Generic;
namespace Library.ViewModels.Account
{
    public class OrderPackageModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<OrderPackageListItem> ListItems { get; set; }
        public OrderPackageStatusItem OrderStatusItem { get; set; }
    }
}
