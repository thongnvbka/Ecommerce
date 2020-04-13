using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using System;
using System.Data.Entity;
using Library.ViewModels;
using Common.Helper;
using System.Linq;
using System.Runtime.ExceptionServices;
using Common.Constant;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    public class ShopController : BaseController
    {
        // GET: Shop
        [LogTracker(EnumAction.View, EnumPage.Shop)]
        public async Task<ActionResult> Index(ModelView<Shop, TitleFilterViewModel> model)
        {
            //var listCate = UnitOfWork.CategoryRepo.GetListDropdown(false);
            //listCate.Insert(0, new DropdownItem() {Text = "--- Select Branch ---", Value = "-1"});
            //ViewBag.ListCategory = listCate;
            if (model.SearchInfo == null)
                model.SearchInfo = new TitleFilterViewModel();
            model.SearchInfo.Keyword = MyCommon.Ucs2Convert(model.SearchInfo.Keyword);
            if (string.IsNullOrEmpty(model.SearchInfo.CategoryId))
                model.SearchInfo.CategoryId = "-1";
            long totalRecord;
            int tmpCategoryId = 0;
            int.TryParse(model.SearchInfo.CategoryId, out tmpCategoryId);
            model.Items = await UnitOfWork.ShopRepo.FindAsync(out totalRecord, x => (model.SearchInfo.Keyword == "" ||
                                                                                     x.Name.Contains(
                                                                                         model.SearchInfo.Keyword)) &&
                                                                                    (model.SearchInfo.CategoryId == "-1" ||
                                                                                     (model.SearchInfo.CategoryId !=
                                                                                      "-1" &&
                                                                                      x.CategoryId == tmpCategoryId)),
                x => x.OrderBy(y => y.Name),
                model.PageInfo.CurrentPage, model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int) totalRecord;
            model.PageInfo.Url = Url.Action("Index", "Shop");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }
            return View(model);
        }

        public ActionResult Add()
        {
            var listCate = UnitOfWork.CategoryRepo.GetListDropdown(false);
            listCate.Insert(0, new DropdownItem() {Text = "--- Select Branch ---", Value = "-1"});
            ViewBag.ListCategory = listCate;
            var model = new Shop() {Id = 0};
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var item = await UnitOfWork.ShopRepo.SingleOrDefaultAsync(x => x.Id == id);
            var listCate = UnitOfWork.CategoryRepo.GetListDropdown(false);
            listCate.Insert(0, new DropdownItem() {Text = "--- Select Branch ---", Value = "-1"});
            ViewBag.ListCategory = listCate;
            if (item == null)
                return HttpNotFound($"No shop with id is {id}");
            return View("Add", item);
        }

        [HttpPost]
        public async Task<ActionResult> AddItem(Shop item)
        {
            var listCate = UnitOfWork.CategoryRepo.GetListDropdown(false);
            listCate.Insert(0, new DropdownItem() {Text = "--- Select Branch ---", Value = "-1"});
            ViewBag.ListCategory = listCate;
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return RedirectToAction("Add", item);
            if (
                await
                    UnitOfWork.ShopRepo.AnyAsync(
                        x => x.Id != item.Id && x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", $"shop name \"{item.Name}\" already exists");
                return RedirectToAction("Add", item);
            }
            var result = 0;
            if (item.Id == 0)
            {
                item.CreateDate = DateTime.Now;
                item.UpdateDate = DateTime.Now;
                UnitOfWork.ShopRepo.Add(item);
                result = await UnitOfWork.ShopRepo.SaveAsync();
                if (result > 0)
                    TempData["Msg"] = $"Add successful shop <b>{item.Name}</b>";
            }
            else
            {
                var tmpItem = await UnitOfWork.ShopRepo.SingleOrDefaultAsync(x => x.Id == item.Id);

                if (tmpItem == null)
                {
                    ModelState.AddModelError("Name", $"shop name \"{item.Name}\" does not exist");
                    return RedirectToAction("Add", item);
                }
                tmpItem.Name = item.Name;
                tmpItem.CategoryId = item.CategoryId;
                tmpItem.CategoryName = item.CategoryName;
                tmpItem.Note = item.Note;
                tmpItem.Url = item.Url;
                tmpItem.Website = item.Website;
                item.UpdateDate = DateTime.Now;
                UnitOfWork.ShopRepo.Update(tmpItem);
                result = await UnitOfWork.ShopRepo.SaveAsync();
                if (result > 0)
                    TempData["Msg"] = $"Edit successfully shop <b>{item.Name}</b>";
            }
            if (result <= 0)
            {
                return RedirectToAction("Add", item);
            }

            return RedirectToAction("Add");
        }

        public int CheckExistsName(string name, int id)
        {
            int result = 0;
            if (UnitOfWork.ShopRepo.Any(x => x.Id != id && x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result = 1;
            }
            return result;
        }

        public JsonResult GetShopSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            var listShop = UnitOfWork.ShopRepo.Find(
                out totalRecord,
                x => !x.IsDelete && (x.Name.Contains(keyword) || x.Url.Contains(keyword)),
                x => x.OrderByDescending(y => y.Name),
                page ?? 1,
                10
            ).ToList();

            return
                Json(
                    new
                    {
                        incomplete_results = true,
                        total_count = totalRecord,
                        items = listShop.Select(x => new {id = x.Id, text = x.Name, url = x.Url, website = x.Website})
                    },
                    JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> AddFash(string name, string link)
        {
            try
            {
                var shop = await UnitOfWork.ShopRepo.SingleOrDefaultAsync(x => x.Url == link && !x.IsDelete);
                if (shop == null)
                {
                    var dateTime = DateTime.Now;
                    shop = new Shop()
                    {
                        Name = name,
                        Url = link,
                        Website = MyCommon.GetDomain(link),
                        CreateDate = dateTime,
                        UpdateDate = dateTime,
                    };

                    UnitOfWork.ShopRepo.Add(shop);

                    // Submit thêm Shop
                    await UnitOfWork.ShopRepo.SaveAsync();
                }
                else
                {
                    return Json(new { shop, status = MsgType.Warning, msg = "Link shop already exists!" }, JsonRequestBehavior.AllowGet);
                }

                var listShop = await UnitOfWork.ShopRepo.Entities.ToListAsync();

                return Json(new { shop, listShop, status = MsgType.Success, msg = "Add shop successful!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex).Throw();
                throw;
            }
        }

        [LogTracker(EnumAction.View, EnumPage.Shop)]
        public async Task<ActionResult> Detail(int id)
        {
            var shop = await UnitOfWork.ShopRepo.FirstOrDefaultAsync(x => x.Id == id);
            return View(shop);
        }

        public ActionResult GetDetailPopup(int id)
        {
            var model = UnitOfWork.ShopRepo.GetAll(id);
            return PartialView(model);
        }
    }
}