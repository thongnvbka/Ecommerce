using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Common.ActionResult;
using Common.Emums;
using Common.Helper;
using Library.Models;
using Library.UnitOfWork;

namespace Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CheckPermissionAttribute : ActionFilterAttribute
    {
        private EnumPage[] Pages { get; set; }
        private EnumAction Action { get; set; }

        /// <summary>
        /// Check quyền thao tác của user đang đăng nhập
        /// </summary>
        /// <param name="action"></param>
        /// <param name="pages">Dictionary với key là ActionId value là mảng các trang</param>
        public CheckPermissionAttribute(EnumAction action, params EnumPage[] pages)
        {
            Pages = pages;
            Action = action;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserState userState = new UserState();
            if (filterContext.HttpContext.User is ClaimsPrincipal)
            {
                var user = filterContext.HttpContext.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                var userStateString = GetClaim(claims, "userState");

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            // Admin có thể vào mọi trang
            if (userState.UserName == "admin")
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            using (var unitOfWork = new UnitOfWork())
            {
                var permissionActions = unitOfWork.PermissionActionRepo.GetPermissionByUserId1(userState.UserId, userState.TitleId ?? 0, userState.OfficeId ?? 0);

                // Thêm quyền mặc định vào danh sách quyền
                permissionActions.AddRange(unitOfWork.PermissionActionRepo.GetPermissionDefault());

                var isPermission = Pages.Any(page => permissionActions.Any(x => x.Checked && x.PageId == (int)page && x.RoleActionId == (byte)Action));

                if (!isPermission)
                {
                    var strPage = Pages.Select(x => x.GetAttributeOfType<DescriptionAttribute>().Description);
                    var permission = Action.GetAttributeOfType<DescriptionAttribute>().Description;
                    var pages = string.Join(", ", strPage);
                    
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