using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ExceptionServices;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class MessageRealTimeRepository : Repository<MessageRealTime>
    {
        public MessageRealTimeRepository(ProjectXContext context) : base(context)
        {
        }

        public List<MessageSearchInboxResults> SearchInbox(int userId, string keyword, bool? isRead,
            bool? star, int currentPage, int recordPerPage, out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query = Db.MessageRealTimes
                .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new {m, mu})
                .Where(x => x.mu.UserId == userId && x.mu.Type == false &&
                            x.m.UnsignTitle.Contains(keyword) && x.mu.IsDelete == false &&
                            x.mu.IsTrash == false && x.m.SendTime != null &&
                            (star == null || x.mu.Star == star.Value) &&
                            (isRead == null || x.mu.IsRead == isRead))
                .Select(x => new MessageSearchInboxResults
                {
                    Id = x.mu.Id,
                    FromUser = x.m.FromUser,
                    FromUserId = x.m.FromUserId,
                    FromAvatar = x.m.FromAvatar,
                    ToUser = x.m.ToUser,
                    CcToUser = x.m.CcToUser,
                    LastModifiedOnDate = x.m.LastModifiedOnDate,
                    Title = x.m.Title,
                    AttachmentCount = x.m.AttachmentCount,
                    SendTime = x.m.SendTime,
                    Star = x.mu.Star,
                    IsRead = x.mu.IsRead,
                    Type = x.mu.Type
                });

            totalRecord = query.Count();

            return
                query.OrderByDescending(x => x.SendTime)
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public List<MessageSearchInboxResults> SearchSent(int userId, string keyword, bool? star, bool? isRead,
            int currentPage, int recordPerPage,
            out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query = Db.MessageRealTimes
                .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new {m, mu})
                .Where(x => x.m.FromUserId == userId && x.mu.UserId == userId && x.mu.Type &&
                            x.m.UnsignTitle.Contains(keyword) && x.mu.IsDelete == false &&
                            x.mu.IsTrash == false && x.m.SendTime != null &&
                            (star == null || x.mu.Star == star.Value) &&
                            (isRead == null || x.mu.IsRead == isRead))
                .Select(x => new MessageSearchInboxResults
                {
                    Id = x.mu.Id,
                    FromUser = x.m.FromUser,
                    FromUserId = x.m.FromUserId,
                    FromAvatar = x.m.FromAvatar,
                    ToUser = x.m.ToUser,
                    CcToUser = x.m.CcToUser,
                    LastModifiedOnDate = x.m.LastModifiedOnDate,
                    Title = x.m.Title,
                    AttachmentCount = x.m.AttachmentCount,
                    SendTime = x.m.SendTime,
                    Star = x.mu.Star,
                    IsRead = x.mu.IsRead,
                    Type = x.mu.Type
                });

            totalRecord = query.Count();

            return
                query.OrderByDescending(x => x.SendTime)
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public List<MessageSearchInboxResults> SearchDraft(int userId, string keyword, bool? star, int currentPage,
            int recordPerPage,
            out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query = Db.MessageRealTimes
                .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new {m, mu})
                .Where(x => x.m.FromUserId == userId &&
                            x.m.UnsignTitle.Contains(keyword) && x.mu.IsDelete == false &&
                            x.mu.IsTrash == false && x.m.SendTime == null &&
                            (star == null || x.mu.Star == star.Value))
                .Select(x => new MessageSearchInboxResults
                {
                    Id = x.mu.Id,
                    FromUser = x.m.FromUser,
                    FromUserId = x.m.FromUserId,
                    FromAvatar = x.m.FromAvatar,
                    ToUser = x.m.ToUser,
                    CcToUser = x.m.CcToUser,
                    LastModifiedOnDate = x.m.LastModifiedOnDate,
                    Title = x.m.Title,
                    AttachmentCount = x.m.AttachmentCount,
                    SendTime = x.m.SendTime,
                    Star = x.mu.Star,
                    IsRead = x.mu.IsRead,
                    Type = x.mu.Type
                });

            totalRecord = query.Count();

            return
                query.OrderByDescending(x => x.SendTime)
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public List<MessageSearchInboxResults> SearchTrash(int userId, string keyword, bool? isRead, bool? star,
            int currentPage, int recordPerPage,
            out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query = Db.MessageRealTimes
                .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new {m, mu})
                .Where(x => x.mu.UserId == userId &&
                            x.m.UnsignTitle.Contains(keyword) && x.mu.IsDelete == false &&
                            x.mu.IsTrash && x.m.SendTime != null &&
                            (star == null || x.mu.Star == star.Value) &&
                            (isRead == null || x.mu.IsRead == isRead))
                .Select(x => new MessageSearchInboxResults
                {
                    Id = x.mu.Id,
                    FromUser = x.m.FromUser,
                    FromUserId = x.m.FromUserId,
                    FromAvatar = x.m.FromAvatar,
                    ToUser = x.m.ToUser,
                    CcToUser = x.m.CcToUser,
                    LastModifiedOnDate = x.m.LastModifiedOnDate,
                    Title = x.m.Title,
                    AttachmentCount = x.m.AttachmentCount,
                    SendTime = x.m.SendTime,
                    Star = x.mu.Star,
                    IsRead = x.mu.IsRead,
                    Type = x.mu.Type
                });

            totalRecord = query.Count();

            return
                query.OrderByDescending(x => x.SendTime)
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public List<MessageSearchInboxResults> SearchStar(int userId, string keyword, bool? isRead, int currentPage,
            int recordPerPage,
            out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query = Db.MessageRealTimes
                .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new {m, mu})
                .Where(x => x.mu.UserId == userId &&
                            x.m.UnsignTitle.Contains(keyword) && x.mu.IsDelete == false &&
                            x.mu.IsTrash == false && x.m.SendTime != null &&
                            x.mu.Star && (isRead == null || x.mu.IsRead == isRead))
                .Select(x => new MessageSearchInboxResults
                {
                    Id = x.mu.Id,
                    FromUser = x.m.FromUser,
                    FromUserId = x.m.FromUserId,
                    FromAvatar = x.m.FromAvatar,
                    ToUser = x.m.ToUser,
                    CcToUser = x.m.CcToUser,
                    LastModifiedOnDate = x.m.LastModifiedOnDate,
                    Title = x.m.Title,
                    AttachmentCount = x.m.AttachmentCount,
                    SendTime = x.m.SendTime,
                    Star = x.mu.Star,
                    IsRead = x.mu.IsRead,
                    Type = x.mu.Type
                });

            totalRecord = query.Count();

            return
                query.OrderByDescending(x => x.SendTime)
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public int SaveDraft(long messageId, long fromUserId, string toUser, string ccToUser, string bccToUser,
            string title, string unsignTitle, string content)
        {
            if (toUser == null)
            {
                toUser = string.Empty;
            }
            if (title == null)
            {
                title = string.Empty;
            }
            if (content == null)
            {
                content = string.Empty;
            }
            var encryptContent = content;

            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == fromUserId && x.Type);

            if (message == null)
                return -2;

            if (!Db.MessageRealTimes.Any(x => x.Id == message.MessageId && x.FromUserId == fromUserId))
                return -2;

            var attachmentCount =
                Db.AttachmentMessages.Count(x => x.MessageId == message.MessageId && x.IsDelete == false);

            var messageUpdate =
                Db.MessageRealTimes.SingleOrDefault(x => x.Id == message.MessageId && x.FromUserId == fromUserId);

            if (messageUpdate == null)
                return -2;

            messageUpdate.ToUser = toUser;
            messageUpdate.CcToUser = ccToUser;
            messageUpdate.BccToUser = bccToUser;
            messageUpdate.Title = title;
            messageUpdate.UnsignTitle = unsignTitle;
            messageUpdate.Content = content;
            messageUpdate.AttachmentCount = (short) attachmentCount;
            messageUpdate.LastModifiedOnDate = DateTime.Now;
            messageUpdate.Content = encryptContent;

            return Db.SaveChanges();
        }

        public long InsertTemp(string fromUser, int fromUserId, string fromAvatar, string usignTitle,
            long? messageIdForward)
        {
            long rs;
            using (var transaction = Db.Database.BeginTransaction())
            {
                try
                {
                    var message = new MessageRealTime()
                    {
                        FromUser = fromUser,
                        FromUserId = fromUserId,
                        FromAvatar = fromAvatar,
                        ToUser = "",
                        CcToUser = null,
                        BccToUser = null,
                        Title = "",
                        UnsignTitle = usignTitle,
                        Content = "",
                        AttachmentCount = 0,
                        SendTime = null,
                        LastModifiedOnDate = DateTime.Now
                    };
                    Db.MessageRealTimes.Add(message);

                    Db.SaveChanges();

                    var messageUser = new MessageUser()
                    {
                        MessageId = message.Id,
                        UserId = fromUserId,
                        IsDelete = false,
                        IsTrash = false,
                        Star = false,
                        IsRead = true,
                        ReadTime = DateTime.Now,
                        Type = true
                    };
                    Db.MessageUsers.Add(messageUser);

                    Db.SaveChanges();

                    rs = messageUser.Id;

                    if (messageIdForward != null && messageIdForward > 0)
                    {
                        var messageUserNew =
                            Db.MessageUsers.SingleOrDefault(
                                x => x.Id == messageIdForward.Value && x.UserId == fromUserId);

                        if (messageUserNew == null)
                        {
                            transaction.Rollback();
                            return -2;
                        }

                        var attachmets = Db.AttachmentMessages.AsNoTracking().Where(x => x.MessageId == messageUserNew.MessageId && x.IsDelete == false).ToList();

                        foreach (var a in attachmets.Select(x => new AttachmentMessage()
                        {
                            AttachmentId = x.AttachmentId,
                            MessageId = messageUserNew.MessageId,
                            IsDelete = false,
                            IsCanEdit = false
                        }))
                        {
                            Db.AttachmentMessages.AddRange(attachmets);
                        }
                        Db.SaveChanges();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return rs;
        }

        public int Discard(long messageId, long fromUserId)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == fromUserId && x.Type);

            if (message == null)
                return -2;

            if (
                !Db.MessageRealTimes.Any(
                    x => x.Id == message.MessageId && x.FromUserId == fromUserId && x.SendTime == null))
                return -2;

            using (var transaction = Db.Database.BeginTransaction())
            {
                try
                {
                    Db.Attachments.RemoveRange(
                        Db.Attachments.Where(
                            a => Db.AttachmentMessages.Any(
                                x => x.MessageId == message.MessageId && x.IsCanEdit && x.AttachmentId == a.Id)));

                    Db.AttachmentMessages.RemoveRange(Db.AttachmentMessages.Where(x => x.MessageId == message.MessageId));

                    Db.MessageUsers.RemoveRange(Db.MessageUsers.Where(x => x.Id == messageId && x.UserId == fromUserId));

                    Db.MessageRealTimes.RemoveRange(
                        Db.MessageRealTimes.Where(
                            x => x.Id == message.MessageId && x.FromUserId == fromUserId && x.SendTime == null));

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

        public int DeleteTrash(long messageId, long userId)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == userId && x.IsTrash);

            if (message == null)
                return -2;

            message.IsDelete = true;

            return Db.SaveChanges();
        }

        public int MoveTrash(long messageId, long userId)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == userId);

            if (message == null)
                return -2;

            message.IsTrash = true;

            return Db.SaveChanges();
        }

        public int UndoMoveTrash(long messageId, long userId)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == userId);

            if (message == null)
                return -2;

            message.IsTrash = false;

            return Db.SaveChanges();
        }

        public int MarkReaded(long messageId, long userId, bool isRead)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == userId);

            if (message == null)
                return -2;

            if (isRead == false)
            {
                message.IsRead = false;
                message.ReadTime = null;

                return Db.SaveChanges();
            }

            message.IsRead = true;
            message.ReadTime = DateTime.Now;
            return Db.SaveChanges();
        }

        public int SetStar(long messageId, long userId, bool star)
        {
            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == userId);

            if (message == null)
                return -2;

            message.Star = star;

            return Db.SaveChanges();
        }

        public MessageRealTime GetById(long id, long userId)
        {
            return
                Db.MessageRealTimes.Join(
                    Db.MessageUsers.Where(mu => mu.UserId == userId && !mu.IsDelete && mu.Id == id), m => m.Id,
                    mu => mu.MessageId, (m, mu) => m).SingleOrDefault();
        }

        public List<MessageSendResults> Send(long messageId, long fromUserId, string toUser, string ccToUser,
            string bccToUser,
            string title, string unsignTitle, string content, List<int> toUserIds, out int rs)
        {
            if (toUser == null)
            {
                throw new ArgumentNullException(nameof(toUser));
            }
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            var encryptContent = content;

            var results = new List<MessageSendResults>();

            var message = Db.MessageUsers.SingleOrDefault(x => x.Id == messageId && x.UserId == fromUserId && x.Type);

            if (message == null)
            {
                rs = -2;
                return results;
            }

            if (!Db.MessageRealTimes.Any(x => x.Id == message.MessageId && x.FromUserId == fromUserId))
            {
                rs = -2;
                return results;
            }

            using (var transaction = Db.Database.BeginTransaction())
            {
                try
                {
                    var attachCount =
                        Db.AttachmentMessages.Count(x => x.MessageId == message.MessageId && x.IsDelete == false);

                    var messages =
                        Db.MessageRealTimes.Where(x => x.Id == message.MessageId && x.FromUserId == fromUserId);

                    foreach (var m in messages)
                    {
                        m.ToUser = toUser;
                        m.CcToUser = ccToUser;
                        m.BccToUser = bccToUser;
                        m.Title = title;
                        m.UnsignTitle = unsignTitle;
                        m.Content = encryptContent;
                        m.AttachmentCount = (short)attachCount;
                        m.SendTime = DateTime.Now;
                        m.LastModifiedOnDate = DateTime.Now;
                    }

                    rs = Db.SaveChanges();

                    var messageUsers = toUserIds.Select(userId => new MessageUser()
                    {
                        MessageId = message.MessageId, UserId = userId, IsDelete = false, IsTrash = false
                    }).ToList();

                    Db.MessageUsers.AddRange(messageUsers);

                    Db.SaveChanges();

                    results.AddRange(messageUsers.Where(x => x.Id > 0)
                            .Select(x => new MessageSendResults() {MessageId = x.MessageId, UserId = x.UserId}));

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return results;
        }

        public MessageGetTotalUnreadResults GetTotalUnread(int userId)
        {
            var rs = new MessageGetTotalUnreadResults
            {
                TotalInboxUnread = Db.MessageRealTimes
                    .Join(Db.MessageUsers, m => m.Id, mu => mu.MessageId, (m, mu) => new {m, mu})
                    .Count(x => x.mu.UserId == userId && x.mu.Type == false && x.mu.IsDelete == false &&
                                x.mu.IsTrash == false && x.m.SendTime != null && x.mu.IsRead == false),
                TotalStarUnread = Db.MessageRealTimes
                    .Join(Db.MessageUsers, m => m.Id, mu => mu.MessageId, (m, mu) => new {m, mu})
                    .Count(x => x.mu.UserId == userId && x.mu.IsDelete == false &&
                                x.mu.IsTrash == false && x.mu.Star && x.mu.IsRead == false),
                TotalDraft = Db.MessageRealTimes
                    .Join(Db.MessageUsers, m => m.Id, mu => mu.MessageId, (m, mu) => new {m, mu})
                    .Count(x => x.m.FromUserId == userId && x.mu.IsDelete == false &&
                                x.mu.IsTrash == false && x.m.SendTime == null)
            };

            return rs;
        }

        public List<MessageGetNotifyResult> GetNotify(long userId, int currentPage, int recordPerPage, out int totalRecord)
        {
            var query = Db.MessageRealTimes
                           .Join(Db.MessageUsers, m => m.Id, u => u.MessageId, (m, mu) => new { m, mu })
                           .Where(x => x.mu.UserId == userId && x.mu.Type == false &&
                               x.mu.IsDelete == false &&
                               x.mu.IsTrash == false && x.m.SendTime != null &&
                               x.mu.IsRead == false)
                           .Select(x => new MessageGetNotifyResult
                           {
                               Id = x.mu.Id,
                               FromUser = x.m.FromUser,
                               FromAvatar = x.m.FromAvatar,
                               Title = x.m.Title,
                               SendTime = x.m.SendTime,
                           });

            totalRecord = query.Count();

            return query.OrderByDescending(x => x.Id)
                    .Skip((currentPage - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToList();
        }

        public Attachment DownloadAttachment(long messageId, long attachmentId, long userId)
        {
            var messageUser = Db.MessageUsers.SingleOrDefault(m => m.Id == messageId && m.UserId == userId && !m.IsDelete);
            if (messageUser == null)
            {
                return null;
            }
            var attachmentObject = Db.AttachmentMessages.SingleOrDefault(a => a.AttachmentId == attachmentId && a.MessageId == messageUser.MessageId);
            if (attachmentObject == null)
            {
                return null;
            }

            var attachment = Db.Attachments.SingleOrDefault(a => a.Id == attachmentId);
            return attachment;
        }

        public MessageGetInboxDetailByMessageIdResults GetDetailById(int id, int userId)
        {
            return Db.MessageRealTimes.Where(x => x.SendTime != null)
                .Join(Db.MessageUsers.Where(
                        mu => mu.UserId == userId && mu.Type == false && mu.IsDelete == false
                              && mu.IsTrash == false),
                    m => m.Id, mu => mu.MessageId, (m, mu) =>
                        new MessageGetInboxDetailByMessageIdResults
                        {
                            Id = mu.Id,
                            FromUser = m.FromUser,
                            FromUserId = m.FromUserId,
                            FromAvatar = m.FromAvatar,
                            ToUser = m.ToUser,
                            CcToUser = m.CcToUser,
                            LastModifiedOnDate = m.LastModifiedOnDate,
                            Title = m.Title,
                            AttachmentCount = m.AttachmentCount,
                            SendTime = m.SendTime,
                            Star = mu.Star,
                            IsRead = mu.IsRead,
                            Type = mu.Type
                        }).FirstOrDefault();
        }

        public List<Attachment> GetAttachmentsByMessageId(long messageId)
        {
            return Db.Attachments
                    .Join(Db.AttachmentMessages
                        .Where(x => x.IsDelete == false), a => a.Id, am => am.AttachmentId, (a, at) => new {a, at})
                    .Join(Db.MessageRealTimes
                        .Where(x => x.Id == messageId), arg => arg.at.MessageId, m => m.Id, (at, m) => at.a)
                    .ToList();
        }

        //public List<SendEmailSearchResults> SendEmailSearch(string keyword, int pageIndex, int pageSize, out int totalRows)
        //{
        //    var output = new ObjectParameter("TotalRows", typeof(int));
        //    var result = DbHrm.SendEmailSearch(keyword, pageIndex, pageSize, output).ToList();
        //    totalRows = Convert.ToInt32(output.Value);
        //    return result;
        //}
    }
}

