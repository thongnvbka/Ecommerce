using System;
using System.Linq;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;
using Library.UnitOfWork;
using Newtonsoft.Json;

namespace Library.DbContext.Repositories
{
    public class SendEmailRepository : Repository<SendEmail>
    {
        public SendEmailRepository(ProjectXContext context) : base(context)
        {
        }

        public long SendEmailInsert(SendEmailMeta sendEmail)
        {
            if (sendEmail.To == null)
            {
                throw new ArgumentNullException("toUser");
            }
            if (sendEmail.Title == null)
            {
                throw new ArgumentNullException("title");
            }
            if (sendEmail.Content == null)
            {
                throw new ArgumentNullException("content");
            }

            var encryptContent = sendEmail.Content;
            var to = JsonConvert.SerializeObject(sendEmail.To);
            var cc = JsonConvert.SerializeObject(sendEmail.Cc);
            var bcc = JsonConvert.SerializeObject(sendEmail.Bcc);

            if (
                Db.SendEmails.Any(
                    x =>
                        x.UnsignName == sendEmail.UnsignName && x.FromUserId == sendEmail.FromUserId &&
                        x.To == to && x.Content == encryptContent &&
                        x.Status == 0))
            {
                return -1;
            }

            var addSendEmail = new SendEmail()
            {
                Title = sendEmail.Title,
                FromUserId = sendEmail.FromUserId,
                FromUserName = sendEmail.FromUserName,
                FromUserFullName = sendEmail.FromUserFullName,
                To = to,
                Cc = cc,
                Bcc = bcc,
                Content = encryptContent,
                Status = sendEmail.Status,
                Type = sendEmail.Type,
                UnsignName = sendEmail.UnsignName,
                Attachments = sendEmail.Attachments,
                AttachmentCount = sendEmail.AttachmentCount,
                FromUserEmail = sendEmail.FromUserEmail,
                CreatedOnDate = DateTime.Now,
            };

            Db.SendEmails.Add(addSendEmail);

            Db.SaveChanges();

            return addSendEmail.Id;
        }

        public long SendEmailInsert(SendEmailMeta sendEmail, NotificationType notificationType)
        {
            if (sendEmail.To == null)
            {
                throw new ArgumentNullException("toUser");
            }
            if (sendEmail.Title == null)
            {
                throw new ArgumentNullException("title");
            }
            if (sendEmail.Content == null)
            {
                throw new ArgumentNullException("content");
            }

            var titleType = EnumHelper.GetEnumDescription(notificationType);

            var title = $"{titleType} {sendEmail.Title}";

            var encryptContent = sendEmail.Content;
            var to = JsonConvert.SerializeObject(sendEmail.To);
            var cc = JsonConvert.SerializeObject(sendEmail.Cc);
            var bcc = JsonConvert.SerializeObject(sendEmail.Bcc);

            var addSendEmail = new SendEmail()
            {
                Title = title,
                FromUserId = sendEmail.FromUserId,
                FromUserName = sendEmail.FromUserName,
                FromUserFullName = sendEmail.FromUserFullName,
                To = to,
                Cc = cc,
                Bcc = bcc,
                Content = encryptContent,
                Status = sendEmail.Status,
                Type = sendEmail.Type,
                UnsignName = sendEmail.UnsignName,
                Attachments = sendEmail.Attachments,
                AttachmentCount = sendEmail.AttachmentCount,
                FromUserEmail = sendEmail.FromUserEmail,
                CreatedOnDate = DateTime.Now,
            };

            Db.SendEmails.Add(addSendEmail);

            Db.SaveChanges();

            return addSendEmail.Id;
        }
    }
}
