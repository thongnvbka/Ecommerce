using AutoMapper;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.ViewModels.Notifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class NotifiCommonController : BaseController
    {
        // GET: NotifiCommon
        [LogTracker(EnumAction.View, EnumPage.NotifiCommon)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy dữ liệu system đổ lên searchData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetRenderSystem()
        {
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listSystem = new List<dynamic>() { new { Text = "- All -", Value = -1, Class = "active", ClassChild = "label-danger" } };
            var listSystemNo = new List<dynamic>();
            listStatus.Add(new { Text = "- Display -", Value = 0 });
            listStatus.Add(new { Text = "- Do not display -", Value = 1 });
            //2. Lấy danh sách System trên hệ thống
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    ClassChild = "label-primary"
                });
                listSystemNo.Add(new
                {
                    Text = item.Name,
                    Value = item.Id,
                    Class = "",
                    ClassChild = "label-primary"
                });
            }

            return Json(new { listStatus, listSystem, listSystemNo }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách khách hàng Official
        /// POST: /NotifiCommon/GetListData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetListData(int page, int pageSize, NotifiSearchModel searchModal)
        {
            List<NotifiCommon> notifiModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new NotifiSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            var tmpStatus = false;
            if (searchModal.Status == 1) tmpStatus = true;
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                notifiModal = await UnitOfWork.NotifiCommonRepo.FindAsync(
                    out totalRecord,
                    x => (x.Title.Contains(searchModal.Keyword))
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == tmpStatus)
                         && x.CreateDate >= dateStart && x.CreateDate <= dateEnd,
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }
            else
            {
                notifiModal = await UnitOfWork.NotifiCommonRepo.FindAsync(
                    out totalRecord,
                    x => (x.Title.Contains(searchModal.Keyword))
                         && (searchModal.SystemId == -1 || x.SystemId == searchModal.SystemId)
                         && (searchModal.Status == -1 || x.Status == tmpStatus),
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, notifiModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa notifi
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteNotitfi(long notiId)
        {
            var tmpNoti = UnitOfWork.NotifiCommonRepo.FirstOrDefault(x => x.Id == notiId);
            if (tmpNoti == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            UnitOfWork.NotifiCommonRepo.Remove(tmpNoti);
            var rs = UnitOfWork.NotifiCommonRepo.SaveAsync();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Detail notifi
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDetailNotitfi(long notiId)
        {
            var notiObj = UnitOfWork.NotifiCommonRepo.FirstOrDefault(x => x.Id == notiId);
            if (notiObj == null)
            {
                notiObj = new NotifiCommon();
                return JsonCamelCaseResult(notiObj, JsonRequestBehavior.AllowGet);
            }  
            return Json(notiObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới thông báo
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult CreateNew(NotifiMetaModel model)
        {
            var typeSuccess = "An error occurred during execution. Please try again in a moment!";
            // Kiểm tra tiêu đề đã tồn tại hay chưa
            var tmpNotiCommon = UnitOfWork.NotifiCommonRepo.FirstOrDefaultAsNoTracking(
                x => x.Title.Equals(model.Title) && x.SystemId == model.SystemId);

            // Tên đăng nhập hoặc Password không đúng
            if (tmpNotiCommon != null)
            {
                return Json(new { status = Result.Failed, msg = "Common notification title has already exists. Please check again!!" },
                    JsonRequestBehavior.AllowGet);
            }
            if(model.Id == 0)
            {
                var obj = new NotifiCommon();
                Mapper.Map(model, obj);
                obj.CreateDate = DateTime.Now;
                obj.UpdateDate = DateTime.Now;
                obj.Url = MyCommon.ConvertToUrlString(model.Title);
                UnitOfWork.NotifiCommonRepo.Add(obj);
                UnitOfWork.NotifiCommonRepo.Save();
                typeSuccess = "Add new notification successfully";
            }
            else
            {
                var obj = UnitOfWork.NotifiCommonRepo.FirstOrDefaultAsNoTracking(
                x => x.Id == model.Id);
                obj.UpdateDate = DateTime.Now;
                obj.Title = model.Title;
                obj.Url = MyCommon.ConvertToUrlString(model.Title);
                obj.Status = model.Status;
                obj.Description = model.Description;
                obj.SystemId = model.SystemId;
                obj.SystemName = model.SystemName;
                UnitOfWork.NotifiCommonRepo.Update(obj);
                UnitOfWork.NotifiCommonRepo.Save();
                typeSuccess = "Edit notification successfully";
            }
            

            return Json(new { msgType = Result.Succeed, msg = typeSuccess },
                JsonRequestBehavior.AllowGet);
        }
    }
}