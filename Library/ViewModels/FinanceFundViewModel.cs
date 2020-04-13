namespace Library.ViewModels
{
    public class FinanceFundViewModel
    {
        public int Selected { get; set; }
        public string Path { get; set; }

        public FinanceFundViewModel()
        {
            Selected = 0;
            Path = "0";
        }
    }
}