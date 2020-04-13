using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common.Items;
using Library.DbContext.Entities;
using Library.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    public class WarehouseConfigController : BaseController
    {
        // GET: WarehouseConfig
        public async Task<ActionResult> Warehouse(ModelView<Warehouse, Library.ViewModels.WarehouseViewModel> model)
        {
            long totalRecord;
            if (model.SearchInfo == null)
            {
                List<SelectListItem> ListUser = new List<SelectListItem>()
            {
                new SelectListItem { Text ="All", Value = "-1", Selected = true }
            };

                foreach (var item in UnitOfWork.DbContext.Warehouses.Select(x => new { x.UserId, x.UserFullName }).ToList())
                {
                    ListUser.Add(new SelectListItem() { Value = item.UserId.ToString(), Text = item.UserFullName });
                }

                var data = new Library.ViewModels.WarehouseViewModel();
                data.ListUser = ListUser;

                model.SearchInfo = data;
            }

            if (model.SearchInfo.Keyword == null)
            {
                model.Items = await UnitOfWork.WarehouseRepo.FindAsync(
                        out totalRecord,
                        x => ((model.SearchInfo.Status == -1 || x.Status == model.SearchInfo.Status) &&
                            (model.SearchInfo.Country == "All" || x.Country == model.SearchInfo.Country) &&
                            (model.SearchInfo.UserId == -1 || x.UserId == model.SearchInfo.UserId)),
                        x => x.OrderBy(y => y.Name),
                        model.PageInfo.CurrentPage,
                        model.PageInfo.PageSize
                    );
            }
            else
            {
                model.Items = await UnitOfWork.WarehouseRepo.FindAsync(
                        out totalRecord,
                        x => (x.Code.Contains(model.SearchInfo.Keyword) ||
                            x.Name.Contains(model.SearchInfo.Keyword) ||
                            x.Description.Contains(model.SearchInfo.Keyword)
                        ),
                        x => x.OrderBy(y => y.Name),
                        model.PageInfo.CurrentPage,
                        model.PageInfo.PageSize
                    );
            }

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Warehouse", "WarehouseConfig");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_LayoutList", model);
            }

            return View(model);
        }

        public ActionResult WarehouseCreate()
        {
            ViewBag.ListUser = UnitOfWork.DbContext.Users.Where(x => !x.IsDelete && x.Status < 2).Select(x => new List<dynamic>() { x.Id + "", x.FullName }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> WarehouseCreate(WarehouseMeta model)
        {
            ViewBag.ListUser = UnitOfWork.DbContext.Users.Where(x => !x.IsDelete && x.Status < 2).Select(x => new List<dynamic>() { x.Id + "", x.FullName }).ToList();
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
                return View();

            // Customer IDo đã tồn tại
            if (await UnitOfWork.WarehouseRepo.AnyAsync(x => x.Code.Equals(model.Code)))
            {
                ModelState.AddModelError("Code", $"Warehouse code\"{model.Code }\" already exists");
                return View();
            }

            // Tên kho đã tồn tại
            if (await UnitOfWork.WarehouseRepo.AnyAsync(x => x.Name.Equals(model.Name)))
            {
                ModelState.AddModelError("Name", $"Warehouse name \"{model.Name }\" already exists");
                return View();
            }

            var user = UnitOfWork.UserRepo.Find(model.UserId);

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var warehouse = Mapper.Map<Warehouse>(model);

                    warehouse.Created = DateTime.Now;
                    warehouse.Updated = DateTime.Now;
                    warehouse.Phone = user.Phone;
                    warehouse.UserFullName = user.FullName;

                    UnitOfWork.WarehouseRepo.Add(warehouse);
                    var rs = await UnitOfWork.WarehouseRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }

                    TempData["Msg"] = $"Successfully added the warehouse \"<b>{warehouse.Code}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("WarehouseCreate");
        }

        public async Task<ActionResult> WarehouseEdit(int id)
        {
            ViewBag.ListUser = UnitOfWork.DbContext.Users.Where(x => !x.IsDelete && x.Status < 2).Select(x => new List<dynamic>() { x.Id + "", x.FullName }).ToList();
            var warehouse = await UnitOfWork.WarehouseRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (warehouse == null)
                return HttpNotFound($"Not found Warehouse has warehouse Id is {id}");//Không có kho hàng có Id là

            return View(Mapper.Map<WarehouseMeta>(warehouse));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> WarehouseEdit(WarehouseMeta model)
        {
            ViewBag.ListUser = UnitOfWork.DbContext.Users.Where(x => !x.IsDelete && x.Status < 2).Select(x => new List<dynamic>() { x.Id + "", x.FullName }).ToList();

            if (!ModelState.IsValid)
                return View(model);

            var warehouse = await UnitOfWork.WarehouseRepo.SingleOrDefaultAsync(x => x.Id == model.Id);

            if (warehouse == null)
            {
                ModelState.AddModelError("NotExist", "Warehouse does not exist or has been deleted");
                return View(model);
            }

            // Customer IDo đã tồn tại
            if (await UnitOfWork.WarehouseRepo.AnyAsync(x => x.Code.Equals(model.Code) && x.Id != model.Id))
            {
                ModelState.AddModelError("Code", $"Warehouse Id \"{model.Code }\" already exists");
                return View(model);
            }

            // Tên kho đã tồn tại
            if (await UnitOfWork.WarehouseRepo.AnyAsync(x => x.Name.Equals(model.Name) && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Warehouse name \"{model.Name }\" already exists");
                return View(model);
            }

            var user = UnitOfWork.UserRepo.Find(model.UserId);

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    warehouse.Phone = user.Phone;
                    warehouse.UserFullName = user.FullName;

                    warehouse = Mapper.Map(model, warehouse);

                    warehouse.Updated = DateTime.Now;

                    //UnitOfWork.OfficeRepo.Add(office);

                    var rs = await UnitOfWork.OfficeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    await UnitOfWork.OfficeRepo.SaveAsync();

                    TempData["Msg"] = $"Updated successfully \"<b>{warehouse.Code}</b>\"";//Cập nhật thành công kho hàng

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Warehouse");
        }

        [HttpPost]
        public async Task<ActionResult> WarehouseDelete(int id)
        {
            var warehouse = await UnitOfWork.WarehouseRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (warehouse == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            //warehouse.IsDelete = true;
            UnitOfWork.WarehouseRepo.Remove(warehouse);

            var rs = await UnitOfWork.WarehouseRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Layout()
        {
            return View();
        }

        public ActionResult LayoutType()
        {
            return View();
        }

        public ActionResult FlowCoordinator()
        {
            return View();
        }
    }
}