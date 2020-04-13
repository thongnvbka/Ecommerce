using Library.DbContext.Entities;
using Library.ViewModels.Items;
namespace Library.ViewModels.Account
{
    public class SourceAddModel
    {
        public Source Source { get; set;}
        public SourceDetail SourceDetail { get; set; }
        public SourceServiceItem SourceServiceItem { get; set; }
    }
}
