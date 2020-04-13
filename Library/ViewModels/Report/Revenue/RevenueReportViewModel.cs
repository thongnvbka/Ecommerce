namespace Library.ViewModels.Report.Revenue
{
    public class RevenueReportSearchModel
    {
        public string Keyword { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int CustomerStatus { get; set; }
    }
}