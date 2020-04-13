namespace Library.Models
{
    public class UserReplyMeta
    {
        public int UserId { get; set; }

        public byte Type { get; set; }

        public string NotifyUrl { get; set; }

        public UserReplyMeta()
        {

        }

        public UserReplyMeta(int userId, byte type, string notifyUrl)
        {
            UserId = userId;
            Type = type;
            NotifyUrl = notifyUrl;
        }
    }
}
