namespace Library.ViewModels
{
    public class FundBillSearchModal
    {
        public string Keyword { get; set; }
        public int TypeId { get; set; }
        public int AccountantSubjectId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int? FinanceFundId { get; set; }
        public int? TreasureId { get; set; }
        public int? UserId { get; set; }
        public string FinanceFundIdPath { get; set; }
        public decimal? CurrencyFluctuations { get; set; }
    }
}