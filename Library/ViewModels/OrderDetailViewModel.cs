namespace Library.ViewModels
{
    public class OrderDetailViewModel
    {
        public string Keyword { get; set; }
        public string Number { get; set; }

        public string ToPrice { get; set; }

        public string FormPrice { get; set; }

        public OrderDetailViewModel()
        {
            Keyword = "";
            Number = "";
            ToPrice = "";
            FormPrice = "";
        }
    }
}