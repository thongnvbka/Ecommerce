using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Cms.Attributes;
using Cms.Helpers;
using Cms.Jobs;
using Common.Emums;
using Common.Helper;
using Hangfire;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.DbContext.Results;
using Library.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    [Authorize]
    public class DeliveryController : BaseController
    {
        // GET: Delivery
        [LogTracker(EnumAction.View, EnumPage.Delivery)]
        public async Task<ActionResult> Index()
        {
            var isManager = UserState.Type.HasValue && ((UserState.Type.Value == 2) || (UserState.Type.Value == 1));

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (isManager || UserState.OfficeType == (byte)OfficeType.Accountancy)
            {
                var warehouses = await UnitOfWork.OfficeRepo.FindAsNoTrackingAsync(
                    x => (UserState.OfficeType == (byte)OfficeType.Accountancy || (isManager && (x.IdPath == UserState.OfficeIdPath || x.IdPath.StartsWith(UserState.OfficeIdPath + ".")) ||
                         !isManager && x.IdPath == UserState.OfficeIdPath)) &&
                        x.Type == (byte)OfficeType.Warehouse &&
                        !x.IsDelete && (x.Status == (byte)OfficeStatus.Use));

                ViewBag.Warehouses = JsonConvert.SerializeObject(warehouses.Select(Mapper.Map<OfficeDropdownResult>),
                    jsonSerializerSettings);
            }

            ViewBag.StatesSelectListItem = Enum.GetValues(typeof(DeliveryStatus))
                .Cast<DeliveryStatus>()
                .Select(
                    v =>
                        new SelectListItem()
                        {
                            Value = ((byte)v).ToString(),
                            Text = EnumHelper.GetEnumDescription<DeliveryStatus>((int)v),
                            Selected = v == DeliveryStatus.Approved
                        })
                .ToList();

            ViewBag.DeliveryStates = JsonConvert.SerializeObject(Enum.GetValues(typeof(DeliveryStatus))
                .Cast<DeliveryStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<DeliveryStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Delivery)]
        public async Task<ActionResult> Search(string warehouseIdPath, int? createdUserId, int? expertiseUserTitleId,
            byte? status, byte? type, int? shipperId, DateTime? fromDate, string moneyText,
            DateTime? toDate, string keyword = "", int currentPage = 1, int recordPerPage = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            // moneyText: dạng: ";0;1;2;3;"
            // moneyText: chứa 0: Phiếu còn nợ tiền
            // moneyText chứa 1: Phiếu đã thu đủ
            // moneyText chứa 2: Phiếu có nợ kỳ trước

            // ReSharper disable once PossibleInvalidOperationException
            var isManager = (UserState.Type.Value == 2) || (UserState.Type.Value == 1);

            // Không phải là trưởng đơn vị hoặc là trưởng đơn vị và warehouseIdPath là null
            var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(warehouseIdPath);

            var isAudit = UserState.OfficeType.HasValue && UserState.OfficeType == (byte)OfficeType.Accountancy;

            if (!isAudit || !isManager || string.IsNullOrWhiteSpace(warehouseIdPath))
                warehouseIdPath = UserState.OfficeIdPath;

            long totalRecord;

            var items = await UnitOfWork.DeliveryRepo.FindAsNoTrackingAsync(out totalRecord,
                x => x.UnsignedText.Contains(keyword) && (status == null || x.Status == status.Value) &&
                     (createdUserId == null || x.CreatedUserId == createdUserId.Value) &&
                     (expertiseUserTitleId == null || x.ExpertiseUserId == expertiseUserTitleId.Value) &&
                     x.IsDelete == false &&
                     (isNullOrWhiteSpace || isManager && (x.CreatedOfficeIdPath == warehouseIdPath
                      || x.CreatedOfficeIdPath.StartsWith(warehouseIdPath +"."))
                      || !isManager && x.CreatedOfficeIdPath == warehouseIdPath) &&
                     (fromDate == null && toDate == null 
                      || fromDate != null && toDate != null && x.CreatedTime >= fromDate && x.CreatedTime <= toDate 
                      || fromDate == null && toDate.HasValue && x.CreatedTime <= toDate 
                      || toDate == null && fromDate.HasValue && x.CreatedTime >= fromDate) &&
                     (moneyText == "" || !moneyText.Contains("0") || moneyText.Contains("0") && x.DebitAfter >= 100) &&
                     (moneyText == "" || !moneyText.Contains("1") || moneyText.Contains("1") && x.DebitAfter < 100) &&
                     (moneyText == "" || !moneyText.Contains("2") || moneyText.Contains("2") && x.DebitPre >= 100),
                x => x.OrderByDescending(y => y.Id),
                currentPage, recordPerPage);

            return JsonCamelCaseResult(new { items, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetDetail(int id)
        {
            var delivery =
                await UnitOfWork.DeliveryRepo.SingleOrDefaultAsNoTrackingAsync(x => x.Id == id && x.IsDelete == false);

            if (delivery == null)
                return JsonCamelCaseResult(new { Status = -1, Text = "Phiếu giao hàng Does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            var packages = await UnitOfWork.DeliveryDetailRepo.FindAsNoTrackingAsync(
                x => x.DeliveryId == id && x.IsDelete == false, x => x.OrderBy(y => y.OrderCode), null);

            return JsonCamelCaseResult(new { packages, delivery }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="codeOrder"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.Add, EnumPage.Delivery)]
        public async Task<ActionResult> SearchOrder(string term, string codeOrder, byte size = 6)
        {
            term = MyCommon.Ucs2Convert(term);

            // Chỉ lấy mã dạng số: (vd: ODR005566 chỉ lấy 005566)
            // term = Regex.Replace(term, "[^0-9]", "");

            //Orders khác ký gửi
            var items = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(
                x => x.Code.Contains(term) && !x.IsDelete && x.Type != (byte)OrderType.Deposit &&
                     (x.Status == (byte)OrderStatus.InWarehouse || x.Status == (byte)OrderStatus.Shipping) &&
                     (codeOrder == "" || !codeOrder.Contains(";" + x.Code + ";")),
                x => x.OrderBy(y => y.Id), 1, size);

            //Orders ký gửi
            var itemDeposits = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(
                x => x.Code.Contains(term) && !x.IsDelete && x.Type == (byte)OrderType.Deposit &&
                     (x.Status == (byte)DepositStatus.InWarehouse || x.Status == (byte)DepositStatus.Shipping) &&
                     (codeOrder == "" || !codeOrder.Contains(";" + x.Code + ";")),
                x => x.OrderBy(y => y.Id), 1, size);

            items.AddRange(itemDeposits);

            return JsonCamelCaseResult(
                items.Select(x => new { x.Code, x.CustomerName, x.CustomerEmail, x.CustomerPhone }),
                JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Approvel, EnumPage.Delivery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForComplete(int id)
        {
            //if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte)OfficeType.Accountancy)
            //    return
            //        JsonCamelCaseResult(
            //            new { Status = -1, Text = "Only the accountant has the right to do this" },
            //            JsonRequestBehavior.AllowGet);

            var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

            if (delivery == null)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu Does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Status != (byte)DeliveryStatus.Approved)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu chưa được duyệt không thể hoàn thành" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Status == (byte)DeliveryStatus.Complete)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu này đã hoàn thành, không thể thao tác" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Receivable.HasValue && delivery.Receivable.Value > 100)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu cần hoàn thành nợ trước khi hoàn thành" },
                    JsonRequestBehavior.AllowGet);

            var customer =
                await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                    x => x.IsDelete == false && x.Id == delivery.CustomerId);

            if (customer == null)
                return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            if (!delivery.IsLast && delivery.DebitAfter >= 100)
                return JsonCamelCaseResult(new { Status = -2, Text = "Bạn phải duyệt phiếu xuất cuối cùng của khách hàng" },
                    JsonRequestBehavior.AllowGet);

            List<Order> orders = null;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                            x => x.IsDelete == false && x.Id == delivery.CustomerId);

                    if (customer == null)
                        return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                            JsonRequestBehavior.AllowGet);

                    //  Lấy ra các phiếu còn nợ tiền
                    var deliveryPre = await UnitOfWork.DeliveryRepo.FindAsync(
                        x => x.IsDelete == false && x.CustomerId == delivery.CustomerId && x.Id <= delivery.Id &&
                             x.Status == (byte)DeliveryStatus.Approved && x.Receivable < 100);

                    deliveryPre = deliveryPre.OrderBy(x => x.DebitPre).ToList();

                    foreach (var d in deliveryPre)
                    {
                        if (d.DebitAfter == null || d.DebitAfter < 100)
                        {
                            d.ExpertiseUserId = UserState.UserId;
                            d.ExpertiseUserFullName = UserState.FullName;
                            d.ExpertiseUserUserName = UserState.UserName;
                            d.ExpertiseOfficeId = UserState.OfficeId;
                            d.ExpertiseOfficeName = UserState.OfficeName;
                            d.ExpertiseOfficeIdPath = UserState.OfficeIdPath;
                            d.ExpertiseUserTitleId = UserState.TitleId;
                            d.ExpertiseUserTitleName = UserState.TitleName;
                            d.ExpertiseTime = DateTime.Now;
                            d.Status = (byte)DeliveryStatus.Complete;
                        }

                        // Cập nhật lịch sử package
                        var packages = await UnitOfWork.DeliveryRepo.GetPackageByDeliveryId(d.Id);

                        foreach (var p in packages)
                        {
                            p.Status = (byte)OrderPackageStatus.Completed;

                            // Thêm lịch sử cho package
                            var packageHistory = new PackageHistory()
                            {
                                PackageId = p.Id,
                                PackageCode = p.Code,
                                OrderId = p.OrderId,
                                OrderCode = p.OrderCode,
                                Type = p.OrderType,
                                Status = (byte)OrderPackageStatus.Completed,
                                Content = EnumHelper.GetEnumDescription(OrderPackageStatus.Completed),
                                CustomerId = p.CustomerId,
                                CustomerName = p.CustomerName,
                                UserId = UserState.UserId,
                                UserName = UserState.UserName,
                                UserFullName = UserState.FullName,
                                CreateDate = DateTime.Now,
                            };

                            UnitOfWork.PackageHistoryRepo.Add(packageHistory);
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        // Cập nhật số lượng kiện đã giao cho các Orders
                        var strCodeOrder = $";{string.Join(";", packages.Select(x => x.OrderCode).Distinct().ToList())};";

                        orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";"));

                        // Cập nhật trạng thái Orders đang giao hàng
                        foreach (var order in orders)
                        {
                            var packageNo = await UnitOfWork.OrderPackageRepo.CountAsync(
                                x =>
                                    x.IsDelete == false && x.OrderId == order.Id &&
                                    x.Status == (byte)OrderPackageStatus.Completed);

                            order.LastUpdate = DateTime.Now;
                            order.PackageNoDelivered = packageNo;
                            order.LastDeliveryTime = DateTime.Now;

                            // Hoàn tiền cho các Orders trong phiếu giao hàng đã hoàn thành
                            if (d.Status == (byte) DeliveryStatus.Complete && order.Debt < -100)
                            {
                                var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                                {
                                    CustomerId = customer.Id,
                                    CurrencyFluctuations = Math.Abs(order.Debt),
                                    Note = $"Return order excess money when completing dispatch note D{d.Code}",
                                    TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
                                });

                                // Lỗi trong quá tình thực hiện thanh toán
                                if (processRechargeBillResult.Status < 0)
                                {
                                    transaction.Rollback();

                                    return JsonCamelCaseResult(
                                            new { Status = 1, Text = processRechargeBillResult.Msg, DeliveryCode = processRechargeBillResult.Status },
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
                                    Note = $"Hoàn tiền thừa dư đơn hàng khi hoàn thành phiếu xuất D{d.Code}",
                                    OrderId = order.Id,
                                    TotalPrice = Math.Abs(order.Debt),
                                    Status = (byte)OrderExchangeStatus.Approved
                                });

                                order.TotalRefunded += Math.Abs(order.Debt);
                                order.Debt = 0;
                            }

                            await UnitOfWork.OrderRepo.SaveAsync();
                        }
                    }

                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    var url = Url.Action("Index", "Delivery");

                    NotifyHelper.CreateAndSendNotifySystemToClient(delivery.CreatedUserId,
                        $"{UserState.FullName} Confirm completing delivery note-{delivery.Code}", EnumNotifyType.Warning,
                        $"{UserState.FullName} has confirmed completing delivery note" +
                        $" PGH-{delivery.Code}. <a href=\"{url}\" title=\"See details\">See more</a>",
                        $"{delivery.Code}_Vertify", url);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            if (orders != null)
            {
                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                // Cập nhật công nợ
                BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));
            }

            //// todo: HOàn tiền khách hàng
            //var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
            //{
            //    CustomerId = customer.Id,
            //    CurrencyFluctuations = 1443,
            //    Note = $"Charge money electronic wallet hàng do Refund Orders: D{delivery.Code}",
            //    TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
            //});

            return JsonCamelCaseResult(new { Status = 1, Text = "Complete the note successfully" },
                JsonRequestBehavior.AllowGet);
        }

        //[CheckPermission(EnumAction.Approvel, EnumPage.Delivery)]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> RechargeForComplete(int id, decimal money)
        //{
        //    if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte)OfficeType.Accountancy)
        //        return
        //            JsonCamelCaseResult(
        //                new { Status = -1, Text = "Only the accountant has the right to do this" },
        //                JsonRequestBehavior.AllowGet);

        //    var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

        //    if (delivery == null)
        //        return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu Does not exist or has been deleted" },
        //            JsonRequestBehavior.AllowGet);

        //    if (delivery.Status != (byte)DeliveryStatus.Approved)
        //        return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu chưa được duyệt không thể hoàn thành" },
        //            JsonRequestBehavior.AllowGet);

        //    if (!delivery.IsLast)
        //        return JsonCamelCaseResult(new { Status = -2, Text = "Bạn phải duyệt phiếu xuất cuối cùng của khách hàng" },
        //            JsonRequestBehavior.AllowGet);

        //    var customer =
        //        await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
        //            x => x.IsDelete == false && x.Id == delivery.CustomerId);

        //    if (customer == null)
        //        return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
        //            JsonRequestBehavior.AllowGet);

        //    if (money < 100)
        //    {
        //        return JsonCamelCaseResult(new { Status = -3, Text = "Số tiền nạp tài khoản khách phải > 100" },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            // Trừ tiền tài khoản khách
        //            var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
        //            {
        //                CustomerId = customer.Id,
        //                CurrencyFluctuations = money,
        //                Note = $"Nạp tiền thanh toán từ phiếu giao: D{delivery.Code}",
        //                TreasureIdd = (int)TreasureCustomerWallet.ShipperReturn
        //            });

        //            // Lỗi trong quá tình thực hiện thanh toán
        //            if (processRechargeBillResult.Status < 0)
        //            {
        //                return JsonCamelCaseResult(new { processRechargeBillResult.Status, Text = processRechargeBillResult.Msg },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
        //                    x => x.IsDelete == false && x.Id == delivery.CustomerId);

        //            if (customer == null)
        //                return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
        //                    JsonRequestBehavior.AllowGet);

        //            var rs = await DeductDeliverys(delivery, customer);

        //            if (rs.Status <= 0)
        //            {
        //                transaction.Rollback();

        //                return JsonCamelCaseResult(new { Status = rs.Status * 100, Text = rs.Msg },
        //                    JsonRequestBehavior.AllowGet);
        //            }

        //            var url = Url.Action("Index", "Delivery");

        //            NotifyHelper.CreateAndSendNotifySystemToClient(delivery.CreatedUserId,
        //                $"{UserState.FullName} Duyệt PGH-{delivery.Code}", EnumNotifyType.Warning,
        //                $"{UserState.FullName} đã duyệt phiếu giao hàng" +
        //                $" PGH-{delivery.Code}. <a href=\"{url}\" title=\"See details\">See more</a>",
        //                $"{delivery.Code}_Vertify", url);

        //            transaction.Commit();
        //        }
        //        catch (Exception)
        //        {
        //            transaction.Rollback();
        //            throw;
        //        }
        //    }

        //    // Lấy lại delivery
        //    var deliveryAfter = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

        //    if (deliveryAfter.DebitAfter > 0)
        //    {
        //        return JsonCamelCaseResult(new
        //        {
        //            Status = 1,
        //            Text = "Nạp tiền, trừ tiền thành công và số tiền của " +
        //                                                            "khách hàng không đủ để hoàn thành phiếu giao"
        //        },
        //            JsonRequestBehavior.AllowGet);
        //    }

        //    return JsonCamelCaseResult(new { Status = 1, Text = "Complete the note successfully" },
        //        JsonRequestBehavior.AllowGet);
        //}

        [CheckPermission(EnumAction.Approvel, EnumPage.Delivery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeductForComplete(int id)
        {
            //if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte)OfficeType.Accountancy)
            //    return
            //        JsonCamelCaseResult(
            //            new { Status = -1, Text = "Only the accountant has the right to do this" },
            //            JsonRequestBehavior.AllowGet);

            var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

            if (delivery == null)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu Does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Status != (byte)DeliveryStatus.Approved)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu chưa được duyệt không thể hoàn thành" },
                    JsonRequestBehavior.AllowGet);

            if (!delivery.IsLast)
                return JsonCamelCaseResult(new { Status = -2, Text = "Bạn phải duyệt phiếu xuất cuối cùng của khách hàng" },
                    JsonRequestBehavior.AllowGet);

            var customer =
                await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                    x => x.IsDelete == false && x.Id == delivery.CustomerId);

            if (customer == null)
                return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var rs = await DeductDeliverys(delivery, customer);

                    if (rs.Status <= 0)
                    {
                        transaction.Rollback();

                        return JsonCamelCaseResult(new { Status = rs.Status * 100, Text = rs.Msg },
                            JsonRequestBehavior.AllowGet);
                    }

                    var url = Url.Action("Index", "Delivery");

                    NotifyHelper.CreateAndSendNotifySystemToClient(delivery.CreatedUserId,
                        $"{UserState.FullName} Confirm completing delivery note-{delivery.Code}", EnumNotifyType.Warning,
                        $"{UserState.FullName} has confirmed completing delivery note" +
                        $" PGH-{delivery.Code}. <a href=\"{url}\" title=\"See details\">See more</a>",
                        $"{delivery.Code}_Vertify", url);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            // Lấy lại delivery
            var deliveryAfter = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

            if (deliveryAfter.DebitAfter > 100)
            {
                return JsonCamelCaseResult(new { Status = 1, Text = "Trừ tiền thành công và số tiền của khách hàng không đủ để hoàn thành phiếu giao" },
                    JsonRequestBehavior.AllowGet);
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Complete the note successfully" },
                JsonRequestBehavior.AllowGet);
        }

        private async Task<ProcessRechargeBillResult> DeductDeliverys(Delivery delivery, Customer customer)
        {
            //  Lấy ra các phiếu còn nợ tiền
            var deliveryPre = await UnitOfWork.DeliveryRepo.FindAsync(
                    x => x.IsDelete == false && x.CustomerId == delivery.CustomerId && x.Id <= delivery.Id &&
                        x.DebitAfter.HasValue && (x.DebitAfter >= 100 || x.Status >= (byte)DeliveryStatus.Approved && x.Status < (byte)DeliveryStatus.Complete));

            deliveryPre = deliveryPre.OrderBy(x => x.DebitPre).ToList();

            List<Order> orders = null;

            foreach (var d in deliveryPre)
            {
                var rs = await PayDelivery(d, customer);

                if (rs.Status < 0)
                    return rs;

                if (d.DebitAfter == null || d.DebitAfter < 100)
                {
                    d.ExpertiseUserId = UserState.UserId;
                    d.ExpertiseUserFullName = UserState.FullName;
                    d.ExpertiseUserUserName = UserState.UserName;
                    d.ExpertiseOfficeId = UserState.OfficeId;
                    d.ExpertiseOfficeName = UserState.OfficeName;
                    d.ExpertiseOfficeIdPath = UserState.OfficeIdPath;
                    d.ExpertiseUserTitleId = UserState.TitleId;
                    d.ExpertiseUserTitleName = UserState.TitleName;
                    d.ExpertiseTime = DateTime.Now;
                    d.Status = (byte)DeliveryStatus.Complete;
                }

                // Cập nhật lịch sử package
                var packages = await UnitOfWork.DeliveryRepo.GetPackageByDeliveryId(d.Id);

                foreach (var p in packages)
                {
                    p.Status = (byte)OrderPackageStatus.Completed;

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        Type = p.OrderType,
                        Status = (byte)OrderPackageStatus.Completed,
                        Content = EnumHelper.GetEnumDescription(OrderPackageStatus.Completed),
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);
                }

                await UnitOfWork.OrderPackageRepo.SaveAsync();

                // Cập nhật số lượng kiện đã giao cho các Orders
                var strCodeOrder = $";{string.Join(";", packages.Select(x => x.OrderCode).Distinct().ToList())};";

                orders = await UnitOfWork.OrderRepo.FindAsync(x => strCodeOrder.Contains(";" + x.Code + ";"));

                // Cập nhật trạng thái Orders đang giao hàng
                foreach (var order in orders)
                {
                    var packageNo = await UnitOfWork.OrderPackageRepo.CountAsync(
                        x =>
                            x.IsDelete == false && x.OrderId == order.Id &&
                            x.Status == (byte)OrderPackageStatus.Completed);

                    order.PackageNoDelivered = packageNo;
                    order.LastDeliveryTime = DateTime.Now;

                    await UnitOfWork.OrderRepo.SaveAsync();
                }
            }

            await UnitOfWork.DeliveryRepo.SaveAsync();

            if (orders != null)
            {
                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                // Cập nhật công nợ
                BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));
            }

            return new ProcessRechargeBillResult() { Status = 1, Msg = "Thanh toán thành công" };
        }

        [CheckPermission(EnumAction.Approvel, EnumPage.Delivery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approvel(int id)
        {
            //if (UserState.OfficeType.HasValue && UserState.OfficeType != (byte)OfficeType.Accountancy)
            //    return
            //        JsonCamelCaseResult(
            //            new { Status = -1, Text = "Only the accountant has the right to do this" },
            //            JsonRequestBehavior.AllowGet);

            var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == id);

            if (delivery == null)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu Does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Status != (byte)DeliveryStatus.New)
                return JsonCamelCaseResult(new { Status = -2, Text = "Phiếu này đã được duyệt" },
                    JsonRequestBehavior.AllowGet);

            if (!delivery.IsLast)
                return JsonCamelCaseResult(new { Status = -2, Text = "Bạn phải duyệt phiếu xuất cuối cùng của khách hàng" },
                    JsonRequestBehavior.AllowGet);

            var customer =
                await UnitOfWork.CustomerRepo.SingleOrDefaultAsNoTrackingAsync(
                    x => x.IsDelete == false && x.Id == delivery.CustomerId);

            if (customer == null)
                return JsonCamelCaseResult(new { Status = -3, Text = "Customer does not exist or has been deleted" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var rs = await PayDeliverys(delivery, customer);

                    if (rs.Status <= 0)
                    {
                        transaction.Rollback();

                        return JsonCamelCaseResult(new { Status = rs.Status * 100, Text = rs.Msg },
                            JsonRequestBehavior.AllowGet);
                    }

                    delivery.ApprovelUserId = UserState.UserId;
                    delivery.ApprovelFullName = UserState.FullName;
                    delivery.ApprovelUserUserName = UserState.UserName;
                    delivery.ApprovelOfficeId = UserState.OfficeId;
                    delivery.ApprovelOfficeName = UserState.OfficeName;
                    delivery.ApprovelOfficeIdPath = UserState.OfficeIdPath;
                    delivery.ApprovelUserTitleId = UserState.TitleId;
                    delivery.ApprovelUserTitleName = UserState.TitleName;
                    delivery.ApprovelTime = DateTime.Now;
                    delivery.Status = (byte)DeliveryStatus.Approved;

                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    var url = Url.Action("Index", "Delivery");

                    NotifyHelper.CreateAndSendNotifySystemToClient(delivery.CreatedUserId,
                        $"{UserState.FullName} Duyệt PGH-{delivery.Code}", EnumNotifyType.Warning,
                        $"{UserState.FullName} đã duyệt phiếu giao hàng" +
                        $" PGH-{delivery.Code}. <a href=\"{url}\" title=\"See details\">See more</a>",
                        $"{delivery.Code}_Approvel", url);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Duyệt thành công" },
                JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thanh toán tất cả các phiếu xuất của khách hàng... (Trừ tiền Account)
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        private async Task<ProcessRechargeBillResult> PayDeliverys(Delivery delivery, Customer customer)
        {
            //  Lấy ra các phiếu còn nợ tiền
            var deliveryPre = await UnitOfWork.DeliveryRepo.FindAsync(
                    x => x.IsDelete == false && x.CustomerId == delivery.CustomerId && x.Id <= delivery.Id &&
                        x.DebitAfter.HasValue && x.DebitAfter >= 100);

            deliveryPre = deliveryPre.OrderBy(x => x.DebitPre).ToList();

            List<Order> orders = null;

            foreach (var d in deliveryPre)
            {
                var rs = await PayDelivery(d, customer);

                if (rs.Status < 0)
                    return rs;

                if (d.Id != delivery.Id && d.Status == (byte)DeliveryStatus.New)
                {
                    d.ApprovelUserId = UserState.UserId;
                    d.ApprovelFullName = UserState.FullName;
                    d.ApprovelUserUserName = UserState.UserName;
                    d.ApprovelOfficeId = UserState.OfficeId;
                    d.ApprovelOfficeName = UserState.OfficeName;
                    d.ApprovelOfficeIdPath = UserState.OfficeIdPath;
                    d.ApprovelUserTitleId = UserState.TitleId;
                    d.ApprovelUserTitleName = UserState.TitleName;
                    d.ApprovelTime = DateTime.Now;
                    d.Status = (byte)DeliveryStatus.Approved;
                }

                // Cập nhật lịch sử package
                var packages = await UnitOfWork.DeliveryRepo.GetPackageByDeliveryId(d.Id);

                foreach (var p in packages)
                {
                    p.Status = (byte)OrderPackageStatus.GoingDelivery;

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = p.Id,
                        PackageCode = p.Code,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        Type = p.OrderType,
                        Status = (byte)OrderPackageStatus.GoingDelivery,
                        Content = $"{EnumHelper.GetEnumDescription(OrderPackageStatus.GoingDelivery)}",
                        JsonData = JsonConvert.SerializeObject(new
                        {
                            deliveryCode = d.Code,
                            warehouseId = UserState.OfficeId,
                            warehouseName = UserState.OfficeName,
                            warehouseIdPath = UserState.OfficeIdPath
                        }),
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);
                }

                var strCodeOrder = $";{string.Join(";", packages.Select(x => x.OrderCode).Distinct().ToList())};";

                orders = await UnitOfWork.OrderRepo.FindAsync(
                            x => strCodeOrder.Contains(";" + x.Code + ";") && !x.IsDelete);

                // Cập nhật trạng thái Orders đang giao hàng
                foreach (var order in orders)
                {
                    //check Orders ký gửi
                    if (order.Type == (byte)OrderType.Deposit)
                    {
                        if (order.Status < (byte)DepositStatus.GoingDelivery)
                        {
                            order.Status = (byte)DepositStatus.GoingDelivery;
                            order.LastUpdate = DateTime.Now;

                            // Thêm lịch sử cho Orders
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = DateTime.Now,
                                Content = $"Đang giao hàng",
                                CustomerId = order.CustomerId ?? 0,
                                CustomerName = order.CustomerName,
                                OrderId = order.Id,
                                Status = order.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = order.Type
                            });

                            UnitOfWork.OrderHistoryRepo.Save();
                        }
                    }
                    else
                    {
                        if (order.Status < (byte)OrderStatus.GoingDelivery)
                        {
                            order.Status = (byte)OrderStatus.GoingDelivery;
                            order.LastUpdate = DateTime.Now;

                            // Thêm lịch sử cho Orders
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = DateTime.Now,
                                Content = $"Đang giao hàng",
                                CustomerId = order.CustomerId ?? 0,
                                CustomerName = order.CustomerName,
                                OrderId = order.Id,
                                Status = order.Status,
                                UserId = UserState.UserId,
                                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                                Type = order.Type
                            });
                            UnitOfWork.OrderHistoryRepo.Save();
                        }
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();
                }
            }

            await UnitOfWork.DeliveryRepo.SaveAsync();

            if (orders != null)
            {
                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                // Cập nhật công nợ
                BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));
            }

            return new ProcessRechargeBillResult() { Status = 1, Msg = "Thanh toán thành công" };
        }

        /// <summary>
        /// Thanh toán nhiều phiếu xuất
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        private async Task<ProcessRechargeBillResult> PayDelivery(Delivery delivery, Customer customer)
        {
            var deliveryDetails =
                await UnitOfWork.DeliveryDetailRepo.FindAsync(x => x.IsDelete == false && x.DeliveryId == delivery.Id);

            decimal paymentDelivery = 0;

            var lisOrderId = deliveryDetails.Select(x => x.OrderId).Distinct().ToList();

            foreach (var orderId in lisOrderId)
            {
                var order =
                        await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.IsDelete == false && x.Id == orderId);

                var total = deliveryDetails.FirstOrDefault(x => x.OrderId == orderId)?.Debit ?? 0;

                var payment = customer.BalanceAvalible > total ? total : customer.BalanceAvalible;

                if (payment > 0)
                {
                    paymentDelivery += payment;

                    // Trừ tiền tài khoản khách
                    var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                    {
                        CustomerId = customer.Id,
                        CurrencyFluctuations = payment,
                        OrderId = orderId,
                        Note = $"Thanh toán đơn trong phiếu giao hàng: D{delivery.Code}",
                        TreasureIdd = (int)TreasureCustomerWallet.Delivery
                    });

                    // Lỗi trong quá tình thực hiện thanh toán
                    if (processRechargeBillResult.Status < 0)
                    {
                        return processRechargeBillResult;
                    }

                    order.TotalPayed += payment;
                    order.Debt -= payment;

                    customer.BalanceAvalible -= payment;

                    // Thêm giao dịch trừ tiền
                    UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                    {
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Currency = Currency.VND.ToString(),
                        ExchangeRate = 0,
                        IsDelete = false,
                        Type = (byte)OrderExchangeType.Pay,
                        Mode = (byte)OrderExchangeMode.Export,
                        ModeName = OrderExchangeType.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                        Note = $"Thánh toán tiền trong phiếu xuất D{delivery.Code}",
                        OrderId = orderId,
                        TotalPrice = payment,
                        Status = (byte)OrderExchangeStatus.Approved
                    });

                    // Cập nhật lại tiền trong Detail phiếu
                    foreach (var d in deliveryDetails.Where(x => x.OrderId == orderId))
                    {
                        if (payment > d.Debit)
                        {
                            d.PricePayed += d.Debit.Value;
                            d.Debit = 0;
                            payment -= d.Debit.Value;
                        }
                        else
                        {
                            d.PricePayed += payment;
                            d.Debit -= payment;
                        }
                    }
                }
            }

            // Cập nhật lịch sử thanh toán tiền 
            await UnitOfWork.DeliveryDetailRepo.SaveAsync();

            // Cập nhật lại nợ kỳ trước
            var lastDelivery = UnitOfWork.DeliveryRepo.Entities
                .Where(x => x.IsDelete == false && x.CustomerId == customer.Id && x.Id < delivery.Id)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            var debitPre = lastDelivery == null || lastDelivery.DebitAfter == null ? 0 : lastDelivery.DebitAfter;

            delivery.Receivable -= delivery.DebitPre;
            delivery.DebitAfter -= delivery.DebitPre;
            delivery.DebitPre = debitPre;
            delivery.Receivable += delivery.DebitPre;
            delivery.DebitAfter += delivery.DebitPre;

            // Cập nhật lại nợ cuối của phiếu
            delivery.DebitAfter -= paymentDelivery;
            delivery.PricePayed += paymentDelivery;

            if (delivery.Debit < paymentDelivery)
            {
                delivery.Debit = 0;
            }
            else
            {
                delivery.Debit -= paymentDelivery;
            }

            delivery.Receivable -= paymentDelivery;

            await UnitOfWork.DeliveryDetailRepo.SaveAsync();

            return new ProcessRechargeBillResult() { Status = 1, Msg = "Thanh toán thành công" };
        }

        [CheckPermission(EnumAction.Add, EnumPage.Delivery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignToShipper(AssignToShipper model)
        {
            var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(x => x.Id == model.DeliveryId);

            if (delivery.Status == (byte)DeliveryStatus.New)
                return JsonCamelCaseResult(new { Status = -2, Text = "Không thể gán Shipper cho phiếu chưa được duyệt!" },
                    JsonRequestBehavior.AllowGet);

            if (delivery.Status == (byte)DeliveryStatus.Complete || delivery.Status == (byte)DeliveryStatus.Cancel)
                return JsonCamelCaseResult(new { Status = -2, Text = "Không thể gán Shipper đã hoàn thành hoặc hủy!" },
                    JsonRequestBehavior.AllowGet);

            if (model.UserId == null)
            {
                delivery.ShipperFullName = null;
                delivery.ShipperUserId = null;
                delivery.ShipperUserUserName = null;
                delivery.ShipperOfficeId = null;
                delivery.ShipperOfficeIdPath = null;
                delivery.ShipperOfficeName = null;
                delivery.ShipperUserTitleName = null;
                delivery.ShipperUserTitleId = null;
                delivery.ShipperTime = DateTime.Now;
            }
            else
            {
                var user = UnitOfWork.UserRepo.GetBySpec((u, p) => new
                {
                    u.Id,
                    u.FullName,
                    u.UserName,
                    p.OfficeId,
                    p.OfficeIdPath,
                    p.OfficeName,
                    p.TitleName,
                    p.TitleId
                },
                    u => u.IsDelete == false && u.IsSystem == false && u.IsCompany == false,
                    p => p.OfficeId == model.OfficeId && p.TitleId == model.TitleId && p.UserId == model.UserId).FirstOrDefault();

                if (user == null)
                    return JsonCamelCaseResult(new { Status = -1, Text = "Shipper này Does not exist or has been deleted!" },
                        JsonRequestBehavior.AllowGet);

                delivery.ShipperFullName = user.FullName;
                delivery.ShipperUserId = user.Id;
                delivery.ShipperUserUserName = user.UserName;
                delivery.ShipperOfficeId = user.OfficeId;
                delivery.ShipperOfficeIdPath = user.OfficeIdPath;
                delivery.ShipperOfficeName = user.OfficeName;
                delivery.ShipperUserTitleName = user.TitleName;
                delivery.ShipperUserTitleId = user.TitleId;
                delivery.ShipperTime = DateTime.Now;
            }

            await UnitOfWork.DeliveryRepo.SaveAsync();

            var url = Url.Action("Index", "Delivery");

            if (delivery.ShipperUserId != null)
            {
                NotifyHelper.CreateAndSendNotifySystemToClient(delivery.ShipperUserId.Value,
                    $"{UserState.FullName} Gán phiếu giao PGH-{delivery.Code} cho bạn", EnumNotifyType.Info,
                    $"{UserState.FullName} đã gán phiêu giao hàng" +
                    $" PGH-{delivery.Code} cho bạn. <a href=\"{url}\" title=\"Xem chi tiết\">Xem thêm</a>",
                    $"{delivery.Code}_Approvel", url);
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "Cập nhật Shipper thành công!" },
                            JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Update, EnumPage.Delivery)]
        public async Task<ActionResult> ShipperRecentSearch()
        {
            var items = await UnitOfWork.UserRepo.RecentSuggetion(UserState.UserId, RecentMode.Shipper);
            return JsonCamelCaseResult(items, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SearchShipper(string keyword, int page = 1, int pageSize = 20)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            int totalRecord;

            var items = await UnitOfWork.UserRepo.SearchShipper(keyword, page, pageSize, out totalRecord);

            return JsonCamelCaseResult(new { totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Update, EnumPage.Delivery)]
        [HttpPost]
        public async Task<ActionResult> ShipperRecentSave(int userId)
        {
            var recent = await
                    UnitOfWork.RecentRepo.SingleOrDefaultAsync(
                        x => x.RecordId == userId && x.Mode == (byte)RecentMode.Shipper && x.UserId == UserState.UserId);

            if (recent != null)
            {
                recent.CountNo += 1;
                await UnitOfWork.RecentRepo.SaveAsync();

                return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
            }

            UnitOfWork.RecentRepo.Add(new Recent()
            {
                CountNo = 1,
                Mode = (byte)RecentMode.Shipper,
                RecordId = userId,
                UserId = UserState.UserId
            });

            await UnitOfWork.RecentRepo.SaveAsync();

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }

        // Cập nhật tiền ship cho Orders
        private async Task ReductPriceShipToOrder(Dictionary<int, decimal> model)
        {
            foreach (var d in model)
            {
                var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                    x =>
                        x.OrderId == d.Key && x.IsDelete == false &&
                        x.ServiceId == (byte)OrderServices.InSideShipping);

                if (shipToHomeService != null)
                {
                    shipToHomeService.TotalPrice = shipToHomeService.TotalPrice > d.Value ? shipToHomeService.TotalPrice - d.Value : 0;
                    shipToHomeService.Value = shipToHomeService.Value > d.Value ? shipToHomeService.Value - d.Value : 0;
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

        [CheckPermission(EnumAction.Delete, EnumPage.Delivery)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int deliveryId)
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var delivery = await UnitOfWork.DeliveryRepo.SingleOrDefaultAsync(
                        x => x.Id == deliveryId && x.Status == (byte) DeliveryStatus.New && x.IsDelete == false);

                    if (delivery == null)
                        return JsonCamelCaseResult(
                                new { Status = -1, Text = "Phiếu giao Does not exist or has been deleted" },
                                JsonRequestBehavior.AllowGet);

                    delivery.IsDelete = true;
                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    var dicOrderPriceShip = UnitOfWork.DeliveryDetailRepo.Find(
                            x => x.IsDelete == false && x.DeliveryId == delivery.Id)
                        .GroupBy(x => x.OrderId)
                        .ToDictionary(x => x.Key, x => x.Sum(y => y.PriceShip));

                    // Trừ tiền ship trong các Orders của phiếu xuất này
                    await ReductPriceShipToOrder(dicOrderPriceShip);
                    await UnitOfWork.DeliveryRepo.SaveAsync();

                    // Cập nhật lại IsLast cho phiếu giao trước của khách hàng
                    if (delivery.IsLast)
                    {
                        // Cập nhật lại IsLast cho phiếu trước
                        var lastDelivery =
                            UnitOfWork.DeliveryRepo.Find(
                                    x => x.IsDelete == false && x.CustomerId == delivery.CustomerId && x.Id != delivery.Id && x.Status == (byte)DeliveryStatus.New)
                                .OrderByDescending(x => x.Id).FirstOrDefault();

                        if (lastDelivery != null)
                        {
                            lastDelivery.IsLast = true;
                            await UnitOfWork.DeliveryRepo.SaveAsync();
                        }
                            
                    }
                    else
                    {
                        // Tính lại tiền nợ kỳ trước của các phiếu của khách hàng này
                        var deliveryPre = await UnitOfWork.DeliveryRepo.FindAsync(
                                    x => x.IsDelete == false && x.CustomerId == delivery.CustomerId &&
                                         x.DebitAfter.HasValue && (x.DebitAfter >= 100 || x.Status < (byte)DeliveryStatus.Complete));

                        deliveryPre = deliveryPre.OrderBy(x => x.DebitPre).ToList();

                        foreach (var d in deliveryPre)
                        {
                            //   Cập nhật lại nợ kỳ trước
                            var lastDelivery = UnitOfWork.DeliveryRepo.Entities
                                .Where(x => x.IsDelete == false && x.CustomerId == d.CustomerId && x.Id < d.Id)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();

                            var debitPre = lastDelivery == null || lastDelivery.DebitAfter == null ? 0 : lastDelivery.DebitAfter;

                            d.Receivable = d.Debit + debitPre;
                            d.DebitPre = debitPre;
                            d.DebitAfter = d.Debit + debitPre;

                            await UnitOfWork.DeliveryRepo.SaveAsync();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(e).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(new {Status = 1, Text = "Xóa phiếu giao thành công"}, JsonRequestBehavior.AllowGet);
        }

        // Tính lại tiền nợ kỳ trước của khách hàng
        public async Task<int> FixDelivery()
        {
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var customerIds = UnitOfWork.DeliveryRepo.Entities.Where(x => x.IsDelete == false &&
                                                                               x.DebitAfter.HasValue &&
                                                                               (x.DebitAfter >= 100 ||
                                                                                x.Status >= (byte)DeliveryStatus.Approved &&
                                                                                x.Status < (byte)DeliveryStatus.Complete))
                        .GroupBy(x => x.CustomerId)
                        .Where(grouping => grouping.Count() > 1)
                        .Select(x => x.Key)
                        .ToList();

                    foreach (var customerId in customerIds)
                    {
                        var deliveryPre = await UnitOfWork.DeliveryRepo.FindAsync(
                            x => x.IsDelete == false && x.CustomerId == customerId &&
                                 x.DebitAfter.HasValue &&
                                 (x.DebitAfter >= 100 ||
                                  x.Status >= (byte)DeliveryStatus.Approved && x.Status < (byte)DeliveryStatus.Complete));

                        deliveryPre = deliveryPre.OrderBy(x => x.DebitPre).ToList();

                        foreach (var delivery in deliveryPre)
                        {
                         //   Cập nhật lại nợ kỳ trước
                            var lastDelivery = UnitOfWork.DeliveryRepo.Entities
                                .Where(x => x.IsDelete == false && x.CustomerId == customerId && x.Id < delivery.Id)
                                .OrderByDescending(x => x.Id)
                                .FirstOrDefault();
                            
                            var debitPre = lastDelivery == null || lastDelivery.DebitAfter == null ? 0 : lastDelivery.DebitAfter;

                            delivery.Receivable = delivery.Debit + debitPre;
                            delivery.DebitPre = debitPre;
                            delivery.DebitAfter = delivery.Debit + debitPre;

                            await UnitOfWork.DeliveryRepo.SaveAsync();
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

            return 1;
        }
    }
}