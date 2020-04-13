namespace Library.ViewModels
{
    public class DebitSearchModal
    {
        // Từ khóa tìm kiếm
        public string Keyword { get; set; }

        // Loại công nợ
        public int Type { get; set; }

        public int SubjectTypeId { get; set; }
        public int SubjectId { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int? FinanceFundId { get; set; }
        public int? TreasureId { get; set; }
        public int? UserId { get; set; }
    }
}