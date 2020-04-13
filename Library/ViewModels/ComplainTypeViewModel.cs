namespace Library.ViewModels
{
    public class ComplainTypeViewModel
    {
        public int Selected { get; set; }
        public string Path { get; set; }

        public ComplainTypeViewModel()
        {
            Selected = 0;
            Path = "0";
        }
    }
}