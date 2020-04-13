using Common.Items;
using System.Collections.Generic;


namespace Library.ViewModels.Account
{
    public class OrderExhibitionModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<OrderItem> ListItems { get; set; }
        public OrderStatusItem OrderStatusItem { get; set; }
    }
}
