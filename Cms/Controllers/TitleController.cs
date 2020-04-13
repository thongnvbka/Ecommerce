using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Common.Items;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.ViewModels;

namespace Cms.Controllers
{
    [Authorize]
    public class TitleController : BaseController
    {
        // GET: Title
        [LogTracker(EnumAction.View, EnumPage.Title)]
        public async Task<ActionResult> Index(ModelView<Title, TitleFilterViewModel> model)
        {
            if (model.SearchInfo == null)
                model.SearchInfo = new TitleFilterViewModel();

            long totalRecord;

            model.SearchInfo.Keyword = MyCommon.Ucs2Convert(model.SearchInfo.Keyword);

            model.Items = await UnitOfWork.TitleRepo.FindAsync(
                            out totalRecord,
                            x => !x.IsDelete &&
                                (
                                    !model.SearchInfo.Status.HasValue ||
                                    x.Status == model.SearchInfo.Status.Value
                                ) &&
                                (
                                    model.SearchInfo.Keyword == "" ||
                                    x.UnsignedName.Contains(model.SearchInfo.Keyword)
                                ),
                                x => x.OrderByDescending(y => y.Id),
                                     model.PageInfo.CurrentPage,
                                     model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Position";
            model.PageInfo.Url = Url.Action("Index", "Title");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            return View(model);
        }

        // POST: Title/Suggetion
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.Office)]
        public async Task<ActionResult> Suggetion(int officeId, string term, int size = 6)
        {
            var titles = await UnitOfWork.TitleRepo.Suggettion(officeId, MyCommon.Ucs2Convert(term), size);

            return JsonCamelCaseResult(titles.Select(Mapper.Map<TitleSuggetionResult>), JsonRequestBehavior.AllowGet);
        }

        // GET: Title/Details/5
        [LogTracker(EnumAction.View, EnumPage.Office)]
        public async Task<ActionResult> Details(int id)
        {
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (title == null)
                return HttpNotFound($"No Position has Id {id}");

            return View(Mapper.Map<TitleMeta>(title));
        }

        // GET: Title/Create
        [LogTracker(EnumAction.Add, EnumPage.Office)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Title/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Office)]
        public async Task<ActionResult> Create(TitleMeta model)
        {
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
                return View();

            // Mã Position đã tồn tại
            if (await UnitOfWork.TitleRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete))
            {
                ModelState.AddModelError("Code", $" Position code \"{model.Code }\" already exists");
                return View();
            }

            // Tên Position đã tồn tại
            if (await UnitOfWork.TitleRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete && x.Status < 2))
            {
                ModelState.AddModelError("Name", $"Tên Position \"{model.Name }\" already exists");
                return View();
            }

            var title = Mapper.Map<Title>(model);

            title.Created = DateTime.Now;
            title.Updated = DateTime.Now;
            title.LastUpdateUserId = UserState.UserId;
            title.IsDelete = false;
            title.UnsignedName = MyCommon.Ucs2Convert($"{title.Name} {title.Code} ${title.ShortName}");

            UnitOfWork.TitleRepo.Add(title);

            var rs = await UnitOfWork.UserRepo.SaveAsync();

            if (rs <= 0)
            {
                return View();
            }

            TempData["Msg"] = $"Added to success Position \"<b>{title.Name}</b>\"";

            return RedirectToAction("Create");
        }

        // GET: Title/Edit/5
        [LogTracker(EnumAction.Update, EnumPage.Office)]
        public async Task<ActionResult> Edit(int id)
        {
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (title == null)
                return HttpNotFound($"No Position has Id {id}");

            return View(Mapper.Map<TitleMeta>(title));
        }

        // POST: Title/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Office)]
        public async Task<ActionResult> Edit(TitleMeta model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Mã Position đã tồn tại
            if (await UnitOfWork.TitleRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("Code", $"Position code \"{model.Code }\" already exists");
                return View(model);
            }

            // Tên Position đã tồn tại
            if (await UnitOfWork.TitleRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete && x.Status < 2 && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Position code \"{model.Name}\" already exists");
                return View(model);
            }

            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (title == null)
            {
                ModelState.AddModelError("NotExist", "This position has been deleted or does not exist");
                return View(model);
            }

            title = Mapper.Map(model, title);

            title.LastUpdateUserId = UserState.UserId;
            title.Updated = DateTime.Now;
            title.UnsignedName = MyCommon.Ucs2Convert($"{title.Name} {title.Code} ${title.ShortName}");

            var rs = await UnitOfWork.TitleRepo.SaveAsync();

            if (rs <= 0)
            {
                return View();
            }

            TempData["Msg"] = $"Updated successfully Position \"<b>{title.Name}</b>\"";

            return RedirectToAction("Index");
        }

        // POST: Title/Delete/5
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Office)]
        public async Task<ActionResult> Delete(int id)
        {
            var title = await UnitOfWork.TitleRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (title == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            title.IsDelete = true;
            var rs = await UnitOfWork.TitleRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
    }
}
