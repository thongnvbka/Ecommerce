namespace Library.ViewModels
{
    public class PayReceivableModel
    {
        public int Selected { get; set; }
        public string Path { get; set; }

        public PayReceivableModel()
        {
            Selected = 0;
            Path = "0";
        }
    }
}