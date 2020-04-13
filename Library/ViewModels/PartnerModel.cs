using Common.Items;
using Library.DbContext.Entities;
using System.Collections.Generic;

namespace Library.ViewModels
{
    public class PartnerModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<Partner> ListItems { get; set; }
    }
}