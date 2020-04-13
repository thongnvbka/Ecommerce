using System.Web.Mvc;
using Common.Helper;
using Common.Emums;
using System.Net.Mime;
using Cms.Helpers;
using Cms.Hubs;
using Microsoft.AspNet.SignalR;

namespace Cms.Controllers
{
    [System.Web.Mvc.Authorize]
    public class GroupChatController : BaseController
    {
        public JsonResult GroupChatContentLoadMore(string groupId, byte type, int page = 0, int pageSize = 10)
        {
            // Kiểm tra người dùng có quyền load về cuộc hội thoại không.
            var isHasPermission = UnitOfWork.GroupChatRepo.GroupChatUserCheckExists(UserState.UserId, groupId, type);

            if (!isHasPermission)
                return Json(-1);

            long totalRows;
            var items = UnitOfWork.GroupChatRepo.GroupChatContentGet(groupId, UserState.UserId, type, page, pageSize, out totalRows);

            foreach (var item in items)
            {
                item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
            }

            return Json(new { items, totalRows });
        }

        public JsonResult GroupChatLike(long contentId, string groupId, byte userType, bool isLike)
        {
            var result = UnitOfWork.GroupChatRepo.GroupChatLikeSave(contentId, groupId, UserState.UserId, UserState.UserName, UserState.FullName, UserState.Avatar, userType, isLike);

            var contentInfo = UnitOfWork.GroupChatRepo.GroupChatContentGetById(contentId);
            if (contentInfo != null)
                GroupChatContentAfterLike(groupId, UserState.UserId, contentId, isLike, contentInfo.ParentId);

            if (result > 0 && isLike && contentInfo != null)
            {
                var title = $"{UserState.FullName} liked your comment";
                var content =
                    $"{UserState.FullName} liked comment \"{MyCommon.HenryDecrypt(contentInfo.Content, MyCommon.Md5Endcoding(groupId))}\" of yours";

                if (contentInfo.UserId == UserState.UserId)
                    return Json(result);

                NotifyHelper.CreateAndSendNotifySystemToClient(contentInfo.UserId, title, EnumNotifyType.Info, content, contentInfo.NotifyUrl);
            }

            return Json(result);
        }

        public JsonResult GroupChatGetListUserLiked(long contentId, string groupId, byte userType)
        {
            // Kiểm tra người dùng có quyền lấy danh sách nhân sự trong nhóm này không.
            var isHasPermission = UnitOfWork.GroupChatRepo.GroupChatUserCheckExists(UserState.UserId, groupId, userType);

            if (!isHasPermission)
                return Json(-1);

            // Lấy về danh sách người dùng đã like tin nhắn này.
            return Json(UnitOfWork.GroupChatRepo.GroupChatGetUserLiked(contentId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetListGroupChatReplies(long contentId, string groupId, byte userType, int page = 0, int pageSize = 10)
        {
            // Kiểm tra người dùng có quyền lấy danh sách nhân sự trong nhóm này không.
            var isHasPermission = UnitOfWork.GroupChatRepo.GroupChatUserCheckExists(UserState.UserId, groupId, userType);

            if (!isHasPermission)
                return Json(-1);

            var result = UnitOfWork.GroupChatRepo.GroupChatContentGetListReplies(UserState.UserId, contentId, userType, page, pageSize);
            foreach (var item in result)
            {
                item.Content = MyCommon.HenryDecrypt(item.Content, MyCommon.Md5Endcoding(groupId));
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadAttachment(long attachmentId, byte userType, string groupChatId)
        {
            // Kiểm tra người dùng có tồn tại trong nhóm này không
            var isHasPermission = UnitOfWork.GroupChatRepo.GroupChatUserCheckExists(UserState.UserId, groupChatId, userType);

            if (!isHasPermission)
            {
                return Content("You do not have permission to perform this function");
            }

            var attachmentInfo = UnitOfWork.AttachmentRepo.GetById(attachmentId);

            if (attachmentInfo == null)
            {
                return Content("The file does not exist");
            }

            var path = Encryptor.Base64Decode(attachmentInfo.AttachmentPath);
            var filePath = Server.MapPath(path);

            if (!System.IO.File.Exists(filePath))
            {
                return Content("The file does not exist or has been deleted");
            }

            return File(filePath, MediaTypeNames.Application.Octet, attachmentInfo.AttachmentName);
        }

        private void GroupChatContentAfterLike(string groupId, int userId, long contentId, bool isLike, long? chatId)
        {
            var listUserConnection = UnitOfWork.GroupChatRepo.GetUserConnectionInGroup(groupId, userId);
            var chatContext = GlobalHost.ConnectionManager.GetHubContext<GroupChatHub>();
            foreach (var item in listUserConnection)
            {
                chatContext.Clients.Client(item.ConnectionId).afterLike(contentId, isLike, chatId);
            }
        }

        [HttpPost]
        public JsonResult StopReceiveNotification(string groupId, byte userType, bool isShowNotify)
        {
            // Kiểm tra người dùng có quyền trong nhóm này không.
            var isHasPermission = UnitOfWork.GroupChatRepo.GroupChatUserCheckExists(UserState.UserId, groupId, userType);

            if (!isHasPermission)
                return Json(-2);

            // Cập nhập lại trạng thái nhận thông báo hay không.
            return Json(UnitOfWork.GroupChatRepo.GroupChatUserUpdateIsShowNotify(groupId, UserState.UserId, userType, isShowNotify));
        }
    }
}