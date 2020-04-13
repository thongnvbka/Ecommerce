using System.Collections.Generic;
using Library.DbContext.Results;

namespace Library.ViewModels.TemplateEmailJob
{
    public class OverDaysOrderViewModel
    {
        public List<OrderRiskResult> Orders { get; set; }
        public int Day { get; set; }
    }

    public class OverDaysOrderViewModel1
    {
        public List<OrderPackageOverDayResult> Orders { get; set; }
        public int Day { get; set; }
    }
}
