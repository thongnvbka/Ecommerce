using System;
using System.Collections.Generic;
using Common.Helper;

namespace Library.Models
{
    public class SendEmailMeta
    {
        public SendEmailMeta()
        {
            Status = 0;
            Type = 1;
            UnsignName = string.IsNullOrEmpty(Title) ? "" : MyCommon.Ucs2Convert(Title);
        }

        public int? Id { get; set; }
        public int FromUserId { get; set; }
        public string Title { get; set; }
        public string FromUserName { get; set; }
        public string FromUserFullName { get; set; }
        public string FromUserEmail { get; set; }
        public List<UserObject> To { get; set; }
        public List<UserObject> Cc { get; set; }
        public List<UserObject> Bcc { get; set; }
        public string Content { get; set; }
        public byte Status { get; set; }
        public DateTime? CreatedOnDate { get; set; }
        public DateTime? SendTime { get; set; }

        // 0: Email 1: Notification
        public byte Type { get; set; }
        public string UnsignName { get; set; }
        public short? AttachmentCount { get; set; }
        public string Attachments { get; set; }
    }

    public class UserObject
    {
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
