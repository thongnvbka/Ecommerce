using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupChatRepository : Repository<GroupChat>
    {
        public GroupChatRepository(ProjectXContext context) : base(context)
        {
        }

        public int GroupChatUserUpdateOnlineStatusByGroupId(string groupId, int userId, byte type, byte status)
        {
            var groupChatUsers = Db.GroupChatUsers.Where(x => x.Type == type && x.UserId == userId && x.GroupId == groupId).ToList();

            if (!groupChatUsers.Any())
                return -1;

            foreach (var groupChatUser in groupChatUsers)
            {
                groupChatUser.Status = status;
            }
            return Db.SaveChanges();
        }

        public bool IsGroupChatExists(string groupId)
        {
            return Db.GroupChats.Any(x => x.Id == groupId && !x.IsDelete);
        }

        public int GroupChatSystemInsert(string id)
        {
            if (Db.GroupChats.Any(x => x.UnsignName == "" && x.CreatorId == 0 && x.IsDelete == false))
            {
                return -1;
            }

            Db.GroupChats.Add(new GroupChat()
            {
                Id = id,
                CreatorId = 0,
                CreatedOnDate = DateTime.Now,
                GroupName = id,
                Image = null,
                CreatorFullName = "",
                CreatorTitleName = "",
                CreatorOfficeName = "",
                UnsignName = id,
                IsSystem = true
            });
            
            return Db.SaveChanges();
        }

        public bool GroupChatUserCheckExists(int userId, string groupId, byte type)
        {
            return Db.GroupChatUsers.Any(x => x.UserId == userId && x.GroupId.Equals(groupId) && x.Type == type);
        }

        public List<GroupChatContentGetResult> GroupChatContentGet(string groupId, int userId, byte type, int pageIndex, int pageSize, out long totalRows)
        {
            var groupChatReads =
                Db.GroupChatReads.Where(x => x.GroupId == groupId && x.UserId == userId && x.UserType == type);

            foreach (var g in groupChatReads)
            {
                g.IsRead = true;
                g.Quantity = 0;
            }

            Db.SaveChanges();

            totalRows = Db.GroupChatContents.LongCount(x => x.GroupId == groupId && x.ParentId == null);

            var items = Db.GroupChatContents.GroupJoin(Db.GroupChatLikes, 
                    gcc => new { Key1 = gcc.Id, Key2 = userId, Key3 = type }, 
                    gcl => new { Key1 = gcl.ContentId, Key2 = gcl.UserId, Key3 = gcl.UserType },
                (gcc, gcl) => new {gcc, gcl })
                .Where(x => x.gcc.GroupId == groupId && x.gcc.ParentId == null && x.gcc.IsDelete == false)
                .SelectMany(x => x.gcl.DefaultIfEmpty(), (x, y) => new GroupChatContentGetResult()
                {
                    Id = x.gcc.Id,
                    UserId = x.gcc.UserId,
                    Type = x.gcc.Type,
                    IsDelete = x.gcc.IsDelete,
                    OfficeName = x.gcc.OfficeName,
                    FullName = x.gcc.FullName,
                    UserName = x.gcc.UserName,
                    AttachmentCount = x.gcc.AttachmentCount,
                    Content = x.gcc.Content,
                    TitleName = x.gcc.TitleName,
                    Image = x.gcc.Image,
                    GroupId = x.gcc.GroupId,
                    ParentId = x.gcc.ParentId,
                    IsSystem = x.gcc.IsSystem,
                    Dislike = x.gcc.Dislike,
                    Like = x.gcc.Like,
                    NumberOfReplies = x.gcc.NumberOfReplies,
                    SentTime = x.gcc.SentTime,
                    Liked = y.IsLike ? 1 : 0
                }).OrderByDescending(x => x.SentTime).Skip(pageIndex).Take(pageSize)
                .ToList();

            return items;
        }

        public int GroupChatUserInsert(string groupId, int userId, string fullName, string image, 
            string titleName, string officeName, int currentUserId, bool isSystem, byte type, 
            byte inviteStatus, byte status, string notifyUrl, bool isShowNotify)
        {
            // Kiểm tra nhóm có tồn tại hay không
            if (!Db.GroupChats.Any(x => x.Id == groupId && x.IsDelete == false))
                return -1;

            // Kiểm tra người dùng hiện tại có quyền mời người khác vào nhóm không
            if (isSystem == false)
            {
                if (!Db.GroupChats.Any(x => x.CreatorId == currentUserId && x.IsDelete == false))
                    return -2;
            }

            // Kiểm tra ngời dùng này đã tồn tại trong nhóm chưa
            if (Db.GroupChatUsers.Any(x => x.UserId == userId && x.GroupId == groupId && x.Type == type))
                return -3;

            Db.GroupChatUsers.Add(new GroupChatUser()
            {
                GroupId = groupId,
                UserId = userId,
                FullName = fullName,
                Image = image,
                TitleName = titleName,
                OfficeName = officeName,
                InvitedByUserId = currentUserId,
                InviteStatus = inviteStatus,
                Type = type,
                Status = status,
                NotifyUrl = notifyUrl,
                IsShowNotify = isShowNotify
            });

            return Db.SaveChanges();
        }

        public long GroupChatContentInsert(int userId, string content, string groupId, string userName, 
            string titleName, string officeName, string image, string fullName, byte type, int attachmentCount)
        {
            //Kiểm tra người dùng có quyền chat trong nhóm này không.
            if (!Db.GroupChatUsers.Any(x => x.UserId == userId && x.GroupId == groupId && x.InviteStatus == 2))
            {
                return -1;
            }

            var groupChatContent = new GroupChatContent()
            {
                GroupId =  groupId,
                UserId = userId,
                UserName = userName,
                FullName = fullName,
                TitleName = titleName,
                OfficeName = officeName,
                Image = image,
                Content = content,
                IsSystem = false,
                Type = type,
                AttachmentCount = attachmentCount
            };

            Db.GroupChatContents.Add(groupChatContent);
            Db.SaveChanges();

            return groupChatContent.Id;
        }

        public List<GroupChatReadUpdateCountResult> GroupChatReadUpdateCount(string groupId, long chatId, int userId)
        {
            var results = new List<GroupChatReadUpdateCountResult>();
            // Thêm mới số lượng bình luận chưa đọc nếu không tồn tại.
            if (!Db.GroupChatReads.Any(x => x.GroupId == groupId))
            {
                // todo: @Henry Đang làm rở 
                var users = Db.GroupChatUsers.Where(x => x.GroupId == groupId && x.Status != 1 && x.UserId != userId);

                var groupChatReads = new List<GroupChatRead>();
                foreach (var u in users)
                {
                    groupChatReads.Add(new GroupChatRead()
                    {
                        GroupId = groupId,
                        UserId = u.UserId,
                        IsRead = false,
                        Quantity = 1,
                        UserType = u.Type
                    });
                }
                Db.GroupChatReads.AddRange(groupChatReads);
                Db.SaveChanges();

                results.AddRange(groupChatReads.Select(x=> new GroupChatReadUpdateCountResult()
                {
                   Type = x.UserType,
                   UserId = x.UserId,
                   IsShowNotify = users.FirstOrDefault(g=> g.GroupId == x.GroupId && g.UserId == x.UserId)?.IsShowNotify ?? false,
                   NotifyUrl = users.FirstOrDefault(g => g.GroupId == x.GroupId && g.UserId == x.UserId)?.NotifyUrl ?? ""
                }));
            }
            else
            {
                // Thêm mới số lượng bình luận chưa đọc nếu không tồn tại.
                var updateComments = Db.GroupChatReads.Where(gcr => gcr.GroupId == groupId && gcr.IsRead == false)
                    .Join(Db.GroupChatUsers.Where(gcu => gcu.Status != 1), gcr => new { gcr.GroupId, gcr.UserId }, gcu => new { gcu.GroupId, gcu.UserId },
                        (gcr, gcu) => gcr);

                foreach (var c in updateComments)
                {
                    c.Quantity += 1;
                }
                Db.SaveChanges();

                // Update lại số lượng bình luận trong trường hợp đã tồn tại và người dùng đã đọc.
                var updateComment2 = Db.GroupChatReads.Where(gcr => gcr.GroupId == groupId && gcr.IsRead)
                    .Join(Db.GroupChatUsers.Where(gcu => gcu.Status != 1), gcr => new {gcr.GroupId, gcr.UserId},
                        gcu => new {gcu.GroupId, gcu.UserId},
                        (gcr, gcu) => gcr);

                foreach (var c in updateComment2)
                {
                    c.Quantity = 1;
                    c.IsRead = false;
                }
                Db.SaveChanges();

                var listResults = Db.GroupChatReads.Where(gcr => gcr.GroupId == groupId && gcr.IsRead)
                    .Join(Db.GroupChatUsers.Where(gcu => gcu.Status != 1), gcr => new {gcr.GroupId, gcr.UserId},
                        gcu => new {gcu.GroupId, gcu.UserId},
                        (gcr, gcu) => new {gcr, gcu}).Select(x => new GroupChatReadUpdateCountResult()
                    {
                        UserId = x.gcr.UserId,
                        Type = x.gcu.Type,
                        IsShowNotify = x.gcu.IsShowNotify,
                        NotifyUrl = x.gcu.NotifyUrl
                    }).ToList();

                results.AddRange(listResults);
            }

            return results.Where(x=> x.UserId != userId).Distinct().ToList();
        }

        public List<GroupChatGetListUserForNotificationResult> GroupChatGetListUserForNotification(string groupId)
        {
            return Db.GroupChatUsers.Where(gcu => gcu.GroupId == groupId && gcu.IsShowNotify)
                    .Join(Db.UserConnections, gcu => gcu.UserId, uc => uc.UserId,
                        (gcu, uc) => new GroupChatGetListUserForNotificationResult()
                        {
                            UserId = uc.UserId,
                            UserType = uc.UserType,
                            NotifyUrl = gcu.NotifyUrl
                        })
                    .Distinct().ToList();
        }

        public GroupChatContent GroupChatContentGetByUserId(int userId, byte type, string groupId, long chatId)
        {
            return Db.GroupChatContents.FirstOrDefault(x => x.UserId == userId && x.Type == type && x.GroupId == groupId && x.Id == chatId);
        }

        public int GroupChatContentUpdateAttachment(GroupChatContent groupChatContent)
        {
            var info = Db.GroupChatContents.SingleOrDefault(x => x.Id == groupChatContent.Id);

            if (info == null)
                return -1;

            info.Content = groupChatContent.Content;
            info.AttachmentCount = groupChatContent.AttachmentCount;

            return Db.SaveChanges();
        }

        public int GroupChatContentDelete(long contentId, int userId, byte type)
        {
            var groupChatContent = Db.GroupChatContents.SingleOrDefault(x => x.Id == contentId && x.UserId == userId);

            if (!Db.GroupChatContents.Any(x => x.Id == contentId && x.UserId == userId && x.Type == type))
                return -1;

            var update =
                Db.GroupChatContents.SingleOrDefault(
                    x => x.Id == contentId && x.UserId == userId && x.Type == type);

            // ReSharper disable once PossibleNullReferenceException
            update.IsDelete = true;

            var rs = Db.SaveChanges();

            if (rs > 0 && groupChatContent != null && groupChatContent.ParentId != null)
            {
                var update2 = Db.GroupChatContents.SingleOrDefault(
                        x => x.Id == groupChatContent.ParentId.Value && x.UserId == userId && x.Type == type);

                // ReSharper disable once PossibleNullReferenceException
                if (update2.NumberOfReplies == 0)
                {
                    update2.NumberOfReplies = null;
                }
                else
                {
                    update2.NumberOfReplies = -1;
                }
                Db.SaveChanges();
            }

            return rs;
        }

        public long GroupChatContentReplyInsert(long contentId, int userId, string groupId, string content, 
            string userName, string fullName, string titleName, string officeName, string image, byte type, 
            int attachmentCount, bool isSystem)
        {
            // Kiểm tra người dùng có trong nhóm này không.
            if (!Db.GroupChatUsers.Any(x => x.GroupId == groupId && x.UserId == userId && x.Type == type))
                return -1;

            if (!Db.GroupChatContents.Any(x => x.Id == contentId && x.IsDelete == false))
                return -2;

            var groupChatContent =new GroupChatContent()
            {
                GroupId =  groupId,
                UserId =  userId,
                UserName = userName,
                FullName =  fullName,
                TitleName =  titleName,
                OfficeName = officeName,
                Image = image,
                Content = content,
                IsSystem = isSystem,
                Type = type,
                AttachmentCount = attachmentCount,
                ParentId = contentId
            };

            Db.GroupChatContents.Add(groupChatContent);
            Db.SaveChanges();

            if (groupChatContent.Id > 0)
            {
                // Update lại số lượng comment cho comment cha
                var update = Db.GroupChatContents.SingleOrDefault(x => x.Id == contentId);

                // ReSharper disable once PossibleNullReferenceException
                update.NumberOfReplies = update.NumberOfReplies + 1 ?? 1;

                Db.SaveChanges();
            }

            return groupChatContent.Id;
        }

        public GroupChatContentMeta GroupChatContentGetById(long contentId)
        {
            return Db.GroupChatContents.Where(gcc => gcc.Id == contentId && !gcc.IsDelete)
                .Join(Db.GroupChatUsers, gcc => new { gcc.UserId, gcc.GroupId }, gcu => new { gcu.UserId, gcu.GroupId }, (gcc, gcu) => new GroupChatContentMeta
                {
                    Id = gcc.Id,
                    Content = gcc.Content,
                    GroupId = gcc.GroupId,
                    UserId = gcc.UserId,
                    UserName = gcc.UserName,
                    FullName = gcc.FullName,
                    Image = gcc.Image,
                    TitleName = gcc.TitleName,
                    OfficeName = gcc.OfficeName,
                    SentTime = gcc.SentTime,
                    IsSystem = gcc.IsSystem,
                    Type = gcc.Type,
                    IsDelete = gcc.IsDelete,
                    AttachmentCount = gcc.AttachmentCount,
                    Like = gcc.Like,
                    Dislike = gcc.Dislike,
                    NumberOfReplies = gcc.NumberOfReplies,
                    ParentId = gcc.ParentId,
                    NotifyUrl = gcu.NotifyUrl
                }).FirstOrDefault();
        }

        public List<UserReplyMeta> GroupChatContentGetListUserIdReply(long contentId, string groupId, 
            Expression<Func<GroupChatContent, GroupChatUser, UserReplyMeta>> projector)
        {
            return Db.GroupChatContents.Where(gc => gc.ParentId == contentId && !gc.IsDelete && gc.GroupId.Equals(groupId))
                .Join(Db.GroupChatUsers.Where(gu => gu.GroupId.Equals(groupId)), gc => new { gc.UserId, gc.GroupId }, gu => new { gu.UserId, gu.GroupId }, projector)
                .Distinct().ToList();
        }

        public long GroupChatLikeSave(long contentId, string groupId, int userId, string userName, string fullName, string image, byte userType, bool isLike)
        {
            // Kiểm tra người dùng có trong nhóm này không.
            if (!Db.GroupChatUsers.Any(x => x.GroupId == groupId && x.UserId == userId && x.Type == userType))
                return -1;

            if (!Db.GroupChatContents.Any(x => x.Id == contentId && x.IsDelete == false))
                return -2;

            // Nếu chưa tồn tại thêm mới. Tồn tại rồi update lại trạng thái cho comment.
            var oldLike =
                Db.GroupChatLikes.FirstOrDefault(
                    x =>
                        x.GroupId == groupId && x.ContentId == contentId &&
                        x.UserId == userId && x.UserType == userType)?.IsLike;

            if (oldLike == null && isLike == false)
                return -3;

            long rs;

            if (oldLike == null)
            {
                var like = new GroupChatLike()
                {
                    ContentId =  contentId,
                    GroupId =  groupId,
                    UserId =  userId,
                    UserName = userName,
                    FullName =  fullName,
                    Image = image,
                    UserType =  userType,
                    IsLike = true
                };
                Db.GroupChatLikes.Add(like);
                Db.SaveChanges();

                rs = like.Id;
                if (rs > 0)
                {
                    var groupChatConent = Db.GroupChatContents.SingleOrDefault(x => x.Id == contentId);
                    if (groupChatConent != null)
                    {
                        groupChatConent.Like = groupChatConent.Like == null ? 1 : groupChatConent.Like + 1;
                        Db.SaveChanges();
                    }
                }

                return rs;
            }

            if (oldLike == isLike)
                return -4;

            var chatLikes = Db.GroupChatLikes.Where(
                    x => x.GroupId == groupId && x.UserId == userId && x.ContentId == contentId &&
                        x.UserType == userType);

            foreach (var l in chatLikes)
            {
                l.IsLike = isLike;
            }

            rs = Db.SaveChanges();

            if (rs > 0)
            {
                var groupChatConent = Db.GroupChatContents.SingleOrDefault(x => x.Id == contentId);
                if (groupChatConent != null)
                {
                    groupChatConent.Like = isLike ? groupChatConent.Like == null ? 1 : groupChatConent.Like + 1 : groupChatConent.Like - 1;
                    Db.SaveChanges();
                }
            }

            return rs;
        }

        public List<GroupChatLike> GroupChatGetUserLiked(long contentId)
        {
            return Db.GroupChatLikes.Where(x => x.ContentId == contentId && x.IsLike).ToList();
        }

        public List<GroupChatContentGetListRepliesResult> GroupChatContentGetListReplies(int userId, long contentId, byte userType, int pageIndex, int pageSize)
        {
            return Db.GroupChatContents.GroupJoin(Db.GroupChatLikes,
                        gcc => new {Key1 = gcc.Id, Key2 = userId, Key3 = userType},
                        gcl => new {Key1 = gcl.ContentId, Key2 = gcl.UserId, Key3 = gcl.UserType},
                        (gcc, gcl) => new {gcc, gcl})
                    .Where(x => x.gcc.ParentId == contentId && x.gcc.IsDelete == false)
                    .SelectMany(x => x.gcl.DefaultIfEmpty(), (x, y) => new GroupChatContentGetListRepliesResult()
                    {
                        Type = x.gcc.Type,
                        UserId = x.gcc.UserId,
                        AttachmentCount = x.gcc.AttachmentCount,
                        Content = x.gcc.Content,
                        Dislike = x.gcc.Dislike,
                        FullName = x.gcc.FullName,
                        GroupId = x.gcc.GroupId,
                        Image = x.gcc.Image,
                        IsDelete = x.gcc.IsDelete,
                        IsSystem = x.gcc.IsSystem,
                        Like = x.gcc.Like,
                        NumberOfReplies = x.gcc.NumberOfReplies,
                        OfficeName = x.gcc.OfficeName,
                        ParentId = x.gcc.ParentId,
                        SentTime = x.gcc.SentTime,
                        TitleName = x.gcc.TitleName,
                        UserName = x.gcc.UserName,
                        Liked = y.IsLike ? 1 : 0
                    }).OrderByDescending(x => x.SentTime).Skip(pageIndex).Take(pageSize).ToList();
        }

        public List<GroupChatGetUserConnectionInGroupResult> GetUserConnectionInGroup(string groupId, int userId)
        {
            return Db.GroupChatUsers.Join(Db.UserConnections,
                    gcu => new {Key1 = gcu.UserId, Key2 = gcu.Type},
                    ut => new {Key1 = ut.UserId, Key2 = ut.UserType ?? 0},
                    (gcu, ut) => new {gcu, ut})
                .Where(x => x.gcu.GroupId == groupId && !(x.gcu.UserId == userId))
                .Select(x => new GroupChatGetUserConnectionInGroupResult()
                {
                    Type = x.gcu.Type,
                    UserId = x.gcu.UserId,
                    ConnectionId = x.ut.ConnectionId
                }).ToList();
        }

        public int GroupChatUserUpdateIsShowNotify(string groupId, int userId, byte type, bool isShowNotify)
        {
            if (!Db.GroupChats.Any(x => x.Id == groupId && x.IsDelete == false))
                return -1;

            if (!Db.GroupChatUsers.Any(x => x.GroupId == groupId && x.UserId == userId))
                return -2;

            var groupChatUser =
                Db.GroupChatUsers.Where(x => x.UserId == userId && x.GroupId == groupId && x.Type == type);

            foreach (var g in groupChatUser)
            {
                g.IsShowNotify = isShowNotify;
            }

            return Db.SaveChanges();
        }
    }
}







