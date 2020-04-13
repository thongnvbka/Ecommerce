using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using Library.Models;
using AutoMapper;
using System;
using System.Runtime.ExceptionServices;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        // GET: Category
        [LogTracker(EnumAction.View, EnumPage.Category)]
        public async Task<ActionResult> Index(ModelView<Category, Library.ViewModels.CategoryViewModel> model)
        {
            long totalRecord;
            if (model.SearchInfo == null)
            {
                model.SearchInfo = new Library.ViewModels.CategoryViewModel();
            }

            model.Items = await UnitOfWork.CategoryRepo.FindAsync(
                out totalRecord,
                x => !x.IsDelete && (model.SearchInfo.Keyword == null || (x.Name.Contains(model.SearchInfo.Keyword) || x.Description.Contains(model.SearchInfo.Keyword))) && (model.SearchInfo.Status == -1 || x.Status == model.SearchInfo.Status),
                x => x.OrderBy(y => y.IdPath),
                model.PageInfo.CurrentPage,
                model.PageInfo.PageSize
            );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Index", "Category");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.CategoryTree = GetCategoryParent();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryMeta model)
        {
            ViewBag.CategoryTree = GetCategoryParent();
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
                return View();

            // Tên chuyên mục đã tồn tại
            if (await UnitOfWork.CategoryRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete && x.Status < 2))
            {
                ModelState.AddModelError("Name", $"Category name \"{model.Name }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var category = Mapper.Map<Category>(model);

                    if (category.ParentId == 0)
                    {
                        category.IdPath = "0";
                        category.NamePath = category.ParentName;
                    }
                    else
                    {
                        category.IdPath = category.ParentId.ToString();
                        category.NamePath = category.ParentName + "/" + category.Name;
                    }

                    category.Created = DateTime.Now;
                    category.LastUpdated = DateTime.Now;
                    category.IsDelete = false;

                    UnitOfWork.CategoryRepo.Add(category);
                    var rs = await UnitOfWork.CategoryRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }

                    //Cập nhật lại IdPath
                    if (category.ParentId == 0)
                        category.IdPath = category.Id.ToString();
                    else
                        category.IdPath = category.IdPath + "." + category.Id;

                    await UnitOfWork.CategoryRepo.SaveAsync();

                    TempData["Msg"] = $"Add to category name \"<b>{category.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            ViewBag.CategoryTree = GetCategoryParent();
            return RedirectToAction("Create");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var category = await UnitOfWork.CategoryRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (category == null)
                return HttpNotFound($"There is no category with ID is {id}");

            ViewBag.CategoryTree = GetCategoryParent();

            return View(Mapper.Map<CategoryMeta>(category));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryMeta model)
        {
            ViewBag.CategoryTree = GetCategoryParent();

            if (!ModelState.IsValid)
                return View(model);

            var category = await UnitOfWork.CategoryRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (category == null)
            {
                ModelState.AddModelError("NotExist", "The category does not exist or has been deleted");
                return View(model);
            }

            Category categoryParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != category.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                if (model.ParentId.HasValue)
                {
                    categoryParent =
                        await UnitOfWork.CategoryRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId.Value && !x.IsDelete);
                    if (categoryParent == null)
                    {
                        ModelState.AddModelError("ParentId",
                            $"Parent category \"{model.ParentName}\" does not exist or has been deleted");
                        return View(model);
                    }
                    model.ParentName = categoryParent.Name;
                }
            }

            // Tên chuyên mục đã tồn tại
            if (
                await UnitOfWork.CategoryRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId.Value && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Category name \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = category.ParentId;
                    var oldIdPath = category.IdPath;

                    category = Mapper.Map(model, category);

                    category.IsDelete = false;
                    category.LastUpdated = DateTime.Now;

                    //UnitOfWork.OfficeRepo.Add(office);

                    var rs = await UnitOfWork.OfficeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (categoryParent == null)
                        {
                            category.IdPath = category.Id.ToString();
                            category.NamePath = category.Name;
                        }
                        else
                        {
                            category.IdPath = $"{categoryParent.IdPath}.{categoryParent.Id}";
                            category.NamePath = $"{categoryParent.NamePath} / {categoryParent.Name}";
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubCategory = await UnitOfWork.CategoryRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        listSubCategory.ForEach(o =>
                        {
                            o.IdPath = $"{category.IdPath}{o.IdPath.Substring(oldIdPath.Length - 1, o.IdPath.Length)}";
                        });

                    }

                    await UnitOfWork.OfficeRepo.SaveAsync();

                    TempData["Msg"] = $"Category updated successfully \"<b>{category.Name}</b>\"";

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

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await UnitOfWork.CategoryRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (category == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            category.IsDelete = true;
            UnitOfWork.CategoryRepo.Update(category);

            var rs = await UnitOfWork.CategoryRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public string GetCategoryParent()
        {
            string str = "";

            var list = UnitOfWork.DbContext.Categories.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
            foreach (var item in list)
            {
                if (item.ParentId == 0)
                {
                    str += "{ id: " + item.Id + ", text: '" + item.Name + "', parent: '#'},";
                }
                else
                {
                    str += "{ id: " + item.Id + ", text: '" + item.Name + "', parent: '" + item.ParentId + "'},";
                }
            }
            return "[" + str + "]";
        }
    }
}