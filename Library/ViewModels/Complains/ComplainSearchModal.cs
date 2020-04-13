namespace Library.ViewModels.Complains
{
    public class ComplainSearchModal
    {
        public string Keyword { get; set; }//key tim kiem
        public int UserId { get; set; }
        public int Status { get; set; }//trang thai cua khieu nai
        public int SystemId { get; set; }//thuoc he thong nao
        public string CountryId { get; set; }//quoc gia nao
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
    }
}