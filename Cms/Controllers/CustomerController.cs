using Common.Items;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Emums;
using Common.Helper;


namespace Cms.Controllers
{
    public class CustomerController : BaseController
    {
        // GET: CustomerMng
        public async Task<ActionResult> Index(ModelView<Customer, CustomerViewModel> model)
        {
            long totalRecord;
            model.PageInfo.PageSize = 3;
            model.Items = await UnitOfWork.CustomerRepo.FindAsync(
                out totalRecord,
                x => !(bool)x.IsDelete,
                x => x.OrderByDescending(y => y.FullName),
                model.PageInfo.CurrentPage,
                model.PageInfo.PageSize);

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Url = Url.Action("Index", "Customer");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_List", model);
            }
            return View(model);
        }
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == id && (bool)!x.IsDelete);

            if (customer == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);

            customer.IsDelete = true;
            UnitOfWork.CustomerRepo.Update(customer);

            var rs = await UnitOfWork.CustomerRepo.SaveAsync();

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Profile()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult GetSearch(int page, int pageSize, string keyword)
        {
            long totalRecord;

            var listCustomer = UnitOfWork.CustomerRepo.Find(
                   out totalRecord,
                   x => !x.IsDelete && x.IsActive && x.FullName.Contains(keyword),
                   x => x.OrderByDescending(y => y.FullName),
                   page,
                   pageSize
              ).ToList();

            return Json(new { totalRecord, listCustomer }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = String.IsNullOrEmpty(keyword) ? "" : keyword.Trim();
            //long totalStaff;
            List<dynamic> items = new List<dynamic>();

            var listCustomer = UnitOfWork.CustomerRepo.Find(
                   out totalRecord,
                    x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.UserFullName.Contains(keyword) || x.Address.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.SystemName.Contains(keyword) || x.Code.Contains(keyword)),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   10
              ).ToList();

            if (UserState.OfficeType == (byte)OfficeType.Deposit || UserState.OfficeType == (byte)OfficeType.Business)
            {
                listCustomer = UnitOfWork.CustomerRepo.Find(
                   out totalRecord,
                    x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.UserFullName.Contains(keyword) || x.Address.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.SystemName.Contains(keyword) || x.Code.Contains(keyword))
                     && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   10
              ).ToList();
            }

            //var listStaff = UnitOfWork.UserRepo.Find(
            //       out totalStaff,
            //       x => !x.IsDelete && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword)),
            //       x => x.OrderByDescending(y => y.FullName),
            //       page ?? 1,
            //       5
            //  ).ToList();

            items.AddRange(listCustomer.Select(
                    x =>
                        new
                        {
                            id = x.Id,
                            code = x.Code,
                            text = x.FullName,
                            email = x.Email,
                            phone = x.Phone,
                            avatar = x.Avatar,
                            systemName = x.SystemName,
                            address = x.Address
                        }));
            //items.AddRange(listStaff.Select(
            //        x =>
            //            new
            //            {
            //                id = x.Id,
            //                code = "",
            //                text = x.FullName,
            //                email = x.Email,
            //                phone = x.Phone,
            //                avatar = x.Avatar,
            //                systemName = "",
            //                address = ""
            //            }));

            return Json(new { incomplete_results = true, total_count = totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubjectSearch(string keyword, int? page)
        {
            long totalCustomer;
            long totalStaff;
            List<dynamic> items = new List<dynamic>();

            //keyword = MyCommon.Ucs2Convert(keyword);
            keyword = String.IsNullOrEmpty(keyword) ? "" : keyword.Trim();

            var listCustomer = UnitOfWork.CustomerRepo.Find(
                   out totalCustomer,
                   x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.UserFullName.Contains(keyword) || x.Address.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.SystemName.Contains(keyword) || x.Code.Contains(keyword)),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   5
              ).ToList();

            var listStaff = UnitOfWork.UserRepo.Find(
                   out totalStaff,
                   x => !x.IsDelete && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword)),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   5
              ).ToList();

            items.AddRange(listCustomer.Select(
                    x =>
                        new
                        {
                            id = x.Id,
                            code = x.Code,
                            text = x.FullName,
                            email = x.Email,
                            phone = x.Phone,
                            avatar = x.Avatar,
                            systemName = x.SystemName,
                            address = x.Address
                        }));
            items.AddRange(listStaff.Select(
                    x =>
                        new
                        {
                            id = x.Id,
                            code = "",
                            text = x.FullName,
                            email = x.Email,
                            phone = x.Phone,
                            avatar = x.Avatar,
                            systemName = "",
                            address = ""
                        }));

            return Json(new { incomplete_results = true, total_count = totalCustomer + totalStaff, items }, JsonRequestBehavior.AllowGet);
        }

    }
}