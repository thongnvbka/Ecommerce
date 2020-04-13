using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.DbContext.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using Library.ViewModels;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using Common.Constant;
using AutoMapper;
using Cms.Attributes;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    public class WithDrawalController : BaseController
    {
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.WithDrawal)]
        public async Task<JsonResult> GetAllWithDrawalList(int page, int pageSize, DrawalSearchModal searchModal)
        {
            List<Draw> drawalModal;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new DrawalSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                drawalModal = await UnitOfWork.DrawRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword)
                    || x.CustomerName.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword))
                    && (searchModal.Status == -1 || x.Status == searchModal.Status)
                    && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                    && (searchModal.UserId == -1 || x.UserId == searchModal.UserId)
                    && x.CreateDate >= dateStart
                    && x.CreateDate <= dateEnd,
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }
            else
            {
                drawalModal = await UnitOfWork.DrawRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword)
                    || x.CustomerName.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword))
                    && (searchModal.Status == -1 || x.Status == searchModal.Status)
                    && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                    && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, drawalModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.WithDrawal)]
        public JsonResult GetWithDrawalSearchData()
        {
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listUser = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listCustomer = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            // Lấy các trạng thái Status
            foreach (WithDrawalStatus withDrawalStatus in Enum.GetValues(typeof(WithDrawalStatus)))
            {
                if (withDrawalStatus >= 0)
                {
                    listStatus.Add(new { Text = withDrawalStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)withDrawalStatus });
                }
            }

            return Json(new { listStatus, listUser, listCustomer }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.WithDrawal)]
        public async Task<JsonResult> GetWithDrawalDetail(int drawalId)
        {
            var withDrawalModal = await UnitOfWork.DrawRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == drawalId);
            if (withDrawalModal == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = Result.Succeed, withDrawalModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.WithDrawal)]
        public JsonResult DeleteWithDrawal(int drawalId)
        {
            // Kiểm tra thông tin phiếu yêu cầu có tồn tại hay không
            var withDrawalDetail = UnitOfWork.DrawRepo.FirstOrDefault(x => x.Id == drawalId);
            if (withDrawalDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Nếu Processed thì không cho xóa
            if (withDrawalDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.WithDrawalIsApproval }, JsonRequestBehavior.AllowGet);
            }

            //Lưu xuống Database
            UnitOfWork.DrawRepo.Remove(withDrawalDetail);
            UnitOfWork.DrawRepo.Save();

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteWithDrawalIsSuccess }, JsonRequestBehavior.AllowGet);
        }

    }
}