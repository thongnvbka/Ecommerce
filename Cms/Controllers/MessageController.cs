using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using Common.Helper;
using System.Net.Mime;
using System.Xml;
using Cms.Helpers;
using Library.Models;
using Newtonsoft.Json;
using Ganss.XSS;

namespace Cms.Controllers
{

    public class MessageController : BaseController
    {
        // Edited by UserState: Add currentPage to return
        public JsonResult SearchInbox(string keyword, bool? isRead, bool? star, int currentPage, int recordPerPage)
        {
            int totalRecord;
            var listMessageInbox = UnitOfWork.MessageRealTimeRepo.SearchInbox(UserState.UserId, keyword, isRead, star, currentPage, recordPerPage, out totalRecord);
            return Json(new { items = listMessageInbox, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        // Edited by UserState: Add currentPage to return
        public JsonResult SearchSent(string keyword, bool? star, bool? isRead, int currentPage, int recordPerPage)
        {
            int totalRecord;
            var listMessageInbox = UnitOfWork.MessageRealTimeRepo.SearchSent(UserState.UserId, keyword, star, isRead, currentPage, recordPerPage, out totalRecord);
            return Json(new { items = listMessageInbox, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        // Edited by @Henry: Add currentPage to return
        public JsonResult SearchDraft(string keyword, bool? star, int currentPage, int recordPerPage)
        {
            int totalRecord;
            var listMessageInbox = UnitOfWork.MessageRealTimeRepo.SearchDraft(UserState.UserId, keyword, star, currentPage, recordPerPage, out totalRecord);
            return Json(new { items = listMessageInbox, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        // Edited by @Henry: Add currentPage to return
        public JsonResult SearchTrash(string keyword, bool? isRead, bool? star, int currentPage, int recordPerPage)
        {
            int totalRecord;
            var listMessageInbox = UnitOfWork.MessageRealTimeRepo.SearchTrash(UserState.UserId, keyword, isRead, star, currentPage, recordPerPage, out totalRecord);
            return Json(new { items = listMessageInbox, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        // Edited by @Henry: Add currentPage to return
        public JsonResult SearchStar(string keyword, bool? isRead, int currentPage, int recordPerPage)
        {
            int totalRecord;
            var listMessageInbox = UnitOfWork.MessageRealTimeRepo.SearchStar(UserState.UserId, keyword, isRead, currentPage, recordPerPage, out totalRecord);
            return Json(new { items = listMessageInbox, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Compose(long? messageIdForward)
        {
            var fromUser =
                JsonConvert.SerializeObject(
                    new
                    {
                        fullName = UserState.FullName,
                        userName = UserState.UserName,
                        userId = UserState.UserId
                    });

            var result = UnitOfWork.MessageRealTimeRepo.InsertTemp(fromUser, UserState.UserId, UserState.Avatar,
                $"{UserState.FullName} {UserState.UserName}", messageIdForward);

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Discard(long[] ids)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var discardSuccessId = new List<long>();
            var messageRepository = UnitOfWork.MessageRealTimeRepo;
            foreach (var id in ids)
            {
                var result = messageRepository.Discard(id, UserState.UserId);
                if (result > 0)
                {
                    discardSuccessId.Add(id);
                }
            }
            return Json(discardSuccessId);
            //var result = UnitOfWork.MessageRealTimeRepo.Discard(id, UserState.UserId);
            //return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SaveDraft(MessageMeta message)
        {
            var result = UnitOfWork.MessageRealTimeRepo.SaveDraft(message.Id, UserState.UserId, message.To, message.Cc, message.Bcc,
                message.Title, MyCommon.Ucs2Convert(message.Title), new HtmlSanitizer().Sanitize(message.Content));
            return Json(result);
        }

        public JsonResult Detail(long id)
        {
            var message = UnitOfWork.MessageRealTimeRepo.GetById(id, UserState.UserId);
            if (message == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            if (message.FromUserId != UserState.UserId)
            {
                message.BccToUser = string.Empty;
            }
            message.Content = string.IsNullOrWhiteSpace(message.Content) ? string.Empty : message.Content;

            var attachments = UnitOfWork.AttachmentRepo.GetByMessageId(message.Id);

            var messageUser =
                UnitOfWork.MessageUserRepo.SingleOrDefault(
                    x => x.MessageId == message.Id && x.UserId == UserState.UserId);

            if (!messageUser.IsRead)
            {
                UnitOfWork.MessageRealTimeRepo.MarkReaded(id, UserState.UserId, true);

                // Thông báo xuống Client Message đã được đọc
                NotifyHelper.UpdateTotalMessageToClient(UserState.UserId, id);
            }

            return
                Json(
                    new
                    {
                        content = message.Content,
                        bcc = message.BccToUser,
                        attachments
                    }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SendMessage(MessageMeta message)
        {
            if (string.IsNullOrWhiteSpace(message.To))
            {
                return Json(-3);
            }

            if (string.IsNullOrWhiteSpace(message.Title))
            {
                return Json(-4);
            }

            if (string.IsNullOrWhiteSpace(message.Content))
            {
                return Json(-5);
            }

            char[] delimiterChars = { ',', ';' };
            var listUserNameTo = message.To.Replace(" ", "").ToLower().Split(delimiterChars);
            var listUserNameCc = string.IsNullOrWhiteSpace(message.Cc) ? null : message.Cc.Replace(" ", "").ToLower().Split(delimiterChars);
            var listUserNameBcc = string.IsNullOrWhiteSpace(message.Bcc) ? null : message.Bcc.Replace(" ", "").ToLower().Split(delimiterChars);
            var allUserName = listUserNameTo;

            if (listUserNameCc != null)
            {
                allUserName = allUserName.Concat(listUserNameCc).ToArray();
            }

            if (listUserNameBcc != null)
            {
                allUserName = allUserName.Concat(listUserNameBcc).ToArray();
            }

            allUserName = allUserName.Distinct().ToArray();

            var allUser =
                UnitOfWork.UserRepo.GetBySpec(
                    u => new { userId = u.Id, userName = u.UserName, fullName = u.FullName, email = u.Email },
                    u => !u.IsDelete && allUserName.Contains(u.UserName));

            if (!allUser.Any())
            {
                return Json(allUserName);
            }

            if (allUser.Count() < allUserName.Length)
            {
                var userNameValid = allUser.Select(u => u.userName);
                var userNameNotExits = allUserName.Where(a => !userNameValid.Contains(a));
                return Json(userNameNotExits);
            }

            var listUserTo = allUser.Where(u => listUserNameTo.Contains(u.userName)).ToList();

            if (!listUserTo.Any())
            {
                return Json(-6);
            }
            var listUserCc = listUserNameCc == null ? null : allUser.Where(u => listUserNameCc.Contains(u.userName));
            var listUserBcc = listUserNameBcc == null ? null : allUser.Where(u => listUserNameBcc.Contains(u.userName));

            var unsignTitle = MyCommon.Ucs2Convert(
                $"{message.Title} {User.Identity.Name} {UserState.FullName} {string.Join(" ", allUser.Select(u => u.userName).ToList())} {string.Join(" ", allUser.Select(u => u.fullName).ToList())}");

            int result;
            var messageUser = UnitOfWork.MessageRealTimeRepo.Send(message.Id, UserState.UserId, JsonConvert.SerializeObject(listUserTo),
                listUserCc == null ? null : JsonConvert.SerializeObject(listUserCc),
                listUserBcc == null ? null : JsonConvert.SerializeObject(listUserBcc), message.Title, unsignTitle, new HtmlSanitizer().Sanitize(message.Content),
                allUser.Select(x=> x.userId).ToList(), out result);

            if (result > 0)
            {
                NotifyHelper.SendMessageToClient(messageUser, UserState.FullName, message.Title,
                    UserState.Avatar, DateTime.Now);

                if (message.IsSendOut)
                {
                    // Kiểm tra có tệp tin đính kèm không
                    var attachments = UnitOfWork.MessageRealTimeRepo.GetAttachmentsByMessageId(message.Id);
                    List<UserObject> listTo = new List<UserObject>();
                    List<UserObject> listCc = new List<UserObject>();
                    List<UserObject> listBcc = new List<UserObject>();

                    if (listUserTo != null)
                    {
                        foreach (var item in listUserTo)
                        {
                            listTo.Add(new UserObject { FullName = item.fullName, Email = item.email });
                        }
                    }

                    if (listUserCc != null)
                    {
                        foreach (var item in listUserCc)
                        {
                            listCc.Add(new UserObject { FullName = item.fullName, Email = item.email });
                        }
                    }

                    if (listUserBcc != null)
                    {
                        foreach (var item in listUserBcc)
                        {
                            listBcc.Add(new UserObject { FullName = item.fullName, Email = item.email });
                        }
                    }

                    // Thêm vào bảng để gửi thư ra bên ngoài
                    var sendEmail = new SendEmailMeta
                    {
                        FromUserId = UserState.UserId,
                        FromUserName = UserState.UserName,
                        FromUserFullName = UserState.FullName,
                        To = listTo,
                        Cc = listCc,
                        Bcc = listBcc,
                        Title = message.Title,
                        Content = new HtmlSanitizer().Sanitize(message.Content),
                        Status = 0, // Pending
                        Type = 0,
                        AttachmentCount = attachments != null ? (short)attachments.Count : (short)0,
                        Attachments = attachments != null && attachments.Count > 0 ? JsonConvert.SerializeObject(attachments) : string.Empty
                    };

                    var resultInsert = UnitOfWork.SendEmailRepo.SendEmailInsert(sendEmail);
                }
            }

            return Json(result);
        }

        public ActionResult DownloadAttachment(long id, long messageId)
        {
            var attachment = UnitOfWork.MessageRealTimeRepo.DownloadAttachment(messageId, id, UserState.UserId);
            if (attachment == null)
            {
                return Content("Attached file does not exist or has been deleted");
            }

            var path = Encryptor.Base64Decode(attachment.AttachmentPath);
            var filePath = Server.MapPath(path);

            if (!System.IO.File.Exists(filePath))
            {
                return Content("Attached file does not exist or has been deleted");
            }
            return File(filePath, MediaTypeNames.Application.Octet, attachment.AttachmentName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RemoveAttachment(long id, long messageId)
        {
            var result = UnitOfWork.AttachmentRepo.DeleteInMessage(id, messageId, UserState.UserId);

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MoveTrash(long[] ids)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var moveSuccessId = new List<long>();
            var messageRepository = UnitOfWork.MessageRealTimeRepo;
            foreach (var id in ids)
            {
                var result = messageRepository.MoveTrash(id, UserState.UserId);
                if (result > 0)
                {
                    moveSuccessId.Add(id);
                }
            }
            return Json(moveSuccessId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UndoMoveTrash(long[] ids)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var undoMoveSuccessId = new List<long>();
            var messageRepository = UnitOfWork.MessageRealTimeRepo;
            foreach (var id in ids)
            {
                var result = messageRepository.UndoMoveTrash(id, UserState.UserId);
                if (result > 0)
                {
                    undoMoveSuccessId.Add(id);
                }
            }
            return Json(undoMoveSuccessId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(long[] ids)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var deleteSuccess = new List<long>();
            var messageRepository = UnitOfWork.MessageRealTimeRepo;
            foreach (var id in ids)
            {
                var result = messageRepository.DeleteTrash(id, UserState.UserId);
                if (result > 0)
                {
                    deleteSuccess.Add(id);
                }
            }
            return Json(deleteSuccess);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MarkReaded(long[] ids, bool isRead)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var success = new List<long>();
            var messageRepository = UnitOfWork.MessageRealTimeRepo;
            foreach (var id in ids)
            {
                var result = messageRepository.MarkReaded(id, UserState.UserId, isRead);
                if (result > 0)
                {
                    success.Add(id);

                    if (isRead)
                    {
                        // Thông báo xuống Client Message đã được đọc
                        NotifyHelper.UpdateTotalMessageToClient(UserState.UserId, id);
                    }
                }
            }
            return Json(success);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetStar(long id, bool star)
        {
            return Json(UnitOfWork.MessageRealTimeRepo.SetStar(id, UserState.UserId, star));
        }


        // @Henry: Remove -> Modal
        //public ActionResult DetailMessage(long id)
        //{
        //    TempData["DetailMessageId"] = id;
        //    return RedirectToAction("Index");
        //}

        public JsonResult GetDetailById(int id)
        {
            var message = UnitOfWork.MessageRealTimeRepo.GetDetailById(id, UserState.UserId);
            if (message == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOfficeAndUser()
        {
            var listOffice = UnitOfWork.OfficeRepo.GetAll(o => new {o.Id, o.Name, o.ParentId, o.IdPath}).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == null || o.ParentId.Value == -1  ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath,
                state = new {opened = o.ParentId == null || o.ParentId.Value == -1 }
            });

            var listUser = UnitOfWork.UserRepo.GetBySpec(
                    u => new { u.Id, u.FullName, u.UserName }, u => !u.IsDelete);

            return Json(new { listOffice, listUser }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByOfficeId(int id, string idPath)
        {
            idPath = string.IsNullOrWhiteSpace(idPath) ? "1." : idPath + ".";

            var listUser = UnitOfWork.UserRepo.GetBySpec(
                    (u, ut) => new { u.Id, u.FullName, u.UserName },
                    u => !u.IsDelete, ut => ut.OfficeIdPath.StartsWith(idPath) || ut.OfficeId == id);

            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SearchSendEmail(string keyword, int page = 1, int pageSize = 20)
        //{
        //    int totalRows;
        //    keyword = MyCommon.Ucs2Convert(keyword);
        //    var items = UnitOfWork.MessageRealTimeRepo.SendEmailSearch(keyword, page, pageSize, out totalRows);

        //    if (Request.IsAjaxRequest())
        //    {
        //        return Json(new { items, totalRows }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        ViewBag.Result = JsonConvert.SerializeObject(new { items, totalRows });
        //        return View();
        //    }
        //}

        //[HttpPost]
        //public JsonResult SendEmailChangeStatus(int id, byte status)
        //{
        //    if (status == (byte)1 || status == (byte)2 || status == (byte)3)
        //        return Json(-4);

        //    return Json(new SendEmailRepository().SendMessageChangeStatus(id, status));
        //}
    }
}