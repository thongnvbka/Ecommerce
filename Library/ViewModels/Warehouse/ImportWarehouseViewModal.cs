namespace Library.ViewModels
{
    public class ImportWarehouseViewModal
    {
        public string Keyword { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }

        public ImportWarehouseViewModal()
        {
            Keyword = string.Empty;
            UserId = -1;
            WarehouseId = -1;
            Status = -1;
            DateStart = string.Empty;
            DateEnd = string.Empty;
        }
    }

    public class ImportWarehouseSearchModal
    {
        public string Keyword { get; set; }
        public int UserId { get; set; }
        public int WarehouseId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}