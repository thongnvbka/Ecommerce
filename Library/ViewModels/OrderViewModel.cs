using System.Collections.Generic;
using System.Web.Mvc;

namespace Library.ViewModels
{
    public class OrderViewModel
    {
        public List<SelectListItem> SystemIdList { get; set; }
        public List<SelectListItem> StatusList { get; set; }

        public string Keyword { get; set; }
        public int SystemId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }

        public OrderViewModel()
        {
            Keyword = string.Empty;
            SystemId = -1;
            Status = -1;
            DateStart = string.Empty;
            DateEnd = string.Empty;
        }
    }
}