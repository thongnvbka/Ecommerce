using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Xml;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class AttachmentRepository : Repository<Attachment>
    {
        public AttachmentRepository(ProjectXContext context) : base(context)
        {
        }

        public long InsertForMessage(string attachmentName, string attachmentPath, string extension, int? size, string sizeString, long objectId, long userId)
        {
            var message =
                Db.MessageRealTimes.SingleOrDefault(x => x.FromUserId == userId && x.Id == objectId && x.SendTime == null);

            if (message == null)
                return -2;

            var att = new Attachment()
            {
                AttachmentName = attachmentName,
                AttachmentPath = attachmentPath,
                Extension = extension,
                Size = size ?? 0,
                SizeString = sizeString,
                CreatedOnDate = DateTime.Now
            };

            Db.Attachments.Add(att);

            Db.SaveChanges();

            var rs = att.Id;

            Db.AttachmentMessages.Add(new AttachmentMessage()
            {
                AttachmentId = rs, 
                MessageId = objectId,
                IsDelete = false,
                IsCanEdit = true
            });

            Db.SaveChanges();

            var newMessage =
                Db.MessageRealTimes.SingleOrDefault(
                    x => x.FromUserId == userId && x.Id == objectId && x.SendTime == null);

            if (newMessage != null)
            {
                newMessage.AttachmentCount = (short)(message.AttachmentCount + 1);
                Db.SaveChanges();
            }

            return rs;
        }

        //public int Insert(string attachmentName, string attachmentPath, string extension, int? size, string sizeString)
        //{
        //    return Db.AttachmentInsert(attachmentName, attachmentPath, extension, size, sizeString).FirstOrDefault() ?? 0;
        //}

        //public List<AttachmentObjectGetResult> GetListAttachment(long objectId, byte type)
        //{
        //    return DbTasks.AttachmentObjectGet(objectId, type).ToList();
        //}

        //public long InsertAttachmentObject(long attachmentId, long objectId, int userId, string fullName, byte type)
        //{
        //    var attachmentObject = new TasksManager.AttachmentObject
        //    {
        //        AttachmentId = attachmentId,
        //        ObjectId = objectId,
        //        UserId = userId,
        //        FullName = fullName,
        //        Type = type,
        //        CreatedOnDate = DateTime.Now
        //    };

        //    DbTasks.AttachmentObject.Add(attachmentObject);
        //    DbTasks.SaveChanges();

        //    if (attachmentObject.Id > 0 && type == (byte)AttachmentObjectType.Tasklist)
        //    {
        //        // Cập nhập lại số lượng bảng file đính kèm bảng task
        //        var tasks = DbTasks.Tasks.SingleOrDefault(x => x.Id == objectId && !x.IsDelete);

        //        if (tasks != null)
        //        {
        //            tasks.AttachmentNo = tasks.AttachmentNo == null ? 1 : tasks.AttachmentNo + 1;
        //            DbTasks.SaveChanges();
        //        }
        //    }

        //    return attachmentObject.Id;
        //}

        //public int RemoveAttachment(long id, int userId)
        //{
        //    var attachment = DbTasks.AttachmentObject.SingleOrDefault(x => x.AttachmentId == id && x.UserId == userId);

        //    if (attachment == null)
        //        return -1;

        //    attachment.IsDelete = true;

        //    var result = DbTasks.SaveChanges();

        //    if (result > 0 && attachment.Type == (byte)AttachmentObjectType.Tasklist)
        //    {
        //        // Cập nhập lại số lượng của file đính kèm trong task
        //        var task = DbTasks.Tasks.SingleOrDefault(x => x.Id == attachment.ObjectId && !x.IsDelete);

        //        if (task != null)
        //        {
        //            task.AttachmentNo = task.AttachmentNo - 1;
        //            DbTasks.SaveChanges();
        //        }
        //    }

        //    return result;
        //}

        public List<Attachment> Insert(List<AttachmentMeta> listAttachmentMeta, long uploaderId, string uploaderFullName)
        {
            var listAttachment = new List<Attachment>();

            listAttachmentMeta.ForEach(item =>
            {
                listAttachment.Add(new Attachment
                {
                    Type = item.Type,
                    TypeEn = item.TypeEn,
                    UploaderId = uploaderId,
                    UploaderFullName = uploaderFullName,
                    AttachmentName = item.Name,
                    AttachmentPath = item.Url,
                    Extension = item.Ext,
                    Size = item.SizeByte,
                    SizeString = item.Size,
                    CreatedOnDate = DateTime.Now
                });
            });

            Db.Attachments.AddRange(listAttachment);
            Db.SaveChanges();

            return listAttachment;
        }

        public int DeleteInMessage(long attachmentId, long messageId, long userId)
        {
            using (var transaction = Db.Database.BeginTransaction())
            {
                try
                {
                    var attachMessage = Db.AttachmentMessages.Where(x => x.AttachmentId == attachmentId)
                        .Join(Db.MessageRealTimes.Where(x => x.SendTime == null && x.FromUserId == userId),
                            ao => ao.MessageId, m => m.Id, (ao, m) => new {ao, m})
                        .Join(Db.MessageUsers, arg => arg.m.Id, mu => mu.MessageId, (arg, mu) => arg.ao)
                        .FirstOrDefault();

                    if (attachMessage == null)
                        return -2;

                    if (attachMessage.IsCanEdit)
                    {
                        Db.AttachmentMessages.RemoveRange(
                            Db.AttachmentMessages.Where(x => x.AttachmentId == attachmentId && x.MessageId == messageId));

                        Db.Attachments.AddRange(Db.Attachments.Where(x => x.Id == attachmentId));
                    }
                    else
                    {
                        Db.AttachmentMessages.RemoveRange(
                            Db.AttachmentMessages.Where(x => x.AttachmentId == attachmentId && x.MessageId == messageId));
                    }

                    var message =
                        Db.MessageRealTimes.Where(x => x.SendTime == null && x.FromUserId == userId && x.Id == messageId);

                    foreach (var m in message)
                    {
                        m.AttachmentCount -= 1;
                    }

                    Db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return 1;
        }

        public List<AttachmentMessageGetByMessageIdResults> GetByMessageId(long messageId)
        {
            //var message = Db.MessageUsers.FirstOrDefault(x => x.MessageId == messageId);

            return Db.Attachments.Join(Db.AttachmentMessages, a => a.Id, m => m.AttachmentId, (a, ao) => new {a, ao})
                .Where(x => x.ao.MessageId == messageId /*x.ao.MessageId == message.MessageId*/ && x.ao.IsDelete == false)
                .Select(x => new AttachmentMessageGetByMessageIdResults()
                {
                    Id = x.a.Id,
                    AttachmentName = x.a.AttachmentName,
                    Size = x.a.Size,
                    SizeString = x.a.SizeString,
                    Extension = x.a.Extension,
                    CreatedOnDate = x.a.CreatedOnDate
                }).ToList();
        }

        public List<T> GetBySpec<T>(Expression<Func<Attachment, T>> projector, Expression<Func<Attachment, bool>> spec = null)
        {
            return spec == null ? Db.Attachments.Select(projector).ToList() : Db.Attachments.Where(spec).Select(projector).ToList();
        }

        public Attachment GetById(long id)
        {
            return Db.Attachments.SingleOrDefault(a => a.Id == id);
        }

        //public int InsertForAssessment(string attachmentName, string attachmentPath, string extension, int? size, string sizeString,
        //    long? noteAssessmentId, int noteForUserId, int criteriaId, byte type, int createdByUserId, string createdByFullName, string createdByUserName,
        //    byte time, byte timetype, short year)
        //{
        //    return DbAchievementManagementEntities.AssessmentAttachmentInsert(attachmentName, attachmentPath, extension, size, sizeString,
        //            noteAssessmentId, noteForUserId, criteriaId, type, createdByUserId, createdByFullName, createdByUserName, time, timetype, year)
        //            .FirstOrDefault() ?? 0;
        //}

        //public int AttachmentObjectInsertFromXml(int userId, long objectId, string fullName, byte type, List<AttachmentObjectMeta> listAttachmentObject)
        //{
        //    var xml = CreateAttachmentObjectXmlDocument(listAttachmentObject);
        //    return DbTasks.AttachmentObjectInsertFromXml(userId, objectId, fullName, type, xml);
        //}

        private static string CreateAttachmentObjectXmlDocument(List<AttachmentObjectMeta> listAttachment)
        {
            if (listAttachment == null || listAttachment.Count <= 0) return string.Empty;

            var xml = new XmlDocument();
            XmlNode rootNode = xml.CreateElement("Root");
            xml.AppendChild(rootNode);

            foreach (var item in listAttachment)
            {
                XmlNode attachment = xml.CreateElement("Attachment");
                rootNode.AppendChild(attachment);

                XmlNode attachmentId = xml.CreateElement("AttachmentId");
                attachmentId.InnerText = item.AttachmentId.ToString();
                attachment.AppendChild(attachmentId);

                XmlNode isDelete = xml.CreateElement("IsDelete");
                isDelete.InnerText = item.IsDelete.ToString();
                attachment.AppendChild(isDelete);
            }

            return xml.InnerXml;
        }
    }
}
