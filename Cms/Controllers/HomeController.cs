using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Cms.Helpers;
using Common.Emums;
using Library.DbContext.Results;

namespace Cms.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.Home)]
        public ActionResult Index()
        {
            var permissionActions = UnitOfWork.PermissionActionRepo.GetPermissionByUserId1(UserState.UserId, UserState.TitleId ?? 0, UserState.OfficeId ?? 0);

            // Thêm quyền mặc định vào danh sách quyền
            permissionActions.AddRange(UnitOfWork.PermissionActionRepo.GetPermissionDefault());

            var navPages = permissionActions.Where(x => x.RoleActionId == (byte)EnumAction.View).ToList();
            var moduleIds = navPages.Select(x => x.ModuleIdPath).Distinct().ToList();

            ViewBag.NavPages = navPages;
            ViewBag.NavModules = UnitOfWork.ModuleRepo.Find(x => moduleIds.Any(mId => mId == x.IdPath) || moduleIds.Any(mId => mId.StartsWith(x.IdPath + ".")));
            ViewBag.NavActivePages = new List<PermissionActionResult>();

            return View();
        }
        public ActionResult Add()
        {
            return View();
        }
        public ActionResult Slidebar()
        {
            return PartialView();
        }

        public JsonResult GetMyNotifications(string keyword, bool? isRead, byte? type, int currentPage = 1, int recordPerPage = 20)
        {
            int totalRecord;
            var notifications = UnitOfWork.NotifyRealTimeRepo.GetNotifySystemByToUserId(keyword, UserState.UserId,
                isRead, type, currentPage, recordPerPage, out totalRecord);

            return Json(new { items = notifications, totalRecord, currentPage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastNotificationsAndMessages()
        {
            int totalRecordNotifySystemUnread, totalMessageUnRead;
            var notifications = UnitOfWork.NotifyRealTimeRepo.GetNotifySystemByToUserId(string.Empty, UserState.UserId, false, null, 1, 20, out totalRecordNotifySystemUnread);
            var messages = UnitOfWork.MessageRealTimeRepo.GetNotify(UserState.UserId, 1, 20, out totalMessageUnRead);

            return Json(new
            {
                notifications,
                messages,
                totalRecordNotifySystemUnread,
                totalMessageUnRead
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult NotifyUpdateIsRead(int id)
        {
            if (User == null)
            {
                return Json(-3);
            }

            var notify = UnitOfWork.NotifyRealTimeRepo.GetById(id, UserState.UserId);

            if (notify == null)
            {
                return Json(-1);
            }

            UnitOfWork.NotifyRealTimeRepo.UpdateIsRead(notify);

            // Thông báo xuống Client Notify đã được đọc
            NotifyHelper.UpdateTotalNotifySystemUnreadToClient(notify.ToUserId, notify.Id);

            return Json(1);
        }

        [HttpPost]
        public JsonResult NotifyMarkReaded(long[] ids, bool isRead)
        {
            if (ids == null)
            {
                return Json(null);
            }
            var success = new List<long>();

            foreach (long id in ids)
            {
                var result = UnitOfWork.NotifyRealTimeRepo.MarkAsReaded(UserState.UserId, isRead, id);
                if (result > 0)
                {
                    success.Add(id);

                    if (isRead)
                    {
                        // Thông báo xuống Client Notify đã được đọc
                        NotifyHelper.UpdateTotalNotifySystemUnreadToClient(UserState.UserId, id);
                    }
                }
            }

            return Json(success);
        }
    }
}