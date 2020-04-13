namespace Library.ViewModels
{
    public class PackingListSearchModal
    {
        public string Keyword { get; set; }

        public int WarehouseSourceId { get; set; }
        public int WarehouseDesId { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}