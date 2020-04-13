using System;

namespace Library.DbContext.Entities
{
    // Attachment
    
    public partial class Attachment
    {
        public long Id { get; set; } // Id (Primary key)
        public string AttachmentName { get; set; } // AttachmentName (length: 100)
        public string AttachmentPath { get; set; } // AttachmentPath (length: 500)
        public string Type { get; set; } // Type (length: 50)
        public string TypeEn { get; set; } // TypeEn (length: 50)
        public string Extension { get; set; } // Extension (length: 10)
        public int Size { get; set; } // Size
        public string SizeString { get; set; } // SizeString (length: 20)
        public DateTime CreatedOnDate { get; set; } // CreatedOnDate
        public long? UploaderId { get; set; } // UploaderId
        public string UploaderFullName { get; set; } // UploaderFullName (length: 150)

        public Attachment()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

    public class Attachment1
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Type { get; set; }
        public string TypeEn { get; set; }
        public string SizeString { get; set; }
        public int SizeByte { get; set; }
        public string Url { get; set; }
        public long UploaderId { get; set; }
        public string UploaderName { get; set; }
        public DateTime Created { get; set; }
    }

}
