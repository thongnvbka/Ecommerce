using Common.Items;
using System.Collections.Generic;

namespace Library.ViewModels.Report
{
    public class ReportBusinessModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
        public List<ReportBusinessItem> ListItems { get; set; }
    }
}
