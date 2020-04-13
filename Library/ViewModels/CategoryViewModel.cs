using System.Collections.Generic;
using System.Web.Mvc;

namespace Library.ViewModels
{
    public class CategoryViewModel
    {
        private List<SelectListItem> _ListItems = new List<SelectListItem>()
        {
            new SelectListItem {Text = @"All", Value = "-1", Selected = true },
            new SelectListItem {Text = @"Activated", Value = ((int)Common.Emums.CategoryStatus.Active).ToString() },
            new SelectListItem {Text = @"Not activated", Value = ((int)Common.Emums.CategoryStatus.NoActive).ToString() }
        };

        public string Keyword { get; set; }
        public int Status { get; set; }

        public List<SelectListItem> ListItems { get; }

        public CategoryViewModel()
        {
            Keyword = string.Empty;
            Status = -1;
            ListItems = _ListItems;
        }
    }
}
