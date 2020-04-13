namespace Library.DbContext.Results
{
    public class AttachmentsGetByMessageIdResults
    {
        public long Id { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentPath { get; set; }
        public string Extension { get; set; }
        public int Size { get; set; }
        public string SizeString { get; set; }
        public System.DateTime CreatedOnDate { get; set; }
    }
}
