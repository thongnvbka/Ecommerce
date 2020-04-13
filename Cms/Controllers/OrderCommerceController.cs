using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Cms.Helpers;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Newtonsoft.Json;
using System.Runtime.ExceptionServices;

namespace Cms.Controllers
{
    public class OrderCommerceController : BaseController
    {
        /// <summary>
        /// Lấy danh sách Order chờ báo giá của chăm sóc khách hàng
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
        [LogTracker(EnumAction.View, EnumPage.OrderCommerce)]
        public async Task<JsonResult> GetOrderCommerce(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
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
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    //&& (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Commerce,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status) && (systemId == -1 || x.SystemId == systemId) && x.Code == (keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete && (customerId == null || x.CustomerId == customerId) && (userId == null || x.UserId == userId)
                    //&& (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && x.Type == (byte)OrderType.Commerce,
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
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.View, EnumPage.OrderCommerce)]
        [HttpPost]
        public JsonResult GetOrderAdd()
        {
            //1. Khởi tạo dữ liệu
            var listOrderService = new List<OrderCommerceServiceResult>();

            //dịch vụ kiểm đếm
            var autditService = new OrderCommerceServiceResult()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.Audit,
                ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                Value = 0,
                Currency = Currency.VND.ToString(),
                Type = (byte)UnitType.Money,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Option,
                Checked = false
            };
            listOrderService.Add(autditService);

            //dịch vụ Packing
            var packingService = new OrderCommerceServiceResult()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.Packing,
                ServiceName = OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                Value = 0,
                Currency = Currency.CNY.ToString(),
                Type = (byte)UnitType.Money,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Option,
                Checked = false
            };
            listOrderService.Add(packingService);

            ////dịch vụ chuyển bằng đường hàng không
            //var fastDeliveryService = new OrderCommerceServiceResult()
            //{
            //    OrderId = 0,
            //    ServiceId = (byte)OrderServices.FastDelivery,
            //    ServiceName = OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
            //    ExchangeRate = ExchangeRate(),
            //    Value = 0,
            //    Currency = Currency.VND.ToString(),
            //    Type = (byte)UnitType.Money,
            //    TotalPrice = 0,
            //    Mode = (byte)OrderServiceMode.Option,
            //    Checked = false
            //};
            //listOrderService.Add(fastDeliveryService);

            //dịch vụ phí ship trung quốc
            var orderShopShippingService = new OrderCommerceServiceResult()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.ShopShipping,
                ServiceName =
                                    (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                Value = 0,
                Currency = Currency.CNY.ToString(),
                Type = (byte)UnitType.Money,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Required,
                Checked = false
            };
            listOrderService.Add(orderShopShippingService);

            //2. Lấy dữ liệu

            //3. Gửi dữ liệu lên view
            return Json(new { listOrderService }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.View, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> GetOrderDetail(int id)
        {
            var status = MsgType.Success;
            var msg = "";

            //1. Lấy thông tin Order Order
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            //2. Check Order có tồn tại hay đã bị xóa
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin Detail của Order
            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == id && !x.IsDelete);
            var listOrderService = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == id && !x.IsDelete);
            var toAddress = await UnitOfWork.OrderAddressRepo.FirstOrDefaultAsync(x => x.Id == order.ToAddressId);
            var formAddress = await UnitOfWork.OrderAddressRepo.FirstOrDefaultAsync(x => x.Id == order.FromAddressId);

            var oderExch =
                UnitOfWork.OrderExchangeRepo.FirstOrDefault(
                    x => !x.IsDelete && x.OrderId == order.Id && x.Type == (byte)OrderExchangeType.Pay);

            var orderExchange = oderExch == null ? 0 : oderExch.TotalPrice;
            var userOrder = order.UserId != null ? await UnitOfWork.UserRepo.GetUserToOfficeOrder(order.UserId.Value) : null;
            var listWarehouse = await UnitOfWork.OfficeRepo.FindAsync(x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Culture == "CH");
            var listPackageView = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == id && !x.IsDelete && x.OrderType == order.Type);
            var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == id && !x.IsDelete && x.OrderType == order.Type);
            var listHistory = await UnitOfWork.OrderHistoryRepo.FindAsync(x => x.OrderId == id && x.Type == order.Type, query => query.OrderByDescending(m => m.CreateDate), null);
            var listOrderExchage = await UnitOfWork.OrderExchangeRepo.Entities.Where(x => !x.IsDelete && x.OrderId == order.Id && x.Type != (byte)OrderExchangeType.Pay).ToListAsync();
            var listLog = await UnitOfWork.OrderLogRepo.FindAsync(x => x.OrderId == id);
            var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id && x.Type == (byte)OrderReasonType.Delay);
            var orderReasonNoCodeOfLading = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id && x.Type == (byte)OrderReasonType.NoCodeOfLading);
            var orderReasonNotEnoughInventory = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == id && x.Type == (byte)OrderReasonType.NotEnoughInventory);
            //var listShop = await UnitOfWork.ShopRepo.Entities.ToListAsync();

            listLog = listLog.OrderByDescending(x => x.CreateDate).ToList();

            var mess = "";
            //check shop đã được đặt hàng trong vòng 2 ngày này ko?
            var dateTwo = DateTime.Now.AddDays(-2);
            var listOrderShop = await UnitOfWork.OrderRepo.FindAsync(x => !x.IsDelete && x.ShopId == order.ShopId && x.Created > dateTwo && x.ShopId != 0 && x.Id != order.Id && x.Status >= (byte)OrderStatus.OrderSuccess);
            var count = listOrderShop.Count;

            if (count > 0 && order.ShopId != 0)
            {
                mess = "Warning: Shop new order 2 days recently";
            }

            //6. Gửi thông tin lên view
            return Json(new { status, msg, listOrderDetail, listOrderService, toAddress, formAddress, orderExchange, userOrder, order, listWarehouse, listPackageView, listContractCode, listHistory, mess, listOrderExchage, listLog, orderReason, listOrderShop, orderReasonNoCodeOfLading, orderReasonNotEnoughInventory }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Add, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> AddOrder(Order order, List<OrderDetail> listDetail, List<OrderService> listOrderService)
        {
            //1. Lấy thông tin Order
            var timeNow = DateTime.Now;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var cus = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == order.CustomerId);

                    var orderAdd = new Order()
                    {
                        Code = string.Empty,
                        WebsiteName = order.WebsiteName,
                        ShopId = order.ShopId,
                        ShopName = order.ShopName?.Trim() ?? "",
                        ShopLink = order.ShopLink,
                        PackageNo = 0,
                        ContractCode = string.Empty,
                        ContractCodes = string.Empty,
                        LevelId = cus.LevelId,
                        LevelName = cus.LevelName,
                        CreatedTool = (byte)CreatedTool.Extension,
                        Currency = Currency.CNY.ToString(),
                        ExchangeRate = ExchangeRate(),
                        WarehouseId = order.WarehouseId,
                        WarehouseName = order.WarehouseName,
                        WarehouseDeliveryId = order.WarehouseDeliveryId,
                        WarehouseDeliveryName = order.WarehouseDeliveryName,
                        CustomerId = cus.Id,
                        CustomerName = cus.FullName,
                        CustomerEmail = cus.Email,
                        CustomerPhone = cus.Phone,
                        CustomerAddress = cus.Address,
                        OrderInfoId = 0,
                        FromAddressId = 0,
                        ToAddressId = 0,
                        SystemId = cus.SystemId,
                        SystemName = cus.SystemName,
                        ServiceType = (byte)ServicePack.Business,
                        Note = order.Note,
                        UserNote = order.UserNote,
                        PrivateNote = string.Empty,
                        BargainType = order.BargainType
                    };

                    orderAdd.Status = (byte)OrderStatus.Order;
                    orderAdd.Type = (byte)OrderType.Commerce;

                    orderAdd.UserId = UserState.UserId;
                    orderAdd.UserName = UserState.UserName;
                    orderAdd.UserFullName = UserState.FullName;
                    orderAdd.OfficeId = UserState.OfficeId;
                    orderAdd.OfficeName = UserState.OfficeName;
                    orderAdd.OfficeIdPath = UserState.OfficeIdPath;

                    UnitOfWork.OrderRepo.Add(orderAdd);
                    await UnitOfWork.OrderRepo.SaveAsync();

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

                    foreach (var detail in listDetail)
                    {
                        var uniqueCode = Encryptor.Base64Encode(detail.Size + detail.Color + detail.Link);
                        var p = new OrderDetail()
                        {
                            OrderId = orderAdd.Id,
                            UniqueCode = uniqueCode,
                            Name = detail.Name,
                            Image = detail.Image,
                            Quantity = detail.Quantity,
                            QuantityBooked = detail.Quantity,
                            BeginAmount = 1,
                            Price = detail.Price,
                            ExchangeRate = order.ExchangeRate,
                            ExchangePrice = detail.Price * order.ExchangeRate,
                            TotalPrice = detail.Price * detail.Quantity,
                            TotalExchange = (detail.Price * order.ExchangeRate) * detail.Quantity,
                            AuditPrice = OrderRepository.OrderAudit(detail.Quantity, detail.Price),
                            Note = detail.Note,
                            UserNote = detail.UserNote,
                            Status = (byte)OrderDetailStatus.Order,
                            Link = detail.Link,
                            Properties = string.Empty,
                            Color = detail.Color,
                            Size = detail.Size,
                            Prices = string.Empty,
                            Min = 1,
                            Max = int.MaxValue,
                            SkullId = string.Empty,
                            ProId = string.Empty,
                            Created = timeNow,
                            LastUpdate = timeNow
                        };
                        UnitOfWork.OrderDetailRepo.Add(p);
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // Cập nhật lại Mã cho Order và Total money

                    // Cập nhật lại Mã cho Order
                    var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == cus.Id && x.Id <= order.Id);
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == cus.Id);
                    orderAdd.Code = $"{customer.Code}-{orderNo}";

                    orderAdd.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderAdd.Id && !x.IsDelete)
                        .SumAsync(x => x.TotalExchange);
                    orderAdd.UnsignName = MyCommon.Ucs2Convert(
                        $"{orderAdd.Code} {MyCommon.ReturnCode(orderAdd.Code)} {orderAdd.CustomerName} {orderAdd.CustomerEmail} {orderAdd.CustomerPhone}");

                    orderAdd.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderAdd.Id && !x.IsDelete)
                        .SumAsync(x => x.TotalPrice);

                    orderAdd.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .CountAsync(x => x.OrderId == orderAdd.Id && !x.IsDelete);

                    orderAdd.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderAdd.Id && !x.IsDelete)
                        .SumAsync(x => x.Quantity);

                    // Submit cập nhật order
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Thêm dịch vụ
                    foreach (var service in listOrderService)
                    {
                        service.OrderId = orderAdd.Id;
                        service.Created = timeNow;
                        service.LastUpdate = timeNow;

                        UnitOfWork.OrderServiceRepo.Add(service);
                    }

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------

                    var shipToHomeService = new OrderService()
                    {
                        OrderId = orderAdd.Id,
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

                    var outSideShippingService = new OrderService()
                    {
                        OrderId = orderAdd.Id,
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

                    // Submit thêm OrderService
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật số lượng Sum
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                        x => x.OrderId == orderAdd.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    orderAdd.Total = totalService + orderAdd.TotalExchange;
                    orderAdd.Debt = orderAdd.Total - (orderAdd.TotalPayed - orderAdd.TotalRefunded);

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Add Order successful!" }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> UpdateCustomer(int id, int customerId, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    var cus = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == customerId);
                    order.CustomerId = cus.Id;
                    order.LastUpdate = timeNow;

                    if (DataCompare(order.CustomerName, cus.FullName))
                    {
                        dataBefore.Add(new LogResult() { Name = "Customer", Value = order.CustomerName });
                        order.CustomerName = cus.FullName;
                        dataAfter.Add(new LogResult() { Name = "Customer", Value = order.CustomerName });
                    }
                    if (DataCompare(order.CustomerEmail, cus.Email))
                    {
                        dataBefore.Add(new LogResult() { Name = "Email", Value = order.CustomerEmail });
                        order.CustomerEmail = cus.Email;
                        dataAfter.Add(new LogResult() { Name = "Email", Value = order.CustomerEmail });
                    }
                    if (DataCompare(order.CustomerPhone, cus.Phone))
                    {
                        dataBefore.Add(new LogResult() { Name = "Phone", Value = order.CustomerPhone });
                        order.CustomerPhone = cus.Phone;
                        dataAfter.Add(new LogResult() { Name = "Phone", Value = order.CustomerPhone });
                    }
                    if (DataCompare(order.CustomerAddress, cus.Address))
                    {
                        dataBefore.Add(new LogResult() { Name = "Address", Value = order.CustomerAddress });
                        order.CustomerAddress = cus.Address;
                        dataAfter.Add(new LogResult() { Name = "Address", Value = order.CustomerAddress });
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Change customer information",
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

            return Json(new { status = MsgType.Success, msg = "Change customer successfully!" }, JsonRequestBehavior.AllowGet);
        }


        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> UpdateShop(int id, int shopId, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();

                    var shop = await UnitOfWork.ShopRepo.FirstOrDefaultAsync(x => x.Id == shopId);
                    order.ShopId = shop.Id;
                    order.LastUpdate = timeNow;

                    if (DataCompare(order.ShopName, shop.Name))
                    {
                        dataBefore.Add(new LogResult() { Name = "shop name", Value = order.ShopName });
                        order.ShopName = shop.Name?.Trim() ?? "";
                        dataAfter.Add(new LogResult() { Name = "shop name", Value = order.ShopName });
                    }
                    if (DataCompare(order.ShopLink, shop.Url))
                    {
                        dataBefore.Add(new LogResult() { Name = "Link shop", Value = order.ShopLink });
                        order.ShopLink = shop.Url;
                        dataAfter.Add(new LogResult() { Name = "Link shop", Value = order.ShopLink });
                    }
                    if (DataCompare(order.WebsiteName, shop.Website))
                    {
                        dataBefore.Add(new LogResult() { Name = "Website", Value = order.WebsiteName });
                        order.WebsiteName = shop.Website;
                        dataAfter.Add(new LogResult() { Name = "Website", Value = order.WebsiteName });
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit information of shop",
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

            return Json(new { status = MsgType.Success, msg = "Change customer successfully!" }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> UpdateNote(int id, string note, string userNote, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    List<LogResult> dataBefore = new List<LogResult>();
                    List<LogResult> dataAfter = new List<LogResult>();
                    order.LastUpdate = timeNow;

                    if (note != "" && note != "undefined" && DataCompare(order.Note, note))
                    {
                        dataBefore.Add(new LogResult() { Name = "Customer note", Value = order.Note });
                        order.Note = note;
                        dataAfter.Add(new LogResult() { Name = "Customer note", Value = order.Note });
                    }
                    if (userNote != "" && userNote != "undefined" && DataCompare(order.UserNote, userNote))
                    {
                        dataBefore.Add(new LogResult() { Name = "Staff's note", Value = order.ShopLink });
                        order.UserNote = userNote;
                        dataAfter.Add(new LogResult() { Name = "Staff's note", Value = order.ShopLink });
                    }
 
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit information of note",
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

            return Json(new { status = MsgType.Success, msg = "Successful exchange of information!" }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> EditDetailOrder(OrderDetail orderDetail, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderDetail.OrderId && !x.IsDelete);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var detail = await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => x.Id == orderDetail.Id && !x.IsDelete && x.Status == orderDetail.Status);
            if (detail == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            if (string.IsNullOrEmpty(orderDetail.Name))
            {
                return Json(new { status = MsgType.Error, msg = "Product name can not be empty!" }, JsonRequestBehavior.AllowGet);
            }


            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var uniqueCode = Encryptor.Base64Encode(orderDetail.Size + orderDetail.Color + orderDetail.Link);
                    detail.UniqueCode = uniqueCode;
                    detail.Name = orderDetail.Name;
                    detail.Link = orderDetail.Link;
                    detail.Image = orderDetail.Image;
                    detail.Color = orderDetail.Color;
                    detail.Size = orderDetail.Size;
                    detail.Note = orderDetail.Note;
                    detail.UserNote = orderDetail.UserNote;
                    detail.Quantity = orderDetail.Quantity;
                    detail.QuantityBooked = orderDetail.Quantity;
                    detail.Price = orderDetail.Price;
                    detail.TotalPrice = orderDetail.Quantity * orderDetail.Price;
                    detail.TotalExchange = detail.TotalPrice * detail.ExchangeRate;
                    detail.AuditPrice = OrderRepository.OrderAudit(detail.Quantity, orderDetail.Price);
                    detail.LastUpdate = timeNow;

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //Tính lại Total money việt nam
                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalExchange);

                    //Tính lại Total money ngoại yuan
                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalPrice);

                    //Tính lại số lượng sản phẩm
                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.QuantityBooked.Value);

                    //Tính lại số linh sản phẩm
                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .CountAsync();

                    await UnitOfWork.OrderRepo.SaveAsync();

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

            return Json(new { status = MsgType.Success, msg = "Successful exchange of information!", listOrderService }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> DeleteDetailOrder(int id, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;

            var detail = await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (detail == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order detail does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == detail.OrderId && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var countOrderDetail = await UnitOfWork.OrderDetailRepo.CountAsync(x => !x.IsDelete && x.Status == (byte)OrderDetailStatus.Order && x.OrderId == detail.OrderId);
            if (countOrderDetail == 1)
            {
                return Json(new { status = MsgType.Error, msg = "Can not perform this action because the order can only be added 1 more link!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    detail.IsDelete = true;
                    detail.LastUpdate = timeNow;

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //Tính lại Total money việt nam
                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalExchange);

                    //Tính lại Total money ngoại yuan
                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.TotalPrice);

                    //Tính lại số lượng sản phẩm
                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .SumAsync(x => x.QuantityBooked.Value);

                    //Tính lại số linh sản phẩm
                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Status != (byte)OrderDetailStatus.Cancel)
                        .CountAsync();

                    await UnitOfWork.OrderRepo.SaveAsync();

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

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Successful exchange of information!" }, JsonRequestBehavior.AllowGet);
        }

        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        [HttpPost]
        public async Task<JsonResult> AddDetailOrder(int id, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (order == null) //does not exist or is deleted
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var detail = new OrderDetail
                    {
                        OrderId = id,
                        UniqueCode = string.Empty,
                        Name = string.Empty,
                        Image = string.Empty,
                        Quantity = 0,
                        QuantityBooked = 0,
                        BeginAmount = 1,
                        Price = 0,
                        ExchangeRate = order.ExchangeRate,
                        ExchangePrice = 0,
                        TotalPrice = 0,
                        TotalExchange = 0,
                        AuditPrice = 0,
                        Note = string.Empty,
                        UserNote = string.Empty,
                        Status = (byte)OrderDetailStatus.Order,
                        Link = string.Empty,
                        Properties = string.Empty,
                        Color = string.Empty,
                        Size = string.Empty,
                        Prices = string.Empty,
                        Min = 1,
                        Max = int.MaxValue,
                        SkullId = string.Empty,
                        ProId = string.Empty,
                        Created = timeNow,
                        LastUpdate = timeNow
                    };

                    UnitOfWork.OrderDetailRepo.Add(detail);

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

            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Successful exchange of information!", listOrderDetail }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> AddContractCodeOrder(int id, string contractCode, decimal pice, byte status)
        {
            //1. khai báo biến
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

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
            return Json(new { status = MsgType.Success, msg = "Add successful Contract code!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> ReviewContractCodeOrder(int id, byte status)
        {
            //1. khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

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
                        Content = $"Send to Contract code #{orderContractCode.ContractCode} Accounting check and billing",
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
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Check back and Pay contract order #" + MyCommon.ReturnCode(order.Code), EnumNotifyType.Info, "Request recheck and pay Contract: " + order.ContractCodes);
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
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> DeleteContractCodeOrder(int id, byte status)
        {
            //1. khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

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
                    orderContractCode.IsDelete = true;
                    orderContractCode.UpdateDate = timeNow;
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    //tính toán cho Order
                    var listOrderContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type && !x.IsDelete);
                    //if (listOrderContractCode.Sum(x => x.TotalPrice) == 0)
                    //{
                    //    order.FeeShipBargain = 0;
                    //}
                    //order.PaidShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);

                    //var priceBargain = (order.TotalPrice + (order.FeeShip ?? 0)) - ((order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0));

                    //order.PriceBargain = priceBargain < 0 ? 0 : priceBargain;

                    order.ContractCodes = listOrderContractCode.Aggregate("", (current, item) => current + (current == "" ? "" : ",") + item.ContractCode);
                    order.LastUpdate = timeNow;

                    order.UnsignName = order.UnsignName.Replace("," + orderContractCode.ContractCode.ToLower(), "");
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Delete contract code #{orderContractCode.ContractCode}",
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Deleted Contract code successful!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> EditContractCodeOrder(int id, string code, decimal? totalPrice, byte status)
        {
            //1. Khai báo biến
            var timeNow = DateTime.Now;
            var orderContractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. check điều kiện
            if (orderContractCode == null) //package đã bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Contract code does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderContractCode.OrderId && !x.IsDelete);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type && x.Id != id);

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

                    if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(orderContractCode.ContractCode))
                    {
                        order.UnsignName = order.UnsignName.Replace(orderContractCode.ContractCode.ToLower(), code.ToLower());
                    }

                    if (DataCompare(orderContractCode.ContractCode, code))
                    {
                        dataBefore.Add(new LogResult() { Name = "Contract code", Value = orderContractCode.ContractCode });
                        orderContractCode.ContractCode = code;
                        dataAfter.Add(new LogResult() { Name = "Contract code", Value = orderContractCode.ContractCode });
                    }
                    if (DataCompare(orderContractCode.TotalPrice, totalPrice ?? 0))
                    {
                        dataBefore.Add(new LogResult() { Name = "Total money", Value = orderContractCode.TotalPrice == null ? "0" : orderContractCode.TotalPrice.Value.ToString("N2") });
                        orderContractCode.TotalPrice = totalPrice ?? 0;
                        dataAfter.Add(new LogResult() { Name = "Total money", Value = orderContractCode.TotalPrice.Value.ToString("N2") });
                    }

                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    var listOrderContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type && !x.IsDelete);

                    //var priceShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);
                    //if (priceShop > order.TotalPrice)
                    //{
                    //    order.FeeShipBargain = order.FeeShipBargain ?? 0;
                    //    order.FeeShipBargain += priceShop - order.TotalPrice;
                    //}

                    //if (priceShop < 0)
                    //{
                    //    order.FeeShipBargain = 0;
                    //}

                    //order.PaidShop = listOrderContractCode.Sum(x => x.TotalPrice) - (order.FeeShipBargain ?? 0);
                    //order.PriceBargain = (order.TotalPrice + (order.FeeShip ?? 0)) - ((order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0));;

                    order.ContractCodes = listOrderContractCode.Aggregate("", (current, item) => current + (current == "" ? "" : ",") + item.ContractCode);
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
                        Content = "Update information of contract code",
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

            var list = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);

            return Json(new { status = MsgType.Success, msg = "Edited successfully contract code!", listContractCode = list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> OrderSuccess(int id, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);
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

            //if (order.PaidShop <= 0 || order.PaidShop == null)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Money paid with shop Can not be empty!" }, JsonRequestBehavior.AllowGet);
            //}

            if (listContractCode.FirstOrDefault((x => x.ContractCode.Trim() == "")) != null)
            {
                return Json(new { status = MsgType.Error, msg = " Contract code is emty!" }, JsonRequestBehavior.AllowGet);
            }

            if (listContractCode.FirstOrDefault((x => x.TotalPrice == 0 || x.TotalPrice == null)) != null)
            {
                return Json(new { status = MsgType.Error, msg = "There is a code of contract in which amount of money has not been entered!" }, JsonRequestBehavior.AllowGet);
            }

            //if (order.TotalPrice < order.PaidShop)
            //{
            //    return Json(new { status = MsgType.Error, msg = "Total amount of money paid by customer must be greater than that paid by company!" }, JsonRequestBehavior.AllowGet);
            //}

            //3. thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.Status = (byte)OrderStatus.WaitAccountant;
                    order.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

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
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id, "Pay contract order #" + MyCommon.ReturnCode(order.Code), EnumNotifyType.Info, "Request pay Contract: " + order.ContractCodes);
                    }

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

            //4. gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Order successfully completed!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> UpdateOrder(int id, decimal feeShip, byte status)
        {
            //1. Khở tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

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
                    order.LastUpdate = timeNow;

                    dataBefore.Add(new LogResult() { Name = "China domestic shipping fee", Value = order.FeeShip == null ? "0" : order.FeeShip.Value.ToString("N2") });
                    order.FeeShip = feeShip;
                    order.FeeShipBargain = feeShip;
                    dataAfter.Add(new LogResult() { Name = "China domestic shipping fee", Value = order.FeeShip.Value.ToString("N2") });

                    var orderServiceShopShipping = await UnitOfWork.OrderServiceRepo.FirstOrDefaultAsync(
                        x =>
                            x.OrderId == order.Id && !x.IsDelete &&
                            x.ServiceId == (byte)OrderServices.ShopShipping
                    );

                    orderServiceShopShipping.Value = orderServiceShopShipping.Value;
                    orderServiceShopShipping.TotalPrice = orderServiceShopShipping.Value * orderServiceShopShipping.ExchangeRate;
                    orderServiceShopShipping.LastUpdate = timeNow;
                    orderServiceShopShipping.Checked = true;
                    orderServiceShopShipping.Note = $"Shipping Shop Fee is {feeShip.ToString("N4", CultureInfo)} CNY equalling to {orderServiceShopShipping.TotalPrice.ToString("N2", CultureInfo)} Baht";

                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật số lượng Sum
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                                x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Edit information of orders",
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
            return Json(new
            {
                status = MsgType.Success,
                msg = "Updated successfully!",
                listOrderService
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderCommerce)]
        public async Task<JsonResult> UpdateCustomerPay(int id, decimal price, byte status)
        {
            //1. Khở tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

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
                    var isNew = false;
                    var orderExchang =
                        UnitOfWork.OrderExchangeRepo.FirstOrDefault(
                            x => !x.IsDelete && x.OrderId == id && x.Type == (byte)OrderExchangeType.Pay);
                    if (orderExchang == null)
                    {
                        isNew = true;
                        orderExchang = new OrderExchange
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            Currency = Currency.VND.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            IsDelete = false,
                            Type = (byte)OrderExchangeType.Pay,
                            Mode = (byte)OrderExchangeMode.Export,
                            ModeName = OrderExchangeType.Pay.GetAttributeOfType<DescriptionAttribute>().Description,
                            Note = "Customers pay money Order",
                            OrderId = order.Id,
                            TotalPrice = 0,
                            Status = (byte)OrderExchangeStatus.Approved
                        };
                    }

                    dataBefore.Add(new LogResult() { Name = "Customers pay", Value = orderExchang.TotalPrice == null ? "0" : orderExchang.TotalPrice.Value.ToString("N2") });
                    orderExchang.TotalPrice = price;
                    dataAfter.Add(new LogResult() { Name = "Customers pay", Value = orderExchang.TotalPrice.Value.ToString("N2") });

                    if (isNew)
                    {
                        UnitOfWork.OrderExchangeRepo.Add(orderExchang);
                    }

                    await UnitOfWork.OrderExchangeRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.DataJson,
                        DataBefore = JsonConvert.SerializeObject(dataBefore),
                        DataAfter = JsonConvert.SerializeObject(dataAfter),
                        Content = "Change the customer account",
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
            return Json(new
            {
                status = MsgType.Success,
                msg = "Updated successfully!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}