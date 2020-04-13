using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Cms.Helpers;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;

namespace Cms.Controllers
{
    [Authorize]
    public class OrderRefundController : BaseController
    {
        // GET: OrderRefund
        [LogTracker(EnumAction.View, EnumPage.OrderRefundPrice)]
        public ActionResult Index()
        {
            ViewBag.Mode = 0;
            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.OrderRefundProduct)]
        public ActionResult Product()
        {
            ViewBag.Mode = 1;
            return View("Index");
        }

        [CheckPermission(EnumAction.View, EnumPage.OrderRefundPrice, EnumPage.OrderRefundProduct)]
        public async Task<ActionResult> Search(byte? status, byte mode, DateTime? fromDate, DateTime? toDate,
            string keyword = "", int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            long totalRecord;

            var items = await UnitOfWork.OrderRefundRepo.Search(status, mode, fromDate, toDate,
                    keyword, currentPage, recordPerPage, out totalRecord);

            return JsonCamelCaseResult(new { items, totalRecord },
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetDetail(int refundId)
        {
            var refund =
                await UnitOfWork.OrderRefundRepo.GetById(refundId);

            if (refund == null)
                return JsonCamelCaseResult(null, JsonRequestBehavior.AllowGet);

            var refundDetails = await UnitOfWork.OrderRefundDetailRepo.GetByRefundId(refundId);

            return JsonCamelCaseResult(new {refund, refundDetails }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Add, EnumPage.OrderRefundPrice, EnumPage.OrderRefundProduct)]
        public async Task<ActionResult> Add(OrderRefundMeta model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join(", ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {messages}" },
                   JsonRequestBehavior.AllowGet);
            }
               
            if (model.Mode == 0 && UserState.OfficeType.HasValue &&
                UserState.OfficeType.Value != (byte) OfficeType.Order)
                return JsonCamelCaseResult(new {Status = -1, Text = "Only new orders have the right to create a refund tracking ticket"},
                    JsonRequestBehavior.AllowGet);

            if (model.Mode == 1 && UserState.OfficeType.HasValue &&
                UserState.OfficeType.Value != (byte)OfficeType.Warehouse)
                return JsonCamelCaseResult(new { Status = -1, Text = "Only warehouse employees have the right to create a return ticket" },
                    JsonRequestBehavior.AllowGet);

            string modeName;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == model.OrderId && x.IsDelete == false);

                    if (order == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Order does not exist or has been deleted!" },
                            JsonRequestBehavior.AllowGet);

                    if (model.Items == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Wrong detail tally is required to enter" },
                            JsonRequestBehavior.AllowGet);

                    var orderDetailCountingIds = $";{string.Join(";", model.Items.Select(x => x.OrderDetailCountingId))};";

                    var orderDetailCountings = await UnitOfWork.OrderDetailCountingRepo.OrderDetailCountingNotHaveRefund(order.Id, orderDetailCountingIds);

                    if (!orderDetailCountings.Any())
                        return JsonCamelCaseResult(new { Status = -1, Text = "Wrong detail tally does not exist or has been redeemed / redeemed" },
                            JsonRequestBehavior.AllowGet);

                    if (orderDetailCountings.Count != model.Items.Count)
                    {
                        return JsonCamelCaseResult(new { Status = -1, Text = "Wrong detail tally does not exist or has been redeemed / redeemed" },
                           JsonRequestBehavior.AllowGet);
                    }

                    if (model.Mode == 0 && UserState.Type == 0 && order.UserId != UserState.UserId)
                        return JsonCamelCaseResult(
                            new { Status = -1, Text = "You do not have permission to perform actions with this order" },
                            JsonRequestBehavior.AllowGet);

                    var timeNow = DateTime.Now;

                    var linkNo = orderDetailCountings.Select(x => x.OrderDetailId).Distinct().Count();
                    var productNo = model.Items.Sum(x => x.QuantityLose);

                    var orderRefund = new OrderRefund()
                    {
                        OrderId = order.Id,
                        IsDelete = false,
                        Status = 0,
                        CreateOfficeId = UserState.OfficeId,
                        CreateOfficeIdPath = UserState.OfficeIdPath,
                        CreateOfficeName = UserState.OfficeName,
                        CreateUserFullName = UserState.FullName,
                        CreateUserId = UserState.UserId,
                        CreateUserName = UserState.UserName,
                        Created = timeNow,
                        Updated = timeNow,
                        Note = model.Note,
                        Mode = model.Mode,
                        LinkNo = linkNo,
                        ProductNo = productNo,
                        CommentNo = 0,
                        AmountActual = model.AmountActual,
                        Code = string.Empty,
                        Amount = 0,
                        Percent =  0,
                        TotalAcount =  0,
                        UnsignText = string.Empty
                    };

                    UnitOfWork.OrderRefundRepo.Add(orderRefund);

                    await UnitOfWork.OrderRefundRepo.SaveAsync();

                    // Cập nhật lại Mã cho Order và Sum tiền
                    var refundOfDay = UnitOfWork.OrderRefundRepo.Count(x =>
                        (x.Created.Year == timeNow.Year) && (x.Created.Month == timeNow.Month) &&
                        (x.Created.Day == timeNow.Day) && (x.Id <= orderRefund.Id));

                    orderRefund.Code = $"{refundOfDay}{timeNow:ddMMyy}";
                    orderRefund.UnsignText = MyCommon.Ucs2Convert(
                        $"{orderRefund.Code} {orderRefund.CreateUserFullName} {orderRefund.CreateUserName}" +
                        $" {order.Code} {order.CustomerPhone} {order.CustomerEmail} {order.CustomerName}");

                    var orderRefundDetails = new List<OrderRefundDetail>();
                    foreach (var d in model.Items)
                    {
                        var c = orderDetailCountings.Single(x => x.Id == d.OrderDetailCountingId);

                        var m = new OrderRefundDetail()
                        {
                            BeginAmount = c.BeginAmount,
                            Created = timeNow,
                            ExchangeRate = c.ExchangeRate,
                            Image = c.Image,
                            IsDelete = false,
                            Link = c.Link,
                            Name = c.Name,
                            Note = d.Note,
                            OrderDetailCountingId = d.OrderDetailCountingId,
                            OrderRefundId = orderRefund.Id,
                            Properties = c.Properties,
                            Price = c.Price,
                            Updated = timeNow,
                            Quantity = c.QuantityLose ?? 0,
                            QuantityLose = d.QuantityLose,
                            ProductNo = c.ProductNo,
                            TotalPriceShop = c.TotalPriceShop,
                            TotalExchangeShop = c.TotalExchangeShop,
                            TotalPriceCustomer = c.TotalPriceCustomer,
                        };

                        // ReSharper disable once PossibleInvalidOperationException
                        m.TotalPrice = m.Quantity.Value * m.Price;
                        m.TotalExchange = m.TotalPrice * m.ExchangeRate;

                        // ReSharper disable once PossibleInvalidOperationException
                        m.TotalPriceLose = m.QuantityLose.Value * m.Price;
                        m.TotalExchangeLose = m.TotalPriceLose * m.ExchangeRate;

                        orderRefundDetails.Add(m);
                    }

                    UnitOfWork.OrderRefundDetailRepo.AddRange(orderRefundDetails);

                    await UnitOfWork.OrderRefundDetailRepo.SaveAsync();

                    var refundAmount = await UnitOfWork.OrderRefundDetailRepo.Entities.Where(
                            x => x.IsDelete == false && x.OrderRefundId == orderRefund.Id).SumAsync(x => x.TotalPriceLose);

                    orderRefund.AmountActual = model.AmountActual;
                    orderRefund.Amount = refundAmount;

                    //if (orderRefund.AmountActual > orderRefund.Amount)
                    //{
                    //    transaction.Rollback();

                    //    return JsonCamelCaseResult(new { Status = -1, Text = "Số tiền hoàn bạn nhập không được lớn hơn số tiền hoàn thực tế" },
                    //       JsonRequestBehavior.AllowGet);
                    //}

                    orderRefund.Percent = orderRefund.AmountActual * 100 / orderRefund.Amount;

                    await UnitOfWork.OrderRefundDetailRepo.SaveAsync();

                    #region Thông báo
                    modeName = model.Mode == 0 ? "Refund" : "goods returns";
                    var url = model.Mode == 0 ? Url.Action("Index", "OrderRefund") : Url.Action("Product", "OrderRefund");

                    // Thông báo tới đặt hàng
                    if (UserState.UserId != order.UserId && order.UserId.HasValue)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                                           $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
                                           $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
                                           $" of order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                                           $"{order.Code}_Refund", url);
                    }

                    // Thông báo tới nhân viên kho
                    //foreach (var i in orderDetailCountings)
                    //{
                    //    if (UserState.UserId != i.UserId && i.UserId.HasValue)
                    //    {
                    //        NotifyHelper.CreateAndSendNotifySystemToClient(i.UserId.Value,
                    //                       $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
                    //                       $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
                    //                       $" of order #{MyCommon.ReturnCode(order.Type, order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                    //                       $"{order.Code}_Refund", url);
                    //    }
                    //}

                    // Thông báo tới kế toán - Phiếu theo dõi Refund
                    // todo: Thông báo kế toán (- Có thể Edit để chỉ thông báo tới 1 kế toán cố định)
                    if (model.Mode == 0)
                    {
                        var usersAccountancy =
                            UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                                position => position.IsDefault && position.Type == 1,
                                office => office.Type == (byte)OfficeType.Accountancy);

                        foreach (var u in usersAccountancy)
                        {
                            if (u.Id == UserState.UserId)
                                continue;

                            NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
                                $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
                                $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
                                $" of order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                                $"{order.Code}_Refund", url);
                        }
                    }
                    else
                    {
                        // Thông báo tới CSKH
                        if (order.CustomerCareUserId.HasValue && order.CustomerCareUserId != UserState.UserId)
                        {
                            // Thông báo tới chăm sóc khác hàng
                            NotifyHelper.CreateAndSendNotifySystemToClient(order.CustomerCareUserId.Value,
                                               $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
                                               $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
                                               $" of order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                                               $"{order.Code}_Refund", url);
                        }
                        else if (order.CustomerCareUserId == null)
                        {
                            // Thông báo đến trưởng phòng chăm sóc khách hàng
                            var users = UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                                position => position.IsDefault && position.Type == 1, office => office.Type == (byte)OfficeType.Order);

                            foreach (var u in users)
                            {
                                if (u.Id == UserState.UserId)
                                    continue;

                                NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
                                               $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
                                               $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
                                               $" of order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                                               $"{order.Code}_Refund", url);
                            }
                        }
                    }
                    #endregion

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = $"Create single forms {modeName} successful"},
                    JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Add, EnumPage.OrderRefundPrice, EnumPage.OrderRefundProduct)]
        public async Task<ActionResult> GetOrderCoutingLose(int orderId)
        {
            var orderDetailCountings = await UnitOfWork.OrderDetailCountingRepo.OrderDetailCountingNotHaveRefund(orderId);

            return JsonCamelCaseResult(orderDetailCountings, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateCommentNo(int refundId)
        {
            int rs;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var refund = await UnitOfWork.OrderRefundRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.OrderId == refundId);

                    refund.CommentNo += 1;

                    rs = await UnitOfWork.OrderRefundRepo.SaveAsync();
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
                return JsonCamelCaseResult(new { Status = -3, Text = "Updating number of comments is unsuccessful" },
                            JsonRequestBehavior.AllowGet);

            return JsonCamelCaseResult(new { Status = 1, Text = "Updated successfully" },
                            JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateStatus(int refundId, byte status)
        {
            var refund = await UnitOfWork.OrderRefundRepo.SingleOrDefaultAsync(x => x.Id == refundId && x.IsDelete == false);

            if (refund == null)
                return JsonCamelCaseResult(
                    new { Status = -1, Text = "The ticket does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            if (refund.Mode == 0 && UserState.OfficeType != (byte)OfficeType.Accountancy)
                return JsonCamelCaseResult(
                    new { Status = -1, Text = "Only accountants have the right to change their status" },
                    JsonRequestBehavior.AllowGet);

            if (refund.Mode == 1 && UserState.OfficeType != (byte)OfficeType.Order)
                return JsonCamelCaseResult(
                    new { Status = -1, Text = "Only new orders have the right to change the status" },
                    JsonRequestBehavior.AllowGet);


            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == refund.OrderId && x.IsDelete == false);

            if (order == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Order does not exist or has been deleted!" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //foreach (var i in items)
                    //{
                    //    i.Status = status;
                    //    i.Note = note;
                    //    i.Updated = DateTime.Now;
                    //}

                    refund.Status = status;
                    refund.Updated = DateTime.Now;

                    await UnitOfWork.OrderRefundRepo.SaveAsync();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            var modeName = refund.Mode == 0 ? "Refund" : "goods returns";
            var url = refund.Mode == 0 ? Url.Action("Index", "OrderRefund") : Url.Action("Product", "OrderRefund");

            // Thông báo tới ngời tạo
            if (UserState.UserId != order.UserId && refund.CreateUserId.HasValue)
            {
                NotifyHelper.CreateAndSendNotifySystemToClient(refund.CreateUserId.Value,
                                   $"{UserState.FullName} Update tracking ticket {modeName}", EnumNotifyType.Warning,
                                   $"{UserState.FullName} Update tracking ticket {modeName} tally wrong" +
                                   $" of order #{MyCommon.ReturnCode(order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
                                   $"{order.Code}_Refund", url);
            }

            //// Thông báo tới kế toán - Phiếu theo dõi Refund
            //// todo: Thông báo kế toán (- Có thể Edit để chỉ thông báo tới 1 kế toán cố định)
            //if (refund.Mode == 0)
            //{
            //    var usersAccountancy =
            //        UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
            //            position => position.IsDefault && position.Type == 1,
            //            office => office.Type == (byte)OfficeType.Accountancy);

            //    foreach (var u in usersAccountancy)
            //    {
            //        if (u.Id == UserState.UserId)
            //            continue;

            //        NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
            //            $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
            //            $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
            //            $" of order #{MyCommon.ReturnCode(order.Type, order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
            //            $"{order.Code}_Refund", url);
            //    }
            //}
            //else
            //{
            //    // Thông báo tới CSKH
            //    if (order.CustomerCareUserId.HasValue && order.CustomerCareUserId != UserState.UserId)
            //    {
            //        // Thông báo tới chăm sóc khác hàng
            //        NotifyHelper.CreateAndSendNotifySystemToClient(order.CustomerCareUserId.Value,
            //                           $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
            //                           $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
            //                           $" of order #{MyCommon.ReturnCode(order.Type, order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
            //                           $"{order.Code}_Refund", url);
            //    }
            //    else if (order.CustomerCareUserId == null)
            //    {
            //        // Thông báo đến trưởng phòng chăm sóc khách hàng
            //        var users = UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
            //            position => position.IsDefault && position.Type == 1, office => office.Type == (byte)OfficeType.Order);

            //        foreach (var u in users)
            //        {
            //            if (u.Id == UserState.UserId)
            //                continue;

            //            NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
            //                           $"{UserState.FullName} Create a tracking ticket {modeName}", EnumNotifyType.Warning,
            //                           $"{UserState.FullName} Create a tracking ticket {modeName} wrong tally" +
            //                           $" of order #{MyCommon.ReturnCode(order.Type, order.Code)}. <a href=\"{url}\" title=\"See details\">See more</a>",
            //                           $"{order.Code}_Refund", url);
            //        }
            //    }
            //}

            return JsonCamelCaseResult(new { Status = 1, Text = "Updated successfully" },
                JsonRequestBehavior.AllowGet);
        }
    }
}