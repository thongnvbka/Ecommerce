using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Library.DbContext.Results;

namespace Cms.Controllers
{
    [Authorize]
    [RoutePrefix("error")]
    public class CustomErrorController : BaseController
    {
        [Route("no-permission")]
        [LogTracker(EnumAction.View, EnumPage.NoPermisison)]
        public ActionResult NoPermission(string permission, string pages)
        {
            object model = pages.IndexOf(",", StringComparison.Ordinal) >= 0
                ? $"The system requires permission <strong>{permission}</strong> for one of the pages <strong>{pages}</strong>"
                : $"The system requires permission <strong>{permission}</strong> in page <strong>{pages}</strong>";


            var permissionActions = UnitOfWork.PermissionActionRepo.GetPermissionByUserId1(UserState.UserId, UserState.TitleId ?? 0, UserState.OfficeId ?? 0);

            // Thêm quyền mặc định vào danh sách quyền
            permissionActions.AddRange(UnitOfWork.PermissionActionRepo.GetPermissionDefault());

            var navPages = permissionActions.Where(x => x.RoleActionId == (byte)EnumAction.View).ToList();
            var moduleIds = navPages.Select(x => x.ModuleIdPath).Distinct().ToList();

            ViewBag.NavPages = navPages;
            ViewBag.NavModules = UnitOfWork.ModuleRepo.Find(x => moduleIds.Any(mId => mId == x.IdPath) || moduleIds.Any(mId => mId.StartsWith(x.IdPath + ".")));
            ViewBag.NavActivePages = new List<PermissionActionResult>();

            return View("NoPermission", model);
        }
    }
}