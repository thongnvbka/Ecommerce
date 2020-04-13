using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using Cms.Jobs;
using Common.Helper;
using Hangfire;

namespace Cms.Controllers
{
    [Authorize]
    public class DebitReportController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.DebitReport)]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetReport()
        {
            // Tiền dv dự kiến thu của Orders
            Expression<Func<Order, bool>> featureOrderQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                 x.Status < (byte)OrderStatus.Pending ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                 x.Status < (byte)DepositStatus.Pending);

            // Tiền dv cần thu của Orders
            Expression<Func<Order, bool>> requiredOrderQuery = x =>
                x.IsDelete == false && (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.Pending &&
                                        x.Status <= (byte)OrderStatus.GoingDelivery ||
                                        x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.Pending &&
                                        x.Status <= (byte)DepositStatus.GoingDelivery);

            // Tiền dv dự kiến thu của package
            Expression<Func<OrderPackage, bool>> featurePackageQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
                x.Status < (byte)OrderPackageStatus.Received;

            // Tiền dv cần thu của package
            Expression<Func<OrderPackage, bool>> requiredPackageQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.Received &&
                x.Status <= (byte)OrderPackageStatus.GoingDelivery;

            // Total money Deposit Orders
            var priceDeposit = UnitOfWork.OrderRepo.Find(
                    x => x.IsDelete == false && x.Type != (byte)OrderType.Deposit &&
                         x.Status >= (byte)OrderStatus.WaitOrder && x.Status < (byte)OrderStatus.OrderSuccess)
                .Sum(x => x.TotalPayed);

            // Total money hoàn trả Orders
            var priceRefund = Math.Abs(UnitOfWork.OrderRepo.Find(
                    x => x.IsDelete == false && (x.Type != (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)OrderStatus.OrderSuccess &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                                                 x.Type == (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)DepositStatus.InWarehouse &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery)
                         && x.Debt < 0)
                .Sum(x => x.Debt));

            // Tiền dịch vụ dự kiến thu của Orders
            var priceFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(featureOrderQuery);

            // Tiền dịch vụ phải thu của Orders
            var priceRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(requiredOrderQuery);

            // Tiền dịch vụ dự kiến thu của package
            var priceFeaturePackage = await UnitOfWork.DebitReportRepo.GetByPackageQuery(featurePackageQuery);

            // Tiền dịch vụ phải thu của package
            var priceRequiredPackage = await UnitOfWork.DebitReportRepo.GetByPackageQuery(requiredPackageQuery);

            var priceFeature = priceFeatureOrder.Sum(x => x.Value) + priceFeaturePackage.Sum(x => x.Value);
            var priceRequired = priceRequiredOrder.Sum(x => x.Value) + priceRequiredPackage.Sum(x => x.Value);

            // Gộp tiền dự kiến thu từ đơn vào kiện
            foreach (var p in priceFeaturePackage)
            {
                priceFeatureOrder.Add(p.Key, p.Value);
            }

            // Gộp tiền phải thu từ đơn vào kiện
            foreach (var p in priceRequiredPackage)
            {
                priceRequiredOrder.Add(p.Key, p.Value);
            }

            var services = new List<dynamic>()
            {
                new { ServiceId = 255, ServiceName = "Cost of goods" },
                new { ServiceId = (byte)OrderServices.ShopShipping, ServiceName = "Transport fee by shop (suppliers)" },
                //new { ServiceId = (byte)OrderServices.Order, ServiceName = "Phí mua hàng" },
                new { ServiceId = (byte)OrderServices.Audit, ServiceName = "Counting fee" },
                new { ServiceId = (byte)OrderServices.Other, ServiceName = "Incurred expenses" },
                new { ServiceId = (byte)OrderServices.OutSideShipping, ServiceName = "Warehouse transfer fee" },
                new { ServiceId = (byte)OrderServices.Packing, ServiceName = "Packaging fee" },
                new { ServiceId = (byte)OrderServices.InSideShipping, ServiceName = "Shipping fee " },
                new { ServiceId = (byte)OrderServices.Storaged, ServiceName = "Warehouse storage fee" },
            };

            var packageStatus = Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int) v)})
                .ToDictionary(x=> x.Id, x=> x.Name);

            return JsonCamelCaseResult(
                new
                {
                    packageStatus,
                    services,
                    priceDeposit,
                    priceRefund,
                    priceFeatureOrder,
                    priceRequiredOrder,
                    priceFeature,
                    priceRequired
                }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Detail(byte type, int serviceId, int currentPage = 1, int recordPerPage = 20)
        {
            Expression<Func<DebitReport, bool>> reportQuery;
            List<OrderResult> orderReports;

            // Dự kiến thu kiện
            Dictionary<int, List<DebitReportPackageResult>> debitFeaturePackages;
            // Dự kiến thu đơn
            Dictionary<int, List<DebitReport>> debitFeatureOrder;

            // Phải thu kiện
            Dictionary<int, List<DebitReportPackageResult>> debitRequiredPackages;
            // Phải thu đơn
            Dictionary<int, List<DebitReport>> debitRequiredOrder;

            string orderIds;
            long totalRecord;

            // Query Orders dự kiến thu
            Expression<Func<Order, bool>> orderFeatureQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                 x.Status < (byte)OrderStatus.Pending ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                 x.Status < (byte)DepositStatus.Pending);

            // Query package dự kiến thu
            Expression<Func<OrderPackage, bool>> packageFeatureQuery = x =>
               x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
               x.Status < (byte)OrderPackageStatus.Received;

            // Query Orders phải thu
            Expression<Func<Order, bool>> orderRequiredQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.Pending &&
                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.Pending &&
                 x.Status <= (byte)DepositStatus.GoingDelivery);

            // query package phải thu
            Expression<Func<OrderPackage, bool>> packageRequiredQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.Received &&
                x.Status <= (byte)OrderPackageStatus.GoingDelivery;

            // Total money các dịch vụ đơn lẻ
            if (type == 0 && serviceId != -1)
            {
                reportQuery = x => x.ServiceId == serviceId && x.Price > 0;

                orderReports = await UnitOfWork.DebitReportRepo.GetByOrderAll(reportQuery, x =>
                        x.IsDelete == false &&
                        (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                         x.Status <= (byte)OrderStatus.GoingDelivery ||
                         x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                         x.Status <= (byte)DepositStatus.GoingDelivery), currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                // Lấy theo đơn (Tiền hàng thiếu, tiền mua hàng, tiền kiểm đếm)
                if (serviceId == 255 || /*serviceId == (byte)OrderServices.Order ||*/ serviceId == (byte)OrderServices.ShopShipping || serviceId == (byte)OrderServices.Audit)
                {
                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery,
                        orderIds);
                    debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery,
                        orderRequiredQuery, orderIds);

                    return JsonCamelCaseResult(
                        new { orderReports, debitFeatureOrder, debitRequiredOrder, totalRecord },
                        JsonRequestBehavior.AllowGet);
                }

                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageFeatureQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageRequiredQuery, orderIds);

                return JsonCamelCaseResult(new { orderReports, debitRequiredPackages, debitFeaturePackages, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            if (type != 0 && serviceId == -1) // Total money: Dự kiến thu, Phải thu, Phải trả
            {
                reportQuery = x => x.Price > 0;

                // Dự kiến thu
                if (type == 1)
                {
                    orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderFeatureQuery, currentPage,
                    recordPerPage, out totalRecord);

                    orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);
                    debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageFeatureQuery, orderIds);

                    return JsonCamelCaseResult(new { orderReports, debitFeatureOrder, debitFeaturePackages, totalRecord }, JsonRequestBehavior.AllowGet);
                }

                // Phải thu
                orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderRequiredQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageRequiredQuery, orderIds);

                return JsonCamelCaseResult(new { orderReports, debitRequiredOrder, debitRequiredPackages, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            if (type == 0 && serviceId == -1) // Sum thu
            {
                reportQuery = x => x.Price > 0;

                orderReports = await UnitOfWork.DebitReportRepo.GetByOrderAll(reportQuery, x =>
                        x.IsDelete == false &&
                        (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                         x.Status <= (byte)OrderStatus.GoingDelivery ||
                         x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                         x.Status <= (byte)DepositStatus.GoingDelivery), currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageRequiredQuery, orderIds);
                debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);
                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageFeatureQuery, orderIds);

                return JsonCamelCaseResult(new { orderReports, debitRequiredOrder, debitRequiredPackages, debitFeatureOrder, debitFeaturePackages, totalRecord }, JsonRequestBehavior.AllowGet);

            }

            // Excess money phải trả của Orders
            List<Order> orders;
            if (serviceId == 254)
            {
                // Total money hoàn trả Orders
                orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                    x => x.IsDelete == false && (x.Type != (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)OrderStatus.OrderSuccess &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                                                 x.Type == (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)DepositStatus.InWarehouse &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery) && x.Debt < 0,
                    o => o.OrderBy(y => y.Id), currentPage, recordPerPage);

                return JsonCamelCaseResult(new { orders, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            if (serviceId == 253) // Tiền Deposit Orders
            {
                // Total money Deposit Orders
                orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                    x => x.IsDelete == false && x.Type != (byte)OrderType.Deposit &&
                         x.Status >= (byte)OrderStatus.WaitOrder && x.Status < (byte)OrderStatus.OrderSuccess && x.TotalPayed > 0,
                    o => o.OrderBy(y => y.Id), currentPage, recordPerPage);

                return JsonCamelCaseResult(new { orders, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            reportQuery = x => x.ServiceId == serviceId && x.Price > 0;

            // Lấy theo đơn (Tiền hàng thiếu, tiền mua hàng, tiền kiểm đếm)
            if (serviceId == 255 || /*serviceId == (byte)OrderServices.Order ||*/ serviceId == (byte)OrderServices.ShopShipping || serviceId == (byte)OrderServices.Audit)
            {
                // Dự kiến thu

                if (type == 1)
                {
                    orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderFeatureQuery, currentPage,
                        recordPerPage, out totalRecord);

                    orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);

                    return JsonCamelCaseResult(new { orderReports, debitFeatureOrder, totalRecord }, JsonRequestBehavior.AllowGet);
                }

                // Phải thu
                orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderRequiredQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);

                return JsonCamelCaseResult(new { orderReports, debitRequiredOrder, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            // Lấy theo package
            // Dự kiến thu
            if (type == 1)
            {
                orderReports = await UnitOfWork.DebitReportRepo.GetByPackage(reportQuery, packageFeatureQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageFeatureQuery, orderIds);

                return JsonCamelCaseResult(new { orderReports, debitFeaturePackages, totalRecord }, JsonRequestBehavior.AllowGet);
            }

            // Phải thu
            orderReports = await UnitOfWork.DebitReportRepo.GetByPackage(reportQuery, packageRequiredQuery, currentPage,
                recordPerPage, out totalRecord);

            orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

            debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery2(reportQuery, packageRequiredQuery, orderIds);

            return JsonCamelCaseResult(new { orderReports, debitRequiredPackages, totalRecord }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Fix()
        {
            long totalRecord;
            var recordPerPage = 100;
            var currentPage = 1;

            var orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                x =>
                    x.IsDelete == false && (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.WaitOrder &&
                     x.Status <= (byte)OrderStatus.GoingDelivery ||
                     x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                     x.Status <= (byte)DepositStatus.GoingDelivery), x=> x.OrderByDescending(y=> y.Id), currentPage, recordPerPage);

            var totalPage = Math.Ceiling((decimal)totalRecord / recordPerPage);

            for (currentPage = 1; currentPage <= totalPage; currentPage++)
            {
                if (currentPage != 1)
                {
                    orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                        x =>
                            x.IsDelete == false &&
                            (x.Type != (byte) OrderType.Deposit && x.Status >= (byte) OrderStatus.WaitOrder &&
                             x.Status <= (byte) OrderStatus.GoingDelivery ||
                             x.Type == (byte) OrderType.Deposit && x.Status >= (byte) DepositStatus.InWarehouse &&
                             x.Status <= (byte) DepositStatus.GoingDelivery), x => x.OrderByDescending(y => y.Id),
                        currentPage, recordPerPage);
                }

                var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

                BackgroundJob.Enqueue(() => OrderJob.ProcessDebitReport(orderIds));
            }

           //var orderIds = $";{string.Join(";", orders.Select(x => x.Id).ToList())};";

           // var packages = await UnitOfWork.OrderPackageRepo.FindAsNoTrackingAsync(
           //     x =>
           //         x.IsDelete == false && orderIds.Contains(";" + x.OrderId + ";") &&
           //         x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
           //         x.Status <= (byte)OrderPackageStatus.Completed);

           // var orderServices =
           //     await UnitOfWork.OrderServiceRepo.FindAsNoTrackingAsync(
           //         x => x.IsDelete == false && x.Checked && orderIds.Contains(";" + x.OrderId + ";"));

           // var debitReports = await UnitOfWork.DebitReportRepo.FindAsync(x => orderIds.Contains(";" + x.OrderId + ";"));

           // var newReports = new List<DebitReport>();

           // foreach (var order in orders)
           // {
           //     // Tiền thực thu của Orders
           //     var inMoney = order.TotalPayed - order.TotalRefunded;

           //     // Tiền hàng còn thiếu
           //     var debitOrder =
           //         debitReports.SingleOrDefault(
           //             x => x.OrderId == order.Id && x.PackageId == null && x.ServiceId == 255);

           //     var isAddNew = false;

           //     if (debitOrder == null)
           //     {
           //         debitOrder = new DebitReport
           //         {
           //             CustomerEmail = order.CustomerEmail,
           //             CustomerId = order.CustomerId ?? 0,
           //             CustomerPhone = order.CustomerPhone,
           //             OrderCode = order.Code,
           //             OrderId = order.Id,
           //             PackageId = null,
           //             PackageCode = null,
           //             ServiceId = 255,
           //             Price = 0
           //         };

           //         isAddNew = true;
           //     }

           //     var payNow = inMoney >= order.TotalExchange
           //         ? order.TotalExchange
           //         : inMoney;

           //     debitOrder.Price = payNow == order.TotalExchange ? 0 : order.TotalExchange - payNow;
           //     inMoney -= payNow;

           //     if (isAddNew)
           //         newReports.Add(debitOrder);

           //     // Tiền dịch vụ mua hàng còn thiếu
           //     AddOrUpdateDebitReport(ref debitReports, ref newReports, order, null, orderServices, ref inMoney,
           //         OrderServices.Order);

           //     // Tiền dịch vụ kiểm đếm
           //     AddOrUpdateDebitReport(ref debitReports, ref newReports, order, null, orderServices, ref inMoney,
           //         OrderServices.Audit);

           //     // Tiền còn thiếu của các package trong Orders
           //     var packagesInOrder = packages.Where(x => x.OrderId == order.Id)
           //         .OrderByDescending(x => x.Status);

           //     foreach (var package in packagesInOrder)
           //     {
           //         // Dịch vụ Packing
           //         AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
           //             OrderServices.Packing);

           //         // Incurred expenses
           //         AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
           //             OrderServices.Other);

           //         // Dịch vụ VC TQ-> VN
           //         AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
           //             OrderServices.OutSideShipping);

           //         // Ship tại VN
           //         AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
           //             OrderServices.InSideShipping);

           //         // Lưu kho
           //         AddOrUpdateDebitReport(ref debitReports, ref newReports, order, package, orderServices, ref inMoney,
           //             OrderServices.Storaged);
           //     }
           // }

           // UnitOfWork.DebitReportRepo.AddRange(newReports);
           // await UnitOfWork.DebitReportRepo.SaveAsync();

            return JsonCamelCaseResult(1, JsonRequestBehavior.AllowGet);
        }

        private void AddOrUpdateDebitReport(ref List<DebitReport> debitReports, ref List<DebitReport> newReports,
            Order order,
            OrderPackage package, List<OrderService> orderServices, ref decimal inMoney, OrderServices service)
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
                UnitOfWork.DebitReportRepo.Remove(debitReport);
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
                if (service == OrderServices.Other) // Incurred expenses cảu kiện hàng
                {
                    serviceOrderPrice = package.OtherService ?? 0;
                }
                else if (service == OrderServices.InSideShipping) // Tiền Ship của package
                {
                    if (package.Status < (byte)OrderPackageStatus.WaitDelivery)
                        return;

                    var deliveryDetail = UnitOfWork.DeliveryDetailRepo.SingleOrDefaultAsNoTracking(package.Id);
                    serviceOrderPrice = deliveryDetail?.PriceShip ?? 0;
                }
                else if (service == OrderServices.Storaged) // Tiền lưu kho của package
                {
                    if (package.Status < (byte)OrderPackageStatus.WaitDelivery)
                        return;

                    var deliveryDetail = UnitOfWork.DeliveryDetailRepo.SingleOrDefaultAsNoTracking(package.Id);
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
        //Xuất File Excel danh sách thu/ chi
        [HttpPost]
        public async Task<ActionResult> ExcelDebit()
        {
            // Tiền dv dự kiến thu của Orders
            Expression<Func<Order, bool>> featureOrderQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                 x.Status < (byte)OrderStatus.Pending ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                 x.Status < (byte)DepositStatus.Pending);

            // Tiền dv cần thu của Orders
            Expression<Func<Order, bool>> requiredOrderQuery = x =>
                x.IsDelete == false && (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.Pending &&
                                        x.Status <= (byte)OrderStatus.GoingDelivery ||
                                        x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.Pending &&
                                        x.Status <= (byte)DepositStatus.GoingDelivery);

            // Tiền dv dự kiến thu của package
            Expression<Func<OrderPackage, bool>> featurePackageQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
                x.Status < (byte)OrderPackageStatus.Received;

            // Tiền dv cần thu của package
            Expression<Func<OrderPackage, bool>> requiredPackageQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.Received &&
                x.Status <= (byte)OrderPackageStatus.GoingDelivery;

            // Total money Deposit Orders
            var priceDeposit = UnitOfWork.OrderRepo.Find(
                    x => x.IsDelete == false && x.Type != (byte)OrderType.Deposit &&
                         x.Status >= (byte)OrderStatus.WaitOrder && x.Status < (byte)OrderStatus.OrderSuccess)
                .Sum(x => x.TotalPayed);

            // Total money hoàn trả Orders
            var priceRefund = Math.Abs(UnitOfWork.OrderRepo.Find(
                    x => x.IsDelete == false && (x.Type != (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)OrderStatus.OrderSuccess &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                                                 x.Type == (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)DepositStatus.InWarehouse &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery)
                         && x.Debt < 0)
                .Sum(x => x.Debt));

            // Tiền dịch vụ dự kiến thu của Orders
            var priceFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(featureOrderQuery);

            // Tiền dịch vụ phải thu của Orders
            var priceRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(requiredOrderQuery);

            // Tiền dịch vụ dự kiến thu của package
            var priceFeaturePackage = await UnitOfWork.DebitReportRepo.GetByPackageQuery(featurePackageQuery);

            // Tiền dịch vụ phải thu của package
            var priceRequiredPackage = await UnitOfWork.DebitReportRepo.GetByPackageQuery(requiredPackageQuery);

            var priceFeature = priceFeatureOrder.Sum(x => x.Value) + priceFeaturePackage.Sum(x => x.Value);
            var priceRequired = priceRequiredOrder.Sum(x => x.Value) + priceRequiredPackage.Sum(x => x.Value);

            // Gộp tiền dự kiến thu từ đơn vào kiện
            foreach (var p in priceFeaturePackage)
            {
                priceFeatureOrder.Add(p.Key, p.Value);
            }

            // Gộp tiền phải thu từ đơn vào kiện
            foreach (var p in priceRequiredPackage)
            {
                priceRequiredOrder.Add(p.Key, p.Value);
            }
            //Lấy về danh sách dịch vụ
            var services = new List<dynamic>()
            {
                new { ServiceId = 255, ServiceName = "Cost of goods" },
                new { ServiceId = (byte)OrderServices.ShopShipping, ServiceName = "Transport fee by shop (suppliers)" },
                //new { ServiceId = (byte)OrderServices.Order, ServiceName = "Phí mua hàng" },
                new { ServiceId = (byte)OrderServices.Audit, ServiceName = "Counting fee" },
                new { ServiceId = (byte)OrderServices.Other, ServiceName = "Incurred expenses" },
                new { ServiceId = (byte)OrderServices.OutSideShipping, ServiceName = "Warehouse transfer fee" },
                new { ServiceId = (byte)OrderServices.Packing, ServiceName = "Packaging fee" },
                new { ServiceId = (byte)OrderServices.InSideShipping, ServiceName = "Shipping fee " },
                new { ServiceId = (byte)OrderServices.Storaged, ServiceName = "Warehouse storage fee" },
            };
            var listType = new List<dynamic>()
            {
                new { Id = 0, Name = "Estimated revenue (Baht):" },
                new { Id = 1, Name = "Receivable (Baht):" },
                new { Id = 2, Name = "Total revenue (Baht):" },
                new { Id = 3, Name = "Payable (Baht):" },

            };

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col++, "", ExcelHorizontalAlignment.Center, true, colorHeader);
                foreach (var service in services)
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col++, service.ServiceName.ToUpper(), ExcelHorizontalAlignment.Center, true, colorHeader);
                }

                ExcelHelper.CreateHeaderTable(sheet, row, col++, "DEPOSIT", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CHANGE DUE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "TOTAL", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "Estimated revenue - Receivable - Payable".ToUpper(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                #endregion

                var customStyle = new CustomExcelStyle
                {
                    IsMerge = false,
                    IsBold = false,
                    Border = ExcelBorderStyle.Thin,
                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                    NumberFormat = "#,##0"
                };

                var no = row + 1;

                foreach (var str in listType)
                {
                    col = 1;
                    ExcelHelper.CreateCellTable(sheet, no, col, str.Name, ExcelHorizontalAlignment.Right, true);
                    col++;
                    if (str.Id == 3)
                    {
                        foreach (var service in services)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col++, "");
                        }

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceDeposit, customStyle);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceRefund, customStyle);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, priceDeposit + priceRefund, customStyle);

                    }
                    else if (str.Id == 0)
                    {
                        foreach (var service in services)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceFeatureOrder.ContainsKey((byte)service.ServiceId) ? priceFeatureOrder[(byte)service.ServiceId] : 0, customStyle);
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceFeature, customStyle);
                    }
                    else if (str.Id == 1)
                    {
                        foreach (var service in services)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceRequiredOrder.ContainsKey((byte)service.ServiceId) ? priceRequiredOrder[(byte)service.ServiceId] : 0, customStyle);
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceRequired, customStyle);
                    }
                    else
                    {

                        foreach (var service in services)
                        {
                            var value = (priceFeatureOrder.ContainsKey((byte)service.ServiceId) ? priceFeatureOrder[(byte)service.ServiceId] : 0) + (priceRequiredOrder.ContainsKey((byte)service.ServiceId) ? priceRequiredOrder[(byte)service.ServiceId] : 0);
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col++, value, customStyle);
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");
                        ExcelHelper.CreateCellTable(sheet, no, col++, "");
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, priceFeature + priceRequired, customStyle);

                    }
                    no++;
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert("Estimated revenue - Receivable - Payable")).Replace("-", "_").Replace("__", "_")}_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ExcelDebitServices(byte type, int serviceId)
        {
            int currentPage = 1;
            int recordPerPage = Int32.MaxValue;
            Expression<Func<DebitReport, bool>> reportQuery;
            List<OrderResult> orderReports;
            var services = new List<dynamic>()
            {
                new { ServiceId = 255, ServiceName = "Cost of goods" },
                new { ServiceId = (byte)OrderServices.ShopShipping, ServiceName = "Transport fee by shop (suppliers)" },
                //new { ServiceId = (byte)OrderServices.Order, ServiceName = "Phí mua hàng" },
                new { ServiceId = (byte)OrderServices.Audit, ServiceName = "Counting fee" },
                new { ServiceId = (byte)OrderServices.Other, ServiceName = "Incurred expenses" },
                new { ServiceId = (byte)OrderServices.OutSideShipping, ServiceName = "Warehouse transfer fee" },
                new { ServiceId = (byte)OrderServices.Packing, ServiceName = "Packaging fee" },
                new { ServiceId = (byte)OrderServices.InSideShipping, ServiceName = "Shipping fee " },
                new { ServiceId = (byte)OrderServices.Storaged, ServiceName = "Warehouse storage fee" },
            };
            // Dự kiến thu kiện
            Dictionary<int, List<DebitReport>> debitFeaturePackages;
            // Dự kiến thu đơn
            Dictionary<int, List<DebitReport>> debitFeatureOrder;

            // Phải thu kiện
            Dictionary<int, List<DebitReport>> debitRequiredPackages;
            // Phải thu đơn
            Dictionary<int, List<DebitReport>> debitRequiredOrder;

            string orderIds;
            long totalRecord;
            string serviceName = "";
            var titleName = GetTitleName(type, serviceId, services);

            // Query Orders dự kiến thu
            Expression<Func<Order, bool>> orderFeatureQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                 x.Status < (byte)OrderStatus.Pending ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                 x.Status < (byte)DepositStatus.Pending);

            // Query package dự kiến thu
            Expression<Func<OrderPackage, bool>> packageFeatureQuery = x =>
               x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.ChinaInStock &&
               x.Status < (byte)OrderPackageStatus.Received;

            // Query Orders phải thu
            Expression<Func<Order, bool>> orderRequiredQuery = x =>
                x.IsDelete == false &&
                (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.Pending &&
                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                 x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.Pending &&
                 x.Status <= (byte)DepositStatus.GoingDelivery);

            // query package phải thu
            Expression<Func<OrderPackage, bool>> packageRequiredQuery = x =>
                x.IsDelete == false && x.Status >= (byte)OrderPackageStatus.Received &&
                x.Status <= (byte)OrderPackageStatus.GoingDelivery;

            foreach (var item in services)
            {
                if (item.ServiceId == serviceId)
                {
                    serviceName = item.ServiceName;
                }
            }
            // Total money các dịch vụ đơn lẻ
            if (type == 0 && serviceId != -1)
            {

                reportQuery = x => x.ServiceId == serviceId && x.Price > 0;

                orderReports = await UnitOfWork.DebitReportRepo.GetByOrderAll(reportQuery, x =>
                        x.IsDelete == false &&
                        (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                         x.Status <= (byte)OrderStatus.GoingDelivery ||
                         x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                         x.Status <= (byte)DepositStatus.GoingDelivery), currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                // Lấy theo đơn (Tiền hàng thiếu, tiền mua hàng, tiền kiểm đếm)
                if (serviceId == 255 /*|| serviceId == (byte)OrderServices.Order*/ || serviceId == (byte)OrderServices.ShopShipping || serviceId == (byte)OrderServices.Audit)
                {
                    if (serviceId == 255)
                    {
                        serviceName = "TIỀN HÀNG";
                    }

                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery,
                        orderIds);
                    debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery,
                        orderRequiredQuery, orderIds);

                    return RenderExcellTemplate0(serviceId, serviceName, titleName, debitFeatureOrder, debitRequiredOrder, orderReports, 0);
                }

                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageFeatureQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageRequiredQuery, orderIds);

                return RenderExcellTemplate0(serviceId, serviceName, titleName, debitFeaturePackages, debitRequiredPackages, orderReports, 1);
            }

            if (type != 0 && serviceId == -1) // Total money: Dự kiến thu, Phải thu, Phải trả
            {
                reportQuery = x => x.Price > 0;

                // Dự kiến thu
                if (type == 1)
                {
                    orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderFeatureQuery, currentPage,
                    recordPerPage, out totalRecord);

                    orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);
                    debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageFeatureQuery, orderIds);

                    return RenderExcellTemplate1(type, services, titleName, debitFeatureOrder, debitFeaturePackages, orderReports, 0);
                }

                // Phải thu
                orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderRequiredQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageRequiredQuery, orderIds);

                return RenderExcellTemplate1(type, services, titleName, debitRequiredOrder, debitRequiredPackages, orderReports, 1);
            }

            if (type == 0 && serviceId == -1) // Sum thu
            {
                reportQuery = x => x.Price > 0;

                orderReports = await UnitOfWork.DebitReportRepo.GetByOrderAll(reportQuery, x =>
                        x.IsDelete == false &&
                        (x.Type != (byte)OrderType.Deposit && x.Status >= (byte)OrderStatus.OrderSuccess &&
                         x.Status <= (byte)OrderStatus.GoingDelivery ||
                         x.Type == (byte)OrderType.Deposit && x.Status >= (byte)DepositStatus.InWarehouse &&
                         x.Status <= (byte)DepositStatus.GoingDelivery), currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);
                debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageRequiredQuery, orderIds);
                debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);
                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageFeatureQuery, orderIds);

               

                return RenderExcellTemplate2(type, services, titleName, debitFeatureOrder, debitRequiredOrder, 
                    debitFeaturePackages, debitRequiredPackages, orderReports);

            }

            // Excess money phải trả của Orders
            List<Order> orders;
            if (serviceId == 254)
            {
                serviceName = "Change due";
                // Tổng tiền hoàn trả đơn hàng
                orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                    x => x.IsDelete == false && (x.Type != (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)OrderStatus.OrderSuccess &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery ||
                                                 x.Type == (byte)OrderType.Deposit &&
                                                 x.Status >= (byte)DepositStatus.InWarehouse &&
                                                 x.Status <= (byte)OrderStatus.GoingDelivery) && x.Debt < 0,
                    o => o.OrderBy(y => y.Id), currentPage, recordPerPage);

                return RenderExcellTemplate4(serviceId, serviceName, titleName, null, orders);
            }

            if (serviceId == 253) // Tiền Deposit Orders
            {
                serviceName = "Deposit";
                // Tổng tiền đặt cọc đơn hàng
                orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(out totalRecord,
                    x => x.IsDelete == false && x.Type != (byte)OrderType.Deposit &&
                         x.Status >= (byte)OrderStatus.WaitOrder && x.Status < (byte)OrderStatus.OrderSuccess && x.TotalPayed > 0,
                    o => o.OrderBy(y => y.Id), currentPage, recordPerPage);

                return RenderExcellTemplate4(serviceId, serviceName, titleName, null, orders);
            }

            reportQuery = x => x.ServiceId == serviceId && x.Price > 0;

            // Lấy theo đơn (Tiền hàng thiếu, tiền mua hàng, tiền kiểm đếm)
            if (serviceId == 255 /*|| serviceId == (byte)OrderServices.Order*/ || serviceId == (byte)OrderServices.ShopShipping || serviceId == (byte)OrderServices.Audit)
            {
                if (serviceId == 255)
                {
                    serviceName = "COST OF GOODS";
                }
                // Dự kiến thu

                if (type == 1)
                {
                    orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderFeatureQuery, currentPage,
                        recordPerPage, out totalRecord);

                    orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                    debitFeatureOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderFeatureQuery, orderIds);

                    return RenderExcellTemplate3(type, serviceName, titleName, debitFeatureOrder, orderReports, 0);
                }

                // Phải thu
                orderReports = await UnitOfWork.DebitReportRepo.GetByOrder(reportQuery, orderRequiredQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitRequiredOrder = await UnitOfWork.DebitReportRepo.GetByOrderQuery(reportQuery, orderRequiredQuery, orderIds);

                return RenderExcellTemplate3(type, serviceName, titleName, debitRequiredOrder, orderReports, 0);
            }

            // Lấy theo package
            // Dự kiến thu
            if (type == 1)
            {
                orderReports = await UnitOfWork.DebitReportRepo.GetByPackage(reportQuery, packageFeatureQuery, currentPage,
                    recordPerPage, out totalRecord);

                orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

                debitFeaturePackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageFeatureQuery, orderIds);

                return RenderExcellTemplate3(type, serviceName, titleName, debitFeaturePackages, orderReports, 1);
            }

            // Phải thu
            orderReports = await UnitOfWork.DebitReportRepo.GetByPackage(reportQuery, packageRequiredQuery, currentPage,
                recordPerPage, out totalRecord);

            orderIds = $";{string.Join(";", orderReports.Select(x => x.Id).ToList())};";

            debitRequiredPackages = await UnitOfWork.DebitReportRepo.GetByPackageQuery(reportQuery, packageRequiredQuery, orderIds);

            return RenderExcellTemplate3(type, serviceName, titleName, debitRequiredPackages, orderReports, 1);
        }


        /// <summary>
        /// Export Excell Sum phải thu, phải trả của dịch vụ
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="serviceName"></param>
        /// <param name="titleFile"></param>
        /// <param name="debitFeatureOrder"></param>
        /// <param name="debitRequiredOrder"></param>
        /// <param name="orderReports"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RenderExcellTemplate0(int serviceId, string serviceName, string titleFile, Dictionary<int, List<DebitReport>> debitFeatureOrder, Dictionary<int, List<DebitReport>> debitRequiredOrder, List<OrderResult> orderReports, int type)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;
                var count = orderReports.Count();
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "CUSTOMER NAME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "PHONE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + 1, serviceName.ToUpper(), ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                //THÊM HÀNG
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col - 1, "ESTIMATED REVENUE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col - 1, "RECEIVABLE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col, row + 1, col, "TOTAL", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleFile.ToUpper(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                #endregion

                var customStyle = new CustomExcelStyle
                {
                    IsMerge = false,
                    IsBold = false,
                    Border = ExcelBorderStyle.Thin,
                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                    NumberFormat = "#,##0"
                };

                var no = row + 2;
                if (orderReports.Any())
                {

                    foreach (var order in orderReports)
                    {
                        decimal type1 = 0;
                        decimal type2 = 0;
                        col = 1;
                        //STT

                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row - 1, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.Code);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerName);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerPhone);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerEmail);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, order.Type == (byte) OrderType.Deposit
                            ? EnumHelper.GetEnumDescription<DepositStatus>(order.Status)
                            : EnumHelper.GetEnumDescription<OrderStatus>(order.Status));

                        //Lấy giá trị
                        if (type == 0)
                        {
                            if (debitFeatureOrder.ContainsKey(order.Id) && debitFeatureOrder[order.Id].Any(x => x.ServiceId == serviceId))
                            {
                                type1 = debitFeatureOrder[order.Id].First(x => x.ServiceId == serviceId).Price;
                            }
                            else if (debitRequiredOrder.ContainsKey(order.Id) && debitRequiredOrder[order.Id].Any(x => x.ServiceId == serviceId))
                            {
                                type2 = debitRequiredOrder[order.Id].Where(x => x.ServiceId == serviceId).Sum(x => x.Price);
                            }
                            
                        } else if(type == 1)
                        {

                            if (debitFeatureOrder.ContainsKey(order.Id) && debitFeatureOrder[order.Id].Any(x => x.ServiceId == serviceId))
                            {
                                type1 = debitFeatureOrder[order.Id].Where(x => x.ServiceId == serviceId).Sum(x => x.Price);
                            }
                            else if (debitRequiredOrder.ContainsKey(order.Id) && debitRequiredOrder[order.Id].Any(x => x.ServiceId == serviceId))
                            {
                                type2 = debitRequiredOrder[order.Id].Where(x => x.ServiceId == serviceId).Sum(x => x.Price);
                            }
                            
                        }
                        
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, type1, customStyle);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, type2, customStyle);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, type1 + type2, customStyle);

                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;
                sheet.Row(5).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
                    $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert(titleFile)).Replace("-", "_").Replace("__", "_")}_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        /// <summary>
        /// Sum các dịch vụ
        /// </summary>
        /// <param name="type"></param>
        /// <param name="services"></param>
        /// <param name="titleFile"></param>
        /// <param name="debitFeatureOrder"></param>
        /// <param name="debitRequiredOrder"></param>
        /// <param name="orderReports"></param>
        /// <param name="typeDebit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RenderExcellTemplate1(byte type, List<dynamic> services, string titleFile, Dictionary<int, List<DebitReport>> debitFeatureOrder, Dictionary<int, List<DebitReport>> debitRequiredOrder, List<OrderResult> orderReports, int typeDebit)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER NAME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);

                foreach (var service in services)
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col++, service.ServiceName.ToUpper(), ExcelHorizontalAlignment.Center, true, colorHeader);
                }

                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleFile.ToUpper(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                #endregion

                var customStyle = new CustomExcelStyle
                {
                    IsMerge = false,
                    IsBold = false,
                    Border = ExcelBorderStyle.Thin,
                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                    NumberFormat = "#,##0"
                };

                var no = row + 1;
                if (orderReports.Any())
                {
                    foreach (var order in orderReports)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.Code);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerName);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerEmail);
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++,
                            order.Type == (byte)OrderType.Deposit ?
                                EnumHelper.GetEnumDescription<DepositStatus>(order.Status)
                            : EnumHelper.GetEnumDescription<OrderStatus>(order.Status));

                        decimal total = 0;
                        foreach (var servicee in services)
                        {
                            decimal price = 0;

                            if (debitFeatureOrder.ContainsKey(order.Id) && debitFeatureOrder[order.Id].Any(x => x.ServiceId == servicee.ServiceId))
                            {
                                price = debitFeatureOrder[order.Id].First(x => x.ServiceId == servicee.ServiceId).Price;
                            }
                            else if (debitRequiredOrder.ContainsKey(order.Id) && debitRequiredOrder[order.Id].Any(x => x.ServiceId == servicee.ServiceId))
                            {
                                price = debitRequiredOrder[order.Id].Where(x => x.ServiceId == servicee.ServiceId).Sum(x => x.Price);
                            }
                            
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col++, price, customStyle);
                            total = total + price;
                        }

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, total, customStyle);
                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
                    $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert(titleFile)).Replace("-", "_").Replace("__", "_")}_{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }
        [HttpPost]
        public ActionResult RenderExcellTemplate2(byte type, List<dynamic> services, string titleFile, 
            Dictionary<int, List<DebitReport>> debitFeatureOrder, Dictionary<int, List<DebitReport>> debitRequiredOrder, 
            Dictionary<int, List<DebitReport>> debitFeaturePackages, Dictionary<int, List<DebitReport>> debitRequiredPackages, 
            List<OrderResult> orderReports)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER NAME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "", ExcelHorizontalAlignment.Center, true, colorHeader);

                foreach (var service in services)
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col++, service.ServiceName.ToUpper(), ExcelHorizontalAlignment.Center, true, colorHeader);
                }

                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleFile.ToUpper(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16,
                });
                #endregion

                var no = row + 1;
                if (orderReports.Any())
                {
                    var cStye0 = new CustomExcelStyle
                    {
                        IsBold = false,
                        IsMerge = true,
                        HorizontalAlign = ExcelHorizontalAlignment.Center,

                    };

                    foreach (var order in orderReports)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no + 1, col++, no - row, cStye0);
                        ExcelHelper.CreateCellTable(sheet, no, col, no + 1, col++, order.Code, cStye0);
                        ExcelHelper.CreateCellTable(sheet, no, col, no + 1, col++, order.CustomerName, cStye0);
                        ExcelHelper.CreateCellTable(sheet, no, col, no + 1, col++, order.CustomerEmail, cStye0);

                        ExcelHelper.CreateCellTable(sheet, no, col, no + 1, col++, 
                            order.Type == (byte)OrderType.Deposit ? 
                                EnumHelper.GetEnumDescription<DepositStatus>(order.Status) 
                            : EnumHelper.GetEnumDescription<OrderStatus>(order.Status), cStye0);

                        var colOld = col;

                        ExcelHelper.CreateCellTable(sheet, no, col++, "Estimated revenue", ExcelHorizontalAlignment.Right, true);
                        ExcelHelper.CreateCellTable(sheet, no + 1, colOld++, "Receivable", ExcelHorizontalAlignment.Right, true);

                        decimal total = 0;
                        decimal totalP = 0;

                        var cStye = new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        };

                        foreach (var servicee in services)
                        {
                            decimal price = 0;
                            decimal priceP = 0;

                            // Tiền dự kiến thu
                            if (debitFeatureOrder.ContainsKey(order.Id) && debitFeatureOrder[order.Id].Any(x=> x.ServiceId == servicee.ServiceId))
                            {
                                price = debitFeatureOrder[order.Id].First(x => x.ServiceId == servicee.ServiceId).Price;
                            } else if (debitFeaturePackages.ContainsKey(order.Id) && debitFeaturePackages[order.Id].Any(x => x.ServiceId == servicee.ServiceId))
                            {
                                price = debitFeaturePackages[order.Id].Where(x => x.ServiceId == servicee.ServiceId).Sum(x => x.Price);
                            }

                            // Tiền phải thu
                            if (debitRequiredOrder.ContainsKey(order.Id) && debitRequiredOrder[order.Id].Any(x => x.ServiceId == servicee.ServiceId))
                            {
                                priceP = debitRequiredOrder[order.Id].First(x => x.ServiceId == servicee.ServiceId).Price;
                            }else if (debitRequiredPackages.ContainsKey(order.Id) && debitRequiredPackages[order.Id].Any(x => x.ServiceId == servicee.ServiceId))
                            {
                                priceP = debitRequiredPackages[order.Id].Where(x => x.ServiceId == servicee.ServiceId).Sum(x => x.Price);
                            }
                            
                            ExcelHelper.CreateCellTable(sheet, no, col, no, col++, price, cStye);
                            ExcelHelper.CreateCellTable(sheet, no + 1, colOld, no + 1, colOld++, priceP, cStye);

                            total += price;
                            totalP += priceP;
                        }
                        
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, total, cStye);
                        ExcelHelper.CreateCellTable(sheet, no + 1, colOld, no + 1, colOld, totalP, cStye);
                        no += 2;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
                    $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert(titleFile)).Replace("-", "_").Replace("__", "_")}{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        [HttpPost]
        public ActionResult RenderExcellTemplate3(byte type, string serviceName, string titleFile, Dictionary<int, List<DebitReport>> debitFeatureOrder, List<OrderResult> orderReports, int typeDebit)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER NAME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, serviceName.ToLower(), ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleFile.ToLower(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                #endregion

                var no = row + 1;
                if (orderReports.Any())
                {
                    foreach (var order in orderReports)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.Code);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerName);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerEmail);

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col,
                            order.Type == (byte)OrderType.Deposit ?
                                EnumHelper.GetEnumDescription<DepositStatus>(order.Status)
                            : EnumHelper.GetEnumDescription<OrderStatus>(order.Status));
                        col++;


                        decimal price = 0;

                        if (typeDebit == 0)
                        {
                            if (debitFeatureOrder.ContainsKey(order.Id))
                            {
                                price = debitFeatureOrder[order.Id][0].Price;
                            }
                        }
                        if (typeDebit == 1)
                        {
                            if (debitFeatureOrder.ContainsKey(order.Id))
                            {
                                price = debitFeatureOrder[order.Id].Sum(s => s.Price);
                            }
                        }

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, price, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });

                        no++;
                    }

                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, 
                    $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert(titleFile)).Replace("-", "_").Replace("__","_")}{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }
        [HttpPost]
        public ActionResult RenderExcellTemplate4(int serviceId, string serviceName, string titleFile, Dictionary<int, List<DebitReport>> debitFeatureOrder, List<Order> orders)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "ORDER CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "CUSTOMER NAME", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                ExcelHelper.CreateHeaderTable(sheet, row, col++, serviceName.ToLower(), ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleFile.ToUpper(), new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                #endregion

                var no = row + 1;
                if (orders.Any())
                {
                    foreach (var order in orders)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col++, no - row, ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.Code);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerName);
                        ExcelHelper.CreateCellTable(sheet, no, col++, order.CustomerEmail);
                        ExcelHelper.CreateCellTable(sheet, no, col++, no, col,
                             order.Type == (byte)OrderType.Deposit ?
                                 EnumHelper.GetEnumDescription<DepositStatus>(order.Status)
                             : EnumHelper.GetEnumDescription<OrderStatus>(order.Status));

                        decimal price = 0;
                        if (serviceId == 254)
                        {
                            price = (-1) * order.Debt;
                        }
                        if (serviceId == 253)
                        {
                            price = order.TotalPayed;
                        }

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col++, price, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        no++;
                    }
                }

                ExcelHelper.CreateColumnAutofit(sheet, 1, col);
                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet,
                    $"{MyCommon.ConvertToUrlString(MyCommon.Ucs2Convert(titleFile)).Replace("-", "_").Replace("__", "_")}{DateTime.Now:yyyyMMdd}.xlsx");
            }
        }

        private string GetTitleName(byte type, int serviceId, List<dynamic> services)
        {
            var typeName = "";
            var serviceName = "";

            if (type == 1)
            {
                typeName = "Estimated revenue";
            }
            else if (type == 2)
            {
                typeName = "Receivable";
            }
            else if (type == 0)
            {
                typeName = "Total revenue";
            }
            else if (type == 3)
            {
                typeName = "Payable";
            }

            if (serviceId == 255)
            {
                serviceName = "Cost of goods";
            }
            else if (serviceId == 254)
            {
                serviceName = "Change due";
            }
            else if (serviceId == 253)
            {
                serviceName = "Deposit";
            }
            else if (serviceId == -1)
            {
                serviceName = "Total";
            }
            else
            {
                var service = services.Single(s => s.ServiceId == serviceId);

                serviceName = service.ServiceName;
            }

            if (type == 0 && serviceId != -1)
            {
                return typeName + " - " + serviceName;
            }
            else if (serviceId == -1 && type != 0)
            {
                return serviceName + " - " + typeName;
            }
            else if (type == 0 && serviceId == -1)
            {
                return "Total service revenue";
            }
            else
            {
               return serviceName + " - " + typeName;
            }
        }
    }
}