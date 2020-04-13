namespace Library.ViewModels
{
    public class RechargeBillSearchModal
    {
        public string Keyword { get; set; }
        public int TypeId { get; set; }
        public int CustomerId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int? CustomerWalletId { get; set; }
        public decimal? CurrencyFluctuations { get; set; }
    }
}