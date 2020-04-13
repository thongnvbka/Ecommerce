using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Common.ActionResult;
using Common.Emums;
using Common.Items;
using Library.DbContext.Entities;
using Library.Models;
using Library.ViewModels;

namespace Cms.Controllers
{
    [Authorize]
    public class PositionController : BaseController
    {
        // GET: Position
        [LogTracker(EnumAction.View, EnumPage.Position)]
        public async Task<ActionResult> Index(ModelView<Position, PositionFilterViewModel> model)
        {
            if (model.SearchInfo == null)
                model.SearchInfo = new PositionFilterViewModel() { HasChilds = false, OfficeId = 1, OfficeIdPath = "1"};

            // Lấy cả Position của đơn vị cấp dưới
            if (model.SearchInfo.HasChilds)
            {
                model.Items = await UnitOfWork.PositionRepo.GetPositionByOfficeIdPath(model.SearchInfo.OfficeIdPath);
            }
            else
            {
                model.Items = await UnitOfWork.PositionRepo.FindAsync(x=> x.OfficeId == model.SearchInfo.OfficeId);
            }

            model.PageInfo.TotalRecord = model.Items.Count();
            model.PageInfo.Name = "Position";
            model.PageInfo.Url = Url.Action("Index", "Position");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            ViewBag.OfficeJsTree = await GetOfficeJsTree(1);

            return View(model);
        }

        [CheckPermission(EnumAction.View, EnumPage.Position)]
        public async Task<ActionResult> PossitionByOffice(int officeId)
        {
            var result = await UnitOfWork.PositionRepo.FindAsync(x => x.OfficeId == officeId);

            return JsonCamelCaseResult(result.Select(x => new {Id = x.TitleId, Name = x.TitleName}),
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Position)]
        public async Task<ActionResult> Add(int officeId, int titleId)
        {
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsNoTrackingAsync(
                        x => x.Id == officeId && !x.IsDelete && x.Status < 2);

            if (office == null)
                return JsonCamelCaseResult(new {Status = -1, Text = "Unit does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);

            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsNoTrackingAsync(
                        x => x.Id == titleId && !x.IsDelete && x.Status < 2);

            if (title == null)
                return JsonCamelCaseResult(new {Status = -2, Text = "Position does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);

            if (await UnitOfWork.PositionRepo.AnyAsync(x => x.OfficeId == office.Id && x.TitleId == title.Id))
                return JsonCamelCaseResult(new
                        {
                            Status = -3,
                            Text = $"Vị trí \"<b>{title.Name}</b>\" Already exists in the unit \"<b>{office.Name}</b>\""
                        }, JsonRequestBehavior.AllowGet);

            UnitOfWork.PositionRepo.Add(new Position()
            {
                OfficeId = office.Id,
                OfficeName = office.Name,
                TitleId = title.Id,
                TitleName = title.Name,
                Created = DateTime.Now
            });

            var rs = await UnitOfWork.PositionRepo.SaveAsync();

            return JsonCamelCaseResult(new {Status = rs, Text = $"Add position\"<b>{title.Name}</b>\" successul"},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Position)]
        public async Task<ActionResult> Delete(int officeId, int titleId)
        {
            var position = await UnitOfWork.PositionRepo.SingleOrDefaultAsync(x => x.OfficeId == officeId && x.TitleId == titleId);

            // Vị trí does not exist or has been deleted
            if (position == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "this position does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            UnitOfWork.PositionRepo.Remove(position);

            var rs = await UnitOfWork.PositionRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
    }
}