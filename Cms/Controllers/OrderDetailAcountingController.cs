using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Common.Emums;
using Cms.Attributes;
using Cms.Helpers;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.Settings;
using Library.UnitOfWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime.ExceptionServices;

namespace Cms.Controllers
{
    [Authorize]
    public class OrderDetailAcountingController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.OrderDetailAcounting)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type.HasValue && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager)
            {
                var warehouses = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x =>
                        ((isManager &&
                          ((x.IdPath == UserState.OfficeIdPath) || x.IdPath.StartsWith(UserState.OfficeIdPath + "."))) ||
                         (!isManager && (x.IdPath == UserState.OfficeIdPath))) &&
                        (x.Type == (byte)OfficeType.Warehouse) &&
                        !x.IsDelete && (x.Status == (byte)OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderDetailAcounting))
                .Cast<OrderDetailAcounting>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderDetailAcounting>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderDetailCountingStatus =
                JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderDetailCountingStatus))
                    .Cast<OrderDetailCountingStatus>()
                    .Select(
                        v =>
                            new
                            {
                                Id = (byte)v,
                                Name = EnumHelper.GetEnumDescription<OrderDetailCountingStatus>((int)v)
                            })
                    .ToList(), jsonSerializerSettings);

            ViewBag.OrderDetailCountingMode =
                JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderDetailCountingMode))
                    .Cast<OrderDetailCountingMode>()
                    .Select(
                        v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderDetailCountingMode>((int)v) })
                    .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            ViewBag.MaxFileLength = GetMaxFileLength();

            ViewBag.OrderStatusInWarehouse = (byte) OrderStatus.InWarehouse;

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.OrderDetailAcounting)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "",
            int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            int totalRecord;
            var items = await UnitOfWork.OrderDetailCountingRepo.Search(out totalRecord, warehouseIdPath, isManager, userId,
                    status, fromDate, toDate, keyword, currentPage, recordPerPage);

            var str = $";{string.Join(";", items.Select(x => x.OrderDetailId).ToList())};";

            var linkAcountings = await UnitOfWork.OrderDetailCountingRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && str.Contains(";" + x.OrderDetailId + ";"));

            return JsonCamelCaseResult(new { items, linkAcountings, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.OrderDetailAcounting)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(OrderAcountingMeta model)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage =
                    string.Join(", ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));

                return JsonCamelCaseResult(new { Status = -1, Text = $"Error: {errorMessage}" },
                    JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var o = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == model.OrderId && !x.IsDelete);

                    if (o == null)
                        return JsonCamelCaseResult(new { Status = -2, Text = "Order does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    if (o.Status != (byte)OrderStatus.InWarehouse)
                        return JsonCamelCaseResult(new { Status = -7, Text = "You cannot edit information of order of which goods is not in stock " },
                            JsonRequestBehavior.AllowGet);
                    var d =
                        await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == model.OrderDetailId);

                    if (d == null)
                        return JsonCamelCaseResult(new { Status = -3, Text = "Product link does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    if (d.OrderId != o.Id)
                        return JsonCamelCaseResult(new { Status = -4, Text = "Product link does not match the order" },
                            JsonRequestBehavior.AllowGet);

                    if (d.QuantityIncorrect == null || d.QuantityIncorrect.Value == 0)
                        return JsonCamelCaseResult(new { Status = -5, Text = "This product link does not have miscounting" },
                            JsonRequestBehavior.AllowGet);

                    if (model.QuantityLose > d.QuantityIncorrect.Value)
                        return JsonCamelCaseResult(new {Status = -5, Text = "Number of wrong products cannot be larger than number of miscounting"},
                            JsonRequestBehavior.AllowGet);

                    var orderAcounting = new OrderDetailCounting()
                    {
                        BeginAmount = d.BeginAmount,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        CustomerAddress = o.CustomerAddress,
                        CustomerEmail = o.CustomerEmail,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName,
                        CustomerPhone = o.CustomerPhone,
                        ExchangeRate = o.ExchangeRate,
                        Image = d.Image,
                        IsDelete = false,
                        ImageJson = model.ImageJson,
                        Link = d.Link,
                        Mode = model.Mode,
                        Name = d.Name,
                        Note = d.Note,
                        NotePrivate = model.Note,
                        UserId = UserState.UserId,
                        UserFullName = UserState.FullName,
                        OfficeId = UserState.OfficeId,
                        OfficeIdPath = UserState.OfficeIdPath,
                        OfficeName = UserState.OfficeName,
                        OrderCode = o.Code,
                        OrderDetailId = d.Id,
                        OrderId = o.Id,
                        ShopId = o.ShopId,
                        ShopLink = o.ShopLink,
                        ShopName = o.ShopName?.Trim() ?? "",
                        Quantity = d.Quantity,
                        ProductNo = o.LinkNo,
                        Properties = d.Properties,
                        QuantityLose = model.QuantityLose,
                        Status = (byte)OrderDetailCountingStatus.New,
                        WarehouseId = o.WarehouseId,
                        WarehouseName = o.WebsiteName,
                        WebsiteName = o.WebsiteName,
                        Price = d.Price,
                        ExchangePrice = d.ExchangePrice,
                        TotalPrice = d.TotalPrice,
                        TotalExchange = d.TotalExchange,
                        TotalPriceLose = model.QuantityLose * d.Price,
                        TotalExchangeLose = model.QuantityLose * d.ExchangePrice
                    };

                    UnitOfWork.OrderDetailCountingRepo.Add(orderAcounting);

                    var rs = await UnitOfWork.OrderDetailCountingRepo.SaveAsync();

                    if (rs > 0)
                    {
                        // Số lượng sản phẩm sai
                        //var detailCountingWrongs = await UnitOfWork.OrderDetailCountingRepo.FindAsync(
                        //    x => x.IsDelete == false && x.OrderDetailId == d.Id &&
                        //         x.Mode == (byte)OrderDetailCountingMode.Wrong);

                        // var wrongNo = detailCountingWrongs.Sum(x => x.QuantityLose) ?? 0;

                        // Kiểm đếm thiếu
                        var countingLose = await UnitOfWork.OrderDetailCountingRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.OrderDetailId == d.Id &&
                                 x.Mode == (byte)OrderDetailCountingMode.Lose);

                        var quantityMissing = d.QuantityBooked == null || d.QuantityRecived == null
                        ? 0
                        : d.QuantityRecived == 0
                            ? d.QuantityBooked.Value
                            : d.QuantityBooked.Value - d.QuantityRecived;

                        //var loseNo = d.QuantityBooked.HasValue == false
                        //    ? 0
                        //    : d.QuantityBooked.Value - ((d.QuantityActuallyReceived ?? 0) + wrongNo);

                        // Không có số lượng sản phẩm thiếu
                        if (quantityMissing <= 0 && countingLose == null)
                        {
                            transaction.Commit();
                            return JsonCamelCaseResult(new { Status = 1, Text = "Updated successfully" }, JsonRequestBehavior.AllowGet);
                        }

                        // Xóa kiểm đếm thiếu
                        if (quantityMissing <= 0 && countingLose != null)
                        {
                            countingLose.IsDelete = true;
                        }
                        else if (countingLose == null)
                        {
                            countingLose = new OrderDetailCounting()
                            {
                                BeginAmount = d.BeginAmount,
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                CustomerAddress = o.CustomerAddress,
                                CustomerEmail = o.CustomerEmail,
                                CustomerId = o.CustomerId,
                                CustomerName = o.CustomerName,
                                CustomerPhone = o.CustomerPhone,
                                ExchangeRate = o.ExchangeRate,
                                Image = d.Image,
                                IsDelete = false,
                                Link = d.Link,
                                Mode = (byte)OrderDetailCountingMode.Lose,
                                Name = d.Name,
                                Note = d.Note,
                                NotePrivate = "Products are missing",
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                OfficeId = UserState.OfficeId,
                                OfficeIdPath = UserState.OfficeIdPath,
                                OfficeName = UserState.OfficeName,
                                OrderCode = o.Code,
                                OrderDetailId = d.Id,
                                OrderId = o.Id,
                                ShopId = o.ShopId,
                                ShopLink = o.ShopLink,
                                ShopName = o.ShopName?.Trim() ?? "",
                                Quantity = d.QuantityBooked ?? 0,
                                ProductNo = o.LinkNo,
                                Properties = d.Properties,
                                QuantityLose = quantityMissing,
                                Status = (byte)OrderDetailCountingStatus.New,
                                WarehouseId = o.WarehouseId,
                                WarehouseName = o.WebsiteName,
                                WebsiteName = o.WebsiteName,
                                Price = d.Price,
                                ExchangePrice = d.ExchangePrice,
                                TotalPrice = d.TotalPrice,
                                TotalExchange = d.TotalExchange,
                                TotalPriceLose = (decimal)quantityMissing * d.Price,
                                TotalExchangeLose = (decimal)quantityMissing * d.ExchangePrice
                            };

                            UnitOfWork.OrderDetailCountingRepo.Add(countingLose);
                        }
                        else
                        {
                            countingLose.QuantityLose = quantityMissing;
                            countingLose.TotalPriceLose = (decimal)quantityMissing * d.Price;
                            countingLose.TotalExchangeLose =(decimal)quantityMissing * d.ExchangePrice;
                            countingLose.Updated = DateTime.Now;
                        }

                        var rs2 = await UnitOfWork.OrderDetailCountingRepo.SaveAsync();

                        var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(o.Type == (byte)OrderType.Order ? (byte)OfficeType.Order : (byte)OfficeType.Deposit)}");

                        // Tạo phiếu thiếu sản phẩm
                        if (rs2 > 0)
                        {
                            // Thông báo cho nhân viên kho
                            if (o.UserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(o.UserId.Value,
                                    $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                    $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                    $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                    $"{d.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                        $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                        $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                        $"{d.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                                }
                                else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow)
                                {
                                    if (o.UserId != u.UserId)
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                            $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                            $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                            $"{d.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                                }
                            }
                        }

                        // Thông báo đến nhân viên đặt hàng khi có wrong tally hàng hóa
                        // Thông báo cho nhân viên kho
                        if (o.UserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                        {
                            NotifyHelper.CreateAndSendNotifySystemToClient(o.UserId.Value,
                                $"{UserState.FullName} requests to handle miscounting", EnumNotifyType.Warning,
                                $"{UserState.FullName} requests to handle miscounting {model.QuantityLose}" +
                                $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                        }

                        // Thông báo tới nhân viên trong cấu hình
                        foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                        {
                            if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                    $"{UserState.FullName} requests to handle miscounting", EnumNotifyType.Warning,
                                    $"{UserState.FullName} requests to handle miscounting {model.QuantityLose}" +
                                    $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                    $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                            }
                            else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                            {
                                if (o.UserId != u.UserId)
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} requests to handle miscounting", EnumNotifyType.Warning,
                                        $"{UserState.FullName} requests to handle miscounting {model.QuantityLose}" +
                                        $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                        $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                            }
                        }

                        // Thông báo đến CSHKH
                        var notifySupport = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.CustomerCare}");

                        if (o.CustomerCareUserId.HasValue && (notifySupport.Settings.IsFollow || !notifySupport.Settings.Users.Any()))
                        {
                            NotifyHelper.CreateAndSendNotifySystemToClient(o.CustomerCareUserId.Value,
                                $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                $"#{MyCommon.ReturnCode(o.Code)} miscount {model.QuantityLose}" +
                                $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                        }

                        // Thông báo tới nhân viên trong cấu hình
                        foreach (var u in notifySupport.Settings.Users.Where(x => x.IsNotify))
                        {
                            if (u.UserId != default(int) && notifySupport.Settings.IsFollow == false)
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                    $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                    $"#{MyCommon.ReturnCode(o.Code)} miscount {model.QuantityLose}" +
                                    $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                    $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                            }
                            else if (u.UserId != default(int) && notifySupport.Settings.IsFollow == true)
                            {
                                if (o.CustomerCareUserId != u.UserId)
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                        $"#{MyCommon.ReturnCode(o.Code)} miscount {model.QuantityLose}" +
                                        $" product with link ID #{d.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                        $"{d.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
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

            return JsonCamelCaseResult(new { Status = 1, Text = "Added successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.OrderDetailAcounting)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrderDetail(int orderDetailId, string note, int quantityRecived, int? quantityIncorrect)
        {
            List<OrderDetailCounting> linkAcountings;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var orderDetail =
                        await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => x.Id == orderDetailId && !x.IsDelete);

                    if (orderDetail == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Product does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    var o = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderDetail.OrderId && !x.IsDelete);

                    if (o == null)
                        return JsonCamelCaseResult(new { Status = -2, Text = "Order does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    //if (o.Status != (byte)OrderStatus.GoingDelivery)
                    //    return JsonCamelCaseResult(new { Status = -7, Text = "You can not Edit thông tin Order created a delivery note" },
                    //        JsonRequestBehavior.AllowGet);

                    if (orderDetail.QuantityBooked.HasValue && quantityRecived > orderDetail.QuantityBooked)
                        return JsonCamelCaseResult(new
                            {
                                Status = -3,
                                Text = "The number of products received is not greater than the number of products ordered"
                        },
                            JsonRequestBehavior.AllowGet);

                    if (quantityRecived < 0 || quantityIncorrect.HasValue && quantityIncorrect.Value < 0)
                        return JsonCamelCaseResult(new
                            {
                                Status = -3,
                                Text = "Amount received and the number of wrong is <0"
                            }, JsonRequestBehavior.AllowGet);

                    var quantityMissing = orderDetail.QuantityBooked == null
                        ? 0
                        : quantityRecived == 0
                            ? orderDetail.QuantityBooked.Value
                            : orderDetail.QuantityBooked.Value - quantityRecived;

                    orderDetail.QuantityRecived = quantityRecived;
                    orderDetail.QuantityIncorrect = quantityIncorrect;
                    orderDetail.QuantityActuallyReceived = quantityRecived == 0
                        ? 0
                        : quantityIncorrect == null || quantityIncorrect.Value == 0
                            ? quantityRecived
                            : quantityRecived - quantityIncorrect.Value;

                    orderDetail.UserNote = note;
                    orderDetail.CountingTime = DateTime.Now;
                    orderDetail.CountingFullName = UserState.FullName;
                    orderDetail.CountingOfficeId = UserState.OfficeId;
                    orderDetail.CountingOfficeIdPath = UserState.OfficeIdPath;
                    orderDetail.CountingOfficeName = UserState.OfficeName;
                    orderDetail.CountingUserId = UserState.UserId;
                    orderDetail.CountingUserName = UserState.UserName;

                    var rs = await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // Số lượng khác số lượng đặt được
                    if (rs > 0)
                    {
                        // Số lượng sản phẩm sai
                        //var detailCountingWrongs = await UnitOfWork.OrderDetailCountingRepo.FindAsync(
                        //        x => x.IsDelete == false && x.OrderDetailId == orderDetail.Id &&
                        //            x.Mode == (byte)OrderDetailCountingMode.Wrong);

                        // var wrongNo = detailCountingWrongs.Sum(x => x.QuantityLose) ?? 0;

                        // Kiểm đếm thiếu
                        var countingLose = await UnitOfWork.OrderDetailCountingRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.OrderDetailId == orderDetail.Id &&
                                 x.Mode == (byte)OrderDetailCountingMode.Lose);

                        //var loseNo = orderDetail.QuantityBooked.HasValue == false
                        //    ? 0
                        //    : orderDetail.QuantityBooked.Value - (orderDetail.QuantityActuallyReceived.Value + wrongNo);

                        // Không có số lượng sản phẩm thiếu
                        if (quantityMissing <= 0 && countingLose == null)
                        {
                            transaction.Commit();

                            linkAcountings = await UnitOfWork.OrderDetailCountingRepo.FindAsNoTrackingAsync(
                                x => !x.IsDelete && x.OrderDetailId == orderDetailId);

                            return JsonCamelCaseResult(new { Status = 1, Text = "Updated successfully", linkAcountings }, JsonRequestBehavior.AllowGet);
                        }

                        // Xóa kiểm đếm thiếu
                        if (quantityMissing <= 0 && countingLose != null)
                        {
                            countingLose.IsDelete = true;
                        }
                        else if (countingLose == null)
                        {
                            countingLose = new OrderDetailCounting()
                            {
                                BeginAmount = orderDetail.BeginAmount,
                                Created = DateTime.Now,
                                Updated = DateTime.Now,
                                CustomerAddress = o.CustomerAddress,
                                CustomerEmail = o.CustomerEmail,
                                CustomerId = o.CustomerId,
                                CustomerName = o.CustomerName,
                                CustomerPhone = o.CustomerPhone,
                                ExchangeRate = o.ExchangeRate,
                                Image = orderDetail.Image,
                                IsDelete = false,
                                Link = orderDetail.Link,
                                Mode = (byte)OrderDetailCountingMode.Lose,
                                Name = orderDetail.Name,
                                Note = orderDetail.Note,
                                NotePrivate = "Products are missing",
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                OfficeId = UserState.OfficeId,
                                OfficeIdPath = UserState.OfficeIdPath,
                                OfficeName = UserState.OfficeName,
                                OrderCode = o.Code,
                                OrderDetailId = orderDetail.Id,
                                OrderId = o.Id,
                                ShopId = o.ShopId,
                                ShopLink = o.ShopLink,
                                ShopName = o.ShopName?.Trim() ?? "",
                                Quantity = orderDetail.QuantityBooked ?? 0,
                                ProductNo = o.LinkNo,
                                Properties = orderDetail.Properties,
                                QuantityLose = quantityMissing,
                                Status = (byte)OrderDetailCountingStatus.New,
                                WarehouseId = o.WarehouseId,
                                WarehouseName = o.WebsiteName,
                                WebsiteName = o.WebsiteName,
                                Price = orderDetail.Price,
                                ExchangePrice = orderDetail.ExchangePrice,
                                TotalPrice = orderDetail.TotalPrice,
                                TotalExchange = orderDetail.TotalExchange,
                                TotalPriceLose = quantityMissing * orderDetail.Price,
                                TotalExchangeLose = quantityMissing * orderDetail.ExchangePrice
                            };

                            UnitOfWork.OrderDetailCountingRepo.Add(countingLose);
                        }
                        else
                        {
                            countingLose.QuantityLose = quantityMissing;
                            countingLose.TotalPriceLose = quantityMissing * orderDetail.Price;
                            countingLose.TotalExchangeLose = quantityMissing * orderDetail.ExchangePrice;
                            countingLose.Updated = DateTime.Now;
                        }

                        var rs2 = await UnitOfWork.OrderDetailCountingRepo.SaveAsync();

                        // Tạo phiếu thiếu sản phẩm
                        if (rs2 > 0)
                        {
                            var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(o.Type == (byte)OrderType.Order ? (byte)OfficeType.Order : (byte)OfficeType.Deposit)}");

                            // Thông báo đến nhân viên đặt hàng khi có wrong tally hàng hóa
                            // Thông báo cho nhân viên kho
                            if (o.UserId.HasValue && (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any()))
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(o.UserId.Value,
                                    $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                    $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                    $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                    $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                        $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                        $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                        $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                                }
                                else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == true)
                                {
                                    if (o.UserId != u.UserId)
                                        NotifyHelper.CreateAndSendNotifySystemToClient(o.UserId.Value,
                                            $"{UserState.FullName} Goods undercounting handling request", EnumNotifyType.Warning,
                                            $"{UserState.FullName} Goods undercounting handling request {quantityMissing}" +
                                            $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                            $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Lose}", Url.Action("Index", "AcountingLose"));
                                }
                            }

                            // Thông báo đến CSHKH
                            var notifySupport = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.CustomerCare}");

                            if (o.CustomerCareUserId.HasValue && (notifySupport.Settings.IsFollow || !notifySupport.Settings.Users.Any()))
                            {
                                NotifyHelper.CreateAndSendNotifySystemToClient(o.CustomerCareUserId.Value,
                                    $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                    $"#{MyCommon.ReturnCode(o.Code)} miscount {quantityMissing}" +
                                    $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                    $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                            }

                            // Thông báo tới nhân viên trong cấu hình
                            foreach (var u in notifySupport.Settings.Users.Where(x => x.IsNotify))
                            {
                                if (u.UserId != default(int) && notifySupport.Settings.IsFollow == false)
                                {
                                    NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                        $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                        $"#{MyCommon.ReturnCode(o.Code)} miscount {quantityMissing}" +
                                        $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                        $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
                                }
                                else if (u.UserId != default(int) && notifySupport.Settings.IsFollow == true)
                                {
                                    if (o.CustomerCareUserId != u.UserId)
                                        NotifyHelper.CreateAndSendNotifySystemToClient(u.UserId,
                                            $"#{MyCommon.ReturnCode(o.Code)} There is order miscounting", EnumNotifyType.Warning,
                                            $"#{MyCommon.ReturnCode(o.Code)} miscount {quantityMissing}" +
                                            $" product with link ID #{orderDetail.Id} in order #{MyCommon.ReturnCode(o.Code)}. <a href=\"{Url.Action("Index", "AcountingLose")}\" title=\"See details\">See more</a>",
                                            $"{orderDetail.Id}_{(byte)OrderDetailCountingMode.Wrong}", Url.Action("Index", "AcountingLose"));
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

            linkAcountings = await UnitOfWork.OrderDetailCountingRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && x.OrderDetailId == orderDetailId);

            return JsonCamelCaseResult(new { Status = 1, Text = "Updated successfully", linkAcountings }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetLinkCountings(int id)
        {
            var linkAcountings = await UnitOfWork.OrderDetailCountingRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && x.OrderDetailId == id);

            return JsonCamelCaseResult(linkAcountings, JsonRequestBehavior.AllowGet);
        }
    }
}