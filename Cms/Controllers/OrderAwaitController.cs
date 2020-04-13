using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using Library.Models;
using Newtonsoft.Json;
using Library.ViewModels.Account;
using System.Runtime.ExceptionServices;
using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;


namespace Cms.Controllers
{
    /// <summary>
    /// Nghiệp vụ báo giá Orders của phòng chăm sóc khách hàng
    /// </summary>
    public class OrderAwaitController : BaseController
    {
        #region [Lấy danh sách ]

        /// <summary>
        /// Lấy danh sách Orders chờ báo giá của chăm sóc khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [LogTracker(EnumAction.View, EnumPage.OrderWait)]
        public async Task<JsonResult> GetOrderWait(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode, bool checkRetail)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);
            int totalAwait;

            totalAwait =
                    UnitOfWork.OrderRepo.Entities.Count(
                        x =>
                            !x.IsDelete
                            && x.Type == (byte)OrderType.Order
                            && x.Status == (byte)OrderStatus.WaitPrice);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail)
                    && (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId)
                    && x.Status == (byte)OrderStatus.AreQuotes && x.Type == (byte)OrderType.Order,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.Code == keyword
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail)
                    && (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId)
                    && x.Status == (byte)OrderStatus.AreQuotes && x.Type == (byte)OrderType.Order,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            //if (listOrder.Any())
            //{
            //    var ids = listOrder.Select(x => x.Id);
            //    var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

            //    //3. Lấy thông tin chat
            //    foreach (var item in listOrder)
            //    {
            //        item.Chat = listChat.Count(x => x.OrderId == item.Id);
            //    }
            //}

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalAwait }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách Orders chờ báo giá của chăm sóc khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [LogTracker(EnumAction.View, EnumPage.OrderWaitNew)]
        public async Task<JsonResult> GetOrderWaitNew(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode, bool checkRetail)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId)
                    && (!checkRetail || x.IsRetail)
                    && x.CustomerCareUserId == null
                    && x.Status == (byte)OrderStatus.WaitPrice
                    && x.Type == (byte)OrderType.Order,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );

            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.Code == keyword
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId)
                    && (!checkRetail || x.IsRetail)
                    && x.CustomerCareUserId == null
                    && x.Status == (byte)OrderStatus.WaitPrice
                    && x.Type == (byte)OrderType.Order,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            //if (listOrder.Any())
            //{
            //    var ids = listOrder.Select(x => x.Id);
            //    var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

            //    //3. Lấy thông tin chat
            //    foreach (var item in listOrder)
            //    {
            //        item.Chat = listChat.Count(x => x.OrderId == item.Id);
            //    }
            //}

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalAwait = totalRecord }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Lấy Orders của chăm sóc khách hàng.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="keyword"></param>
        /// <param name="status"></param>
        /// <param name="systemId"></param>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <param name="userId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [LogTracker(EnumAction.View, EnumPage.OrderCustomerCare)]
        public async Task<JsonResult> GetOrderCustomerCare(int page, int pageSize, string keyword, int status, int type, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode, bool checkRetail)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (type == -1 || ((type == (byte)OrderType.Deposit || x.Type != (byte)OrderType.Deposit) && (type != (byte)OrderType.Deposit || x.Type == (byte)OrderType.Deposit)))
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail)
                   //&& (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                   //&& (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId)
                   /* && x.Type == (byte)OrderType.Order*/,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );

            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (type == -1 || ((type == (byte)OrderType.Deposit || x.Type != (byte)OrderType.Deposit) && (type != (byte)OrderType.Deposit || x.Type == (byte)OrderType.Deposit)))
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.Code == keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail)
                   //&& (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                   //&& (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId)
                   /* && x.Type == (byte)OrderType.Order*/,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            //if (listOrder.Any())
            //{
            //    var ids = listOrder.Select(x => x.Id);
            //    var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

            //    //3. Lấy thông tin chat
            //    foreach (var item in listOrder)
            //    {
            //        item.Chat = listChat.Count(x => x.OrderId == item.Id);
            //    }
            //}

            //lấy list OrderId
            var listOrderId = listOrder.Select(x => x.Id).ToList();
            //Lấy danh sách Khiếu nại
            var listComplain = UnitOfWork.ComplainRepo.Entities.Where(x => listOrderId.Contains(x.OrderId) && x.IsDelete == false).GroupBy(p => p.OrderId,
                               p => p.Id,
                               (key, g) => new
                               {
                                   OrderId = key,
                                   Count = g.ToList().Count()
                               }).ToList();

            var listClaim = UnitOfWork.ClaimForRefundRepo.Entities.Where(x => listOrderId.Contains(x.OrderId) && x.IsDelete == false && x.Status == (byte) ClaimForRefundStatus.Success).GroupBy(p => p.OrderId,
                               p => p.RealTotalRefund,
                               (key, g) => new
                               {
                                   OrderId = key,
                                   RealTotal = g.Sum() ?? 0
                               }).ToList();

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, listComplain, listClaim }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.View, EnumPage.OrderCustomerCare)]
        public async Task<ActionResult> ExcelGetOrderCustomerCare(string keyword, int status, int type, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode, bool checkRetail)
        {
            var page = 1;
            var pageSize = Int32.MaxValue;
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (type == -1 || ((type == (byte)OrderType.Deposit || x.Type != (byte)OrderType.Deposit) && (type != (byte)OrderType.Deposit || x.Type == (byte)OrderType.Deposit)))
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail),
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );

            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (type == -1 || ((type == (byte)OrderType.Deposit || x.Type != (byte)OrderType.Deposit) && (type != (byte)OrderType.Deposit || x.Type == (byte)OrderType.Deposit)))
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.Code == keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.CustomerCareUserId == userId)
                    && (!checkRetail || x.IsRetail),
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            //lấy list OrderId
            var listOrderId = listOrder.Select(x => x.Id).ToList();
            //Lấy danh sách Khiếu nại
            var listComplain = UnitOfWork.ComplainRepo.Entities.Where(x => listOrderId.Contains(x.OrderId) && x.IsDelete == false).GroupBy(p => p.OrderId,
                               p => p.Id,
                               (key, g) => new
                               {
                                   OrderId = key,
                                   Count = g.ToList().Count()
                               }).ToList();

            var listClaim = UnitOfWork.ClaimForRefundRepo.Entities.Where(x => listOrderId.Contains(x.OrderId) && x.IsDelete == false && x.Status == (byte)ClaimForRefundStatus.Success).GroupBy(p => p.OrderId,
                               p => p.RealTotalRefund,
                               (key, g) => new
                               {
                                   OrderId = key,
                                   RealTotal = g.Sum() ?? 0
                               }).ToList();

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "VALUE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "COMPLAINT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "COMPENSATION AMOUNT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++,"STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "SYSTEM", ExcelHorizontalAlignment.Center, true, colorHeader);
                

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "DANH SÁCH package", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                //var start = GetStartOfDay(fromDate ?? DateTime.Now);
                //var end = GetEndOfDay(toDate ?? DateTime.Now);

                string ngay;
                if (dateStart == null || dateEnd == null)
                {
                    ngay = "";
                }
                else
                {
                    ngay = "Từ: " + dateStart.Value.ToShortDateString() + " đến " + dateEnd.Value.ToShortDateString();
                }

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;
                var style = new CustomExcelStyle
                {
                    IsMerge = false,
                    IsBold = false,
                    Border = ExcelBorderStyle.Thin,
                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                    NumberFormat = "#,##0"
                };
                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {

                        col = 1;
                        var count = listComplain.Find(s => s.OrderId == order.Id) == null ? 0 : listComplain.Find(s => s.OrderId == order.Id).Count;
                        var claim = listClaim.Find(s => s.OrderId == order.Id) == null ? 0 : listClaim.Find(s => s.OrderId == order.Id).RealTotal;

                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++,order.Code, ExcelHorizontalAlignment.Left, true);
                        if(order.Type  == (byte)OrderType.Deposit)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, EnumHelper.GetEnumDescription<DepositStatus>(order.Status), ExcelHorizontalAlignment.Right, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Right, true);
                        }
                        
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerCareFullName, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, no, col, order.Total, style);
                        ExcelHelper.CreateCellTable(sheet, no, col++, count, ExcelHorizontalAlignment.Right, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, no, col, claim, style);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.UserFullName, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.SystemName, ExcelHorizontalAlignment.Center, true);

                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"DanhSachDon_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        #endregion

        #region[Xử lý Orders báo giá]

        //Cancel Orders của nhân viên chăm sóc khách hàng
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderWaitNew, EnumPage.OrderWait, EnumPage.OrderCustomerCare)]
        public async Task<JsonResult> OrderCancelCustomerCase(int id, byte type, string note, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Type == type && x.Status < (byte)OrderStatus.WaitOrder && x.Status == status);

            //2. Kiểm tra Orders
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist, having status modified or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.ReasonCancel = note;

                    //Kiểm tra Orders đã có người nhận xử lý chưa
                    if (order.CustomerCareUserId == null)
                    {
                        order.CustomerCareUserId = UserState.UserId;
                        order.CustomerCareName = UserState.UserName;
                        order.CustomerCareFullName = UserState.FullName;
                        order.CustomerCareOfficeId = UserState.OfficeId;
                        order.CustomerCareOfficeName = UserState.OfficeName;
                        order.CustomerCareOfficeIdPath = UserState.OfficeIdPath;
                    }

                    order.LastUpdate = timeNow;
                    order.Status = (byte)OrderStatus.Cancel;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Hủy link hàng
                    var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == order.Id);
                    foreach (var orderDetail in listOrderDetail)
                    {
                        orderDetail.LastUpdate = timeNow;
                        orderDetail.QuantityBooked = 0;
                        orderDetail.TotalPrice = 0;
                        orderDetail.TotalExchange = 0;
                        orderDetail.Status = (byte)OrderDetailStatus.Cancel;
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Order cancelled due to: " + note,
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
                        OrderId = order.Id,
                        Status = order.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = order.Type
                    });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = "Order cancelled due to: " + note,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = MsgType.Success, msg = "Order cancelled successfully!" }, JsonRequestBehavior.AllowGet);
        }


        //Trưởng phòng phân Orders báo giá cho nhân viên phòng mình
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderWaitNew, EnumPage.OrderWait)]
        public async Task<JsonResult> AssignedOrderCustomerCase(int orderId, byte orderType, UserOfficeResult user, byte status)
        {
            //1. Khởi tạo dữ liệu
            DateTime timeNow;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete && x.Status == status);

            //2. Kiểm tra điều kiện
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "There is no order left to accept!" }, JsonRequestBehavior.AllowGet);
            }
            timeNow = DateTime.Now;

            if (order.CustomerCareUserId != null)
            {
                return Json(new { status = MsgType.Error, msg = "Order has been processed by another staff!" }, JsonRequestBehavior.AllowGet);
            }

            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.CustomerCareUserId == user.Id && x.Status == (byte)OrderStatus.AreQuotes && !x.IsDelete);
            if (countOrder >= 15) // vượt quá Orders number nhân viên đang xử lý
            {
                return Json(new { status = MsgType.Error, msg = "More than the maximum number of orders a staff may handle!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Kiểm tra Orders đã có người nhận xử lý chưa
                    if (order.CustomerCareUserId == null)
                    {
                        order.CustomerCareUserId = user.Id;
                        order.CustomerCareName = UserState.UserName;
                        order.CustomerCareFullName = user.FullName;
                        order.CustomerCareOfficeId = user.OfficeId;
                        order.CustomerCareOfficeName = user.OfficeName;
                        order.CustomerCareOfficeIdPath = user.OfficeIdPath;
                    }

                    order.LastUpdate = timeNow;

                    if (order.Type != (byte)OrderType.Deposit)
                    {
                        if (order.Status == (byte)OrderStatus.WaitPrice)
                        {
                            order.Status = (byte)OrderStatus.AreQuotes;
                        }
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    if (customer != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"Assign the order to staff <b>{user.FullName} - {user.TitleName}<b> to handle",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Assign the order to staff <b>{user.FullName} - {user.TitleName}<b> to handle",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = MsgType.Success, msg = "Assigning order hanlding to staff successfully!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Nhân viên nhận 5 Orders về xử lý cùng lúc
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderWaitNew, EnumPage.OrderWait)]
        public async Task<JsonResult> ReceivePurchaseMultiOrderCustomerCase()
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var listOrderCode = new List<string>();
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(x => x.CustomerCareUserId == null && (x.Status == (byte)OrderStatus.WaitPrice) && !x.IsDelete && x.Type == (byte)OrderType.Order);
            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.CustomerCareUserId == UserState.UserId && x.Status == (byte)OrderStatus.AreQuotes && !x.IsDelete && x.Type == (byte)OrderType.Order);
            var count = (10 - countOrder) > 2 ? 2 : (10 - countOrder); //Lấy Orders number có thể nhận

            //2. check điều kiện
            if (listOrder.Count == 0)
            {
                return Json(new { status = MsgType.Error, msg = "There is no order left to accept!" }, JsonRequestBehavior.AllowGet);
            }

            if (countOrder >= 10)
            {
                return Json(new { status = MsgType.Error, msg = "More than the number of orders being processed!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var i = 1;
                    foreach (var order in listOrder)
                    {
                        if (i > count) break;

                        //Kiểm tra Orders đã có người nhận xử lý chưa
                        if (order.CustomerCareUserId == null)
                        {
                            order.CustomerCareUserId = UserState.UserId;
                            order.CustomerCareName = UserState.UserName;
                            order.CustomerCareFullName = UserState.FullName;
                            order.CustomerCareOfficeId = UserState.OfficeId;
                            order.CustomerCareOfficeName = UserState.OfficeName;
                            order.CustomerCareOfficeIdPath = UserState.OfficeIdPath;
                        }

                        order.LastUpdate = timeNow;
                        order.Status = order.Status == (byte)OrderStatus.WaitPrice ? (byte)OrderStatus.AreQuotes : (byte)OrderStatus.Order;
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = "Accept order quote",
                            CustomerId = order.CustomerId.Value,
                            CustomerName = order.CustomerName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });

                        listOrderCode.Add(order.Code);
                        await UnitOfWork.OrderHistoryRepo.SaveAsync();

                        //Ghi log thao tác
                        var orderLog = new OrderLog
                        {
                            OrderId = order.Id,
                            CreateDate = timeNow,
                            Type = (byte)OrderLogType.Acction,
                            DataBefore = null,
                            DataAfter = null,
                            Content = "Accept order quote",
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            UserOfficeId = UserState.OfficeId,
                            UserOfficeName = UserState.OfficeName,
                            UserOfficePath = UserState.OfficeIdPath
                        };
                        UnitOfWork.OrderLogRepo.Add(orderLog);
                        await UnitOfWork.OrderLogRepo.SaveAsync();

                        i++;
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Order accepted successfully!", listOrderCode }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Chuyển đơn hàng cho nhân viên khác xử lý
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait)]
        public async Task<JsonResult> OrderReplyCustomerCase(int orderId, UserOfficeResult user)
        {
            //1. Lấy thông tin Orders
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.CustomerCareUserId == user.Id && x.Status == (byte)OrderStatus.AreQuotes && !x.IsDelete);

            //2. Check điều kiện
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.CustomerCareUserId == user.Id)
            {
                return Json(new { status = MsgType.Error, msg = $"Staff {user.FullName} is handling this order!" }, JsonRequestBehavior.AllowGet);
            }

            if (countOrder >= 15)// vượt quá Orders number nhân viên đang xử lý
            {
                return Json(new { status = MsgType.Error, msg = $"More than the maximum number of orders {user.FullName} is handling!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.CustomerCareUserId = user.Id;
                    order.CustomerCareName = UserState.UserName;
                    order.CustomerCareFullName = user.FullName;
                    order.CustomerCareOfficeId = user.OfficeId;
                    order.CustomerCareOfficeName = user.OfficeName;
                    order.CustomerCareOfficeIdPath = user.OfficeIdPath;
                    order.LastUpdate = timeNow;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = $"Change staff <b>{user.FullName} - {user.TitleName}<b> to handle",
                        CustomerId = order.CustomerId.Value,
                        CustomerName = order.CustomerName,
                        OrderId = order.Id,
                        Status = order.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = order.Type
                    });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Change staff <b>{user.FullName} - {user.TitleName}<b> to handle",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = $"Assigning order to staff {user.FullName} to handle successfully!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật phí ship cho Orders
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait)]
        public async Task<JsonResult> UpdateFeeShip(int id, decimal value)
        {
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
            var orderShopShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.ShopShipping);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    if (orderShopShippingService == null)
                    {
                        orderShopShippingService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.ShopShipping,
                            ServiceName = (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = value,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = value * order.ExchangeRate,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Note = $"Shop shipping fee is {value.ToString("N4", CultureInfo)} CNY equaling to  {(value * order.ExchangeRate).ToString("N2", CultureInfo)} Baht"
                        };
                        UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }
                    else
                    {
                        orderShopShippingService.Value = value;
                        orderShopShippingService.TotalPrice = value * order.ExchangeRate;
                        orderShopShippingService.Checked = true;
                        orderShopShippingService.LastUpdate = timeNow;
                        orderShopShippingService.Note = $"Shop shipping fee is {value.ToString("N4", CultureInfo)} CNY equaling to  {(value * order.ExchangeRate).ToString("N2", CultureInfo)} Baht";
                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    if (DataCompare(order.FeeShip, value))
                    {
                        dataBefore.Add(new LogResult()
                        {
                            Name = "China internal shipping fee to quote customer",
                            Value = order.FeeShip == null ? "0" : order.FeeShip.Value.ToString("N2")
                        });
                        order.FeeShip = value;
                        dataAfter.Add(new LogResult()
                        {
                            Name = "China internal shipping fee to quote customer",
                            Value = order.FeeShip.Value.ToString("N2")
                        });
                    }

                    // Cập nhật số lượng Sum
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);
                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Order infomation edit",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { listOrderService, status = MsgType.Success, msg = "Servive fee update completed!" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region [Gửi báo giá]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> OrderWait(int id, byte type)
        {
            var timeNow = DateTime.Now;

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == (byte)OrderStatus.AreQuotes);

            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Đơn hàng không tồn tại, chuyển trạng thái hoặc bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            //if (order.ShopId == 0 || order.ShopId == null)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Orders chưa shop name" }, JsonRequestBehavior.AllowGet);
            //}

            var countOrderDetail = await UnitOfWork.OrderDetailRepo.CountAsync(x => !x.IsDelete && x.OrderId == order.Id && x.Status == (byte)OrderDetailStatus.WaitPrice);
            if (countOrderDetail > 0)
            {
                return Json(new { status = MsgType.Error, msg = "Certain product link has not confirm price quote!" }, JsonRequestBehavior.AllowGet);
            }

            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.Status != (byte)OrderDetailStatus.Cancel);
            if (listOrderDetail.Count(x => x.ShopName == null) > 0)
            {
                return Json(new { status = MsgType.Error, msg = "There remains order detail that has not had shop name" }, JsonRequestBehavior.AllowGet);
            }

            if (listOrderDetail.Select(x => x.ShopName).Distinct().Count() > 1)
            {
                return Json(new { status = MsgType.Error, msg = "Order separation has not been performed" }, JsonRequestBehavior.AllowGet);
            }

            //if (listOrderDetail.Count(x => x.ShopId != order.ShopId) > 0)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Detail Orders có shop khác với shop của Orders" }, JsonRequestBehavior.AllowGet);
            //}

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.Type = type;
                    order.Status = (byte)OrderStatus.WaitDeposit;
                    order.LastUpdate = timeNow;


                    var detail = listOrderDetail.FirstOrDefault();
                    if (detail != null)
                    {
                        order.WebsiteName = detail.WebsiteName;
                        order.ShopId = detail.ShopId;
                        order.ShopName = detail.ShopName?.Trim() ?? "";
                        order.ShopLink = detail.ShopLink?.Trim() ?? "";

                        if (string.IsNullOrEmpty(order.WebsiteName))
                        {
                            order.WebsiteName = MyCommon.GetDomain(listOrderDetail.FirstOrDefault().Link);
                        }
                    }
                    else
                    {
                        return Json(new { status = MsgType.Error, msg = "Order does not have any order details" }, JsonRequestBehavior.AllowGet);
                    }


                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Resend quote price to customer",
                        CustomerId = order.CustomerId.Value,
                        CustomerName = order.CustomerName,
                        OrderId = order.Id,
                        Status = order.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = order.Type
                    });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Orders
                    var totalExchange = order.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                    // Orders nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (order.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //var orderServcie = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //        x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);

                    #region  Thêm các dịch vụ cho đơn hàng

                    //// DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                    //if (orderServcie == null)
                    //{
                    //    orderServcie = new OrderService()
                    //    {
                    //        OrderId = order.Id,
                    //        ServiceId = (byte)OrderServices.Order,
                    //        ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                    //        ExchangeRate = order.ExchangeRate,
                    //        IsDelete = false,
                    //        Created = timeNow,
                    //        LastUpdate = timeNow,
                    //        HashTag = string.Empty,
                    //        Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange),
                    //        Currency = Currency.VND.ToString(),
                    //        Type = (byte)UnitType.Percent,
                    //        TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice,
                    //        Mode = (byte)OrderServiceMode.Required,
                    //        Checked = true
                    //    };

                    //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                    //}
                    //else // Cập nhật dịch vụ mua hàng
                    //{
                    //    orderServcie.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                    //    orderServcie.TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice;
                    //}

                    //// Triết khấu phí mua hàng
                    //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
                    //if (discount > 0)
                    //{
                    //    orderServcie.TotalPrice -= orderServcie.TotalPrice * discount / 100;
                    //    orderServcie.Note =
                    //        $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                    //}

                    // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                    var autditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.Audit);

                    if (autditService == null)
                    {
                        autditService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Audit,
                            ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = false,
                            Created = timeNow,
                            LastUpdate = timeNow
                        };
                        UnitOfWork.OrderServiceRepo.Add(autditService);
                    }
                    else
                    {
                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == order.Id
                                    && x.Status == (byte)OrderDetailStatus.Order && x.AuditPrice != null)
                                .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        autditService.Value = totalAuditPrice;
                        autditService.TotalPrice = totalAuditPrice;
                    }

                    // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                    var packingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.Packing);

                    if (packingService == null)
                    {
                        packingService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Packing,
                            ServiceName = OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = false,
                            Created = timeNow,
                            LastUpdate = timeNow
                        };
                        UnitOfWork.OrderServiceRepo.Add(packingService);
                    }
                    // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                    var outSideShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.OutSideShipping);

                    if (outSideShippingService == null)
                    {
                        outSideShippingService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.OutSideShipping,
                            ServiceName = OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow
                        };
                        UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                    }

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                    var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.InSideShipping);

                    if (shipToHomeService == null)
                    {
                        shipToHomeService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.InSideShipping,
                            ServiceName = OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow
                        };
                        UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                    }

                    #endregion

                    // Submit thêm OrderService
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật số lượng Sum
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                                x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = "Resend quote price to customer",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Quote price sending has been completed successfully!" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> UpdateShop(int id, int shopId)
        {
            var timeNow = DateTime.Now;

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var shop = await UnitOfWork.ShopRepo.FirstOrDefaultAsync(x => x.Id == shopId && !x.IsDelete);
            if (shop == null)
            {
                return Json(new { status = MsgType.Error, msg = "Shop does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    order.ShopId = shop.Id;

                    if (DataCompare(order.ShopName, shop.Name))
                    {
                        dataBefore.Add(new LogResult() {Name = "Shop", Value = order.ShopName});
                        order.ShopName = shop.Name?.Trim() ?? "";
                        dataAfter.Add(new LogResult() {Name = "Shop", Value = order.ShopName});
                    }

                    order.ShopLink = shop.Url;

                    if (DataCompare(order.WebsiteName, shop.Website))
                    {
                        dataBefore.Add(new LogResult() {Name = "Website", Value = order.WebsiteName});
                        order.WebsiteName = shop.Website;
                        dataAfter.Add(new LogResult() {Name = "Website", Value = order.WebsiteName});
                    }

                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit shop's information",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Shop information editing completed!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait)]
        public async Task<JsonResult> SeparationOrder(int id)
        {
            var timeNow = DateTime.Now;

            var listOrderView = new List<dynamic>();

            var orderOld = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (orderOld == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var countOrderDetail = await UnitOfWork.OrderDetailRepo.CountAsync(x => !x.IsDelete && x.OrderId == orderOld.Id && x.Status == (byte)OrderDetailStatus.WaitPrice);
            if (countOrderDetail > 0)
            {
                return Json(new { status = MsgType.Error, msg = "Certain product link has not confirmed price quote!" }, JsonRequestBehavior.AllowGet);
            }

            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => !x.IsDelete && x.OrderId == orderOld.Id && x.Status != (byte)OrderDetailStatus.Cancel);
            if (listOrderDetail.Count(x => x.ShopId == 0) > 0)
            {
                return Json(new { status = MsgType.Error, msg = "There are order details that have not selected shop(s)" }, JsonRequestBehavior.AllowGet);
            }

            if (listOrderDetail.Count() <= 1)
            {
                return Json(new { status = MsgType.Error, msg = "Order has only one link and cannot be seperated" }, JsonRequestBehavior.AllowGet);
            }

            if (listOrderDetail.GroupBy(x => x.ShopName.Trim()).Count() <= 1)
            {
                return Json(new { status = MsgType.Error, msg = "Links from one shop cannot be seperated" }, JsonRequestBehavior.AllowGet);
            }
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    //Thêm mới Orders
                    foreach (var listDetail in listOrderDetail.GroupBy(x => x.ShopId))
                    {
                        var firstOrDefault = listDetail.FirstOrDefault();
                        if (firstOrDefault == null) continue;
                        var order = new Order()
                        {
                            Code = string.Empty,
                            Type = (byte)OrderType.Order,
                            WebsiteName = firstOrDefault.WebsiteName,
                            ShopId = firstOrDefault.ShopId,
                            ShopName = firstOrDefault.ShopName?.Trim() ?? "",
                            ShopLink = firstOrDefault.ShopLink,
                            PackageNo = 0,
                            ContractCode = string.Empty,
                            ContractCodes = string.Empty,
                            LevelId = orderOld.LevelId,
                            LevelName = orderOld.LevelName,
                            CreatedTool = (byte)CreatedTool.Auto,
                            Currency = Currency.CNY.ToString(),
                            ExchangeRate = orderOld.ExchangeRate,
                            WarehouseId = orderOld.WarehouseId,
                            WarehouseName = orderOld.WarehouseName,
                            WarehouseDeliveryId = orderOld.WarehouseDeliveryId,
                            WarehouseDeliveryName = orderOld.WarehouseDeliveryName,
                            CustomerId = orderOld.CustomerId,
                            CustomerName = orderOld.CustomerName,
                            CustomerEmail = orderOld.CustomerEmail,
                            CustomerPhone = orderOld.CustomerPhone,
                            Status = orderOld.Status,
                            OrderInfoId = orderOld.OrderInfoId,
                            FromAddressId = orderOld.FromAddressId,
                            ToAddressId = orderOld.ToAddressId,
                            SystemId = orderOld.SystemId,
                            SystemName = orderOld.SystemName,
                            ServiceType = orderOld.ServiceType,
                            Note = orderOld.Note,
                            PrivateNote = orderOld.PrivateNote,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            CustomerCareUserId = orderOld.CustomerCareUserId,
                            CustomerCareName = orderOld.CustomerCareName,
                            CustomerCareFullName = orderOld.CustomerCareFullName,
                            CustomerCareOfficeId = orderOld.CustomerCareOfficeId,
                            CustomerCareOfficeName = orderOld.CustomerCareOfficeName,
                            CustomerCareOfficeIdPath = orderOld.CustomerCareOfficeIdPath,
                            FeeShip = orderOld.FeeShip
                        };

                        UnitOfWork.OrderRepo.Add(order);

                        // Submit thêm order
                        await UnitOfWork.OrderRepo.SaveAsync();

                        //Edit lại Detail Orders theo order mới
                        foreach (var detail in listDetail)
                        {
                            if (string.IsNullOrEmpty(order.WebsiteName))
                            {
                                if (!CheckWebsite(detail.Link))
                                {
                                    order.WebsiteName = MyCommon.GetDomain(detail.Link);
                                }
                            }


                            var orderDetai = new OrderDetail()
                            {
                                OrderId = order.Id,
                                Name = detail.Name,
                                Image = detail.Image,
                                Quantity = detail.Quantity,
                                BeginAmount = detail.BeginAmount,
                                Price = detail.Price,
                                AuditPrice = detail.AuditPrice,
                                ExchangeRate = detail.ExchangeRate,
                                ExchangePrice = detail.ExchangePrice,
                                TotalPrice = detail.TotalPrice,
                                TotalExchange = detail.TotalExchange,
                                Note = detail.Note,
                                Status = detail.Status,
                                Link = detail.Link,
                                QuantityBooked = detail.QuantityBooked,
                                Properties = detail.Properties,
                                HashTag = detail.HashTag,
                                CategoryId = detail.CategoryId,
                                CategoryName = detail.CategoryName,
                                Created = timeNow,
                                LastUpdate = timeNow,
                                IsDelete = detail.IsDelete,
                                UniqueCode = detail.UniqueCode,
                                Size = detail.Size,
                                Color = detail.Color,
                                UserNote = detail.UserNote,
                                QuantityActuallyReceived = detail.QuantityActuallyReceived,
                                CountingTime = detail.CountingTime,
                                CountingUserId = detail.CountingUserId,
                                CountingUserName = detail.CountingUserName,
                                CountingFullName = detail.CountingFullName,
                                CountingOfficeId = detail.CountingOfficeId,
                                CountingOfficeName = detail.CountingOfficeName,
                                CountingOfficeIdPath = detail.CountingOfficeIdPath,
                                Min = detail.Min,
                                Max = detail.Max,
                                Prices = detail.Prices,
                                ProId = detail.ProId,
                                SkullId = detail.SkullId,
                                WebsiteName = detail.WebsiteName,
                                ShopId = detail.ShopId,
                                ShopName = detail.ShopName?.Trim() ?? "",
                                ShopLink = detail.ShopLink
                            };
                            UnitOfWork.OrderDetailRepo.Add(orderDetai);
                        }
                        await UnitOfWork.OrderDetailRepo.SaveAsync();

                        //Thêm các phần dịch vụ

                        // Cập nhật lại Mã cho Orders
                        var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == order.CustomerId && x.Id <= order.Id);
                        var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId);
                        order.Code = $"{customer.Code}-{orderNo}";

                        order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalExchange);
                        order.UnsignName = MyCommon.Ucs2Convert(
                            $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone}");

                        order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalPrice);

                        order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                            .CountAsync(x => x.OrderId == order.Id && !x.IsDelete);

                        order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.QuantityBooked.Value);

                        if (string.IsNullOrEmpty(order.WebsiteName))
                        {
                            order.WebsiteName = MyCommon.GetDomain(listDetail.FirstOrDefault().Link);
                        }

                        // Submit cập nhật order
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Orders
                        var totalExchange = order.TotalExchange;
                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                        // Orders nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        if (order.TotalExchange < 2000000)
                        {
                            totalPrice = 150000;
                        }

                        //var orderServcie = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                        //        x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);

                        #region  Thêm các dịch vụ cho đơn hàng

                        //// DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                        //if (orderServcie == null)
                        //{
                        //    orderServcie = new OrderService()
                        //    {
                        //        OrderId = order.Id,
                        //        ServiceId = (byte)OrderServices.Order,
                        //        ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                        //        ExchangeRate = order.ExchangeRate,
                        //        IsDelete = false,
                        //        Created = timeNow,
                        //        LastUpdate = timeNow,
                        //        HashTag = string.Empty,
                        //        Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange),
                        //        Currency = Currency.VND.ToString(),
                        //        Type = (byte)UnitType.Percent,
                        //        TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice,
                        //        Mode = (byte)OrderServiceMode.Required,
                        //        Checked = true
                        //    };

                        //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                        //}
                        //else // Cập nhật dịch vụ mua hàng
                        //{
                        //    orderServcie.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //    orderServcie.TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice;
                        //}

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    orderServcie.TotalPrice -= orderServcie.TotalPrice * discount / 100;
                        //    orderServcie.Note =
                        //        $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
                        var orderShopShippingService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x =>
                                    x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.ShopShipping);

                        var orderShopShippingServiceOld = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x =>
                                    x.OrderId == orderOld.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.ShopShipping);

                        if (orderShopShippingService == null)
                        {
                            orderShopShippingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.ShopShipping,
                                ServiceName =
                                    (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = orderShopShippingServiceOld.Value,
                                Currency = Currency.CNY.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = orderShopShippingServiceOld.Value * order.ExchangeRate,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = orderShopShippingServiceOld.Checked,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                        }

                        // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                        var totalAuditPrice = listDetail
                               .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                           && x.Status == (byte)OrderDetailStatus.Order)
                               .Sum(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        var autditService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Audit);

                        var autditServiceOld = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                               x => x.OrderId == orderOld.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.Audit);

                        if (autditService == null)
                        {
                            autditService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Audit,
                                ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = totalAuditPrice,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = totalAuditPrice,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = autditServiceOld.Checked,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(autditService);
                        }
                        else
                        {
                            autditService.Value = totalAuditPrice;
                            autditService.TotalPrice = totalAuditPrice;
                        }

                        // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                        var packingService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Packing);

                        var packingServiceOld = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == orderOld.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Packing);

                        if (packingService == null)
                        {
                            packingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Packing,
                                ServiceName =
                                    OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = 0,
                                Currency = Currency.CNY.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = packingServiceOld.Checked,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(packingService);
                        }
                        // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                        var outSideShippingService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.OutSideShipping);

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                        }

                        //// DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                        //var fastDeliveryService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => x.OrderId == order.Id && !x.IsDelete &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery);

                        //var fastDeliveryServiceOld = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //       x => x.OrderId == orderOld.Id && !x.IsDelete &&
                        //            x.ServiceId == (byte)OrderServices.FastDelivery);

                        //if (fastDeliveryService == null)
                        //{
                        //    fastDeliveryService = new OrderService()
                        //    {
                        //        OrderId = order.Id,
                        //        ServiceId = (byte)OrderServices.FastDelivery,
                        //        ServiceName =
                        //            OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
                        //        ExchangeRate = order.ExchangeRate,
                        //        Value = 0,
                        //        Currency = Currency.VND.ToString(),
                        //        Type = (byte)UnitType.Money,
                        //        TotalPrice = 0,
                        //        Mode = (byte)OrderServiceMode.Option,
                        //        Checked = fastDeliveryServiceOld.Checked,
                        //        Created = timeNow,
                        //        LastUpdate = timeNow
                        //    };
                        //    UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);
                        //}

                        // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                        var shipToHomeService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.InSideShipping);

                        if (shipToHomeService == null)
                        {
                            shipToHomeService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.InSideShipping,
                                ServiceName =
                                    OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                        }


                        // PHÍ DỊCH VỤ HÀNG LẺ
                        var retailChargeService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.RetailCharge);

                        if (!CheckWebsite(order.WebsiteName))
                        {
                            if (retailChargeService == null)
                            {
                                retailChargeService = new OrderService()
                                {
                                    OrderId = order.Id,
                                    ServiceId = (byte)OrderServices.RetailCharge,
                                    ServiceName =
                                        OrderServices.RetailCharge.GetAttributeOfType<DescriptionAttribute>()
                                            .Description,
                                    ExchangeRate = order.ExchangeRate,
                                    Value = 50,
                                    Currency = Currency.CNY.ToString(),
                                    Type = (byte)UnitType.Money,
                                    TotalPrice = 50 * order.ExchangeRate,
                                    Mode = (byte)OrderServiceMode.Option,
                                    Checked = true,
                                    Created = timeNow,
                                    LastUpdate = timeNow
                                };
                                UnitOfWork.OrderServiceRepo.Add(retailChargeService);
                            }
                            else
                            {
                                retailChargeService.Checked = true;
                            }
                        }
                        else
                        {
                            if (retailChargeService != null)
                            {
                                retailChargeService.Checked = false;
                            }
                        }

                        #endregion

                        // Submit thêm OrderService
                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật số lượng Sum
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                            x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        await UnitOfWork.OrderRepo.SaveAsync();

                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        if (order.CustomerId != null)
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeNow,
                                Content = "[System] Added from seperating orders",
                                CustomerId = order.CustomerId.Value,
                                CustomerName = order.CustomerName,
                                OrderId = order.Id,
                                Status = order.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = order.Type
                            });

                        await UnitOfWork.OrderHistoryRepo.SaveAsync();

                        listOrderView.Add(new
                        {
                            order.Id,
                            order.Code
                        });
                    }

                    //Cancel Orders cũ đi
                    orderOld.Status = (byte)OrderStatus.Cancel;
                    orderOld.LastUpdate = timeNow;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Hủy link hàng
                    var listOrderDetailOld = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == orderOld.Id);
                    foreach (var orderDetail in listOrderDetailOld)
                    {
                        orderDetail.LastUpdate = timeNow;
                        orderDetail.QuantityBooked = 0;
                        orderDetail.TotalPrice = 0;
                        orderDetail.TotalExchange = 0;
                        orderDetail.Status = (byte)OrderDetailStatus.Cancel;
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    if (orderOld.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = "[System] Order cancelled due to link seperating",
                            CustomerId = orderOld.CustomerId.Value,
                            CustomerName = orderOld.CustomerName,
                            OrderId = orderOld.Id,
                            Status = orderOld.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = orderOld.Type
                        });
                    UnitOfWork.OrderHistoryRepo.Save();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = orderOld.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Perfom seperating orders '{MyCommon.ReturnCode(orderOld.Code)}'",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Order seperation completed!", listOrderView }, JsonRequestBehavior.AllowGet);
        }

        public bool CheckWebsite(string website)
        {
            if (website.Contains("taobao.com"))
                return true;
            if (website.Contains("1688.com"))
                return true;
            if (website.Contains("tmall.com"))
                return true;
            return false;
        }


        /// <summary>
        /// Chuyển link Orders
        /// </summary>
        /// <param name="orderOldId"></param>
        /// <param name="orderNewId"></param>
        /// <param name="orderDetailId"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCustomerCare, EnumPage.OrderWait)]
        public async Task<JsonResult> ForwardLink(int orderOldId, int orderNewId, int orderDetailId)
        {
            var checkCancel = false;
            var timeNow = DateTime.Now;
            //1. lấy các thông tin
            var orderOld = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == orderOldId && x.Status > (byte)OrderStatus.New && x.Status < (byte)OrderStatus.WaitOrder);
            if (orderOld == null)
            {
                return Json(new { status = MsgType.Error, msg = "Link removing order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var orderNew = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == orderNewId && x.Status > (byte)OrderStatus.New && x.Status < (byte)OrderStatus.WaitOrder);
            if (orderNew == null)
            {
                return Json(new { status = MsgType.Error, msg = "Link receipt order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var orderDetail = await UnitOfWork.OrderDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == orderDetailId);
            if (orderDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Transferred link does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //2. thực hiện thao tác chuyển link
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //1. Thực hiện xóa link cũ
                    orderDetail.IsDelete = true;
                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //2. Tính toán lại thông tin Orders cũ
                    orderOld.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                          .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                          .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                          .SumAsync(x => x.TotalExchange) : 0;

                    orderOld.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalPrice) : 0;

                    orderOld.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.QuantityBooked.Value) : 0;

                    orderOld.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .CountAsync() : 0;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Cập nhật lại giá khi Edit số lượng sản phẩm
                    if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                    {
                        var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == orderOld.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
                            .ToListAsync();

                        var totalQuantity = productDetails.Sum(x => x.QuantityBooked.Value);

                        var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(orderDetail.Prices);

                        var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin) || (totalQuantity >= x.Begin && totalQuantity <= x.End));

                        if (price != null)
                        {
                            foreach (var pd in productDetails)
                            {
                                pd.Price = price.Price;
                                pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                if (pd.QuantityBooked == null) continue;
                                pd.TotalPrice = pd.Price * pd.QuantityBooked.Value;
                                pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.QuantityBooked.Value;
                                pd.AuditPrice = OrderRepository.OrderAudit(orderOld.ProductNo, pd.Price);
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }
                    }

                    //--------- Cập nhật lại giá dịch vụ -----------
                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Orders
                    var totalExchange = orderOld.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(orderOld.ServiceType, totalExchange) / 100;

                    // Orders nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (orderOld.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == orderOld.Id && !x.IsDelete);

                    //if (service != null)
                    //{
                    //    service.LastUpdate = timeNow;
                    //    service.Value = OrderRepository.OrderPrice(orderOld.ServiceType, totalExchange);
                    //    service.TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice;
                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    //    // Triết khấu phí mua hàng
                    //    var discount = UnitOfWork.OrderRepo.CustomerVipLevel(orderOld.LevelId).Order;
                    //    if (discount > 0)
                    //    {
                    //        service.TotalPrice -= service.TotalPrice * discount / 100;
                    //        service.Note =
                    //            $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                    //    }

                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();
                    //}

                    // Cập nhật tiền dịch vụ kiểm đếm
                    var orderAuditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                               x => x.OrderId == orderOld.Id && !x.IsDelete &&
                                   x.ServiceId == (byte)OrderServices.Audit && x.Checked);

                    if (orderAuditService != null)
                    {
                        var listDetailSer = UnitOfWork.OrderDetailRepo.Entities.Where(x => !x.IsDelete && x.OrderId == orderOld.Id && x.AuditPrice != null && x.Status == (byte)OrderDetailStatus.Order);
                        var totalAuditPrice = (decimal)0;
                        if (listDetailSer.Any())
                        {
                            totalAuditPrice = listDetailSer.Sum(x => x.AuditPrice.Value * x.QuantityBooked.Value);
                        }

                        orderAuditService.Value = totalAuditPrice;
                        orderAuditService.TotalPrice = totalAuditPrice;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }


                    // Cập nhật Total order amount
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == orderOld.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    orderOld.LastUpdate = timeNow;
                    orderOld.Total = totalService + orderOld.TotalExchange;
                    orderOld.Debt = orderOld.Total - (orderOld.TotalPayed - orderOld.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = orderOld.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Transferring product link: <a href='{orderDetail.Link}'>{orderDetail.Link}</a> to order '{MyCommon.ReturnCode(orderNew.Code)}'",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //Check Orders cũ hết link thì Cancel Orders đi
                    var countOrderDetail = await UnitOfWork.OrderDetailRepo.CountAsync(x => !x.IsDelete && x.OrderId == orderOld.Id);
                    if (countOrderDetail == 0)
                    {
                        orderOld.LastUpdate = timeNow;
                        orderOld.Status = (byte)OrderStatus.Cancel;
                        orderOld.ReasonCancel = $"[System] Order cancelled due to link(s) being sent to other order '{MyCommon.ReturnCode(orderNew.Code)}'";

                        await UnitOfWork.OrderRepo.SaveAsync();
                        checkCancel = true;
                    }

                    //2. tạo link mới sang Orders mới

                    var orderDetailNew = orderDetail;
                    orderDetailNew.OrderId = orderNew.Id;
                    orderDetailNew.Created = timeNow;
                    orderDetailNew.LastUpdate = timeNow;
                    orderDetailNew.ExchangeRate = orderNew.ExchangeRate;
                    orderDetailNew.ExchangePrice = orderDetailNew.Price * orderDetailNew.ExchangeRate;
                    orderDetailNew.TotalPrice = orderDetailNew.Price * orderDetailNew.Quantity;
                    orderDetailNew.TotalExchange = (orderDetailNew.Price * orderDetailNew.ExchangeRate) * orderDetailNew.Quantity;
                    orderDetailNew.IsDelete = false;

                    UnitOfWork.OrderDetailRepo.Add(orderDetailNew);
                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //3. Tính lại thông tin Orders mới
                    orderNew.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalExchange) : 0;

                    orderNew.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalPrice) : 0;

                    orderNew.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.QuantityBooked.Value) : 0;

                    orderNew.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .CountAsync() : 0;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Cập nhật lại giá khi Edit số lượng sản phẩm
                    if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                    {
                        var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == orderNew.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
                            .ToListAsync();

                        var totalQuantity = productDetails.Sum(x => x.QuantityBooked.Value);

                        var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(orderDetail.Prices);

                        var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin) || (totalQuantity >= x.Begin && totalQuantity <= x.End));

                        if (price != null)
                        {
                            foreach (var pd in productDetails)
                            {
                                pd.Price = price.Price;
                                pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                if (pd.QuantityBooked == null) continue;
                                pd.TotalPrice = pd.Price * pd.QuantityBooked.Value;
                                pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.QuantityBooked.Value;
                                pd.AuditPrice = OrderRepository.OrderAudit(orderNew.ProductNo, pd.Price);
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }
                    }

                    //--------- Cập nhật lại giá dịch vụ -----------
                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Orders
                    totalExchange = orderNew.TotalExchange;
                    totalPrice = totalExchange * OrderRepository.OrderPrice(orderNew.ServiceType, totalExchange) / 100;

                    // Orders nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (orderOld.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == orderNew.Id && !x.IsDelete);

                    //if (service != null)
                    //{
                    //    service.LastUpdate = timeNow;
                    //    service.Value = OrderRepository.OrderPrice(orderNew.ServiceType, totalExchange);
                    //    service.TotalPrice = totalPrice == 0 ? 0 : totalPrice < 5000 ? 5000 : totalPrice;
                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    //    // Triết khấu phí mua hàng
                    //    var discount = UnitOfWork.OrderRepo.CustomerVipLevel(orderNew.LevelId).Order;
                    //    if (discount > 0)
                    //    {
                    //        service.TotalPrice -= service.TotalPrice * discount / 100;
                    //        service.Note =
                    //            $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                    //    }

                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();
                    //}

                    // Cập nhật tiền dịch vụ kiểm đếm
                    orderAuditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                               x => x.OrderId == orderNew.Id && !x.IsDelete &&
                                   x.ServiceId == (byte)OrderServices.Audit && x.Checked);

                    if (orderAuditService != null)
                    {
                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == orderNew.Id && x.AuditPrice != null
                                    && x.Status == (byte)OrderDetailStatus.Order)
                                .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        orderAuditService.Value = totalAuditPrice;
                        orderAuditService.TotalPrice = totalAuditPrice;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }


                    // Cập nhật Total order amount
                    totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == orderNew.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    orderNew.LastUpdate = timeNow;
                    orderNew.Total = totalService + orderNew.TotalExchange;
                    orderNew.Debt = orderNew.Total - (orderNew.TotalPayed - orderNew.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    orderLog = new OrderLog
                    {
                        OrderId = orderNew.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Get product link: <a href='{orderDetail.Link}'>{orderDetail.Link}</a> from order '{MyCommon.ReturnCode(orderOld.Code)}'",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Order link transfering completed!", checkCancel }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lấy danh sách Orders chuyển link
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.OrderCustomerCare, EnumPage.OrderWait)]
        public async Task<JsonResult> GetOrderForward(int customerId, int orderId)
        {
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(x => !x.IsDelete && x.Status > (byte)OrderStatus.WaitPrice && x.Status < (byte)OrderStatus.WaitOrder && x.Type == (byte)OrderType.Order && x.CustomerId == customerId && x.Id != orderId);

            return Json(new { listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> IsView(int id)
        {
            var orderDetail = await UnitOfWork.OrderDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (orderDetail != null)
            {
                orderDetail.IsView = true;
                await UnitOfWork.OrderDetailRepo.SaveAsync();
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderWaitSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = String.IsNullOrEmpty(keyword) ? "" : keyword.Trim();
            //long totalStaff;
            List<dynamic> items = new List<dynamic>();

            var listCustomer = UnitOfWork.OrderRepo.Find(
                   out totalRecord,
                    x => !x.IsDelete
                    && x.Status == (byte)OrderStatus.WaitPrice
                    && (x.Id.ToString().Contains(keyword)
                    || x.UserFullName.Contains(keyword)
                    || x.Code.Contains(keyword)
                    || x.CustomerEmail.Contains(keyword)
                    || x.CustomerName.Contains(keyword)
                    || x.CustomerPhone.Contains(keyword)
                    || x.SystemName.Contains(keyword)
                    ),
                   x => x.OrderByDescending(y => y.Code),
                   page ?? 1,
                   10
              ).ToList();


            items.AddRange(listCustomer.Select(
                    x =>
                        new
                        {
                            id = x.Id,
                            code = x.Code,
                            text = x.Code,
                            systemName = x.SystemName,
                        }));

            return Json(new { incomplete_results = true, total_count = totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateRetail(int orderId, bool isRetail)
        {
            var timeNow = DateTime.Now;

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Status > (byte)OrderStatus.WaitPrice && x.Status < (byte)OrderStatus.WaitOrder && x.Type == (byte)OrderType.Order && x.Id == orderId);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //2. thực hiện thao tác chuyển link
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Cập nhật Orders lẻ
                    order.IsRetail = isRetail;
                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    var retailChargeService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.RetailCharge);

                    if (isRetail)
                    {
                        if (retailChargeService == null)
                        {
                            retailChargeService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.RetailCharge,
                                ServiceName = OrderServices.RetailCharge.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = order.ExchangeRate,
                                Value = 50,
                                Currency = Currency.CNY.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 50 * order.ExchangeRate,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = true,
                                Created = timeNow,
                                LastUpdate = timeNow
                            };
                            UnitOfWork.OrderServiceRepo.Add(retailChargeService);
                        }
                        else
                        {
                            retailChargeService.Checked = true;
                        }
                    }
                    else
                    {
                        if (retailChargeService != null)
                        {
                            retailChargeService.Checked = false;
                        }
                    }

                    // Submit thêm OrderService
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật số lượng Sum
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                        x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}