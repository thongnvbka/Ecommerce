using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Cms.Helpers;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Settings;
using Library.UnitOfWork;

namespace Cms.Controllers
{
    [Authorize]
    public class AcountingLoseController : BaseController
    {
        // GET: OrderDetailCountin
        [LogTracker(EnumAction.View, EnumPage.AcountingLose)]
        public ActionResult Index()
        {
            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.AcountingLose)]
        public async Task<ActionResult> Search(byte? status, byte? tabMode, DateTime? fromDate, DateTime? toDate,
            string keyword = "", int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            Expression<Func<AcountingLoseResult, bool>> countingQuery = x =>
                tabMode == 0 || tabMode == 1 || tabMode == 2 && x.UserId == UserState.UserId;

            Expression<Func<OrderDetailCounting, bool>> orderDetailCoutingQuery = x =>
                tabMode == 0 || tabMode == 2 || tabMode == 1 && x.UserId == UserState.UserId;

            int totalRecord;

            var items = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLose(out totalRecord,
                orderDetailCoutingQuery, countingQuery, status, fromDate, toDate, keyword, currentPage, recordPerPage);

            int allNo;
            int createdNo;
            int handleNo;

            switch (tabMode)
            {
                case 0:
                    allNo = totalRecord;

                    createdNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(
                        x => x.UserId == UserState.UserId,
                        x => true, status, fromDate, toDate, keyword);

                    handleNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(x => true,
                        x => x.UserId == UserState.UserId, status, fromDate, toDate, keyword);
                    break;
                case 1:
                    allNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(
                        x => true, x => true, status, fromDate, toDate, keyword);

                    createdNo = totalRecord;

                    handleNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(x => true,
                        x => x.UserId == UserState.UserId, status, fromDate, toDate, keyword);
                    break;
                default:
                    allNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(
                        x => true, x => true, status, fromDate, toDate, keyword);

                    createdNo = await UnitOfWork.OrderDetailCountingRepo.GetOrderCountingLoseCount(
                        x => x.UserId == UserState.UserId, x => true, status,
                        fromDate, toDate, keyword);

                    handleNo = totalRecord;
                    break;
            }

            return JsonCamelCaseResult(new {items, totalRecord, allNo, createdNo, handleNo},
                JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.View, EnumPage.AcountingLose)]
        public async Task<ActionResult> GetByOrderId(int orderId)
        {
            var items =
                await UnitOfWork.OrderDetailCountingRepo.FindAsync(x => x.IsDelete == false && x.OrderId == orderId);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.AcountingLose)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateAcountingLose(int orderDetailId,  int quantityRecived)
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var countingLose = await UnitOfWork.OrderDetailCountingRepo.SingleOrDefaultAsync(
                        x => x.IsDelete == false && x.Id == orderDetailId &&
                             x.Mode == (byte) OrderDetailCountingMode.Wrong);

                    if (countingLose == null)
                    {
                        return JsonCamelCaseResult(new { Status = -1, Text = "Wrong counting handling note does not exist" },
                            JsonRequestBehavior.AllowGet);
                    }

                    var orderDetail = await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.Id == countingLose.OrderDetailId);

                    if (orderDetail == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Product link does not exist in the order or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    if (orderDetail.QuantityIncorrect == null)
                        return JsonCamelCaseResult(
                                new { Status = -1, Text = "This product link does not have wrong counting problem" },
                                JsonRequestBehavior.AllowGet);

                    if (quantityRecived > countingLose.QuantityLose)
                        return JsonCamelCaseResult(new { Status = -1, Text = "The number of receiving customers can not be larger than wrong product number" },
                            JsonRequestBehavior.AllowGet);

                    if (quantityRecived == countingLose.QuantityLose)
                        countingLose.IsDelete = true;

                    orderDetail.QuantityIncorrect -= quantityRecived;
                    orderDetail.QuantityActuallyReceived += quantityRecived;

                    await UnitOfWork.OrderDetailCountingRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            
            return JsonCamelCaseResult(new {Status = -1, Text = "Updated successfully"},
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateCommentNo(int orderId)
        {
            int rs;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var items =
                        await UnitOfWork.OrderDetailCountingRepo.FindAsync(
                            x => x.IsDelete == false && x.OrderId == orderId);

                    foreach (var i in items)
                        i.CommentNo += 1;

                    rs = await UnitOfWork.OrderDetailCountingRepo.SaveAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return
                JsonCamelCaseResult(rs <= 0
                    ? new {Status = -3, Text = "Updating number of comments is unsuccessful"}
                    : new {Status = 1, Text = "Updated successfully"}, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateStatus(int orderId, byte status, string note)
        {
            if (UserState.OfficeType != (byte) OfficeType.Order)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "Only ordering staf has permission to perform this action "},
                    JsonRequestBehavior.AllowGet);

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && x.IsDelete == false);

            if (order == null)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "Orders Does not exist or has been deleted"},
                    JsonRequestBehavior.AllowGet);

            if (UserState.Type == 0 && order.UserId != UserState.UserId)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "You do not have permission to perform actions with this order"},
                    JsonRequestBehavior.AllowGet);

            var items =
                await UnitOfWork.OrderDetailCountingRepo.FindAsync(x => x.IsDelete == false && x.OrderId == orderId);

            if (!items.Any())
                return
                    JsonCamelCaseResult(
                        new {Status = -1, Text = "Wrong-counting handling request does not exist or has been deleted"},
                        JsonRequestBehavior.AllowGet);

            if (status != 0 && note.Trim() == "")
                return JsonCamelCaseResult(
                    new {Status = -2, Text = "To settle handling method is compulsory" },
                    JsonRequestBehavior.AllowGet);

            int rs;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var i in items)
                    {
                        i.Status = status;
                        i.Note = note;
                        i.Updated = DateTime.Now;
                    }

                    rs = await UnitOfWork.OrderDetailCountingRepo.SaveAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            if (rs <= 0)
                return JsonCamelCaseResult(new {Status = -3, Text = "Updating status is unsuccessful"},
                    JsonRequestBehavior.AllowGet);

            // Thông báo tới đặt hàng
            if (UserState.UserId != order.UserId && order.UserId.HasValue)
                NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                    $"{UserState.FullName} edit information of wrong-counting order", EnumNotifyType.Warning,
                    $"{UserState.FullName} edit information of wrong-counting order" +
                    $" of the order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                    $"{order.Code}_Update", Url.Action("Index", "AcountingLose"));

            // Thông báo tới nhân viên kho
            foreach (var i in items)
                if (UserState.UserId != i.UserId && i.UserId.HasValue)
                    NotifyHelper.CreateAndSendNotifySystemToClient(i.UserId.Value,
                        $"{UserState.FullName} edit information of wrong-counting order", EnumNotifyType.Warning,
                        $"{UserState.FullName} edit information of wrong-counting order" +
                        $" of the order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                        $"{order.Code}_Update", Url.Action("Index", "AcountingLose"));

            // Thông báo tới CSKH
            if (order.CustomerCareUserId.HasValue && order.CustomerCareUserId != UserState.UserId)
            {
                // Thông báo tới chăm sóc khác hàng
                NotifyHelper.CreateAndSendNotifySystemToClient(order.CustomerCareUserId.Value,
                    $"{UserState.FullName} edit information of wrong-counting order", EnumNotifyType.Warning,
                    $"{UserState.FullName} edit information of wrong-counting order" +
                    $" of the order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                    $"{order.Code}_Update", Url.Action("Index", "AcountingLose"));
            }
            else if (order.CustomerCareUserId == null)
            {
                // Thông báo đến trưởng phòng đặt hàng
                var users = UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                    position => position.IsDefault && position.Type == 1,
                    office => office.Type == (byte) OfficeType.Order);

                foreach (var u in users)
                {
                    if (u.Id == UserState.UserId)
                        continue;

                    NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
                        $"{UserState.FullName} edit information of wrong-counting order", EnumNotifyType.Warning,
                        $"{UserState.FullName} edit information of wrong-counting order" +
                        $" of the order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                        $"{order.Code}_Update", Url.Action("Index", "AcountingLose"));
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Updated successfully"},
                JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Lấy dữ liệu system đổ lên searchData
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult GetRenderSystem()
        //{
        //    var listStatus = new List<dynamic>() { new { Text = "- Tất cả -", Value = -1 } };
        //    foreach (CountingStatus item in Enum.GetValues(typeof(CountingStatus)))
        //    {
        //        var txtStatus = ((CountingStatus)(byte)item).GetAttributeOfType<System.ComponentModel.DescriptionAttribute>().Description;
        //        listStatus.Add(new { Text = txtStatus, Value = (byte)item });
        //    }

        //    return Json(new { listStatus }, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Lấy danh sách khách hàng Official
        ///// POST: /OrderDetailCounting/GetListData
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<JsonResult> GetListData(int page, int pageSize, CountingSearchModel searchModal)
        //{
        //    List<OrderDetailCounting> countingModal;
        //    long totalRecord;
        //    if (searchModal == null)
        //    {
        //        searchModal = new CountingSearchModel();
        //    }
        //    searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

        //    if (!string.IsNullOrEmpty(searchModal.DateStart))
        //    {
        //        var dateStart = DateTime.Parse(searchModal.DateStart);
        //        var dateEnd = DateTime.Parse(searchModal.DateEnd);

        //        countingModal = await UnitOfWork.OrderDetailCountingRepo.FindAsync(
        //            out totalRecord,
        //            x => (x.CustomerPhone.Contains(searchModal.Keyword) || x.CustomerName.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword))
        //                 && (searchModal.Status == -1 || x.Status == searchModal.Status)
        //                 && x.Created >= dateStart && x.Created <= dateEnd,
        //            x => x.OrderByDescending(y => y.Created),
        //            page,
        //            pageSize
        //        );
        //    }
        //    else
        //    {
        //        countingModal = await UnitOfWork.OrderDetailCountingRepo.FindAsync(
        //            out totalRecord,
        //            x => (x.CustomerPhone.Contains(searchModal.Keyword) || x.CustomerName.Contains(searchModal.Keyword) || x.OrderCode.Contains(searchModal.Keyword))
        //                 && (searchModal.Status == -1 || x.Status == searchModal.Status),
        //            x => x.OrderByDescending(y => y.Created),
        //            page,
        //            pageSize
        //        );
        //    }

        //    return Json(new { totalRecord, countingModal }, JsonRequestBehavior.AllowGet);
        //}

        ///// <summary>
        ///// Detail counting
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult GetDetailCounting(int id)
        //{
        //    var obj = UnitOfWork.OrderDetailCountingRepo.FirstOrDefault(x => x.Id == id);
        //    if (obj == null)
        //    {
        //        obj = new OrderDetailCounting();
        //        return JsonCamelCaseResult(obj, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(obj, JsonRequestBehavior.AllowGet);
        //}
    }
}