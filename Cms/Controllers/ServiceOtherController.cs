using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Common.Emums;
using Common.Helper;
using Hangfire;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Jobs;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    /// <summary>
    /// Theo dõi chi phí phát sinh
    /// </summary>
    [Authorize]
    public class ServiceOtherController : BaseController
    {
        // GET: ServiceOther
        [LogTracker(EnumAction.View, EnumPage.ServiceOther)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type != null && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

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

            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<OrderType>((int) v)})
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => (byte) v, v => EnumHelper.GetEnumDescription<OrderType>((int) v)),
                jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.ServiceOther)]
        public async Task<ActionResult> Search(string warehouseIdPath, byte? mode, int? systemId, byte? orderType, 
            DateTime? fromDate, DateTime? toDate, string keyword = "", int currentPage = 1,
           int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var isManager = UserState.Type != null && (UserState.Type.Value == 2 || UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            //if (!isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
            //    warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord;

            var items = await UnitOfWork.OrderServiceOtherRepo.Search(out totalRecord, isManager, warehouseIdPath, mode,
                systemId, orderType,
                fromDate, toDate, keyword, currentPage,
                recordPerPage);

            return JsonCamelCaseResult(new { items, totalRecord},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.ServiceOther)]
        public async Task<ActionResult> UpdateNote(int id, string note)
        {
            var serviceOther = await UnitOfWork.OrderServiceOtherRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (serviceOther == null)
                return JsonCamelCaseResult(
                    new { Status = -2, Text = "This incurred expense does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var order =
                await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == serviceOther.OrderId);

                    if (order == null)
                        return JsonCamelCaseResult(
                            new { Status = -3, Text = "Order does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

//                    var packages = serviceOther.Type == 2
//                        ? UnitOfWork.OrderPackageRepo.GetByOrderIdAndWalletId(order.Id,
//                            serviceOther.ObjectId)
//                        : UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
//                            serviceOther.ObjectId);

                    // Kiểm tra Order này đã được giao cho khách hàng hay chưa
//                    if (packages.Any(x => x.Status >= (byte)OrderPackageStatus.WaitDelivery))
//                        return JsonCamelCaseResult(
//                            new { Status = -4, Text = "Không thể sử Order đã tạo phiếu giao hàng" },
//                            JsonRequestBehavior.AllowGet);

                    if(order.Type != (byte)OrderType.Deposit && order.Status == (byte)OrderStatus.Finish ||
                        order.Type == (byte)OrderType.Deposit && order.Status == (byte)DepositStatus.Finish)
                        return JsonCamelCaseResult(
                            new { Status = -4, Text = "Unable to use already completed Order" },
                            JsonRequestBehavior.AllowGet);

                    serviceOther.Note = note;

                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                    x => x.PackageId == null && x.OrderId == serviceOther.OrderId && x.ObjectId == serviceOther.Id &&
                        x.Mode == (byte)PackageNoteMode.OtherSerive);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = serviceOther.OrderId,
                            OrderCode = serviceOther.OrderCode,
                            PackageId = null,
                            PackageCode = null,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = serviceOther.Id,
                            ObjectCode = serviceOther.Id.ToString(),
                            Mode = (byte)PackageNoteMode.OtherSerive,
                            Content = note,
                            DataJson = JsonConvert.SerializeObject(serviceOther, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() })
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

                    await UnitOfWork.ImportWarehouseDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(
                new { Status = 1, Text = "Update note successfully" },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.ServiceOther)]
        public async Task<ActionResult> UpdateValue(int id, decimal value)
        {
            if (value <= 0)
                return JsonCamelCaseResult(
                    new {Status = -1, Text = "The cost incurred must be > 0" },
                    JsonRequestBehavior.AllowGet);

            var timeNow = DateTime.Now;
            Order order;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceOther = await UnitOfWork.OrderServiceOtherRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if (serviceOther == null)
                        return JsonCamelCaseResult(
                            new { Status = -2, Text = "This incurred fee does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    order =
                        await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == serviceOther.OrderId);

                    if (order == null)
                        return JsonCamelCaseResult(
                            new { Status = -3, Text = "Order does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

//                    var packages = serviceOther.Type == 2
//                        ? UnitOfWork.OrderPackageRepo.GetByOrderIdAndWalletId(order.Id,
//                            serviceOther.ObjectId)
//                        : UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
//                            serviceOther.ObjectId);

                    // Kiểm tra Order này đã được giao cho khách hàng hay chưa
//                    if (packages.Any(x => x.Status >= (byte)OrderPackageStatus.WaitDelivery))
//                        return JsonCamelCaseResult(
//                            new { Status = -4, Text = "Không thể sử Order đã tạo phiếu giao hàng" },
//                            JsonRequestBehavior.AllowGet);

                    if (order.Type != (byte) OrderType.Deposit && order.Status == (byte) OrderStatus.Finish ||
                        order.Type == (byte) OrderType.Deposit && order.Status == (byte) DepositStatus.Finish)
                        return JsonCamelCaseResult(
                            new {Status = -4, Text = "Unable to use already completed Order"},
                            JsonRequestBehavior.AllowGet);

                    serviceOther.Value = value;
                    serviceOther.TotalPrice = value * serviceOther.ExchangeRate;

                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                    // Cập nhật lại tiền dịch vụ của Order
                    var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                    var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                    x.ServiceId == (byte)OrderServices.Other && x.Checked);

                    if (otherService == null)
                    {
                        otherService = new OrderService
                        {
                            IsDelete = false,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Value = totalServiceOther.Sum(x => x.Value),
                            Currency = Currency.CNY.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                            HashTag = string.Empty,
                            Mode = (byte)OrderServiceMode.Option,
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Other,
                            ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                .Description,
                            Type = (byte)UnitType.Money,
                            Note = $"Service fees incurred. (Shop charging shipping fees after stock arrival, forklift renting fee...)",
                        };

                        UnitOfWork.OrderServiceRepo.Add(otherService);
                    }
                    else
                    {
                        otherService.LastUpdate = timeNow;
                        otherService.Value = totalServiceOther.Sum(x => x.Value);
                        otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                    }
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật lại tiền dịch vụ của Order
                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                             x.IsDelete == false && x.Checked)
                        .ToList()
                        .Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
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

            // Tính toán chi phí phát sinh cảu Order
            BackgroundJob.Enqueue(() => PackageJob.UpdateOtherServiceOfPackage(new List<int> { order.Id }));

            return JsonCamelCaseResult(
                new {Status = 1, Text = "Update incurred fees successfully"},
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Delete, EnumPage.ServiceOther)]
        public async Task<ActionResult> Delete(int id)
        {
            var timeNow = DateTime.Now;
            Order order;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceOther = await UnitOfWork.OrderServiceOtherRepo.SingleOrDefaultAsync(x => x.Id == id);

                    if (serviceOther == null)
                        return JsonCamelCaseResult(
                            new {Status = -2, Text = "This incurred expense does not exist or has been deleted"},
                            JsonRequestBehavior.AllowGet);

                    order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => x.IsDelete == false && x.Id == serviceOther.OrderId);

                    if (order == null)
                        return JsonCamelCaseResult(
                            new {Status = -3, Text = "Order does not exist or has been deleted"},
                            JsonRequestBehavior.AllowGet);

                    if (order.Type != (byte) OrderType.Deposit && order.Status == (byte) OrderStatus.Finish ||
                        order.Type == (byte) OrderType.Deposit && order.Status == (byte) DepositStatus.Finish)
                        return JsonCamelCaseResult(
                            new {Status = -4, Text = "Unable to use already completed Order"},
                            JsonRequestBehavior.AllowGet);

                    UnitOfWork.OrderServiceOtherRepo.Remove(serviceOther);
                    await UnitOfWork.OrderServiceOtherRepo.SaveAsync();

                    // Cập nhật lại tiền dịch vụ của Order
                    var totalServiceOther = await UnitOfWork.OrderServiceOtherRepo.FindAsync(x => x.OrderId == order.Id);

                    var otherService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        x => !x.IsDelete && x.OrderId == order.Id &&
                             x.ServiceId == (byte) OrderServices.Other && x.Checked);

                    if (otherService == null)
                    {
                        otherService = new OrderService
                        {
                            IsDelete = false,
                            Checked = true,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            Value = totalServiceOther.Sum(x => x.Value),
                            Currency = Currency.CNY.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            TotalPrice = totalServiceOther.Sum(x => x.TotalPrice),
                            HashTag = string.Empty,
                            Mode = (byte)OrderServiceMode.Option,
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.Other,
                            ServiceName = (OrderServices.Other).GetAttributeOfType<DescriptionAttribute>()
                                .Description,
                            Type = (byte)UnitType.Money,
                            Note = $"Service fees incurred. (Shop charging shipping fees after stock arrival at warehouse, forklift renting fee...)",
                        };

                        UnitOfWork.OrderServiceRepo.Add(otherService);
                    }
                    else
                    {
                        otherService.LastUpdate = timeNow;
                        otherService.Value = totalServiceOther.Sum(x => x.Value);
                        otherService.TotalPrice = totalServiceOther.Sum(x => x.TotalPrice);
                    }
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật lại tiền dịch vụ của Order
                    var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                             x.IsDelete == false && x.Checked)
                        .ToList()
                        .Sum(x => x.TotalPrice);

                    order.Total = order.TotalExchange + totalService;
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

            // Tính toán chi phí phát sinh cảu Order
            BackgroundJob.Enqueue(() => PackageJob.UpdateOtherServiceOfPackage(new List<int> { order.Id }));

            return JsonCamelCaseResult(
                new { Status = 1, Text = "Delete incurred fees successfully" },
                JsonRequestBehavior.AllowGet);
        }
    }
}