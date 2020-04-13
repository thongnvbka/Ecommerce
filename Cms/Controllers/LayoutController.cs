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
using Library.Models;
using Library.ViewModels;
using Library.ViewModels.Warehouse;
using System.Data.Entity;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Common.Host;
using Newtonsoft.Json;

namespace Cms.Controllers
{
    [Authorize]
    public class LayoutController : BaseController
    {
        // GET: Layout
        [LogTracker(EnumAction.View, EnumPage.Layout)]
        public async Task<ActionResult> Index(ModelView<Layout, LayoutFilderViewModel> model)
        {
            ViewBag.WareHouseJsTree = GetListWareHouse();
            if (model.SearchInfo == null)
                model.SearchInfo = new LayoutFilderViewModel() {
                    Keyword = "",
                    Status = -1,
                    WarehouseId = -1
                };

            long totalRecord;

            model.SearchInfo.Keyword = MyCommon.Ucs2Convert(model.SearchInfo.Keyword);

            model.Items = await UnitOfWork.LayoutRepo.FindAsync(out totalRecord,
                x =>
                    !x.IsDelete && (model.SearchInfo.Status == -1 || (model.SearchInfo.Status != -1 && x.Status == model.SearchInfo.Status)) &&
                    (model.SearchInfo.WarehouseId == -1 || (model.SearchInfo.WarehouseId != -1 && x.WarehouseId == model.SearchInfo.WarehouseId)) &&
                    (model.SearchInfo.Keyword == "" || x.UnsignName.Contains(model.SearchInfo.Keyword)),
                x => x.OrderBy(y => y.IdPath),
                model.PageInfo.CurrentPage, model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Layout";
            model.PageInfo.Url = Url.Action("Index", "Layout");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }

            return View(model);
        }

        public ActionResult Create()
        {
            ViewBag.WareHouseJsTree = GetListWareHouse();
            return View();
        }

        public async Task<string> GetLayoutJsTreeByWareHouse(int? selectedId = null)
        {
            var offices = await
                UnitOfWork.LayoutRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && (x.WarehouseId == selectedId))
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToListAsync();

            var listJsTree = offices.Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentLayoutId?.ToString() ?? "#",
                state = new { opened = !o.ParentLayoutId.HasValue, selected = selectedId != null && o.Id == selectedId },
                idPath = o.IdPath,
                code = o.Code
            });

            return JsonConvert.SerializeObject(listJsTree);
        }

        public List<DropdownItem> GetListWareHouse()
        {
            var list = new List<DropdownItem>();
            var layouts = UnitOfWork.OfficeRepo.Entities.Where(x => !x.IsDelete && x.Status < 2 && x.Type == (byte)OfficeType.Warehouse)
                    .OrderByDescending(x => x.IdPath)
                    .AsNoTracking().ToList();

            list = layouts.Select(o => new DropdownItem()
            {
                Value = o.Id.ToString(),
                Text = o.Name
            }).ToList();
            list.Insert(0, new DropdownItem() { Text = "Choose warehouse", Value = "-1" });
            return list;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LayoutMeta model)
        {
            ModelState.Remove("Id");
            ViewBag.layoutJsTree = await GetLayoutJsTree();
            ViewBag.WareHouseJsTree = GetListWareHouse();

            if (!ModelState.IsValid)
                return View();

            Layout layoutParent = null;

            // Kiểm tra layout Cha có tồn tại hay không
            if (model.ParentLayoutId.HasValue)
            {
                layoutParent =
                    await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == model.ParentLayoutId.Value && !x.IsDelete);
                if (layoutParent == null)
                {
                    ModelState.AddModelError("ParentLayoutId",
                        $"Layout cha \"{model.ParentLayoutName}\" Does not exist or has been deleted");
                    return View();
                }
                model.ParentLayoutName = layoutParent.Name;
            }

            // Mã layout đã tồn tại
            if (await UnitOfWork.LayoutRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete))
            {
                ModelState.AddModelError("Code", $"Layout code \"{model.Code}\" already exists");
                return View();
            }

            // Tên layout đã tồn tại
            if (
                await
                    UnitOfWork.LayoutRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentLayoutId == model.ParentLayoutId.Value))
            {
                ModelState.AddModelError("Name", $"Layout name \"{model.Name}\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var layout = Mapper.Map<Layout>(model);

                    layout.IsDelete = false;
                    layout.UnsignName = MyCommon.Ucs2Convert($"{layout.Name} {layout.Code}");
                    layout.Created = DateTime.Now;
                    layout.Updated = DateTime.Now;
                    layout.ChildNo = 0;

                    UnitOfWork.LayoutRepo.Add(layout);

                    var rs = await UnitOfWork.LayoutRepo.SaveAsyncNoCheck();

                    if (rs <= 0)
                    {
                        return View();
                    }

                    // Cập nhật lại IdPath và NamePath cho layout

                    if (layoutParent == null)
                    {
                        layout.IdPath = layout.Id.ToString();
                        layout.NamePath = layout.Name;
                    }
                    else
                    {
                        layout.IdPath = $"{layoutParent.IdPath}.{layout.Id}";
                        layout.NamePath = $"{layoutParent.NamePath} / {layout.Name}";

                        // Cập nhật lại số lượng layout chả
                        var childNo = await UnitOfWork.LayoutRepo.CountAsync(x => x.ParentLayoutId == layoutParent.Id && !x.IsDelete);
                        layoutParent.ChildNo = childNo;
                    }

                    await UnitOfWork.LayoutRepo.SaveAsyncNoCheck();

                    TempData["Msg"] = $"Layout added successfully \"<b>{layout.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    OutputLog.WriteOutputLog(ex);
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Create");
        }

        // GET: Layout/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var layout = await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (layout == null)
                return HttpNotFound($"There isn't such position with id as {id}");

            ViewBag.layoutJsTree = await GetLayoutJsTree();
            ViewBag.WareHouseJsTree = GetListWareHouse();

            return View(Mapper.Map<LayoutMeta>(layout));
        }

        // POST: Layout/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LayoutMeta model)
        {
            ViewBag.layoutJsTree = await GetLayoutJsTree();
            ViewBag.WareHouseJsTree = GetListWareHouse();

            if (!ModelState.IsValid)
                return View(model);

            var layout = await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (layout == null)
            {
                ModelState.AddModelError("NotExist", "Layout does not exist or has been deleted");
                return View(model);
            }

            Layout layoutParent = null;

            // Có thay đổi layout cha
            if (model.ParentLayoutId != layout.ParentLayoutId)
            {
                // Kiểm tra layout Cha có tồn tại hay không
                if (model.ParentLayoutId.HasValue)
                {
                    layoutParent =
                        await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == model.ParentLayoutId.Value && !x.IsDelete);
                    if (layoutParent == null)
                    {
                        ModelState.AddModelError("ParentId",
                            $"Parent layout \"{model.ParentLayoutName}\" does not exist or has been deleted");
                        return View(model);
                    }
                    model.ParentLayoutName = layoutParent.Name;
                }
                
            }


            // Mã layout đã tồn tại
            if (await UnitOfWork.LayoutRepo.AnyAsync(x => x.Code.Equals(model.Code) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("Code", $"Layout code \"{model.Code}\" already exists");
                return View(model);
            }

            // Tên layout đã tồn tại
            if (
                await UnitOfWork.LayoutRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentLayoutId == model.ParentLayoutId.Value && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Layout name \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = layout.ParentLayoutId;
                    var oldIdPath = layout.IdPath;
                    layout.WarehouseId = model.WarehouseId;
                    layout.WarehouseName = model.WarehouseName;
                    layout.Name = model.Name;
                    layout.Mode = model.Mode;
                    layout.Code = model.Code;
                    layout.ParentLayoutId = model.ParentLayoutId;
                    layout.ParentLayoutName = model.ParentLayoutName;
                    layout.Description = model.Description;
                    layout.Status = model.Status;
                    layout.Length = model.Length;
                    layout.Width = model.Width;
                    layout.Height = model.Height;
                    layout.MaxWeight = model.MaxWeight;

                    layout.IsDelete = false;
                    layout.UnsignName = MyCommon.Ucs2Convert($"{layout.Name} {layout.Code}");
                    layout.Updated = DateTime.Now;
                    
                    var rs = await UnitOfWork.LayoutRepo.SaveAsyncNoCheck();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho layout
                    if (model.ParentLayoutId != oldParentId)
                    {
                        if (layoutParent == null)
                        {
                            layout.IdPath = layout.Id.ToString();
                            layout.NamePath = layout.Name;
                        }
                        else
                        {
                            layout.IdPath = $"{layoutParent.IdPath}.{layout.Id}";
                            layout.NamePath = $"{layoutParent.NamePath} / {layout.Name}";

                            // Cập nhật lại số lượng layout cha cũ
                            var childNo = await UnitOfWork.LayoutRepo.CountAsync(x => x.ParentLayoutId == oldParentId && !x.IsDelete);
                            var oldLayoutParent = await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == oldParentId);
                            if (oldLayoutParent != null)
                            {
                                oldLayoutParent.ChildNo = childNo;
                            }
                            // Cập nhật lại số lượng layout cha mới
                            var childNo1 = await UnitOfWork.LayoutRepo.CountAsync(x => x.ParentLayoutId == layoutParent.Id && !x.IsDelete);
                            layoutParent.ChildNo = childNo1;

                        }

                        // Cập nhật lại IdPath của tất cả các layout bên dưới
                        var listSubLayout = await UnitOfWork.LayoutRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));

                        listSubLayout.ForEach(o =>
                        {
                            o.IdPath = $"{layout.IdPath}{o.IdPath.Substring(oldIdPath.Length - 1, o.IdPath.Length)}";
                        });

                    }

                    await UnitOfWork.LayoutRepo.SaveAsyncNoCheck();

                    TempData["Msg"] = $"Layout updated successfully \"<b>{layout.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    OutputLog.WriteOutputLog(ex);
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Index");
        }

        // POST: Layout/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var layout = await UnitOfWork.LayoutRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            // Layout does not exist or has been deleted
            if (layout == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            // Không được xóa layout đã có layout con
            if (layout.ChildNo > 0)
                return JsonCamelCaseResult(-2, JsonRequestBehavior.AllowGet);

            // Cập nhật trạng thái IsDelete của layout
            layout.IsDelete = true;

            var rs = await UnitOfWork.LayoutRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
    }
}