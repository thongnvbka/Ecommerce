using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Helpers;
using Library.Models;
using Newtonsoft.Json;
using Library.ViewModels.Account;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Library.Settings;
using Library.UnitOfWork;
using System.Runtime.ExceptionServices;
using Hangfire;
using Cms.Jobs;
using Microsoft.Ajax.Utilities;
using Library.Jobs;

namespace Cms.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        #region [1. Lây danh sách Order Order]

        /// <summary>
        /// lây danh sách Order chưa nhân xử lý
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
        [LogTracker(EnumAction.View, EnumPage.OrderNew)]
        public async Task<JsonResult> GetOrderNew(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            var user = UnitOfWork.UserRepo.Entities.Find(UserState.UserId);
            var totalAwait = UnitOfWork.OrderRepo.Entities.Count(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitOrder && !x.IsRetail);
            var totalAwaitUser = !string.IsNullOrEmpty(user.Websites) ? UnitOfWork.OrderRepo.Entities.Count(x => user.Websites.Contains(x.WebsiteName) && !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitOrder && !x.IsRetail) : totalAwait;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status)
                      && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                      && (systemId == -1 || x.SystemId == systemId) && (customerId == null || x.CustomerId == customerId)
                      && (dateStart == null || x.Created >= dateStart)
                      && (dateEnd == null || x.Created <= dateEnd)
                      && (x.Status == (byte)OrderStatus.WaitOrder)
                      && x.UserId == null && !x.IsDelete && x.Type == (byte)OrderType.Order,
                  x => x.OrderByDescending(y => y.Created),
                  page,
                  pageSize
              );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                  out totalRecord,
                  x =>
                      (status == -1 || x.Status == status)
                      && (x.Code == (keyword))
                      && (systemId == -1 || x.SystemId == systemId) && (customerId == null || x.CustomerId == customerId)
                      && (dateStart == null || x.Created >= dateStart)
                      && (dateEnd == null || x.Created <= dateEnd)
                      && (x.Status == (byte)OrderStatus.WaitOrder)
                      && x.UserId == null && !x.IsDelete && x.Type == (byte)OrderType.Order,
                  x => x.OrderByDescending(y => y.Created),
                  page,
                  pageSize
              );
            }

            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalAwait, totalAwaitUser }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lấy danh sách Order đã nhận xử lý
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
        [LogTracker(EnumAction.View, EnumPage.OrderOrder)]
        public async Task<JsonResult> GetOrder(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            var user = UnitOfWork.UserRepo.Entities.Find(UserState.UserId);
            var totalAwait = UnitOfWork.OrderRepo.Entities.Count(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitOrder && !x.IsRetail);
            var totalAwaitUser = !string.IsNullOrEmpty(user.Websites) ? UnitOfWork.OrderRepo.Entities.Count(x => user.Websites.Contains(x.WebsiteName) && !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitOrder && !x.IsRetail) : totalAwait;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Order
                    && x.Status >= (byte)OrderStatus.Order
                    && !x.IsRetail,
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
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.Code == (keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Order
                    && x.Status >= (byte)OrderStatus.Order
                    && !x.IsRetail,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            var listStatus = new List<StatuShow>();

            var listOrderCount = await UnitOfWork.OrderRepo.FindAsync(x => !x.IsDelete && x.Status > (byte)OrderStatus.WaitDeposit && x.Type == (byte)OrderType.Order);

            foreach (var list in listOrderCount.OrderBy(x => x.Status).GroupBy(x => x.Status))
            {
                var firstOrDefault = list.FirstOrDefault();
                if (firstOrDefault != null)
                    listStatus.Add(new StatuShow { Status = firstOrDefault.Status, Count = list.Count() });
            }


            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalAwait, totalAwaitUser, listStatus }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// lấy danh sách Order trễ xử lý
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
        /// <param name="isAllDelay"></param>
        /// <returns></returns>
        [LogTracker(EnumAction.View, EnumPage.OrderDelay)]
        public async Task<JsonResult> GetOrderDelay(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool isAllDelay, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);
            var dateDelay = DateTime.Now.AddMinutes(TimeDelay);

            //2. Lấy dữ liệu

            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x =>
                    (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && !x.IsDelete
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId) && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && (x.Status == (byte)OrderStatus.Order) && x.Type == (byte)OrderType.Order
                    && x.Created <= dateDelay,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x =>
                    (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && !x.IsDelete
                    && (x.Code == (keyword))
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId) && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && (x.Status == (byte)OrderStatus.Order) && x.Type == (byte)OrderType.Order
                    && x.Created <= dateDelay,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            if (isAllDelay)
            {
                listOrder = await UnitOfWork.OrderRepo.GetOrderDelayAll(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);
            }

            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.View, EnumPage.OrderRetail)]
        public async Task<JsonResult> GetOrderRetail(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);

            var totalAwait = UnitOfWork.OrderRepo.Entities.Count(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitOrder && x.IsRetail);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    //&& (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Order
                    && x.Status >= (byte)OrderStatus.WaitOrder
                    && x.IsRetail,
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
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.Code == (keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    //&& (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Order
                    && x.Status >= (byte)OrderStatus.WaitOrder
                    && x.IsRetail,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            }

            var listStatus = new List<StatuShow>();

            var listOrderCount = await UnitOfWork.OrderRepo.FindAsync(x => !x.IsDelete && x.Status > (byte)OrderStatus.WaitDeposit && x.Type == (byte)OrderType.Order && x.IsRetail);

            foreach (var list in listOrderCount.OrderBy(x => x.Status).GroupBy(x => x.Status))
            {
                var firstOrDefault = list.FirstOrDefault();
                if (firstOrDefault != null)
                    listStatus.Add(new StatuShow { Status = firstOrDefault.Status, Count = list.Count() });
            }


            if (listOrder.Any())
            {
                var ids = listOrder.Select(x => x.Id);
                var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => !x.IsRead.Value && ids.Contains(x.OrderId) && x.CustomerId != null);

                //3. Lấy thông tin chat
                foreach (var item in listOrder)
                {
                    item.Chat = listChat.Count(x => x.OrderId == item.Id);
                }
            }

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalAwait, totalAwaitUser = totalAwait, listStatus }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách mã vận đơn
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
        [LogTracker(EnumAction.View, EnumPage.OrderLadingCode)]
        public async Task<JsonResult> GetOrderLadingCode(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);
            //var dateDelay = DateTime.Now.AddMinutes(TimeDelay);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.GetOrderLadingCode(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, checkExactCode);

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// lấy danh sách nhân viên
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [LogTracker(EnumAction.View, EnumPage.OrderLadingCode)]
        public async Task<JsonResult> GetUser(int page, int pageSize, int? userId)
        {
            //1. Khởi tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu

            var listOrder = await UnitOfWork.UserRepo.GetUserToOfficeType(out totalRecord, page, pageSize, userId, OfficeType.Order);

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        #endregion [1. Lây danh sách Order Order]

        #region [2. Các thao tác với Order]

        /// <summary>
        /// Nhân viên nhận một Order về xử lý
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderNew)]
        public async Task<JsonResult> ReceivePurchaseOrder(int id)
        {
            //1. Lấy thông tin Order
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == (byte)OrderStatus.WaitOrder);

            //2. Check dữ liệu
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.Status != (byte)OrderStatus.WaitOrder)
            {
                return Json(new { status = MsgType.Error, msg = "Order must be queued!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.UserId != null)
            {
                return Json(new { status = MsgType.Error, msg = "Order have already had other staff accept to process!" }, JsonRequestBehavior.AllowGet);
            }

            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == UserState.UserId && x.Status == (byte)OrderStatus.Order && !x.IsDelete);
            //if (countOrder >= 15)// vượt quá Order number nhân viên đang xử lý
            //{
            //    return Json(new { status = MsgType.Error, msg = "Vượt quá Order number Staff handling!" }, JsonRequestBehavior.AllowGet);
            //}
            if (countOrder >= 10)// vượt quá Order number nhân viên đang xử lý
            {
                return Json(new { status = MsgType.Error, msg = "Exceeds number of orders for staff to handle!" }, JsonRequestBehavior.AllowGet);
            }

            //3. nhận Order về xử lý
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.UserId = UserState.UserId;
                    order.UserName = UserState.UserName;
                    order.UserFullName = UserState.FullName;
                    order.OfficeId = UserState.OfficeId;
                    order.OfficeName = UserState.OfficeName;
                    order.OfficeIdPath = UserState.OfficeIdPath;
                    order.LastUpdate = timeNow;

                    order.Status = (byte)OrderStatus.Order;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    if (order.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = "Accept to handle order",
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
                        Content = "Accept to handle order",
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

            // 4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Order received successfully!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Nhân viên nhận 5 Order về xử lý cùng lúc
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderNew, EnumPage.OrderOrder)]
        public async Task<JsonResult> ReceivePurchaseMultiOrder()
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var listOrderCode = new List<string>();
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == UserState.UserId);

            var listOrder = await UnitOfWork.OrderRepo.FindAsync(x => x.UserId == null && (x.Status == (byte)OrderStatus.WaitOrder) && !x.IsDelete && !x.IsRetail);

            if (!string.IsNullOrEmpty(user.Websites))
            {
                var websiteArray = user.Websites.Split(' ');

                listOrder = await UnitOfWork.OrderRepo.FindAsync(x => x.UserId == null && (x.Status == (byte)OrderStatus.WaitOrder) && !x.IsDelete && websiteArray.Any(y => x.WebsiteName.Contains(y)) && !x.IsRetail);
            }
            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == UserState.UserId && x.Status == (byte)OrderStatus.Order && !x.IsDelete);
            //var count = (15 - countOrder) > 5 ? 5 : (15 - countOrder); //Lấy Order number có thể nhận
            var count = (10 - countOrder) > 5 ? 5 : (10 - countOrder); //Lấy Order number có thể nhận

            //2. check điều kiện
            if (listOrder.Count == 0)
            {
                return Json(new { status = MsgType.Error, msg = "No Order to be received!" }, JsonRequestBehavior.AllowGet);
            }

            //if (countOrder >= 15)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Exceed Order number is processing!" }, JsonRequestBehavior.AllowGet);
            //}
            if (countOrder >= 10)
            {
                return Json(new { status = MsgType.Error, msg = "Exceeds the number of orders that are currently being processed!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var i = 1;
                    listOrder = listOrder.OrderBy(x => x.Created).ToList();
                    foreach (var order in listOrder)
                    {
                        if (i > count) break;
                        //Kiểm tra Order đã có người nhận xử lý chưa
                        if (order.UserId == null)
                        {
                            order.UserId = UserState.UserId;
                            order.UserName = UserState.UserName;
                            order.UserFullName = UserState.FullName;
                            order.OfficeId = UserState.OfficeId;
                            order.OfficeName = UserState.OfficeName;
                            order.OfficeIdPath = UserState.OfficeIdPath;
                        }

                        order.LastUpdate = timeNow;
                        order.Status = (byte)OrderStatus.Order;
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        if (order.CustomerId != null)
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeNow,
                                Content = "Accept to handle order",
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
                            Content = "Accept to handle order",
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
            return Json(new { status = MsgType.Success, msg = "Order received successfully!", listOrderCode }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Nhân viên nhận 5 Order về xử lý cùng lúc
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderRetail)]
        public async Task<JsonResult> ReceivePurchaseMultiOrderRetail()
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var listOrderCode = new List<string>();
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == UserState.UserId);

            var listOrder = await UnitOfWork.OrderRepo.FindAsync(x => x.UserId == null && (x.Status == (byte)OrderStatus.WaitOrder) && !x.IsDelete && x.IsRetail);
            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == UserState.UserId && x.Status == (byte)OrderStatus.Order && !x.IsDelete);
            //var count = (15 - countOrder) > 5 ? 5 : (15 - countOrder); //Lấy Order number có thể nhận
            var count = (10 - countOrder) > 5 ? 5 : (10 - countOrder); //Lấy Order number có thể nhận

            //2. check điều kiện
            if (listOrder.Count == 0)
            {
                return Json(new { status = MsgType.Error, msg = "No order can be received!" }, JsonRequestBehavior.AllowGet);
            }

            //if (countOrder >= 15)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Exceed Order number is processing!" }, JsonRequestBehavior.AllowGet);
            //}
            if (countOrder >= 10)
            {
                return Json(new { status = MsgType.Error, msg = "Exceeds the number of orders currently being processed!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var i = 1;
                    listOrder = listOrder.OrderBy(x => x.Created).ToList();
                    foreach (var order in listOrder)
                    {
                        if (i > count) break;
                        //Kiểm tra Order đã có người nhận xử lý chưa
                        if (order.UserId == null)
                        {
                            order.UserId = UserState.UserId;
                            order.UserName = UserState.UserName;
                            order.UserFullName = UserState.FullName;
                            order.OfficeId = UserState.OfficeId;
                            order.OfficeName = UserState.OfficeName;
                            order.OfficeIdPath = UserState.OfficeIdPath;
                        }

                        order.LastUpdate = timeNow;
                        order.Status = (byte)OrderStatus.Order;
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        if (order.CustomerId != null)
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeNow,
                                Content = "Accept to handle order",
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
                            Content = "Accept to handle order",
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
            return Json(new { status = MsgType.Success, msg = "Order received successfully!", listOrderCode }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Convert staff khác xử lý
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderNew, EnumPage.OrderDelay, EnumPage.OrderOrder)]
        public async Task<JsonResult> OrderReply(int orderId, UserOfficeResult user)
        {
            //1. Lấy thông tin Order
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == user.Id && x.Status == (byte)OrderStatus.Order && !x.IsDelete);

            //2. Check điều kiện
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.UserId == user.Id)
            {
                return Json(new { status = MsgType.Error, msg = $"Staff {user.FullName} is handling this order!" }, JsonRequestBehavior.AllowGet);
            }

            //if (countOrder >= 15)// vượt quá Order number nhân viên đang xử lý
            //{
            //    return Json(new { status = MsgType.Error, msg = $"Vượt quá số đơn hàng mà nhân viên {user.FullName} đang xử lý!" }, JsonRequestBehavior.AllowGet);
            //}

            if (countOrder >= 40)// vượt quá Order number nhân viên đang xử lý
            {
                return Json(new { status = MsgType.Error, msg = $"Exceeds the number of orders staff {user.FullName} is handling!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.UserId = user.Id;
                    order.UserName = UserState.UserName;
                    order.UserFullName = user.FullName;
                    order.OfficeId = user.OfficeId;
                    order.OfficeName = user.OfficeName;
                    order.OfficeIdPath = user.OfficeIdPath;
                    order.LastUpdate = timeNow;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    if (order.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"Transfer to staff {user.FullName} - {user.TitleName} to process",
                            CustomerId = order.CustomerId.Value,
                            CustomerName = order.CustomerName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });
                    UnitOfWork.OrderHistoryRepo.Save();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Transfer order to staff <b>{user.FullName} - {user.TitleName}</b> to handle",
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

            //4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = $"Successfully transfer order to staff {user.FullName} to process!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật thông tin Order
        /// </summary>
        /// <param name="id"> Order code</param>
        /// <param name="priceBargain">Bargain order is</param>
        /// <param name="totalShop">Total money paid company with shop</param>
        /// <param name="feeShipBargain">Shipment fee for bargain order is with shop</param>
        /// <param name="feeShip">Domestic Chinese domestic shipping fee</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderSourcing)]
        public async Task<JsonResult> UpdateOrder(int id, decimal? priceBargain, decimal? totalShop, decimal? feeShipBargain, decimal? feeShip)
        {
            //1. Khở tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                List<LogResult> dataBefore = new List<LogResult>();
                List<LogResult> dataAfter = new List<LogResult>();

                try
                {
                    //cập nhật thông tin Order
                    order.IsPayWarehouseShip = false;

                    if (totalShop != null && DataCompare(order.PaidShop, totalShop ?? 0))
                    {
                        dataBefore.Add(new LogResult() { Name = "Payment to shop", Value = order.PaidShop.ToString() });

                        order.PaidShop = totalShop - (feeShipBargain ?? 0);

                        dataAfter.Add(new LogResult() { Name = "Payment to shop", Value = order.PaidShop.ToString() });
                    }

                    order.LastUpdate = timeNow;

                    //if (priceBargain != null)
                    //{
                    //    dataBefore.Add(new LogResult() { Name = "Tiền bargain với shop", Value = order.PriceBargain == null ? "0" : order.PriceBargain.Value.ToString("N2") });
                    //    order.PriceBargain = priceBargain;
                    //    dataAfter.Add(new LogResult() { Name = "Tiền bargain với shop", Value = order.PriceBargain.Value.ToString("N2") });
                    //}

                    if (feeShipBargain != null && DataCompare(order.FeeShipBargain, feeShipBargain ?? 0))
                    {
                        dataBefore.Add(new LogResult() { Name = "Domestic China bargained shipping fee ", Value = order.FeeShipBargain == null ? "0" : order.FeeShipBargain.Value.ToString("N2") });
                        order.FeeShipBargain = feeShipBargain;
                        dataAfter.Add(new LogResult() { Name = "Domestic China bargained shipping fee ", Value = order.FeeShipBargain.Value.ToString("N2") });

                        if (feeShipBargain > totalShop)
                        {
                            transaction.Rollback();
                            return Json(new { status = MsgType.Error, msg = "The shipping charge paid by the company must be less than the total amount paid by the company!" }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    if (feeShip != null && DataCompare(order.FeeShip, feeShip ?? 0))
                    {
                        dataBefore.Add(new LogResult() { Name = "China domestic shipping fee", Value = order.FeeShip == null ? "0" : order.FeeShip.Value.ToString("N2") });
                        order.FeeShip = feeShip;
                        dataAfter.Add(new LogResult() { Name = "China domestic shipping fee", Value = order.FeeShip.Value.ToString("N2") });

                        var orderServiceShopShipping = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                            x =>
                                x.OrderId == order.Id && !x.IsDelete &&
                                x.ServiceId == (byte)OrderServices.ShopShipping
                        );

                        orderServiceShopShipping.Value = feeShip.Value;
                        orderServiceShopShipping.TotalPrice = orderServiceShopShipping.Value * orderServiceShopShipping.ExchangeRate;
                        orderServiceShopShipping.LastUpdate = timeNow;
                        orderServiceShopShipping.Checked = true;
                        orderServiceShopShipping.Note = $"Shop shipping fee is {feeShip.Value.ToString("N4", CultureInfo)} Yuan equalling to {orderServiceShopShipping.TotalPrice.ToString("N0", CultureInfo)} VND";

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật số lượng Sum
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                                    x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        var priceBargainTotal = (order.TotalPrice + (order.FeeShip ?? 0)) - ((order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0));

                        order.PriceBargain = priceBargainTotal < 0 ? 0 : priceBargainTotal;
                    }

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

            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            //4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Updated successfully!", listOrderService }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật note của nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderSourcing)]
        public async Task<JsonResult> UpdateOrderUserNote(int id, string note)
        {
            //1. Khở tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "Staff's note", Value = order.UserNote });
                    order.UserNote = note;
                    order.LastUpdate = timeNow;
                    dataAfter.Add(new LogResult() { Name = "Staff's note", Value = order.UserNote });

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit order note",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //Ghi chú toàn system cho package, Order
                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                            x =>
                                x.PackageId == null && x.OrderId == order.Id && x.ObjectId == order.Id &&
                                x.Mode == (byte)PackageNoteMode.Order);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(order.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = order.Id,
                            OrderCode = order.Code,
                            PackageId = null,
                            PackageCode = null,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = order.Id,
                            ObjectCode = order.Code,
                            Mode = (byte)PackageNoteMode.Order,
                            Content = order.Note
                        });
                    }
                    else if (packageNote != null && !string.IsNullOrWhiteSpace(order.Note))
                    {
                        packageNote.Content = order.Note;
                    }
                    else if (packageNote != null && string.IsNullOrWhiteSpace(order.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Remove(packageNote);
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
            return Json(new { status = MsgType.Success, msg = "Updated successfully!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Nhân viên đặt hàng thành công chờ kế toàn thanh toán
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderSourcing)]
        public async Task<JsonResult> OrderSuccess(int id)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => !x.IsDelete && x.OrderId == id);

            //2. check điều kiện
            if (order.WarehouseId <= 0)
            {
                return Json(new { status = MsgType.Error, msg = "Warehouse not selected!" }, JsonRequestBehavior.AllowGet);
            }

            if (listContractCode.Count == 0)
            {
                return Json(new { status = MsgType.Error, msg = "Enter Contract code!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.PaidShop <= 0 || order.PaidShop == null)
            {
                return Json(new { status = MsgType.Error, msg = "Money paid to shop Can not be empty!" }, JsonRequestBehavior.AllowGet);
            }

            if (listContractCode.FirstOrDefault((x => x.ContractCode.Trim() == "")) != null)
            {
                return Json(new { status = MsgType.Error, msg = " A certain contract code is being left blank!" }, JsonRequestBehavior.AllowGet);
            }

            if (listContractCode.FirstOrDefault((x => x.TotalPrice == 0 || x.TotalPrice == null)) != null)
            {
                return Json(new { status = MsgType.Error, msg = "There exists a certain code of contract in which money amount has not been entered!" }, JsonRequestBehavior.AllowGet);
            }

            if (order.TotalPrice < order.PaidShop)
            {
                return Json(new { status = MsgType.Error, msg = "Total amount of money paid by customer must be greater than that paid by company!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.Status = (byte)OrderStatus.WaitAccountant;
                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    ////kiểm tra kho hà nội thì bỏ check dịch vụ đi bay.
                    //if (order.WarehouseDeliveryId == 8)
                    //{
                    //    var fastDeliveryServiceCheck = await
                    //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //        x => !x.IsDelete && x.OrderId == order.Id &&
                    //             x.ServiceId == (byte)OrderServices.FastDelivery);

                    //    if (fastDeliveryServiceCheck != null)
                    //    {
                    //        fastDeliveryServiceCheck.Checked = false;
                    //        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    //    }
                    //}

                    // Thêm lịch sử thay đổi trạng thái
                    if (order.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = "Ordered successfully, waiting for accountant disbursement",
                            CustomerId = order.CustomerId.Value,
                            CustomerName = order.CustomerName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });
                    UnitOfWork.OrderHistoryRepo.Save();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = "Ordered successfully, waiting for accountant disbursement",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    // Bắn Notification
                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);

                    foreach (var item in listContractCode)
                    {
                        item.Status = (byte)ContractCodeType.AwaitingPayment;
                        item.UpdateDate = timeNow;
                    }
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    foreach (var user in listUser)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Pay order contract #" + MyCommon.ReturnCode(order.Code), EnumNotifyType.Info, "Request to pay for contract: " + order.ContractCodes);
                    }

                    // Lưu vào Database
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

            // Cập nhật công nợ
            BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport($";{id};"));

            //4. gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Order successfully completed!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật kho hàng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouseId"></param>
        /// <param name="warehouseName"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderSourcing, EnumPage.OrderCommerce)]
        public async Task<JsonResult> UpdateOrderWarehouse(int id, int warehouseId, string warehouseName)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "China warehouse", Value = order.WarehouseName });

                    order.WarehouseId = warehouseId;
                    order.WarehouseName = warehouseName;
                    order.LastUpdate = timeNow;

                    dataAfter.Add(new LogResult() { Name = "China warehouse", Value = order.WarehouseName });

                    await UnitOfWork.OrderRepo.SaveAsync();

                    var listPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type);

                    var warehouse = await UnitOfWork.OfficeRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == warehouseId);

                    foreach (var item in listPackage)
                    {
                        item.WarehouseId = warehouse.Id;
                        item.WarehouseName = warehouse.Name;
                        item.WarehouseAddress = warehouse.Address;
                        item.WarehouseIdPath = warehouse.IdPath;

                        item.LastUpdate = timeNow;
                    }
                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit receiving warehouse information",
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
            return Json(new { status = MsgType.Success, msg = "Warehouse updated successful!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật kho hàng việt nam
        /// </summary>
        /// <param name="id"></param>
        /// <param name="warehouseDeliveryId"></param>
        /// <param name="warehouseDeliveryName"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderSourcing, EnumPage.OrderCustomerCare, EnumPage.OrderWait, EnumPage.OrderCommerce)]
        public async Task<JsonResult> UpdateOrderWarehouseVn(int id, int warehouseDeliveryId, string warehouseDeliveryName)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var toWarehouse =
                await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == warehouseDeliveryId);

            if (toWarehouse == null) // Kho đích does not exist or has been deleted
            {
                return Json(new { status = MsgType.Error, msg = "Warehouse destination does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "Vietnam warehouse", Value = order.WarehouseDeliveryName });

                    order.WarehouseDeliveryId = warehouseDeliveryId;
                    order.WarehouseDeliveryName = warehouseDeliveryName;
                    order.LastUpdate = timeNow;

                    dataAfter.Add(new LogResult() { Name = "Vietnam warehouse", Value = order.WarehouseDeliveryName });

                    // @Henry  bổ xung cập nhật lại thông tin kho khách chọn
                    var packages =
                        await UnitOfWork.OrderPackageRepo.FindAsync(x => x.IsDelete == false && x.OrderId == order.Id);

                    foreach (var p in packages)
                    {
                        p.CustomerWarehouseId = toWarehouse.Id;
                        p.CustomerWarehouseName = toWarehouse.Name;
                        p.CustomerWarehouseAddress = toWarehouse.Address;
                        p.CustomerWarehouseIdPath = toWarehouse.IdPath;
                    }

                    // Cập nhật lại tiền vận chuyển của Order
                    #region Update Goods shipping to Vietnam service
                    ////kiểm tra kho hà nội thì bỏ check dịch vụ đi bay.
                    //if (warehouseDeliveryId == 8)
                    //{
                    //    var fastDeliveryServiceCheck = await
                    //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //        x => !x.IsDelete && x.OrderId == order.Id &&
                    //             x.ServiceId == (byte)OrderServices.FastDelivery);

                    //    if (fastDeliveryServiceCheck != null)
                    //    {
                    //        fastDeliveryServiceCheck.Checked = false;
                    //        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    //    }
                    //}

                    //var fastDeliveryService = await
                    //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //        x => !x.IsDelete && x.OrderId == order.Id &&
                    //             x.ServiceId == (byte)OrderServices.FastDelivery && x.Checked);

                    //var optimalService = await
                    //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //        x => !x.IsDelete && x.OrderId == order.Id &&
                    //             x.ServiceId == (byte) OrderServices.Optimal && x.Checked);

                    var outSideShippingService = await
                            UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                    x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                    decimal serviceValue;

                    var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                    // Cân nặng các package được xuất giao tại TQ
                    var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                    // Sum cân nặng tính tiền vc của Order
                    var orderWeight = order.TotalWeight - orderWeightIgnore;

                    decimal outSideShipping;

                    // Order ký gửi
                    if (order.Type == (byte)OrderType.Deposit)
                    {
                        serviceValue = order.ApprovelPrice ?? 0;

                        if (orderWeight >= 50)
                        {
                            outSideShipping = orderWeight * serviceValue;
                        }
                        else
                        {
                            outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                        }
                    }
                    else
                    { // Order Order
                      // VC tiết kiệm
                        //if (optimalService != null)
                        //{
                        //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                        //        order.WarehouseDeliveryId ?? 0);
                        //}
                        //else if (fastDeliveryService != null) // VC nhanh
                        //{
                        //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                        //        order.WarehouseDeliveryId ?? 0);
                        //}
                        //else // VC bình thường
                        //{
                            serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                order.WarehouseDeliveryId ?? 0);
                        //}

                        outSideShipping = orderWeight * serviceValue;
                    }

                    if (outSideShippingService == null)
                    {
                        outSideShippingService = new OrderService()
                        {
                            IsDelete = false,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Value = serviceValue,
                            Currency = Currency.VND.ToString(),
                            ExchangeRate = 0,
                            TotalPrice = outSideShipping,
                            HashTag = string.Empty,
                            Mode = (byte)OrderServiceMode.Required,
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.OutSideShipping,
                            ServiceName =
                                (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                    .Description,
                            Type = (byte)UnitType.Money,
                        };

                        // Triết khấu Vip cho Order Order
                        if (order.Type == (byte)OrderType.Order)
                        {
                            // Trừ tiền triết khấu theo cấp Level Vip
                            outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                 outSideShippingService.TotalPrice / 100;
                            outSideShippingService.Note =
                                $"Shipping service fee to Vietnam {serviceValue.ToString("N0", CultureInfo)} vnd/1kg" +
                                $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                            UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                        }
                    }
                    else
                    {
                        outSideShippingService.LastUpdate = timeNow;
                        outSideShippingService.Value = serviceValue;
                        outSideShippingService.TotalPrice = outSideShipping;

                        // Triết khấu Vip cho Order Order
                        if (order.Type == (byte)OrderType.Order)
                        {
                            // Trừ tiền triết khấu theo cấp Level Vip
                            outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                 outSideShippingService.TotalPrice / 100;
                            outSideShippingService.Note =
                                $"Shipping service fee to Vietnam {serviceValue.ToString("N0", CultureInfo)} vnd/1kg" +
                                $" And was discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                        }
                    }
                    #endregion

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật lại Total money của Order
                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                             x.IsDelete == false && x.Checked)
                        .ToList()
                        .Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    order.LastUpdate = DateTime.Now;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit information of warehouse customer fetching goods",
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
            return Json(new { status = MsgType.Success, msg = "Warehouse updated successful!" }, JsonRequestBehavior.AllowGet);
        }

        #region [Cập nhật ngày dự kiến giao hàng]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> OrderExpectedDate(int id, DateTime date)
        {
            var rs = 0;
            var timeNow = DateTime.Now;

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                rs = -1;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }


            order.ExpectedDate = date;
            order.LastUpdate = timeNow;

            await UnitOfWork.OrderHistoryRepo.SaveAsync();
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion [Cập nhật ngày dự kiến giao hàng]

        #region [Lấy order detail]

        [HttpPost]
        public async Task<JsonResult> GetOrderDetail(int orderId)
        {
            var dateTime = DateTime.Now;
            var status = MsgType.Success;
            var msg = "";

            //1. Lấy thông tin Order Order
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);

            //2. Check Order có tồn tại hay đã bị xóa
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //Kiểm tra các dịch vụ không có thì thêm vào.
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == order.CustomerId && !x.IsDelete);

            if (order.Status == (byte)OrderStatus.AreQuotes)
            {
                // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Order
                var totalExchange = order.TotalExchange;
                var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                // Order nhỏ hơn 2 triệu bị tính 150.000 vnđ
                if (order.TotalExchange < 2000000)
                {
                    totalPrice = 150000;
                }

                //var orderServcie =
                //    await
                //        UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                //            x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);

                #region  Thêm các dịch vụ cho Order

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
                //        Created = dateTime,
                //        LastUpdate = dateTime,
                //        HashTag = string.Empty,
                //        Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange),
                //        Currency = Currency.VND.ToString(),
                //        Type = (byte)UnitType.Percent,
                //        TotalPrice = totalPrice < 5000 ? 5000 : totalPrice,
                //        Mode = (byte)OrderServiceMode.Required,
                //        Checked = true
                //    };

                //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                //}
                //else // Cập nhật dịch vụ mua hàng
                //{
                //    orderServcie.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                //    orderServcie.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;
                //}

                //// Triết khấu phí mua hàng
                //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(customer.LevelId).Order;
                //if (discount > 0)
                //{
                //    orderServcie.TotalPrice -= orderServcie.TotalPrice * discount / 100;
                //    orderServcie.Note =
                //        $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                //}

                // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
                var orderShopShippingService =
                    await
                        UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                            x =>
                                x.OrderId == order.Id && !x.IsDelete &&
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
                        Value = 0,
                        Currency = Currency.CNY.ToString(),
                        Type = (byte)UnitType.Money,
                        TotalPrice = 0,
                        Mode = (byte)OrderServiceMode.Required,
                        Checked = false,
                        Created = dateTime,
                        LastUpdate = dateTime
                    };
                    UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                }

                // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                var autditService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
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
                        Created = dateTime,
                        LastUpdate = dateTime
                    };
                    UnitOfWork.OrderServiceRepo.Add(autditService);
                }
                else
                {
                    var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                    && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                    autditService.Value = totalAuditPrice;
                    autditService.TotalPrice = totalAuditPrice;
                }

                // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                var packingService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                    x => x.OrderId == order.Id && !x.IsDelete &&
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
                        Checked = false,
                        Created = dateTime,
                        LastUpdate = dateTime
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
                        Created = dateTime,
                        LastUpdate = dateTime
                    };
                    UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                }

                //// DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                //var fastDeliveryService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                //    x => x.OrderId == order.Id && !x.IsDelete &&
                //         x.ServiceId == (byte)OrderServices.FastDelivery);

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
                //        Checked = false,
                //        Created = dateTime,
                //        LastUpdate = dateTime
                //    };
                //    UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);
                //}

                //// DỊCH VỤ CHUYỂN TỐI ƯU --------------------------------------------------------------------------
                //var optimalService = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                //    x => x.OrderId == order.Id && !x.IsDelete &&
                //         x.ServiceId == (byte)OrderServices.Optimal);

                //if (optimalService == null)
                //{
                //    optimalService = new OrderService()
                //    {
                //        OrderId = order.Id,
                //        ServiceId = (byte)OrderServices.Optimal,
                //        ServiceName =
                //            OrderServices.Optimal.GetAttributeOfType<DescriptionAttribute>().Description,
                //        ExchangeRate = order.ExchangeRate,
                //        Value = 0,
                //        Currency = Currency.VND.ToString(),
                //        Type = (byte)UnitType.Money,
                //        TotalPrice = 0,
                //        Mode = (byte)OrderServiceMode.Option,
                //        Checked = false,
                //        Created = dateTime,
                //        LastUpdate = dateTime
                //    };
                //    UnitOfWork.OrderServiceRepo.Add(optimalService);
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
                        Created = dateTime,
                        LastUpdate = dateTime
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
            }

            //3. Lấy thông tin Detail của Order
            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == orderId && !x.IsDelete);
            var toAddress = await UnitOfWork.OrderAddressRepo.FirstOrDefaultAsync(x => x.Id == order.ToAddressId);
            var formAddress = await UnitOfWork.OrderAddressRepo.FirstOrDefaultAsync(x => x.Id == order.FromAddressId);
            var orderExchange = await UnitOfWork.OrderExchangeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.OrderId == order.Id && x.Type == (byte)OrderExchangeType.Product);
            var userOrder = order.UserId != null ? await UnitOfWork.UserRepo.GetUserToOfficeOrder(order.UserId.Value) : null;
            var listWarehouse = await UnitOfWork.OfficeRepo.FindAsync(x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Culture == "CH");
            var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == orderId && !x.IsDelete && x.OrderType == order.Type);
            var listOrderExchage = await UnitOfWork.OrderExchangeRepo.Entities.Where(x => !x.IsDelete && x.OrderId == order.Id && x.Type != (byte)OrderExchangeType.Product).ToListAsync();
            var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == orderId && x.Type == (byte)OrderReasonType.Delay);
            var orderReasonNoCodeOfLading = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == orderId && x.Type == (byte)OrderReasonType.NoCodeOfLading);
            var orderReasonNotEnoughInventory = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == orderId && x.Type == (byte)OrderReasonType.NotEnoughInventory);
            //var listShop = await UnitOfWork.ShopRepo.Entities.ToListAsync();

            var mess = "";
            //check shop đã được đặt hàng trong vòng 2 ngày này ko?
            var dateTwo = DateTime.Now.AddDays(-2);
            var listOrderShop = await UnitOfWork.OrderRepo.FindAsync(x => !x.IsDelete && x.ShopId == order.ShopId && x.Created > dateTwo && x.ShopId != 0 && x.Id != order.Id && x.Status >= (byte)OrderStatus.OrderSuccess);
            var count = listOrderShop.Count;

            if (count > 0 && order.ShopId != 0)
            {
                mess = "Warning: Shop has had orders within 2 recent days";
            }

            //6. Gửi thông tin lên view
            return Json(new { status, msg, listOrderService, toAddress, formAddress, customer, orderExchange, userOrder, order, listWarehouse, listContractCode, mess, listOrderExchage, orderReason, listOrderShop, orderReasonNoCodeOfLading, orderReasonNotEnoughInventory, /*listShop*/ }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Lấy lịch sử thay đổi trạng thái, lịch sử thao tác
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetOrderHistory(int orderId)
        {
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            var listHistory = await UnitOfWork.OrderHistoryRepo.FindAsync(x => x.OrderId == orderId && x.Type == order.Type, query => query.OrderByDescending(m => m.CreateDate), null);
            var listLog = await UnitOfWork.OrderLogRepo.FindAsync(x => x.OrderId == orderId);
            listLog = listLog.OrderByDescending(x => x.CreateDate).ToList();

            return Json(new { listHistory, listLog, }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy danh sách order detail
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetOrderListDetail(int orderId)
        {
            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == orderId && !x.IsDelete, x => x.OrderBy(y => y.Id), null);
            return Json(new { listOrderDetail, }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrderListPackage(int orderId)
        {
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            var listPackageView = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == orderId && !x.IsDelete && x.OrderType == order.Type);
            return Json(new { listPackageView, }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy order detail]

        public async Task<JsonResult> Reason(int id, byte orderReasonId, string orderReasonText)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id);

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var dataBefore = new List<LogResult>();
                    var dataAfter = new List<LogResult>();

                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "Reason for delayed processing", Value = orderReason.Reason });

                    orderReason.ReasonId = orderReasonId;
                    orderReason.Reason = orderReasonId == (byte)OrderReasons.TheOtherReason ? orderReasonText : EnumHelper.GetEnumDescription<OrderReasons>(orderReasonId);
                    orderReason.CreateDate = orderReason.CreateDate ?? timeNow;

                    dataAfter.Add(new LogResult() { Name = "Reason for delayed processing", Value = orderReason.Reason });

                    await UnitOfWork.OrderReasonRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit reason for order delayed processing",
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
            return Json(new { status = MsgType.Success, msg = "Edit reason for order delayed processing successfully!", orderReason }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ReasonNoCodeOfLading(int id, byte orderReasonId, string orderReasonText)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id);

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var dataBefore = new List<LogResult>();
                    var dataAfter = new List<LogResult>();

                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "Reason: There has not been waybill code for more than 3 days", Value = orderReason.Reason });

                    orderReason.ReasonId = orderReasonId;
                    orderReason.Reason = EnumHelper.GetEnumDescription<OrderReasonNoCodeOfLading>(orderReasonId);
                    orderReason.CreateDate = orderReason.CreateDate ?? timeNow;

                    dataAfter.Add(new LogResult() { Name = "Reason: There has not been waybill code for more than 3 days", Value = orderReason.Reason });

                    await UnitOfWork.OrderReasonRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit reason of there has not been waybill code for more than 3 days",
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
            return Json(new { status = MsgType.Success, msg = "Edit reason of there has not been waybill code for more than 3 days successfully!", orderReason }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ReasonNotEnoughInventory(int id, byte orderReasonId, string orderReasonText)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id);

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var dataBefore = new List<LogResult>();
                    var dataAfter = new List<LogResult>();

                    //cập nhật thông tin Order
                    dataBefore.Add(new LogResult() { Name = "Reason: There have not been enough goods packages coming into warehouse for more than 4 days", Value = orderReason.Reason });

                    orderReason.ReasonId = orderReasonId;
                    orderReason.Reason = EnumHelper.GetEnumDescription<OrderReasonNotEnoughInventory>(orderReasonId);
                    orderReason.CreateDate = orderReason.CreateDate ?? timeNow;

                    dataAfter.Add(new LogResult() { Name = "Reason: There have not been enough goods packages coming into warehouse for more than 4 days", Value = orderReason.Reason });

                    await UnitOfWork.OrderReasonRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit reason of there have not been enough goods packages coming into warehouse for more than 4 days",
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
            return Json(new { status = MsgType.Success, msg = "Edit reason of there have not been enough goods packages coming into warehouse for more than 4 days successfully!", orderReason }, JsonRequestBehavior.AllowGet);
        }

        #endregion [2. Các thao tác với Order]

        #region [3. Các thao tác với order detail]

        #region [Phần xử lý đặt được order detail]

        /// <summary>
        /// Cập nhật lại thông tin hàng hóa trong Order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderWait, EnumPage.OrderCustomerCare)]
        public async Task<JsonResult> OrderDetailSuccess(int id)
        {
            var timeNow = DateTime.Now;

            //1. Lấy thông tin Order
            var orderDetail = UnitOfWork.OrderDetailRepo.FirstOrDefault(x => !x.IsDelete && x.Id == id);
            if (orderDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    orderDetail.LastUpdate = timeNow;
                    //orderDetail.QuantityBooked = orderDetail.Quantity;
                    //orderDetail.QuantityActuallyReceived = orderDetail.Quantity;

                    orderDetail.Status = (byte)OrderDetailStatus.Order;
                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //Cập nhật Order
                    var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderDetail.OrderId && !x.IsDelete);

                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                          .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                          .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                          .SumAsync(x => x.TotalExchange) : 0;

                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalPrice) : 0;

                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.QuantityBooked.Value) : 0;

                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .CountAsync() : 0;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Cập nhật lại giá khi Edit số lượng sản phẩm
                    if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                    {
                        var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
                            .ToListAsync();

                        var totalQuantity = productDetails.Sum(x => x.QuantityBooked);

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
                                pd.AuditPrice = OrderRepository.OrderAudit(order.ProductNo, pd.Price);
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }
                    }

                    //--------- Cập nhật lại giá dịch vụ -----------
                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Order
                    var totalExchange = order.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                    // Order nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (order.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                    //if (service != null)
                    //{
                    //    service.LastUpdate = timeNow;
                    //    service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                    //    service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;
                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    //    // Triết khấu phí mua hàng
                    //    var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
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
                               x => x.OrderId == order.Id && !x.IsDelete &&
                                   x.ServiceId == (byte)OrderServices.Audit && x.Checked);

                    if (orderAuditService != null)
                    {
                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                    && x.Status == (byte)OrderDetailStatus.Order)
                                .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        orderAuditService.Value = totalAuditPrice;
                        orderAuditService.TotalPrice = totalAuditPrice;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    // Cập nhật Total order amount
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.LastUpdate = timeNow;
                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Has ordered product link: <a href='{orderDetail.Link}'>{orderDetail.Link}</a>",
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

            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == orderDetail.OrderId && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Action performed successfully!", listOrderService }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Phần xử lý đặt được order detail]

        #region [Phần xử lý không đặt được order detail]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderWait, EnumPage.OrderCustomerCare)]
        public async Task<JsonResult> OrderDetailCancel(int id)
        {
            var timeNow = DateTime.Now;

            var orderDetail = UnitOfWork.OrderDetailRepo.FirstOrDefault(x => !x.IsDelete && x.Id == id);

            if (orderDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            var countOrderDetail = await UnitOfWork.OrderDetailRepo.CountAsync(x => !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order && x.OrderId == orderDetail.OrderId);
            if (countOrderDetail == 1)
            {
                return Json(new { status = MsgType.Error, msg = "Can not perform this action because the order can only be added 1 more link!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    orderDetail.LastUpdate = timeNow;
                    orderDetail.QuantityBooked = 0;
                    orderDetail.TotalPrice = 0;
                    orderDetail.TotalExchange = 0;
                    orderDetail.Status = (byte)OrderDetailStatus.Cancel;
                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //cập nhật Order
                    var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderDetail.OrderId && !x.IsDelete);

                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalExchange) : 0;

                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.TotalPrice) : 0;

                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .SumAsync(x => x.QuantityBooked.Value) : 0;

                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order).AnyAsync() ? await UnitOfWork.OrderDetailRepo.Entities
                           .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order)
                           .CountAsync() : 0;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Cập nhật lại giá khi Edit số lượng sản phẩm
                    if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                    {
                        var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
                            .ToListAsync();

                        var totalQuantity = productDetails.Sum(x => x.QuantityBooked ?? 0);

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
                                pd.AuditPrice = OrderRepository.OrderAudit(order.ProductNo, pd.Price);
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }
                    }

                    //--------- Cập nhật lại giá dịch vụ -----------
                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Order
                    var totalExchange = order.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                    // Order nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (order.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                    //if (service != null)
                    //{
                    //    service.LastUpdate = timeNow;
                    //    service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                    //    service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;
                    //    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    //    // Triết khấu phí mua hàng
                    //    var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
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
                               x => x.OrderId == order.Id && !x.IsDelete &&
                                   x.ServiceId == (byte)OrderServices.Audit && x.Checked);

                    if (orderAuditService != null)
                    {
                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                    && x.Status == (byte)OrderDetailStatus.Order)
                                .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        orderAuditService.Value = totalAuditPrice;
                        orderAuditService.TotalPrice = totalAuditPrice;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }


                    // Cập nhật Total order amount
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.LastUpdate = timeNow;
                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Cannot add product link: <a href='{orderDetail.Link}'>{orderDetail.Link}</a>",
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
            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == orderDetail.OrderId && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "This action has been performed successfully", listOrderService }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Phần xử lý không đặt được order detail]

        #region [Cập nhật Detail sản phẩm]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> OrderDetailUpdate(OrderDetail orderDetailMode)
        {
            var timeNow = DateTime.Now;

            //1. Lấy thông tin order detail
            var orderDetail = UnitOfWork.OrderDetailRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderDetailMode.Id);

            if (orderDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked > orderDetail.Quantity)
            {
                return Json(new { status = MsgType.Error, msg = "Number of ordered links cannot be greater than the number ordered by customers!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ordered number must be greater than 0!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked != null && orderDetailMode.QuantityBooked.Value <= 0)
            {
                return Json(new { status = MsgType.Error, msg = "Ordered number must be greater than 0!" }, JsonRequestBehavior.AllowGet);
            }

            //Tính toán lại Total order amount
            var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderDetail.OrderId && !x.IsDelete);

            var listOrderDetail = await UnitOfWork.OrderDetailRepo.Entities.Where(x => !x.IsDelete && x.OrderId == order.Id && x.Id != orderDetailMode.Id && x.Status != (byte)OrderDetailStatus.Cancel).ToListAsync();

            if ((listOrderDetail.Sum(x => x.TotalPrice) + orderDetailMode.QuantityBooked * orderDetailMode.Price) < order.PaidShop)
            {
                return Json(new { status = MsgType.Error, msg = "Total amount of money paid by customer cannot be greater than cost of goods paid by company!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Cập nhật thông tin
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                var checkEditQuantity = false;
                try
                {
                    var str = "";
                    if (orderDetail.Price != orderDetailMode.Price)
                    {
                        str += $" Price from {orderDetail.Price:N2} into {orderDetailMode.Price:N2}";
                    }

                    if (orderDetail.QuantityBooked != orderDetailMode.QuantityBooked)
                    {
                        checkEditQuantity = true;
                        str += $" Quantity from {orderDetail.QuantityBooked:N0} into {orderDetailMode.QuantityBooked:N0}";
                    }

                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    //cập nhật thông tin Order

                    if (DataCompare(orderDetail.Price, orderDetailMode.Price))
                    {
                        dataBefore.Add(new LogResult() { Name = "Price", Value = orderDetail.Price.ToString("N2") });
                        //cập nhật order detail
                        orderDetail.Price = orderDetailMode.Price;
                        dataAfter.Add(new LogResult() { Name = "Price", Value = orderDetail.Price.ToString("N2") });
                    }
                    if (DataCompare(orderDetail.QuantityBooked, orderDetailMode.QuantityBooked))
                    {
                        dataBefore.Add(new LogResult() { Name = "Ordered quantity", Value = orderDetail.QuantityBooked == null ? "0" : orderDetail.QuantityBooked.Value.ToString("N0") });
                        orderDetail.QuantityBooked = orderDetailMode.QuantityBooked;
                        dataAfter.Add(new LogResult() { Name = "Ordered quantity", Value = orderDetail.QuantityBooked == null ? "0" : orderDetail.QuantityBooked.Value.ToString("N0") });
                    }
                    if (DataCompare(orderDetail.UserNote, orderDetailMode.UserNote))
                    {
                        dataBefore.Add(new LogResult() { Name = "Note", Value = orderDetail.UserNote });
                        orderDetail.UserNote = orderDetailMode.UserNote;
                        dataAfter.Add(new LogResult() { Name = "Note", Value = orderDetail.UserNote });
                    }

                    if (str != "")
                    {
                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            OrderId = order.Id,
                            SystemId = order.SystemId,
                            SystemName = order.SystemName,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            CreateDate = timeNow,
                            UpdateDate = timeNow,
                            OrderType = 1, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = "Edit information of order detail",
                            Description = $"Edit information of product {orderDetail.Name}: {str}"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);
                        await UnitOfWork.NotificationRepo.SaveAsync();
                    }

                    //Tính toán lại tiền số tiền
                    if (orderDetail.QuantityBooked != null)
                    {
                        orderDetail.TotalPrice = orderDetail.Price * (orderDetail.QuantityBooked ?? 0);
                        orderDetail.TotalExchange = orderDetail.TotalPrice * orderDetail.ExchangeRate;
                        orderDetail.AuditPrice = OrderRepository.OrderAudit(orderDetail.QuantityBooked.Value, orderDetail.Price);

                        //Cập nhật thời gian thay đổi.
                        orderDetail.LastUpdate = timeNow;
                        await UnitOfWork.OrderDetailRepo.SaveAsync();

                        //Tính lại số lượng sản phẩm
                        order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                            .SumAsync(x => x.QuantityBooked.Value);

                        // Cập nhật lại giá khi Edit số lượng sản phẩm
                        if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                        {
                            var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => x.OrderId == order.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
                                .ToListAsync();

                            var totalQuantity = productDetails.Sum(x => x.QuantityBooked);

                            var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(orderDetail.Prices);

                            var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin) || (totalQuantity >= x.Begin && totalQuantity <= x.End));

                            if (price != null)
                            {
                                foreach (var pd in productDetails)
                                {
                                    if (checkEditQuantity)
                                    {
                                        pd.Price = price.Price;
                                        pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                    }
                                    if (pd.QuantityBooked == null) continue;
                                    pd.TotalPrice = pd.Price * pd.QuantityBooked.Value;
                                    pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.QuantityBooked.Value;
                                    pd.AuditPrice = OrderRepository.OrderAudit(order.ProductNo, pd.Price);
                                }

                                await UnitOfWork.OrderDetailRepo.SaveAsync();
                            }
                        }

                        //Tính lại Total money việt nam
                        order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                            .SumAsync(x => x.TotalExchange);

                        //Tính lại Total money ngoại yuan
                        order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                            .SumAsync(x => x.TotalPrice);

                        //Tính lại số linh sản phẩm
                        order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                            .CountAsync();

                        await UnitOfWork.OrderRepo.SaveAsync();

                        //--------- Cập nhật lại giá dịch vụ -----------
                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Order
                        var totalExchange = order.TotalExchange;
                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                        // Order nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        if (order.TotalExchange < 2000000)
                        {
                            totalPrice = 150000;
                        }

                        //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                        //service.LastUpdate = timeNow;
                        //service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    service.TotalPrice -= service.TotalPrice * discount / 100;
                        //    service.Note = $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // Cập nhật dịch vụ kiểm đếm
                        var serviceAudit = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.ServiceId == (byte)OrderServices.Audit && x.OrderId == order.Id && !x.IsDelete);

                        serviceAudit.LastUpdate = timeNow;

                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                            && x.Status == (byte)OrderDetailStatus.Order)
                                .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                        serviceAudit.Value = totalAuditPrice;
                        serviceAudit.TotalPrice = totalAuditPrice;
                    }

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật Total order amount
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.LastUpdate = timeNow;
                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = $"Edit information of product detail link: <a href='{orderDetail.Link}'>See</a>",
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

            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == orderDetail.OrderId && !x.IsDelete);
            listOrderDetail = await UnitOfWork.OrderDetailRepo.Entities.Where(x => !x.IsDelete && x.OrderId == order.Id).ToListAsync();

            return Json(new { status = MsgType.Success, msg = "Update successfully!", listOrderService, listOrderDetail }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm cập nhật order detail chờ báo giá
        /// </summary>
        /// <param name="orderDetailMode">OrderDetail Model</param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderWait, EnumPage.OrderCustomerCare)]
        public async Task<JsonResult> UpdateOrderDetailAwait(OrderDetail orderDetailMode)
        {
            var timeNow = DateTime.Now;

            //1. Lấy thông tin order detail
            var orderDetail = UnitOfWork.OrderDetailRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderDetailMode.Id);

            if (orderDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked > orderDetailMode.Quantity)
            {
                return Json(new { status = MsgType.Error, msg = "Number of ordered links cannot be larger than the number ordered by customers!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked == null)
            {
                return Json(new { status = MsgType.Error, msg = "Ordered number must be greater than 0!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.QuantityBooked != null && orderDetailMode.QuantityBooked.Value <= 0)
            {
                return Json(new { status = MsgType.Error, msg = "Ordered number must be greater than 0!" }, JsonRequestBehavior.AllowGet);
            }

            if (orderDetailMode.Quantity <= 0)
            {
                return Json(new { status = MsgType.Error, msg = "Quantity ordered by customer must be greater than 0!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Cập nhật thông tin
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var str = "";
                    var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderDetail.OrderId && !x.IsDelete);

                    if (orderDetail.Price != orderDetailMode.Price)
                    {
                        str += $" Price from {orderDetail.Price:N2} into {orderDetailMode.Price:N2}";
                    }

                    if (orderDetail.QuantityBooked != orderDetailMode.QuantityBooked)
                    {
                        str += $" Quantity from {orderDetail.QuantityBooked:N0} into {orderDetailMode.QuantityBooked:N0}";
                    }

                    if (orderDetail.Quantity != orderDetailMode.Quantity)
                    {
                        str += $" Quantity ordered by customer from {orderDetail.Quantity:N0} into {orderDetailMode.Quantity:N0}";
                    }

                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    //cập nhật thông tin Order

                    if (DataCompare(orderDetail.Price, orderDetailMode.Price))
                    {
                        dataBefore.Add(new LogResult() { Name = "Price", Value = orderDetail.Price.ToString("N2") });
                        orderDetail.Price = orderDetailMode.Price;
                        dataAfter.Add(new LogResult() { Name = "Price", Value = orderDetail.Price.ToString("N2") });
                    }
                    if (DataCompare(orderDetail.QuantityBooked, orderDetailMode.QuantityBooked))
                    {
                        dataBefore.Add(new LogResult() { Name = "Ordered quantity", Value = orderDetail.QuantityBooked.Value.ToString("N0") });
                        orderDetail.QuantityBooked = orderDetailMode.QuantityBooked;
                        dataAfter.Add(new LogResult() { Name = "Ordered quantity", Value = orderDetail.QuantityBooked.Value.ToString("N0") });
                    }
                    if (DataCompare(orderDetail.Quantity, orderDetailMode.Quantity))
                    {
                        dataBefore.Add(new LogResult() { Name = "Quantity ordered by customer", Value = orderDetail.Quantity.ToString("N0") });
                        orderDetail.Quantity = orderDetailMode.Quantity;
                        dataAfter.Add(new LogResult() { Name = "Quantity ordered by customer", Value = orderDetail.Quantity.ToString("N0") });
                    }
                    if (DataCompare(orderDetail.UserNote, orderDetailMode.UserNote))
                    {
                        dataBefore.Add(new LogResult() { Name = "Note", Value = orderDetail.UserNote });
                        orderDetail.UserNote = orderDetailMode.UserNote;
                        dataAfter.Add(new LogResult() { Name = "Note", Value = orderDetail.UserNote });
                    }
                    if (DataCompare(orderDetail.Note, orderDetailMode.Note))
                    {
                        dataBefore.Add(new LogResult() { Name = "Customer's note", Value = orderDetail.Note });
                        orderDetail.Note = orderDetailMode.Note;
                        dataAfter.Add(new LogResult() { Name = "Customer's note", Value = orderDetail.Note });
                    }
                    if (DataCompare(orderDetail.Color, orderDetailMode.Color))
                    {
                        dataBefore.Add(new LogResult() { Name = "Color", Value = orderDetail.Color });
                        orderDetail.Color = orderDetailMode.Color;
                        dataAfter.Add(new LogResult() { Name = "Color", Value = orderDetail.Color });
                    }
                    if (DataCompare(orderDetail.Size, orderDetailMode.Size))
                    {
                        dataBefore.Add(new LogResult() { Name = "Size", Value = orderDetail.Size });
                        orderDetail.Size = orderDetailMode.Size;
                        dataAfter.Add(new LogResult() { Name = "Size", Value = orderDetail.Size });
                    }
                    if (DataCompare(orderDetail.Name, orderDetailMode.Name))
                    {
                        dataBefore.Add(new LogResult() { Name = "Product name", Value = orderDetail.Name });
                        orderDetail.Name = orderDetailMode.Name;
                        dataAfter.Add(new LogResult() { Name = "Product name", Value = orderDetail.Name });
                    }
                    if (DataCompare(orderDetail.Link, orderDetailMode.Link))
                    {
                        dataBefore.Add(new LogResult() { Name = "Product link", Value = orderDetail.Link });
                        orderDetail.Link = orderDetailMode.Link;
                        dataAfter.Add(new LogResult() { Name = "Product link", Value = orderDetail.Link });
                    }
                    if (DataCompare(orderDetail.Image, orderDetailMode.Image))
                    {
                        dataBefore.Add(new LogResult() { Name = "Picture", Value = orderDetail.Image });
                        orderDetail.Image = orderDetailMode.Image;
                        dataAfter.Add(new LogResult() { Name = "Picture", Value = orderDetail.Image });
                    }

                    var shop = await UnitOfWork.ShopRepo.FirstOrDefaultAsync(x => x.Name == orderDetailMode.ShopName);
                    if (shop != null)
                    {
                        orderDetailMode.ShopId = shop.Id;
                        orderDetailMode.ShopName = shop.Name?.Trim() ?? "";
                        orderDetailMode.WebsiteName = shop.Website;
                        orderDetailMode.ShopLink = shop.Url;

                        orderDetail.ShopId = orderDetailMode.ShopId;
                        if (DataCompare(orderDetail.ShopName, orderDetailMode.ShopName))
                        {
                            dataBefore.Add(new LogResult() { Name = "Shop name", Value = orderDetail.ShopName });
                            orderDetail.ShopName = orderDetailMode.ShopName?.Trim() ?? "";
                            dataAfter.Add(new LogResult() { Name = "Shop name", Value = orderDetail.ShopName });
                        }
                        if (DataCompare(orderDetail.WebsiteName, orderDetailMode.WebsiteName))
                        {
                            dataBefore.Add(new LogResult() { Name = "Link shop", Value = orderDetail.WebsiteName });
                            orderDetail.WebsiteName = orderDetailMode.WebsiteName;
                            dataAfter.Add(new LogResult() { Name = "Link shop", Value = orderDetail.WebsiteName });
                        }
                        if (DataCompare(orderDetail.ShopLink, orderDetailMode.ShopLink))
                        {
                            dataBefore.Add(new LogResult() { Name = "Website", Value = orderDetail.ShopLink });
                            orderDetail.ShopLink = orderDetailMode.ShopLink;
                            dataAfter.Add(new LogResult() { Name = "Website", Value = orderDetail.ShopLink });
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(orderDetailMode.ShopName))
                        {
                            shop = new Shop()
                            {
                                Name = orderDetailMode.ShopName?.Trim() ?? "",
                                Url = orderDetailMode.Link,
                                Website = MyCommon.GetDomain(orderDetailMode.Link),
                                CreateDate = timeNow,
                                UpdateDate = timeNow,
                            };

                            UnitOfWork.ShopRepo.Add(shop);

                            // Submit thêm Shop
                            await UnitOfWork.ShopRepo.SaveAsync();

                            orderDetailMode.ShopId = shop.Id;
                            orderDetailMode.ShopName = shop.Name?.Trim() ?? "";
                            orderDetailMode.WebsiteName = shop.Website;
                            orderDetailMode.ShopLink = shop.Url;

                            orderDetail.ShopId = orderDetailMode.ShopId;
                            if (DataCompare(orderDetail.ShopName, orderDetailMode.ShopName))
                            {
                                dataBefore.Add(new LogResult() { Name = "shop name", Value = orderDetail.ShopName });
                                orderDetail.ShopName = orderDetailMode.ShopName?.Trim() ?? "";
                                dataAfter.Add(new LogResult() { Name = "shop name", Value = orderDetail.ShopName });
                            }
                            if (DataCompare(orderDetail.WebsiteName, orderDetailMode.WebsiteName))
                            {
                                dataBefore.Add(new LogResult() { Name = "Link shop", Value = orderDetail.WebsiteName });
                                orderDetail.WebsiteName = orderDetailMode.WebsiteName;
                                dataAfter.Add(new LogResult() { Name = "Link shop", Value = orderDetail.WebsiteName });
                            }
                            if (DataCompare(orderDetail.ShopLink, orderDetailMode.ShopLink))
                            {
                                dataBefore.Add(new LogResult() { Name = "Website", Value = orderDetail.ShopLink });
                                orderDetail.ShopLink = orderDetailMode.ShopLink;
                                dataAfter.Add(new LogResult() { Name = "Website", Value = orderDetail.ShopLink });
                            }
                        }
                    }

                    if (str != "")
                    {
                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            OrderId = order.Id,
                            SystemId = order.SystemId,
                            SystemName = order.SystemName,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            CreateDate = timeNow,
                            UpdateDate = timeNow,
                            OrderType = 1, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = "Edit information of order detail",
                            Description = $"Edit information of product {orderDetail.Name}: {str}"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);
                        await UnitOfWork.NotificationRepo.SaveAsync();
                    }

                    //Tính toán lại tiền số tiền
                    orderDetail.TotalPrice = orderDetail.Price * orderDetail.QuantityBooked.Value;
                    orderDetail.TotalExchange = orderDetail.TotalPrice * orderDetail.ExchangeRate;
                    //orderDetail.AuditPrice = OrderRepository.OrderAudit(orderDetail.QuantityBooked.Value, orderDetail.Price);

                    //Cập nhật thời gian thay đổi.
                    orderDetail.LastUpdate = timeNow;
                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //Tính lại số lượng sản phẩm
                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities.Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel).SumAsync(x => x.QuantityBooked.Value);

                    // Cập nhật lại giá khi Edit số lượng sản phẩm
                    if (!string.IsNullOrEmpty(orderDetail.Prices) && !string.IsNullOrWhiteSpace(orderDetail.ProId))
                    {
                        var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && x.IsDelete == false && x.ProId == orderDetail.ProId)
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
                                pd.TotalPrice = pd.Price * pd.QuantityBooked.Value;
                                pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.QuantityBooked.Value;
                                pd.AuditPrice = OrderRepository.OrderAudit(order.ProductNo, pd.Price);
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }
                    }

                    //Tính lại Total money việt nam
                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalExchange);

                    //Tính lại Total money ngoại yuan
                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalPrice);

                    //Tính lại số linh sản phẩm
                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .CountAsync();

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //--------- Cập nhật lại giá dịch vụ -----------
                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho Order
                    var totalExchange = order.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                    // Order nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (order.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                    //service.LastUpdate = timeNow;
                    //service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                    //service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                    //// Triết khấu phí mua hàng
                    //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId).Order;
                    //if (discount > 0)
                    //{
                    //    service.TotalPrice -= service.TotalPrice * discount / 100;
                    //    service.Note =
                    //        $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                    //}

                    // Cập nhật dịch vụ kiểm đếm
                    var serviceAudit = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        x => x.ServiceId == (byte)OrderServices.Audit && x.OrderId == order.Id && !x.IsDelete);

                    serviceAudit.LastUpdate = timeNow;

                    var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                    && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.AuditPrice.Value * x.QuantityBooked.Value);

                    serviceAudit.Value = totalAuditPrice;
                    serviceAudit.TotalPrice = totalAuditPrice;

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật Total order amount
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.LastUpdate = timeNow;
                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = $"Edit product detail: <a href='{orderDetail.Link}'>See</a>",
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

            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == orderDetail.OrderId && !x.IsDelete);
            var listOrderDetail = await UnitOfWork.OrderDetailRepo.Entities.Where(x => !x.IsDelete && x.OrderId == orderDetail.OrderId).ToListAsync();

            return Json(new { status = MsgType.Success, msg = "Updated successfully!", listOrderService, listOrderDetail }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Cập nhật Detail sản phẩm]

        #endregion [3. Các thao tác với order detail]

        #region [4. Thao tác với dịch vụ Order]

        #region [Cập nhật dịch vụ Order]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderWait, EnumPage.OrderCustomerCare, EnumPage.OrderCommerce)]
        public async Task<JsonResult> UpdateService(int id)
        {
            var rs = 0;
            var timeNow = DateTime.Now;

            var orderServiceSet = UnitOfWork.OrderServiceRepo.FirstOrDefault(x => !x.IsDelete && x.Id == id);

            if (orderServiceSet == null)
            {
                rs = -1;
            }
            else
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var order = UnitOfWork.OrderRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderServiceSet.OrderId);

                        orderServiceSet.Checked = !orderServiceSet.Checked;
                        orderServiceSet.LastUpdate = timeNow;

                        ////Kiểm tra dịch vụ bay nhanh
                        //if (order.WarehouseDeliveryId == 8)
                        //{
                        //    if (orderServiceSet.ServiceId == (byte)OrderServices.FastDelivery)
                        //    {
                        //        orderServiceSet.Checked = false;
                        //    }
                        //}

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        //tính lại tiền dịch vụ

                        // Cập nhật Total order amount
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.LastUpdate = timeNow;
                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        await UnitOfWork.OrderRepo.SaveAsync();

                        var str = orderServiceSet.Checked ? "sử dụng" : "ngừng sử dụng";
                        //Ghi log thao tác
                        var orderLog = new OrderLog
                        {
                            OrderId = order.Id,
                            CreateDate = timeNow,
                            Type = (byte)OrderLogType.Acction,
                            DataBefore = null,
                            DataAfter = null,
                            Content = $"Update state of order service: {orderServiceSet.ServiceName} into {str}",
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            UserOfficeId = UserState.OfficeId,
                            UserOfficeName = UserState.OfficeName,
                            UserOfficePath = UserState.OfficeIdPath
                        };
                        UnitOfWork.OrderLogRepo.Add(orderLog);
                        await UnitOfWork.OrderLogRepo.SaveAsync();

                        //OrderService serviceUpdate = null;

                        //// Bỏ dịch vụ bay nhanh
                        //if (orderServiceSet.ServiceId == (byte)OrderServices.Optimal && orderServiceSet.Checked)
                        //{
                        //    serviceUpdate = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => x.ServiceId == (byte)OrderServices.FastDelivery && x.OrderId == orderServiceSet.OrderId && !x.IsDelete && x.Checked);
                        //}
                        //else if (orderServiceSet.ServiceId == (byte)OrderServices.FastDelivery && orderServiceSet.Checked)
                        //{
                        //    // Bỏ dịch vụ VC tiết kiệm
                        //    serviceUpdate = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x =>
                        //            x.ServiceId == (byte)OrderServices.Optimal && x.OrderId == orderServiceSet.OrderId &&
                        //            !x.IsDelete && x.Checked);
                        //}

                        //if (serviceUpdate != null)
                        //{
                        //    serviceUpdate.Checked = false;
                        //    serviceUpdate.LastUpdate = DateTime.Now;

                        //    Ghi log thao tác
                        //    UnitOfWork.OrderLogRepo.Add(new OrderLog
                        //    {
                        //        OrderId = order.Id,
                        //        CreateDate = timeNow,
                        //        Type = (byte)OrderLogType.Acction,
                        //        DataBefore = null,
                        //        DataAfter = null,
                        //        Content = $"Cập nhật trạng thái dịch vụ Order: {serviceUpdate.ServiceName} thành ngừng sử dụng",
                        //        UserId = UserState.UserId,
                        //        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        //        UserOfficeId = UserState.OfficeId,
                        //        UserOfficeName = UserState.OfficeName,
                        //        UserOfficePath = UserState.OfficeIdPath
                        //    });
                        //    await UnitOfWork.OrderLogRepo.SaveAsync();
                        //}

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ExceptionDispatchInfo.Capture(ex).Throw();
                        throw;
                    }
                }
            }

            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        #endregion [Cập nhật dịch vụ Order]

        #endregion [4. Thao tác với dịch vụ Order]

        #region [5. Actions with waybill code]

        //Add the tracking code into order
        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet, EnumPage.OrderCommerce)]
        public async Task<JsonResult> AddContractCode(int id, byte? mode, int? packageId, string codePackage, string note)
        {
            var timeNow = DateTime.Now;

            if (!string.IsNullOrEmpty(codePackage))
            {
                codePackage = codePackage.Trim();
            }

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var checkContractCode = await
                UnitOfWork.OrderContractCodeRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && x.OrderId == order.Id && x.Status == (byte)ContractCodeType.AwaitingPayment);
            if (checkContractCode.Count > 0)
            {
                return Json(new { status = MsgType.Error, msg = "There is code of contract waiting for paymnent, cannot add the tracking code!" }, JsonRequestBehavior.AllowGet);
            }

            var checkOrderPackage =
                await
                    UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(
                        x => !x.IsDelete && x.OrderId == order.Id && x.TransportCode == codePackage);

            if (checkOrderPackage != null)
            {
                return Json(new { status = MsgType.Error, msg = $" Waybill code coincided '#{codePackage}'!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (order.Status == (byte)OrderStatus.OrderSuccess)
                    {

                        //shop trung quốc phát hàng
                        if (mode == null || packageId == null)
                        {
                            if (order.PackageNo == 0)
                            {
                                if (order.Type == (byte)OrderType.Deposit)
                                {
                                    if (order.Status < (byte)DepositStatus.InWarehouse)
                                    {
                                        order.Status = (byte)DepositStatus.InWarehouse;
                                        order.LastUpdate = timeNow;

                                        await UnitOfWork.OrderRepo.SaveAsync();

                                        // Thêm lịch sử thay đổi trạng thái
                                        if (order.CustomerId != null)
                                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                            {
                                                CreateDate = timeNow,
                                                Content = "Receive goods",
                                                CustomerId = order.CustomerId.Value,
                                                CustomerName = order.CustomerName,
                                                OrderId = order.Id,
                                                Status = order.Status,
                                                UserId = UserState.UserId,
                                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                                Type = order.Type
                                            });

                                        await UnitOfWork.OrderHistoryRepo.SaveAsync();
                                    }
                                }
                                else
                                {
                                    if (order.Status < (byte)OrderStatus.InWarehouse)
                                    {
                                        order.Status = (byte)OrderStatus.DeliveryShop;
                                        order.LastUpdate = timeNow;

                                        await UnitOfWork.OrderRepo.SaveAsync();

                                        // Thêm lịch sử thay đổi trạng thái
                                        if (order.CustomerId != null)
                                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                            {
                                                CreateDate = timeNow,
                                                Content = "Shop dispatching goods",
                                                CustomerId = order.CustomerId.Value,
                                                CustomerName = order.CustomerName,
                                                OrderId = order.Id,
                                                Status = order.Status,
                                                UserId = UserState.UserId,
                                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                                Type = order.Type
                                            });

                                        await UnitOfWork.OrderHistoryRepo.SaveAsync();
                                    }
                                }
                            }
                        }
                        else
                        {
                            order.Status = (byte)OrderStatus.DeliveryShop;
                            order.LastUpdate = timeNow;

                            await UnitOfWork.OrderRepo.SaveAsync();

                            // Thêm lịch sử thay đổi trạng thái
                            if (order.CustomerId != null)
                                UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                {
                                    CreateDate = timeNow,
                                    Content = "Shop dispatching goods",
                                    CustomerId = order.CustomerId.Value,
                                    CustomerName = order.CustomerName,
                                    OrderId = order.Id,
                                    Status = order.Status,
                                    UserId = UserState.UserId,
                                    UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                    Type = order.Type
                                });

                            await UnitOfWork.OrderHistoryRepo.SaveAsync();
                        }
                    }

                    //Tạo mã vận đơn
                    //lấy thông tin khách hàng
                    var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == order.CustomerId.Value);
                    if (customer == null) //does not exist Order hoặc Order bị xóa
                    {
                        return Json(new { status = MsgType.Error, msg = "There is no customer information!" }, JsonRequestBehavior.AllowGet);
                    }

                    //lấy thông tin kho nguồn
                    var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == order.WarehouseId);

                    if (warehouseStart == null)
                    {
                        return Json(new { status = MsgType.Error, msg = "Receiving China warehouse has not been selected!" }, JsonRequestBehavior.AllowGet);
                    }

                    //lấy thông tin kho đích
                    var warehouseEnd = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == order.WarehouseDeliveryId);

                    if (warehouseEnd == null)
                    {
                        return Json(new { status = MsgType.Error, msg = "Customer has not selected warehouse in Vietnam !" }, JsonRequestBehavior.AllowGet);
                    }

                    var listService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == order.Id && x.Mode == (byte)OrderServiceMode.Option && x.Checked && !x.IsDelete);

                    OrderPackage orderPackage;

                    if (mode == null || packageId == null)
                    {
                        orderPackage = new OrderPackage()
                        {
                            Code = string.Empty,
                            Status = (byte)OrderPackageStatus.ShopDelivery,
                            OrderId = order.Id,
                            OrderCode = order.Code,
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            CustomerUserName = customer.Email,
                            CustomerLevelId = customer.LevelId,
                            CustomerLevelName = customer.LevelName,
                            CustomerWarehouseId = warehouseEnd.Id,
                            CustomerWarehouseName = warehouseEnd.Name,
                            CustomerWarehouseIdPath = warehouseEnd.IdPath,
                            CustomerWarehouseAddress = warehouseEnd.Address,
                            //CurrentWarehouseId = UserState.OfficeId,
                            //CurrentWarehouseIdPath = UserState.OfficeIdPath,
                            //CurrentWarehouseName = UserState.OfficeName,
                            //CurrentWarehouseAddress = UserState.OfficeAddress,
                            TransportCode = codePackage,
                            WarehouseId = warehouseStart.Id,
                            WarehouseName = warehouseStart.Name,
                            WarehouseIdPath = warehouseStart.IdPath,
                            WarehouseAddress = warehouseStart.Address,
                            SystemId = order.SystemId,
                            SystemName = order.SystemName,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            HashTag = string.Empty,
                            PackageNo = 0,
                            UnsignedText =
                                MyCommon.Ucs2Convert(
                                    $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.UserFullName} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone} {codePackage} {order.ShopName}"),
                            ForcastDate = DateTime.Now.AddDays(2),
                            OrderType = order.Type,
                            Note = note,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName
                        };

                        UnitOfWork.OrderPackageRepo.Add(orderPackage);
                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //cập nhật lại mã code
                        var orderPackageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                                     x.Created.Year == orderPackage.Created.Year && x.Created.Month == orderPackage.Created.Month &&
                                     x.Created.Day == orderPackage.Created.Day && x.Id <= orderPackage.Id);
                        orderPackage.Code = $"{orderPackageOfDay}{orderPackage.Created:ddMMyy}";
                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //Check trùng mã vận đơn
                        var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == codePackage && x.OrderId > 0);

                        if (checkPackageCount > 1)
                        {
                            PackageJob.UpdateSameTransportCode(orderPackage.TransportCode, orderPackage.Code, UnitOfWork, UserState);
                        }
                    }
                    else
                    {
                        orderPackage = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageId && x.IsDelete == false && x.HashTag == ";packagelose;");

                        if (orderPackage == null)
                            return Json(new { status = -1, msg = "This code-missing package does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                        orderPackage.HashTag = string.Empty;
                        orderPackage.Status = (byte)OrderPackageStatus.ShopDelivery;
                        orderPackage.OrderId = order.Id;
                        orderPackage.OrderCode = order.Code;
                        orderPackage.CustomerId = customer.Id;
                        orderPackage.CustomerName = customer.FullName;
                        orderPackage.CustomerUserName = customer.Email;
                        orderPackage.CustomerLevelId = customer.LevelId;
                        orderPackage.CustomerLevelName = customer.LevelName;
                        orderPackage.CustomerWarehouseId = warehouseEnd.Id;
                        orderPackage.CustomerWarehouseName = warehouseEnd.Name;
                        orderPackage.CustomerWarehouseIdPath = warehouseEnd.IdPath;
                        orderPackage.CustomerWarehouseAddress = warehouseEnd.Address;
                        orderPackage.TransportCode = codePackage;
                        orderPackage.WarehouseId = warehouseStart.Id;
                        orderPackage.WarehouseName = warehouseStart.Name;
                        orderPackage.WarehouseIdPath = warehouseStart.IdPath;
                        orderPackage.WarehouseAddress = warehouseStart.Address;
                        orderPackage.SystemId = order.SystemId;
                        orderPackage.SystemName = order.SystemName;
                        orderPackage.Created = timeNow;
                        orderPackage.LastUpdate = timeNow;
                        orderPackage.HashTag = string.Empty;
                        orderPackage.PackageNo = 0;
                        orderPackage.UnsignedText =
                            MyCommon.Ucs2Convert(
                                $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.UserFullName} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone} {codePackage} {order.ShopName}");
                        orderPackage.ForcastDate = DateTime.Now.AddDays(2);
                        orderPackage.OrderType = order.Type;
                        orderPackage.Note = note;
                        orderPackage.UserId = UserState.UserId;
                        orderPackage.UserFullName = UserState.FullName;
                    }

                    if (listService.Any())
                    {
                        orderPackage.OrderServices = string.Join(", ", listService.Select(x => x.ServiceName.Replace("Phí dịch", "Dịch")).ToList());
                    }

                    orderPackage.UnsignedText += MyCommon.Ucs2Convert($"{orderPackage.Code} {order.Code} {MyCommon.ReturnCode(order.Code)} P{orderPackage.Code} {orderPackage.WarehouseName}").ToLower();

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Ghi chú toàn system cho package, Order
                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                          x =>
                          x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                          x.Mode == (byte)PackageNoteMode.Order);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = orderPackage.OrderId,
                            OrderCode = orderPackage.OrderCode,
                            PackageId = orderPackage.Id,
                            PackageCode = orderPackage.Code,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = null,
                            ObjectCode = string.Empty,
                            Mode = (byte)PackageNoteMode.Order,
                            Content = orderPackage.Note
                        });
                    }
                    else if (packageNote != null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        packageNote.Content = orderPackage.Note;
                    }
                    else if (packageNote != null && string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Remove(packageNote);
                    }

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = orderPackage.Id,
                        PackageCode = orderPackage.Code,
                        OrderId = order.Id,
                        OrderCode = order.Code,
                        Type = order.Type,
                        Status = (int)OrderPackageStatus.ShopDelivery,
                        Content = EnumHelper.GetEnumDescription(OrderPackageStatus.ShopDelivery),
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                    await UnitOfWork.PackageHistoryRepo.SaveAsync();

                    // Thêm thông tin vào hàng mất REQUEST CODE đặt hàng duyệt
                    if (mode != null)
                    {
                        // Tạo phiếu Hàng mất mã
                        var packageNoCode = new PackageNoCode()
                        {
                            Created = timeNow,
                            CreateOfficeId = UserState.OfficeId,
                            CreateOfficeIdPath = UserState.OfficeIdPath,
                            CreateOfficeName = UserState.OfficeName,
                            CreateUserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            CreateUserId = UserState.UserId,
                            CreateUserName = UserState.UserName,
                            ImageJson = null,
                            Note = note,
                            PackageCode = orderPackage.Code,
                            PackageId = orderPackage.Id,
                            Updated = timeNow,
                            Status = 0,
                            Mode = 1,
                            UnsignText = MyCommon.Ucs2Convert($"{note} {orderPackage.Code} {orderPackage.TransportCode}" +
                                                              $" {UserState.OfficeName} {UserState.FullName}" +
                                                              $" {UserState.UserName} {order.Code}")
                        };

                        if (order.UserId == null)
                            return Json(new { status = MsgType.Error, msg = "This order does not have any ordering employee!" }, JsonRequestBehavior.AllowGet);

                        var userOrder = await UnitOfWork.UserRepo.GetUser(order.UserId.Value);

                        packageNoCode.UpdateUserId = userOrder.Id;
                        packageNoCode.UpdateOfficeId = userOrder.OfficeId;
                        packageNoCode.UpdateOfficeIdPath = userOrder.OfficeIdPath;
                        packageNoCode.UpdateOfficeName = userOrder.OfficeName;
                        packageNoCode.UpdateUserName = userOrder.UserName;
                        packageNoCode.UpdateUserFullName = userOrder.FullName;

                        UnitOfWork.PackageNoCodeRepo.Add(packageNoCode);

                        var rs2 = await UnitOfWork.PackageNoCodeRepo.SaveAsync();

                        // Thông báo cho đặt hàng vào xác nhận
                        if (rs2 > 0)
                        {
                            var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(order.Type == (byte)OrderType.Order ? (byte)OfficeType.Order : (byte)OfficeType.Deposit)}");

                            // Thông báo cho nhân viên kho
                            if (order.UserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                                                                    $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                                                    $"{UserState.FullName} requests confirming package" +
                                                                    $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                                                                    $" into order {MyCommon.ReturnCode(order.Code)}",
                                                                    $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                                                        $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                                                        $"{UserState.FullName} requests confirming package" +
                                                                        $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                                                                        $" into order {MyCommon.ReturnCode(order.Code)}",
                                                                        $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                }
                                else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                                {
                                    if (order.UserId != u.UserId)
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                                                            $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                                                            $"{UserState.FullName} requests confirming package" +
                                                                            $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                                                                            $" into order {MyCommon.ReturnCode(order.Code)}",
                                                                            $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                }
                            }

                            //// Thông báo tới nhân viên quản lý Order
                            //NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                            //        $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                            //        $"{UserState.FullName} requests confirming package" +
                            //        $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                            //        $" into order {MyCommon.ReturnCode(order.Code)}",
                            //        $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));

                            //// Lấy ra trưởng phòng đặt hàng
                            //var leaderOrder =
                            //    await UnitOfWork.UserRepo.GetLeaderByOfficeId(packageNoCode.UpdateOfficeId.Value, 1);

                            //// Thông báo tới trưởng phòng đặt hàng thông tin này
                            //if (leaderOrder != null && leaderOrder.Id != order.UserId.Value)
                            //{
                            //    NotifyHelper.CreateAndSendNotifySystemToClient(leaderOrder.Id,
                            //        $"{UserState.FullName} gửi yêu cầu xác nhận kiện cho Order cho {packageNoCode.UpdateUserFullName}",
                            //        EnumNotifyType.Info,
                            //        $"{UserState.FullName} gửi requests confirming package" +
                            //        $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                            //        $" into order {MyCommon.ReturnCode(order.Code)} cho nhân viên {packageNoCode.UpdateUserFullName}",
                            //        $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                            //}
                        }
                    }

                    //Lây ra số package trong Order
                    var listPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type);

                    //Tính trung bình số giá trị package
                    var totalPrice = order.TotalExchange / listPackage.Count;
                    foreach (var item in listPackage)
                    {
                        item.TotalPrice = totalPrice;
                        item.LastUpdate = timeNow;
                        item.PackageNo = listPackage.Count;
                    }

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    order.PackageNo = listPackage.Count;
                    order.LastUpdate = timeNow;

                    //Cập nhật unsignName
                    order.UnsignName += $" {codePackage.ToLower()}";

                    await UnitOfWork.OrderRepo.SaveAsync();

                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Add the tracking code: #{orderPackage.TransportCode} - package code #{orderPackage.Code}",
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

            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);
            return Json(new { status = MsgType.Success, msg = "Waybill code added successfully!", list }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gán package mất mã với mã vận đơn
        /// </summary>
        /// <param name="packageNoCodeId">Id package mất mã</param>
        /// <param name="transportCode">Mã vận đơn</param>
        /// <param name="orderId">Id Order</param>
        /// <param name="note">Ghi chú</param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCode, EnumPage.PackageNoCodeApprovel, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> AssignPackage(int packageNoCodeId, string transportCode, int orderId, string note)
        {
            if (!string.IsNullOrEmpty(transportCode))
            {
                transportCode = transportCode.Trim();
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var packageNoCode =
                        await UnitOfWork.PackageNoCodeRepo.SingleOrDefaultAsync(x => x.Id == packageNoCodeId);

                    // Code-missing package does not exist
                    if (packageNoCode == null)
                        return Json(new { status = -1, msg = "Code-missing package does not exist!" },
                            JsonRequestBehavior.AllowGet);

                    var orderPackage =
                        await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageNoCode.PackageId
                        && x.IsDelete == false);

                    // package does not exist
                    if (orderPackage == null)
                        return Json(new { status = -1, msg = "Goods package does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && x.IsDelete == false);

                    // Order does not exist or has been deleted
                    if (order == null)
                        return Json(new { status = -1, msg = "Order does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    // Chỉ có đặt hàng mới quá quyền thực hiện thao tác này
                    if (packageNoCode.Mode == 0 && (order.Type == (byte)OrderType.Order && UserState.OfficeType != (byte)OfficeType.Order
                        || order.Type == (byte)OrderType.Deposit && UserState.OfficeType != (byte)OfficeType.Deposit))
                        return Json(new { status = -1, msg = "Only ordering staf has permission to perform this action !" },
                                    JsonRequestBehavior.AllowGet);

                    // Chỉ có đặt hàng mới quá quyền thực hiện thao tác này
                    if (packageNoCode.Mode == 0 && (order.Type == (byte)OrderType.Deposit && UserState.OfficeType != (byte)OfficeType.Deposit
                        || order.Type == (byte)OrderType.Deposit && UserState.OfficeType != (byte)OfficeType.Deposit))
                        return Json(new { status = -1, msg = "Only staff handling consigned goods has the permission to perform this action!" },
                                    JsonRequestBehavior.AllowGet);

                    // Chặn quyền thao tác của nhân viên đặt hàng với Edit yêu cầu xác nhận
                    //if (packageNoCode.Mode == 1 && (order.Type == (byte)OrderType.Order && UserState.OfficeType == (byte)OfficeType.Order
                    //     || order.Type == (byte)OrderType.Deposit && UserState.OfficeType == (byte)OfficeType.Deposit) &&
                    //    UserState.Type == 0 && UserState.UserId != packageNoCode.UpdateUserId)
                    //    return Json(new { status = -1, msg = "You do not have permission to perform this action!" },
                    //        JsonRequestBehavior.AllowGet);

                    // Chặn thao tác của nhân viên kho
                    if (packageNoCode.Mode == 1 && UserState.OfficeType == (byte)OfficeType.Warehouse && UserState.Type == 0
                        && UserState.UserId != packageNoCode.CreateUserId)
                        return Json(new { status = -1, msg = "You do not have permission to perform this action!" },
                                    JsonRequestBehavior.AllowGet);

                    // Không có quyền thao tác với Order
                    //if (UserState.Type == 0 && order.UserId != UserState.UserId)
                    //    return Json(new { status = -1, msg = "Bạn không có quyền thao tác với Order này!" },
                    //        JsonRequestBehavior.AllowGet);

                    // Trùng mã vận đơn
                    if (await UnitOfWork.OrderPackageRepo.AnyAsync(
                                x => x.TransportCode == transportCode && x.Id != orderPackage.Id && x.OrderId == order.Id
                                && x.IsDelete == false))
                        return Json(new { status = -1, msg = "Waybill code does not exist!" },
                                JsonRequestBehavior.AllowGet);

                    var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == order.CustomerId);

                    // Customer does not exist
                    if (customer == null)
                        return Json(new { status = -1, msg = "Customer does not exist!" },
                                JsonRequestBehavior.AllowGet);

                    var warehouse = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == order.WarehouseDeliveryId);

                    // Warehouse does not exist
                    if (warehouse == null)
                        return Json(new { status = -1, msg = "Warehouse does not exist" },
                                JsonRequestBehavior.AllowGet);

                    var warehouseOutSide = await UnitOfWork.OfficeRepo.SingleOrDefaultAsync(x => x.Id == order.WarehouseId);

                    // Warehouse does not exist
                    if (warehouseOutSide == null)
                        return Json(new { status = -1, msg = "Warehouse does not exist" },
                                JsonRequestBehavior.AllowGet);

                    // Update PackageNoCode
                    packageNoCode.UpdateUserId = UserState.UserId;
                    packageNoCode.UpdateOfficeId = UserState.OfficeId;
                    packageNoCode.UpdateOfficeIdPath = UserState.OfficeIdPath;
                    packageNoCode.UpdateOfficeName = UserState.OfficeName;
                    packageNoCode.UpdateUserFullName = UserState.FullName;
                    packageNoCode.UpdateUserName = UserState.UserName;
                    packageNoCode.Updated = DateTime.Now;
                    packageNoCode.Status = 1;

                    var packageNo =
                        await UnitOfWork.OrderPackageRepo.CountAsync(x => x.OrderId == orderId && x.IsDelete == false);

                    //Ghi chú toàn system cho package, Order
                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                          x =>
                          x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                          x.Mode == (byte)PackageNoteMode.PackageNoCode);

                    // Update Package
                    orderPackage.TransportCode = transportCode;
                    orderPackage.OrderId = order.Id;
                    orderPackage.OrderCode = order.Code;
                    orderPackage.CustomerId = customer.Id;
                    orderPackage.CustomerName = customer.FullName;
                    orderPackage.CustomerUserName = customer.Email;
                    orderPackage.CustomerLevelId = customer.LevelId;
                    orderPackage.CustomerLevelName = customer.LevelName;
                    orderPackage.CustomerWarehouseId = warehouse.Id;
                    orderPackage.CustomerWarehouseName = warehouse.Name;
                    orderPackage.CustomerWarehouseIdPath = warehouse.IdPath;
                    orderPackage.CustomerWarehouseAddress = warehouse.Address;
                    orderPackage.TransportCode = transportCode;
                    orderPackage.SystemId = order.SystemId;
                    orderPackage.SystemName = order.SystemName;
                    orderPackage.LastUpdate = DateTime.Now;
                    orderPackage.PackageNo = packageNo;
                    orderPackage.UnsignedText =
                        MyCommon.Ucs2Convert(
                            $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.UserFullName} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone} {order.ShopName}");
                    orderPackage.OrderType = order.Type;
                    orderPackage.Note = note;

                    // Cập nhật thông tin Order
                    order.PackageNo = packageNo;

                    var listService = await UnitOfWork.OrderServiceRepo.FindAsync(
                                x => x.OrderId == order.Id && x.Mode == (byte)OrderServiceMode.Option && x.Checked &&
                                    !x.IsDelete);

                    if (listService.Any())
                    {
                        orderPackage.OrderServices = string.Join(", ", listService.Select(x => x.ServiceName.Replace("Phí dịch", "Dịch")).ToList());
                    }

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Check trùng mã vận đơn
                    var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == orderPackage.TransportCode && x.OrderId > 0);

                    if (checkPackageCount > 1)
                    {
                        PackageJob.UpdateSameTransportCode(orderPackage.TransportCode, orderPackage.Code, UnitOfWork, UserState);
                    }

                    orderPackage.UnsignedText += MyCommon.Ucs2Convert($"{orderPackage.Code} P{orderPackage.Code} {orderPackage.WarehouseName} {transportCode}").ToLower();

                    var rs = await UnitOfWork.OrderPackageRepo.SaveAsync();

                    if (rs > 0)
                    {
                        // Thêm lịch sử cho package
                        var packageHistory = new PackageHistory()
                        {
                            PackageId = orderPackage.Id,
                            PackageCode = orderPackage.Code,
                            OrderId = order.Id,
                            OrderCode = order.Code,
                            Type = order.Type,
                            Status = (int)OrderPackageStatus.ShopDelivery,
                            Content = EnumHelper.GetEnumDescription(OrderPackageStatus.ShopDelivery),
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            UserId = UserState.UserId,
                            UserName = UserState.UserName,
                            UserFullName = UserState.FullName,
                            CreateDate = DateTime.Now,
                        };

                        UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                        await UnitOfWork.PackageHistoryRepo.SaveAsync();

                        if (packageNote == null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.Id,
                                PackageCode = orderPackage.Code,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                Time = DateTime.Now,
                                ObjectId = null,
                                ObjectCode = string.Empty,
                                Mode = (byte)PackageNoteMode.PackageNoCode,
                                Content = orderPackage.Note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            packageNote.Content = orderPackage.Note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }

                        // Cập nhật lại Total money các kiện trong Order


                        // XỬ LÝ HÀNG MẤT MÃ
                        // Thông báo cho nhân viên kho package mất mã đã được gán tới Order
                        if (packageNoCode.Mode == 0 && packageNoCode.CreateUserId.HasValue && packageNoCode.CreateUserId != UserState.UserId)
                        {
                            NotifyHelper.CreateAndSendNotifySystemToClient(packageNoCode.CreateUserId.Value,
                                $"{UserState.FullName} has completed handling code-missing goods", EnumNotifyType.Warning,
                                $"{UserState.FullName} has completed handling code-missing goods for package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                        }

                        // XỬ LÝ YÊU CẦU XÁC NHẬN KIỆN TRONG Order   
                        if (packageNoCode.Mode == 1)
                        {
                            // Nhân viên đặt hàng thao tác
                            if (UserState.OfficeType.HasValue &&
                                (UserState.OfficeType.Value == (byte)OfficeType.Order || UserState.OfficeType.Value == (byte)OfficeType.Deposit))
                            {
                                // Thông báo cho nhân viên tạo
                                if (packageNoCode.CreateUserId.HasValue && packageNoCode.CreateUserId != UserState.UserId)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(packageNoCode.CreateUserId.Value,
                                    $"{UserState.FullName} has completed handling code-missing goods", EnumNotifyType.Warning,
                                    $"{UserState.FullName} has completed handling code-missing goods for package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                    $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                }
                            }

                            // Nhân viên kho thao tác Edit
                            if (UserState.OfficeType.HasValue &&
                                UserState.OfficeType.Value == (byte)OfficeType.Warehouse)
                            {
                                var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(order.Type == (byte)OrderType.Order ? (byte)OfficeType.Order : (byte)OfficeType.Deposit)}");

                                // Thông báo cho nhân viên kho
                                if (order.UserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                                         $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                         $"{UserState.FullName} requests confirming package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                         $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                }

                                // Thông báo tới nhân viên trong cấu hình
                                foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                                {
                                    if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                    {
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                             $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                             $"{UserState.FullName} requests confirming package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                             $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                    }
                                    else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                                    {
                                        if (order.UserId != u.UserId)
                                            NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                                 $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                                 $"{UserState.FullName} requests confirming package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                                 $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                    }
                                }

                                //// Thông báo tới nhân viên đặt hàng của Order này
                                //if (order.UserId.HasValue)
                                //    NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                                //        $"{UserState.FullName} requests confirming package for order", EnumNotifyType.Warning,
                                //        $"{UserState.FullName} requests confirming package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a> into order {MyCommon.ReturnCode(order.Code)}",
                                //        $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));

                                //// Lấy ra trưởng phòng đặt hàng
                                //if (packageNoCode.UpdateOfficeId.HasValue && order.UserId.HasValue)
                                //{
                                //    var leaderOrder =
                                //    await UnitOfWork.UserRepo.GetLeaderByOfficeId(packageNoCode.UpdateOfficeId.Value, 1);

                                //    // Thông báo tới trưởng phòng đặt hàng thông tin này
                                //    if (leaderOrder != null && leaderOrder.Id != order.UserId.Value)
                                //    {
                                //        NotifyHelper.CreateAndSendNotifySystemToClient(leaderOrder.Id,
                                //            $"{UserState.FullName} gửi yêu cầu xác nhận kiện cho Order cho {packageNoCode.UpdateUserFullName}", EnumNotifyType.Info,
                                //            $"{UserState.FullName} gửi requests confirming package" +
                                //            $" <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>" +
                                //            $" into order {MyCommon.ReturnCode(order.Code)} cho nhân viên {packageNoCode.UpdateUserFullName}",
                                //            $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                                //    }
                                //}
                            }
                        }
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

            return Json(new { status = 1, msg = "Assigning package to order successfully!" },
                            JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xác nhận Order mất mã
        /// </summary>
        /// <param name="packageNoCodeId"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCodeApprovel, EnumPage.ImportWarehouse)]
        public async Task<ActionResult> ApprovelAssignPackage(int packageNoCodeId)
        {
            var packageNoCode =
                await UnitOfWork.PackageNoCodeRepo.SingleOrDefaultAsync(x => x.Id == packageNoCodeId && x.Mode == 1);

            if (packageNoCode == null)
                return Json(new { status = -1, msg = "This code-missing package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

            //if (UserState.Type == 0 && UserState.UserId != packageNoCode.UpdateUserId)
            //    return Json(new { status = -1, msg = "Bạn không có quyền xác nhận package mất mã này" },
            //                JsonRequestBehavior.AllowGet);

            var package = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageNoCode.PackageId);

            if (package == null)
                return Json(new { status = -1, msg = "This code-missing package does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

            if (package.OrderType == (byte)OrderType.Order && UserState.OfficeType != (byte)OfficeType.Order
                || package.OrderType == (byte)OrderType.Deposit && UserState.OfficeType != (byte)OfficeType.Deposit)
                return Json(new { status = -1, msg = "Only new orders are allowed to perform this operation" },
                    JsonRequestBehavior.AllowGet);

            //Check trùng mã vận đơn
            var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == package.TransportCode && x.OrderId > 0);

            if (checkPackageCount > 1)
            {
                PackageJob.UpdateSameTransportCode(package.TransportCode, package.Code, UnitOfWork, UserState);
            }

            packageNoCode.Status = 1;
            packageNoCode.Updated = DateTime.Now;

            var rs = await UnitOfWork.PackageNoCodeRepo.SaveAsync();

            if (package.OrderId > 0)
            {
                var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == package.OrderId);
                //Thêm thông tin kiện đã nhập kho cho Order
                order.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.OrderId == order.Id);
                order.UnsignName += $" {package.TransportCode.ToLower()}";
                await UnitOfWork.OrderRepo.SaveAsync();
            }

            if (rs > 0)
            {
                var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(package.OrderType == (byte)OrderType.Order ? (byte)OfficeType.Order : (byte)OfficeType.Deposit)}");

                // Thông báo cho nhân viên kho
                if (packageNoCode.CreateUserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                {
                    NotifyHelper.CreateAndSendNotifySystemToClient(packageNoCode.CreateUserId.Value,
                    $"{UserState.FullName} has confirmed code-missing package into order", EnumNotifyType.Warning,
                    $"{UserState.FullName} has confirmed package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{packageNoCode.PackageCode}</a> into order {MyCommon.ReturnCode(package.OrderCode)}",
                    $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                }

                // Thông báo tới nhân viên trong cấu hình
                foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                {
                    if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                          $"{UserState.FullName} has confirmed code-missing package into order", EnumNotifyType.Warning,
                          $"{UserState.FullName} has confirmed package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{packageNoCode.PackageCode}</a> into order {MyCommon.ReturnCode(package.OrderCode)}",
                          $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                    }
                    else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                    {
                        if (packageNoCode.CreateUserId != u.UserId)
                            NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                $"{UserState.FullName} has confirmed code-missing package into order",
                                EnumNotifyType.Warning,
                                $"{UserState.FullName} has confirmed package <a href=\"{Url.Action("Approvel", "PackageNoCode")}\" target=\"_blank\">P{packageNoCode.PackageCode}</a> into order {MyCommon.ReturnCode(package.OrderCode)}",
                                $"PackageLose_P{packageNoCode.Id}", Url.Action("Approvel", "PackageNoCode"));
                    }
                }
            }

            return Json(new { status = 1, msg = "Package confirmed successfully" },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm package mất mã
        /// </summary>
        /// <param name="imageJson">Image Json của package</param>
        /// <param name="note">Ghi chú thông tin cho package</param>
        /// <param name="tranportCode">Mã vận đơn của package</param>
        /// <param name="packageId">Mã kiện (Thêm yêu cầu cho package đã có sẵn)</param>
        /// <returns></returns>
        //Add the tracking code into order
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCode, EnumPage.ImportWarehouse)]
        public async Task<JsonResult> AddPackageNoCode(string imageJson, string note, string tranportCode, int? packageId)
        {
            if (!string.IsNullOrEmpty(tranportCode))
            {
                tranportCode = tranportCode.Trim();
            }

            if (UserState.OfficeType != (byte)OfficeType.Warehouse)
                return Json(new { status = -1, msg = "Only warehouse staff have permission to perform this action!" },
                            JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //lấy thông tin kho nguồn
                    var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == UserState.OfficeId);

                    if (warehouseStart == null)
                    {
                        return Json(new { status = MsgType.Error, msg = "Cannot identify receiving warehouse!" }, JsonRequestBehavior.AllowGet);
                    }

                    OrderPackage orderPackage;
                    int rs;

                    if (packageId == null)
                    {
                        // Trùng mã vận đơn
                        if (await UnitOfWork.OrderPackageRepo.AnyAsync(
                                    x => x.TransportCode == tranportCode && x.OrderId == 0 && x.IsDelete == false))
                            return Json(new { status = -1, msg = "Waybill code does not exist!" },
                                    JsonRequestBehavior.AllowGet);

                        orderPackage = new OrderPackage()
                        {
                            Code = string.Empty,
                            OrderId = 0,
                            OrderCode = string.Empty,
                            CustomerId = 0,
                            CustomerName = string.Empty,
                            CustomerUserName = string.Empty,
                            CustomerLevelId = 0,
                            CustomerLevelName = string.Empty,
                            CustomerWarehouseId = 0,
                            CustomerWarehouseName = string.Empty,
                            CustomerWarehouseIdPath = string.Empty,
                            CustomerWarehouseAddress = string.Empty,
                            TransportCode = string.IsNullOrWhiteSpace(tranportCode) ? string.Empty : tranportCode,
                            WarehouseId = warehouseStart.Id,
                            WarehouseName = warehouseStart.Name,
                            WarehouseIdPath = warehouseStart.IdPath,
                            WarehouseAddress = warehouseStart.Address,
                            SystemId = 0,
                            SystemName = string.Empty,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            HashTag = string.Empty,
                            PackageNo = 0,
                            UnsignedText = string.Empty,
                            ForcastDate = null,
                            OrderType = 0,
                            Note = note,
                            Status = (byte)OrderPackageStatus.ChinaReceived,
                            CurrentWarehouseId = UserState.OfficeId,
                            CurrentWarehouseName = UserState.OfficeName,
                            CurrentWarehouseIdPath = UserState.OfficeIdPath,
                            CurrentWarehouseAddress = UserState.OfficeAddress,
                        };

                        UnitOfWork.OrderPackageRepo.Add(orderPackage);
                        rs = await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }
                    else
                    {
                        orderPackage = await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageId && x.IsDelete == false && x.HashTag == ";packagelose;");

                        if (orderPackage == null)
                            return Json(new { status = -1, msg = "This code-missing package does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                        orderPackage.HashTag = string.Empty;
                        rs = 1;
                    }

                    //cập nhật lại mã code
                    var orderPackageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                                 x.Created.Year == orderPackage.Created.Year && x.Created.Month == orderPackage.Created.Month &&
                                 x.Created.Day == orderPackage.Created.Day && x.Id <= orderPackage.Id);
                    orderPackage.Code = $"{orderPackageOfDay}{orderPackage.Created:ddMMyy}";
                    orderPackage.UnsignedText = MyCommon.Ucs2Convert($"{orderPackage.OrderCode} {orderPackage.Code} {orderPackage.TransportCode} {orderPackage.CustomerName} {orderPackage.WarehouseName}").ToLower();
                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    if (rs > 0)
                    {
                        //Ghi chú toàn system cho package, Order
                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                               x =>
                               x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                               x.Mode == (byte)PackageNoteMode.PackageNoCode);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.Id,
                                PackageCode = orderPackage.Code,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                Time = DateTime.Now,
                                ObjectId = null,
                                ObjectCode = string.Empty,
                                Mode = (byte)PackageNoteMode.PackageNoCode,
                                Content = orderPackage.Note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            packageNote.Content = orderPackage.Note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }

                        // Thêm lịch sử cho hàng mất mã
                        // todo: Thêm lịch sử cho hàng mất mã

                        // Tạo phiếu Hàng mất mã
                        var packageNoCode = new PackageNoCode()
                        {
                            Created = timeNow,
                            CreateOfficeId = UserState.OfficeId,
                            CreateOfficeIdPath = UserState.OfficeIdPath,
                            CreateOfficeName = UserState.OfficeName,
                            CreateUserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            CreateUserId = UserState.UserId,
                            CreateUserName = UserState.UserName,
                            ImageJson = imageJson,
                            Note = note,
                            PackageCode = orderPackage.Code,
                            PackageId = orderPackage.Id,
                            Updated = timeNow,
                            Status = 0,
                            Mode = 0,
                            UnsignText = MyCommon.Ucs2Convert($"{note} {orderPackage.Code}  {orderPackage.TransportCode}" +
                                                              $" {UserState.OfficeName} {UserState.FullName}" +
                                                              $" {UserState.UserName}")

                        };

                        UnitOfWork.PackageNoCodeRepo.Add(packageNoCode);

                        var rs2 = await UnitOfWork.PackageNoCodeRepo.SaveAsync();

                        if (rs2 > 0)
                        {
                            #region Thông báo

                            #region Thông phòng đặt hàng

                            var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Order}");
                            // Thông báo đến nhân viên đặt hàng có hàng mất mã
                            var users =
                                UnitOfWork.UserRepo.GetByExpression(
                                    user => user.IsDelete == false && user.Status < 5,
                                    position => position.IsDefault, office => office.Type == (byte)OfficeType.Order);

                            // Thông báo tới tất cả đặt hàng khi nv trong cấu hình chỉ là theo dõi hoặc không có nhân viên trong cấu hình
                            if (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any(x => x.IsNotify))
                            {
                                // Thông báo tới tất cả các đặt hàng
                                foreach (var u2 in users)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u2.Id,
                                        $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                            {
                                // Lấy nv trong cấu hình thay thế đặt hàng
                                if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));

                                }
                                else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                                {
                                    // Nhân viên trong cấu hình k phải là nhân viên đặt hàng
                                    if (users.All(x => x.Id != u.UserId))
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                            $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                            $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            #endregion

                            #region Thông báo phòng Gom công

                            var notifyDeposit = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Deposit}");

                            // Thông báo tới các nhân viên gom công
                            var users2 = UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                            position => position.IsDefault, office => office.Type == (byte)OfficeType.Deposit);

                            if (notifyDeposit.Settings.IsFollow || !notifyDeposit.Settings.Users.Any(x => x.IsNotify))
                            {
                                // Thông báo tới tất cả các đặt hàng
                                foreach (var u2 in users2)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u2.Id,
                                             $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                             $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                             $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyDeposit.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                                else if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow == true)
                                {
                                    if (users2.All(x => x.Id != u.UserId))
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"{UserState.FullName} requests to find code-missing goods", EnumNotifyType.Warning,
                                            $"{UserState.FullName} requests to find code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                            $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            #endregion

                            #endregion
                        }
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

            return Json(new { status = MsgType.Success, msg = "Waybill code added successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Cập nhật thông tin kiện mất mã
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCode, EnumPage.ImportWarehouse)]
        public async Task<JsonResult> UpdatePackageNoCode(int id, string imageJson, string note, string tranportCode)
        {
            if (!string.IsNullOrEmpty(tranportCode))
            {
                tranportCode = tranportCode.Trim();
            }

            if (UserState.OfficeType != (byte)OfficeType.Warehouse)
                return Json(new { status = -1, msg = "Only warehouse staff have permission to perform this action!" },
                            JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var packageNoCode = await UnitOfWork.PackageNoCodeRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if (packageNoCode == null)
                        return Json(new { status = -1, msg = "This code-missing package does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    var orderPackage =
                        await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == packageNoCode.PackageId);

                    if (orderPackage == null)
                        return Json(new { status = -1, msg = "This code-missing goods package does not exist or has been deleted !" },
                            JsonRequestBehavior.AllowGet);

                    packageNoCode.Note = note;
                    packageNoCode.ImageJson = imageJson;
                    packageNoCode.Updated = timeNow;

                    var pOld = orderPackage.TransportCode;
                    orderPackage.TransportCode = tranportCode;
                    orderPackage.LastUpdate = timeNow;

                    if (pOld != orderPackage.TransportCode)
                    {
                        var arrayP = orderPackage.UnsignedText.Split(' ');
                        arrayP = arrayP.Where(x => x != pOld.ToLower()).ToArray();

                        orderPackage.UnsignedText = string.Join(" ", arrayP) + $" { orderPackage.TransportCode.ToLower() }";
                    }

                    var rs2 = await UnitOfWork.PackageNoCodeRepo.SaveAsync();

                    if (rs2 > 0)
                    {
                        //Ghi chú toàn system cho package, Order
                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                              x =>
                              x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                              x.Mode == (byte)PackageNoteMode.PackageNoCode);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.Id,
                                PackageCode = orderPackage.Code,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                Time = DateTime.Now,
                                ObjectId = null,
                                ObjectCode = string.Empty,
                                Mode = (byte)PackageNoteMode.PackageNoCode,
                                Content = orderPackage.Note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            packageNote.Content = orderPackage.Note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(orderPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }

                        await UnitOfWork.PackageNoteRepo.SaveAsync();
                        // Thông báo đến tất cả nhân viên đặt hàng nếu chưa có Staff handling
                        if (packageNoCode.UpdateUserId == null)
                        {
                            var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Order}");
                            // Thông báo đến nhân viên đặt hàng có hàng mất mã
                            var users =
                                UnitOfWork.UserRepo.GetByExpression(
                                    user => user.IsDelete == false && user.Status < 5,
                                    position => position.IsDefault, office => office.Type == (byte)OfficeType.Order);

                            // Thông báo tới tất cả đặt hàng khi nv trong cấu hình chỉ là theo dõi hoặc không có nhân viên trong cấu hình
                            if (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any(x => x.IsNotify))
                            {
                                // Thông báo tới tất cả các đặt hàng
                                foreach (var u2 in users)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u2.Id,
                                        $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                            {
                                // Lấy nv trong cấu hình thay thế đặt hàng
                                if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));

                                }
                                else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                                {
                                    // Nhân viên trong cấu hình k phải là nhân viên đặt hàng
                                    if (users.All(x => x.Id != u.UserId))
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                            $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                            $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            var notifyDeposit = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Deposit}");

                            // Thông báo tới các nhân viên gom công
                            var users2 = UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                            position => position.IsDefault, office => office.Type == (byte)OfficeType.Deposit);

                            if (notifyDeposit.Settings.IsFollow || !notifyDeposit.Settings.Users.Any(x => x.IsNotify))
                            {
                                // Thông báo tới tất cả các đặt hàng
                                foreach (var u2 in users2)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u2.Id,
                                        $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyDeposit.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                        $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                                else if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow == true)
                                {
                                    if (users2.All(x => x.Id != u.UserId))
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                            $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                            $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                                }
                            }

                            //// Thông báo đến nhân viên đặt hàng có hàng mất mã
                            //var users =
                            //    UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                            //        position => position.IsDefault,
                            //        office =>
                            //            (orderPackage.OrderType == (byte)OrderType.Order &&
                            //             office.Type == (byte)OfficeType.Order)
                            //            ||
                            //            (orderPackage.OrderType == (byte)OrderType.Deposit &&
                            //             office.Type == (byte)OfficeType.Deposit));

                            //// Thông báo tới tất cả các đặt hàng
                            //foreach (var u in users)
                            //{
                            //    NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
                            //        $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                            //        $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                            //        $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                            //}
                        }
                        else
                        {
                            NotifyHelper.CreateAndSendNotifySystemToClient(packageNoCode.UpdateUserId.Value,
                                   $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                   $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                   $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                        }

                        // Thông báo cho nhân viên tạo
                        if (packageNoCode.CreateUserId != null && packageNoCode.CreateUserId != UserState.UserId)
                        {
                            NotifyHelper.CreateAndSendNotifySystemToClient(packageNoCode.CreateUserId.Value,
                                   $"{UserState.FullName} has updated information for code-missing goods", EnumNotifyType.Warning,
                                   $"{UserState.FullName} has updated information for code-missing goods of package <a href=\"{Url.Action("Index", "PackageNoCode")}\" target=\"_blank\">P{orderPackage.Code}</a>",
                                   $"PackageLose_P{orderPackage.Code}", Url.Action("Index", "PackageNoCode"));
                        }
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

            return Json(new { status = MsgType.Success, msg = "Updated successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Cập nhật note kiện mất mã
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCode, EnumPage.ImportWarehouse)]
        public async Task<JsonResult> UpdatePackage(int id, string note)
        {
           
            if (UserState.OfficeType != (byte)OfficeType.Order)
                return Json(new { status = -1, msg = "Only ordering staf has permission to perform this action !" },
                            JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var orderPackage =
                        await UnitOfWork.OrderPackageRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if (orderPackage == null)
                        return Json(new { status = -1, msg = "package no code does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    orderPackage.Note = note;
                    orderPackage.LastUpdate = timeNow;

                    var rs2 = await UnitOfWork.OrderPackageRepo.SaveAsync();

                    if (rs2 > 0)
                    {
                        //Ghi chú toàn system cho package, Order
                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                              x =>
                              x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                              x.Mode == (byte)PackageNoteMode.Order);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = orderPackage.OrderId,
                                OrderCode = orderPackage.OrderCode,
                                PackageId = orderPackage.Id,
                                PackageCode = orderPackage.Code,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                Time = DateTime.Now,
                                ObjectId = null,
                                ObjectCode = string.Empty,
                                Mode = (byte)PackageNoteMode.Order,
                                Content = note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(note))
                        {
                            packageNote.Content = note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }

                        await UnitOfWork.PackageNoteRepo.SaveAsync();
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

            return Json(new { status = MsgType.Success, msg = "Update successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Delete waybill code
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderCommerce)]
        public async Task<JsonResult> DeleteContractCode(int id)
        {
            var rs = 0;
            var timeNow = DateTime.Now;

            var orderPackage = await UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (orderPackage == null) //package đã bị xóa
            {
                rs = -2;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderPackage.OrderId && !x.IsDelete);
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                rs = -1;
                return Json(rs, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var array = order.UnsignName.Split(' ');
                    array = array.Where(x => x != orderPackage.TransportCode.ToLower()).ToArray();

                    order.UnsignName = string.Join(" ", array);
                    await UnitOfWork.OrderRepo.SaveAsync();

                    orderPackage.IsDelete = true;
                    orderPackage.LastUpdate = timeNow;

                    rs = await UnitOfWork.OrderPackageRepo.SaveAsync();

                    order.PackageNo = UnitOfWork.OrderPackageRepo.Count(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);
                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Lây ra số package trong Order
                    var listPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id);

                    //Tính trung bình số giá trị package
                    var totalPrice = order.TotalExchange / listPackage.Count;
                    foreach (var item in listPackage)
                    {
                        item.TotalPrice = totalPrice;
                        item.LastUpdate = timeNow;
                    }

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Delete waybill code: #{orderPackage.TransportCode} - package code #{orderPackage.Code}",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //xóa trùng mã vận đơn
                    var listP = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.TransportCode == orderPackage.TransportCode && x.OrderId > 0);
                    if (listP.Count() == 1)
                    {
                        var item = listP.FirstOrDefault();
                        item.Note = MyCommon.RemoveHash(MyCommon.RemoveHash(item.Note, "[TM]"), "[XL]");
                        item.Mode = null;
                        item.SameCodeStatus = 0;

                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }
                    else
                    {
                        //Check trùng mã vận đơn
                        var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == orderPackage.TransportCode && x.OrderId > 0);

                        if (checkPackageCount > 1)
                        {
                            PackageJob.UpdateSameTransportCode(orderPackage.TransportCode, listP.FirstOrDefault().Code, UnitOfWork, UserState);
                        }
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

            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);

            return Json(new { rs, list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay, EnumPage.OrderCommerce)]
        public async Task<JsonResult> EditContractCode(int packageId, string packageName, DateTime? date, string note)
        {
            var timeNow = DateTime.Now;

            if (!string.IsNullOrEmpty(packageName))
            {
                packageName = packageName.Trim();
            }

            var orderPackage = await UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(x => x.Id == packageId && !x.IsDelete);
            if (orderPackage == null) //package đã bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Package does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderPackage.OrderId && !x.IsDelete);
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);
            }

            var checkOrderPackage =
               await
                   UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(
                       x => !x.IsDelete && x.OrderId == order.Id && x.TransportCode == packageName && x.Id != packageId);

            if (checkOrderPackage != null)
            {
                return Json(new { status = MsgType.Error, msg = $" Waybill code coincided '#{packageName}'!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    var pOld = orderPackage.TransportCode;

                    var array = order.UnsignName.Split(' ');
                    array = array.Where(x => x != orderPackage.TransportCode.ToLower()).ToArray();

                    order.UnsignName = string.Join(" ", array) + " " + packageName;
                    await UnitOfWork.OrderRepo.SaveAsync();
                    if (DataCompare(orderPackage.TransportCode, packageName))
                    {
                        dataBefore.Add(new LogResult() { Name = "Waybill code", Value = orderPackage.TransportCode });
                        orderPackage.TransportCode = packageName;
                        dataAfter.Add(new LogResult() { Name = "Waybill code", Value = orderPackage.TransportCode });
                    }

                    orderPackage.LastUpdate = timeNow;

                    if (date != null && DataCompare(orderPackage.ForcastDate, date))
                    {
                        if (orderPackage.ForcastDate != null)
                            dataBefore.Add(new LogResult()
                            {
                                Name = "Estimated time of goods arrival (ETA)",
                                Value = orderPackage.ForcastDate.Value.ToString("dd/MM/yyyy HH:mm:ss.fff")
                            });
                        orderPackage.ForcastDate = date;
                        dataAfter.Add(new LogResult()
                        {
                            Name = "Estimated time of goods arrival (ETA)",
                            Value = orderPackage.ForcastDate.Value.ToString("dd/MM/yyyy HH:mm:ss.fff")
                        });
                    }
                    if (DataCompare(orderPackage.Note, note))
                    {
                        dataBefore.Add(new LogResult() { Name = "Note", Value = orderPackage.Note });
                        orderPackage.Note = note;
                        dataAfter.Add(new LogResult() { Name = "Note", Value = orderPackage.Note });
                    }

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Check trùng mã vận đơn
                    var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == packageName && x.OrderId > 0);

                    if (checkPackageCount > 1)
                    {
                        PackageJob.UpdateSameTransportCode(orderPackage.TransportCode, orderPackage.Code, UnitOfWork, UserState);
                    }

                    //Ghi chú toàn system cho package, Order
                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                          x =>
                          x.PackageId == orderPackage.Id && x.OrderId == orderPackage.OrderId && x.ObjectId == null &&
                          x.Mode == (byte)PackageNoteMode.Order);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = orderPackage.OrderId,
                            OrderCode = orderPackage.OrderCode,
                            PackageId = orderPackage.Id,
                            PackageCode = orderPackage.Code,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = null,
                            ObjectCode = string.Empty,
                            Mode = (byte)PackageNoteMode.Order,
                            Content = orderPackage.Note
                        });
                    }
                    else if (packageNote != null && !string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        packageNote.Content = orderPackage.Note;
                    }
                    else if (packageNote != null && string.IsNullOrWhiteSpace(orderPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Remove(packageNote);
                    }

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = $"Update information of package 'P{orderPackage.Code}'",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    if (pOld != orderPackage.TransportCode)
                    {
                        var arrayP = orderPackage.UnsignedText.Split(' ');
                        arrayP = arrayP.Where(x => x != pOld.ToLower()).ToArray();

                        orderPackage.UnsignedText = string.Join(" ", arrayP) + $" { orderPackage.TransportCode.ToLower() }";
                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //xóa trùng mã vận đơn
                        var listP = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.TransportCode == pOld && x.OrderId > 0);
                        if (listP.Count() == 1)
                        {
                            var item = listP.FirstOrDefault();
                            item.Note = MyCommon.RemoveHash(MyCommon.RemoveHash(item.Note, "[TM]"), "[XL]");
                            item.Mode = null;
                            item.SameCodeStatus = 0;

                            await UnitOfWork.OrderPackageRepo.SaveAsync();
                        }
                        else
                        {
                            //Check trùng mã vận đơn
                            var checkPackageCount2 = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == pOld && x.OrderId > 0);

                            if (checkPackageCount2 > 1)
                            {
                                PackageJob.UpdateSameTransportCode(pOld, listP.FirstOrDefault().Code, UnitOfWork, UserState);
                            }
                        }
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

            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);

            return Json(new { status = MsgType.Success, msg = "Updated successfully", list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [5. Actions with waybill code]

        #region [6. Các thao tác với Contract code]

        #region [Thêm Contract code]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> AddContractCodeOrder(int id, string contractCode, decimal pice)
        {
            //1. khai báo biến
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist Order hoặc Order bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);

                    var orderContractCode = new OrderContractCode()
                    {
                        OrderId = order.Id,
                        OrderType = order.Type,
                        ContractCode = "",
                        TotalPrice = null,
                        IsDelete = false,
                        CreateDate = timeNow,
                        UpdateDate = timeNow,
                        Status = (byte)ContractCodeType.New
                    };

                    order.ContractCodes = listContractCode.Select(x => x.ContractCode).ToString();
                    order.UnsignName += $" {contractCode}";

                    UnitOfWork.OrderContractCodeRepo.Add(orderContractCode);
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //4. gửi dữ liệu lên view
            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete && x.OrderType == order.Type);
            return Json(new { status = MsgType.Success, msg = "Contract code added successfully!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Thêm Contract code]

        #region [Gửi lại Contract code]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> ReviewContractCodeOrder(int id)
        {
            //1. khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (orderContractCode == null)
            {
                return Json(new { status = MsgType.Error, msg = "Contract ID does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderContractCode.OrderId && !x.IsDelete);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    orderContractCode.Status = (byte)ContractCodeType.AwaitingPayment;
                    orderContractCode.UpdateDate = timeNow;
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Resend contract code #{orderContractCode.ContractCode} accountant inspecting and disbursing",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);
                    foreach (var user in listUser)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Recheck and  pay the order #" + MyCommon.ReturnCode(order.Code), EnumNotifyType.Info, "Request to recheck and pay the order contract: " + order.ContractCodes);
                    }

                    var listOrderExchange =
                        await
                            UnitOfWork.OrderExchangeRepo.FindAsync(
                                x =>
                                    !x.IsDelete && x.Status == (byte)OrderExchangeStatus.Approved &&
                                    x.Type == (byte)OrderExchangeType.Product && x.OrderId == order.Id);

                    var sum = listOrderExchange.Sum(x => x.TotalPrice);

                    if (order.Total < sum)
                    {
                        //Ghi công nợ
                        if (order.CustomerId != null)
                        {
                            var total = sum.Value - order.Total;
                            if (total > 0)
                            {
                                var autoUpdateDebitModel = new AutoUpdateDebitModel()
                                {
                                    SubjectId = order.CustomerId.Value, // Id khách hàng
                                    SubjectTypeIdd = (int)EnumAccountantSubject.Customer,
                                    Money = total, // Số tiền
                                    OrderId = order.Id, // Id Order
                                    PayReceivableIdd = (int)TreasureMustReturn.MissingLinkOrder,
                                    // Đinh khoản (Enum TreasureMustReturn)
                                    OrderType = order.Type,
                                    OrderCode = order.Code
                                };

                                var result = UnitOfWork.DebitRepo.UpdateDebit(autoUpdateDebitModel);

                                // Lỗi trong quá tình thực hiện công nợ
                                if (result.Status < 0)
                                {
                                    transaction.Rollback();
                                    return Json(new { status = MsgType.Error, msg = result.Msg }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Send to Contract code successful!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> SendContractCodeOrder(int id)
        {
            //1. khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (orderContractCode == null)
            {
                return Json(new { status = MsgType.Error, msg = "Contract code does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderContractCode.OrderId && !x.IsDelete);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    orderContractCode.Status = (byte)ContractCodeType.AwaitingPayment;
                    orderContractCode.UpdateDate = timeNow;
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Send contract code #{orderContractCode.ContractCode} to accountant to make payment",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);
                    foreach (var user in listUser)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Accountant disburses to pay order contract #" + MyCommon.ReturnCode(order.Code), EnumNotifyType.Info, "Request accountant to pay order contract: " + order.ContractCodes);
                    }

                    var listOrderExchange =
                        await
                            UnitOfWork.OrderExchangeRepo.FindAsync(
                                x =>
                                    !x.IsDelete && x.Status == (byte)OrderExchangeStatus.Approved &&
                                    x.Type == (byte)OrderExchangeType.Product && x.OrderId == order.Id);

                    var sum = listOrderExchange.Sum(x => x.TotalPrice);

                    if (order.Total < sum)
                    {
                        //Ghi công nợ
                        if (order.CustomerId != null)
                        {
                            var total = sum.Value - order.Total;
                            if (total > 0)
                            {
                                var autoUpdateDebitModel = new AutoUpdateDebitModel()
                                {
                                    SubjectId = order.CustomerId.Value, // Id khách hàng
                                    SubjectTypeIdd = (int)EnumAccountantSubject.Customer,
                                    Money = total, // Số tiền
                                    OrderId = order.Id, // Id Order
                                    PayReceivableIdd = (int)TreasureMustReturn.MissingLinkOrder,
                                    // Đinh khoản (Enum TreasureMustReturn)
                                    OrderType = order.Type,
                                    OrderCode = order.Code
                                };

                                var result = UnitOfWork.DebitRepo.UpdateDebit(autoUpdateDebitModel);

                                // Lỗi trong quá tình thực hiện công nợ
                                if (result.Status < 0)
                                {
                                    transaction.Rollback();
                                    return Json(new { status = MsgType.Error, msg = result.Msg }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Contract code sent successfully!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Delete contract code]

        #region [Delete contract code]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> DeleteContractCodeOrder(int id)
        {
            //1. khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (orderContractCode == null)
            {
                return Json(new { status = MsgType.Error, msg = "Contract ID does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderContractCode.OrderId && !x.IsDelete);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    orderContractCode.IsDelete = true;
                    orderContractCode.UpdateDate = timeNow;
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    //tính toán cho Order
                    var listOrderContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type && !x.IsDelete);
                    if (listOrderContractCode.Sum(x => x.TotalPrice) == 0)
                    {
                        order.FeeShipBargain = 0;
                    }
                    order.PaidShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);

                    var priceBargain = (order.TotalPrice + (order.FeeShip ?? 0)) - ((order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0));

                    order.PriceBargain = priceBargain < 0 ? 0 : priceBargain;

                    order.ContractCodes = listOrderContractCode.Select(x => x.ContractCode).ToString();
                    order.LastUpdate = timeNow;

                    var array = order.UnsignName.Split(' ');
                    array = array.Where(x => x != orderContractCode.ContractCode.ToLower()).ToArray();
                    order.UnsignName = string.Join(" ", array);
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Delete contract order #{orderContractCode.ContractCode}",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    var listOrderExchange =
                        await
                            UnitOfWork.OrderExchangeRepo.FindAsync(
                                x =>
                                    !x.IsDelete && x.Status == (byte)OrderExchangeStatus.Approved &&
                                    x.Type == (byte)OrderExchangeType.Product && x.OrderId == order.Id);

                    var sum = listOrderExchange.Sum(x => x.TotalPrice);

                    if (order.Total < sum)
                    {
                        //Ghi công nợ
                        if (order.CustomerId != null)
                        {
                            var total = sum.Value - order.Total;
                            if (total > 0)
                            {
                                var autoUpdateDebitModel = new AutoUpdateDebitModel()
                                {
                                    SubjectId = order.CustomerId.Value, // Id khách hàng
                                    SubjectTypeIdd = (int)EnumAccountantSubject.Customer,
                                    Money = total, // Số tiền
                                    OrderId = order.Id, // Id Order
                                    PayReceivableIdd = (int)TreasureMustReturn.MissingLinkOrder,
                                    // Đinh khoản (Enum TreasureMustReturn)
                                    OrderType = order.Type,
                                    OrderCode = order.Code
                                };

                                var result = UnitOfWork.DebitRepo.UpdateDebit(autoUpdateDebitModel);

                                // Lỗi trong quá tình thực hiện công nợ
                                if (result.Status < 0)
                                {
                                    transaction.Rollback();
                                    return Json(new { status = MsgType.Error, msg = result.Msg },
                                        JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Contract code deleted successfully!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Delete contract code]

        #region [Edit Contract code]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> EditContractCodeOrder(int id, string code, decimal? totalPrice)
        {
            //1. Khai báo biến
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            //2. check điều kiện
            if (orderContractCode == null) //package đã bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Contract code does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderContractCode.OrderId && !x.IsDelete);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type && x.Id != id);
            if ((listContractCode.Sum(x => x.TotalPrice) + totalPrice ?? 0) > order.TotalPrice + (order.FeeShip ?? 0))
            {
                return Json(new { status = MsgType.Error, msg = "Total amount customer pays must be larger than total amount company pays !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    //Kiểm tra Contract code có là duy nhất
                    if (order.ContractCode == orderContractCode.ContractCode)
                    {
                        order.ContractCode = code;
                    }

                    var array = order.UnsignName.Split(' ');
                    array = array.Where(x => x != orderContractCode.ContractCode.ToLower()).ToArray();

                    order.UnsignName = string.Join(" ", array) + " " + code;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    if (DataCompare(orderContractCode.ContractCode, code))
                    {
                        dataBefore.Add(new LogResult() { Name = "Contract code", Value = orderContractCode.ContractCode });
                        orderContractCode.ContractCode = code;
                        dataAfter.Add(new LogResult() { Name = "Contract code", Value = orderContractCode.ContractCode });
                    }
                    if (DataCompare(orderContractCode.TotalPrice, totalPrice))
                    {
                        dataBefore.Add(new LogResult() { Name = "Total", Value = orderContractCode.TotalPrice == null ? "0" : orderContractCode.TotalPrice.Value.ToString("N2") });
                        orderContractCode.TotalPrice = totalPrice ?? 0;
                        dataAfter.Add(new LogResult() { Name = "Total", Value = orderContractCode.TotalPrice.Value.ToString("N2") });
                    }

                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    var listOrderContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type && !x.IsDelete);

                    var priceShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);
                    if (priceShop > order.TotalPrice)
                    {
                        order.FeeShipBargain = order.FeeShipBargain ?? 0;
                        order.FeeShipBargain += priceShop - order.TotalPrice;
                    }

                    if (priceShop < 0)
                    {
                        order.FeeShipBargain = 0;
                    }

                    order.PaidShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);

                    var priceBargain = (order.TotalPrice + (order.FeeShip ?? 0)) - ((order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0));

                    order.PriceBargain = priceBargain < 0 ? 0 : priceBargain;

                    order.ContractCodes = listOrderContractCode.Select(x => x.ContractCode).ToString();

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
                        Content = "Contract code updated successfully",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    var listOrderExchange =
                        await
                            UnitOfWork.OrderExchangeRepo.FindAsync(
                                x =>
                                    !x.IsDelete && x.Status == (byte)OrderExchangeStatus.Approved &&
                                    x.Type == (byte)OrderExchangeType.Product && x.OrderId == order.Id);

                    var sum = listOrderExchange.Sum(x => x.TotalPrice);

                    if (order.Total < sum)
                    {
                        //Ghi công nợ
                        if (order.CustomerId != null)
                        {
                            var total = sum.Value - order.Total;
                            if (total > 0)
                            {
                                var autoUpdateDebitModel = new AutoUpdateDebitModel()
                                {
                                    SubjectId = order.CustomerId.Value, // Id khách hàng
                                    SubjectTypeIdd = (int)EnumAccountantSubject.Customer,
                                    Money = total, // Số tiền
                                    OrderId = order.Id, // Id Order
                                    PayReceivableIdd = (int)TreasureMustReturn.MissingLinkOrder,
                                    // Đinh khoản (Enum TreasureMustReturn)
                                    OrderType = order.Type,
                                    OrderCode = order.Code
                                };

                                var result = UnitOfWork.DebitRepo.UpdateDebit(autoUpdateDebitModel);

                                // Lỗi trong quá tình thực hiện công nợ
                                if (result.Status < 0)
                                {
                                    transaction.Rollback();
                                    return Json(new { status = MsgType.Error, msg = result.Msg },
                                        JsonRequestBehavior.AllowGet);
                                }
                            }
                        }
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Contract code edited successfully", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Edit Contract code]

        #endregion [6. Các thao tác với Contract code]

        #region [Lấy danh sách Order theo khách hàng]

        public async Task<JsonResult> GetOrderByCustomer(int page, int pageSize, int customerId)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy danh sách Order
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => x.CustomerId == customerId && !x.IsDelete,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );

            //3. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy danh sách Order theo khách hàng]

        #region @Henry Suggettion Order

        /// <summary>
        /// Suggetion Order cho tìm Order cho package mất mã
        /// </summary>
        /// <param name="term">Từ khóa</param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns>Danh sách Order</returns>
        [HttpPost]
        public async Task<ActionResult> Suggetion(string term, int page = 1, int pageSize = 6)
        {
            term = MyCommon.Ucs2Convert(term);

            long totalRecord = 0;
            var items = new List<Order>();

            if (UserState.OfficeType == (byte)OfficeType.Warehouse)
            {
                items = await UnitOfWork.OrderRepo.FindAsync(out totalRecord, x => !x.IsDelete &&
                                                                  x.UnsignName.Contains(term) &&
                                                                  x.Status != (byte)OrderStatus.Cancel &&
                                                                  x.Status != (byte)OrderStatus.Finish &&
                                                                  x.Status != (byte)OrderStatus.New &&
                                                                  x.Status != (byte)OrderStatus.Order &&
                                                                  x.WarehouseId == UserState.OfficeId.Value,
                x => x.OrderBy(o => o.Id), page, pageSize);
            }
            else if (UserState.OfficeType == (byte)OfficeType.Order || UserState.OfficeType == (byte)OfficeType.Deposit)
            {
                if (UserState.Type == 0)
                {
                    items = await UnitOfWork.OrderRepo
                        .FindAsync(out totalRecord, x => !x.IsDelete &&
                                                         x.UnsignName.Contains(term) &&
                                                         x.Status !=
                                                         (byte)OrderStatus.Cancel &&
                                                         //x.Status !=
                                                         //(byte)OrderStatus.Finish &&
                                                         x.Status !=
                                                         (byte)OrderStatus.New &&
                                                         x.Status !=
                                                         (byte)OrderStatus.Order &&
                                                         //x.UserId == UserState.UserId
                                                         //&&
                                                         (UserState.OfficeType == (byte)OfficeType.Deposit &&
                                                          x.Type == (byte)OrderType.Deposit ||
                                                          UserState.OfficeType != (byte)OfficeType.Deposit &&
                                                          x.Type != (byte)OrderType.Deposit),
                            x => x.OrderBy(o => o.Id), page, pageSize);
                }
                else
                {
                    items = await UnitOfWork.OrderRepo
                        .FindAsync(out totalRecord, x => !x.IsDelete &&
                                                         x.UnsignName.Contains(term) &&
                                                         x.Status !=
                                                         (byte)OrderStatus.Cancel &&
                                                         //x.Status !=
                                                         //(byte)OrderStatus.Finish &&
                                                         x.Status !=
                                                         (byte)OrderStatus.New &&
                                                         x.Status !=
                                                         (byte)OrderStatus.Order
                                                         &&
                                                         (UserState.OfficeType ==
                                                          (byte)OfficeType.Deposit &&
                                                          x.Type ==
                                                          (byte)OrderType.Deposit ||
                                                          UserState.OfficeType !=
                                                          (byte)OfficeType.Deposit &&
                                                           x.Type !=
                                                          (byte)OrderType.Deposit),
                            x => x.OrderBy(o => o.Id), page, pageSize);
                }
            }


            return JsonCamelCaseResult(new { totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> RecentSuggettion()
        {
            var items = await UnitOfWork.OrderRepo.RecentSuggetion(UserState.OfficeId ?? 0, UserState.UserId, RecentMode.Order);

            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> SaveSuggetionRecent(int orderId)
        {
            var recent = await
                    UnitOfWork.RecentRepo.SingleOrDefaultAsync(
                        x => x.RecordId == orderId && x.Mode == (byte)RecentMode.Order && x.UserId == UserState.UserId);

            if (recent != null)
            {
                recent.CountNo += 1;
                await UnitOfWork.RecentRepo.SaveAsync();

                return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
            }

            UnitOfWork.RecentRepo.Add(new Recent()
            {
                CountNo = 1,
                Mode = (byte)RecentMode.Order,
                RecordId = orderId,
                UserId = UserState.UserId
            });

            await UnitOfWork.RecentRepo.SaveAsync();

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region [Export Excel]
        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderOrder)]
        public async Task<ActionResult> ExportExcelOrder(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                    out totalRecord,
                    x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Order,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CREATION TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COST OF GOODS (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SHIPPING FEE WITHIN CHINA (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BARGAIN AMOUNT (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "AMOUNT PAID TO SHOP (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF LINKS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF PRODUCT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF IN CHARGE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF ORDERS", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.TotalPrice, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.FeeShip, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.PriceBargain, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.PaidShop, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.LinkNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.ProductNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserNote, ExcelHorizontalAlignment.Left, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderDelay)]
        public async Task<ActionResult> ExportExcelOrderDelay(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var listOrder = await UnitOfWork.OrderRepo.GetOrderDelayReasonAll(keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CREATION TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COST OF GOODS (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "SHIPPING FEE WITHIN CHINA (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BARGAIN AMOUNT (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "AMOUNT PAID TO SHOP (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF LINKS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF PRODUCT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF IN CHARGE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REASON OF DELAYED PROCESSING", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF DELAYED PROCESSING ORDERS ", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.TotalPrice, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.FeeShip, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.PriceBargain, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.PaidShop, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.LinkNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.ProductNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, $"{order.ReasonId}. {order.Reason}", ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserNote, ExcelHorizontalAlignment.Left, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_DELAY_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderOrder)]
        public async Task<ActionResult> ExportExcelOrderNew(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                    out totalRecord,
                    x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && (x.UnsignName.Contains(keyword) || x.ContractCodes.Contains(keyword))
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && x.Status == (byte)OrderStatus.WaitOrder
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    && x.Type == (byte)OrderType.Order,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "COST OF GOODS (CNY)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF LINKS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NUMBER OF PRODUCT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF NEW ORDERS", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, order.TotalPrice, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.LinkNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.ProductNo, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Note, ExcelHorizontalAlignment.Left, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_NEW_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderNoCodeOfLading)]
        public async Task<ActionResult> ExportExcelOrderNoCodeOfLading(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool isAllNoCodeOfLading)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                var listOrder = await UnitOfWork.OrderRepo.GetOrderNoContractCodeExcel(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);

                if (isAllNoCodeOfLading)
                {
                    listOrder = await UnitOfWork.OrderRepo.GetOrderNoContractCode3DayExcel(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);
                }

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WAYBILL CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TRANSACTION COMPLETED", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REASON FOR DELAYED GOODS DISPATCHING", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "HAS CONTACTED CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF DELAYED GOODS DISPATCHING ORDERS", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, "--", ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.LastUpdate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Note, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, null, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserNote, ExcelHorizontalAlignment.Left, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_NO_CODE_OF_LADING_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderNotEnoughInventory)]
        public async Task<ActionResult> ExportExcelOrderNotEnoughInventory(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool isAllNotEnoughInventory)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                var listOrder = await UnitOfWork.OrderRepo.GetOrderNotEnoughInventoryExcel(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);

                if (isAllNotEnoughInventory)
                {
                    listOrder = await UnitOfWork.OrderRepo.GetOrderNotEnoughInventory4DayExcel(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState);
                }

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WAYBILL CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TRANSACTION COMPLETED", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CHINA SHOP DISPATCHING GOODS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "PACKAGE STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REASON FOR DELAYED GOODS RECEIPTS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "HAS CONTACTED CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF ORDERS WITH DELAYED GOODS ARRIVAL", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        foreach (var item in order.Packages.Where(x => x.Status == (byte)OrderPackageStatus.ShopDelivery).ToList())
                        {
                            col = 1;
                            ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, item.TransportCode, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.UserEmail, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.LastUpdate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, item.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderPackageStatus>(item.Status), ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.Note, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, null, ExcelHorizontalAlignment.Left, true);
                            col++;
                            ExcelHelper.CreateCellTable(sheet, no, col, order.UserNote, ExcelHorizontalAlignment.Left, true);

                            no++;
                        }
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_NO_CODE_OF_LADING_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        [HttpPost]
        [LogTracker(EnumAction.Export, EnumPage.OrderAwaitingPayment)]
        public ActionResult ExportExcelOrderAwaitingPayment(string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                var listOrder = UnitOfWork.OrderRepo.GetOrderAccountant(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, UserState, false);

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TYPE OF BARGAIN", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "WEBSITE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TRANSACTION COMPLETED", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CONTRACT NUMBER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "LIST OF ACCOUNTANT DISBURSEMENT AWAITING ORDERS", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = dateStart?.ToShortDateString() ?? "__";
                var end = dateEnd?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listOrder.Any())
                {
                    foreach (var order in listOrder)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderStatus>(order.Status), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, (order.BargainType == null) ? "" : EnumHelper.GetEnumDescription<BargainType>((int)order.BargainType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.WebsiteName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.LastUpdate.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, $"{order.PackageNoInStock}/{order.PackageNo}", ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, order.UserNote, ExcelHorizontalAlignment.Left, true);

                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_AWAITING_PAYMENT_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        #endregion

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderUserWebsite)]
        public async Task<JsonResult> UpdateUserWebsite(int userId, string website)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var user = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => x.Id == userId && !x.IsDelete);

            //2. check điều kiện
            if (user == null) //does not exist or has been deleted
            {
                return Json(new { status = MsgType.Error, msg = "Staff does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    user.Websites = website;
                    user.Updated = timeNow;
                    await UnitOfWork.UserRepo.SaveAsync();

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
            return Json(new { status = MsgType.Success, msg = "Changed successfully!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderDelay)]
        public async Task<JsonResult> UpdateOrderBargainType(int orderId, byte bargainType)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist or has been deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }


            order.BargainType = bargainType;
            order.LastUpdate = timeNow;
            await UnitOfWork.OrderRepo.SaveAsync();

            //4. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Changed successfully!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OrderHistoryShop(string name)
        {
            var listOrder = UnitOfWork.DbContext.Orders.Where(x => !x.IsDelete && x.Type != (byte)OrderType.Deposit && x.ShopName == name && x.Status < (byte)OrderStatus.Cancel && x.Status > (byte)OrderStatus.OrderSuccess).ToList();
            var listOrderBargain = listOrder.Where(x => x.PriceBargain != null && x.PriceBargain != 0).ToList();

            //tiền mặc cả
            var listBargain = listOrderBargain.Count != 0 ? listOrderBargain.OrderByDescending(x => x.PriceBargain).Take(10).ToList() : listOrderBargain;

            //Tiền hàng
            var listPrice = listOrder.OrderByDescending(x => x.PaidShop).Take(10).ToList();

            //Tỉ lệ mặc cả 
            var listRatioPrice = listOrderBargain.Count != 0 ? listOrderBargain.OrderByDescending(x => x.PriceBargain / x.PaidShop).Take(10).ToList() : listOrderBargain;

            return Json(new { status = MsgType.Success, msg = "Successful!", listBargain, listPrice, listRatioPrice }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOrderView(int id)
        {
            var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == id);

            return Json(order, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FixOrderPackge()
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var array = new int[]
                    {
                        //id package
                    };
                    var list = UnitOfWork.OrderPackageRepo.Find(x => !x.IsDelete & x.OrderId > 0 && array.Contains(x.Id)).ToList();
                    var listHistory = new List<PackageHistory>();
                    foreach (var item in list)
                    {
                        listHistory.Add(new PackageHistory()
                        {
                            PackageId = item.Id,
                            PackageCode = item.Code,
                            OrderId = item.OrderId,
                            OrderCode = item.OrderCode,
                            Type = 1,
                            Status = 0,
                            Content = "Chinese shops dispatching goods",
                            CustomerId = item.CustomerId,
                            CustomerName = item.CustomerName,
                            UserId = item.UserId,
                            UserFullName = item.UserFullName,
                            CreateDate = DateTime.Now
                        });
                    }

                    UnitOfWork.PackageHistoryRepo.AddRange(listHistory);
                    UnitOfWork.PackageHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }


            return View();
        }
    }

    public class StatuShow
    {
        public int Status { get; set; }
        public int Count { get; set; }
    }
}