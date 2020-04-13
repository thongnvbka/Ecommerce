using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Common.ActionResult;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;
using Newtonsoft.Json;

namespace Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LogTrackerAttribute : ActionFilterAttribute
    {
        private EnumPage[] Pages { get; set; }
        private EnumAction Action { get; set; }

        /// <summary>
        /// Check quyền thao tác của user đang đăng nhập
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pages">Dictionary với key là ActionId value là mảng các trang</param>
        public LogTrackerAttribute(EnumAction action, params EnumPage[] pages)
        {
            Pages = pages;
            Action = action;
        }

        private int GetMaxFileLength()
        {
            var maxLengthConfig = ConfigurationManager.AppSettings["MaxFileLength"];
            int maxLength;
            if (string.IsNullOrWhiteSpace(maxLengthConfig))
            {
                maxLength = 5120000;
            }
            else
            {
                if (!int.TryParse(maxLengthConfig, out maxLength))
                {
                    maxLength = 5120000;
                }
            }

            return maxLength;
        }

        private string[] GetBlackListExtensions()
        {
            string[] blackList = { ".exe", ".cshtml", ".vbhtml", ".aspx", ".ascx", ".msi", ".bin", ".js", ".bat", ".cmd", ".ps1", ".reg", ".rgs", ".ws", ".wsf" };
            var blacklistConfig = ConfigurationManager.AppSettings["BlackListExtentions"];

            if (!string.IsNullOrWhiteSpace(blacklistConfig))
            {
                blacklistConfig = blacklistConfig.Replace(" ", "").ToLower();
                var split = blacklistConfig.Split(',', ';');
                if (split.Length != 0)
                {
                    blackList = split;
                }
            }

            return blackList;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userState = new UserState();
            if (filterContext.HttpContext.User is ClaimsPrincipal)
            {
                var user = filterContext.HttpContext.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                var userStateString = GetClaim(claims, "userState");

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            using (var unitOfWork = new UnitOfWork())
            {
                // Get all notification
                int totalNotification;
                var notifications = unitOfWork.NotifyRealTimeRepo.GetNotifySystemByToUserId(string.Empty, userState.UserId, false, null, 1, 20, out totalNotification);

                filterContext.Controller.ViewBag.Notifications = JsonConvert.SerializeObject(new { items = notifications, totalNotification });

                int totalMessage;
                var listMessageInbox = unitOfWork.MessageRealTimeRepo.SearchInbox(userState.UserId, string.Empty, null, null, 1, 20, out totalMessage);

                filterContext.Controller.ViewBag.ListMessage = JsonConvert.SerializeObject(new { items = listMessageInbox, totalMessage });

                var totalUnread = unitOfWork.MessageRealTimeRepo.GetTotalUnread(userState.UserId);
                filterContext.Controller.ViewBag.TotalInboxUnread = totalUnread.TotalInboxUnread;
                filterContext.Controller.ViewBag.TotalStarUnread = totalUnread.TotalStarUnread;
                filterContext.Controller.ViewBag.TotalDraft = totalUnread.TotalDraft;
                filterContext.Controller.ViewBag.MessageDetail = "null";
                filterContext.Controller.ViewBag.MaxFileLength = GetMaxFileLength();
                filterContext.Controller.ViewBag.BlackListExtensions = JsonConvert.SerializeObject(GetBlackListExtensions());

                // Log action
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var timeNow = DateTime.Now;
                    unitOfWork.TrackerRepo.Add(new Tracker()
                    {
                        Browser = filterContext.HttpContext.Request.Browser.Browser,
                        UrlReferrer = filterContext.HttpContext.Request.UrlReferrer == null ? "" : filterContext.HttpContext.Request.UrlReferrer.OriginalString,
                        Version = filterContext.HttpContext.Request.Browser.MajorVersion.ToString(),
                        Os = filterContext.HttpContext.Request.Browser.Platform,
                        SessionId = userState.SessionId,
                        PageUrl = filterContext.HttpContext.Request.CurrentExecutionFilePath,
                        Ip = MyCommon.ClientIp(),
                        IsMobileDevice = filterContext.HttpContext.Request.Browser.IsMobileDevice,
                        MobileDeviceManufacturer = filterContext.HttpContext.Request.Browser.MobileDeviceManufacturer,
                        WebsiteId = 0,
                        Day = (byte)timeNow.Day,
                        Month = (byte)timeNow.Month,
                        Quater = (byte)MyCommon.GetQuater(timeNow),
                        Year = (short)timeNow.Year,
                    });

                    unitOfWork.TrackerRepo.Save();
                }

                // Lấy ra các Position kiêm nhiệm của nhân viên
                filterContext.Controller.ViewBag.TitleConcurrents =
                    unitOfWork.UserPositionRepo.Find(x => x.UserId == userState.UserId);

                // Admin có thể vào mọi trang
                IEnumerable<PermissionActionResult> navPages;
                List<string> moduleIds;
                List<PermissionActionResult> permissionActions;
                if (userState.UserName == "admin")
                {
                    // Lấy ra tất cả các quyền
                    permissionActions = unitOfWork.PermissionActionRepo.GetAllPermission();

                    // 
                    navPages = permissionActions.Where(x=> x.RoleActionId == (byte)EnumAction.View && x.IsShowMenu).ToList();
                    moduleIds = navPages.Select(x => x.ModuleIdPath).Distinct().ToList();

                    filterContext.Controller.ViewBag.NavPages = navPages;
                    filterContext.Controller.ViewBag.NavModules = unitOfWork.ModuleRepo.Find(x => moduleIds.Any(mId => mId == x.IdPath) || moduleIds.Any(mId => mId.StartsWith(x.IdPath + ".")));
                    filterContext.Controller.ViewData["PermissionActions"] = permissionActions;
                    filterContext.Controller.ViewBag.NavActivePages = navPages.Where(x => Pages.Any(p => x.PageId == (int)p)).ToList();

                    base.OnActionExecuting(filterContext);
                    return;
                }

                permissionActions = unitOfWork.PermissionActionRepo.GetPermissionByUserId1(userState.UserId, userState.TitleId ?? 0, userState.OfficeId ?? 0);

                // Thêm quyền mặc định vào danh sách quyền
                permissionActions.AddRange(unitOfWork.PermissionActionRepo.GetPermissionDefault());

                var isPermission = Pages.Any(page => permissionActions.Any(x => x.Checked && x.PageId == (int)page && x.RoleActionId == (byte)Action));

                var strPage = Pages.Select(x => x.GetAttributeOfType<DescriptionAttribute>().Description);
                var permission = Action.GetAttributeOfType<DescriptionAttribute>().Description;
                var pages = string.Join(", ", strPage);

                navPages = permissionActions.Where(x => x.RoleActionId == (byte)EnumAction.View && x.IsShowMenu).ToList();
                moduleIds = navPages.Select(x => x.ModuleIdPath).Distinct().ToList();

                filterContext.Controller.ViewBag.NavPages = navPages;
                filterContext.Controller.ViewBag.NavModules = unitOfWork.ModuleRepo.Find(x=> moduleIds.Any(mId=> mId == x.IdPath) || moduleIds.Any(mId => mId.StartsWith(x.IdPath + ".")));
                filterContext.Controller.ViewData["PermissionActions"] = permissionActions;
                filterContext.Controller.ViewBag.NavActivePages = navPages.Where(x=> Pages.Any(p=> x.PageId == (int)p)).ToList();

                // Bỏ qua bắt quyền truy cập của các trang này
                if (Pages.Any(p=> p == EnumPage.NoPermisison || p == EnumPage.Home))
                {
                    base.OnActionExecuting(filterContext);
                    return;
                }

                if (!isPermission)
                {
                    // Trả về Json khi là ajax request
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var text = $"system yêu cầu quyền <strong>{permission}</strong> một trong các trang <strong>{pages}</strong>";

                        filterContext.Result =
                            new JsonCamelCaseResult(
                                new
                                {
                                    Status = -123,
                                    Text = text
                                },
                                JsonRequestBehavior.AllowGet);
                    }
                    else // Trả về content thông thường
                    {
                        // todo: có thể chuyển hướng đến một trang và có thông báo cụ thể
                        filterContext.Result = new RedirectResult($"/error/no-permission?permission={permission}&pages={pages}");

                        //filterContext.Result = new ContentResult()
                        //{
                        //    Content = text
                        //};
                    }

                    base.OnActionExecuting(filterContext);
                    return;
                }
            }
            base.OnActionExecuting(filterContext);
        }

        public string GetClaim(List<Claim> claims, string key)
        {

            var claim = claims.FirstOrDefault(c => c.Type == key);

            return claim?.Value;
        }
    }
}