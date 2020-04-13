namespace Library.DbContext.Results
{
    public class GroupChatContentGetListRepliesResult
    {
        public long Id { get; set; }
        public string GroupId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string TitleName { get; set; }
        public string OfficeName { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public System.DateTime SentTime { get; set; }
        public bool IsSystem { get; set; }
        public byte Type { get; set; }
        public bool IsDelete { get; set; }
        public int? AttachmentCount { get; set; }
        public int? Like { get; set; }
        public int? Dislike { get; set; }
        public int? NumberOfReplies { get; set; }
        public long? ParentId { get; set; }
        public int Liked { get; set; }
    }
}
