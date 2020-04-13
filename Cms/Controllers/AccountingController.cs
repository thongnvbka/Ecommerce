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
    public class AccountingController : BaseController
    {
        #region Các hàm xử lý trên danh sách cấu hình đối tượng kế toán
        // GET: Accounting
        [LogTracker(EnumAction.View, EnumPage.AccountantSubject)]
        public ActionResult Index()
        {
            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.AccountantSubject)]
        public async Task<ActionResult> AccountantSubject(ModelView<AccountantSubject, Library.ViewModels.AccountantSubjectViewModel> model)
        {
            long totalRecord;
            //model.PageInfo.PageSize = 3;
            model.Items = await UnitOfWork.AccountantSubjectRepo.FindAsync(
                        out totalRecord,
                        x => !x.IsDelete,
                        x => x.OrderByDescending(y => y.Id),
                        model.PageInfo.CurrentPage,
                        model.PageInfo.PageSize
                    );
            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("AccountantSubject", "Accounting");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AccountantSubjectList", model);
            }

            return View(model);
        }
        public ActionResult CreateSubject()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Add, EnumPage.AccountantSubject)]
        public async Task<ActionResult> CreateSubject(AccountantSubjectMeta model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return View();

            // Tên đối tượng kế toán đã tồn tại
            if (await UnitOfWork.AccountantSubjectRepo.AnyAsync(x => x.SubjectName.Equals(model.SubjectName) && !x.IsDelete))
            {
                ModelState.AddModelError("SubjectName", $"Accounting subject name \"{model.SubjectName }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customertype = Mapper.Map<AccountantSubject>(model);
                    UnitOfWork.AccountantSubjectRepo.Add(customertype);
                    var rs = await UnitOfWork.CategoryRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View();
                    }
                    TempData["Msg"] = $"Accounting subject added successfully \"<b>{customertype.SubjectName}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("CreateSubject");
        }

        [LogTracker(EnumAction.Update, EnumPage.AccountantSubject)]
        public async Task<ActionResult> EditSubject(int id)
        {
            var customertype = await UnitOfWork.AccountantSubjectRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customertype == null)
                return HttpNotFound($"There is no accounting subject with the Id of {id}");

            return View(Mapper.Map<AccountantSubjectMeta>(customertype));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Update, EnumPage.AccountantSubject)]
        public async Task<ActionResult> EditSubject(AccountantSubjectMeta model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var customertype = await UnitOfWork.AccountantSubjectRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (customertype == null)
            {
                ModelState.AddModelError("NotExist", "Accounting subject does not exist or has been deleted");
                return View(model);
            }

            // Tên đối tượng kế toán  đã tồn tại
            if (
                await UnitOfWork.AccountantSubjectRepo.AnyAsync(
                        x => x.SubjectName.Equals(model.SubjectName) && !x.IsDelete && x.Id != model.Id))
            {
                ModelState.AddModelError("SubjectName", $"Accounting subject name  \"{model.SubjectName}\" already exists");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    customertype = Mapper.Map(model, customertype);
                    var rs = await UnitOfWork.AccountantSubjectRepo.SaveAsync();
                    if (rs <= 0)
                    {
                        return View(model);
                    }
                    TempData["Msg"] = $"Successfully updated accounting subject \"<b>{customertype.SubjectName}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("AccountantSubject");
        }

        [HttpPost]
        [LogTracker(EnumAction.Delete, EnumPage.AccountantSubject)]
        public async Task<ActionResult> DeleteSubject(int id)
        {
            var customertype = await UnitOfWork.AccountantSubjectRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (customertype == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            customertype.IsDelete = true;
            UnitOfWork.AccountantSubjectRepo.Update(customertype);

            var rs = await UnitOfWork.AccountantSubjectRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Các hàm lấy thông tin

        #endregion
    }
}