namespace Library.ViewModels
{
    public class UserFilterViewModel
    {
        public string Keyword { get; set; }
        public int OfficeId { get; set; }
        public int? TitleId { get; set; }
        public string OfficeIdPath { get; set; }
        public bool HasChilds { get; set; }
    }
}
