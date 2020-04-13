using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;

namespace Common.MailHelper
{
    public class MailHelper
    {
        static Thread _sendMailThread;
        public static List<EmailQueue> EmailQueueList = new List<EmailQueue>();

        public static void StartThreadSendMail()
        {
            _sendMailThread = new Thread(new ThreadStart(SendMailbyThread));
            _sendMailThread.Start();
        }

        public class EmailQueue
        {
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsFinished { get; set; }

            public EmailQueue()
            {
                Email = "";
                Subject = "";
                Body = "";
                IsFinished = false;
            }
        }

        public static bool SendMail(string emailto, string subject, string body, bool insertToQueue = true)
        {
            if (insertToQueue)
            {
                EmailQueue emailQueue = new EmailQueue();
                emailQueue.Email = emailto;
                emailQueue.Subject = subject;
                emailQueue.Body = body;
                EmailQueueList.Add(emailQueue);
                return true;
            }
            else
            {
                var displayname = ConfigurationManager.AppSettings["FromEmailDisplayName"];
                var emailAccount = ConfigurationManager.AppSettings["FromEmailAddress"];
                //var emailAdmin = ConfigurationManager.AppSettings["EmailRecive"];
                var passwordAccount = ConfigurationManager.AppSettings["FromEmailPassword"];
                var smtpHost = ConfigurationManager.AppSettings["SMTPHost"];
                var smtpPort = int.Parse(ConfigurationManager.AppSettings["SMTPPort"]);
                var enablessl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSSL"]);

                var smtpServer = new SmtpClient
                {
                    Credentials = new System.Net.NetworkCredential(emailAccount, passwordAccount),
                    Port = smtpPort,
                    Host = smtpHost,
                    EnableSsl = enablessl
                };
                var mail = new MailMessage();

                try
                {
                    mail.From = new MailAddress(emailAccount, displayname, Encoding.UTF8);
                    mail.To.Add(emailto);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    //mail.ReplyTo = new MailAddress(emailAccount);
                    mail.ReplyToList.Add(new MailAddress(emailAccount));
                    mail.Priority = MailPriority.High;
                    mail.IsBodyHtml = true;
                    smtpServer.Send(mail);
                    return true;
                }
                catch (Exception) { return false; }
            }
        }


        //public static void SendEmailOrderActivities(Order orderEntity, Enum.OrderActions action, List<string> mailList, string host)
        //{
        //    string subject = "";
        //    string body = "";
        //    switch (action)
        //    {
        //        case Enum.OrderActions.ChangeStatus:
        //            subject = orderEntity.Title + " change status";
        //            body = "Change status to " + orderEntity.Status;
        //            body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
        //            body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
        //            break;
        //        case Enum.OrderActions.Comment:
        //            subject = "Comment on " + orderEntity.Title;
        //            body = "New comment on " + orderEntity.Title;
        //            body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
        //            body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
        //            break;
        //        case Enum.OrderActions.UpdatePercent:
        //            subject = "Update percent on " + orderEntity.Title + " to " + orderEntity.OrderPercent;
        //            body = "Update pecent to " + orderEntity.OrderPercent;
        //            body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
        //            body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
        //            break;
        //        case Enum.OrderActions.UploadFile:
        //            subject = "Upload file on " + orderEntity.Title;
        //            body = "New File uploaded on " + orderEntity.Title;
        //            body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
        //            body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
        //            break;
        //        default:
        //            break;
        //    }

        //    foreach (var item in mailList)
        //    {
        //        EmailQueue emailQueue = new EmailQueue();
        //        emailQueue.Email = item;
        //        emailQueue.Subject = subject;
        //        emailQueue.Body = body;
        //        EmailQueueList.Add(emailQueue);
        //    }
        //}

        static void SendMailbyThread()
        {
            while (_sendMailThread.IsAlive)
            {
                while (EmailQueueList.Count > 0)
                {
                    try
                    {
                        var item = EmailQueueList.FirstOrDefault(m => !m.IsFinished);
                        if (item == null)
                            break;

                        item.IsFinished = true;
                        SendMail(item.Email, item.Subject, item.Body, false);
                        EmailQueueList.Remove(item);
                        Thread.Sleep(2500);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                Thread.Sleep(2500);
            }

        }
    }
}
