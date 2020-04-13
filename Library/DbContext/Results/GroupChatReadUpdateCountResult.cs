namespace Library.DbContext.Results
{
    public class GroupChatReadUpdateCountResult
    {
        public int? UserId { get; set; }
        public byte? Type { get; set; }
        public bool? IsShowNotify { get; set; }
        public string NotifyUrl { get; set; }
    }
}
