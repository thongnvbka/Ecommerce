using Library.DbContext.Entities;
using Library.ViewModels.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Account
{
    public class SourceDetailModel
    {
        public List<SourceSupplierItem> ListSourceSupplier { get; set; }

        public SourceDetailItem SourceDetailItem { get; set; }

        public SourceProductItem SourceProductItem { get; set; }

        public List<OrderComment> ListOrderComment { get; set; }
    }
}
