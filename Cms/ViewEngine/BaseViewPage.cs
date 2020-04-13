using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Common.Emums;
using Library.Models;
using Library.DbContext.Results;
using Cms.Controllers;
using Library.DbContext.Entities;

namespace Cms.ViewEngine
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual UserState UserState => (UserState)ViewData["UserState"] ?? new UserState();
        public virtual List<UserPosition> TitleConcurrents => (List<UserPosition>)ViewData["TitleConcurrents"] ?? new List<UserPosition>();
        public virtual List<PermissionActionResult> PermissionActions => (List<PermissionActionResult>)ViewData["PermissionActions"] ?? new List<PermissionActionResult>();

        public bool CheckPermision(EnumPage page, params EnumAction[] actions)
        {
            if (UserState != null && UserState.UserName == "admin")
                return true;

            return PermissionActions.Any(
                    x => x.Checked && x.PageId == (int)page && actions.Any(action => x.RoleActionId == (byte)action));
        }

        public bool CheckOfficeType(int officeId, OfficeType type)
        {
            OfficeController officeController = new OfficeController();
            return officeController.CheckOfficeType(officeId,(byte)type);
        }
    }

    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual UserState UserState => (UserState)ViewData["UserState"] ?? new UserState();
        public virtual List<UserPosition> TitleConcurrents => (List<UserPosition>)ViewData["TitleConcurrents"] ?? new List<UserPosition>();
        public virtual List<PermissionActionResult> PermissionActions => (List<PermissionActionResult>)ViewData["PermissionActions"] ?? new List<PermissionActionResult>();
        public bool CheckPermision(EnumPage page, params EnumAction[] actions)
        {
            if (UserState != null && UserState.UserName == "admin")
                return true;

            return PermissionActions.Any(
                    x => x.Checked && x.PageId == (int)page && actions.Any(action => x.RoleActionId == (byte)action));
        }
        public bool CheckPermision(EnumAction action, params EnumPage[] pages)
        {
            if (UserState != null && UserState.UserName == "admin")
                return true;

            return PermissionActions.Any(
                    x => x.Checked && pages.Any(page => x.PageId == (int)page) && x.RoleActionId == (byte)action);
        }

        public bool CheckOfficeType(int officeId, OfficeType type)
        {
            OfficeController officeController = new OfficeController();
            return officeController.CheckOfficeType(officeId, (byte)type);
        }
    }
}