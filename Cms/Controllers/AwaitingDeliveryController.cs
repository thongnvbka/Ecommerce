using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Cms.Helpers;
using Cms.Jobs;
using Common.Emums;
using Common.Helper;
using Hangfire;
using Library.DbContext.Entities;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    [Authorize]
    public class AwaitingDeliveryController : BaseController
    {
        // GET: AwaitingDelivery
        [LogTracker(EnumAction.View, EnumPage.AwaitingDelivery)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type != null && (UserState.Type.Value == 2 || UserState.Type.Value == 1);

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager)
            {
                var systems = await UnitOfWork.SystemRepo.FindAsNoTrackingAsync(x => x.Status == 1);
                ViewBag.Systems = JsonConvert.SerializeObject(systems.Select(x => new { x.Id, x.Domain, x.Name }),
                    jsonSerializerSettings);
            }

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderType>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte)v, v => EnumHelper.GetEnumDescription<OrderType>((int)v)),
                jsonSerializerSettings);

            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.AwaitingDelivery)]
        public async Task<ActionResult> Search(byte? systemId, byte? orderType, string keyword = "", 
            int currentPage = 1, int recordPerPage = 20)
        {
            int totalRecord;
            var customers = await UnitOfWork.DeliveryRepo.Search(UserState, systemId, orderType, keyword, currentPage, recordPerPage,
                out totalRecord);

            var customerIds = $";{string.Join(";", customers.Select(x => x.Id).ToList())};";

            var callHistories = await UnitOfWork.CustomerCallHistoryRepo.GetByCustomerIds(customerIds);

            var packages = await UnitOfWork.DeliveryRepo.Search(UserState, systemId, orderType, customerIds, keyword);

            return JsonCamelCaseResult(new {totalRecord, customers, packages, callHistories }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.AwaitingDelivery)]
        public async Task<ActionResult> AddCustomerCallHistory(string note, int customerId)
        {
            var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                    x => x.Id == customerId && x.IsDelete == false && x.IsActive);

            if(customer == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Customer does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

            CustomerCallHistory callHistory;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                // ReSharper disable once TooWideLocalVariableScope
                CustomerCallHistory lastCall;
                try
                {
                    lastCall = await UnitOfWork.CustomerCallHistoryRepo.FirstOrDefaultAsync(
                        x =>
                            x.IsLast && x.CustomerId == customerId &&
                            x.Mode == (byte) CustomerCallHistoryMode.CallDelivery);

                    callHistory = new CustomerCallHistory()
                    {
                        Content = note,
                        UserId = UserState.UserId,
                        Mode = (byte)CustomerCallHistoryMode.CallDelivery,
                        Created = DateTime.Now,
                        CustomerEmail = customer.Email,
                        CustomerId = customer.Id,
                        CustomerName = customer.UserFullName,
                        CustomerPhone = customer.Phone,
                        CustomerVipId = customer.LevelId,
                        CustomerVipName = customer.LevelName,
                        IsLast = true,
                        ObjectId = null,
                        OfficeId = UserState.OfficeId ?? 0,
                        OfficeIdPath = UserState.OfficeIdPath,
                        OfficeName = UserState.OfficeName,
                        OfficeNamePath = UserState.OfficeName,
                        TitleId = UserState.TitleId ?? 0,
                        TitleName = UserState.TitleName,
                        UserFullName = UserState.FullName,
                        UserName = UserState.UserName
                    };

                    UnitOfWork.CustomerCallHistoryRepo.Add(callHistory);

                    await UnitOfWork.CustomerCallHistoryRepo.SaveAsync();

                    if (lastCall != null)
                    {
                        lastCall.IsLast = false;

                        await UnitOfWork.CustomerCallHistoryRepo.SaveAsync();
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

            return JsonCamelCaseResult(new { Status = 1, Text = "Added history call successfully", Data = callHistory },
                JsonRequestBehavior.AllowGet);
        }

        #region Kiểm tra công nợ của phiếu xuất kho
        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.AwaitingDelivery)]
        public async Task<ActionResult> CheckDebit(string packageIds)
        {
            var packages = await UnitOfWork.OrderPackageRepo.GetByPackageIds(packageIds);

            var orderIds = $";{string.Join(";", packages.Select(x => x.OrderId).Distinct().ToList())};";

            // Các dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
            var listServiceOrder = UnitOfWork.OrderServiceRepo.Find(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                                         x.IsDelete == false && x.Checked &&
                                                                         (/*x.ServiceId == (byte) OrderServices.Order ||*/ x.ServiceId == (byte)OrderServices.Audit || x.ServiceId == (byte)OrderServices.ShopShipping))
                .ToList();

            // Total money dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
            var totalServiceOrder = listServiceOrder.Sum(x => x.TotalPrice);

            //// Dic tiền dịch vụ hoàn thành đơn theo từng Orders
            //var serviceOrder = listServiceOrder.GroupBy(x => x.OrderId)
            //    .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

            // các Orders trong phiếu xuất
            var listOrder = UnitOfWork.OrderRepo.Find(x => orderIds.Contains(";" + x.Id + ";") &&
                                                           x.IsDelete == false)
                .ToList();

            // Total money hàng của các Orders trong phiếu xất
            var totalOrder = listOrder.Sum(x => x.TotalExchange);

            //// Total money hàng
            //var order = listOrder.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(y => y.TotalExchange));

            // Total money Packing
            var totalPricePacking = packages.ToList().Sum(x => x.TotalPriceWapperExchange ?? 0);

            // Total money dịch vụ phát sinh của package
            var totalPriceOther = packages.ToList().Sum(x => x.OtherService ?? 0);

            // Tính tiền vận chuyển TQ, VN
            var priceWeigth = UnitOfWork.OrderServiceRepo.Entities.Where(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                             x.IsDelete == false && x.Checked &&
                                                             x.ServiceId == (byte)OrderServices.OutSideShipping)
                 .GroupBy(x => x.OrderId)
                 .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

            decimal totalPriceWeigth = 0;

            foreach (var pw in priceWeigth)
            {
                var totalPercent = packages.Where(x => x.OrderId == pw.Key).Sum(x => x.WeightActualPercent ?? 0);
                totalPriceWeigth += pw.Value * totalPercent / 100;
            }

            // Total money khác hàng đã thanh toán
            var totalDepositMoney = listOrder.Sum(x => x.TotalPayed - x.TotalRefunded);

            // Tính tiền phí lưu kho của các package
            var listPackageStored = await UnitOfWork.OrderPackageRepo.GetPriceStoredInWarehouse(packageIds, UserState);

            var priceStored = new Dictionary<int, decimal>();

            foreach (var p in listPackageStored)
            {
                var dayStored = DateTime.Now.Subtract(p.PutAwayTime.AddDays(7)).TotalDays;

                if (dayStored <= 0 || p.WeightActual == null)
                {
                    priceStored.Add(p.Id, 0);
                }
                else
                {
                    priceStored.Add(p.Id, (decimal)dayStored * 1000 * p.WeightActual.Value);
                }
            }

            var totalPriceStored = priceStored.Sum(x => x.Value);

            var debit = (totalServiceOrder + totalOrder + totalPricePacking + totalPriceOther + totalPriceWeigth +
                         totalPriceStored) - totalDepositMoney;

            return JsonCamelCaseResult(debit, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  Lấy dữ liệu công nợ của package
        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.AwaitingDelivery)]
        public async Task<ActionResult> GetDataToAddBill(string packageIds, int customerId)
        {
            var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.IsActive && x.IsDelete == false && x.Id == customerId);

            var packages = await UnitOfWork.OrderPackageRepo.GetByPackageIds(packageIds);

            var orderIds = $";{string.Join(";", packages.Select(x => x.OrderId).Distinct().ToList())};";

            // Các dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
            var listServiceOrder = UnitOfWork.OrderServiceRepo.Find(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                                         x.IsDelete == false && x.Checked &&
                                                                         (/*x.ServiceId == (byte)OrderServices.Order||*/ x.ServiceId == (byte)OrderServices.Audit
                                                                          || x.ServiceId == (byte)OrderServices.ShopShipping
                                                                          || x.ServiceId == (byte)OrderServices.RetailCharge))
                .ToList();

            // Total money dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
            var totalServiceOrder = listServiceOrder.Sum(x => x.TotalPrice);

            // Dic tiền dịch vụ hoàn thành đơn theo từng Orders
            var serviceOrder = listServiceOrder.GroupBy(x => x.OrderId)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

            // các Orders trong phiếu xuất
            var listOrder = UnitOfWork.OrderRepo.Find(x => orderIds.Contains(";" + x.Id + ";") &&
                                                           x.IsDelete == false)
                .ToList();

            // Total money hàng của các Orders trong phiếu xất
            var totalOrder = listOrder.Sum(x => x.TotalExchange);

            // Total money hàng
            var order = listOrder.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(y => y.TotalExchange));

            // Total money Packing
            var totalPricePacking = packages.ToList().Sum(x => x.TotalPriceWapperExchange ?? 0);

            // Total money dịch vụ phát sinh của package
            var totalPriceOther = packages.ToList().Sum(x => x.OtherService ?? 0);

            // Tính tiền vận chuyển TQ, VN
            var orderPriceWeigth = UnitOfWork.OrderServiceRepo.Entities.Where(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                             x.IsDelete == false && x.Checked &&
                                                             x.ServiceId == (byte)OrderServices.OutSideShipping)
                 .GroupBy(x => x.OrderId)
                 .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

            // Total money khác hàng đã thanh toán
            var totalDepositMoney = listOrder.Sum(x => x.TotalPayed - x.TotalRefunded);

            var depositMoney = listOrder.GroupBy(x => x.Id)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPayed - y.TotalRefunded));

            // Tính tiền phí lưu kho của các package
            var listPackageStored = await UnitOfWork.OrderPackageRepo.GetPriceStoredInWarehouse(packageIds, UserState);

            var priceStored = new Dictionary<int, decimal>();

            foreach (var p in listPackageStored)
            {
                var dayStored = DateTime.Now.Subtract(p.PutAwayTime.AddDays(7)).TotalDays;

                if (dayStored <= 0 || p.WeightActual == null)
                {
                    priceStored.Add(p.Id, 0);
                }
                else
                {
                    priceStored.Add(p.Id, (decimal)dayStored * 1000 * p.WeightActual.Value);
                }
            }

            var totalPriceStored = priceStored.Sum(x => x.Value);

            // các Orders đã có phiếu xuất trước
            var listOrderId = await UnitOfWork.DeliveryRepo.GetOrderHaveDelivery(orderIds);

            // Tiền Packing trong phiếu này
            var thisPricePacking = totalPricePacking;
            // Tiền dịch vụ phát sinh trong phiếu này
            var thisPriceOther = totalPriceOther;
            // tiền lưu kho trong phiếu này
            var thisPriceStored = totalPriceStored;

            // Cước cân
            decimal thisPriceWeigth = 0;

            var priceWeigth = new Dictionary<int, decimal>();
            foreach (var p in packages)
            {
                if (p.CustomerWarehouseId == UserState.OfficeId)
                {
                    priceWeigth.Add(p.Id, (p.WeightActualPercent ?? 0) * orderPriceWeigth[p.OrderId] / 100);
                    thisPriceWeigth += priceWeigth[p.Id];
                }
            }

            // tiền hàng trong phiếu này
            decimal thisOrder = 0;

            foreach (var o in order.ToArray())
            {
                if (listOrderId.Any(x => x == o.Key))
                {
                    order[o.Key] = 0;
                    continue;
                }                  

                thisOrder += o.Value;
            }

            // tiền dịch vụ hoàn thành đơn trong phiếu này
            decimal thisServiceOrder = 0;

            foreach (var s in serviceOrder.ToArray())
            {
                if (listOrderId.Any(x => x == s.Key))
                {
                    serviceOrder[s.Key] = 0;
                    continue;
                }

                thisServiceOrder += s.Value;
            }

            // Tiền đã thanh toán của đơn này
            decimal thisDepositMoney = 0;

            foreach (var d in depositMoney.ToArray())
            {
                if (listOrderId.Any(x => x == d.Key))
                {
                    depositMoney[d.Key] = 0;
                    continue;
                }

                thisDepositMoney += d.Value;
            }

            // Total money của phiếu giao
            var thisTotal = thisOrder + thisServiceOrder + thisPriceWeigth + thisPricePacking 
                + thisPriceOther + thisPriceStored;

            // Tiền nợ của phiếu này
            var debit = thisTotal - thisDepositMoney;

            // Tiền nợ kỳ trước
            var lastDelivery = UnitOfWork.DeliveryRepo.SingleOrDefaultAsNoTracking(
                x => x.IsDelete == false && x.CustomerId == customerId && x.IsLast);

            var debitPre = lastDelivery == null || lastDelivery.DebitAfter == null ? 0 : lastDelivery.DebitAfter;

            //var debitPre = UnitOfWork.DeliveryRepo.FindAsNoTracking(
            //        x => x.IsDelete == false && x.CustomerId == customerId)
            //    .Sum(x => x.DebitAfter ?? 0);

            var receivable = debit + debitPre;

            var vipShip = UnitOfWork.CustomerLevelRepo.Find(x => x.IsDelete == false && x.Status)
                        .ToDictionary(x => x.Id, x => x.Ship);

            return JsonCamelCaseResult(new
            {
            // Triết khấu cân nặng theo levelVip của khách hàng
            vipShip,
            thisPriceWeigth,
            // Tiền Packing trong phiếu này
            thisPricePacking,
            // Tiền dịch vụ phát sinh trong phiếu này
            thisPriceOther,
            // tiền lưu kho trong phiếu này
            thisPriceStored,
            // Total money trong phiếu
            thisTotal,
            // Tiền nợ phiếu
            thisOrder,
            // Tiền hoàn thành đơn
            thisServiceOrder,
            // Tiền đã thanh toán
            totalDepositMoney,
            // Tiền nợ của phiếu àny
            debit,
            // Tiền nợ của các phiếu trước
            debitPre,
            // Tiền phải thu
            receivable,
            // Dic tiền theo Orders
            priceWeigth,
            totalOrder,
            totalServiceOrder,
            order,
            serviceOrder,
            depositMoney,
            thisDepositMoney,
            priceStored,
            packages,
            customer
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddBill(DeliveryBillMeta model)
        {
            if (!ModelState.IsValid)
            {
                return JsonCamelCaseResult(new { Status = -1, Text = "Input data are not correct " },
                    JsonRequestBehavior.AllowGet);
            }

            var timeNow = DateTime.Now;

            Delivery delivery;
            string orderIds;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == model.CustomerId && x.IsDelete == false && x.IsActive);

                    if (customer == null)
                    {
                        return JsonCamelCaseResult(new { Status = -1, Text = "Customer does not exist or has been deleted, inactivated" },
                            JsonRequestBehavior.AllowGet);
                    }

                    // Kiểm tra các package đã được tạo trong phiếu xuất
                    var packagesInDelivery = await UnitOfWork.DeliveryRepo.GetPackageInDelivery(model.PackageIds);

                    if (packagesInDelivery.Any())
                    {
                        return JsonCamelCaseResult(
                                new
                                {
                                    Status = -1,
                                    Text =
                                    $"Packages: {string.Join(", ", packagesInDelivery.Select(x => "P" + x.PackageCode).ToList())} has been created dispatch note"
                                },
                                JsonRequestBehavior.AllowGet);
                    }

                    var packages = await UnitOfWork.OrderPackageRepo.GetByPackageIds(model.PackageIds);

                    var listOrderIds = packages.Select(x => x.OrderId).Distinct().ToList();

                    orderIds = $";{string.Join(";", listOrderIds)};";

                    // các Orders trong phiếu xuất
                    var listOrder = UnitOfWork.OrderRepo.Find(x => orderIds.Contains(";" + x.Id + ";") &&
                                                                   x.IsDelete == false)
                        .ToList();

                    // Kiểm tra và Refund cho Orders
                    foreach (var o in listOrder)
                    {
                        if (o.Debt < -100)
                        {
                            var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                            {
                                CustomerId = customer.Id,
                                CurrencyFluctuations = Math.Abs(o.Debt),
                                Note = "Return excess money",
                                TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
                            });

                            // Lỗi trong quá tình thực hiện thanh toán
                            if (processRechargeBillResult.Status < 0)
                            {
                                transaction.Rollback();

                                return JsonCamelCaseResult(
                                        new {Status = 1, Text = processRechargeBillResult.Msg, DeliveryCode = processRechargeBillResult.Status},
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
                                Note = "Return excess money",
                                OrderId = o.Id,
                                TotalPrice = Math.Abs(o.Debt),
                                Status = (byte)OrderExchangeStatus.Approved
                            });

                           o.TotalRefunded += Math.Abs(o.Debt);
                           o.Debt = 0;
                        }
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Các dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
                    var listServiceOrder = UnitOfWork.OrderServiceRepo.Find(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                                                 x.IsDelete == false && x.Checked &&
                                                                                 (/*x.ServiceId == (byte)OrderServices.Order||*/ x.ServiceId == (byte)OrderServices.Audit
                                                                                  || x.ServiceId == (byte)OrderServices.ShopShipping
                                                                                  || x.ServiceId == (byte)OrderServices.RetailCharge))
                        .ToList();

                    // Total money dịch vụ cần để hoàn thành đơn của các Orders trong phiếu xuất
                    //var totalServiceOrder = listServiceOrder.Sum(x => x.TotalPrice);

                    // Dic tiền dịch vụ hoàn thành đơn theo từng Orders
                    var serviceOrder = listServiceOrder.GroupBy(x => x.OrderId)
                        .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

                    // Total money hàng của các Orders trong phiếu xất
                    //var totalOrder = listOrder.Sum(x => x.TotalExchange);

                    // Total money hàng
                    var order = listOrder.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(y => y.TotalExchange));

                    // Total money Packing
                    var totalPricePacking = packages.ToList().Sum(x => x.TotalPriceWapperExchange ?? 0);

                    // Total money dịch vụ phát sinh của package
                    var totalPriceOther = packages.ToList().Sum(x => x.OtherService ?? 0);

                    // Tính tiền vận chuyển TQ, VN
                    var priceWeigth = UnitOfWork.OrderServiceRepo
                        .Entities.Where(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                             x.IsDelete == false && x.Checked &&
                                             x.ServiceId == (byte) OrderServices.OutSideShipping)
                        .GroupBy(x => x.OrderId)
                        .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

                    decimal totalPriceWeigth = 0;

                    foreach (var pw in priceWeigth)
                    {
                        var totalPercent = packages.Where(
                                x => x.OrderId == pw.Key && x.CustomerWarehouseId == (UserState.OfficeId ?? 0))
                            .Sum(x => x.WeightActualPercent ?? 0);

                        totalPriceWeigth += pw.Value * totalPercent / 100;
                    }

                    // tiền khách hàng đã thanh toán
                    var depositMoney = listOrder.GroupBy(x => x.Id)
                        .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPayed - y.TotalRefunded));

                    // Tính tiền phí lưu kho của các package
                    var listPackageStored = await UnitOfWork.OrderPackageRepo.GetPriceStoredInWarehouse(model.PackageIds, UserState);

                    var priceStored = new Dictionary<int, decimal>();

                    foreach (var p in listPackageStored)
                    {
                        var dayStored = DateTime.Now.Subtract(p.PutAwayTime.AddDays(7)).TotalDays;

                        if (dayStored <= 0 || p.WeightActual == null)
                        {
                            priceStored.Add(p.Id, 0);
                        }
                        else
                        {
                            priceStored.Add(p.Id, (decimal)dayStored * 1000 * p.WeightActual.Value);
                        }
                    }

                    var totalPriceStored = priceStored.Sum(x => x.Value);

                    // các Orders đã có phiếu xuất trước
                    var listOrderId = await UnitOfWork.DeliveryRepo.GetOrderHaveDelivery(orderIds);

                    // Tiền cân nặng trong phiếu này
                    var thisPriceWeigth = totalPriceWeigth;
                    // Tiền Packing trong phiếu này
                    var thisPricePacking = totalPricePacking;
                    // Tiền dịch vụ phát sinh trong phiếu này
                    var thisPriceOther = totalPriceOther;
                    // tiền lưu kho trong phiếu này
                    var thisPriceStored = totalPriceStored;

                    // tiền hàng trong phiếu này
                    order = order.Where(o => listOrderId.All(x => x != o.Key))
                        .ToDictionary(x=> x.Key, x=> x.Value);
                    var thisOrder = order.Sum(x=> x.Value);

                    // tiền dịch vụ hoàn thành đơn trong phiếu này
                    serviceOrder = serviceOrder.Where(s => listOrderId.All(x => x != s.Key))
                        .ToDictionary(x=> x.Key, x=> x.Value);
                    var thisServiceOrder = serviceOrder.Sum(x=> x.Value);

                    // Tiền đã thanh toán của đơn này
                    depositMoney = depositMoney.Where(d => listOrderId.All(x => x != d.Key))
                        .ToDictionary(x => x.Key, x => x.Value);

                    var thisDepositMoney = depositMoney.Sum(x=> x.Value);

                    // Total money của phiếu giao
                    var thisTotal = thisOrder + thisServiceOrder + thisPriceWeigth + thisPricePacking
                        + thisPriceOther + thisPriceStored + model.PriceShip;

                    // Tiền nợ của phiếu này
                    var debit = thisTotal - thisDepositMoney;

                    // Tiền nợ kỳ trước
                    var lastDelivery = UnitOfWork.DeliveryRepo.Entities
                        .Where(x => x.IsDelete == false && x.CustomerId == model.CustomerId)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    var debitPre = lastDelivery == null || lastDelivery.DebitAfter == null ? 0 : lastDelivery.DebitAfter;

                    var prices = UnitOfWork.OrderServiceRepo.Find(
                            x => x.IsDelete == false && x.ServiceId == (byte)OrderServices.OutSideShipping &&
                                 orderIds.Contains(";" + x.OrderId + ";"))
                        .ToDictionary(x => x.OrderId, service => service.Value);

                    var vipShip = UnitOfWork.CustomerLevelRepo.Find(x => x.IsDelete == false && x.Status)
                        .ToDictionary(x=> x.Id, x=> x.Ship);

                    // Cập nhật lại trạng thái phiếu cuối cùng phiếu của khách
                    var oldDeliverys =
                        await UnitOfWork.DeliveryRepo.FindAsync(
                            x => x.IsDelete == false && x.IsLast && x.CustomerId == model.CustomerId);

                    foreach (var d in oldDeliverys)
                    {
                        d.IsLast = false;
                    }

                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    delivery = new Delivery()
                    {
                        IsDelete = false,
                        AccountantFullName = string.Empty,
                        AccountantOfficeId = null,
                        AccountantOfficeIdPath = string.Empty,
                        AccountantOfficeName = string.Empty,
                        AccountantTime = null,
                        AccountantUserId = null,
                        AccountantUserTitleId = null,
                        AccountantUserTitleName = string.Empty,
                        AccountantUserUserName = string.Empty,
                        CreatedOfficeId = UserState.OfficeId ?? 0,
                        CreatedOfficeIdPath = UserState.OfficeIdPath,
                        CreatedOfficeName = UserState.OfficeName,
                        CreatedTime = timeNow,
                        CreatedUserFullName = UserState.FullName,
                        CreatedUserId = UserState.UserId,
                        CreatedUserTitleId = UserState.TitleId ?? 0,
                        CreatedUserTitleName = UserState.TitleName,
                        CreatedUserUserName = UserState.UserName,
                        IsLast = true,
                        Note = model.Note,
                        Status = (byte)DeliveryStatus.New,
                        OrderNo = listOrderIds.Count,
                        PackageNo = packages.Count,
                        CustomerId = customer.Id,
                        CustomerCode = customer.Code,
                        CustomerFullName = customer.FullName,
                        CustomerEmail = customer.Email,
                        CustomerPhone = customer.Phone,
                        CustomerVipId = customer.LevelId,
                        CustomerVipName = customer.LevelName,
                        WarehouseId = UserState.OfficeId ?? 0,
                        WarehouseIdPath = UserState.OfficeIdPath,
                        WarehouseName = UserState.OfficeName,
                        WarehouseAddress = string.Empty,
                        //PriceFast = totalFastDelivery,
                        PriceOrder = thisServiceOrder + thisOrder,
                        PriceOther = thisPriceOther,
                        PricePacking = thisPricePacking,
                        PriceShip = model.PriceShip,
                        PriceStored = thisPriceStored,
                        PriceWeight = thisPriceWeigth,
                        Weight = packages.Sum(x => x.Weight ?? 0),
                        WeightActual = packages.Sum(x => x.WeightActual ?? 0),
                        WeightConverted = packages.Sum(x => x.WeightConverted ?? 0),
                        Total = thisTotal,
                        Debit = debit,
                        PricePayed = thisDepositMoney,
                        Receivable = debit + debitPre,
                        DebitPre = debitPre,
                        DebitAfter = debit + debitPre,
                        Code = string.Empty,
                        UnsignedText = string.Empty
                    };

                    if (string.IsNullOrWhiteSpace(delivery.CustomerAddress))
                    {
                        delivery.CustomerAddress = string.Empty;
                    }

                    UnitOfWork.DeliveryRepo.Add(delivery);

                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    // Cập nhật lại Mã cho Orders và Total money
                    var deliveryOfDay = UnitOfWork.DeliveryRepo.Count(x =>
                        (x.CreatedTime.Year == timeNow.Year) && (x.CreatedTime.Month == timeNow.Month) &&
                        (x.CreatedTime.Day == timeNow.Day) && (x.Id <= delivery.Id));

                    delivery.Code = $"{deliveryOfDay}{timeNow:ddMMyy}";
                    delivery.UnsignedText = MyCommon.Ucs2Convert(
                        $"{delivery.Code} {customer.Email} {customer.FullName} {customer.Code} {customer.Phone} " +
                        $"{string.Join(" ", packages.Select(x=> x.Code).ToList() )} " +
                        $"{string.Join(" ", packages.Select(x => x.OrderCode).ToList())}");

                    var rs = await UnitOfWork.DeliveryRepo.SaveAsync();

                    // Thêm Detail cho phiếu xuất kho
                    if (rs > 0)
                    {
                        var dicOrderPriceShip = new Dictionary<int, decimal>();
                        var listDeliveryDetail = new List<DeliveryDetail>();

                        foreach (var p in packages)
                        {
                            var d = new DeliveryDetail
                            {
                                Created = timeNow,
                                DeliveryCode = delivery.Code,
                                DeliveryId = delivery.Id,
                                WalletCode = p.WalletCode,
                                IsDelete = false,
                                LayoutId = p.CurrentLayoutId ?? 0,
                                LayoutName = p.CurrentLayoutName,
                                Note = p.Note,
                                OrderCode = p.OrderCode,
                                OrderId = p.OrderId,
                                OrderPackageNo = p.PackageNo,
                                OrderServices = p.OrderServices,
                                OrderType = p.OrderType,
                                PackageCode = p.Code,
                                PackageId = p.Id,
                                TransportCode = p.TransportCode,
                                Updated = timeNow,
                                Weight = p.Weight,
                                WeightActual = p.WeightActual,
                                WeightConverted = p.WeightConverted,
                                WarehouseAddress = p.WarehouseAddress,
                                WarehouseIdPath = p.WarehouseIdPath,
                                WarehouseId = p.WarehouseId,
                                WarehouseName = p.WarehouseName,
                                PricePacking = p.TotalPriceWapperExchange,
                                PriceOther = p.OtherService,
                                Status = 0,
                                ShipDiscount = vipShip[p.CustomerLevelId],
                                Price = prices[p.OrderId],
                                PriceWeight =
                                    priceWeigth.ContainsKey(p.OrderId)
                                        ? priceWeigth[p.OrderId] * (p.WeightActualPercent ?? 0) / 100
                                        : 0,
                                PriceStored = priceStored.ContainsKey(p.Id) ? priceStored[p.Id] : 0,
                                PricePayed = depositMoney.ContainsKey(p.OrderId) ? depositMoney[p.OrderId] : 0
                            };

                            d.Price = prices.ContainsKey(p.OrderId) ? prices[p.OrderId] : 0;
                            d.PriceShip = ((d.WeightActual ?? 0) * 100 / (delivery.WeightActual ?? 0)) * delivery.PriceShip / 100;

                            if (p.CustomerWarehouseId != UserState.OfficeId.Value)
                            {
                                d.PriceWeight = 0;
                            }

                            // Tính tiền hoàn thành đơn của Orders trong phiếu giao hàng
                            if (order.ContainsKey(d.OrderId) && serviceOrder.ContainsKey(d.OrderId))
                            {
                                d.PriceOrder = order[d.OrderId] + serviceOrder[d.OrderId];
                            }else if (order.ContainsKey(d.OrderId))
                            {
                                d.PriceOrder = order[d.OrderId];
                            }else if (serviceOrder.ContainsKey(d.OrderId))
                            {
                                d.PriceOrder = serviceOrder[d.OrderId];
                            }
                            else
                            {
                                d.PriceOrder = 0;
                            }

                            if (dicOrderPriceShip.ContainsKey(d.OrderId))
                            {
                                dicOrderPriceShip[d.OrderId] += d.PriceShip;
                            }
                            else
                            {
                                dicOrderPriceShip.Add(d.OrderId, d.PriceShip);
                            }

                            listDeliveryDetail.Add(d);
                        }

                        // Tính tiền nợ của các package trong phiếu giao hàng
                        foreach (var d in listDeliveryDetail)
                        {
                            d.Debit = listDeliveryDetail.Where(x => x.OrderId == d.OrderId)
                                .Sum(x => (x.PriceWeight ?? 0) + (x.PricePacking ?? 0) + (x.PriceOther ?? 0) +
                                        (x.PriceStored ?? 0) + x.PriceShip);

                            var orderFirst = listDeliveryDetail.FirstOrDefault(x => x.OrderId == d.OrderId);

                            d.Debit += orderFirst?.PriceOrder ?? 0;
                            d.Debit -= orderFirst?.PricePayed ?? 0;
                        }

                        UnitOfWork.DeliveryDetailRepo.AddRange(listDeliveryDetail);

                        // Cập nhật tiền Ship hàng cho Orders
                        if (dicOrderPriceShip.Any())
                        {
                            await UpdatePriceShipToOrder(dicOrderPriceShip);
                        }

                        await UnitOfWork.DeliveryDetailRepo.SaveAsync();

                        // Cập nhật lịch sử package
                        var packagesHistory = await UnitOfWork.DeliveryRepo.GetPackageByDeliveryId(delivery.Id);

                        foreach (var p in packagesHistory)
                        {
                            p.Status = (byte)OrderPackageStatus.WaitDelivery;

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte) OrderPackageStatus.WaitDelivery,
                                Content = $"[{UserState.OfficeName}] {EnumHelper.GetEnumDescription(OrderPackageStatus.WaitDelivery)}",
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                                
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                            // Thêm note cho package và Orders
                            await PackageNote(p, delivery, PackageNoteMode.Delivery);
                        }
                    }

                    // Thông báo cho trưởng phòng kế toán duyệt phiếu giao hàng
                    // todo: Thông báo kế toán (- Có thể Edit để chỉ thông báo tới 1 kế toán cố định)
                    var usersAccountancy =
                        UnitOfWork.UserRepo.GetByExpression(user => user.IsDelete == false && user.Status < 5,
                            position => position.IsDefault && position.Type == 1,
                            office => office.Type == (byte) OfficeType.Accountancy);

                    var url = Url.Action("Index", "Delivery");

                    foreach (var u in usersAccountancy)
                    {
                        if (u.Id == UserState.UserId)
                            continue;

                        NotifyHelper.CreateAndSendNotifySystemToClient(u.Id,
                            $"{UserState.FullName} Request: to review delivery note", EnumNotifyType.Warning,
                            $"{UserState.FullName} Request: to review delivery note" +
                            $" #PGH-{delivery.Code}. <a href=\"{url}\" title=\"See details\">See more</a>",
                            $"{delivery.Code}_Approvel", url);
                    }

                    await UnitOfWork.DeliveryDetailRepo.SaveAsync();

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
            BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));

            return JsonCamelCaseResult(new { Status = 1, Text = "Created note successfully", DeliveryCode = delivery.Code },
                    JsonRequestBehavior.AllowGet);
        }

        // Cập nhật tiền ship cho Orders
        private async Task UpdatePriceShipToOrder(Dictionary<int, decimal> model)
        {
            foreach (var d in model)
            {
                var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    x =>
                        x.OrderId == d.Key && x.IsDelete == false &&
                        x.ServiceId == (byte) OrderServices.InSideShipping);

                if (shipToHomeService == null)
                {
                    shipToHomeService = new OrderService()
                    {
                        OrderId = d.Key,
                        ServiceId = (byte) OrderServices.InSideShipping,
                        ServiceName =
                            OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                        ExchangeRate = ExchangeRate(),
                        Value = d.Value,
                        Currency = Currency.VND.ToString(),
                        Type = (byte) UnitType.Money,
                        TotalPrice = d.Value,
                        Mode = (byte) OrderServiceMode.Required,
                        Checked = true,
                        Created = DateTime.Now,
                        LastUpdate = DateTime.Now
                    };
                    UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                }
                else
                {
                    shipToHomeService.TotalPrice += d.Value;
                    shipToHomeService.Value += d.Value;
                }

                await UnitOfWork.OrderServiceRepo.SaveAsync();


                // Cập nhật lại Total money của Orders
                var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == d.Key);

                var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                         x.IsDelete == false && x.Checked)
                    .ToList()
                    .Sum(x => x.TotalPrice);

                order.Total = order.TotalExchange + totalService;
                order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                await UnitOfWork.OrderRepo.SaveAsync();
            }
        }

        private async Task PackageNote(OrderPackage package, Delivery delivery, PackageNoteMode packageNoteMode)
        {
            //Thêm note cho các Orders trong phiếu nhập
            var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                                x =>
                                    x.PackageId == null && x.OrderId == package.OrderId && x.ObjectId == delivery.Id &&
                                    x.Mode == (byte)packageNoteMode);

            if (packageNote == null && !string.IsNullOrWhiteSpace(delivery.Note))
            {
                UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                {
                    OrderId = package.OrderId,
                    OrderCode = package.OrderCode,
                    PackageId = null,
                    PackageCode = null,
                    UserId = delivery.CreatedUserId,
                    UserFullName = delivery.CreatedUserFullName,
                    Time = DateTime.Now,
                    ObjectId = delivery.Id,
                    ObjectCode = delivery.Code,
                    Mode = (byte)packageNoteMode,
                    Content = delivery.Note
                });
            }
            else if (packageNote != null && !string.IsNullOrWhiteSpace(delivery.Note))
            {
                packageNote.Content = delivery.Note;
            }
            else if (packageNote != null && string.IsNullOrWhiteSpace(delivery.Note))
            {
                UnitOfWork.PackageNoteRepo.Remove(packageNote);
            }
            
            await UnitOfWork.PackageNoteRepo.SaveAsync();
        }


        public async Task<ActionResult> FixDelivery()
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    int deliveryId = 464;

                    var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.Id == deliveryId);

                    var deliveryDetail =
                        await UnitOfWork.DeliveryDetailRepo.FindAsync(x => x.IsDelete == false && x.DeliveryId == delivery.Id);

                    var orderIds = $";{string.Join(";", deliveryDetail.Select(x => x.OrderId).Distinct().ToList())};";
                    var packageIds = $";{string.Join(";", deliveryDetail.Select(x => x.PackageId).Distinct().ToList())};";


                    var packages = await UnitOfWork.OrderPackageRepo.GetByPackageIds(packageIds);

                    // Tính tiền vận chuyển TQ, VN
                    var priceWeigth = UnitOfWork.OrderServiceRepo.Entities.Where(x => orderIds.Contains(";" + x.OrderId + ";") &&
                                                                     x.IsDelete == false && x.Checked &&
                                                                     x.ServiceId == (byte)OrderServices.OutSideShipping)
                         .GroupBy(x => x.OrderId)
                         .ToDictionary(x => x.Key, x => x.Sum(y => y.TotalPrice));

                    decimal totalPriceWeigth = 0;

                    foreach (var pw in priceWeigth)
                    {
                        var totalPercent =
                            packages.Where(x => x.OrderId == pw.Key && x.CustomerWarehouseId == delivery.CreatedOfficeId)
                                .Sum(x => x.WeightActualPercent ?? 0);
                        totalPriceWeigth += pw.Value * totalPercent / 100;
                    }

                    // Tiền cân nặng trong phiếu này
                    var thisPriceWeigth = totalPriceWeigth;

                    var oldPriceWight = delivery.PriceWeight;

                    delivery.PriceWeight = thisPriceWeigth;
                    delivery.Weight = packages.Sum(x => x.Weight ?? 0);
                    delivery.WeightActual = packages.Sum(x => x.WeightActual ?? 0);
                    delivery.WeightConverted = packages.Sum(x => x.WeightConverted ?? 0);
                    delivery.Total = delivery.Total - oldPriceWight + thisPriceWeigth;
                    delivery.Debit = delivery.Total - delivery.PricePayed;
                    delivery.Receivable = delivery.Debit + delivery.DebitPre;
                    delivery.DebitAfter = delivery.Debit + delivery.DebitAfter;

                    await UnitOfWork.DeliveryDetailRepo.SaveAsync();

                    foreach (var d in deliveryDetail)
                    {
                        var p = packages.SingleOrDefault(x => x.Id == d.PackageId);

                        d.PriceWeight = priceWeigth.ContainsKey(p.OrderId)
                            ? priceWeigth[p.OrderId] * (p.WeightActualPercent ?? 0) / 100
                            : 0;

                        if (p.CustomerWarehouseId != delivery.CreatedOfficeId)
                            d.PriceWeight = 0;
                    }

                    await UnitOfWork.DeliveryDetailRepo.SaveAsync();


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }
    }
}