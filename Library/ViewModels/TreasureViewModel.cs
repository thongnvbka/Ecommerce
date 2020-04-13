 
namespace Library.ViewModels
{
    public class TreasureViewModel
    {
        public int Selected { get; set; }
        public string Path { get; set; }

        public TreasureViewModel()
        {
            Selected = 0;
            Path = "0";
        }
    }
}