using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Helpers;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;
using Library.UnitOfWork;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace Cms.Hubs
{
    [Microsoft.AspNet.SignalR.Authorize]
    public class GroupChatHub : Hub
    {
        //public string GetClaim(List<Claim> claims, string key)
        //{
        //    var claim = claims.FirstOrDefault(c => c.Type == key);

        //    return claim?.Value;
        //}

        public override Task OnConnected()
        {
            var customerType = byte.Parse(Context.QueryString["type"]);
            var groupId = Context.QueryString["groupId"];

            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");
                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var unitOfWork = new UnitOfWork();

            // Thêm mới connection vào trong bảng User_Connection.
            unitOfWork.UserRepo.InsertUserConnection(userState.UserId, userState.UserName, userState.FullName,
                userState.OfficeId, userState.OfficeName, userState.TitleName, userState.Avatar, Context.ConnectionId, userState.SessionId,
                MyCommon.Ucs2Convert($"{userState.UserName} {userState.FullName} {userState.TitleName} {userState.OfficeName}"),
                customerType);

            // Update lại trạng thái online trong bảng groupChatUser.
            unitOfWork.GroupChatRepo.GroupChatUserUpdateOnlineStatusByGroupId(groupId, userState.UserId, customerType, 1);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var customerType = byte.Parse(Context.QueryString["type"]);
            var groupId = Context.QueryString["groupId"];

            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var unitOfWork = new UnitOfWork();

            // Update lại trạng thái của người dùng trong bảng groupChatUser.
            unitOfWork.GroupChatRepo.GroupChatUserUpdateOnlineStatusByGroupId(groupId, userState.UserId, customerType, 0);

            return base.OnDisconnected(stopCalled);
        }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public async Task JoinGroupChat(string groupId, byte customerType, int pageIndex = 0, int pageSize = 10)
        {
            long totalRows = 0;
            string chatContents = "";
            var unitOfWork = new UnitOfWork();

            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);

            // Kiểm tra nhóm đã tồn tại chưa.
            var isGroupChatExist = unitOfWork.GroupChatRepo.IsGroupChatExists(groupId);

            if (!isGroupChatExist)
            {
                var result = unitOfWork.GroupChatRepo.GroupChatSystemInsert(groupId);

                if (result < 0)
                {
                    Clients.Caller.AfterJoinedToGroup(-1, "");
                }
            }

            // Kiểm tra người dùng này đã có trong nhóm chưa.
            if (unitOfWork.GroupChatRepo.GroupChatUserCheckExists(userState.UserId, groupId, customerType))
            {
                // Lấy về nội dung chat của nhóm.
                var result = unitOfWork.GroupChatRepo.GroupChatContentGet(groupId, userState.UserId, customerType, pageIndex, pageSize, out totalRows);

                foreach (var item in result)
                {
                    item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
                }

                chatContents = JsonConvert.SerializeObject(result);
            }
            else
            {
                // Thêm người dùng vào nhóm
                var resultInsert = unitOfWork.GroupChatRepo.GroupChatUserInsert(groupId, userState.UserId,
                    userState.FullName, userState.Avatar, userState.TitleName, userState.OfficeName, userState.UserId, true, customerType, 2,
                    1, userInfo.NotifyUrl, false);

                if (resultInsert > 0)
                {
                    // Lấy về nội dung chat của nhóm.
                    var result = unitOfWork.GroupChatRepo.GroupChatContentGet(groupId, userState.UserId, customerType, pageIndex, pageSize, out totalRows);

                    foreach (var item in result)
                    {
                        item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
                    }
                    chatContents = JsonConvert.SerializeObject(result);
                }
            }

            await Groups.Add(Context.ConnectionId, groupId);
            Clients.Caller.AfterJoinedToGroup(chatContents, totalRows);
        }
        public async Task ModalJoinGroupChat(string groupId, byte customerType, int pageIndex = 0, int pageSize = 10)
        {
            long totalRows = 0;
            string chatContents = "";
            var unitOfWork = new UnitOfWork();

            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }


            var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);
            

            // Kiểm tra nhóm đã tồn tại chưa.
            var isGroupChatExist = unitOfWork.GroupChatRepo.IsGroupChatExists(groupId);

            if (!isGroupChatExist)
            {
                var result = unitOfWork.GroupChatRepo.GroupChatSystemInsert(groupId);

                if (result < 0)
                {
                    Clients.Caller.ModalAfterJoinedToGroup(-1, "");
                }
            }

            // Kiểm tra người dùng này đã có trong nhóm chưa.
            if (unitOfWork.GroupChatRepo.GroupChatUserCheckExists(userState.UserId, groupId, customerType))
            {
                // Lấy về nội dung chat của nhóm.
                var result = unitOfWork.GroupChatRepo.GroupChatContentGet(groupId, userState.UserId, customerType, pageIndex, pageSize, out totalRows);

                foreach (var item in result)
                {
                    item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
                }

                chatContents = JsonConvert.SerializeObject(result);
            }
            else
            {
                var resultInsert = unitOfWork.GroupChatRepo.GroupChatUserInsert(groupId, userState.UserId, userState.FullName, userState.Avatar,
                    userState.TitleName, userState.OfficeName, userState.UserId, true, customerType, 2, 1, userInfo?.NotifyUrl ?? "", false);

                if (resultInsert > 0)
                {
                    // Lấy về nội dung chat của nhóm.
                    var result = unitOfWork.GroupChatRepo.GroupChatContentGet(groupId, userState.UserId, customerType, pageIndex, pageSize, out totalRows);

                    foreach (var item in result)
                    {
                        item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
                    }
                    chatContents = JsonConvert.SerializeObject(result);
                }
            }

            await Groups.Add(Context.ConnectionId, groupId);
            Clients.Caller.ModalAfterJoinedToGroup(chatContents, totalRows);
        }

        public Task LeaveGroupChat(string groupId)
        {
            return Groups.Remove(Context.ConnectionId, groupId);
        }

        [ValidateInput(false)]
        public void PageGroupChatSendMessage(string groupId, string message, byte customerType, List<Attachment> attachments, string pageTitle, string pageUrl)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);

            var listUserTaged = new List<int>();
            var html = new HtmlDocument();
            html.LoadHtml(message);

            var nodes = html.DocumentNode.SelectNodes("a");
            if (nodes != null)
            {
                var userTagedNodes = nodes.Where(x => x.HasAttributes && x.Attributes["class"].Value.Equals("user-tag-item"));
                foreach (var node in userTagedNodes)
                {
                    listUserTaged.Add(int.Parse(node.Attributes["data-id"].Value));
                }
            }

            message = MyCommon.HenryEndcrypt(message, MyCommon.Md5Endcoding(groupId));

            if (attachments != null && attachments.Count > 0)
            {
                message = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));
            }

            var resultInsert = unitOfWork.GroupChatRepo.GroupChatContentInsert(userState.UserId, message, groupId,
                userState.UserName, userState.TitleName, userState.OfficeName, userState.Avatar, userState.FullName,
                customerType, attachments?.Count ?? 0);

            if (resultInsert > 0)
            {
                Clients.Group(groupId).afterSendMessage(JsonConvert.SerializeObject(new
                {
                    Id = resultInsert,
                    GroupId = groupId,
                    ParentId = (int?)null, userState.UserId,
                    NotifyUrl = "", userState.UserName, userState.FullName,
                    Image = userState.Avatar,
                    Content = MyCommon.HenryDecrypt(message, MyCommon.Md5Endcoding(groupId)),
                    Type = customerType,
                    SentTime = DateTime.Now,
                    AttachmentCount = attachments?.Count ?? 0
                }));

                var groupChatName = groupId.Split('_')[0];

                var groupChatId = (GroupChatId)Enum.Parse(typeof(GroupChatId), groupChatName);

                var pageDescription = EnumHelper.GetEnumDescription(groupChatId);
                string title = $"{userState.FullName} bình luận {pageDescription}: \"{pageTitle}\"";

                // Thêm số lượng tn chưa đọc cho người dùng không online.
                var usersUpdated = unitOfWork.GroupChatRepo.GroupChatReadUpdateCount(groupId, resultInsert, userState.UserId);

                foreach (var userUpdate in usersUpdated.Where(x => x.IsShowNotify.HasValue && x.IsShowNotify.Value))
                {
                    //// Gửi Notify đến cho khách hàng.
                    //if (userUpdate.Type == 1)
                    //{
                    //    var notifyContent = string.Format("{0} {1} đã bình luận {2}: \"<a href='{4}'>{3}</a>\"",
                    //        customerType == 0 ? "Nhân viên" : "Khách hàng", userState.FullName, pageDescription, pageTitle, userUpdate.NotifyUrl);
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer(userUpdate.UserId ?? 0, title,
                    //        EnumNotifyType.CustomerInfo, notifyContent, userUpdate.NotifyUrl);
                    //}

                    // Gửi notify đến cho nhân viên.
                    if (userUpdate.Type == 0)
                    {
                        var notifyContent = string.Format("{0} {1} đã bình luận {2}: \"<a href='{4}'>{3}</a>\"",
                             customerType == 0 ? "Nhân viên" : "Khách hàng", userState.FullName, pageDescription, pageTitle, userUpdate.NotifyUrl);
                        NotifyHelper.CreateAndSendNotifySystemToClient(userUpdate.UserId ?? 0, title, EnumNotifyType.Info,
                            notifyContent, userUpdate.NotifyUrl);
                    }
                }

                // Thêm notification cho người dung online.
                var usersOnline = unitOfWork.GroupChatRepo.GroupChatGetListUserForNotification(groupId);

                foreach (var item in usersOnline.Where(x => x.UserId != userState.UserId))
                {
                    //// Gửi Notify đến cho khách hàng.
                    //if (item.UserType == 1)
                    //{
                    //    var notifyContent = string.Format("{0} {1} đã bình luận {2}: \"<a href='{4}'>{3}</a>\"",
                    //        customerType == 0 ? "Nhân viên" : "Khách hàng", userState.FullName, pageDescription, pageTitle, item.NotifyUrl);
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer(item.UserId, title, EnumNotifyType.CustomerInfo, notifyContent, item.NotifyUrl);
                    //}

                    // Gửi notify đến cho nhân viên.
                    if (item.UserType == 0)
                    {
                        var notifyContent = string.Format("{0} {1} đã bình luận {2}: \"<a href='{4}'>{3}</a>\"", customerType == 0 ? "Nhân viên" : "Khách hàng", userState.FullName, pageDescription, pageTitle, item.NotifyUrl);

                        NotifyHelper.CreateAndSendNotifySystemToClient(item.UserId, title, EnumNotifyType.Info, notifyContent, item.NotifyUrl);
                    }
                }

                // Gửi notify đến người được tag.
                foreach (var id in listUserTaged)
                {
                    var tagTitle = $"{userInfo.FullName} vừa nhắc đến bạn trong một bình luận.";
                    var tagContent = $"{userInfo.FullName} vừa nhắc đến bạn trong một bình luận: {MyCommon.HenryDecrypt(message, MyCommon.Md5Endcoding(groupId))} \n xem Detail <a href=\"{pageUrl}\">Tại đây</a>.";

                    NotifyHelper.CreateAndSendNotifySystemToClient(id, tagTitle, EnumNotifyType.Info, tagContent, pageUrl);
                }
            }
            else
            {
                Clients.Group(groupId).afterSendMessage(resultInsert);
            }
        }

        [ValidateInput(false)]
        public void PageModalGroupChatSendMessage(string groupId, string message, byte customerType, List<Attachment> attachments, string pageTitle, string pageUrl)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);

            var listUserTaged = new List<int>();
            var html = new HtmlDocument();
            html.LoadHtml(message);

            var nodes = html.DocumentNode.SelectNodes("a");
            if (nodes != null)
            {
                var userTagedNodes = nodes.Where(x => x.HasAttributes && x.Attributes["class"].Value.Equals("user-tag-item"));
                foreach (var node in userTagedNodes)
                {
                    listUserTaged.Add(int.Parse(node.Attributes["data-id"].Value));
                }
            }

            message = MyCommon.HenryEndcrypt(message, MyCommon.Md5Endcoding(groupId));

            if (attachments != null && attachments.Count > 0)
            {
                message = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));
            }

            var resultInsert = unitOfWork.GroupChatRepo.GroupChatContentInsert(userState.UserId, message, groupId, userState.UserName, 
                userState.TitleName, userState.OfficeName, userState.Avatar, userState.FullName, customerType, attachments?.Count ?? 0);

            if (resultInsert > 0)
            {
                Clients.Group(groupId).modalAfterSendMessage(JsonConvert.SerializeObject(new
                {
                    Id = resultInsert,
                    GroupId = groupId,
                    ParentId = (int?)null, userState.UserId, userInfo.NotifyUrl, userState.UserName, userState.FullName,
                    Image = userState.Avatar,
                    Content = MyCommon.HenryDecrypt(message, MyCommon.Md5Endcoding(groupId)),
                    Type = customerType,
                    SentTime = DateTime.Now,
                    AttachmentCount = attachments?.Count ?? 0
                }));

                //var groupChatName = groupId.Split('_')[0];
                //var groupChatId = (GroupChatId)Enum.Parse(typeof(GroupChatId), groupChatName);
                //var pageDescription = EnumHelper.GetEnumDescription(groupChatId);
                var title = $"{userState.FullName} bình luận: \"{pageTitle}\"";
                var content = $"{userState.FullName} đã bình luận: \"<a href='{userInfo.NotifyUrl}'>{pageTitle}</a>\"";

                // Thêm số lượng tn chưa đọc cho người dùng không online.
                var usersUpdated = unitOfWork.GroupChatRepo.GroupChatReadUpdateCount(groupId, resultInsert, userState.UserId);

                foreach (var userUpdate in usersUpdated.Where(x => x.IsShowNotify.HasValue && x.IsShowNotify.Value))
                {
                    // Gửi Notify đến cho khách hàng.
                    //if (userUpdate.Type == 1)
                    //{
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer((int)userUpdate.UserId, title, EnumNotifyType.CustomerInfo, content, userUpdate.NotifyUrl);
                    //}

                    // Gửi notify đến cho nhân viên.
                    if (userUpdate.Type == 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(userUpdate.UserId ?? 0, title, EnumNotifyType.Info, content, userUpdate.NotifyUrl);
                    }
                }

                // Thêm notification cho người dung online.
                var usersOnline = unitOfWork.GroupChatRepo.GroupChatGetListUserForNotification(groupId);

                foreach (var item in usersOnline.Where(x => x.UserId != userState.UserId))
                {
                    // Gửi Notify đến cho khách hàng.
                    //if (item.UserType == 1)
                    //{
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer((int)item.UserId, title, EnumNotifyType.CustomerInfo, content, item.NotifyUrl);
                    //}

                    // Gửi notify đến cho nhân viên.
                    if (item.UserType == 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(item.UserId, title, EnumNotifyType.Info, content, item.NotifyUrl);
                    }
                }

                // Gửi notify đến người được tag.
                foreach (var id in listUserTaged)
                {
                    var tagTitle = $"{userInfo.FullName} vừa nhắc đến bạn trong một bình luận.";
                    var tagContent =
                        $"{userInfo.FullName} vừa nhắc đến bạn trong một bình luận: {MyCommon.HenryDecrypt(message, MyCommon.Md5Endcoding(groupId))} \n xem Detail <a href=\"{pageUrl}\">Tại đây</a>.";
                    NotifyHelper.CreateAndSendNotifySystemToClient(id, tagTitle, EnumNotifyType.Info, tagContent, pageUrl);
                }
            }
            else
            {
                Clients.Group(groupId).modalAfterSendMessage(resultInsert);
            }
        }

        public void RemoveAttachmentFile(string groupId, long attachmentId, long chatId, byte customerType)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            // Kiểm tra người dùng có thuộc nhóm chat này không.
            var isUserExists = unitOfWork.GroupChatRepo.GroupChatUserCheckExists(userState.UserId, groupId, customerType);

            if (!isUserExists)
            {
                Clients.Caller.AfterRemoveAttachmentFile(-1);
            }
            else
            {
                // Kiểm tra tin nhắn này có phải của người dùng hiện tại.
                var groupChatContentInfo = unitOfWork.GroupChatRepo.GroupChatContentGetByUserId(userState.UserId, customerType, groupId, chatId);

                if (groupChatContentInfo == null)
                {
                    Clients.Caller.AfterRemoveAttachmentFile(-1);
                }
                else
                {
                    var attachments = JsonConvert.DeserializeObject<List<Attachment>>(MyCommon.HenryDecrypt(groupChatContentInfo.Content, MyCommon.Md5Endcoding(groupId)));

                    attachments?.Remove(attachments.FirstOrDefault(x => x.Id == attachmentId));

                    if ((attachments?.Count ?? 0) > 0)
                    {
                        // Upate lại danh sách yuanp tin đính kèm sau khi xóa.
                        groupChatContentInfo.AttachmentCount = attachments?.Count ?? 0;
                        groupChatContentInfo.Content = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));

                        unitOfWork.GroupChatRepo.GroupChatContentUpdateAttachment(groupChatContentInfo);

                        Clients.Group(groupId).AfterRemoveAttachmentFile(attachmentId, chatId, attachments?.Count ?? 0);
                    }
                    else
                    {
                        var resultRemove = unitOfWork.GroupChatRepo.GroupChatContentDelete(groupChatContentInfo.Id, userState.UserId, customerType);

                        if (resultRemove < 0)
                            Clients.Caller.AfterRemoveAttachmentFile(-1);
                        else
                            Clients.Group(groupId).AfterRemoveChatContent(groupChatContentInfo.ParentId, groupChatContentInfo.Id);
                    }
                }
            }
        }

        public void ModalRemoveAttachmentFile(string groupId, long attachmentId, long chatId, byte customerType)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            // Kiểm tra người dùng có thuộc nhóm chat này không.
            var isUserExists = unitOfWork.GroupChatRepo.GroupChatUserCheckExists(userState.UserId, groupId, customerType);

            if (!isUserExists)
            {
                Clients.Caller.ModalAfterRemoveAttachmentFile(-1);
            }
            else
            {
                // Kiểm tra tin nhắn này có phải của người dùng hiện tại.
                var groupChatContentInfo = unitOfWork.GroupChatRepo.GroupChatContentGetByUserId(userState.UserId, customerType, groupId, chatId);

                if (groupChatContentInfo == null)
                {
                    Clients.Caller.ModalAfterRemoveAttachmentFile(-1);
                }
                else
                {
                    var attachments = JsonConvert.DeserializeObject<List<Attachment>>(MyCommon.HenryDecrypt(groupChatContentInfo.Content, MyCommon.Md5Endcoding(groupId)));

                    attachments?.Remove(attachments.FirstOrDefault(x => x.Id == attachmentId));

                    if (attachments != null && attachments.Count > 0)
                    {
                        // Upate lại danh sách yuanp tin đính kèm sau khi xóa.
                        groupChatContentInfo.AttachmentCount = attachments.Count;
                        groupChatContentInfo.Content = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));

                        unitOfWork.GroupChatRepo.GroupChatContentUpdateAttachment(groupChatContentInfo);

                        Clients.Group(groupId).ModalAfterRemoveAttachmentFile(attachmentId, chatId, attachments.Count);
                    }
                    else
                    {
                        var resultRemove = unitOfWork.GroupChatRepo.GroupChatContentDelete(groupChatContentInfo.Id, userState.UserId, customerType);

                        if (resultRemove < 0)
                            Clients.Caller.AfterRemoveAttachmentFile(-1);
                        else
                            Clients.Group(groupId).ModalAfterRemoveChatContent(groupChatContentInfo.ParentId, groupChatContentInfo.Id);
                    }
                }
            }
        }

        public void GroupChatReply(long contentId, string groupId, string content, byte userType, List<Attachment> attachments)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            if (userType == 0)
            {
                var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);

                if (userInfo == null)
                    Clients.Caller.afterReply(-1);
            }
            //else
            //{
            //    var customerInfo = GetCustomerInfo(Context.User.Identity.Name);

            //    if (customerInfo == null)
            //        Clients.Caller.afterReply(-1);

            //    userState.UserId = customerInfo.Id;
            //    userName = customerInfo.UserName;
            //    userState.FullName = customerInfo.Name;
            //    image = null;
            //}

            if (attachments != null && attachments.Count > 0)
            {
                content = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));
            }
            else
            {
                content = MyCommon.HenryEndcrypt(content, MyCommon.Md5Endcoding(groupId));
            }

            var result = unitOfWork.GroupChatRepo.GroupChatContentReplyInsert(contentId, userState.UserId, groupId, content, userState.UserName, userState.FullName,
                userState.TitleName, userState.OfficeName, userState.Avatar, userType, attachments?.Count ?? 0, false);

            Clients.Group(groupId).afterReply(JsonConvert.SerializeObject(new
            {
                Id = result,
                ParentId = contentId, userState.UserId, userState.UserName, userState.FullName,
                Image = userState.Avatar,
                Type = userType,
                Content = MyCommon.HenryDecrypt(content, MyCommon.Md5Endcoding(groupId)),
                SentTime = DateTime.Now,
                AttachmentCount = attachments?.Count ?? 0
            }));

            if (result > 0)
            {
                // Lấy về thông tin người dùng theo nội dung tin nhắn.
                var contentInfo = unitOfWork.GroupChatRepo.GroupChatContentGetById(contentId);

                if (contentInfo == null)
                    return;

                // Gửi notifycation cho người tạo
                string title = $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận của bạn";
                string notifyContent;

                if (contentInfo.AttachmentCount != null && contentInfo.AttachmentCount > 0)
                {
                    notifyContent =
                        $"{title} - <a href='{(contentInfo.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}{contentId}'>Detail</a>";
                }
                else
                {
                    notifyContent =
                        $"{MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId))} <i class='fa fa-long-arrow-right'></i> {MyCommon.HenryDecrypt(content, MyCommon.Md5Endcoding(groupId))}";
                }

                var directNotifyUrl =
                    $"{contentInfo.NotifyUrl}{(contentInfo.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}target={contentInfo.Id}";
                if (contentInfo.Type == 0 && contentInfo.UserId != userState.UserId)
                {
                    NotifyHelper.CreateAndSendNotifySystemToClient(contentInfo.UserId, title, EnumNotifyType.Info, notifyContent, directNotifyUrl);
                }

                //if (contentInfo.Type == 1 && contentInfo.UserId != userState.UserId)
                //{
                //    NotifyHelper.CreateAndSendNotifySystemToCustomer(contentInfo.UserId, title, EnumNotifyType.CustomerInfo, notifyContent, directNotifyUrl);
                //}

                // Gửi notification cho những người cùng trả lời bình luận này và khác user của người tạo bình luận và người dùng hiện tại
                var listUserReply = unitOfWork.GroupChatRepo.GroupChatContentGetListUserIdReply(contentInfo.Id, groupId, 
                    (gc, gu) => new UserReplyMeta() { UserId = gc.UserId, Type = gc.Type, NotifyUrl = gu.NotifyUrl });

                foreach (var item in listUserReply.Where(x => x.UserId != contentInfo.UserId && x.UserId != userState.UserId))
                {
                    string replyTitle, notifyForUserContent;

                    if (contentInfo.AttachmentCount != null && contentInfo.AttachmentCount > 0)
                    {
                        replyTitle = $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận yuanp tin đính kèm";
                        notifyForUserContent =
                            $"{replyTitle} - <a href='{(item.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}{contentId}'>Detail</a>";
                    }
                    else
                    {
                        replyTitle =
                            $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận: \"{MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId))}\"";
                        notifyForUserContent = MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId));
                    }

                    var userDirectNotityUrl =
                        $"{item.NotifyUrl}{(item.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}target={contentInfo.Id}";
                    if (item.Type == 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(item.UserId, replyTitle, EnumNotifyType.Info, notifyForUserContent, userDirectNotityUrl);
                    }
                    //else
                    //{
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer(item.UserId, replyTitle, EnumNotifyType.CustomerInfo, notifyForUserContent, userDirectNotityUrl);
                    //}
                }
            }
        }

        public void GroupChatModalReply(long contentId, string groupId, string content, byte userType, List<Attachment> attachments)
        {
            var unitOfWork = new UnitOfWork();
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            if (userType == 0)
            {
                var userInfo = unitOfWork.UserRepo.GroupChatGetUserByUserName(userState.UserName);

                if (userInfo == null)
                    Clients.Caller.modalAfterReply(-1);
            }
            //else
            //{
            //    var customerInfo = GetCustomerInfo(Context.User.Identity.Name);

            //    if (customerInfo == null)
            //        Clients.Caller.modalAfterReply(-1);

            //    userState.UserId = customerInfo.Id;
            //    userName = customerInfo.UserName;
            //    userState.FullName = customerInfo.Name;
            //    image = null;
            //}

            if (attachments != null && attachments.Count > 0)
            {
                content = MyCommon.HenryEndcrypt(JsonConvert.SerializeObject(attachments), MyCommon.Md5Endcoding(groupId));
            }
            else
            {
                content = MyCommon.HenryEndcrypt(content, MyCommon.Md5Endcoding(groupId));
            }

            var result = unitOfWork.GroupChatRepo.GroupChatContentReplyInsert(contentId, userState.UserId, groupId, content, 
                userState.UserName, userState.FullName, userState.TitleName, userState.OfficeName, userState.Avatar, userType, attachments?.Count ?? 0, false);

            Clients.Group(groupId).modalAfterReply(JsonConvert.SerializeObject(new
            {
                Id = result,
                ParentId = contentId, userState.UserId, userState.UserName, userState.FullName,
                Image = userState.Avatar,
                Type = userType,
                Content = MyCommon.HenryDecrypt(content, MyCommon.Md5Endcoding(groupId)),
                SentTime = DateTime.Now,
                AttachmentCount = attachments?.Count ?? 0
            }));

            if (result > 0)
            {
                // Lấy về thông tin người dùng theo nội dung tin nhắn.
                var contentInfo = unitOfWork.GroupChatRepo.GroupChatContentGetById(contentId);

                if (contentInfo == null)
                    return;

                // Gửi notifycation cho người tạo
                string title = $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận của bạn";
                string notifyContent;

                if (contentInfo.AttachmentCount != null && contentInfo.AttachmentCount > 0)
                {
                    notifyContent =
                        $"{title} - <a href='{(contentInfo.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}{contentId}'>Detail</a>";
                }
                else
                {
                    notifyContent =
                        $"{MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId))} <i class='fa fa-long-arrow-right'></i> {MyCommon.HenryDecrypt(content, MyCommon.Md5Endcoding(groupId))}";
                }

                var directNotifyUrl =
                    $"{contentInfo.NotifyUrl}{(contentInfo.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}target={contentInfo.Id}";
                if (contentInfo.Type == 0 && contentInfo.UserId != userState.UserId)
                {
                    NotifyHelper.CreateAndSendNotifySystemToClient(contentInfo.UserId, title, EnumNotifyType.Info, notifyContent, directNotifyUrl);
                }

                //if (contentInfo.Type == 1 && contentInfo.UserId != userState.UserId)
                //{
                //    NotifyHelper.CreateAndSendNotifySystemToCustomer(contentInfo.UserId, title, EnumNotifyType.CustomerInfo, notifyContent, directNotifyUrl);
                //}

                // Gửi notification cho những người cùng trả lời bình luận này và khác user của người tạo bình luận và người dùng hiện tại
                var listUserReply = unitOfWork.GroupChatRepo.GroupChatContentGetListUserIdReply(contentInfo.Id, groupId, (gc, gu) => new UserReplyMeta() { UserId = gc.UserId, Type = gc.Type, NotifyUrl = gu.NotifyUrl });

                foreach (var item in listUserReply.Where(x => x.UserId != contentInfo.UserId && x.UserId != userState.UserId))
                {
                    string replyTitle, notifyForUserContent;

                    if (contentInfo.AttachmentCount != null && contentInfo.AttachmentCount > 0)
                    {
                        replyTitle = $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận yuanp tin đính kèm";
                        notifyForUserContent =
                            $"{replyTitle} - <a href='{(item.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}{contentId}'>Detail</a>";
                    }
                    else
                    {
                        replyTitle =
                            $"{(userType == 0 ? "" : "KH ")}{userState.FullName} đã trả lời bình luận: \"{MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId))}\"";
                        notifyForUserContent = MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId));
                    }

                    var userDirectNotityUrl =
                        $"{item.NotifyUrl}{(item.NotifyUrl.IndexOf("?", StringComparison.Ordinal) > 0 ? "&target=" : "?target=")}target={contentInfo.Id}";
                    if (item.Type == 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(item.UserId, replyTitle, EnumNotifyType.Info, notifyForUserContent, userDirectNotityUrl);
                    }
                    //else
                    //{
                    //    NotifyHelper.CreateAndSendNotifySystemToCustomer(item.UserId, replyTitle, EnumNotifyType.CustomerInfo, notifyForUserContent, userDirectNotityUrl);
                    //}
                }
            }
        }
    }
}