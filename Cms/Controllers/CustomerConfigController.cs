using AutoMapper;
using Common.Items;
using Library.DbContext.Entities;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.UnitOfWork;
using Library.ViewModels;
using Library.Models;
using System;
using System.Runtime.ExceptionServices;
using Common.Emums;
using Cms.Attributes;

namespace Cms.Controllers
{
    [Authorize]
    public class CustomerConfigController : BaseController
    {
        #region customer level

        // GET: CustomerConfig
        [LogTracker(EnumAction.View, EnumPage.Level)]
        public async Task<ActionResult> Level(ModelView<CustomerLevel, Library.ViewModels.CustomerLevelViewModel> model)
        {
            long totalRecord;
            model.Items = await UnitOfWork.CustomerLevelRepo.FindAsync(
                out totalRecord,
                //null,
                x => !x.IsDelete,
                x => x.OrderBy(y => y.Id),
                model.PageInfo.CurrentPage,
                model.PageInfo.PageSize
            );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Level", "CustomerConfig");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_LevelList", model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCustomerLevel(int id)
        {
            var customerLevel = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customerLevel == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            customerLevel.IsDelete = true;
            UnitOfWork.CustomerLevelRepo.Update(customerLevel);

            var rs = await UnitOfWork.CustomerLevelRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCustomerLevel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCustomerLevel(CustomerLevelMeta model)
        {
            ModelState.Remove("Id");
            ModelState.Remove("Status");
            ModelState.Remove("IsDelete");

            if (!ModelState.IsValid)
                return View();

            // Tên level khách hàng đã tồn tại
            if (await UnitOfWork.CustomerLevelRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete == true && x.Status == true))
            {
                ModelState.AddModelError("Name", $"Name of customer level \"{model.Name }\" already exists");
                return View();
            }
            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customerLevel = Mapper.Map<CustomerLevel>(model);
                    UnitOfWork.CustomerLevelRepo.Add(customerLevel);
                    var rs = await UnitOfWork.CategoryRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View();
                    }
                    TempData["Msg"] = $"Customer group added successfully \"<b>{customerLevel.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("CreateCustomerLevel");
        }

        public async Task<ActionResult> EditCustomerLevel(int id)
        {
            var customerLevel = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customerLevel == null)
                return HttpNotFound($"There is no customer level with the ID {id}");

            return View(Mapper.Map<CustomerLevelMeta>(customerLevel));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCustomerLevel(CustomerLevelMeta model)
        {
            ModelState.Remove("Status");
            ModelState.Remove("IsDelete");
            if (!ModelState.IsValid)
                return View(model);

            var customerLevel = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (customerLevel == null)
            {
                ModelState.AddModelError("NotExist", "Customer group does not exist or has been deleted");
                return View(model);
            }

            // Tên nhóm khách hàng  đã tồn tại
            if (
                await UnitOfWork.CustomerTypeRepo.AnyAsync(
                        x => x.NameType.Equals(model.Name) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Customer group name \"{model.Name}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    customerLevel = Mapper.Map(model, customerLevel);
                    var rs = await UnitOfWork.CustomerLevelRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View(model);
                    }
                    TempData["Msg"] = $"Customer group updated successfully \"<b>{customerLevel.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Level");
        }

        #endregion customer level

        #region Favorable

        //public async Task<ActionResult> Favorable(ModelView<CustomerSale, CustomerSaleViewModel> model)
        //{
        //    long totalRecord;
        //    model.PageInfo.PageSize = 3;
        //    model.Items = await UnitOfWork.CustomerSaleRepo.FindAsync(
        //        out totalRecord,
        //        x => (bool)!x.IsDelete,
        //        x => x.OrderByDescending(y => y.CardNumber),
        //        model.PageInfo.CurrentPage,
        //        model.PageInfo.PageSize);

        //    model.PageInfo.TotalRecord = (int)totalRecord;
        //    model.PageInfo.Url = Url.Action("Favorable", "CustomerConfig");

        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_FavorableList", model);
        //    }
        //    return View(model);
        //}

        #endregion Favorable

        #region Customer type

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [LogTracker(EnumAction.View, EnumPage.Type)]
        public async Task<ActionResult> Type(ModelView<CustomerType, Library.ViewModels.CustomerTypeViewModel> model)
        {
            long totalRecord;
            model.Items = await UnitOfWork.CustomerTypeRepo.FindAsync(
                        out totalRecord,
                        x => !x.IsDelete,
                        x => x.OrderByDescending(y => y.Id),
                        model.PageInfo.CurrentPage,
                        model.PageInfo.PageSize
                    );
            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Type", "CustomerConfig");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_TypeList", model);
            }

            return View(model);
        }

        public ActionResult CreateCustomerType()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCustomerType(CustumerTypeMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return View();

            // Tên nhóm khách hàng đã tồn tại
            if (await UnitOfWork.CustomerTypeRepo.AnyAsync(x => x.NameType.Equals(model.NameType) && !x.IsDelete))
            {
                ModelState.AddModelError("NameType", $"Customer group name \"{model.NameType }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customertype = Mapper.Map<CustomerType>(model);
                    UnitOfWork.CustomerTypeRepo.Add(customertype);
                    var rs = await UnitOfWork.CustomerTypeRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View();
                    }
                    TempData["Msg"] = $"Customer group added successfully \"<b>{customertype.NameType}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("CreateCustomerType");
        }

        public async Task<ActionResult> EditCustomerType(int id)
        {
            var customertype = await UnitOfWork.CustomerTypeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customertype == null)
                return HttpNotFound($"There is no customer group with ID {id}");

            return View(Mapper.Map<CustumerTypeMeta>(customertype));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditCustomerType(CustumerTypeMeta model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customertype = await UnitOfWork.CustomerTypeRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (customertype == null)
            {
                ModelState.AddModelError("NotExist", "Customer group does not exist or has been deleted");
                return View(model);
            }

            // Tên nhóm khách hàng  đã tồn tại
            if (
                await UnitOfWork.CustomerTypeRepo.AnyAsync(
                        x => x.NameType.Equals(model.NameType) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("NameType", $"Customer group name \"{model.NameType}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    customertype = Mapper.Map(model, customertype);
                    var rs = await UnitOfWork.CustomerTypeRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View(model);
                    }
                    TempData["Msg"] = $"Customer group updated successfully \"<b>{customertype.NameType}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("Type");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteCustomerType(int id)
        {
            var customertype = await UnitOfWork.CustomerTypeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customertype == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            customertype.IsDelete = true;
            UnitOfWork.CustomerTypeRepo.Update(customertype);

            var rs = await UnitOfWork.CustomerTypeRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion Customer type
    }
}