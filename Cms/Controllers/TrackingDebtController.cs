using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    [Authorize]
    public class TrackingDebtController : BaseController
    {
        // GET: TrackingDebt
        [LogTracker(EnumAction.View, EnumPage.TrackingDebt)]
        public async Task<ActionResult> Index()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Object Javascript: Loại đơn hàng
            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<OrderType>((int) v)),
                jsonSerializerSettings);

            // Array Javascript: Loại đơn hàng
            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .Select(x => new {Id = (byte) x, Name = EnumHelper.GetEnumDescription<OrderType>((int) x), Checked = false})
                    .ToList(),
                jsonSerializerSettings);

            // Object Javascript: Trạng thái đơn hàng
            ViewBag.OrderStatus = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<OrderStatus>((int) v)),
                jsonSerializerSettings);

            // Array Javascript: Trạng thái đơn hàng
            ViewBag.OrderStatuss = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderStatus))
                    .Cast<OrderStatus>()
                    .Select(x => new { Id = (byte)x, Name = EnumHelper.GetEnumDescription<OrderStatus>((int)x), Checked = false })
                    .ToList(),
                jsonSerializerSettings);
            
            
            // Object Javascript: Trạng thái đơn hàng ký gửi
            ViewBag.DepositStatus = JsonConvert.SerializeObject(Enum.GetValues(typeof(DepositStatus))
                    .Cast<DepositStatus>()
                    .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<DepositStatus>((int) v)),
                jsonSerializerSettings);

            // Array Javascript: Trạng thái đơn hàng ký gửi
            ViewBag.DepositStatuss = JsonConvert.SerializeObject(Enum.GetValues(typeof(DepositStatus))
                    .Cast<DepositStatus>()
                    .Select(x => new { Id = (byte)x, Name = EnumHelper.GetEnumDescription<DepositStatus>((int)x), Checked = false })
                    .ToList(),
                jsonSerializerSettings);

            var warehouses = await UnitOfWork.OfficeRepo.FindAsync(
                x => x.IsDelete == false && x.Status == (byte) OfficeStatus.Use &&
                    x.Type == (byte) OfficeType.Warehouse);

            // Array Javascript: Các kho hàng
            ViewBag.Warehouses = JsonConvert.SerializeObject(
                warehouses.Select(x => new {x.Id, x.Name, Checked = false}),
                jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.TrackingDebt)]
        public async Task<ActionResult> Search(string keyword, byte? searchType, string moneyText, string statusText, 
            string statusDepositText, string warehouseIdText, string orderTypeText, int currentPage = 1,
            int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword).Trim();

            long totalRecord;

            // moneyText: dạng: ";0;1;2;3;"
            // moneyText: chứa 0: Khách đã thanh toán tiền
            // moneyText chứa 1: Có Refund cho khách
            // moneyText chứa 2: Đang nợ tiền khách
            // moneyText chứ 3: Khách đang nợ công ty

            var orders = await UnitOfWork.OrderRepo.FindAsync(out totalRecord,
                x =>
                    x.IsDelete == false && (searchType == null && x.UnsignName.Contains(keyword)
                    || searchType == 0 && x.Code == keyword
                    || searchType == 1 && x.CustomerPhone == keyword
                    || searchType == 2 && x.CustomerEmail == keyword) &&
                    (x.Type != (byte)OrderType.Deposit && (statusText == "" || statusText.Contains(";" + x.Status + ";"))  
                    || x.Type == (byte)OrderType.Deposit && (statusDepositText == "" || statusDepositText.Contains(";" + x.Status + ";"))) &&
                    (warehouseIdText == "" || warehouseIdText.Contains(";" + x.WarehouseDeliveryId + ";")) &&
                    (orderTypeText == "" || orderTypeText.Contains(";" + x.Type + ";")) &&
                    (moneyText == "" || !moneyText.Contains("0") || moneyText.Contains("0") &&  x.TotalPayed > 0) &&
                    (moneyText == "" || !moneyText.Contains("1") || moneyText.Contains("1") && x.TotalRefunded > 0) &&
                    (moneyText == "" || !moneyText.Contains("2") || moneyText.Contains("2") && x.Debt < 0) &&
                    (moneyText == "" || !moneyText.Contains("3") || moneyText.Contains("3") && x.Debt > 0),
                o => o.OrderBy(x => x.Id), currentPage, recordPerPage);

            var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

            var orderServices =
                await UnitOfWork.OrderServiceRepo.FindAsync(
                    x => x.IsDelete == false && x.Checked && orderIds.Contains(";" + x.OrderId + ";"));

            var services = orderServices.GroupBy(x => x.OrderId).ToDictionary(x => x.Key, x => x.ToList());

            return JsonCamelCaseResult(new { orders, services, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetOrderExchange(int id, byte? mode)
        {
            var items = await UnitOfWork.OrderExchangeRepo.FindAsync(
                x => x.IsDelete == false && x.Status == (byte) OrderExchangeStatus.Approved &&
                     (mode == null || x.Mode == mode) && x.OrderId == id);

            return JsonCamelCaseResult(new { items, mode }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetOrderServiceOther(int id)
        {
            var orderServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(
                x => x.OrderId == id);

            return JsonCamelCaseResult(orderServiceOther, JsonRequestBehavior.AllowGet);
        }


        [CheckPermission(EnumAction.Update, EnumPage.TrackingDebt)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrderPayment(PaymentRefundMeta model)
        {
            if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte) OfficeType.Accountancy)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "Only the accountant has the right to do this"},
                    JsonRequestBehavior.AllowGet);

            if (model.Amount <= 0)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "The refund must be greater than  0"},
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var order = await UnitOfWork.OrderRepo
                        .SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == model.OrderId &&
                                                   (x.Type != (byte) OrderType.Deposit &&
                                                    x.Status >= (byte) OrderStatus.WaitOrder &&
                                                    x.Status <= (byte) OrderStatus.GoingDelivery
                                                    || x.Type == (byte) OrderType.Deposit &&
                                                    x.Status == (byte) DepositStatus.GoingDelivery));

                    if (order == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Order does not exist or is not refundable" },
                            JsonRequestBehavior.AllowGet);

                    if (order.Debt <= 0)
                        return JsonCamelCaseResult(new {Status = -3, Text = "This application has no remaining receivables"},
                            JsonRequestBehavior.AllowGet);

                    if (model.Amount > order.Debt && model.Amount - order.Debt >= 100)
                        return JsonCamelCaseResult(
                                new {Status = -3, Text = "Can not exceed the total amount owed on the order"},
                                JsonRequestBehavior.AllowGet);

                    var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                        x => x.IsDelete == false && x.Id == order.CustomerId);

                    if (customer == null)
                        return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    // Trừ tiền tài khoản khách
                    var processRechargeBillResult =
                        UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                        {
                            CustomerId = customer.Id,
                            CurrencyFluctuations = model.Amount,
                            OrderId = model.OrderId,
                            Note = $"Collection of orders: " + model.Note,
                            TreasureIdd = (int) TreasureCustomerWallet.Delivery
                        });

                    // Lỗi trong quá tình thực hiện thanh toán
                    if (processRechargeBillResult.Status < 0)
                    {
                        return Json(new { processRechargeBillResult.Status, Text = processRechargeBillResult.Msg });
                    }

                    order.TotalPayed += model.Amount;
                    order.Debt -= model.Amount;

                    // Thêm giao dịch trừ tiền
                    UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                    {
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Currency = Currency.VND.ToString(),
                        ExchangeRate = 0,
                        IsDelete = false,
                        Type = (byte) OrderExchangeType.Pay,
                        Mode = (byte) OrderExchangeMode.Export,
                        ModeName = OrderExchangeType.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                        Note = $"Collection of orders: " + model.Note,
                        OrderId = model.OrderId,
                        TotalPrice = model.Amount,
                        Status = (byte) OrderExchangeStatus.Approved
                    });

                    await UnitOfWork.OrderExchangeRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Commit();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }

                return JsonCamelCaseResult(new {Status = 1, Text = "Thu tiền thành công"}, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission(EnumAction.Update, EnumPage.TrackingDebt)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OrderRefund(PaymentRefundMeta model)
        {
            if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte) OfficeType.Accountancy)
                return JsonCamelCaseResult(
                        new {Status = -1, Text = "Only the accountant has the right to do this"},
                        JsonRequestBehavior.AllowGet);

            if(model.Amount <= 0)
                return JsonCamelCaseResult(
                        new { Status = -1, Text = "The refund must be greater than  0" },
                        JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var order = await UnitOfWork.OrderRepo
                        .SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == model.OrderId &&
                                                   (x.Type != (byte) OrderType.Deposit &&
                                                    x.Status >= (byte) OrderStatus.WaitOrder &&
                                                    x.Status <= (byte) OrderStatus.GoingDelivery
                                                    || x.Type == (byte) OrderType.Deposit &&
                                                    x.Status >= (byte) DepositStatus.GoingDelivery && x.Status <= (byte)DepositStatus.Finish));

                    if (order == null)
                        return JsonCamelCaseResult(new {Status = -1, Text = "Order does not exist or is not refundable"},
                            JsonRequestBehavior.AllowGet);

                    if (order.Debt >= 0 && order.Type != (byte) OrderType.Deposit && order.Status == (byte) OrderStatus.GoingDelivery)
                        return JsonCamelCaseResult(new {Status = -3, Text = "This order has no item to refund"},
                            JsonRequestBehavior.AllowGet);

                    if (order.Debt >= 0 && order.Type == (byte) OrderType.Deposit)
                        return JsonCamelCaseResult(new {Status = -3, Text = "This order has no item to refund"},
                            JsonRequestBehavior.AllowGet);

                    var totalRefund = order.TotalRefunded + model.Amount;

                    if (totalRefund > order.TotalPayed)
                        return JsonCamelCaseResult(
                            new {Status = -1, Text = "Total refund is not greater than the total amount paid"},
                            JsonRequestBehavior.AllowGet);

                    var vipLevel = await UnitOfWork.CustomerLevelRepo.SingleOrDefaultAsync(x => x.Id == order.LevelId);

                    if(vipLevel == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = "Vip level does not exist" },
                            JsonRequestBehavior.AllowGet);

                    var requiredPayed = vipLevel.PercentDeposit * order.TotalExchange / 100;
                    var totalPayed = order.TotalPayed - model.Amount;

                    if (totalPayed < requiredPayed)
                        return JsonCamelCaseResult(
                            new {Status = -1, Text = "The amount paid can not be less than the deposit amount"},
                            JsonRequestBehavior.AllowGet);

                    var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                        x => x.IsDelete == false && x.Id == order.CustomerId);

                    if (customer == null)
                        return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                    {
                        CustomerId = customer.Id,
                        CurrencyFluctuations = model.Amount,
                        Note = $"Hoàn tiền đơn hàng: " + model.Note,
                        TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
                    });

                    // Lỗi trong quá tình thực hiện thanh toán
                    if (processRechargeBillResult.Status < 0)
                    {
                        transaction.Rollback();
                        
                        return JsonCamelCaseResult(new { processRechargeBillResult.Status, Text = processRechargeBillResult.Msg  },
                                JsonRequestBehavior.AllowGet);
                    }

                    // Thêm giao dịch Refund
                    UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                    {
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Currency = Currency.VND.ToString(),
                        ExchangeRate = 0,
                        IsDelete = false,
                        Type = (byte)OrderExchangeType.Pay,
                        Mode = (byte)OrderExchangeMode.Import,
                        ModeName = OrderExchangeType.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                        Note = $"Hoàn tiền đơn hàng: " + model.Note,
                        OrderId = order.Id,
                        TotalPrice = model.Amount,
                        Status = (byte)OrderExchangeStatus.Approved
                    });

                    order.TotalRefunded += model.Amount;
                    order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);
                    order.LastUpdate = DateTime.Now;

                    await UnitOfWork.OrderExchangeRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Commit();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Reimbursement of success"}, JsonRequestBehavior.AllowGet);
        }
    }
}