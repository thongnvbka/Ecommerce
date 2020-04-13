namespace Library.DbContext.Entities
{
    // Attachment_Message
    
    public partial class AttachmentMessage
    {
        public long AttachmentId { get; set; } // AttachmentId (Primary key)
        public long MessageId { get; set; } // MessageId (Primary key)
        public bool IsDelete { get; set; } // IsDelete
        public bool IsCanEdit { get; set; } // IsCanEdit

        public AttachmentMessage()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
