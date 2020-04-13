using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Common.Helper;
using Hangfire;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Jobs;
using Library.Models;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    [RoutePrefix("page")]
    public class PageController : BaseController
    {
        // GET: Page
        [LogTracker(EnumAction.View, EnumPage.Pages)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy tất cả các Page
        /// </summary>
        /// <returns></returns>
        [Route("all")]
        public async Task<ActionResult> GetAll()
        {
            var pages = await UnitOfWork.PageRepo.GetAll();

            return JsonCamelCaseResult(pages, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy Page theo AppId
        /// </summary>
        /// <returns></returns>
        [Route("app-id{appId}")]
        public async Task<ActionResult> GetByAppId(int appId)
        {
            var pages = await UnitOfWork.PageRepo.GetByAppId(appId);
            return JsonCamelCaseResult(pages, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy Page theo ModuleId
        /// </summary>
        /// <returns></returns>
        [Route("module-id{moduleId}")]
        public async Task<ActionResult> GetByModuleId(int moduleId)
        {
            var pages = await UnitOfWork.PageRepo.GetByModuleId(moduleId);

            return JsonCamelCaseResult(pages, JsonRequestBehavior.AllowGet);
        }

        [Route("page-all-tree")]
        public async Task<ActionResult> GetPageAllTree()
        {
            var apps = await UnitOfWork.AppRepo.FindAsync(x => !x.IsDelete);
            var modules = await UnitOfWork.ModuleRepo.FindAsync(x => !x.IsDelete);
            var pages = await UnitOfWork.PageRepo.FindAsync(x => !x.IsDelete);

            string pageId = $";{string.Join(";", pages.Select(x => x.Id))};";

            var actions = await UnitOfWork.PermissionActionRepo.FindAsync(
                x => !x.IsDelete && pageId.Contains(";" + x.PageId + ";") && x.GroupPermisionId == null && x.Checked);

            return JsonCamelCaseResult(new { apps, modules, pages = pages.Select(Mapper.Map<PageResult>), actions }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Search page
        /// </summary>
        /// <returns></returns>
        [Route("search")]
        public async Task<ActionResult> Search(string keyword = "", int? appId = null, int? moduleId = null, int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            long totalRecord;
            var pages = await UnitOfWork.PageRepo.FindAsync(out totalRecord, x => !x.IsDelete && x.UnsignedName.Contains(keyword)
            && (appId == null || x.AppId == appId.Value) && (moduleId == null || x.ModuleId == moduleId.Value), x=> x.OrderBy(y => new { y.ModuleId, y.OrderNo }), currentPage, recordPerPage);

            string pageId = $";{string.Join(";", pages.Select(x => x.Id))};";

            var permissionActions = await UnitOfWork.PermissionActionRepo.FindAsync(
                x => !x.IsDelete && pageId.Contains(";" + x.PageId + ";") && x.GroupPermisionId == null);

            return JsonCamelCaseResult(new { totalRecord, items = pages.Select(x => Mapper.Map<PageResult>(x)), permissionActions }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("add")]
        public async Task<ActionResult> Add(PageMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage =
                    string.Join(", ", ModelState.Values.Select(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra Id của Page đã tồn tại trong system
            if (await UnitOfWork.PageRepo.AnyAsync(x => !x.IsDelete && x.Id == model.Id))
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "This page id already exists in the system" }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra appId có tồn tại hay không
            var app = await UnitOfWork.AppRepo.SingleOrDefaultAsync(x => x.Id == model.AppId && !x.IsDelete);
            if (app == null)
            {
                return JsonCamelCaseResult(new { Status = -2, Content = "App does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra moduleId có tồn tại hay không
            var module =
                await UnitOfWork.ModuleRepo.SingleOrDefaultAsync(x => x.Id == model.ModuleId && !x.IsDelete);
            if (module == null)
            {
                return JsonCamelCaseResult(new { Status = -3, Content = "Module does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            var page = Mapper.Map<Page>(model);

            page.AppName = app.Name;
            page.ModuleName = module.Name;
            page.UnsignedName = MyCommon.Ucs2Convert($"{model.Name} {app.Name} {module.Name}").ToLower();
            page.Created = DateTime.Now;

            // Transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    UnitOfWork.PageRepo.Add(page);

                    var rs = await UnitOfWork.PageRepo.SaveAsync();

                    if (rs > 0)
                    {
                        // Thêm quyền cho Page
                        foreach (var roleAction in model.RoleActions)
                        {
                            UnitOfWork.PermissionActionRepo.Add(new PermissionAction()
                            {
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                PageId = page.Id,
                                PageName = page.Name,
                                AppId = app.Id,
                                AppName = app.Name,
                                Checked = roleAction.Checked,
                                ModuleId = module.Id,
                                ModuleName = module.Name,
                                RoleActionId = roleAction.Id,
                                RoleName = roleAction.Name
                            });
                        }

                        await UnitOfWork.PermissionActionRepo.SaveAsync();
                    }

                    // Comit transaction
                    transaction.Commit();

                    return JsonCamelCaseResult(new { Status = rs, Content = "Add page successfully" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    // Rollback transaction
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult> Update(PageMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));

                return JsonCamelCaseResult(new { Status = -100, Content = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra page tồn tại hay khong
            var page = await UnitOfWork.PageRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);
            if (page == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "Page does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra appId có tồn tại hay không
            var app = await UnitOfWork.AppRepo.SingleOrDefaultAsync(x => x.Id == model.AppId && !x.IsDelete);
            if (app == null)
            {
                return JsonCamelCaseResult(new { Status = -2, Content = "App does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra moduleId có tồn tại hay không
            var module =
                await UnitOfWork.ModuleRepo.SingleOrDefaultAsync(x => x.Id == model.ModuleId && !x.IsDelete);
            if (module == null)
            {
                return JsonCamelCaseResult(new { Status = -3, Content = "Module does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            var isUpdateRefTable = model.AppId != page.AppId || model.ModuleId != page.ModuleId || !model.Name.Equals(page.Name);

            page = Mapper.Map(model, page);

            int rs;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Cập nhật lại thông trin của trang
                    UnitOfWork.PageRepo.Update(page);

                    rs = await UnitOfWork.PageRepo.SaveAsync();

                    if (rs > 0)
                    {
                        var oldRoleActions = await
                            UnitOfWork.PermissionActionRepo.FindAsync(
                                x => !x.IsDelete && x.PageId == page.Id && x.GroupPermisionId == null);

                        // Thêm và cập nhật Action trong trang
                        foreach (var roleAction in model.RoleActions)
                        {
                            var oldRoleAction = oldRoleActions.SingleOrDefault(x => x.RoleActionId == roleAction.Id);
                            bool needUpdateGroupPermission;

                            // Thêm Action cho trang
                            if (oldRoleAction == null)
                            {
                                UnitOfWork.PermissionActionRepo.Add(new PermissionAction()
                                {
                                    Created = DateTime.Now,
                                    Updated = DateTime.Now,
                                    PageId = page.Id,
                                    PageName = page.Name,
                                    AppId = app.Id,
                                    AppName = app.Name,
                                    Checked = roleAction.Checked,
                                    ModuleId = module.Id,
                                    ModuleName = module.Name,
                                    RoleActionId = roleAction.Id,
                                    RoleName = roleAction.Name
                                });

                                needUpdateGroupPermission = true;
                            }
                            // Cập nhật action cho trang
                            else
                            {
                                // Bỏ bớt quyền trong trang
                                if (oldRoleAction.Checked && !roleAction.Checked)
                                {
                                    var permissionActions =
                                        await UnitOfWork.PermissionActionRepo
                                                .GetPermisionActionByPageIdAndRoleActionId(page.Id, roleAction.Id);

                                    if (permissionActions.Any())
                                    {
                                        transaction.Rollback();

                                        var strGroupPermissions = string.Join(", ", permissionActions.Select(x => x.GroupPermisionName));

                                        return JsonCamelCaseResult(new
                                        {
                                            Status = -4,
                                            Content = $"Check permissions in permissions group \"{strGroupPermissions}\"",
                                            permissionActions
                                        }, JsonRequestBehavior.AllowGet);
                                    }
                                }

                                needUpdateGroupPermission = !oldRoleAction.Checked && roleAction.Checked;

                                oldRoleAction.Updated = DateTime.Now;
                                oldRoleAction.PageId = page.Id;
                                oldRoleAction.PageName = page.Name;
                                oldRoleAction.AppId = app.Id;
                                oldRoleAction.AppName = app.Name;
                                oldRoleAction.ModuleId = module.Id;
                                oldRoleAction.ModuleName = module.Name;
                                oldRoleAction.RoleName = roleAction.Name;
                                oldRoleAction.Checked = roleAction.Checked;
                            }

                            // Thêm Action cho các nhóm quyền đã tồn tại
                            if (needUpdateGroupPermission)
                            {
                                // Lấy ra các trang trong nhóm quyền cần thêm Action
                                var permissionActions = await UnitOfWork.PermissionActionRepo.GetForUpdateRoleAction(page.Id,
                                    roleAction.Id);

                                foreach (var ac in permissionActions)
                                {
                                    if (await UnitOfWork.PermissionActionRepo.SingleOrDefaultAsync(
                                        x =>
                                            x.GroupPermisionId == ac.GroupPermisionId && x.PageId == ac.PageId &&
                                            x.RoleActionId == roleAction.Id && !x.IsDelete) == null)
                                    {
                                        UnitOfWork.PermissionActionRepo.Add(new PermissionAction()
                                        {
                                            Created = DateTime.Now,
                                            Updated = DateTime.Now,
                                            PageId = ac.PageId,
                                            PageName = ac.PageName,
                                            AppId = ac.AppId,
                                            AppName = ac.AppName,
                                            Checked = false,
                                            GroupPermisionId = ac.GroupPermisionId,
                                            GroupPermisionName = ac.GroupPermisionName,
                                            ModuleId = ac.ModuleId,
                                            ModuleName = ac.ModuleName,
                                            RoleActionId = roleAction.Id,
                                            RoleName = roleAction.Name
                                        });
                                    }
                                }
                            }
                        }

                        await UnitOfWork.PermissionActionRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            // Cập nhật lại thông tin của bảng liên quan
            if (isUpdateRefTable)
            {
                // Job cập nhật thông tin liên quan
                BackgroundJob.Enqueue(() => PageJob.UpdatePageName(page.Id, page.Name, page.AppId, page.AppName, page.ModuleId,
                            page.ModuleName));
            }

            return JsonCamelCaseResult(new { Status = rs, Content = "Update page successful" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult> Delete(int id)
        {
            var page = await UnitOfWork.PageRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (page == null)
            {
                return JsonCamelCaseResult(new { Status = -1, Content = "Page does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            UnitOfWork.PageRepo.Remove(id);

            // Xóa quyền của page
            UnitOfWork.PermissionActionRepo.RemoveRange(
                await UnitOfWork.PermissionActionRepo.FindAsync(
                        x => !x.IsDelete && x.PageId == page.Id));

            var rs = await UnitOfWork.PageRepo.SaveAsync();

            return JsonCamelCaseResult(rs > 0 ? new { Status = rs, Content = $"Deleted page successfully \"{page.Name}\"" } : new { Status = rs, Content = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}
