using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;

namespace Cms.Controllers
{
    public class OrderRiskController : BaseController
    {
        // GET: OrderRicks
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderRisk(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode, bool isAllNoCodeOfLading)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = UnitOfWork.OrderRepo.GetOrderNoContractCode(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, checkExactCode);
            if (isAllNoCodeOfLading)
            {
                listOrder = UnitOfWork.OrderRepo.GetOrderNoContractCode3Day(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, checkExactCode);
            }
            //3. Tạo bản ghi lý do
            var list = UnitOfWork.OrderRepo.OrderNoCodeOfLadingOverDays(3);
            var listId = list.Select(x => x.Id).ToList();
            var listReason = UnitOfWork.OrderReasonRepo.Entities.Where(x => x.Type == (byte)OrderReasonType.NoCodeOfLading && listId.Contains(x.OrderId)).Select(x => x.OrderId);
            var listOrderReason = list.Where(x => !listReason.Contains(x.Id)).Select(item => new OrderReason()
            {
                OrderId = item.Id,
                ReasonId = (byte)OrderReasons.ReasonsNotSelected,
                Reason = EnumHelper.GetEnumDescription<OrderReasonNoCodeOfLading>((byte)OrderReasonNoCodeOfLading.ReasonsNotSelected),
                Type = (byte)OrderReasonType.NoCodeOfLading
            }).ToList();
            UnitOfWork.OrderReasonRepo.AddRange(listOrderReason);
            await UnitOfWork.OrderReasonRepo.SaveAsync();
            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderNoContractCode(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool isAllNoCodeOfLading, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = UnitOfWork.OrderRepo.GetOrderNoContractCode(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, checkExactCode);

            if (isAllNoCodeOfLading)
            {
                listOrder = UnitOfWork.OrderRepo.GetOrderNoContractCode3Day(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, checkExactCode);
            }

            //3. Tạo bản ghi lý do
            var list = UnitOfWork.OrderRepo.OrderNoCodeOfLadingOverDays(3);
            var listReason = UnitOfWork.OrderReasonRepo.Entities.Where(x => x.Type == (byte) OrderReasonType.NoCodeOfLading).Select(x=>x.OrderId);
            var listOrderReason = list.Where(x => !listReason.Contains(x.Id)).Select(item => new OrderReason()
            {
                OrderId = item.Id,
                ReasonId = (byte) OrderReasons.ReasonsNotSelected,
                Reason = EnumHelper.GetEnumDescription<OrderReasonNoCodeOfLading>((byte) OrderReasonNoCodeOfLading.ReasonsNotSelected),
                Type = (byte) OrderReasonType.NoCodeOfLading
            }).ToList();
            UnitOfWork.OrderReasonRepo.AddRange(listOrderReason);
            await UnitOfWork.OrderReasonRepo.SaveAsync();

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderAccountant(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = UnitOfWork.OrderRepo.GetOrderAccountant(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, checkExactCode);

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderNoWarehouse(int page, int pageSize, string keyword, int status, 
            int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, 
            bool isAllNotEnoughInventory, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = UnitOfWork.OrderRepo.GetOrderNoWarehouse(out totalRecord, page, pageSize, keyword, status, systemId, 
                dateStart, dateEnd, userId, customerId, UserState, checkExactCode);

            if (isAllNotEnoughInventory)
            {
                listOrder = await UnitOfWork.OrderRepo.GetOrderNoWarehouse4Day(out totalRecord, page, pageSize, keyword, status, systemId, 
                    dateStart, dateEnd, userId, customerId, UserState, checkExactCode);
            }

            //3. Tạo bản ghi lý do
            var list = UnitOfWork.OrderRepo.OrderNotEnoughInventoryOverDays(4);
            var listReason = UnitOfWork.OrderReasonRepo.Entities.Where(x => x.Type == (byte)OrderReasonType.NotEnoughInventory).Select(x => x.OrderId);
            var listOrderReason = list.Where(x => !listReason.Contains(x.Id)).Select(item => new OrderReason()
            {
                OrderId = item.Id,
                ReasonId = (byte)OrderReasons.ReasonsNotSelected,
                Reason = EnumHelper.GetEnumDescription<OrderReasonNotEnoughInventory>((byte)OrderReasonNotEnoughInventory.ReasonsNotSelected),
                Type = (byte)OrderReasonType.NotEnoughInventory
            }).ToList();
            UnitOfWork.OrderReasonRepo.AddRange(listOrderReason);
            await UnitOfWork.OrderReasonRepo.SaveAsync();

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }
    }
}