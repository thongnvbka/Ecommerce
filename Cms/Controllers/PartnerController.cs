using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using AutoMapper;
using Common.Constant;
using Library.DbContext.Entities;
using Library.ViewModels.Notifi;
using System;
using System.Collections.Generic;

namespace Cms.Controllers
{
    [Authorize]
    public class PartnerController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.Partner)]
        public ActionResult Index()
        {
            return View();
        }

        [CheckPermission(EnumAction.Add, EnumPage.Partner)]
        public async Task<ActionResult> Suggetion(string term, int pageIndex = 1, int recordPerPage = 20)
        {
            term = MyCommon.Ucs2Convert(term);

            long totalRecord;
            var items = await
                    UnitOfWork.PartnerRepo.FindAsync(out totalRecord,
                        x => !x.IsDelete && x.Status == (byte)PartnerStatus.Current && x.UnsignName.Contains(term),
                        partners => partners.OrderBy(partner => partner.Id), pageIndex, recordPerPage);

            return JsonCamelCaseResult(new { totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách khách hàng Official
        /// POST: /NotifiCommon/GetListData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetListData(int page, int pageSize)
        {
            List<Partner> partnerModal;
            long totalRecord;

            partnerModal = await UnitOfWork.PartnerRepo.FindAsync(
                   out totalRecord,
                   x => !x.IsDelete,
                   x => x.OrderBy(y => y.PriorityNo),
                   page,
                   pageSize
               );
            return Json(new { totalRecord, partnerModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa partner
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeletePartner(int id)
        {
            var tmpPartner = UnitOfWork.PartnerRepo.FirstOrDefault(x => x.Id == id);
            if (tmpPartner == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            tmpPartner.IsDelete = true;
            UnitOfWork.PartnerRepo.Update(tmpPartner);
            var rs = UnitOfWork.PartnerRepo.Save();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Detail notifi
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDetailPartner(int id)
        {
            var tmpObj = UnitOfWork.PartnerRepo.FirstOrDefault(x => x.Id == id);
            if (tmpObj == null)
            {
                tmpObj = new Partner();
                return JsonCamelCaseResult(tmpObj, JsonRequestBehavior.AllowGet);
            }
            return Json(tmpObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới đối tác
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult CreateNew(Partner model)
        {
            var typeSuccess = "There was an error in the implementation process. Please try again in a moment!";
            // Kiểm tra Title đã tồn tại hay chưa
            var tmpObj = UnitOfWork.PartnerRepo.FirstOrDefaultAsNoTracking(
                x => x.Name.Equals(model.Name));

            // Tên đăng nhập hoặc Password không đúng
            if (tmpObj != null)
            {
                return Json(new { status = Result.Failed, msg = "Shipping name already exists. Please check again!" },
                    JsonRequestBehavior.AllowGet);
            }
            if (model.Id == 0)
            {
                model.IsDelete = false;
                model.UnsignName = MyCommon.ConvertVni2Unicode(model.Name);
                UnitOfWork.PartnerRepo.Add(model);
                UnitOfWork.PartnerRepo.Save();
                typeSuccess = "Add new success";
            }
            else
            {
                var obj = UnitOfWork.PartnerRepo.FirstOrDefaultAsNoTracking(
                x => x.Id == model.Id);
                obj.Status = model.Status;
                obj.UnsignName = MyCommon.ConvertVni2Unicode(model.Name);
                obj.Description = model.Description;
                obj.Name = model.Name;
                obj.Note = model.Note;
                obj.PriorityNo = model.PriorityNo;
                UnitOfWork.PartnerRepo.Update(obj);
                UnitOfWork.PartnerRepo.Save();
                typeSuccess = "Edit success";
            }


            return Json(new { msgType = Result.Succeed, msg = typeSuccess },
                JsonRequestBehavior.AllowGet);
        }
    }
}