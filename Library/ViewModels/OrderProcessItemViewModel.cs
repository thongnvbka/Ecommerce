namespace Library.ViewModels
{
    public class OrderProcessItemSearchModel
    {
        public string Keyword { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int WarehouseId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}