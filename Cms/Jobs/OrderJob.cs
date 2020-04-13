using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Cms.Helpers;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Settings;
using Library.UnitOfWork;
using Library.ViewModels.TemplateEmailJob;

namespace Cms.Jobs
{
    public class OrderJob
    {
        /// <summary>
        /// - Cập nhập Orders là hoàn thành
        /// - Tính từ thời điểm phiếu xuất "đã nhận hàng" + 7 ngày so với thời điểm hiện tại.
	    /// - Orders không có phát sinh khiếu nại, hoặc có phát sinh khiếu nại nhưng đã hoàn thành(Hàm sẽ được cung cấp)
	    /// - Các package xuất giao = package trên Orders
        /// </summary>
        /// <param name="day">Số ngày sau khi hoàn thành phiếu giao hàng cuối cùng</param>
        public static void UpdateOrderToComplete(int day)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                using (var transaction = unitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var dateTime = DateTime.Now.AddDays(day * -1);

                        var orders = unitOfWork.OrderRepo.GetOrderToSetComplete(dateTime);

                        foreach (var order in orders)
                        {
                            // Bỏ qua hoàn thành khi Orders vẫn còn khiếu nại đang xử lý
                            var rs = unitOfWork.ComplainRepo.AutoCheckComplainResult(order.Id);

                            if (rs.Status < 0)
                                continue;

                            var timeNow = DateTime.Now;
                            if (order.Type == (byte) OrderType.Deposit)
                            {
                                order.Status = (byte)DepositStatus.Finish;

                                // Thêm lịch sử cho Orders
                                unitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                {
                                    CreateDate = timeNow,
                                    Content = "Completed",
                                    CustomerId = order.CustomerId ?? 0,
                                    CustomerName = order.CustomerName,
                                    OrderId = order.Id,
                                    Status = order.Status,
                                    UserId = 0,
                                    UserFullName = $"system",
                                    Type = order.Type
                                });
                            }
                            else
                            {
                                order.Status = (byte)OrderStatus.Finish;

                                // Thêm lịch sử cho Orders
                                unitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                                {
                                    CreateDate = timeNow,
                                    Content = "Completed",
                                    CustomerId = order.CustomerId ?? 0,
                                    CustomerName = order.CustomerName,
                                    OrderId = order.Id,
                                    Status = order.Status,
                                    UserId = 0,
                                    UserFullName = $"system",
                                    Type = order.Type
                                });
                            }
                            
                            order.LastUpdate = timeNow;

                            var customer = unitOfWork.CustomerRepo.SingleOrDefault(x => x.Id == order.CustomerId);

                            customer.Balance += order.Total;
                            var level = unitOfWork.OrderRepo.GetCustomerLevel(customer.Balance);

                            customer.LevelId = (byte)level.Id;
                            customer.LevelName =level.Name;

                            unitOfWork.OrderRepo.Save();
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
            }
        }

        /// <summary>
        /// Cảnh báo thông tin cho đặt hàng
        /// </summary>
        /// <param name="day"></param>
        public static void OrderWarningByEmail(int day)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var userIds = new List<int>();
                var userIdsDisposit = new List<int>();

                // Orders quá ngày Not entered mã vận đơn
                var orderImportOvertimes = unitOfWork.OrderRepo.OrderNoCodeOfLadingOverDays(day);
                foreach (var o in orderImportOvertimes)
                {
                    if (o.Type == (byte) OrderType.Order)
                    {
                        if (o.UserId == null || userIds.Any(x => x == o.UserId))
                            continue;

                        userIds.Add(o.UserId.Value);
                    }
                    else
                    {
                        if (o.UserId == null || userIdsDisposit.Any(x => x == o.UserId))
                            continue;

                        userIdsDisposit.Add(o.UserId.Value);
                    }
                }

                // Orders kiện chưa đủ về kho
                var orderLoses = unitOfWork.OrderRepo.OrderNotEnoughInventoryOverDays(day);
                foreach (var o in orderLoses)
                {
                    if (o.Type == (byte)OrderType.Order)
                    {
                        if (o.UserId == null || userIds.Any(x => x == o.UserId))
                            continue;

                        userIds.Add(o.UserId.Value);
                    }
                    else
                    {
                        if (o.UserId == null || userIdsDisposit.Any(x => x == o.UserId))
                            continue;

                        userIdsDisposit.Add(o.UserId.Value);
                    }
                }

                // package quá ngày nhập kho
                var packageOvertimes = unitOfWork.OrderPackageRepo.OrderPackageOverDay(day);
                foreach (var o in packageOvertimes)
                {
                    if (o.OrderType == (byte)OrderType.Order)
                    {
                        if (userIds.Any(x => x == o.UserId))
                            continue;

                        userIds.Add(o.UserId);
                    }
                    else
                    {
                        if (userIdsDisposit.Any(x => x == o.UserId))
                            continue;

                        userIdsDisposit.Add(o.UserId);
                    }
                }

                // Phòng đặt hàng
                var notifyOrder = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Order}");
                // Thông báo tới tất cả đặt hàng khi nv trong cấu hình chỉ là theo dõi hoặc không có nhân viên trong cấu hình
                if (notifyOrder.Settings.IsFollow || !notifyOrder.Settings.Users.Any(x => x.IsNotify))
                {
                    // Gửi email đến nhân viên
                    foreach (var userId in userIds)
                    {
                        GetValue(day, orderImportOvertimes, userId, orderLoses, packageOvertimes);
                    }
                }

                // Thông báo tới nhân viên trong cấu hình
                foreach (var u in notifyOrder.Settings.Users.Where(x => x.IsNotify))
                {
                    // Lấy nv trong cấu hình thay thế đặt hàng
                    if (u.UserId != default(int) && notifyOrder.Settings.IsFollow == false)
                    {
                        GetValue(day, orderImportOvertimes, u.UserId, orderLoses, packageOvertimes);

                    }
                    else if (u.UserId != default(int) && notifyOrder.Settings.IsFollow)
                    {
                        // Nhân viên trong cấu hình k phải là nhân viên đặt hàng
                        if (userIds.All(x => x != u.UserId))
                            GetValue(day, orderImportOvertimes, u.UserId, orderLoses, packageOvertimes);
                    }
                }

                // Phòng gom công
                var notifyDeposit = new SettingProvider<NotifySetting>($"OfficeType_{(byte)OfficeType.Deposit}");
                // Thông báo tới tất cả đặt hàng khi nv trong cấu hình chỉ là theo dõi hoặc không có nhân viên trong cấu hình
                if (notifyDeposit.Settings.IsFollow || !notifyDeposit.Settings.Users.Any(x => x.IsNotify))
                {
                    // Gửi email đến nhân viên
                    foreach (var userId in userIdsDisposit)
                    {
                        GetValue(day, orderImportOvertimes, userId, orderLoses, packageOvertimes);
                    }
                }

                // Thông báo tới nhân viên trong cấu hình
                foreach (var u in notifyDeposit.Settings.Users.Where(x => x.IsNotify))
                {
                    // Lấy nv trong cấu hình thay thế đặt hàng
                    if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow == false)
                    {
                        GetValue(day, orderImportOvertimes, u.UserId, orderLoses, packageOvertimes);

                    }
                    else if (u.UserId != default(int) && notifyDeposit.Settings.IsFollow)
                    {
                        // Nhân viên trong cấu hình k phải là nhân viên đặt hàng
                        if (userIdsDisposit.All(x => x != u.UserId))
                            GetValue(day, orderImportOvertimes, u.UserId, orderLoses, packageOvertimes);
                    }
                }
            }
        }

        private static void GetValue(int day, List<OrderRiskResult> orderImportOvertimes, int userId, List<OrderRiskResult> orderLoses, List<OrderPackageOverDayResult> packageOvertimes)
        {
            var userOrderImportOvertimes = orderImportOvertimes.Where(x => x.UserId == userId).ToList();
            var orderImportOvertimeContent = userOrderImportOvertimes.Any()
                ? StringUtil.RenderPartialToString("~/Views/Shared/_OrderTransportCodeOvertime.cshtml",
                    new OverDaysOrderViewModel()
                    {
                        Day = day,
                        Orders = userOrderImportOvertimes
                    })
                : string.Empty;

            var userOrderLoses = orderLoses.Where(x => x.UserId == userId).ToList();
            var orderLoseContent = userOrderLoses.Any()
                ? StringUtil.RenderPartialToString("~/Views/Shared/_OrderPackageLose.cshtml",
                    new OverDaysOrderViewModel()
                    {
                        Day = day,
                        Orders = userOrderLoses
                    })
                : string.Empty;

            var userPackageOvertimes = packageOvertimes.Where(x => x.UserId == userId).ToList();
            var packageOvertimeContent = userPackageOvertimes.Any()
                ? StringUtil.RenderPartialToString("~/Views/Shared/_OrderImportOvertime.cshtml",
                    new OverDaysOrderViewModel1()
                    {
                        Day = day,
                        Orders = userPackageOvertimes
                    })
                : string.Empty;

            var content = orderImportOvertimeContent + orderLoseContent + packageOvertimeContent;

            if (!string.IsNullOrWhiteSpace(content))
                MessageHelper.SystemSendMessage(new List<int>() {userId},
                    $"[Cảnh báo] Orders ngày {DateTime.Now:d}", content);
        }

        public static void ProcessDebitReport(string orderIds1)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var orders = unitOfWork.OrderRepo.FindAsNoTracking(
               x =>
                   x.IsDelete == false &&
                   (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.WaitOrder &&
                    x.Status <= (byte)OrderStatus.GoingDelivery ||
                    x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                    x.Status <= (byte)DepositStatus.GoingDelivery) && orderIds1.Contains(";" + x.Id + ";")).ToList();

                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                var packages = unitOfWork.OrderPackageRepo.FindAsNoTracking(
                    x =>
                        x.IsDelete == false && orderIds.Contains(";" + x.OrderId + ";") &&
                        x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
                        x.Status <= (byte)OrderPackageStatus.Completed).ToList();

                var orderServices = unitOfWork.OrderServiceRepo.FindAsNoTracking(
                        x => x.IsDelete == false && x.Checked && orderIds.Contains(";" + x.OrderId + ";")).ToList();

                var debitReports = unitOfWork.DebitReportRepo.Find(x => orderIds.Contains(";" + x.OrderId + ";")).ToList();

                var newReports = new List<DebitReport>();

                foreach (var order in orders)
                {
                    // Tiền thực thu của Orders
                    var inMoney = order.TotalPayed - order.TotalRefunded;

                    // Tiền hàng còn thiếu
                    var debitOrder =
                        debitReports.SingleOrDefault(
                            x => x.OrderId == order.Id && x.PackageId == null && x.ServiceId == 255);

                    var isAddNew = false;

                    if (debitOrder == null)
                    {
                        debitOrder = new DebitReport
                        {
                            CustomerEmail = order.CustomerEmail,
                            CustomerId = order.CustomerId ?? 0,
                            CustomerPhone = order.CustomerPhone,
                            OrderCode = order.Code,
                            OrderId = order.Id,
                            PackageId = null,
                            PackageCode = null,
                            ServiceId = 255,
                            Price = 0
                        };

                        isAddNew = true;
                    }

                    var payNow = inMoney >= order.TotalExchange
                        ? order.TotalExchange
                        : inMoney;

                    debitOrder.Price = payNow == order.TotalExchange ? 0 : order.TotalExchange - payNow;
                    inMoney -= payNow;

                    if (isAddNew)
                        newReports.Add(debitOrder);

                    //// Tiền dịch vụ mua hàng còn thiếu
                    //AddOrUpdateDebitReport(ref debitReports, ref newReports, order, null, orderServices, ref inMoney,
                    //    OrderServices.Order, unitOfWork);

                    // Tiền shop vận chuyển
                    AddOrUpdateDebitReport(ref debitReports, ref newReports, order, null, orderServices, ref inMoney,
                        OrderServices.ShopShipping, unitOfWork);

                    // Tiền dịch vụ kiểm đếm
                    AddOrUpdateDebitReport(ref debitReports, ref newReports, order, null, orderServices, ref inMoney,
                        OrderServices.Audit, unitOfWork);

                    // Tiền còn thiếu của các package trong Orders
                    var packagesInOrder = packages.Where(x => x.OrderId == order.Id)
                        .OrderByDescending(x => x.Status);

                    foreach (var package in packagesInOrder)
                    {
                        // Dịch vụ Packing
                        AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
                            OrderServices.Packing, unitOfWork);

                        // Phí phát sinh
                        AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
                            OrderServices.Other, unitOfWork);

                        // Dịch vụ VC TQ-> VN
                        AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
                            OrderServices.OutSideShipping, unitOfWork);

                        // Ship tại VN
                        AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
                            OrderServices.InSideShipping, unitOfWork);

                        // Lưu kho
                        AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
                            OrderServices.Storaged, unitOfWork);
                    }
                }

                unitOfWork.DebitReportRepo.AddRange(newReports);
                unitOfWork.DebitReportRepo.Save();
            }
        }

        private static void AddOrUpdateDebitReport(ref List<DebitReport> debitReports, ref List<DebitReport> newReports,
            Order order, OrderPackage package, List<OrderService> orderServices, ref decimal inMoney, OrderServices service, UnitOfWork unitOfWork)
        {
            // Phí dịch vụ của Orders
            var serviceOrder = orderServices.SingleOrDefault(
                x => x.OrderId == order.Id && x.ServiceId == (byte)service);

            var debitReport = debitReports.SingleOrDefault(
                x => x.OrderId == order.Id &&
                     (package == null && x.PackageId == null || package != null && x.PackageId == package.Id) &&
                     x.ServiceId == (byte)service);

            // Không có dịch vụ này trong Orders
            if (serviceOrder == null && debitReport != null)
            {
                unitOfWork.DebitReportRepo.Remove(debitReport);
                return;
            }

            var isAddNew = false;

            if (debitReport == null)
            {
                debitReport = new DebitReport
                {
                    CustomerEmail = order.CustomerEmail,
                    CustomerId = order.CustomerId ?? 0,
                    CustomerPhone = order.CustomerPhone,
                    OrderCode = order.Code,
                    OrderId = order.Id,
                    ServiceId = (byte)service,
                    Price = 0
                };

                if (package != null)
                {
                    debitReport.PackageId = package.Id;
                    debitReport.PackageCode = package.Code;
                }

                isAddNew = true;
            }

            var serviceOrderPrice = serviceOrder?.TotalPrice ?? 0;

            if (package != null)
                if (service == OrderServices.Other) // Phí phát sinh cảu package
                {
                    serviceOrderPrice = package.OtherService ?? 0;
                }
                else if (service == OrderServices.InSideShipping) // Tiền Ship của package
                {
                    if (package.Status < (byte)OrderPackageStatus.WaitDelivery)
                        return;

                    var deliveryDetail = unitOfWork.DeliveryDetailRepo.SingleOrDefaultAsNoTracking(package.Id);
                    serviceOrderPrice = deliveryDetail?.PriceShip ?? 0;
                }
                else if (service == OrderServices.Storaged) // Tiền lưu kho của package
                {
                    if (package.Status < (byte)OrderPackageStatus.WaitDelivery)
                        return;

                    var deliveryDetail = unitOfWork.DeliveryDetailRepo.SingleOrDefaultAsNoTracking(package.Id);
                    serviceOrderPrice = deliveryDetail?.PriceStored ?? 0;
                }
                else // Các phí khác của package
                {
                    serviceOrderPrice = serviceOrderPrice * (package.WeightActualPercent ?? 0) / 100;
                }

            var payNow = inMoney >= serviceOrderPrice
                ? serviceOrderPrice
                : inMoney;

            debitReport.Price = payNow == serviceOrderPrice ? 0 : serviceOrderPrice - payNow;
            inMoney -= payNow;

            if (isAddNew)
                newReports.Add(debitReport);
        }


        ///// <summary>
        ///// Chưa đủ kiện về kho
        ///// </summary>
        ///// <param name="day"></param>
        //public static void OrderNotEnoughInventoryOverDays(int day)
        //{
        //    using (var unitOfWork = new Library.UnitOfWork.UnitOfWork())
        //    {
        //        var results = unitOfWork.OrderRepo.OrderNotEnoughInventoryOverDays(day);

        //        if (!results.Any())
        //            return;

        //        var orders = results.Select(x => x.Id);

        //        foreach (var orderId in orders)
        //        {
        //            var info = results.First(x => x.Id == orderId);

        //            if (info.UserId != null)
        //                NotifyHelper.CreateAndSendNotifySystemToClient(info.UserId.Value, $"#ORD{info.Code} quá {day} ngày chưa đủ kiện về kho",
        //                    EnumNotifyType.Warning, $"Các package: {info.PackageCodes} của Orders #ORD{info.Code} quá {day} ngày chưa có mã vận đơn");
        //        }
        //    }
        //}
    }
}