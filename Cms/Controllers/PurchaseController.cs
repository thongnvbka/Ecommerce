using AutoMapper;
using Cms.Attributes;
using Cms.Helpers;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.ViewModels.Complains;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ResourcesLikeOrderThaiLan;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cms.Controllers
{
    [Authorize]
    public class PurchaseController : BaseController
    {
        #region [system]

        #region [Khởi tạo dữ liệu lúc đầu load trang]

        [LogTracker(EnumAction.View, EnumPage.Order, EnumPage.ReportOrder, EnumPage.OrderDelay, EnumPage.OrderDeposit,
             EnumPage.OrderDepositDelay, EnumPage.OrderDepositNew,
             EnumPage.OrderNew, EnumPage.OrderOrder, EnumPage.OrderSourcing, EnumPage.OrderSourcingNew, EnumPage.OrderSourcingDelay, EnumPage.CustomerSearch,
            EnumPage.OrderWanning, EnumPage.OrderAccountant, EnumPage.OrderSupport, EnumPage.OrderClaimForRefund, EnumPage.OrderNoCodeOfLading, EnumPage.OrderAwaitingPayment,
            EnumPage.OrderNotEnoughInventory, EnumPage.OrderTallyMissing, EnumPage.OrderSuccess)]
        public async Task<ActionResult> Order()
        {
            //1. Lấy danh sách trạng thái
            var listStatus = new List<dynamic>() { new { Value = -1, Text = "All" } };
            listStatus.AddRange(Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderStatus>((int)x) }));

            //2. Lấy danh sách Order order
            var listOrder = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(x => !x.IsDelete && x.Type == (byte)OrderType.Order);

            //6. Lấy danh sách system
            var listSystem = new List<dynamic>()
            {
                new
                {
                     Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listOrder.Count,
                    ClassChild = "label-danger"
                }
            };
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            listSystem.AddRange(listSystemDb.Select(x => new
            {
                Text = x.Domain,
                Value = x.Id,
                Class = "",
                Total = listOrder.Count(y => y.SystemId == x.Id),
                ClassChild = "label-primary"
            }));

            //6. Truyền dữ liệu lên view

            //Order order mới
            ViewBag.TotalOrderNew = 0;
            //Order order chờ báo giá
            ViewBag.TotalOrderWait = 0;
            //Order order đang xử lý
            ViewBag.TotalOrder = 0;
            //Order order trễ xử lý
            ViewBag.TotalOrderLate = 0;

            //Đơn ký gửi mới
            ViewBag.TotalOrderDepositNew = 0;
            //Đơn ký gửi đang xử lý
            ViewBag.TotalOrderDeposit = 0;
            //Đơn ký gửi trễ xử lý
            ViewBag.TotalOrderDepositLate = 0;

            //Phiếu báo giá tìm nguồn mới
            ViewBag.TotalStockQuoesNew = 0;
            //Phiếu báo giá tìm nguồn đang xử lý
            ViewBag.TotalStockQuoes = 0;

            //Đờn tìm nguồn đang xử lý
            ViewBag.TotalOrderSourcing = 0;

            //Order chưa có mã vận đơn
            ViewBag.TotalOrderRisk = 0;

            //Order chờ kế toán thanh toán
            ViewBag.TotalOrderAccountant = 0;

            //Order chưa đủ kiện về kho
            ViewBag.TotalOrderNoWarehouse = 0;

            //Danh sách kho trung quốc
            ViewBag.ListWarehouse = JsonConvert.SerializeObject(UnitOfWork.DbContext.Offices.Select(x => new { x.Id, x.Code, x.Name, x.Address, x.Culture, x.Type, x.IsDelete }).Where(x => x.Culture == "CH" && x.Type == (byte)OfficeType.Warehouse && !x.IsDelete));
            //Danh sách kho việt nam
            ViewBag.ListWarehouseVN = JsonConvert.SerializeObject(UnitOfWork.DbContext.Offices.Select(x => new { x.Id, x.Code, x.Name, x.Address, x.Culture, x.Type, x.IsDelete }).Where(x => x.Culture == "VN" && x.Type == (byte)OfficeType.Warehouse && !x.IsDelete));
            //Danh sách trạng thái
            ViewBag.ListStatus = JsonConvert.SerializeObject(listStatus);
            //Danh sách hộ thống
            ViewBag.ListSystem = JsonConvert.SerializeObject(listSystem);
            //Danh sách danh mục sản phẩm
            ViewBag.ListCategory = GetCatetgoryJsTree();

            if (UserState.OfficeId == null) return View();
            if (UserState.Type == null) return View();
            //Danh sách nhân viên trong phòng
            var listUser = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, UserState.Type.Value, UserState.OfficeIdPath, UserState.OfficeId.Value);
            listUser.Add(new UserOfficeResult { Id = -1, FullName = "All" });
            ViewBag.ListUser = JsonConvert.SerializeObject(listUser.OrderBy(x => x.Id));

            ViewBag.ExchangeRate = ExchangeRate();

            var listOrderService = new List<dynamic>()
            {
                new
                {
                    ServiceId = (byte)OrderServices.Audit,
                    ServiceName = EnumHelper.GetEnumDescription<OrderServices>((byte)OrderServices.Audit),
                    ExchangeRate = "active",
                    Currency = Currency.VND,
                    Type = (byte)UnitType.Money,
                    TotalPrice = "label-danger",
                    Mode =(byte)OrderServiceMode.Option,
                    Checked = false
                },
                new
                {
                    ServiceId = (byte)OrderServices.Packing,
                    ServiceName = EnumHelper.GetEnumDescription<OrderServices>((byte)OrderServices.Packing),
                    ExchangeRate = "active",
                    Currency = Currency.CNY,
                    Type = (byte)UnitType.Money,
                    TotalPrice = "label-danger",
                    Mode =(byte)OrderServiceMode.Option,
                    Checked = false
                }
            };

            //tạo lý do Order trễ xử lý
            var delay = DateTime.Now.AddMinutes(TimeDelay);
            var listOrderDelay =
                UnitOfWork.OrderRepo.Entities.Where(
                        x =>
                            (x.Status == (byte)OrderStatus.Order ||
                            x.Status == (byte)OrderStatus.AreQuotes) && x.Created <= delay && !x.IsDelete && x.UserId != null)
                    .ToList();

            var listOrderReason = new List<OrderReason>();

            foreach (var order in listOrderDelay)
            {
                var orderReason = await UnitOfWork.OrderReasonRepo.FirstOrDefaultAsync(x => x.OrderId == order.Id);
                if (orderReason != null) continue;
                orderReason = new OrderReason()
                {
                    OrderId = order.Id,
                    ReasonId = (byte)OrderReasons.ReasonsNotSelected,
                    Reason = EnumHelper.GetEnumDescription<OrderReasons>((byte)OrderReasons.ReasonsNotSelected),
                    Type = (byte)OrderReasonType.Delay
                };

                listOrderReason.Add(orderReason);
            }
            UnitOfWork.OrderReasonRepo.AddRange(listOrderReason);
            await UnitOfWork.OrderReasonRepo.SaveAsync();

            ViewBag.ListOrderService = JsonConvert.SerializeObject(listOrderService);

            //danh sách lý do chậm xử lý
            var listReason = new List<dynamic>();
            listReason.AddRange(Enum.GetValues(typeof(OrderReasons))
                .Cast<OrderReasons>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderReasons>((int)x) }));

            ViewBag.ListReason = JsonConvert.SerializeObject(listReason.Where(x => x.Value != 0).ToList());

            //danh sách lý do quá thời gian mà chưa có mã vận đơn
            var listReasonNoCodeOfLading = new List<dynamic>();
            listReasonNoCodeOfLading.AddRange(Enum.GetValues(typeof(OrderReasonNoCodeOfLading))
                .Cast<OrderReasonNoCodeOfLading>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderReasonNoCodeOfLading>((int)x) }));

            ViewBag.ListReasonNoCodeOfLading = JsonConvert.SerializeObject(listReasonNoCodeOfLading.Where(x => x.Value != 0).ToList());

            // danh sách lý do quá thời gian chưa đủ kiện về kho
            var listReasonNotEnoughInventory = new List<dynamic>();
            listReasonNotEnoughInventory.AddRange(Enum.GetValues(typeof(OrderReasonNotEnoughInventory))
                .Cast<OrderReasonNotEnoughInventory>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderReasonNotEnoughInventory>((int)x) }));

            ViewBag.ListReasonNotEnoughInventory = JsonConvert.SerializeObject(listReasonNotEnoughInventory.Where(x => x.Value != 0).ToList());

            // lấy danh sách bargain
            var listBargainType = new List<dynamic>();
            listBargainType.AddRange(Enum.GetValues(typeof(BargainType))
                .Cast<BargainType>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<BargainType>((int)x) }));

            ViewBag.ListBargainType = JsonConvert.SerializeObject(listBargainType.ToList());

            //Danh sách Type khiếu nại cho CSKH chốt
            var listtypeClose = UnitOfWork.ComplainTypeRepo.Find(x => !x.IsDelete /*!x.IsParent*/).ToList();
            var complainType = new List<SelectListItem>();
            foreach (var item in listtypeClose)
            {
                complainType.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.ListcomplainType = JsonConvert.SerializeObject(complainType);

            return View();
        }

        #endregion [Khởi tạo dữ liệu lúc đầu load trang]

        #region [Lấy danh sách hệ thống]

        [HttpPost]
        public async Task<JsonResult> GetRenderSystem(string active)
        {
            var dateDelay = DateTime.Now.AddMinutes(TimeDelay);
            //1. Lấy danh sách trạng thái
            var listStatus = new List<dynamic>() { new { Value = -1, Text = "All" } };

            listStatus.AddRange(Enum.GetValues(typeof(OrderStatus))
                .Cast<OrderStatus>()
                .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderStatus>((int)x) }));

            //2. Khởi tạo và lấy ra danh sách Order theo từng Type
            var listOrder = new List<SystemMeta>();
            long totalRecord;

            switch (active)
            {
                //Order order mới
                case "order-new":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.Status == (byte)OrderStatus.WaitOrder && x.UserId == null && !x.IsDelete && x.Type == (byte)OrderType.Order).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Order order chờ báo giá
                case "order-cus":
                    listStatus = new List<dynamic>() { new { Value = -1, Text = "All", Type = -1 } };

                    listStatus.AddRange(Enum.GetValues(typeof(OrderStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (byte)OrderType.Order + "." + (int)x, Text = EnumHelper.GetEnumDescription<OrderStatus>((int)x), Type = OrderType.Order }));

                    listStatus.AddRange(Enum.GetValues(typeof(DepositStatus))
                        .Cast<DepositStatus>()
                        .Select(x => new { Value = (byte)OrderType.Deposit + "." + (int)x, Text = EnumHelper.GetEnumDescription<DepositStatus>((int)x), Type = OrderType.Deposit }));

                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete
                        && (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                        && (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId))
                        .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Order order chờ báo giá
                case "order-wait":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.Id > 0 && x.Status == (byte)OrderStatus.AreQuotes && !x.IsDelete && x.Type == (byte)OrderType.Order
                    && (UserState.Type == 0 || (x.CustomerCareOfficeIdPath == UserState.OfficeIdPath || x.CustomerCareOfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.CustomerCareUserId == UserState.UserId))
                    .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Order order chờ báo giá mới
                case "order-wait-new":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.Id > 0 && x.Status == (byte)OrderStatus.WaitPrice && !x.IsDelete && x.Type == (byte)OrderType.Order)
                    .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Order order đang xử lý
                case "order":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status >= (byte)OrderStatus.Order
                   && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                   && (UserState.Type != 0 || x.UserId == UserState.UserId))
                   .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;

                case "order-delay":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => (x.Status == (byte)OrderStatus.Order) && x.Created <= dateDelay && !x.IsDelete && x.Type == (byte)OrderType.Order
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId))
                    .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Complete order
                case "order-success":
                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x =>
                      ((x.Type != (byte)OrderType.Deposit && x.Status == (byte)OrderStatus.GoingDelivery) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.GoingDelivery))
                      && !x.IsDelete
                      && x.Debt > -100 && x.Debt < 100
                      && x.PackageNo == x.PackageNoDelivered
                      && x.LastDeliveryTime != null)
                   .Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Đơn ký gửi mới
                case "order-deposit-new":
                    listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(DepositStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<DepositStatus>((int)x) }));

                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete && x.UserId == null && x.Status == (byte)DepositStatus.WaitDeposit && x.Type == (byte)OrderType.Deposit).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Đơn ký gửi
                case "order-deposit":
                    listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(DepositStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<DepositStatus>((int)x) }));

                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.UserId != null && !x.IsDelete && x.Type == (byte)OrderType.Deposit).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Đơn ký gửi trễ xử lý
                case "order-deposit-delay":
                    listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(DepositStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<DepositStatus>((int)x) }));

                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.UserId != null && !x.IsDelete && x.Status == (byte)DepositStatus.Order && x.Created <= dateDelay && x.Type == (byte)OrderType.Deposit).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Phiếu báo giá tìm nguồn chưa nhận xử lý
                case "stock-quotes-new":
                    listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(SourceStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<SourceStatus>((int)x) }));

                    listOrder = UnitOfWork.SourceRepo.FindAsNoTracking(x => x.Id > 0 && !x.IsDelete && x.Status == (byte)SourceStatus.WaitProcess && x.SourceSupplierId == null).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //phiếu báo giá tìm nguồn
                case "stock-quotes":
                    listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(SourceStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<SourceStatus>((int)x) }));

                    listOrder = UnitOfWork.SourceRepo.FindAsNoTracking(x => x.Id > 0 && !x.IsDelete && x.SourceSupplierId == null && x.UserId != null).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //Order tìm nguồn
                case "order-sourcing":
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(SourceStatus))
                        .Cast<OrderStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<SourceStatus>((int)x) }));

                    listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete && x.UserId != null && x.Type == (byte)OrderType.Source).Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //lấy thông tin mã vẫn đơn chờ thanh toán
                case "accountantOrder":
                    decimal totalPrice;
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(ContractCodeType))
                        .Cast<ContractCodeType>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<ContractCodeType>((int)x) }));

                    var listAccountantOrder = await UnitOfWork.OrderRepo.GetOrderContractCode(out totalPrice, out totalRecord, 1, 1000000, "", -1, -1, null, null, null, null);
                    listOrder = listAccountantOrder.Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //lấy thông tin Order chưa có mã vận đơn
                case "order-risk":
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(ContractCodeType))
                        .Cast<ContractCodeType>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<ContractCodeType>((int)x) }));

                    var listRisk = UnitOfWork.OrderRepo.GetOrderNoContractCode(out totalRecord, 1, 1000000, "", -1, -1, null, null, null, null, UserState, false);
                    listOrder = listRisk.Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //lấy thông tin Order chờ kế toán thanh toán
                case "order-accountant":
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(ContractCodeType))
                        .Cast<ContractCodeType>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<ContractCodeType>((int)x) }));

                    var listAccountant = UnitOfWork.OrderRepo.GetOrderAccountant(out totalRecord, 1, 1000000, "", -1, -1, null, null, null, null, UserState, false);
                    listOrder = listAccountant.Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //lấy thông tin Order chưa đủ kiện về kho
                case "order-warehouse":
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(ContractCodeType))
                        .Cast<ContractCodeType>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<ContractCodeType>((int)x) }));

                    var listWarehouse = UnitOfWork.OrderRepo.GetOrderNoWarehouse(out totalRecord, 1, 1000000, "", -1, -1, null, null, null, null, UserState, false);
                    listOrder = listWarehouse.Select(x => new SystemMeta() { SystemId = x.SystemId }).ToList();
                    break;
                //mã vận đơn
                case "lading-code":
                    listStatus = new List<dynamic> { new { Text = "All", Value = -1 } };
                    listStatus.AddRange(Enum.GetValues(typeof(OrderPackageStatus))
                        .Cast<OrderPackageStatus>()
                        .Select(x => new { Value = (int)x, Text = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)x) }));

                    var list = await UnitOfWork.OrderRepo.GetOrderLadingCode(out totalRecord, 1, 1000000, "", -1, -1, null, null, null, null, false);
                    listOrder = list.Select(x => new SystemMeta() { SystemId = x.SystemId, Count = x.PackageNo.Value }).ToList();
                    break;
            };

            //3. Lấy danh sách hệ thống
            var listSystem = new List<dynamic> { new { Text = "All", Value = -1, Class = "active", Total = listOrder.Count, ClassChild = "label-danger" } };
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            if (active != "lading-code")
            {
                listSystem.AddRange(listSystemDb.Select(x => new
                {
                    Text = x.Domain,
                    Value = x.Id,
                    Class = "",
                    Total = listOrder.Count(y => y.SystemId == x.Id),
                    ClassChild = "label-primary"
                }));
            }
            else
            {
                listSystem = new List<dynamic> { new { Text = "All", Value = -1, Class = "active", Total = listOrder.Sum(y => y.Count), ClassChild = "label-danger" } };
                listSystem.AddRange(listSystemDb.Select(x => new
                {
                    Text = x.Domain,
                    Value = x.Id,
                    Class = "",
                    Total = listOrder.Where(y => y.SystemId == x.Id).Sum(y => y.Count),
                    ClassChild = "label-primary"
                }));
            }
            //4. Trả về dữ liệu
            return Json(new { listSystem, listStatus }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy danh sách hệ thống]

        #region [Dữ liệu thông báo]

        [HttpPost]
        public async Task<JsonResult> GetInit()
        {
            var totalClaimForRefund = 0;
            //1. Lấy danh sách
            //Order order
            var listOrder = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(x => !x.IsDelete && x.Type == (byte)OrderType.Order && x.Status >= (byte)OrderStatus.WaitOrder && x.Status < (byte)OrderStatus.Finish);
            //Order ký gửi
            var listOrderDeposit = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(x => !x.IsDelete && x.Type == (byte)OrderType.Deposit && x.Status < (byte)DepositStatus.Finish);

            //2.Lây thông tin hiển thị lên view
            //Sum đơn order mới
            var totalOrderNew = listOrder.Count(x => (x.Status == (byte)OrderStatus.WaitOrder) && x.UserId == null);
            //Sum đơn order chờ báo giá
            var totalOrderWait = 0;
            //Sum đơn order đang xử lý
            var totalOrder = listOrder.Count(x => x.Status >= (byte)OrderStatus.Order && x.Status < (byte)OrderStatus.Finish
                && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || (x.OfficeIdPath == null || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                && (UserState.Type != 0 || x.UserId == UserState.UserId) && !x.IsRetail);
            //Sum đơn order trễ xử lý
            var totalOrderLate = listOrder.Count(x => (x.Status == (byte)OrderStatus.Order) && x.Created <= DateTime.Now.AddMinutes(TimeDelay)
                && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || (x.OfficeIdPath == null || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + "."))))
                && (UserState.Type != 0 || x.UserId == UserState.UserId));

            //Sum đơn ký gửi mới
            var totalOrderDepositNew = listOrderDeposit.Count(x => x.Status == (byte)DepositStatus.WaitDeposit && x.UserId == null);
            //Sum đơn ký gửi đang xử lý
            var totalOrderDeposit = listOrderDeposit.Count(x => x.Status >= (byte)DepositStatus.Order);
            //Sum Order trễ xử lý
            var totalOrderDepositLate = listOrderDeposit.Count(x => x.Status == (byte)DepositStatus.Order && x.Created <= DateTime.Now.AddMinutes(TimeDelay));

            //lây Order number chưa có mã vận đơn
            long totalOrderRisk;
            UnitOfWork.OrderRepo.GetOrderNoContractCode(out totalOrderRisk, 1, 10, "", -1, -1, null, null, null, null, UserState, false);

            //Lấy Order number chờ kế toán thanh toán
            long totalOrderAccountant;
            UnitOfWork.OrderRepo.GetOrderAccountant(out totalOrderAccountant, 1, 10, "", -1, -1, null, null, null, null, UserState, false);

            //Lấy Order number chưa đủ kiện về kho
            long totalOrderNoWarehouse;
            UnitOfWork.OrderRepo.GetOrderNoWarehouse(out totalOrderNoWarehouse, 1, 10, "", -1, -1, null, null, null, null, UserState, false);
            long totalTicketSupport = await UnitOfWork.ComplainRepo.TicketSupportCountAsync(UserState);

            //7. Tính toán Sum số Customer Refund Request
            var listClaimForRefund = await UnitOfWork.ClaimForRefundRepo.FindAsync(s => (UserState.Type > 0 || s.OrderUserId == UserState.UserId));

            totalClaimForRefund = listClaimForRefund.Count();

            //3. Hiển thị lên view
            return Json(new { totalTicketSupport, totalOrderNew, totalOrderWait, totalOrder, totalOrderLate, totalOrderDepositNew, totalOrderDeposit, totalOrderDepositLate, totalStockQuoesNew = 0, totalOrderSourcing = 0, totalStockQuoes = 0, totalClaimForRefund, totalOrderRisk, totalOrderAccountant, totalOrderNoWarehouse }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Dữ liệu thông báo]

        #region [Lấy dữ liệu báo cáo theo ngày hiện tại]

        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.ReportOrder)]
        public JsonResult GetOrderReport()
        {
            //1. Lây danh sach Order xử lý trong ngày
            var dateNow = DateTime.Now;
            var listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.LastUpdate.Year == dateNow.Year && x.LastUpdate.Month == dateNow.Month
                && x.LastUpdate.Day == dateNow.Day && !x.IsDelete && x.Type == (byte)OrderType.Order).OrderBy(x => x.Status).ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                var status = (int)orderStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = orderStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listOrder.Count(x => x.Status == status)
                };

                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailOrder = new List<int>();
            var detailPrice = new List<dynamic>();

            foreach (var order in listOrder.Where(x => x.Status >= (int)OrderStatus.OrderSuccess).GroupBy(x => x.UserId).ToList())
            {
                var firstOrDefault = order.FirstOrDefault();
                if (firstOrDefault != null) detailName.Add(firstOrDefault.UserFullName);
                detailOrder.Add(order.Count());
                detailPrice.Add(order.Sum(x => x.TotalExchange));
            }

            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailOrder, detailPrice }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy dữ liệu báo cáo theo ngày hiện tại]

        #endregion [Hệ thống]

        #region [Hủy đơn hàng]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderOrder, EnumPage.OrderNew, EnumPage.OrderDelay, EnumPage.StockQuotesNew, EnumPage.StockQuotes,
            EnumPage.OrderSourcing, EnumPage.OrderSourcingNew, EnumPage.OrderSourcingDelay, EnumPage.OrderDepositNew, EnumPage.OrderDeposit, EnumPage.OrderDepositDelay)]
        public async Task<JsonResult> OrderCancel(int id, byte type, string note, byte status)
        {
            //1. Khởi tạo thông báo
            dynamic obj = new { status = (byte)MsgType.Success, msg = "Cancel Order failed!" };

            //2. Kiểm tra các Order Type
            switch (type)
            {
                //Đơn order
                case (byte)OrderType.Order:
                    obj = await CancelOrder(id, type, note, status);
                    //3. Trả dữ liệu lên view
                    return Json(new { obj.Data.status, obj.Data.msg }, JsonRequestBehavior.AllowGet);
                //Đơn ký gửi
                case (byte)OrderType.Deposit:
                    obj = await CancelDeposit(id, type, note, status);
                    //3. Trả dữ liệu lên view
                    return Json(new { obj.status, obj.msg }, JsonRequestBehavior.AllowGet);
                //Đơn tìm nguồn
                case (byte)OrderType.Source:
                    obj = await CancelSource(id, type, note, status);
                    //3. Trả dữ liệu lên view
                    return Json(new { obj.status, obj.msg }, JsonRequestBehavior.AllowGet);
                //Phiếu tìn nguồn
                case (byte)OrderType.StockQuotes:
                    obj = await CancelStockQuotes(id, type, note, status);
                    //3. Trả dữ liệu lên view
                    return Json(new { obj.status, obj.msg }, JsonRequestBehavior.AllowGet);
            }
            //3. Trả dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Perform a successful operation" }, JsonRequestBehavior.AllowGet);
        }

        //hủy đơn order
        public async Task<dynamic> CancelOrder(int id, byte type, string note, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Type == type && x.Status == status);

            //2. Kiểm tra Order
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.ReasonCancel = note;

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

                    //Lấy lại số tiền khách đã Deposit
                    var orderDeposit = UnitOfWork.OrderExchangeRepo.FirstOrDefaultAsNoTracking(x => x.OrderId == order.Id
                        && !x.IsDelete
                        && x.Type == (byte)OrderExchangeType.Product
                        && x.Mode == (byte)OrderExchangeMode.Export
                        && x.Status == (byte)OrderExchangeStatus.Approved
                    );

                    order.LastUpdate = timeNow;
                    order.Status = (byte)OrderStatus.Cancel;
                    order.Debt = 0;
                    order.TotalRefunded = orderDeposit.TotalPrice.Value;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Hủy link hàng
                    var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == order.Id);
                    foreach (var orderDetail in listOrderDetail)
                    {
                        orderDetail.LastUpdate = timeNow;
                        orderDetail.QuantityBooked = 0;
                        orderDetail.TotalPrice = 0;
                        orderDetail.TotalExchange = 0;
                        orderDetail.Status = (byte)OrderDetailStatus.Cancel;
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    //Cộng lại tiền cho khách
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);

                    var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                    {
                        CustomerId = order.CustomerId.Value,
                        CurrencyFluctuations = orderDeposit.TotalPrice.Value,
                        OrderId = order.Id,
                        Note = $"Deposit refund due to cancellation of order!",
                        TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
                    });

                    // Lỗi trong quá tình thực hiện thanh toán
                    if (processRechargeBillResult.Status < 0)
                    {
                        transaction.Rollback();
                        return Json(new { status = MsgType.Error, msg = processRechargeBillResult.Msg }, JsonRequestBehavior.AllowGet);
                    }

                    // Thêm giao dịch trong Order
                    UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                    {
                        Created = timeNow,
                        Updated = timeNow,
                        Currency = Currency.VND.ToString(),
                        ExchangeRate = order.ExchangeRate,
                        IsDelete = false,
                        Type = (byte)OrderExchangeType.Product,
                        Mode = (byte)OrderExchangeMode.Import,
                        ModeName = "Refunds canceled by Order",
                        Note = $"Get money back Deposit {orderDeposit.TotalPrice} VND Order canceled",
                        OrderId = order.Id,
                        TotalPrice = orderDeposit.TotalPrice,
                        Status = (byte)OrderExchangeStatus.Approved
                    });

                    await UnitOfWork.OrderExchangeRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Cancel Order by: " + note,
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
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
                        Content = "Cancel Order",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //Cập nhật lại trạng thái các package do Order này bị hủy
                    var lstOrderPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type);
                    foreach (var item in lstOrderPackage)
                    {
                        item.IsDelete = true;

                        UnitOfWork.OrderPackageRepo.Update(item);
                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }

                    // Gửi thông báo Notification cho khách hàng
                    var notification = new Notification()
                    {
                        SystemId = order.SystemId,
                        SystemName = order.SystemName,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = order.Id,
                        OrderType = 0,
                        IsRead = false,
                        Title = $"Cancel Order '{MyCommon.ReturnCode(order.Code)}' do '{note}'",
                        Description = $"Cancel Order '{MyCommon.ReturnCode(order.Code)}' do '{note}'"
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    await UnitOfWork.NotificationRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = MsgType.Success, msg = "Successfully canceled Order!" }, JsonRequestBehavior.AllowGet);
        }

        //hủy đơn ký gửi
        public async Task<dynamic> CancelDeposit(int id, byte type, string note, byte status)
        {
            //1. Khở tạo dữ liệu
            var timeNow = DateTime.Now;
            var deposit = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Type == type && x.Status == status);

            //2. Kiểm tra Order
            if (deposit == null)
            {
                return new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" };
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    deposit.ReasonCancel = note;

                    //Kiểm tra Order đã có người nhận xử lý chưa
                    if (deposit.UserId == null)
                    {
                        deposit.UserId = UserState.UserId;
                        deposit.UserName = UserState.UserName;
                        deposit.UserFullName = UserState.FullName;
                        deposit.OfficeId = UserState.OfficeId;
                        deposit.OfficeName = UserState.OfficeName;
                        deposit.OfficeIdPath = UserState.OfficeIdPath;
                    }

                    deposit.LastUpdate = timeNow;
                    deposit.Status = (byte)DepositStatus.Cancel;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == deposit.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Cancel Order by: " + note,
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
                        OrderId = (int)deposit.Id,
                        Status = deposit.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = deposit.Type
                    });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = deposit.Id,
                        CreateDate = timeNow,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = "Cancel Order",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //Cập nhật lại trạng thái các package do Order này bị hủy
                    var lstOrderPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == deposit.Id && x.OrderType == deposit.Type);
                    foreach (var item in lstOrderPackage)
                    {
                        item.IsDelete = true;

                        UnitOfWork.OrderPackageRepo.Update(item);
                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }

                    // Gửi thông báo Notification cho khách hàng
                    var notification = new Notification()
                    {
                        SystemId = deposit.SystemId,
                        SystemName = deposit.SystemName,
                        CustomerId = deposit.CustomerId,
                        CustomerName = deposit.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = deposit.Id,
                        OrderType = 1,
                        IsRead = false,
                        Title = $"Cancel Order '{MyCommon.ReturnCode(deposit.Code)}' do '{note}'",
                        Description = $"Cancel Order '{MyCommon.ReturnCode(deposit.Code)}' do '{note}'"
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    await UnitOfWork.NotificationRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return new { status = MsgType.Success, msg = "Successfully canceled Order!" };
        }

        //hủy phiếu tìn nguồn
        public async Task<dynamic> CancelStockQuotes(int id, byte type, string note, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var source = await UnitOfWork.SourceRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Status == status);

            //2. Kiểm tra Order
            if (source == null)
            {
                return new { status = MsgType.Error, msg = "Phiếu tìm nguồn không tồn tại hoặc đã bị xóa!" };
            }

            //3. Tao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    source.ReasonCancel = note;

                    //Kiểm tra Order đã có người nhận xử lý chưa
                    if (source.UserId == null)
                    {
                        source.UserId = UserState.UserId;
                        source.UserFullName = UserState.FullName;
                        source.OfficeId = UserState.OfficeId;
                        source.OfficeName = UserState.OfficeName;
                        source.OfficeIdPath = UserState.OfficeIdPath;
                    }

                    source.UpdateDate = timeNow;
                    source.Status = (byte)SourceStatus.Cancel;
                    await UnitOfWork.SourceRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == source.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Cancel Order by: " + note,
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
                        OrderId = (int)source.Id,
                        Status = source.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = source.Type
                    });

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

            return new { status = MsgType.Success, msg = "Successful coupon cancellation!" };
        }

        //hủy đơn tìm nguồn
        public async Task<dynamic> CancelSource(int id, byte type, string note, byte status)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && x.Type == type && x.Status == status);

            //2. Kiểm tra Order
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    order.ReasonCancel = note;

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
                    order.Status = (byte)OrderStatus.Cancel;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Lấy lại số tiền khách đã Deposit
                    var orderDeposit = UnitOfWork.OrderExchangeRepo.FirstOrDefaultAsNoTracking(x => x.OrderId == order.Id
                        && !x.IsDelete
                        && x.Type == (byte)OrderExchangeType.Product
                        && x.Mode == (byte)OrderExchangeMode.Export
                        && x.Status == (byte)OrderExchangeStatus.Approved
                    );

                    //Cộng lại tiền cho khách
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);
                    if (orderDeposit.TotalPrice != null)
                    {
                        customer.BalanceAvalible += orderDeposit.TotalPrice.Value;
                        customer.Updated = timeNow;

                        await UnitOfWork.CustomerRepo.SaveAsync();

                        // Thêm giao dịch trong Order
                        UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            Currency = Currency.VND.ToString(),
                            ExchangeRate = order.ExchangeRate,
                            IsDelete = false,
                            Type = (byte)OrderExchangeType.Product,
                            Mode = (byte)OrderExchangeMode.Import,
                            ModeName = "Refunds canceled by Order",
                            Note = $"Get Money back Deposit {orderDeposit.TotalPrice} VND Order canceled",
                            OrderId = order.Id,
                            TotalPrice = orderDeposit.TotalPrice,
                            Status = (byte)OrderExchangeStatus.Approved
                        });

                        await UnitOfWork.OrderExchangeRepo.SaveAsync();
                    }

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = "Cancel Order by: " + note,
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
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
                        Content = "Cancel Order",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    //Cập nhật lại trạng thái các package do Order này bị hủy
                    var lstOrderPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type);
                    foreach (var item in lstOrderPackage)
                    {
                        item.IsDelete = true;

                        UnitOfWork.OrderPackageRepo.Update(item);
                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }

                    // Gửi thông báo Notification cho khách hàng
                    var notification = new Notification()
                    {
                        SystemId = order.SystemId,
                        SystemName = order.SystemName,
                        CustomerId = order.CustomerId,
                        CustomerName = order.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = order.Id,
                        OrderType = 2,
                        IsRead = false,
                        Title = $"Cancel Order 'SOU{order.Code}' do '{note}'",
                        Description = $"Cancel Order 'SOU{order.Code}' do '{note}'"
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    await UnitOfWork.NotificationRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = MsgType.Success, msg = "Successfully canceled Order!" }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Cancel Order]

        #region [Kế toán thanh toán hợp đồng]

        /// <summary>
        /// Danh sách Contract code
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
        [HttpPost]
        public async Task<JsonResult> Accountant(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId)
        {
            //1. Tạo các biến
            long totalRecord;
            decimal totalPrice;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.GetOrderContractCode(out totalPrice, out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId);

            //3. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder, totalPrice }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// hủy thanh toán Contract code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AccountantCancel(int id, byte status)
        {
            //1. Khai báo biến
            var timeDate = DateTime.Now;
            var contractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && x.Status == status);
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == contractCode.OrderId);

            //2. Check điều kiện
            if (contractCode == null)
            {
                return Json(new { status = MsgType.Error, msg = "Contract does not exist or is deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    contractCode.Status = (byte)ContractCodeType.Review;
                    contractCode.UpdateDate = timeDate;
                    contractCode.AccountantId = UserState.UserId;
                    contractCode.AccountantFullName = UserState.FullName;
                    contractCode.AccountantOfficeId = UserState.OfficeId;
                    contractCode.AccountantOfficeName = UserState.OfficeName;
                    contractCode.AccountantOfficeIdPath = UserState.OfficeIdPath;
                    contractCode.AccountantDate = timeDate;
                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();
                    //shop trung quốc phát hàng
                    order.Status = (byte)OrderStatus.AccountantProcessing;

                    var listOrderContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsync(x => x.OrderId == order.Id && x.OrderType == order.Type && !x.IsDelete);
                    order.PaidShop = listOrderContractCode.Sum(x => x.TotalPrice);

                    var priceBargain = order.TotalPrice + (order.FeeShip ?? 0) - (order.PaidShop ?? 0) + (order.FeeShipBargain ?? 0);

                    order.PriceBargain = priceBargain < 0 ? 0 : priceBargain;

                    order.ContractCodes = listOrderContractCode.Aggregate("", (current, item) => current + (current == "" ? "" : ",") + item.ContractCode);
                    order.LastUpdate = timeDate;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeDate,
                        Content = $"Trip Contract code #{contractCode.ContractCode} for order check, order check Order awaiting payment",
                        CustomerId = order.CustomerId.Value,
                        CustomerName = order.CustomerName,
                        OrderId = order.Id,
                        Status = order.Status,
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        Type = order.Type
                    });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                           $"{UserState.FullName} - {UserState.TitleName}: Request recheck Contract code #{contractCode.ContractCode}", EnumNotifyType.Warning,
                           $"{UserState.FullName} - {UserState.TitleName}: Request recheck Contract code #{contractCode.ContractCode}, Order #{order.Code}");

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeDate,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Resend Contract code #{contractCode.ContractCode} for ordering check",
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

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //4. Gửi thông tin lên view
            return Json(new { status = MsgType.Success, msg = "Transfer for ordering successful checkout!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thanh toán Contract code
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AccountantAwaitingPayment(int id, FundBillMeta model, byte status)
        {
            var timeDate = DateTime.Now;

            //1. Kiểm tra Contract code
            var contractCode = await UnitOfWork.OrderContractCodeRepo.FirstOrDefaultAsync(x => x.Id == id && x.Status == status);
            if (contractCode == null)
            {
                return Json(new { status = MsgType.Error, msg = "Contract does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == contractCode.OrderId);
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin Detail quỹ
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            model.Type = 1;//Chi tiền

            //3. Chặn không cho trừ quá Balances tồn tại trong quỹ
            if (model.Type == (byte)FundBillType.Diminishe)
            {
                if (model.CurrencyFluctuations > financeFundDetail.Balance)
                {
                    return Json(new { status = MsgType.Error, msg = ConstantMessage.CurrencyFluctuationsImpossible }, JsonRequestBehavior.AllowGet);
                }
            }

            //4. Kiểm tra User Details
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == order.CustomerId);
            if (customerDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Account does not exist or are being temporarily locked !" }, JsonRequestBehavior.AllowGet);
            }

            //5. Lấy thông tin Detail đối tượng là nhân viên thanh toán
            var userDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == UserState.UserId);
            if (userDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = ConstantMessage.CustomerIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //6. Lấy thông tin Detail Type định khoản
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Idd == (int)TreasureEnum.PayForShop && x.IsIdSystem == true);
            if (treasureDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Does not exist payment terms Shop (Idd = 600) !" }, JsonRequestBehavior.AllowGet);
            }

            //=========================== Khởi tạo Model để lưu
            model.Status = (byte)FundBillStatus.Approved;               // Cho xác nhận Approved vào quỹ

            model.Code = string.Empty;                                  // Khởi tạo code FundBill

            var fundBillOfDay = UnitOfWork.FundBillRepo.Count(x =>
                x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                x.Created.Day == DateTime.Now.Day);
            model.Code = $"{fundBillOfDay}{DateTime.Now:ddMMyy}";

            //=========================== Khởi tạo số dư ban đầu
            model.CurencyStart = null;
            model.CurencyEnd = null;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Thay đổi trạng thái Contract
                    contractCode.Status = (byte)ContractCodeType.Paid;
                    contractCode.UpdateDate = timeDate;
                    contractCode.AccountantId = UserState.UserId;
                    contractCode.AccountantFullName = UserState.FullName;
                    contractCode.AccountantOfficeId = UserState.OfficeId;
                    contractCode.AccountantOfficeName = UserState.OfficeName;
                    contractCode.AccountantOfficeIdPath = UserState.OfficeIdPath;
                    contractCode.AccountantDate = timeDate;

                    await UnitOfWork.OrderContractCodeRepo.SaveAsync();

                    var listContractCode = await UnitOfWork.OrderContractCodeRepo.FindAsNoTrackingAsync(
                                x =>
                                    !x.IsDelete && x.OrderId == order.Id && x.OrderType == order.Type &&
                                    x.Status == (byte)ContractCodeType.AwaitingPayment);

                    if (listContractCode.Count > 0)
                    {
                        //shop trung quốc phát hàng
                        if (order.Status < (byte)OrderStatus.DeliveryShop)
                        {
                            order.Status = (byte)OrderStatus.AccountantProcessing;
                        }
                        order.LastUpdate = timeDate;

                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = $"Payment finished Contract #{contractCode.ContractCode}, Order Being paid",
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
                    else
                    {
                        //shop trung quốc phát hàng
                        if (order.Status < (byte)OrderStatus.DeliveryShop)
                        {
                            order.Status = (byte)OrderStatus.OrderSuccess;
                        }
                        order.LastUpdate = timeDate;

                        if (order.Debt < 0)
                        {
                            var totalDebt = order.Debt * -1;
                            if (totalDebt > 100)
                            {
                                //cập nhật tiền hoàn cho khách
                                order.TotalRefunded = totalDebt;
                                order.Debt = 0;

                                var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                                {
                                    CustomerId = order.CustomerId.Value,
                                    CurrencyFluctuations = totalDebt,
                                    OrderId = order.Id,
                                    Note = $"Complete Excess money Order when the accountant pays for the goods!",
                                    TreasureIdd = (int)TreasureCustomerWallet.OrderReturn
                                });

                                // Lỗi trong quá tình thực hiện thanh toán
                                if (processRechargeBillResult.Status < 0)
                                {
                                    transaction.Rollback();
                                    return Json(new { status = MsgType.Error, msg = processRechargeBillResult.Msg }, JsonRequestBehavior.AllowGet);
                                }

                                // Thêm giao dịch trừ tiền
                                UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                                {
                                    Created = timeDate,
                                    Updated = timeDate,
                                    Currency = Currency.VND.ToString(),
                                    ExchangeRate = 0,
                                    IsDelete = false,
                                    Type = (byte)OrderExchangeType.Product,
                                    Mode = (byte)OrderExchangeMode.Import,
                                    ModeName = OrderExchangeType.Product.GetAttributeOfType<DescriptionAttribute>().Description,
                                    Note = $"Hoàn Excess money cho khách",
                                    OrderId = order.Id,
                                    TotalPrice = totalDebt,
                                    Status = (byte)OrderExchangeStatus.Approved
                                });

                                await UnitOfWork.OrderExchangeRepo.SaveAsync();
                            }
                        }

                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm lịch sử thay đổi trạng thái
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = $"Payment finished Contract #{contractCode.ContractCode}, Order has been paid",
                            CustomerId = order.CustomerId.Value,
                            CustomerName = order.CustomerName,
                            OrderId = order.Id,
                            Status = order.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = order.Type
                        });

                        await UnitOfWork.OrderHistoryRepo.SaveAsync();

                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            SystemId = order.SystemId,
                            SystemName = order.SystemName,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderId = order.Id,
                            OrderType = 0,
                            IsRead = false,
                            Title = Resource.DonHang + $"'{MyCommon.ReturnCode(order.Code)}' ได้สั่งสำเร็จ",
                            Description = Resource.DonHang + $" '{MyCommon.ReturnCode(order.Code)}' ได้สั่งสำเร็จ"
                        };
                        UnitOfWork.NotificationRepo.Add(notification);
                        await UnitOfWork.NotificationRepo.SaveAsync();
                    }

                    // Lưu thông tin phiếu nạp/trừ quỹ
                    var fundBillData = Mapper.Map<FundBill>(model);

                    // Map lại các thông tin để lưu
                    // Cập nhật lại số tiền vào phiếu trừ tiền
                    fundBillData.CurrencyFluctuations = (decimal)contractCode.TotalPrice;
                    fundBillData.Type = (byte)FundBillType.Diminishe;
                    fundBillData.CurencyStart = financeFundDetail.Balance;
                    fundBillData.CurencyEnd = financeFundDetail.Balance - contractCode.TotalPrice;

                    fundBillData.Diminishe = (decimal)contractCode.TotalPrice;
                    fundBillData.Increase = 0;

                    fundBillData.FinanceFundId = financeFundDetail.Id;
                    fundBillData.FinanceFundName = financeFundDetail.Name;
                    fundBillData.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillData.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillData.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillData.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillData.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillData.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    fundBillData.AccountantSubjectId = customerDetail.TypeId;
                    fundBillData.AccountantSubjectName = customerDetail.TypeName;

                    fundBillData.SubjectId = customerDetail.Id;
                    fundBillData.SubjectCode = customerDetail.Code;
                    fundBillData.SubjectName = customerDetail.FullName;
                    fundBillData.SubjectPhone = customerDetail.Phone;
                    fundBillData.SubjectEmail = customerDetail.Email;

                    fundBillData.TreasureId = treasureDetail.Id;
                    fundBillData.TreasureName = treasureDetail.Name;

                    //Lấy Type người dùng
                    fundBillData.AccountantSubjectId = customerDetail.TypeId;
                    fundBillData.AccountantSubjectName = customerDetail.TypeName;

                    //Cập nhật thêm thông tin Order
                    fundBillData.OrderId = order.Id;
                    fundBillData.OrderCode = order.Code;
                    fundBillData.OrderType = order.Type;

                    // Gắn thông tin người tạo
                    fundBillData.UserId = UserState.UserId;
                    fundBillData.UserName = UserState.UserName;
                    fundBillData.UserApprovalId = 0;
                    fundBillData.UserApprovalCode = string.Empty;
                    fundBillData.UserApprovalName = "[system]";
                    fundBillData.LastUpdated = timeDate;

                    fundBillData.Note +=
                        $" Thanh toán Contract: {contractCode.ContractCode} - Order: {MyCommon.ReturnCode(order.Code)}";

                    // Lưu lại vào database
                    UnitOfWork.FundBillRepo.Add(fundBillData);
                    UnitOfWork.FundBillRepo.Save();

                    //TODO: Cần thực hiện cập nhật số tiền vào quỹ tương ứng bị trừ tiền
                    financeFundDetail.Balance = financeFundDetail.Balance - contractCode.TotalPrice.Value;

                    UnitOfWork.FinaceFundRepo.Update(financeFundDetail);
                    UnitOfWork.FinaceFundRepo.Save();

                    // Gửi Notification lên system
                    NotifyHelper.CreateAndSendNotifySystemToClient(order.UserId.Value,
                            $"{UserState.FullName} - {UserState.TitleName}: Payment finished Contract code #{contractCode.ContractCode}", EnumNotifyType.Warning,
                            $"{UserState.FullName} - {UserState.TitleName}: Payment finished Contract code #{contractCode.ContractCode}, Order #{order.Code}");

                    //Ghi log thao tác
                    var orderLog = new OrderLog
                    {
                        OrderId = order.Id,
                        CreateDate = timeDate,
                        Type = (byte)OrderLogType.Acction,
                        DataBefore = null,
                        DataAfter = null,
                        Content = $"Thanh toán Contract code #{contractCode.ContractCode}",
                        UserId = UserState.UserId,
                        UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                        UserOfficeId = UserState.OfficeId,
                        UserOfficeName = UserState.OfficeName,
                        UserOfficePath = UserState.OfficeIdPath
                    };
                    UnitOfWork.OrderLogRepo.Add(orderLog);
                    await UnitOfWork.OrderLogRepo.SaveAsync();

                    var listOrderExchange = await UnitOfWork.OrderExchangeRepo.FindAsync(x =>
                                    !x.IsDelete && x.Status == (byte)OrderExchangeStatus.Approved &&
                                    x.Type == (byte)OrderExchangeType.Product && x.OrderId == order.Id);

                    var sum = listOrderExchange.Sum(x => x.TotalPrice);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = MsgType.Success, msg = "Pay Contract successful!" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAccountant(int orderId)
        {
            var listAccountantSubject = new List<SearchMeta>();
            // Lấy danh sách các Type đối tượng quỹ
            // TODO: Cần lấy danh sách từ bảng AccountantSubject
            var accountantSubject = await UnitOfWork.AccountantSubjectRepo.FindAsNoTrackingAsync(x => x.Id > 0);
            var tempaccountantSubjectList = from p in accountantSubject
                                            select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            listAccountantSubject.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listAccountantSubject.AddRange(tempaccountantSubjectList.ToList());

            var treasureTree = await GetTreasureJsTree();
            var curence = Currency.ALP.ToString();
            var financeFundTree = await GetFinanceFundJsAccountTree(curence);
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Id == orderId);
            var subject = new Customer();
            if (order != null)
            {
                subject = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == order.CustomerId);
            }

            return Json(new { listAccountantSubject, treasureTree, financeFundTree, subject }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Kế toán thanh toán hợp đồng]

        #region [Phân đơn hàng cho nhân viên xử lý]

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderNew, EnumPage.StockQuotesNew, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> AssignedOrder(int orderId, byte orderType, UserOfficeResult user, byte status)
        {
            //1. Lấy thông tin Order
            dynamic obj = new { status = (byte)MsgType.Success, msg = "Successfully assigned order to staff to handle!!" };

            //2. Kiểm tra dữ liệu
            switch (orderType)
            {
                //Đơn order
                case (byte)OrderType.Order:
                    obj = await OrderAssigned(orderId, orderType, user, status);
                    break;
                //Ticket find source
                case (byte)OrderType.StockQuotes:
                    obj = await SourseAssigned(orderId, orderType, user, status);
                    break;
                //Đơn ký gửi
                case (byte)OrderType.Deposit:
                    obj = await DepositAssigned(orderId, orderType, user, status);
                    break;
            }

            //3. Hiển thị lên view
            return Json(new { obj.status, obj.msg }, JsonRequestBehavior.AllowGet);
        }

        //Phân đơn order
        public async Task<dynamic> OrderAssigned(int orderId, byte orderType, UserOfficeResult user, byte status)
        {
            //1. Khởi tạo dữ liệu
            DateTime timeNow;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete && x.Status == status);

            //2. Kiểm tra điều kiện
            if (order == null)
            {
                return new { status = MsgType.Error, msg = "No order can longer be received!" };
            }
            timeNow = DateTime.Now;

            if (order.Status != (byte)OrderStatus.WaitOrder)
            {
                return new { status = MsgType.Error, msg = "Order can not be assigned to staff!" };
            }

            if (order.UserId != null)
            {
                return new { status = MsgType.Error, msg = "Order already have handlers!" };
            }

            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == user.Id && x.Status == (byte)OrderStatus.Order && !x.IsDelete);
            if (countOrder >= 40) // vượt quá Order number nhân viên đang xử lý
            {
                return new { status = MsgType.Error, msg = "Exceeds the number of divide for Staff handling!" };
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Kiểm tra Order đã có người nhận xử lý chưa
                    if (order.UserId == null)
                    {
                        order.UserId = user.Id;
                        order.UserName = UserState.UserName;
                        order.UserFullName = user.FullName;
                        order.OfficeId = user.OfficeId;
                        order.OfficeName = user.OfficeName;
                        order.OfficeIdPath = user.OfficeIdPath;
                    }

                    order.LastUpdate = timeNow;
                    order.Status = (byte)OrderStatus.Order;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    if (customer != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"Assign order to staff <b>{user.FullName}</b> to handle",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
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
                        Content = $"Assign order to staff <b>{user.FullName}</b> to handle",
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
            return new { status = MsgType.Success, msg = " Successfully assigned order to staff to handle!" };
        }

        //Phân Ticket find source
        public async Task<dynamic> SourseAssigned(int orderId, byte orderType, UserOfficeResult user, byte status)
        {
            //1. Khở tạo dữ liệu
            DateTime timeNow;
            var source = await UnitOfWork.SourceRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete && x.Status == status);

            //2. Kiểm tra điều kiện
            if (source == null)
            {
                return new { status = MsgType.Error, msg = "No longer can the Ticket find source be able to receive!" };
            }
            timeNow = DateTime.Now;

            if (source.Status != (byte)SourceStatus.WaitProcess)
            {
                return new { status = MsgType.Error, msg = "Ticket find source không thể phân cho nhân viên!" };
            }

            if (source.UserId != null)
            {
                return new { status = MsgType.Error, msg = "Ticket find source has a handle!" };
            }

            var countSource = await UnitOfWork.SourceRepo.CountAsync(x => x.UserId == user.Id && x.Status == (byte)SourceStatus.Process && !x.IsDelete);
            var countOrderSount = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == user.Id && x.Status == (byte)OrderStatus.Order && !x.IsDelete);
            if (countSource + countOrderSount >= 40) // vượt quá Order number nhân viên đang xử lý
            {
                return new { status = MsgType.Error, msg = "Exceeds the number of stools for Staff handling!" };
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Kiểm tra Order đã có người nhận xử lý chưa
                    if (source.UserId == null)
                    {
                        source.UserId = user.Id;
                        source.UserFullName = user.FullName;
                        source.OfficeId = user.OfficeId;
                        source.OfficeName = user.OfficeName;
                        source.OfficeIdPath = user.OfficeIdPath;
                    }

                    source.UpdateDate = timeNow;
                    source.Status = (byte)SourceStatus.Process;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == source.CustomerId && !x.IsDelete);

                    if (customer != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"Divide Order for staff <b>{user.FullName}</b> xử lý",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            OrderId = (int)source.Id,
                            Status = source.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = source.Type
                        });

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
            return new { status = MsgType.Success, msg = "Stacked Requests for Staff handlingg!" };
        }

        //Phân đơn ký gửi
        public async Task<dynamic> DepositAssigned(int orderId, byte orderType, UserOfficeResult user, byte status)
        {
            //1. Khở tạo dữ liệu
            DateTime timeNow;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete && x.Status == status);

            //2. Kiểm tra dữ liệu
            if (order == null)
            {
                return new { status = MsgType.Error, msg = "No order can be received!" };
            }
            timeNow = DateTime.Now;

            if (order.Status != (byte)DepositStatus.WaitDeposit)
            {
                return new { status = MsgType.Error, msg = "Order Can not allocate to staff!" };
            }

            if (order.UserId != null)
            {
                return new { status = MsgType.Error, msg = "Order  have been people handling!" };
            }

            var countOrder = await UnitOfWork.OrderRepo.CountAsync(x => x.UserId == user.Id && x.Status == (byte)DepositStatus.Processing && !x.IsDelete);
            if (countOrder >= 40) // vượt quá Order number nhân viên đang xử lý
            {
                return new { status = MsgType.Error, msg = "Exceeds the number of stools for Staff handling!" };
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Kiểm tra Order đã có người nhận xử lý chưa
                    if (order.UserId == null)
                    {
                        order.UserId = user.Id;
                        order.UserName = UserState.UserName;
                        order.UserFullName = user.FullName;
                        order.OfficeId = user.OfficeId;
                        order.OfficeName = user.OfficeName;
                        order.OfficeIdPath = user.OfficeIdPath;
                    }

                    order.LastUpdate = timeNow;
                    order.Status = (byte)DepositStatus.Processing;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == order.CustomerId && !x.IsDelete);

                    // Thêm lịch sử thay đổi trạng thái
                    if (customer != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = $"Assign order to staff <b>{user.FullName}</b> to handle",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
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
                        Content = $"Assign order to staff <b>{user.FullName}</b> to handle",
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
            return new { status = MsgType.Success, msg = " Successfully assigned order to staff to handle!" };
        }

        #endregion [Phân Order cho Staff handling]

        #region [Lấy danh sách nhân viên trong phòng]

        [HttpPost]
        public async Task<JsonResult> GetUserOffice()
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, 1, UserState.OfficeIdPath, UserState.OfficeId.Value);
            if (UserState.Type == 0)
            {
                if (UserState.OfficeType != (byte)OfficeType.CustomerCare && UserState.OfficeType != (byte)OfficeType.Order)
                {
                    listUser = listUser.Where(x => x.Type != 0).ToList();
                }
            }

            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy danh sách nhân viên trong phòng]

        #region [Danh sách hỗ trợ khiếu nại]

        [HttpPost]
        public async Task<JsonResult> GetAllSearchData()
        {
            var listStatusRefund = new List<dynamic>() { new { Text = "All", Value = -1 } };
            var listComplainStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
            var listComplainSystem = new List<dynamic>() { new { Text = "All", Value = -1 } };
            var listUserDetail = new List<dynamic>();
            var listUser = new List<SearchMeta>();

            // Lấy các trạng thái Status của Complain
            foreach (ComplainStatus ticketStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (ticketStatus >= 0)
                {
                    listComplainStatus.Add(new { Text = ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)ticketStatus });
                }
            }

            // Lấy danh sách System
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listComplainSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }
            //var listComplain = await UnitOfWork.ComplainRepo.FindAsync(s => !s.IsDelete);

            var count = 0/*listComplain.Count(item => CountUser(item.Id) > 0)*/;

            //Lấy ra danh sách nhân viên
            if (UserState.Type != null && UserState.OfficeId != null)
            {
                var listUserTemp = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, (byte)UserState.Type, UserState.OfficeIdPath, (int)UserState.OfficeId);
                var tempUserList = from p in listUserTemp
                                   select new SearchMeta() { Text = p.UserName + " - " + p.FullName, Value = p.Id };

                listUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
                listUser.AddRange(tempUserList.ToList());
            }
            //Lấy danh sách nhân viên hỗ trợ
            var listUserSelect = await UnitOfWork.UserRepo.FindAsync(s => s.Id != UserState.UserId && !s.IsDelete);
            foreach (var item in listUserSelect)
            {
                listUserDetail.Add(new
                {
                    Text = item.FullName + '-' + Position(item.Id),
                    Value = item.Id
                });
            }

            //Lấy danh sách trạng thái Refund
            foreach (ClaimForRefundStatus refundStatus in Enum.GetValues(typeof(ClaimForRefundStatus)))
            {
                if (refundStatus >= 0)
                {
                    listStatusRefund.Add(new { Text = refundStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)refundStatus });
                }
            }

            return Json(new { count, listComplainStatus, listComplainSystem, listUser, listUserDetail, listStatusRefund }, JsonRequestBehavior.AllowGet);
        }

        //Lấy về vị trí  công tác
        public string Position(int id)
        {
            var x = "";

            // var user = UnitOfWork.DbContext.UserPositions.SingleOrDefault(d => d.UserId == id && d.IsDefault);
            var user = UnitOfWork.DbContext.UserPositions.FirstOrDefault(d => d.UserId == id && d.IsDefault);
            if (user != null)
            {
                x = user.TitleName;
            }
            return x;
        }

        public int CountUser(long complainId)
        {
            var userId = UserState.UserId;
            var count = UnitOfWork.ComplainUserRepo.Find(d => d.ComplainId == complainId && d.UserId == userId && d.IsCare == false).Count();
            return count;
        }

        [HttpPost]
        public async Task<JsonResult> GetRenderSystemTab(string active)
        {
            var listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
            foreach (ComplainStatus complainStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (complainStatus != 0)
                    listStatus.Add(new { Value = (int)complainStatus, Text = complainStatus.GetAttributeOfType<DescriptionAttribute>().Description });
            }

            var listComplain = new List<SystemMeta>();
            var listcomplainuser = new List<TicketComplain>();

            if (active == "ticket-support")
            {
                listcomplainuser = UnitOfWork.ComplainRepo.SystemTicketSupport(UserState); ;
            }

            listComplain = listcomplainuser.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();

            //5. Lấy danh sách system
            var listSystem = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listComplain.Count,
                    ClassChild = "label-danger"
                }
            };
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    Total = listComplain.Count(x => x.SystemId == item.Id),
                    ClassChild = "label-primary"
                });
            }

            var listComplainSystem = new List<dynamic>() { new { Text = "All", Value = -1 } };
            var listSystemDb1 = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb1)
            {
                listComplainSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }
            return Json(new { listSystem, listStatus, listComplainSystem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetAllTicketListByStaff(int page, int pageSize, ComplainSearchModal searchModal)
        {
            //var ticketModal = new List<TicketComplain>();
            //var office = (byte)OfficeType.Order;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var dateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var dateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            var ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketSupportOfficeList(out totalRecord, page,
                pageSize, searchModal.Keyword, searchModal.Status,
                searchModal.SystemId, dateStart, dateEnd, UserState);

            return Json(new { totalRecord, ticketModal }, JsonRequestBehavior.AllowGet);
        }

        #region CHI TIẾT KHIẾU NẠI

        #region [Lấy danh sách nhân viên trong phòng chăm sóc khách hàng]

        [HttpPost]
        public async Task<JsonResult> GetUserComplain()
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, 3, UserState.OfficeIdPath, UserState.OfficeId.Value);

            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        #endregion [Lấy danh sách nhân viên trong phòng chăm sóc khách hàng]

        [HttpPost]
        public async Task<JsonResult> GetTicketDetail(int ticketId)
        {
            var customer = new Customer();
            var complainuserlist = new List<ComplainDetail>();
            var list = new List<ComplainDetail>();
            var usersupportlist = new List<User>();
            var usersupport = new User();
            var ticketModal = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(p => p.Id == ticketId);
            //var complainuser = UnitOfWork.DbContext.ComplainUsers.Where(s => s.ComplainId == ticketId).OrderBy(b=>b.CreateDate);
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == ticketId
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );

            //Lọc Staff handling
            var complainusersupport = (from d in UnitOfWork.DbContext.ComplainUsers
                                       where (d.ComplainId == ticketId && d.IsCare == false)
                                       orderby d.CreateDate
                                       select new { d.UserId, d.UserName }).Distinct().ToList();
            //Sum complainusser theo complainId
            foreach (var item in complainuser)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {
                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    if (posit != null)
                    {
                        userPosition = posit.TitleName;
                    }
                }

                complainuserlist.Add(new ComplainDetail
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = item.ComplainId,
                    Content = item.Content,
                    AttachFile = item.AttachFile,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    UserRequestId = item.UserRequestId,
                    UserRequestName = item.UserRequestName,
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    IsRead = item.IsRead,
                    IsCare = item.IsCare
                });
            }

            //Lọc người xử lý(hỗ trợ)
            foreach (var item in complainusersupport)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {
                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    userPosition = posit.TitleName;
                }
                list.Add(new ComplainDetail
                {
                    Id = 0,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = 0
                });
            }

            //Lấy ra nhân viên chịu trách nhiệm chính
            var complainusermain = complainuserlist.SingleOrDefault(s => s.IsCare == true);
            var hascustomer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(p => p.Id == ticketModal.CustomerId);
            customer = hascustomer;
            //}

            return Json(new { ticketModal, customer, complainuserlist, complainusermain, list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllUser()
        {
            var listUser =
                UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" })
                    .ToList();
            return Json(new { listUser }, JsonRequestBehavior.AllowGet);
        }

        //Phản hồi cho khách hàng

        [HttpPost]
        public JsonResult feedbackComplain(string content, int complainId)
        {
            var com = UnitOfWork.ComplainUserRepo.FirstOrDefault(d => d.ComplainId == complainId && d.UserId == UserState.UserId);
            var complainback = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            var complain = new ComplainUser
            {
                UserId = UserState.UserId,
                UserName = UserState.UserName
            };
            if (com != null)
            {
                complain.UserRequestId = com.UserRequestId;
                complain.UserRequestName = com.UserRequestName;
            }
            complain.Content = content;
            complain.ComplainId = complainId;
            complain.CreateDate = DateTime.Now;
            complain.CustomerId = complainback.CustomerId;
            complain.CustomerName = complainback.CustomerName;
            UnitOfWork.ComplainUserRepo.Add(complain);
            UnitOfWork.ComplainUserRepo.Save();

            return Json(complainback, JsonRequestBehavior.AllowGet);
        }

        //Hoàn thành ticket
        [HttpPost]
        public JsonResult finishComplain(int complainId)
        {
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            complain.Status = (byte)ComplainStatus.Success;
            complain.LastUpdateDate = DateTime.Now;
            UnitOfWork.ComplainRepo.Save();
            return Json(complain, JsonRequestBehavior.AllowGet);
        }

        #endregion CHI TIẾT KHIẾU NẠI

        #endregion [Danh sách hỗ trợ khiếu nại]

        [HttpPost]
        public async Task<JsonResult> GetId(string code, byte type)
        {
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Code == code && x.Type == type && !x.IsDelete);

            return order == null ? Json(new { status = MsgType.Error, msg = "Order does not exist or is deleted!" }, JsonRequestBehavior.AllowGet) : Json(new { status = MsgType.Success, msg = "", id = order.Id }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetType(string code)
        {
            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Code == code && !x.IsDelete);

            return order == null ? Json(new { status = MsgType.Error, msg = "Order does not exist or is deleted!" }, JsonRequestBehavior.AllowGet) : Json(new { status = MsgType.Success, msg = "", type = order.Type, id = order.Id }, JsonRequestBehavior.AllowGet);
        }

        //Lấy danh sách yêu cầu Refund
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund, EnumPage.OrderClaimForRefund)]
        public async Task<JsonResult> GetClaimForRefundList(int page, int pageSize, ClaimForRefundSearchModal searchModal)
        {
            List<ClaimForRefund> claimForRefundModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ClaimForRefundSearchModal();
            }

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword)
                    && (UserState.Type > 0 || x.OrderUserId == UserState.UserId)
                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.OrderUserId == searchModal.UserId)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword)
                    && (UserState.Type > 0 || x.OrderUserId == UserState.UserId)
                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            //lấy list OrderId
            var listOrderId = claimForRefundModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listOrder = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName }).ToList();

            //lấy listComplainId
            var listComplainId = claimForRefundModal.Select(x => x.TicketId).ToList();
            //Lấy danh sách Complain
            var listComplain = UnitOfWork.ComplainUserRepo.Entities.Where(x => listComplainId.Contains((int)x.ComplainId) && x.IsCare == true).Select(x => new { x.Id, x.ComplainId, x.UserId, x.UserName }).ToList();
            return Json(new { totalRecord, claimForRefundModal, listOrder, listComplain }, JsonRequestBehavior.AllowGet);
        }

        #region [Thống kê Tổng tiền mac ca đơn hàng]

        //Thong ke tong tien mac ca theo nhan vien
        [HttpPost]
        public async Task<JsonResult> GetTotalMoneyReportAllDay(DateTime? startDay, DateTime? endDay, bool type)
        {
            var statusOrdered = (byte)OrderStatus.OrderSuccess;
            var statusFinished = (byte)OrderStatus.Finish;
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);
            var ordered = new List<CustomerUser>();
            if (type)
            {
                ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportAllDay(listUser, startDay, endDay, statusFinished);
            }
            else
            {
                ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportAllDay(listUser, startDay, endDay, statusOrdered);
            }

            //1. Tạo các dữ liệu theo báo cáo
            var detailNameordered = new List<string>();
            var detailUserordered = new List<int>();
            var detailPriceordered = new List<dynamic>();

            foreach (var or in ordered)
            {
                detailNameordered.Add(or.FullName);
                detailUserordered.Add(or.TotalCusstomer);
                detailPriceordered.Add(or.TotalMoney);
            }

            //2. Trả kết quả lên view
            return Json(new { detailNameordered, detailUserordered, detailPriceordered }, JsonRequestBehavior.AllowGet);
        }

        //Thong ke tong tien mac ca theo nhan vien
        [HttpPost]
        public async Task<JsonResult> GetTotalMoneyReportDay(DateTime? startDay, bool type)
        {
            var statusOrdered = (byte)OrderStatus.OrderSuccess;
            var statusFinished = (byte)OrderStatus.Finish;
            //1. Tạo các dữ liệu theo báo cáo
            var detailNameordered = new List<string>();
            var detailUserordered = new List<int>();
            var detailPriceordered = new List<dynamic>();

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = new List<CustomerUser>();
            if (type)
            {
                ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportDay(listUser, startDay, statusFinished);
            }
            else
            {
                ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportDay(listUser, startDay, statusOrdered);
            }
            foreach (var or in ordered)
            {
                detailNameordered.Add(or.FullName);
                detailUserordered.Add(or.TotalCusstomer);
                detailPriceordered.Add(or.TotalMoney);
            }
            //2. Trả kết quả lên view
            return Json(new { detailNameordered, detailUserordered, detailPriceordered }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Thống kê Tổng tiền mac ca đơn hàng]

        #region [Xuất Excel Thống kê Tổng tiền đơn hàng]

        //Xuất Excel danh sách tong tien thanh toan cho shop theo nhân viên trong ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelGetTotalMoneyReportDay(string titleExcel, DateTime? startDay, bool all, bool status)
        {
            var statusOrdered = (byte)OrderStatus.OrderSuccess;
            var statusFinished = (byte)OrderStatus.Finish;

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportDay(listUser, startDay, statusOrdered);
            var finished = UnitOfWork.OrderRepo.GetTotalMoneyReportDay(listUser, startDay, statusFinished);
            if (!status)
            {
                return CommonCustomerReport(titleExcel, startDay, null, all, ordered);
            }
            else
                return CommonCustomerReport(titleExcel, startDay, null, all, finished);
        }

        //Export Excel danh sách tong tien thanh toan cho shop theo nhân viên tat ca ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelGetTotalMoneyReportAllDay(string titleExcel, DateTime? startDay, DateTime? endDay, bool all, bool status)
        {
            var statusOrdered = (byte)OrderStatus.OrderSuccess;
            var statusFinished = (byte)OrderStatus.Finish;

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalMoneyReportAllDay(listUser, startDay, endDay, statusOrdered);
            var finished = UnitOfWork.OrderRepo.GetTotalMoneyReportAllDay(listUser, startDay, endDay, statusFinished);

            if (!status)
            {
                return CommonCustomerReport(titleExcel, startDay, endDay, all, ordered);
            }
            else
                return CommonCustomerReport(titleExcel, startDay, endDay, all, finished);
        }

        public FileContentResult CommonCustomerReport(string titleExcel, DateTime? startDay, DateTime? endDay, bool all, List<CustomerUser> listUserExcel)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Account name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Gender", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Birthday", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Email", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Phone", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Date start work", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Account creation date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order number", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Total money", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title

                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleExcel, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = startDay?.ToShortDateString() ?? "__";
                var end = endDay?.ToShortDateString() ?? "__";

                // biến all=true : lấy tất cả, all = false: Lấy theo ngày
                if (all == true)
                {
                    ngay = "from the: " + start + " to date " + end;
                }
                else
                {
                    //ngay = DateTime.Now.ToLongDateString();
                    ngay = start;
                }
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });

                #endregion Title

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.TypeName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<SexCustomer>(w.Gender), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Birthday, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Email, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Phone, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.StartDate?.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalCusstomer, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalMoney, new CustomExcelStyle
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

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_STAFF{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        #endregion [Xuất Excel Thống kê Tổng tiền đơn hàng]

        #region [Thống kê tiền đơn hàng theo đầu nhân viên]

        //Tất cả các ngày
        [HttpPost]
        public async Task<JsonResult> GetTotalPriceBargainReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);
            //var type = (byte)BargainType.BusinessBargain;
            var ordered = UnitOfWork.OrderRepo.GetTotalPriceBargainReportAllDay(listUser, startDay, endDay);

            //1. Tạo các dữ liệu theo báo cáo
            var detailNameordered = new List<string>();
            var detailUserordered = new List<int>();
            var detailPriceordered = new List<dynamic>();

            foreach (var or in ordered)
            {
                detailNameordered.Add(or.FullName);
                detailUserordered.Add(or.TotalCusstomer);
                detailPriceordered.Add(or.TotalMoney);
            }

            //2. Trả kết quả lên view
            return Json(new { detailNameordered, detailUserordered, detailPriceordered }, JsonRequestBehavior.AllowGet);
        }

        //Thong ke tong tien don hang theo nhan vien
        [HttpPost]
        public async Task<JsonResult> GetTotalPriceBargainReportDay(DateTime? startDay)
        {
            //var type = (byte)BargainType.BusinessBargain;

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalPriceBargainReportDay(listUser, startDay);

            //1. Tạo các dữ liệu theo báo cáo
            var detailNameordered = new List<string>();
            var detailUserordered = new List<int>();
            var detailPriceordered = new List<dynamic>();

            foreach (var or in ordered)
            {
                detailNameordered.Add(or.FullName);
                detailUserordered.Add(or.TotalCusstomer);
                detailPriceordered.Add(or.TotalMoney);
            }

            //2. Trả kết quả lên view
            return Json(new { detailNameordered, detailUserordered, detailPriceordered }, JsonRequestBehavior.AllowGet);
        }

        //Thong ke tong tien don hang ky gui theo nhan vien
        [HttpPost]
        public async Task<JsonResult> GetTotalDepositBargainReportDay(DateTime? startDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Deposit, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalPriceDepositReportDay(startDay);

            //2. Trả kết quả lên view
            return Json(new { ordered, listUser }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetTotalDepositBargainReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Deposit, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalPriceDepositReportAllDay(startDay, endDay);

            //2. Trả kết quả lên view
            return Json(new { ordered, listUser }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Thống kê tiền đơn hàng theo đầu nhân viên]

        #region [Xuất Excel Thống kê tiền  đơn hàng theo đầu nhân viên]

        //Xuất Excel danh sách mặc cả shop theo nhân viên trong ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelGetTotalPriceBargainReportDay(string titleExcel, DateTime? startDay, bool all)
        {
            //var type = (byte)BargainType.NotBargain;
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalPriceBargainReportDay(listUser, startDay);
            return CommonCustomerReport(titleExcel, startDay, null, all, ordered);
        }

        //Export Excel danh sách bargain shop theo nhân viên tat ca ngày
        [HttpPost]
        public async Task<FileContentResult> ExcelGetTotalPriceBargainReportAllDay(string titleExcel, DateTime? startDay, DateTime? endDay, bool all)
        {
            //var type = (byte)BargainType.NotBargain;
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalPriceBargainReportAllDay(listUser, startDay, endDay);
            return CommonCustomerReport(titleExcel, startDay, endDay, all, ordered);
        }

        #endregion [Xuất Excel Thống kê tiền  đơn hàng theo đầu nhân viên]

        #region [Xuất Excel Thống kê tình hình đơn hàng theo thời gian]

        //Tình hình đơn hàng
        [HttpPost]
        public async Task<FileContentResult> ExcelReportProfitOrder(string titleExcel, DateTime? startDay, DateTime? endDay)
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalProfitReportAllDay(startDay, endDay, UserState);
            return CommonProfitOrderReport(titleExcel, startDay, endDay, ordered);
        }

        //Lợi nhuận bargain theo thời gian
        [HttpPost]
        public async Task<FileContentResult> ExcelReportProfitOrderFinished(string titleExcel, DateTime? startDay, DateTime? endDay)
        {
            var status = (byte)OrderStatus.Finish;
            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.Order, UserState);

            var ordered = UnitOfWork.OrderRepo.GetTotalProfitBargainReportAllDay(startDay, endDay, UserState, status);
            return CommonProfitOrderReport(titleExcel, startDay, endDay, ordered);
        }

        public FileContentResult CommonProfitOrderReport(string titleExcel, DateTime? startDay, DateTime? endDay, List<ProfitDay> listUserExcel)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "No.", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Date", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order number", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Total revenue", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Total profit", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title

                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, titleExcel, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = startDay?.ToShortDateString() ?? "__";
                var end = endDay?.ToShortDateString() ?? "__";

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });

                #endregion Title

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalOrder, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalMoney, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalBargain, new CustomExcelStyle
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

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;
                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"ORDER_CREATEDDATE{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        #endregion [Xuất Excel Thống kê tình hình đơn hàng theo thời gian]

        #region [Thống kê lợi nhuận theo thời gian]

        [HttpPost]
        public JsonResult GetTotalProfitReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var ordered = UnitOfWork.OrderRepo.GetTotalProfitReportAllDay(startDay, endDay, UserState);

            //1. Tạo các dữ liệu theo báo cáo
            var totalOrder = new List<int>();
            var totalBargain = new List<dynamic>();
            var totalMoney = new List<dynamic>();
            var day = new List<string>();

            foreach (var or in ordered)
            {
                day.Add(or.Created);
                totalOrder.Add(or.TotalOrder);
                totalBargain.Add(or.TotalBargain);
                totalMoney.Add(or.TotalMoney);
            }
            //2. Trả kết quả lên view
            return Json(new { day, totalBargain, totalMoney, totalOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTotalProfitBargainReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var status = (byte)OrderStatus.Finish;
            var ordered = UnitOfWork.OrderRepo.GetTotalProfitBargainReportAllDay(startDay, endDay, UserState, status);

            //1. Tạo các dữ liệu theo báo cáo
            var totalOrder = new List<int>();
            var totalBargain = new List<dynamic>();
            var totalMoney = new List<dynamic>();
            var day = new List<string>();

            foreach (var or in ordered)
            {
                day.Add(or.Created);
                totalOrder.Add(or.TotalOrder);
                totalBargain.Add(or.TotalBargain);
            }
            //2. Trả kết quả lên view
            return Json(new { day, totalBargain, totalMoney, totalOrder }, JsonRequestBehavior.AllowGet);
        }

        #endregion [Thống kê lợi nhuận theo thời gian]

        //hoàn thành Order
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.OrderSuccess)]
        public JsonResult GetOrderSuccess(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart, DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = UnitOfWork.OrderRepo.GetOrderSuccess(out totalRecord, page, pageSize, keyword, status, systemId, dateStart, dateEnd, userId, customerId, checkExactCode);
            //var listOrderId = listOrder.Select(x => x.Id).ToList();
            //var listHistory =
            //    UnitOfWork.DbContext.OrderHistories.Where(
            //        x =>
            //            ((x.Type != (byte) OrderType.Deposit && x.Status == (byte) OrderStatus.GoingDelivery) ||
            //             (x.Type == (byte) OrderType.Deposit && x.Status == (byte) DepositStatus.GoingDelivery)) &&
            //            listOrderId.Contains(x.OrderId)).ToList();

            //foreach (var item in listOrder)
            //{
            //    var firstOrDefault = listHistory.FirstOrDefault(x => x.OrderId == item.Id);
            //    if (firstOrDefault != null)
            //        item.Created = firstOrDefault.CreateDate;
            //}

            //4. Gửi dữ liệu lên view
            return Json(new { totalRecord, listOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.OrderSuccess)]
        public async Task<JsonResult> OrderFinish(int id)
        {
            //1. Khởi tạo dữ liệu
            var timeNow = DateTime.Now;
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete && ((x.Type != (byte)OrderType.Deposit && x.Status == (byte)OrderStatus.GoingDelivery) || (x.Type == (byte)OrderType.Deposit && x.Status == (byte)DepositStatus.GoingDelivery)));

            //2. Kiểm tra điều kiện
            if (order == null)
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Thao tác với dữ liệu
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Cập nhật trạng thái Order
                    order.LastUpdate = timeNow;

                    if (order.Type == (byte)OrderType.Deposit)
                        order.Status = (byte)DepositStatus.Finish;
                    else
                        order.Status = (byte)OrderStatus.Finish;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //cập nhật số tiền đã tiêu cho khách.
                    var customer = UnitOfWork.CustomerRepo.SingleOrDefault(x => x.Id == order.CustomerId);

                    customer.Balance += order.Total;
                    var level = UnitOfWork.OrderRepo.GetCustomerLevel(customer.Balance);

                    customer.LevelId = (byte)level.Id;
                    customer.LevelName = level.Name;

                    await UnitOfWork.CustomerRepo.SaveAsync();

                    // Thêm lịch sử thay đổi trạng thái
                    UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                    {
                        CreateDate = timeNow,
                        Content = $"Complete order",
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
                        Content = $"Complete order",
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
            return Json(new { status = MsgType.Success, msg = "Order successfully completed!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> OrderCodeWarehouse(int idWarehouse, string code)
        {
            var w = await UnitOfWork.OfficeRepo.FirstAsNoTrackingAsync(x => x.Id == idWarehouse);
            return Json(w != null ? new { code = $"{w.Code}-{code}" } : new { code }, JsonRequestBehavior.AllowGet);
        }
    }
}