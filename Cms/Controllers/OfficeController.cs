using System;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Common.Items;
using Library.DbContext.Entities;
using Library.Models;
using Library.ViewModels;

namespace Cms.Controllers
{
    [Authorize]
    public class OfficeController : BaseController
    {
        // GET: Office
        [LogTracker(EnumAction.View, EnumPage.Office)]
        public async Task<ActionResult> Index(ModelView<Office, OfficeFilterViewModel> model)
        {
            if (model.SearchInfo == null)
                model.SearchInfo = new OfficeFilterViewModel();

            long totalRecord;

            model.SearchInfo.Keyword = MyCommon.Ucs2Convert(model.SearchInfo.Keyword);

            model.Items = await UnitOfWork.OfficeRepo.FindAsync(out totalRecord,
                x =>
                    !x.IsDelete && (!model.SearchInfo.Status.HasValue || x.Status == model.SearchInfo.Status.Value) &&
                    (!model.SearchInfo.Type.HasValue || x.Type == model.SearchInfo.Type.Value) &&
                    (model.SearchInfo.Keyword == "" || x.UnsignedName.Contains(model.SearchInfo.Keyword)),
                x => x.OrderBy(y => y.IdPath),
                model.PageInfo.CurrentPage, model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Unit";
            model.PageInfo.Url = Url.Action("Index", "Office");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            return View(model);
        }

        // GET: Office/Details/5
        [LogTracker(EnumAction.View, EnumPage.Office)]
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Office/Create
        [LogTracker(EnumAction.Add, EnumPage.Office)]
        public async Task<ActionResult> Create()
        {
            ViewBag.officeJsTree = await GetOfficeJsTree();

            return View();
        }

        // POST: Office/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.Office)]
        public async Task<ActionResult> Create(OfficeMeta model)
        {
            ModelState.Remove("Id");

            ViewBag.officeJsTree = await GetOfficeJsTree();

            if (!ModelState.IsValid)
                return View();

            Office officeParent = null;

            // Kiểm tra đơn vị Cha có tồn tại hay không
            if (model.ParentId.HasValue)
            {
                officeParent =
                    await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId.Value && !x.IsDelete);
                if (officeParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Parent office\"{model.ParentName}\" does not exist or has been deleted");
                    return View();
                }
                model.ParentName = officeParent.Name;
            }

            // Chỉ có duy nhất một đơn vị có ParentId là Null
            if (!model.ParentId.HasValue && await UnitOfWork.OfficeRepo.AnyAsync(x => !x.IsDelete))
            {
                ModelState.AddModelError("ParentId", "Selecting the office is compulsory");
                return View();
            }

            // Mã đơn vị đã tồn tại
            if (await UnitOfWork.OfficeRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete))
            {
                ModelState.AddModelError("Code", $"Office ID \"{model.Code}\" already exists");
                return View();
            }

            // Tên đơn vị đã tồn tại
            if (
                await
                    UnitOfWork.OfficeRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId.Value))
            {
                ModelState.AddModelError("Name", $"Office name \"{model.Name}\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var office = Mapper.Map<Office>(model);

                    office.IsDelete = false;
                    office.UnsignedName = MyCommon.Ucs2Convert($"{office.Name} {office.Code} {office.ShortName}");
                    office.Created = DateTime.Now;
                    office.Updated = DateTime.Now;
                    office.LastUpdateUserId = UserState.UserId;
                    office.ChildNo = 0;
                    office.TitleNo = 0;
                    office.UserNo = 0;

                    UnitOfWork.OfficeRepo.Add(office);

                    var rs = await UnitOfWork.OfficeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị

                    if (officeParent == null)
                    {
                        office.IdPath = office.Id.ToString();
                        office.NamePath = office.Name;
                    }
                    else
                    {
                        office.IdPath = $"{officeParent.IdPath}.{office.Id}";
                        office.NamePath = $"{officeParent.NamePath} / {office.Name}";

                        // Cập nhật lại số lượng đơn vị chả
                        var childNo = await UnitOfWork.OfficeRepo.CountAsync(x => x.ParentId == officeParent.Id && !x.IsDelete);
                        officeParent.ChildNo = childNo;
                    }

                    await UnitOfWork.OfficeRepo.SaveAsync();

                    TempData["Msg"] = $"Office added successfully \"<b>{office.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Create");
        }

        // GET: Office/Edit/5
        [LogTracker(EnumAction.Update, EnumPage.Office)]
        public async Task<ActionResult> Edit(int id)
        {
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (office == null)
                return HttpNotFound($"There isn't such a position with id as {id}");

            ViewBag.officeJsTree = await GetOfficeJsTree();

            return View(Mapper.Map<OfficeMeta>(office));
        }

        // POST: Office/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.Office)]
        public async Task<ActionResult> Edit(OfficeMeta model)
        {
            ViewBag.officeJsTree = await GetOfficeJsTree();

            if (!ModelState.IsValid)
                return View(model);

            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (office == null)
            {
                ModelState.AddModelError("NotExist", "Office does not exist or has been deleted");
                return View(model);
            }

            Office officeParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != office.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                if (model.ParentId.HasValue)
                {
                    officeParent =
                        await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId.Value && !x.IsDelete);
                    if (officeParent == null)
                    {
                        ModelState.AddModelError("ParentId",
                            $"Parent office\"{model.ParentName}\" does not exist or has been deleted");
                        return View(model);
                    }
                    model.ParentName = officeParent.Name;
                }

                // Chỉ có duy nhất một đơn vị có ParentId là Null
                if (!model.ParentId.HasValue && await UnitOfWork.OfficeRepo.AnyAsync(x => !x.IsDelete))
                {
                    ModelState.AddModelError("ParentId", "Selecting the office is compulsory");
                    return View(model);
                }
            }


            // Mã đơn vị đã tồn tại
            if (await UnitOfWork.OfficeRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("Code", $"Office ID \"{model.Code}\" already exists");
                return View(model);
            }

            // Tên đơn vị đã tồn tại
            if (
                await UnitOfWork.OfficeRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId.Value && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Office name \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = office.ParentId;
                    var oldIdPath = office.IdPath;

                    office = Mapper.Map(model, office);

                    office.IsDelete = false;
                    office.UnsignedName = MyCommon.Ucs2Convert($"{office.Name} {office.Code} {office.ShortName}");
                    office.Updated = DateTime.Now;
                    office.LastUpdateUserId = UserState.UserId;

                    //UnitOfWork.OfficeRepo.Add(office);

                    var rs = await UnitOfWork.OfficeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (officeParent == null)
                        {
                            office.IdPath = office.Id.ToString();
                            office.NamePath = office.Name;
                        }
                        else
                        {
                            office.IdPath = $"{officeParent.IdPath}.{office.Id}";
                            office.NamePath = $"{officeParent.NamePath} / {office.Name}";

                            // Cập nhật lại số lượng đơn vị cha cũ
                            var childNo = await UnitOfWork.OfficeRepo.CountAsync(x => x.ParentId == oldParentId && !x.IsDelete);
                            var oldOfficeParent = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == oldParentId);
                            oldOfficeParent.ChildNo = childNo;

                            // Cập nhật lại số lượng đơn vị cha mới
                            var childNo1 = await UnitOfWork.OfficeRepo.CountAsync(x => x.ParentId == officeParent.Id && !x.IsDelete);
                            officeParent.ChildNo = childNo1;
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubOffice = await UnitOfWork.OfficeRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        listSubOffice.ForEach(o =>
                        {
                            o.IdPath = $"{office.IdPath}{o.IdPath.Substring(oldIdPath.Length - 1, o.IdPath.Length)}";
                        });

                    }

                    await UnitOfWork.OfficeRepo.SaveAsync();

                    TempData["Msg"] = $"Office updated successfully \"<b>{office.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Office/Delete/5
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Office)]
        public async Task<ActionResult> Delete(int id)
        {
            var office = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            // Office does not exist or has been deleted
            if (office == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            // Không được xóa đơn vị đã có đơn vị con
            if (office.ChildNo > 0)
                return JsonCamelCaseResult(-2, JsonRequestBehavior.AllowGet);

            // Cập nhật trạng thái IsDelete của đơn vị
            office.IsDelete = true;

            var rs = await UnitOfWork.OfficeRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// kiểm tra nhân viên thuộc đơn vị nào
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CheckOfficeType(int officeId , byte type)
        {
            return UnitOfWork.OfficeRepo.CheckOfficeType(officeId, type);
        }
    }
}
