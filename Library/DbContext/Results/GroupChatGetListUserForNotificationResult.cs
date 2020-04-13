namespace Library.DbContext.Results
{
    public class GroupChatGetListUserForNotificationResult
    {
        public int UserId { get; set; }
        public byte? UserType { get; set; }
        public string NotifyUrl { get; set; }
    }
}
