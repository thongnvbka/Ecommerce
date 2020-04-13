using Common.Items;

namespace Library.ViewModels.Counting
{
    public class CountingCommonModel
    {
        public PageItem Page { get; set; }
        public SearchInfor Search { get; set; }
    }

    public class CountingSearchModel
    {
        public string Keyword { get; set; }
        public int Status { get; set; }
        public string DateStart { get; set; }
        public string DateEnd { get; set; }
        public int SystemId { get; set; }
    }
}