namespace Library.ViewModels.Customer
{
    public class PotentialCustomerSearchModal
    {
        public string Keyword { get; set; }
        public int GenderId { get; set; }
        public int UserId { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int SystemId { get; set; }
        public int CustomerType { get; set; }
    }
}