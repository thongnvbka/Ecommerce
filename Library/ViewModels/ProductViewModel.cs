namespace Library.ViewModels
{
    public class ProductViewModel
    {
        public string Keyword { get; set; }
        public string Number { get; set; }
        public int CategoryId { get; set; }

        public ProductViewModel()
        {
            Keyword = string.Empty;
            Number = string.Empty;
            CategoryId = -1;
        }
    }
}
