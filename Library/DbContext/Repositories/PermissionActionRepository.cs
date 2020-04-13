using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;
using System.Linq.Expressions;
using System;

namespace Library.DbContext.Repositories
{
    public class PermissionActionRepository : Repository<PermissionAction>
    {
        public PermissionActionRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<PermissionAction1Result>> GetForUpdateRoleAction(int pageId, byte roleActionId)
        {
            return Db.PermissionActions.Where(x => x.GroupPermisionId.HasValue && !x.IsDelete)
                .Select(x => new PermissionAction1Result
                {
                    AppId = x.AppId,
                    ModuleId = x.ModuleId,
                    PageId = x.PageId,
                    GroupPermisionId = x.GroupPermisionId,
                    AppName = x.AppName,
                    ModuleName = x.ModuleName,
                    PageName = x.PageName,
                    GroupPermisionName = x.GroupPermisionName
                })
                .Distinct()
                .Join(Db.PermissionActions.Where(
                    x => !x.GroupPermisionId.HasValue && x.PageId == pageId && x.RoleActionId == roleActionId && !x.IsDelete),
                    action => action.PageId, action => action.PageId, (action, permissionAction) => action)
                .ToListAsync();
        }

        public Task<List<PermissionAction1Result>> GetPermisionActionByPageIdAndRoleActionId(int pageId, byte roleActionId)
        {
            return Db.PermissionActions.Where(
                    x =>
                        x.GroupPermisionId.HasValue && !x.IsDelete && x.PageId == pageId &&
                        x.RoleActionId == roleActionId)
                    .Select(x => new PermissionAction1Result
                    {
                        AppId = x.AppId,
                        ModuleId = x.ModuleId,
                        PageId = x.PageId,
                        GroupPermisionId = x.GroupPermisionId,
                        AppName = x.AppName,
                        ModuleName = x.ModuleName,
                        PageName = x.PageName,
                        GroupPermisionName = x.GroupPermisionName
                    }).Distinct().ToListAsync();
        }

        //public Task<List<PermissionActionResult>> GetPermissionByUserId(int userId, int titleId, int officeId)
        //{
        //    return Db.PermissionActions.Where(x => !x.IsDelete && x.Checked)
        //        .Join(Db.UserPositions.Where(x => x.UserId == userId && x.TitleId == titleId && x.OfficeId == officeId),
        //            action => action.GroupPermisionId, position => position.GroupPermisionId, (action, position) =>
        //                new PermissionActionResult
        //                {
        //                    AppId = action.AppId,
        //                    AppName = action.AppName,
        //                    ModuleId = action.ModuleId,
        //                    ModuleName = action.ModuleName,
        //                    PageId = action.PageId,
        //                    PageName = action.PageName,
        //                    RoleActionId = action.RoleActionId,
        //                    RoleName = action.RoleName,
        //                    Checked = action.Checked,
        //                    GroupPermisionId = action.GroupPermisionId,
        //                    GroupPermisionName = action.GroupPermisionName
        //                })
        //        .Distinct()
        //        .ToListAsync();
        //}

        public List<PermissionActionResult> GetPermissionByUserId1(int userId, int titleId, int officeId)
        {
            return Db.PermissionActions.Where(x => !x.IsDelete && x.Checked && x.GroupPermisionId != null)
                .Join(Db.Pages.Where(x=> x.IsDelete == false), pa => pa.PageId, p => p.Id, (pa, p) => new { pa, p })
                .Join(Db.Modules, arg => arg.pa.ModuleId, m => m.Id, (arg, m) => new { action = arg.pa, arg.p, m })
                .Join(Db.UserPositions.Where(x => x.UserId == userId && x.TitleId == titleId && x.OfficeId == officeId),
                    arg => arg.action.GroupPermisionId, position => position.GroupPermisionId, (arg, position) =>
                        new PermissionActionResult
                        {
                            AppId = arg.action.AppId,
                            AppName = arg.action.AppName,
                            ModuleId = arg.action.ModuleId,
                            ModuleName = arg.action.ModuleName,
                            ModuleLevel = arg.m.Level,
                            PageId = arg.action.PageId,
                            PageName = arg.action.PageName,
                            PageUrl = arg.p.Url,
                            PageIcon = arg.p.Icon,
                            PageOrderNo = arg.p.OrderNo,
                            RoleActionId = arg.action.RoleActionId,
                            RoleName = arg.action.RoleName,
                            Checked = arg.action.Checked,
                            GroupPermisionId = arg.action.GroupPermisionId,
                            GroupPermisionName = arg.action.GroupPermisionName,
                            ModuleIdPath = arg.m.IdPath,
                            ModuleNamePath = arg.m.NamePath,
                            ModuleIcon = arg.m.Icon,
                            IsShowMenu = arg.p.ShowInMenu
                        })
                .Distinct()
                .ToList();
        }

        public List<PermissionActionResult> GetPermissionDefault()
        {
            return Db.PermissionActions.Where(x => !x.IsDelete && x.Checked && x.GroupPermisionId != null)
                .Join(Db.Pages.Where(x=> x.IsDelete == false), pa => pa.PageId, p => p.Id, (pa, p) => new { pa, p})
                .Join(Db.Modules, arg => arg.pa.ModuleId, m => m.Id, (arg, m) => new { action = arg.pa, arg.p, m })
                .Join(Db.GroupPermisions.Where(x => !x.IsDelete && x.IsDefault && x.IsSystem),
                    arg => arg.action.GroupPermisionId.Value, group => group.Id, (arg, gp) =>
                        new PermissionActionResult
                        {
                            AppId = arg.action.AppId,
                            AppName = arg.action.AppName,
                            ModuleId = arg.action.ModuleId,
                            ModuleName = arg.action.ModuleName,
                            ModuleLevel = arg.m.Level,
                            PageId = arg.action.PageId,
                            PageName = arg.action.PageName,
                            PageUrl = arg.p.Url,
                            PageIcon = arg.p.Icon,
                            PageOrderNo = arg.p.OrderNo,
                            RoleActionId = arg.action.RoleActionId,
                            RoleName = arg.action.RoleName,
                            Checked = arg.action.Checked,
                            GroupPermisionId = arg.action.GroupPermisionId,
                            GroupPermisionName = arg.action.GroupPermisionName,
                            ModuleIdPath = arg.m.IdPath,
                            ModuleNamePath = arg.m.NamePath,
                            ModuleIcon = arg.m.Icon,
                            IsShowMenu = arg.p.ShowInMenu
                        })
                .Distinct()
                .ToList();
        }

        public List<PermissionActionResult> GetAllPermission()
        {
            return Db.PermissionActions.Where(x => x.IsDelete == false && x.Checked && x.GroupPermisionId == null)
                .Join(Db.Pages.Where(x => x.IsDelete == false), pa => pa.PageId, p => p.Id, (pa, p) => new {pa, p})
                .Join(Db.Modules, arg => arg.pa.ModuleId, m => m.Id, (arg, m) =>
                    new PermissionActionResult
                    {
                        AppId = arg.p.AppId,
                        AppName = arg.p.AppName,
                        ModuleId = arg.p.ModuleId,
                        ModuleName = arg.p.ModuleName,
                        ModuleLevel = m.Level,
                        PageId = arg.pa.PageId,
                        PageName = arg.pa.PageName,
                        PageUrl = arg.p.Url,
                        PageIcon = arg.p.Icon,
                        PageOrderNo = arg.p.OrderNo,
                        RoleActionId = arg.pa.RoleActionId,
                        RoleName = arg.pa.RoleName,
                        Checked = arg.pa.Checked,
                        GroupPermisionId = arg.pa.GroupPermisionId,
                        GroupPermisionName = arg.pa.GroupPermisionName,
                        ModuleIdPath = m.IdPath,
                        ModuleNamePath = m.NamePath,
                        ModuleIcon = m.Icon,
                        IsShowMenu = arg.p.ShowInMenu
                    })
                .Distinct()
                .ToList();
        }
    }
}
