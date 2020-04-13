
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class CustomerInforModel
    {
        public CustomerInforItem CustomerInforItem { get; set;}
        public List<CusInforOrderItem> CusInforOrderItem { get; set; }
        public OrderStatusItem OrderStatusItem { get; set; }
    }
    public class CustomerInforV2Model
    {
        public CustomerInforItem CustomerInforItem { get; set; }
        public CustomerReporItem ReportCountItem { get; set; }
    }
}
