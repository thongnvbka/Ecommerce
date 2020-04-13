using AutoMapper;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.ViewModels.Counting;
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
    public class RequestShipController : BaseController
    {
        // GET: RequestShip
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Lấy dữ liệu system đổ lên searchData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetRenderSystem()
        {
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            foreach (CountingStatus item in Enum.GetValues(typeof(CountingStatus)))
            {
                var txtStatus = ((CountingStatus)(byte)item).GetAttributeOfType<System.ComponentModel.DescriptionAttribute>().Description;
                listStatus.Add(new { Text = txtStatus, Value = (byte)item });
            }

            return Json(new { listStatus }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách khách hàng Official
        /// POST: /OrderDetailCounting/GetListData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetListData(int page, int pageSize, CountingSearchModel searchModal)
        {
            List<RequestShip> requestModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new CountingSearchModel();
            }
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                requestModal = await UnitOfWork.RequestShipRepo.FindAsync(
                    out totalRecord,
                    x => (x.CustomerPhone.Contains(searchModal.Keyword) || x.CustomerName.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword) || x.Code.Contains(searchModal.Keyword))
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && x.CreateDate >= dateStart && x.CreateDate <= dateEnd,
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }
            else
            {
                requestModal = await UnitOfWork.RequestShipRepo.FindAsync(
                    out totalRecord,
                    x => (x.CustomerPhone.Contains(searchModal.Keyword) || x.CustomerName.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword))
                         && (searchModal.Status == -1 || x.Status == searchModal.Status),
                    x => x.OrderByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );
            }
            return Json(new { totalRecord, requestModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Detail request ship
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDetail(long id)
        {
            var model = UnitOfWork.RequestShipRepo.GetById(id);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// cập nhật trạng thái xử lý của yêu cầu ship
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public JsonResult CreateNew(RequestShip model)
        {
            var typeSuccess = "There was an error in the implementation process. Please try again in a moment!";
            var obj = UnitOfWork.RequestShipRepo.FirstOrDefaultAsNoTracking(
                x => x.Id == model.Id);
            
            obj.Status = model.Status;
            
            UnitOfWork.RequestShipRepo.Update(obj);
            UnitOfWork.RequestShipRepo.Save();
            typeSuccess = "Update request ship successfully";
            return Json(new { status = true, msg = typeSuccess },
                JsonRequestBehavior.AllowGet);
        }
    }
}