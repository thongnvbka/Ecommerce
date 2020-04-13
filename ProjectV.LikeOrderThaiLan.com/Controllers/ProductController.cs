using AutoMapper;
using Common.Emums;
using Common.Helper;
using Common.Host;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using Library.Models;
using Library.ViewModels.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Controllers
{
    [RoutePrefix("{culture}/Product")]
    public class ProductController : BaseController
    {
        //[Route("current-tool-version")]
        public string VersionTool()
        {
            return "1.0";
        }

        [Authorize]
        //[Route("gio-hang")]
        public ActionResult Cart()
        {
            ViewBag.VipLevel = JsonConvert.SerializeObject(UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId),
                new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });

            return View();
        }

        [Route("Districts/{provinceId}")]
        public async Task<ActionResult> Districts(int provinceId)
        {
            var districts = await UnitOfWork.DistrictRepo.FindAsNoTrackingAsync(x => x.ProvinceId == provinceId);

            var district2 = new List<dynamic>() { new { Id = "", Text = "Chọn Quận/Huyện", Selected = true } };
            district2.AddRange(
                districts.Select(x => new { Id = x.Id.ToString(), Text = x.Name, Selected = false }).ToList());

            return JsonCamelCaseResult(district2, JsonRequestBehavior.AllowGet);
        }

        [Route("Wards/{districtId}")]
        public async Task<ActionResult> Wards(int districtId)
        {
            var wards = await UnitOfWork.WardRepo.FindAsNoTrackingAsync(x => x.DistrictId == districtId);

            var wards2 = new List<dynamic>() { new { Id = "", Text = "Chọn Phường/Xã", Selected = true } };
            wards2.AddRange(wards.Select(x => new { Id = x.Id.ToString(), Text = x.Name, Selected = false }).ToList());

            return JsonCamelCaseResult(wards2, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [Route("Deposit/{orderIds}")]
        public async Task<ActionResult> Deposit(string orderIds)
        {
            //lay kho hang ve
            var list = new List<Office>();
            //list.Add(new Office { Id = -1, Name = "--Chọn kho hàng về--" });
            var allWarehouseDelivery = UnitOfWork.OfficeRepo.FindAsNoTracking(
                 x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use && x.Culture == "VN").ToList();
            list.AddRange(allWarehouseDelivery);
            ViewBag.ListWardDelivery = list;
            var modelResult = new OrderDepositViewModel()
            {
                TypeLevel = "0"
            };

            //phan Tuan lam
            ViewBag.OrderIds = orderIds;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (orderIds.Count(x => x == ';') == 2)
            {
                var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                    x => !x.IsDelete && x.CustomerId == CustomerState.Id && orderIds.Contains(";" + x.Id + ";") &&
                         x.Status == (byte)OrderStatus.New);

                if (order == null)
                    return RedirectToAction("BuyOrder", "Order", new { Area = "CMS" });

                modelResult.WarehouseDeliveryName = CustomerState.WarehouseName;
                modelResult.WarehouseDeliveryId = CustomerState.WarehouseId;
                //phan Tuan lam
                ViewBag.Order = order;
                ViewBag.OrderDetails =
                    await UnitOfWork.OrderDetailRepo.FindAsNoTrackingAsync(x => x.OrderId == order.Id && !x.IsDelete);
                ViewBag.OrderServices =
                    await
                        UnitOfWork.OrderServiceRepo.FindAsNoTrackingAsync(
                            x => !x.IsDelete && x.Checked && x.OrderId == order.Id);

                // Đơn hàng chứa link hàng mua ít hơn yêu cầu tối thiểu của Shop
                List<OrderDetail> orderDetails;

                if (CheckMinQuantityOrders(new List<Order> { order }, out orderDetails) == false)
                {
                    TempData["orderDetails"] =
                        JsonConvert.SerializeObject(
                            orderDetails.Select(x => new { x.Name, x.BeginAmount, x.ShopName }).ToList().Distinct(),
                            jsonSerializerSettings);

                    return RedirectToAction("Cart", "Product");
                }
            }
            else
            {
                List<OrderDetail> orderDetails;

                var orders = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(
                    x => !x.IsDelete && x.CustomerId == CustomerState.Id && x.Status == (byte)OrderStatus.New &&
                         orderIds.Contains(";" + x.Id + ";"));

                if (!orders.Any())
                    return RedirectToAction("BuyOrder", "Order", new { Area = "CMS" });

                // Đơn hàng chứa link hàng mua ít hơn yêu cầu tối thiểu của Shop
                if (CheckMinQuantityOrders(orders, out orderDetails) == false)
                {
                    TempData["orderDetails"] = JsonConvert.SerializeObject(
                        orderDetails.Select(x => new { x.Name, x.BeginAmount, x.ShopName }).ToList().Distinct(),
                        jsonSerializerSettings);

                    return RedirectToAction("Cart", "Product");
                }
                modelResult.WarehouseDeliveryName = CustomerState.WarehouseName;
                modelResult.WarehouseDeliveryId = CustomerState.WarehouseId;

                ViewBag.Orders = orders;
            }

            var user = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == CustomerState.Id);

            ViewBag.Address = user.Address;
            //không lấy địa chỉ quận huyện
            //ViewBag.ProvinceAndDistrict = $"{user.DistrictName}, {user.ProvinceName}";
            ViewBag.ProvinceAndDistrict = "";
            ViewBag.Phone = user.Phone;

            var provinces = await UnitOfWork.ProvinceRepo.FindAsNoTrackingAsync(x => x.Culture == CustomerState.Culture);
            ViewBag.Provinces =
                provinces.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

            ViewBag.VipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId);

            return View(modelResult);
        }

        private bool CheckMinQuantityOrders(List<Order> orders, out List<OrderDetail> productNames)
        {
            var allow = true;

            var productNameErrors = new List<OrderDetail>();

            foreach (var order in orders)
            {
                var products = UnitOfWork.OrderDetailRepo.Find(x => x.OrderId == order.Id && x.IsDelete == false).ToList();

                foreach (var product in products)
                {
                    var productDetails = products.Where(x => x.Link == product.Link).ToList();

                    var totalQuantity = productDetails.Sum(x => x.Quantity);
                    if (product.BeginAmount.HasValue && totalQuantity < product.BeginAmount.Value)
                    {
                        productNameErrors.Add(product);
                        allow = false;
                    }
                }
            }

            productNames = productNameErrors.Distinct().ToList();

            return allow;
        }

        [Authorize]
        [Route("Deposit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Deposit(OrderDepositViewModel model)
        {
            var isAjaxRequest = Request.IsAjaxRequest();

            if (!ModelState.IsValid)
            {
                if (!isAjaxRequest)
                    goto HasError;

                var errorMessage = string.Join(", ",
                    ModelState.Values.SelectMany(x => x.Errors.Select(m => m.ErrorMessage)));

                return JsonCamelCaseResult(new { Status = -1, Text = errorMessage },
                    JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var timeNow = DateTime.Now;

                    var orders = await UnitOfWork.OrderRepo.FindAsync(
                        x => !x.IsDelete && x.CustomerId == CustomerState.Id && x.Status == (byte)OrderStatus.New &&
                             model.OrderIds.Contains(";" + x.Id + ";"));

                    // Đơn hàng không tồn tại
                    if (orders == null || !orders.Any())
                    {
                        if (isAjaxRequest)
                        {
                            return JsonCamelCaseResult(
                                new { Status = -2, Text = Resource.DHKhongTonTaiHoacBiXoa },
                                //new { Status = -2, Text = "Đơn hàng không tồn tại hoặc đã bị xóa" },
                                JsonRequestBehavior.AllowGet);
                        }

                        ModelState.AddModelError("Order", Resource.DHKhongTonTaiHoacBiXoa);
                        //ModelState.AddModelError("Order", "Đơn hàng không tồn tại hoặc đã bị xóa");
                        goto HasError;
                    }

                    foreach (var order in orders)
                    {
                        var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                            x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                        // Tài khoản đang bị khóa
                        if (customer == null)
                        {
                            if (isAjaxRequest)
                            {
                                return
                                    JsonCamelCaseResult(
                                        new
                                        {
                                            Status = -3,
                                            Text = Resource.TKCuaBanDangBiKhoa
                                            //Text = "Tải khoản của bạn đang bị khóa không thể thực hiện giao dịch"
                                        },
                                        JsonRequestBehavior.AllowGet);
                            }

                            ModelState.AddModelError("Account", Resource.TKCuaBanDangBiKhoa);
                            //ModelState.AddModelError("Account", "Tải khoản của bạn đang bị khóa không thể thực hiện giao dịch");
                            goto HasError;
                        }

                        // Tính lại số tiền đơn hàng trong tỷ số hiện tại
                        order.CustomerName = CustomerState.FullName;
                        order.CustomerEmail = CustomerState.Email;
                        order.CustomerPhone = CustomerState.Phone;
                        //Thêm địa chỉ kho hàng về
                        order.WarehouseDeliveryId = model.WarehouseDeliveryId;
                        order.WarehouseDeliveryName = model.WarehouseDeliveryName;

                        var exchangeRate = ExchangeRate();

                        // Tỷ giá hiện tại khác với tỷ giá lúc đặt hàng
                        if (order.ExchangeRate != exchangeRate)
                        {
                            // Cập nhật lại tỷ giá và thành tiền của sản phẩm
                            var orderDetails = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == order.Id
                                                                                               && x.IsDelete == false);

                            foreach (var orderDetail in orderDetails)
                            {
                                orderDetail.ExchangeRate = exchangeRate;
                                orderDetail.ExchangePrice = orderDetail.Price * exchangeRate;
                                orderDetail.TotalExchange = orderDetail.TotalPrice * exchangeRate;
                            }

                            order.TotalExchange = order.TotalPrice * order.ExchangeRate;

                            //                            // Cập nhật tiền dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                            //                            var totalPrice = order.TotalExchange *
                            //                                             OrderRepository.OrderPrice(order.ServiceType, order.TotalExchange) / 100;
                            //
                            //                            // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                            //                            if (order.TotalExchange < 2000000)
                            //                            {
                            //                                totalPrice = 150000;
                            //                            }

                            //var orderService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            //            x =>
                            //                x.OrderId == order.Id && !x.IsDelete &&
                            //                x.ServiceId == (byte)OrderServices.Order);

                            //orderService.Value = OrderRepository.OrderPrice(order.ServiceType, order.TotalExchange);
                            //orderService.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                            //// Triết khấu phí mua hàng
                            //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                            //if (discount > 0)
                            //{
                            //    orderService.TotalPrice -= orderService.TotalPrice * discount / 100;
                            //    orderService.Note =
                            //        $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                            //        //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                            //}

                            // Cập nhật tiền dịch vụ kiểm đếm
                            var orderAuditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Audit && x.Checked);

                            if (orderAuditService != null)
                            {
                                var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                    .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null)
                                    .SumAsync(x => x.AuditPrice.Value * x.Quantity);

                                orderAuditService.Value = totalAuditPrice;
                                orderAuditService.TotalPrice = totalAuditPrice;
                            }

                            // Cập nhật tổng iteefn đơn hàng
                            order.Total = orderAuditService == null
                                ? order.TotalExchange /*+ orderService.TotalPrice*/
                                : order.TotalExchange /*+ orderService.TotalPrice*/ + orderAuditService.TotalPrice;
                        }

                        // Kiểm tra số tiền đặt cọc
                        order.LevelId = customer.LevelId;
                        order.LevelName = customer.LevelName;
                        order.Created = timeNow;
                        order.LastUpdate = timeNow;
                        order.Status = (byte)OrderStatus.WaitOrder;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(customer.LevelId);
                        //Kiem tra dat coc 100 hoac theo level
                        if (model.TypeLevel == "1")
                        {
                            vipLevel.Deposit = 100;
                            // Tài khoản không đủ số tiền để đặt cọc đơn hàng này
                            if (customer.BalanceAvalible < order.TotalExchange)
                            {
                                if (isAjaxRequest)
                                {
                                    return
                                        JsonCamelCaseResult(
                                            new
                                            {
                                                Status = -11,
                                                Text = Resource.KhongDuTienDatCoc
                                                //Text = "Tài khoản của bạn không đủ số tiền để đặt cọc đơn hàng này"
                                            },
                                            JsonRequestBehavior.AllowGet);
                                }

                                ModelState.AddModelError("BalanceAvalible",
                                    Resource.TKCuaBanKhongDu + (order.TotalExchange).ToString("N2", CultureInfo) +
                                    Resource.Currency + " " + Resource.DeDatCoc +
                                    Resource.TKCuaBanConThieu +
                                    (order.TotalExchange - customer.BalanceAvalible).ToString("N2", CultureInfo) +
                                    Resource.Currency + " " + Resource.DeDCThanhCong);
                                //  $"บัญชีของคุณไม่เพียงพอที่  {(order.TotalExchange).ToString("N2", CultureInfo)} (BAHT) để đặt cọc. " +
                                //$"Tài khoản của bạn không đủ {(order.TotalExchange).ToString("N2", CultureInfo)} (VND) để đặt cọc. " +
                                //  $"การขาดดุลบัญชีของคุณที่จะสามารถฝาก {(order.TotalExchange - customer.BalanceAvalible).ToString("N2", CultureInfo)} (BAHT) ความสำเร็จ");
                                //$"Tài khoản của bạn còn thiếu {(order.TotalExchange - customer.BalanceAvalible).ToString("N2", CultureInfo)} (BAHT) để có thể đặt cọc thành công");
                                goto HasError;
                            }
                        }
                        else
                        {
                            // Tài khoản không đủ số tiền để đặt cọc đơn hàng này
                            if (customer.BalanceAvalible < (order.TotalExchange * vipLevel.Deposit / 100))
                            {
                                if (isAjaxRequest)
                                {
                                    return
                                        JsonCamelCaseResult(
                                            new
                                            {
                                                Status = -11,
                                                Text = Resource.KhongDuTienDatCoc
                                                //Text = "Tài khoản của bạn không đủ số tiền để đặt cọc đơn hàng này"
                                            },
                                            JsonRequestBehavior.AllowGet);
                                }

                                ModelState.AddModelError("BalanceAvalible",
                                      Resource.TKCuaBanKhongDu + (order.TotalExchange * vipLevel.Deposit / 100).ToString("N2", CultureInfo) +
                                    Resource.Currency + " " + Resource.DeDatCoc +
                                    Resource.TKCuaBanConThieu +
                                    (order.TotalExchange * vipLevel.Deposit / 100 - customer.BalanceAvalible).ToString("N2", CultureInfo) +
                                    Resource.Currency + " " + Resource.DeDCThanhCong);

                                //$"Tài khoản của bạn không đủ {(order.TotalExchange * vipLevel.Deposit / 100).ToString("N2", CultureInfo)} (BAHT) để đặt cọc. " +
                                //    $"Tài khoản của bạn còn thiếu {(order.TotalExchange * vipLevel.Deposit / 100 - customer.BalanceAvalible).ToString("N2", CultureInfo)} (BAHT) để có thể đặt cọc thành công");
                                goto HasError;
                            }
                        }

                        // Tạo phiếu trừ tiền tài khoản khách hàng
                        //var rechargeBill = new RechargeBill()
                        //{
                        //    Code = string.Empty,
                        //    Created = timeNow,
                        //    LastUpdated = timeNow,
                        //    IsDelete = false,
                        //    CurencyStart = customer.BalanceAvalible,
                        //    Status = (byte)RechargeBillStatus.Approved,
                        //    CurrencyFluctuations = order.Total * vipLevel.Deposit / 100,
                        //    CustomerId = order.CustomerId,
                        //    CustomerName = order.CustomerName,
                        //    CustomerEmail = order.CustomerEmail,
                        //    CustomerPhone = order.CustomerPhone,
                        //    CustomerAddress = order.CustomerAddress,
                        //    CustomerCode = customer.Code,
                        //    Type = (byte)RechargeBillType.Diminishe,
                        //    UserName = "[Hệ thống]",
                        //    UserCode = "SYSTEM",
                        //    UserApprovalCode = "SYSTEM",
                        //    UserApprovalName = "[Hệ thống]",
                        //    TreasureId = 0,
                        //    TreasureIdd = 0,
                        //    IsAutomatic = false,
                        //    OrderId = order.Id,
                        //    OrderCode = order.Code,
                        //    OrderType = order.Type,
                        //    Note = $"{order.CustomerName} ({order.CustomerEmail}) đặt cọc đơn hàng #ORD{order.Code}"
                        //};

                        //tao thong bao cho tai khoan 

                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            OrderId = order.Id,
                            SystemId = order.SystemId,
                            SystemName = order.SystemName,
                            CustomerId = order.CustomerId,
                            CustomerName = order.CustomerName,
                            CreateDate = timeNow,
                            UpdateDate = timeNow,
                            OrderType = 1, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = "คำสั่งซื้อเงินฝากที่ประสบความสำเร็จ",
                            Description = $"คำสั่งซื้อเงินฝากที่ประสบความสำเร็จ #ORD {order.Code}"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);
                        await UnitOfWork.NotificationRepo.SaveAsync();


                        // Tiền đặt cọc
                        var depositMoney = order.TotalExchange * vipLevel.Deposit / 100;

                        //customer.BalanceAvalible -= depositMoney;

                        // Thanh toán tiền
                        var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                        {
                            CustomerId = customer.Id,
                            CurrencyFluctuations = depositMoney,
                            OrderId = order.Id,
                            Note = Resource.TruTienDCDonHang + $"﻿ ORD{order.Code}",
                            //Note = $"Trừ tiền đặt cọc đơn hàng ORD{order.Code}",
                            TreasureIdd = (int)TreasureCustomerWallet.AdvanceOrder
                        });

                        // Lỗi trong quá tình thực hiện thanh toán
                        if (processRechargeBillResult.Status < 0)
                        {
                            if (isAjaxRequest)
                            {
                                return JsonCamelCaseResult(
                                    new { Status = processRechargeBillResult.Status + (-100), Text = processRechargeBillResult.Msg },
                                    JsonRequestBehavior.AllowGet);
                            }

                            ModelState.AddModelError("ProcessRechargeBill", processRechargeBillResult.Msg);
                            goto HasError;
                        }

                        // Thêm giao dịch trong đơn hàng
                        UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            Currency = Currency.VND.ToString(),
                            ExchangeRate = exchangeRate,
                            IsDelete = false,
                            Type = (byte)OrderExchangeType.Product,
                            Mode = (byte)OrderExchangeMode.Export,
                            ModeName = OrderExchangeType.Product.GetAttributeOfType<DescriptionAttribute>().Description,
                            //Note = $"Đặt cọc tiền hàng {vipLevel.Deposit}% đương đương với {depositMoney.ToString("N2", CultureInfo)}VND",
                            Note = Resource.ThanhToanLucDCDH,
                            //Note = "Thanh toán lúc đặt cọc đơn hàng",
                            OrderId = order.Id,
                            TotalPrice = depositMoney > order.TotalExchange ? order.TotalExchange : depositMoney,
                            Status = (byte)OrderExchangeStatus.Approved
                        });

                        // Tiền đặt cọc nhiều hơn tiền hàng -> Chia tiền đặt cọc ra tiền thanh toán tiền mua hàng hộ
                        if (depositMoney > order.TotalExchange)
                        {
                            UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                            {
                                Created = timeNow,
                                Updated = timeNow,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = exchangeRate,
                                IsDelete = false,
                                Type = (byte)OrderExchangeType.Order,
                                Mode = (byte)OrderExchangeMode.Export,
                                ModeName =
                                    OrderExchangeType.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                                //Note = $"Đặt cọc tiền hàng {vipLevel.Deposit}% đương đương với {depositMoney.ToString("N2", CultureInfo)}VND",
                                Note = Resource.ThanhToanLucDCDH,
                                //Note = "Thanh toán lúc đặt cọc đơn hàng",
                                OrderId = order.Id,
                                TotalPrice = depositMoney - order.TotalExchange,
                                Status = (byte)OrderExchangeStatus.Approved
                            });
                        }

                        // Thêm lịch sử thay đổi trạng thái
                        // Todo: Giỏi bổ xung thêm loại đơn hàng để phân biệt đơn hàng order, ký gửi, tìm nguồn
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = "มัดจำ",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            OrderId = order.Id,
                            Status = order.Status,
                            Type = order.Type
                        });
                        UnitOfWork.OrderHistoryRepo.Save();

                        var toAddress = new OrderAddress();
                        // Địa chỉ nhận không giống địa chỉ của tài khoản hiện tại
                        if (model.IsOtherAddress)
                        {
                            //var province = await
                            //    UnitOfWork.ProvinceRepo.SingleOrDefaultAsync(
                            //        x => x.Id == model.ProvinceId && x.Culture == CustomerState.Culture);

                            // Tỉnh/Thành phố không tồn tại

                            //Todo xoa di do ben thai lan khong co quan huyen 

                            //if (province == null)
                            //{
                            //    if (isAjaxRequest)
                            //    {
                            //        return JsonCamelCaseResult(
                            //            new { Status = -4, Text = "Tỉnh/Thành phố không tồn tại hoặc đã bị xóa" },
                            //            JsonRequestBehavior.AllowGet);
                            //    }

                            //    ModelState.AddModelError("ProvinceId", @"Tỉnh/Thành phố không tồn tại hoặc đã bị xóa");
                            //    goto HasError;
                            //}

                            //var district = await
                            //    UnitOfWork.DistrictRepo.SingleOrDefaultAsync(
                            //        x => x.Id == model.DistrictId && x.Culture == CustomerState.Culture);

                            // Quận/Huyện không tồn tại

                            //Todo xoa di do ben thai lan khong co quan huyen 

                            //if (district == null)
                            //{
                            //    if (isAjaxRequest)
                            //    {
                            //        return
                            //            JsonCamelCaseResult(
                            //                new { Status = -5, Text = "Quận/Huyện không tồn tại hoặc đã bị xóa" },
                            //                JsonRequestBehavior.AllowGet);
                            //    }

                            //    ModelState.AddModelError("DistrictId", @"Quận/Huyện không tồn tại hoặc đã bị xóa");
                            //    goto HasError;
                            //}

                            // Quận/Huyện không đúng với Tỉnh/TP

                            //Todo xoa di do ben thai lan khong co quan huyen 

                            //if (district.ProvinceId != province.Id)
                            //{
                            //    if (isAjaxRequest)
                            //    {
                            //        return JsonCamelCaseResult(new
                            //        {
                            //            Status = -4,
                            //            Text =
                            //                $"Tỉnh/Thành phố \"{province.Name}\" không có Quận/Huyện \"{district.Name}\""
                            //        },
                            //            JsonRequestBehavior.AllowGet);
                            //    }

                            //    ModelState.AddModelError("DistrictId",
                            //        $"Tỉnh/Thành phố \"{province.Name}\" không có Quận/Huyện \"{district.Name}\"");
                            //    goto HasError;
                            //}

                            //var ward = await
                            //    UnitOfWork.WardRepo.SingleOrDefaultAsync(
                            //        x => x.Id == model.WardId && x.Culture == CustomerState.Culture);

                            // Xã/Phường không tồn tại

                            //Todo xoa di do ben thai lan khong co quan huyen 

                            //if (ward == null)
                            //{
                            //    if (isAjaxRequest)
                            //    {
                            //        return
                            //            JsonCamelCaseResult(
                            //                new { Status = -6, Text = "Xã/Phường không tồn tại hoặc đã bị xóa" },
                            //                JsonRequestBehavior.AllowGet);
                            //    }

                            //    ModelState.AddModelError("WardId", "Xã/Phường không tồn tại hoặc đã bị xóa");
                            //    goto HasError;
                            //}

                            // Xã/Phường không đúng với Quận/Huyện

                            //Todo xoa di do ben thai lan khong co quan huyen 

                            //if (ward.DistrictId != district.Id)
                            //{
                            //    if (isAjaxRequest)
                            //    {
                            //        return JsonCamelCaseResult(new
                            //        {
                            //            Status = -7,
                            //            Text = $"Quận/Huyện \"{district.Name}\" không có Xã/Phường \"{ward.Name}\""
                            //        }, JsonRequestBehavior.AllowGet);
                            //    }

                            //    ModelState.AddModelError("WardId",
                            //        "Quận/Huyện \"{district.Name}\" không có Xã/Phường \"{ward.Name}\"");
                            //    transaction.Rollback();
                            //    goto HasError;
                            //}

                            toAddress.Address = model.Address;
                            toAddress.Phone = model.Phone;
                            //không lấy địa chỉ quận huyện
                            toAddress.ProvinceId = 0;//province.Id;
                                                     //                                        //toAddress.ProvinceName = province.Name;
                            toAddress.ProvinceName = model.Address;
                            toAddress.DistrictId = 0; //district.Id;
                            //                                         //toAddress.DistrictName = district.Name;
                            toAddress.DistrictName = model.Address;
                            toAddress.WardId = 0; //ward.Id;
                            toAddress.WardName = model.Address; //ward.Name;
                            toAddress.FullName = model.FullName;
                        }
                        else
                        {
                            toAddress.Address = customer.Address;
                            toAddress.Phone = customer.Phone;
                            toAddress.ProvinceId = 0;
                            //không lấy địa chỉ quận huyện
                            //toAddress.ProvinceName = customer.ProvinceName;
                            toAddress.ProvinceName = customer.Address;
                            toAddress.DistrictId = 0;
                            //toAddress.DistrictName = customer.DistrictName;
                            toAddress.DistrictName = customer.Address;
                            toAddress.WardId = 0;
                            toAddress.WardName = customer.WardsName;
                            toAddress.FullName = customer.FullName;
                        }

                        // Tạo địa chỉ người đặt hàng
                        var fromAddress = new OrderAddress()
                        {
                            Address = customer.Address,
                            Phone = customer.Phone,
                            ProvinceId = 0,
                            //ProvinceName = customer.Address,
                            ProvinceName = customer.Address,
                            DistrictId = 0,
                            DistrictName = customer.Address,
                            //DistrictName = customer.DistrictName,
                            WardId = 0,
                            WardName = customer.Address,
                            FullName = customer.FullName
                        };

                        UnitOfWork.OrderAddressRepo.Add(fromAddress);
                        UnitOfWork.OrderAddressRepo.Add(toAddress);

                        await UnitOfWork.OrderAddressRepo.SaveAsync();

                        //// Cập nhật lại mã của phiếu nap tiền: rechargeBill
                        //var rechargeBillOfDay = UnitOfWork.RechargeBillRepo.Count(x =>
                        //    x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                        //    x.Created.Day == DateTime.Now.Day && x.Id <= rechargeBill.Id);

                        //rechargeBill.Code = $"{rechargeBillOfDay}{DateTime.Now:ddMMyy}";

                        order.FromAddressId = fromAddress.Id;
                        order.ToAddressId = toAddress.Id;
                        order.DepositPercent = vipLevel.Deposit;
                        order.TotalPayed = depositMoney;
                        order.Debt = order.Total - order.TotalPayed;

                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Tính công nợ tiền phải thu
                        var autoUpdateDebitModel = new AutoUpdateDebitModel()
                        {
                            SubjectId = order.CustomerId ?? 0, // Id khách hàng
                            SubjectTypeIdd = (int)EnumAccountantSubject.Customer,
                            Money = order.Total - depositMoney, // Số tiền
                            OrderId = order.Id,                 // Id đơn hàng
                            PayReceivableIdd = (int)TreasureMustCollect.AdvanceOrder, // Đinh khoản (Enum TreasureMustReturn)
                            OrderType = order.Type,
                            OrderCode = order.Code
                        };

                        var result = UnitOfWork.DebitRepo.UpdateDebit(autoUpdateDebitModel);

                        // Lỗi trong quá tình thực hiện công nợ
                        if (result.Status < 0)
                        {
                            ModelState.AddModelError("AutoUpdateDebitModel", result.Msg);
                            goto HasError;
                        }
                    }

                    transaction.Commit();

                    if (orders.Count > 1)
                    {
                        TempData["Msg"] =
                           Resource.DatCocThanhCongDH + $" \"<b>{string.Join(", ", orders.Select(x => "#" + x.Code))}</b>\"";
                        //$"Đặt cọc thành công đơn hàng \"<b>{string.Join(", ", orders.Select(x => "#" + x.Code))}</b>\"";
                    }
                    else
                    {
                        TempData["Msg"] = Resource.DatCocThanhCongDH + $"#\"<b>{orders.First().Code}</b>\"";
                        //TempData["Msg"] = $"Đặt cọc thành công đơn hàng #\"<b>{orders.First().Code}</b>\"";
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            // Khách hàng này còn đơn hàng chờ đặt cọc -> Chuyển sang link giỏ hàng
            var hasShopingCart =
                await UnitOfWork.OrderRepo.AnyAsync(x => !x.IsDelete && x.Type == (byte)OrderType.Order &&
                                                         x.Status == (byte)OrderStatus.New &&
                                                         x.CustomerId == CustomerState.Id);

            if (isAjaxRequest)
            {
                if (hasShopingCart)
                {
                    return JsonCamelCaseResult(new
                    {
                        Status = 1,
                        Text = Url.Action("Cart", "Product")
                    }, JsonRequestBehavior.AllowGet);
                }

                //Chuyển qua link tài khoản
                //todo: Chờ làm phần này chưa biết link của tài khoản là gì
                return JsonCamelCaseResult(new
                {
                    Status = 1,
                    Text = Url.Action("Cart")
                }, JsonRequestBehavior.AllowGet);
            }

            if (hasShopingCart)
            {
                return RedirectToAction("Cart", "Product");
            }

            //Chuyển qua link tài khoản
            return RedirectToAction("BuyOrder", "Order", new { area = "Cms" });

            // Có lỗi trong quá trình xử lý
            HasError:
            var orders2 = await UnitOfWork.OrderRepo.FindAsNoTrackingAsync(
                x => !x.IsDelete && x.CustomerId == CustomerState.Id && x.Status == (byte)OrderStatus.New &&
                     model.OrderIds.Contains(";" + x.Id + ";"));

            // Đơn hàng không tồn tại
            if (orders2 == null || !orders2.Any())
            {
                throw new HttpException(404, Resource.DHKhongTonTaiHoacBiXoa);
                //throw new HttpException(404, "Đơn hàng không tồn tại");
            }

            if (orders2.Count == 1)
            {
                var order = orders2.First();
                ViewBag.Order = order;
                ViewBag.OrderDetails =
                    await UnitOfWork.OrderDetailRepo.FindAsNoTrackingAsync(x => x.OrderId == order.Id && !x.IsDelete);
                ViewBag.OrderServices =
                    await
                        UnitOfWork.OrderServiceRepo.FindAsNoTrackingAsync(
                            x => !x.IsDelete && x.Checked && x.OrderId == order.Id);
            }
            else
            {
                ViewBag.Orders = orders2;
            }

            var user = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == CustomerState.Id);

            ViewBag.Address = user.Address;
            //không lấy địa chỉ quận huyện
            //ViewBag.ProvinceAndDistrict = $"{user.DistrictName}, {user.ProvinceName}";
            ViewBag.ProvinceAndDistrict = "";
            ViewBag.Phone = user.Phone;

            var provinces = await UnitOfWork.ProvinceRepo.FindAsNoTrackingAsync(x => x.Culture == CustomerState.Culture);
            ViewBag.Provinces =
                provinces.Select(x => new SelectListItem() { Value = x.Id.ToString(), Text = x.Name }).ToList();

            ViewBag.VipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(user.LevelId);
            //lay kho hang ve
            var list = new List<Office>();
            //list.Add(new Office { Id = -1, Name = "--Chọn kho hàng về--" });
            var allWarehouseDelivery = UnitOfWork.OfficeRepo.FindAsNoTracking(
                 x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use && x.Culture == "VN").ToList();
            list.AddRange(allWarehouseDelivery);
            ViewBag.ListWardDelivery = list;
            return View(model);
        }

        //[Authorize]
        //[Route("shoping-cart")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> AddOrder(List<ShopingCartViewModel> shopingCarts)
        {
            //Không có shoping cart update to database
            if (shopingCarts == null)
                return JsonCamelCaseResult(await GetShopingCart(CustomerState.Id), JsonRequestBehavior.AllowGet);

            if (!ModelState.IsValid)
            {
                var messages = string.Join(", ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));

                return JsonCamelCaseResult(new { Status = -1, Text = Resource.DinhDangDuLieuKhongDung + $": { messages}" },
                //return JsonCamelCaseResult(new { Status = -1, Text = $"Định dạng dữ liệu không đúng: {messages}" },
                    JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var dateTime = DateTime.Now;
                    foreach (var shopingCart in shopingCarts)
                    {
                        // Tìm shop mới
                        var shop =
                            await
                                UnitOfWork.ShopRepo.SingleOrDefaultAsync(
                                    x => x.Url == shopingCart.ShopLink && !x.IsDelete);

                        // Thêm shop mới
                        if (shop == null)
                        {
                            shop = new Shop()
                            {
                                Name = shopingCart.ShopName?.Trim() ?? "",
                                Url = shopingCart.ShopLink,
                                Website = MyCommon.GetDomain(shopingCart.ShopLink),
                                CreateDate = dateTime,
                                UpdateDate = dateTime,
                            };

                            UnitOfWork.ShopRepo.Add(shop);

                            // Submit thêm Shop
                            await UnitOfWork.ShopRepo.SaveAsync();
                        }

                        // Thêm order
                        var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => !x.IsDelete && x.CustomerId == CustomerState.Id &&
                                 x.Status == (byte)OrderStatus.New && x.ShopId == shop.Id);

                        //lấy thông tin khách hàng
                        var cus = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == CustomerState.Id);

                        var orderIsNew = false;
                        if (order == null)
                        {
                            orderIsNew = true;
                            order = new Order()
                            {
                                Code = string.Empty,
                                Type = (byte)OrderType.Order,
                                WebsiteName = shop.Website,
                                ShopId = shop.Id,
                                ShopName = shop.Name?.Trim() ?? "",
                                ShopLink = shop.Url,
                                PackageNo = 0,
                                ContractCode = string.Empty,
                                ContractCodes = string.Empty,
                                LevelId = CustomerState.LevelId,
                                LevelName = CustomerState.LevelName,
                                CreatedTool = (byte)CreatedTool.Extension,
                                Currency = Currency.CNY.ToString(),
                                ExchangeRate = ExchangeRate(),
                                WarehouseId = 0,
                                WarehouseName = string.Empty,
                                WarehouseDeliveryId = cus.WarehouseId,
                                WarehouseDeliveryName = cus.WarehouseName,
                                CustomerId = CustomerState.Id,
                                CustomerName = CustomerState.FullName,
                                CustomerEmail = CustomerState.Email,
                                CustomerPhone = CustomerState.Phone,
                                Status = (byte)OrderStatus.New,
                                OrderInfoId = 0,
                                FromAddressId = 0,
                                ToAddressId = 0,
                                SystemId = SystemId,
                                SystemName = SystemName,
                                ServiceType = (byte)ServicePack.Business,
                                Note = shopingCart.Note,
                                PrivateNote = shopingCart.PrivateNote,
                                Created = dateTime,
                                LastUpdate = dateTime,
                            };

                            UnitOfWork.OrderRepo.Add(order);

                            // Submit thêm order
                            await UnitOfWork.OrderRepo.SaveAsync();

                            // Cập nhật lại Mã cho đơn hàng
                            var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == cus.Id && x.Id <= order.Id);
                            var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == cus.Id);
                            order.Code = $"{customer.Code}-{orderNo}";
                            // Submit thêm order
                            await UnitOfWork.OrderRepo.SaveAsync();
                        }

                        // Thêm chi tiết Order
                        var hasAddProduct = false;
                        var addedProducts = new List<string>();
                        var dicProLinks = new Dictionary<string, List<PriceMeta>>();

                        var orderDetails = new List<OrderDetail>();

                        foreach (var product in shopingCart.Products)
                        {
                            //if()
                            var property = JsonConvert.SerializeObject(product.Propeties);
                            var prices = product.Prices == null || !product.Prices.Any() ?
                                null : JsonConvert.SerializeObject(product.Prices);

                            var uniqueCode = Encryptor.Base64Encode(property + product.Link);

                            var p = orderIsNew
                                ? null
                                : await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(
                                    x => x.OrderId == order.Id && x.IsDelete == false && x.UniqueCode == uniqueCode);

                            // Bỏ qua sản phẩm đã tồn tại và sản phẩm trùng link & thuộc tính
                            //if (p != null || addedProducts.Any(x => x == uniqueCode)) continue;

                            if (p != null)
                            {
                                p.IsDelete = true;
                                p.LastUpdate = DateTime.Now;
                            }
                            else if (addedProducts.Any(x => x == uniqueCode))
                            {
                                continue;
                            }

                            p = new OrderDetail()
                            {
                                OrderId = order.Id,
                                UniqueCode = uniqueCode,
                                Name = product.Name,
                                Image = product.Image,
                                Quantity = product.Quantity,
                                QuantityBooked = product.Quantity,
                                BeginAmount = product.BeginAmount,
                                Price = product.Price,
                                ExchangeRate = ExchangeRate(),
                                ExchangePrice = product.Price * ExchangeRate(),
                                TotalPrice = product.Price * product.Quantity,
                                TotalExchange = (product.Price * ExchangeRate()) * product.Quantity,
                                Note = product.Note,
                                Status = (byte)OrderDetailStatus.Order,
                                Link = product.Link,
                                Properties = property,
                                Prices = prices,
                                Min = product.Min,
                                Max = product.Max,
                                SkullId = product.SkullId,
                                ProId = product.ProId,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };

                            hasAddProduct = true;
                            addedProducts.Add(uniqueCode);

                            orderDetails.Add(p);

                            // Không có khoảng giá theo số lượng
                            if (string.IsNullOrWhiteSpace(p.Prices) || string.IsNullOrWhiteSpace(p.ProId))
                                continue;

                            if (dicProLinks.ContainsKey(p.Link))
                            {
                                dicProLinks[p.Link] = product.Prices;
                                continue;
                            }

                            dicProLinks.Add(p.Link, product.Prices);
                        }

                        UnitOfWork.OrderDetailRepo.AddRange(orderDetails);

                        // Submit thêm sản phẩm
                        if (hasAddProduct)
                            await UnitOfWork.OrderDetailRepo.SaveAsync();

                        // Cập nhật lại tiền tính kếm đếm của đơn hàng
                        var totalQuantityProduct = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && x.IsDelete == false).Select(x => x.Quantity)
                            .ToList().Sum(x => x);

                        if (dicProLinks.Any())
                        {
                            foreach (var dicProLink in dicProLinks)
                            {
                                var productDetails = await UnitOfWork.OrderDetailRepo.Entities.Where(
                                        x => x.OrderId == order.Id && x.IsDelete == false && x.Link == dicProLink.Key)
                                    .ToListAsync();

                                var totalQuantity = productDetails.Sum(x => x.Quantity);

                                var price =
                                    dicProLink.Value.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin)
                                                                        ||
                                                                        (totalQuantity >= x.Begin &&
                                                                         totalQuantity <= x.End));

                                if (price == null)
                                    continue;

                                foreach (var pd in productDetails)
                                {
                                    pd.Price = price.Price;
                                    pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                    pd.TotalPrice = pd.Price * pd.Quantity;
                                    pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.Quantity;
                                    pd.AuditPrice = OrderRepository.OrderAudit(totalQuantityProduct, pd.Price);
                                }
                            }

                            await UnitOfWork.OrderDetailRepo.SaveAsync();
                        }

                        order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalExchange);
                        order.UnsignName = MyCommon.Ucs2Convert(
                            $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone}");

                        order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalPrice);

                        order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                            .CountAsync(x => x.OrderId == order.Id && !x.IsDelete);

                        order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.Quantity);

                        // Submit cập nhật order
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                        //var totalExchange = order.TotalExchange;
                        //                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;
                        //
                        //                        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        //                        if (order.TotalExchange < 2000000)
                        //                        {
                        //                            totalPrice = 150000;
                        //                        }

                        //var orderServcie = orderIsNew
                        //    ? null
                        //    : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);

                        #region Thêm các dịch vụ cho đơn hàng

                        //// DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                        //if (orderServcie == null)
                        //{
                        //    orderServcie = new OrderService()
                        //    {
                        //        OrderId = order.Id,
                        //        ServiceId = (byte)OrderServices.Order,
                        //        ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                        //        ExchangeRate = ExchangeRate(),
                        //        IsDelete = false,
                        //        Created = dateTime,
                        //        LastUpdate = dateTime,
                        //        HashTag = string.Empty,
                        //        Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange),
                        //        Currency = Currency.VND.ToString(),
                        //        Type = (byte)UnitType.Percent,
                        //        //TotalPrice = totalPrice < 5000 ? 5000 : totalPrice,
                        //        TotalPrice = 0,
                        //        Mode = (byte)OrderServiceMode.Required,
                        //        Checked = true
                        //    };

                        //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                        //}
                        //else // Cập nhật dịch vụ mua hàng
                        //{
                        //    orderServcie.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //    orderServcie.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;
                        //}

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    orderServcie.TotalPrice -= orderServcie.TotalPrice * discount / 100;
                        //    orderServcie.Note =
                        //        $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                        //        //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
                        var orderShopShippingService = orderIsNew
                            ? null
                            : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x =>
                                    x.OrderId == order.Id && !x.IsDelete &&
                                    x.ServiceId == (byte)OrderServices.ShopShipping);

                        if (orderShopShippingService == null)
                        {
                            orderShopShippingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.ShopShipping,
                                ServiceName =
                                    (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                Value = 0,
                                Currency = Currency.CNY.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = false,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };
                            UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                        }

                        // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                        var autditService = orderIsNew
                            ? null
                            : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Audit);

                        if (autditService == null)
                        {
                            autditService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Audit,
                                ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = false,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };
                            UnitOfWork.OrderServiceRepo.Add(autditService);
                        }
                        else
                        {
                            var totalAuditPrice = UnitOfWork.OrderDetailRepo.Find(
                                x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                     && x.Status == (byte)OrderDetailStatus.Order)
                                .Sum(x => (x.AuditPrice ?? 0) * x.Quantity);

                            autditService.Value = totalAuditPrice;
                            autditService.TotalPrice = totalAuditPrice;
                        }

                        // DỊCH VỤ ĐÓNG KIỆN HÀNG HÓA --------------------------------------------------------------------------
                        var packingService = orderIsNew
                            ? null
                            : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.Packing);

                        if (packingService == null)
                        {
                            packingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.Packing,
                                ServiceName =
                                    OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                Value = 0,
                                Currency = Currency.CNY.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = false,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };
                            UnitOfWork.OrderServiceRepo.Add(packingService);
                        }
                        // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                        var outSideShippingService = orderIsNew
                            ? null
                            : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.OutSideShipping);

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = true,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };
                            UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                        }

                        //// DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                        //var fastDeliveryService = orderIsNew
                        //    ? null
                        //    : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => x.OrderId == order.Id && !x.IsDelete &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery);

                        //if (fastDeliveryService == null)
                        //{
                        //    fastDeliveryService = new OrderService()
                        //    {
                        //        OrderId = order.Id,
                        //        ServiceId = (byte)OrderServices.FastDelivery,
                        //        ServiceName =
                        //            OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
                        //        ExchangeRate = ExchangeRate(),
                        //        Value = 0,
                        //        Currency = Currency.VND.ToString(),
                        //        Type = (byte)UnitType.Money,
                        //        TotalPrice = 0,
                        //        Mode = (byte)OrderServiceMode.Option,
                        //        Checked = false,
                        //        Created = dateTime,
                        //        LastUpdate = dateTime
                        //    };
                        //    UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);
                        //}

                        // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                        var shipToHomeService = orderIsNew
                            ? null
                            : await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == order.Id && !x.IsDelete &&
                                     x.ServiceId == (byte)OrderServices.InSideShipping);

                        if (shipToHomeService == null)
                        {
                            shipToHomeService = new OrderService()
                            {
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.InSideShipping,
                                ServiceName =
                                    OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Money,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Required,
                                Checked = true,
                                Created = dateTime,
                                LastUpdate = dateTime
                            };
                            UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                        }

                        #endregion Thêm các dịch vụ cho đơn hàng

                        // Submit thêm OrderService
                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật số lượng tổng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                            x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.TotalRefunded = 0;
                        order.Debt = totalService + order.TotalExchange;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    OutputLog.WriteOutputLog(ex);
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(await GetShopingCart(CustomerState.Id), JsonRequestBehavior.AllowGet);
        }

        private async Task<List<ShopingCartViewModel>> GetShopingCart(int customerId)
        {
            var dateTime = DateTime.Now.AddDays(-30);

            // Lấy ra các đơn hàng từ 30 ngày trở lại
            var orders = await UnitOfWork.OrderRepo.FindAsync(
                x =>
                    x.CustomerId == customerId && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.New &&
                    x.Created >= dateTime && !x.IsDelete,
                query => query.OrderByDescending(m => m.Created), null);

            // List shopingCart sau khi thêm vào database
            var shopingCarts = new List<ShopingCartViewModel>();

            var index = 0;
            foreach (var order in orders)
            {
                // ShopingCart sau khi thêm vào database
                var newShopingCart = new ShopingCartViewModel();

                Mapper.Map(order, newShopingCart);
                newShopingCart.ShowDetail = index == 0;

                // Dịch vụ sau khi thêm vào database
                var services = await UnitOfWork.OrderServiceRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);
                newShopingCart.Services = services.Select(Mapper.Map<Service>).ToList();

                // Lấy lại Product sau khi thêm vào database
                var products = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == order.Id && !x.IsDelete);
                newShopingCart.Products = products.Select(Mapper.Map<Product>).ToList();

                shopingCarts.Add(newShopingCart);
                index += 1;
            }

            return shopingCarts;
        }

        [Authorize]
        //[Route("cap-nhat-san-pham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProduct(UpdateProductViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1000, Text = "Dữ liệu truyền lên không đúng định dạng" },
                    JsonRequestBehavior.AllowGet);

            if (!await UnitOfWork.OrderDetailRepo.CheckCustomerHasUpdateProductShoppingCart(model.Id, CustomerState.Id,
                OrderStatus.New))
                return JsonCamelCaseResult(new { Status = -200, Text = "Bạn không có quyền cập nhật sản phẩm này" },
                    JsonRequestBehavior.AllowGet);

            var product = await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);

            if (product == null)
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.SPKhongTonTaiHoacBiXoa },
                //return JsonCamelCaseResult(new { Status = -1, Text = "Sản phẩm này không tồn tại hoặc đã bị xóa" },
                    JsonRequestBehavior.AllowGet);

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == product.OrderId && x.IsDelete == false);

            if (order == null)
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.DHKhongTonTaiHoacBiXoa },
                //return JsonCamelCaseResult(new { Status = -1, Text = "Đơn hàng không tồn tại hoặc đã bị xóa" },
                    JsonRequestBehavior.AllowGet);

            if (model.Quantity > product.Max)
            {
                return JsonCamelCaseResult(
                    new { Status = -2, Text = Resource.SoLuongSanPhamShopConLa + $" {product.Max}" },
                    //new { Status = -2, Text = $"Số lượng sản phẩm Shop còn là {product.Max}" },
                    JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && x.IsDelete == false)
                        .ToListAsync();

                    var dateTime = DateTime.Now;

                    var isUpdateQuantity = false;
                    if (model.Quantity != product.Quantity)
                    {
                        product.Quantity = model.Quantity;
                        product.QuantityBooked = model.Quantity;
                        product.TotalPrice = product.Price * product.Quantity;
                        product.TotalExchange = product.TotalPrice * product.ExchangeRate;
                        isUpdateQuantity = true;
                    }

                    product.Note = model.Note;
                    product.LastUpdate = dateTime;

                    var rs = await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // Số lượng mới cập nhật nhỏ hơn số lượng yêu cầu tối thiểu của Shop
                    var totalQuantity = productDetails.Sum(x => x.Quantity);
                    if (product.BeginAmount.HasValue && totalQuantity < product.BeginAmount.Value)
                    {
                        transaction.Rollback();
                        return JsonCamelCaseResult(
                            new { Status = -2, Text = Resource.SoLuongNhoHonSLCuaShop },
                            //new { Status = -2, Text = "Số lượng nhỏ hơn số lượng tối thiêu yêu cầu của Shop" },
                            JsonRequestBehavior.AllowGet);
                    }

                    var totalQuantityProduct = UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && x.IsDelete == false).Select(x => x.Quantity)
                        .ToList().Sum(x => x);

                    // Cập nhật lại giá và nội dung liên quan
                    if (rs > 0 && isUpdateQuantity)
                    {
                        // Cập nhật lại giá kiểm đếm cho đơn hàng
                        foreach (var pd in productDetails)
                        {
                            pd.AuditPrice = OrderRepository.OrderAudit(totalQuantityProduct, pd.Price);
                        }

                        // Cập nhật lại giá khi sửa số lượng sản phẩm
                        if (!string.IsNullOrEmpty(product.Prices) && !string.IsNullOrWhiteSpace(product.ProId))
                        {
                            var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(product.Prices);

                            var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin)
                                                                          ||
                                                                          (totalQuantity >= x.Begin &&
                                                                           totalQuantity <= x.End));

                            if (price != null)
                            {
                                foreach (var pd in productDetails.Where(x => x.Link == product.Link))
                                {
                                    pd.Price = price.Price;
                                    pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                    pd.TotalPrice = pd.Price * pd.Quantity;
                                    pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.Quantity;
                                }

                                await UnitOfWork.OrderDetailRepo.SaveAsync();
                            }
                        }

                        order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalExchange);

                        order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalPrice);

                        order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.Quantity);

                        await UnitOfWork.OrderRepo.SaveAsync();

                        //--------- Cập nhật lại giá dịch vụ -----------
                        //                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                        //                        var totalExchange = order.TotalExchange;
                        //                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;
                        //
                        //                        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        //                        if (order.TotalExchange < 2000000)
                        //                        {
                        //                            totalPrice = 150000;
                        //                        }

                        //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                        //service.LastUpdate = dateTime;
                        //service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    service.TotalPrice -= service.TotalPrice * discount / 100;
                        //    service.Note =
                        //        $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                        //    //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // Cập nhật dịch vụ kiểm đếm
                        var serviceAudit = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.ServiceId == (byte)OrderServices.Audit && x.OrderId == order.Id && !x.IsDelete);

                        serviceAudit.LastUpdate = dateTime;

                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                        && x.Status == (byte)OrderDetailStatus.Order)
                            .SumAsync(x => x.AuditPrice.Value * x.Quantity);

                        serviceAudit.Value = totalAuditPrice;
                        serviceAudit.TotalPrice = totalAuditPrice;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật tổng tiền đơn hàng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.LastUpdate = dateTime;
                        order.Total = totalService + order.TotalExchange;
                        order.TotalRefunded = 0;
                        order.Debt = order.Total;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = Resource.CapNhatSPThanhCong },
            //return JsonCamelCaseResult(new { Status = 1, Text = "Cập nhật sản phẩm thành công" },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //[Route("cap-nhat-don-hang")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrder(UpdateOrderViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1000, Text = "Dữ liệu truyền lên không đúng định dạng" },
                    JsonRequestBehavior.AllowGet);

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                x => x.Id == model.Id && x.Status == (byte)OrderStatus.New &&
                     x.CustomerId == CustomerState.Id && !x.IsDelete);

            if (order == null)
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.DHKhongTonTaiHoacBiXoa },
                //return JsonCamelCaseResult(new { Status = -1, Text = "Đơn hàng không tồn tại hoặc đã bị xóa" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var dateTime = DateTime.Now;

                    order.Note = model.Note;
                    order.PrivateNote = model.PrivateNote;

                    var isUpdateServiceType = false;
                    if (order.ServiceType != model.ServiceType)
                    {
                        order.ServiceType = model.ServiceType;
                        isUpdateServiceType = true;
                    }

                    var rs = await UnitOfWork.OrderRepo.SaveAsync();

                    if (rs > 0 && isUpdateServiceType)
                    {
                        //                        // Cập nhật lại phí mua hàng
                        //                        var totalExchange = order.TotalExchange;
                        //                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;
                        //
                        //                        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        //                        if (order.TotalExchange < 2000000)
                        //                        {
                        //                            totalPrice = 150000;
                        //                        }

                        //var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //    x => x.ServiceId == (byte)OrderServices.Order && x.OrderId == order.Id && !x.IsDelete);

                        //service.LastUpdate = dateTime;
                        //service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    service.TotalPrice -= service.TotalPrice * discount / 100;
                        //    service.Note =
                        //        $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                        //        //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // Cập nhật tổng tiền đơn hàng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked)
                            .SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total;
                        order.LastUpdate = dateTime;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "การปรับปรุงผลิตภัณฑ์ที่ประสบความสำเร็จ" },
            //return JsonCamelCaseResult(new { Status = 1, Text = "Cập nhật đơn hàng thành công" },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //[Route("cap-nhat-dich-vu")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateOrderService(UpdateServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1000, Text = "Dữ liệu truyền lên không đúng định dạng" },
                    JsonRequestBehavior.AllowGet);

            if (!await UnitOfWork.OrderDetailRepo.CheckCustomerHasUpdateServiceShoppingCart(model.Id, CustomerState.Id,
                OrderStatus.New))
                return JsonCamelCaseResult(new { Status = -200, Text = "Bạn không có quyền cập đơn hàng này" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var service = await UnitOfWork.OrderServiceRepo
                        .SingleOrDefaultAsync(x => x.OrderId == model.OrderId && x.ServiceId == model.Id && !x.IsDelete);

                    if (service == null)
                        return JsonCamelCaseResult(new { Status = -1, Text = Resource.DHKhongTonTaiHoacBiXoa },
                        //return JsonCamelCaseResult(new { Status = -1, Text = Resource"Bản ghi này không tồn tại hoặc đã bị xóa" },
                            JsonRequestBehavior.AllowGet);

                    service.Checked = model.Checked;

                    var rs = await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Tính lại chi phí nếu chọn dịch vụ mở hàng
                    if (rs > 0 && model.Id == (byte)OrderServices.Audit)
                    {
                        var order =
                            await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == model.OrderId && !x.IsDelete);

                        var dateTime = DateTime.Now;

                        // Cập nhật dịch vụ kiểm đếm
                        var serviceAudit = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.ServiceId == (byte)OrderServices.Audit && x.OrderId == order.Id && !x.IsDelete);

                        serviceAudit.LastUpdate = dateTime;

                        if (serviceAudit.Checked)
                        {
                            var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => x.OrderId == order.Id && x.IsDelete == false)
                                .ToListAsync();

                            var totalQuantityProduct = UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => x.OrderId == order.Id && x.IsDelete == false).Select(x => x.Quantity)
                                .ToList().Sum(x => x);

                            // Cập nhật lại giá kiểm đếm cho đơn hàng
                            foreach (var pd in productDetails)
                            {
                                pd.AuditPrice = OrderRepository.OrderAudit(totalQuantityProduct, pd.Price);
                            }

                            var totalAuditPrice = productDetails.Sum(x => (x.AuditPrice ?? 0) * x.Quantity);

                            serviceAudit.Value = totalAuditPrice;
                            serviceAudit.TotalPrice = totalAuditPrice;
                        }
                        else
                        {
                            serviceAudit.Value = 0;
                            serviceAudit.TotalPrice = 0;
                        }

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật tổng tiền đơn hàng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked && !x.IsDelete)
                            .SumAsync(x => x.TotalPrice);

                        order.LastUpdate = dateTime;
                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "การปรับปรุงผลิตภัณฑ์ที่ประสบความสำเร็จ" },
            //return JsonCamelCaseResult(new { Status = 1, Text = "Cập đơn hàng thành công" },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        //[Route("xoa-san-pham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await UnitOfWork.OrderDetailRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (product == null)
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.SPKhongTonTaiHoacBiXoa },
                //return JsonCamelCaseResult(new { Status = -1, Text =Resource.SPKhongTonTaiHoacBiXoa "Sản phẩm không tồn tại hoặc đã bị xóa" },
                    JsonRequestBehavior.AllowGet);

            if (!await UnitOfWork.OrderDetailRepo.CheckCustomerHasUpdateProductShoppingCart(id, CustomerState.Id,
                OrderStatus.New))
                return JsonCamelCaseResult(new { Status = -2, Text = "Bạn không có quyền xóa sản phẩm này" },
                    JsonRequestBehavior.AllowGet);

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                        x => !x.IsDelete && x.Id == product.OrderId && x.CustomerId == CustomerState.Id);

                    if (order.LinkNo == 1)
                    {
                        order.IsDelete = true;
                    }

                    product.IsDelete = true;
                    product.LastUpdate = DateTime.Now;

                    var productDetails = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && x.IsDelete == false)
                        .ToListAsync();

                    var totalQuantity = productDetails.Sum(x => x.Quantity);

                    if (product.BeginAmount.HasValue && productDetails.Count > 1 && (totalQuantity - product.Quantity) < product.BeginAmount.Value)
                        return JsonCamelCaseResult(
                            new { Status = -2, Text = Resource.SoLuongNhoHonSLCuaShop },
                            //new { Status = -2, Text = "Số lượng nhỏ hơn số lượng tối thiêu yêu cầu của Shop" },
                            JsonRequestBehavior.AllowGet);

                    var rs = await UnitOfWork.OrderDetailRepo.SaveAsync();

                    if (rs > 0 && order.LinkNo != 1)
                    {
                        var totalQuantityProduct = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && x.IsDelete == false).Select(x => x.Quantity)
                            .ToList().Sum(x => x);

                        // Cập nhật giá kiểm đếm của link sản phẩm
                        foreach (var pd in productDetails)
                        {
                            pd.AuditPrice = OrderRepository.OrderAudit(totalQuantityProduct, pd.Price);
                        }

                        // Cập nhật lại giá khi sửa số lượng sản phẩm
                        if (!string.IsNullOrEmpty(product.Prices) && !string.IsNullOrWhiteSpace(product.ProId))
                        {
                            var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(product.Prices);

                            var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin)
                                                                          ||
                                                                          (totalQuantity >= x.Begin &&
                                                                         totalQuantity <= x.End));

                            if (price != null)
                            {
                                foreach (var pd in productDetails.Where(x => x.Link == product.Link))
                                {
                                    pd.Price = price.Price;
                                    pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                    pd.TotalPrice = pd.Price * pd.Quantity;
                                    pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.Quantity;
                                }

                                await UnitOfWork.OrderDetailRepo.SaveAsync();
                            }
                        }

                        order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalExchange);

                        order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.TotalPrice);

                        order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                            .CountAsync(x => x.OrderId == order.Id && !x.IsDelete);

                        order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .SumAsync(x => x.Quantity);

                        // Submit cập nhật order
                        await UnitOfWork.OrderRepo.SaveAsync();

                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                        // var totalExchange = order.TotalExchange;
                        //var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        //                        if (order.TotalExchange < 2000000)
                        //                        {
                        //                            totalPrice = 150000;
                        //                        }

                        //var orderServcie = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //    x => x.OrderId == order.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);

                        //orderServcie.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                        //orderServcie.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                        //// Triết khấu phí mua hàng
                        //var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                        //if (discount > 0)
                        //{
                        //    orderServcie.TotalPrice -= orderServcie.TotalPrice * discount / 100;
                        //    orderServcie.Note =
                        //        $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                        //        //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // Thêm các dịch vụ Option (Kiểm đếm, Đóng kiện, nhận hàng tại nhà)
                        var orderService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.OrderId == order.Id && !x.IsDelete &&
                                 x.ServiceId == (byte)(OrderServices.Packing));

                        if (orderService.Checked && orderService.ServiceId == (byte)OrderServices.Packing)
                        {
                            var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                            && x.Status == (byte)OrderDetailStatus.Order)
                                .SumAsync(x => x.AuditPrice.Value * x.Quantity);

                            orderService.Value = totalAuditPrice;
                            orderService.TotalPrice = totalAuditPrice;
                        }

                        // Submit thêm OrderService
                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật số lượng tổng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                            x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total;

                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = Resource.XoaSanPhamThanhCong },
            //return JsonCamelCaseResult(new { Status = 1, Text = "Xóa sản phẩm thành công" },
                JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteOrder(List<OrderDeleteMeta> orderModels)
        {
            if (!ModelState.IsValid)
            {
                var messages = string.Join("; ", ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.DinhDangDuLieuKhongDung + $": { messages}" },
                //return JsonCamelCaseResult(new { Status = -1, Text = $"Định dạng dữ liệu không đúng: {messages}" },
                    JsonRequestBehavior.AllowGet);
            }

            if (orderModels == null || !orderModels.Any())
                return JsonCamelCaseResult(new { Status = -1, Text = Resource.DinhDangDuLieuKhongDung },
                            JsonRequestBehavior.AllowGet);

            var orderIds = $";{string.Join(";", orderModels.Select(x => x.Id).ToList())};";

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var orders = await
                    UnitOfWork.OrderRepo.FindAsync(
                        x => orderIds.Contains(";" + x.Id + ";") && x.IsDelete == false &&
                             x.CustomerId == CustomerState.Id);

                    if (orders.Count != orderModels.Count)
                    {
                        var shopNames = string.Join(", ", orderModels.Where(x => orders.All(o => o.Id != x.Id)).Select(x => x.ShopName).ToList());

                        return JsonCamelCaseResult(new { Status = -1, Text = $"Đơn hàng {shopNames} không tồn tại hoặc đã bị xóa" },
                            JsonRequestBehavior.AllowGet);
                    }

                    var timeNow = DateTime.Now;

                    foreach (var o in orders)
                    {
                        o.IsDelete = true;
                        o.LastUpdate = timeNow;
                    }

                    await UnitOfWork.OrderRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return JsonCamelCaseResult(new { Status = 1, Text = "นำผลิตภัณฑ์ที่ประสบความสำเร็จ!" },
            //return JsonCamelCaseResult(new { Status = 1, Text = "Xóa đơn hàng thành công!" },
                JsonRequestBehavior.AllowGet);
        }
    }
}