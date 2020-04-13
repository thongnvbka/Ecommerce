using System.Collections.Generic;
using System.Web.Mvc;

namespace Library.ViewModels
{
    public class WarehouseViewModel
    {
        private List<SelectListItem> _ListCountry = new List<SelectListItem>()
        {
            new SelectListItem {Text = @"All", Value = "All", Selected = true },
            new SelectListItem {Text = @"VietNam", Value = "Việt Nam" },
            new SelectListItem {Text = @"China", Value = "Trung Quốc" }
        };

        private List<SelectListItem> _ListStatus = new List<SelectListItem>()
        {
            new SelectListItem {Text = @"All", Value = "-1", Selected = true },
            new SelectListItem {Text = @"activated", Value = "1" },
            new SelectListItem {Text = @"Not activated", Value = "2" }
        };

        public string Keyword { get; set; }
        public int Status { get; set; }
        public string Country { get; set; }
        public int UserId { get; set; }

        public List<SelectListItem> ListCountry { get; }
        public List<SelectListItem> ListStatus { get; }
        public List<SelectListItem> ListUser { get; set; }

        public WarehouseViewModel()
        {
            Keyword = string.Empty;
            Status = -1;
            Country = "All";
            UserId = -1;
            ListStatus = _ListStatus;
            ListCountry = _ListCountry;
            ListUser = null;
        }
    }
}