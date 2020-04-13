using Library.DbContext.Entities;
using Library.ViewModels.Items;
using System.Collections.Generic;


namespace Library.ViewModels.Account
{
    public class DepositDetailModel
    {
        public List<DepositDetailViewItem> ListDetail { get; set; }

        public DepositViewItem DepositViewItem { get; set; }

        public List<OrderComment> ListOrderComment { get; set; }
        public List<OrderServiceItem> ListOrderService { get; set; }

        public OrderExchange OrderExchange { get; set; }
        public List<RechargeBill> RechargeBill { get; set; }
        public List<Complain> Complains { get; set; }
        public List<OrderPackageItem> OrderPackages { get; set; }
    }
}
