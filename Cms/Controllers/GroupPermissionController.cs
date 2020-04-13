using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Library.DbContext.Results;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    [RoutePrefix("group-permission")]
    public class GroupPermissionController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.GroupPermission)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("search")]
        //[CheckPermission(EnumAction.View, EnumPage.Position)]
        public async Task<ActionResult> Search(string keyword = "", int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            long totalRecord;
            var groupPermissions = await UnitOfWork.GroupPermissionRepo.FindAsync(out totalRecord,
                x => !x.IsDelete && x.Name.Contains(keyword), o => o.OrderByDescending(x => new { x.IsSystem, x.Id }), currentPage,
                recordPerPage);

            // Insert quyền mặc định của system
            if (!await UnitOfWork.GroupPermissionRepo.AnyAsync(x=> x.IsDefault && x.IsSystem))
            {
                var dateTime = DateTime.Now;

                // Add quyền mặc định trong system
                UnitOfWork.GroupPermissionRepo.Add(new GroupPermision()
                {
                    AppNo = 0,
                    Created = dateTime,
                    Updated = dateTime,
                    Description = "Default permission applied for all accounts",
                    IsDefault = true,
                    IsSystem = true,
                    IsDelete = false,
                    ModuleNo = 0,
                    Name = "Default permission",
                    PageNo = 0,
                    UserNo = 0,
                    UnsignedName = "Default permission"
                });

                await UnitOfWork.GroupPermissionRepo.SaveAsync();

                groupPermissions = await UnitOfWork.GroupPermissionRepo.FindAsync(out totalRecord,
                x => !x.IsDelete && x.Name.Contains(keyword), o => o.OrderByDescending(x => x.IsSystem), currentPage,
                recordPerPage);

                return JsonCamelCaseResult(new { totalRecord, items = groupPermissions.Select(Mapper.Map<GroupPermissionResult>) }, JsonRequestBehavior.AllowGet);
            }

            return JsonCamelCaseResult(new { totalRecord, items = groupPermissions.Select(Mapper.Map<GroupPermissionResult>) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("add")]
        //[CheckPermission(EnumAction.Add, EnumPage.Position)]
        public async Task<ActionResult> Add(GroupPermissionMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            var permission = Mapper.Map<GroupPermision>(model);

            permission.UnsignedName = MyCommon.Ucs2Convert(model.Name);
            permission.Created = DateTime.Now;
            permission.Updated = DateTime.Now;

            UnitOfWork.GroupPermissionRepo.Add(permission);

            var rs = await UnitOfWork.GroupPermissionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = rs, Content = "Group of permissions successfully added" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("update")]
        //[CheckPermission(EnumAction.Update, EnumPage.Position)]
        public async Task<ActionResult> Update(GroupPermissionMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            var permission =
                await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.Id);

            if (permission == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "Group of permissions does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            permission.UnsignedName = MyCommon.Ucs2Convert(model.Name);
            permission.Updated = DateTime.Now;
            permission.Name = model.Name;
            permission.Description = model.Description;

            var rs = await UnitOfWork.GroupPermissionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = rs, Content = "Group of permissions updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("delete")]
        //[CheckPermission(EnumAction.Delete, EnumPage.Position)]
        public async Task<ActionResult> Delete(int id)
        {
            var permission = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (permission == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "Group of permissions not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            // Begin Transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    permission.IsDelete = true;
                    permission.Updated = DateTime.Now;

                    var rs = await UnitOfWork.GroupPermissionRepo.SaveAsync();

                    if (rs <= 0) return JsonCamelCaseResult(new { Status = rs, Content = "Group of permissions deleted successfully" }, JsonRequestBehavior.AllowGet);

                    // Xóa quyền truy cập của nhóm quyền
                    var permissionActions = await
                        UnitOfWork.PermissionActionRepo.FindAsync(x => !x.IsDelete && x.GroupPermisionId == id);

                    var timeNow = DateTime.Now;
                    permissionActions.ForEach(pa =>
                    {
                        pa.IsDelete = true;
                        pa.Updated = timeNow;
                    });

                    await UnitOfWork.PermissionActionRepo.SaveAsync();

                    transaction.Commit();
                    return JsonCamelCaseResult(new { Status = rs, Content = "Group of permissions deleted successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("add-permission")]
        public async Task<ActionResult> AddPermission(PermissionMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            var groupPermision = await UnitOfWork.GroupPermissionRepo.SingleOrDefaultAsync(
                x => !x.IsDelete && x.Id == model.GroupPermissionId);

            if (groupPermision == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "Groups of permissions does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            var page = await UnitOfWork.PageRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.PageId);

            if (page == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "The page does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            foreach (var action in model.Actions)
            {
                var actions = await
                    UnitOfWork.PermissionActionRepo.SingleOrDefaultAsync(
                        x => x.GroupPermisionId == groupPermision.Id && x.PageId == page.Id && !x.IsDelete && x.RoleActionId == action.Id);

                if (actions == null)
                {
                    var permissionAction = new PermissionAction()
                    {
                        AppId = page.AppId,
                        AppName = page.AppName,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        GroupPermisionId = groupPermision.Id,
                        GroupPermisionName = groupPermision.Name,
                        ModuleId = page.ModuleId,
                        ModuleName = page.ModuleName,
                        Checked = action.Checked,
                        RoleActionId = action.Id,
                        RoleName = action.Name,
                        IsDelete = false,
                        PageId = page.Id,
                        PageName = page.Name
                    };

                    UnitOfWork.PermissionActionRepo.Add(permissionAction);
                }
                else
                {
                    actions.AppId = page.AppId;
                    actions.AppName = page.AppName;
                    actions.Updated = DateTime.Now;
                    actions.GroupPermisionName = groupPermision.Name;
                    actions.ModuleId = page.ModuleId;
                    actions.ModuleName = page.ModuleName;
                    actions.Checked = action.Checked;
                    actions.RoleActionId = action.Id;
                    actions.RoleName = action.Name;
                    actions.PageName = page.Name;
                }
            }

            var rs = await UnitOfWork.PermissionActionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = rs, Content = "Permission added successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("delete-permission")]
        public async Task<ActionResult> DeletePermission(int permissionId, int pageId)
        {
            var permissionActions = await UnitOfWork.PermissionActionRepo.FindAsync(
                x => !x.IsDelete && x.GroupPermisionId == permissionId && x.PageId == pageId);

            if (!permissionActions.Any())
                return JsonCamelCaseResult(new { Status = -1, Content = "This page does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            permissionActions.ForEach(x =>
            {
                x.IsDelete = true;
                x.Updated = DateTime.Now;
            });

            var rs = await UnitOfWork.PermissionActionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = rs, Content = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("update-permission")]
        public async Task<ActionResult> UpdatePermission(UpdatePermissionMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            var permissionActions = await UnitOfWork.PermissionActionRepo.SingleOrDefaultAsync(
                x => !x.IsDelete && x.GroupPermisionId == model.PermissionId && x.PageId == model.PageId && x.RoleActionId == model.ActionId);

            if (permissionActions == null)
                return JsonCamelCaseResult(new { Status = -1, Content = "This page does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            permissionActions.Checked = model.Checked;
            permissionActions.Updated = DateTime.Now;

            var rs = await UnitOfWork.PermissionActionRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = rs, Content = "Updated successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}
