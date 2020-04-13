using System.Web.Mvc;

namespace Library.Models
{
    public class MessageMeta
    {
        public long Id { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Title { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public bool IsSendOut { get; set; }
    }
}
