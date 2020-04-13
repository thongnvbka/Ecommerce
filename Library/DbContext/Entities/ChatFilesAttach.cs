namespace Library.DbContext.Entities
{
    // ChatFilesAttach
    
    public partial class ChatFilesAttach
    {
        public long Id { get; set; } // Id (Primary key)
        public string GroupId { get; set; } // GroupId (length: 50)
        public long? ChatId { get; set; } // ChatId
        public string FileName { get; set; } // FileName (length: 300)
        public string FileUrl { get; set; } // FileUrl
        public string DownloaderIds { get; set; } // DownloaderIds
        public int? FileSize { get; set; } // FileSize
        public string FileSizeText { get; set; } // FileSizeText (length: 150)
        public string ContentType { get; set; } // ContentType (length: 50)
        public int? DownloadCount { get; set; } // DownloadCount
        public bool IsRemove { get; set; } // IsRemove

        public ChatFilesAttach()
        {
            IsRemove = false;
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
