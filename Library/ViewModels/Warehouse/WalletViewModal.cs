namespace Library.ViewModels
{
    public class WalletSearchModal
    {
        public string Keyword { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}