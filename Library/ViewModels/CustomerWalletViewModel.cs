namespace Library.ViewModels
{
    public class CustomerWalletViewModel
    {
        public int Selected { get; set; }
        public string Path { get; set; }

        public CustomerWalletViewModel()
        {
            Selected = 0;
            Path = "0";
        }
    }
}