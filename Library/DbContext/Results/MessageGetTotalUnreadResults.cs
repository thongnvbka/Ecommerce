namespace Library.DbContext.Results
{
    public partial class MessageGetTotalUnreadResults
    {
        public int? TotalInboxUnread { get; set; }
        public int? TotalStarUnread { get; set; }
        public int? TotalDraft { get; set; }
    }

}
