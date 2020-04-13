namespace Library.ViewModels.Complains
{
    public class ClaimForRefundSearchModal
    {
        public string Keyword { get; set; } //key tim kiem
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; } //trang thai cua khieu nai
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}