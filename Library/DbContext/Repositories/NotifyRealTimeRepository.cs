using System;
using System.Collections.Generic;
using System.Linq;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class NotifyRealTimeRepository : Repository<NotifyRealTime>
    {
        public NotifyRealTimeRepository(ProjectXContext context) : base(context)
        {
        }

        public List<NotifyGetNotifySystemByToUserIdResults> GetNotifySystemByToUserId(string keyword, int userId, bool? isRead, byte? type, int currentPage, int recordPerPage, out int totalRecord)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var query =
                Db.NotifyRealTimes.Where(
                    x =>
                        x.ToUserId == userId && x.FromUserId == 0 && (isRead == null || x.IsRead == isRead) &&
                        x.UnsignName.Contains(keyword) && ((type == null && (x.Type == 0 || x.Type == 1))) ||
                        (type != null && x.Type == type));

            totalRecord = query.Count();

            return query.Select(x => new NotifyGetNotifySystemByToUserIdResults
            {
                Id = x.Id,
                Title = x.Title,
                Type = x.Type,
                SendTime = x.SendTime,
                IsRead = x.IsRead,
                Content = x.Content,
                Url = x.Url
            }).OrderByDescending(x => x.Id).Skip((currentPage - 1)*recordPerPage).Take(recordPerPage).ToList();
        }

        public long InsertNotifySystem(int toUserId, string title, EnumNotifyType type, string content, DateTime sendTime, string url)
        {
           var notify = new NotifyRealTime()
           {
               ToUserId = toUserId,
               FromUserId = 0,
               Title =  title,
               Avatar = null,
               Content = content,
               SendTime = sendTime,
               Type = (byte)type,
               IsRead = false,
               UnsignName = MyCommon.Ucs2Convert(title),
               Url = url
           };

            Db.NotifyRealTimes.Add(notify);
            Db.SaveChanges();

            return notify.Id;
        }

        /// <summary>
        /// Hệ thống thông báo tới người dùng
        /// (Nếu người dùng chưa đọc mà có cùng một Group thì nội dung của thông báo chưa đọc sẽ được cập nhật)
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="title"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="sendTime"></param>
        /// <param name="group"></param>
        /// <param name="url"></param>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public long InsertNotifySystem(int toUserId, string title, EnumNotifyType type, string content, DateTime sendTime,
            string @group,
            string url, out bool inserted)
        {
            NotifyRealTime notify;
            if (!string.IsNullOrWhiteSpace(group))
            {
                notify = Db.NotifyRealTimes.FirstOrDefault(
                    x => x.ToUserId == toUserId && x.Type == 0 && x.Group == @group && x.IsRead == false);

                if (notify != null)
                {
                    notify.Title = title;
                    notify.Content = content;
                    notify.UnsignName = MyCommon.Ucs2Convert(title);
                    notify.Url = url;

                    inserted = false;
                    Db.SaveChanges();

                    return notify.Id;
                }
            }
            
            notify = new NotifyRealTime()
            {
                FromUserId = 0,
                ToUserId = toUserId,
                Title = title,
                Avatar = null,
                Content = content,
                SendTime = sendTime,
                Type = 0,
                IsRead = false,
                UnsignName = MyCommon.Ucs2Convert(title),
                Url = url,
                Group = @group
            };

            Db.NotifyRealTimes.Add(notify);

            Db.SaveChanges();
            inserted = true;

            return notify.Id;
        }

        public NotifyRealTime GetById(int id, int userId)
        {
            return Db.NotifyRealTimes.SingleOrDefault(n => n.Id == id && n.ToUserId == userId);
        }

        public void UpdateIsRead(NotifyRealTime notify)
        {
            if (notify == null)
            {
                throw new ArgumentNullException("notify");
            }
            if (notify.IsRead)
            {
                return;
            }
            notify.IsRead = true;

            Db.SaveChanges();
        }

        public int MarkAsReaded(int userId, bool isRead, long notifyId)
        {
            var notify = Db.NotifyRealTimes.SingleOrDefault(x => x.Id == notifyId && x.ToUserId == userId);

            if (notify == null)
                return -1;

            notify.IsRead = isRead;

            return Db.SaveChanges();
        }
    }
}
