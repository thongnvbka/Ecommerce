using Common.Constant;
using Common.Emums;
using Common.Items;
using Library.Models;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using Common.Helper;
using Library.DbContext.Entities;
using Library.DbContext.Repositories;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Common.Host;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using Library.ViewModels.Items;
using System.Web;
using System.IO;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class OrderController : BaseController
    {
        #region order

        // GET: CMS/Order
        //TODO Đơn hàng mua hàng
        public ActionResult BuyOrder()
        {
            ViewBag.ActiveBuyOrder = "cl_on";
            return View();
        }

        //TODO Tạo đơn hàng
        public ActionResult CreateOrder()
        {
            var listWarehouseDelivery = new List<SelectListItem>();
            var allWarehouseDelivery =
                UnitOfWork.OfficeRepo.FindAsNoTracking(
                    x =>
                        !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use &&
                        x.Culture == "VN");
            listWarehouseDelivery.AddRange(
                allWarehouseDelivery.Select(item => new SelectListItem { Text = item.Name, Value = item.Id.ToString() }));

            //Khong su dung dich vu di bay
            //var objFastDeliveryService = new OrderService()
            //{
            //    OrderId = 0,
            //    ServiceId = (byte)OrderServices.FastDelivery,
            //    ServiceName = OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
            //    ExchangeRate = ExchangeRate(),
            //    IsDelete = false,
            //    Created = DateTime.Now,
            //    LastUpdate = DateTime.Now,
            //    HashTag = string.Empty,
            //    Value = 0,
            //    Currency = Currency.VND.ToString(),
            //    Type = (byte)UnitType.Percent,
            //    TotalPrice = 0,
            //    Mode = (byte)OrderServiceMode.Option,
            //    Checked = false
            //};
            var objPackingService = new OrderService()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.Packing,
                ServiceName = OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                IsDelete = false,
                Created = DateTime.Now,
                LastUpdate = DateTime.Now,
                HashTag = string.Empty,
                Value = 0,
                Currency = Currency.VND.ToString(),
                Type = (byte)UnitType.Percent,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Option,
                Checked = false
            };

            var objAuditService = new OrderService()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.Audit,
                ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                IsDelete = false,
                Created = DateTime.Now,
                LastUpdate = DateTime.Now,
                HashTag = string.Empty,
                Value = 0,
                Currency = Currency.VND.ToString(),
                Type = (byte)UnitType.Percent,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Option,
                Checked = false
            };

            var listService = new List<OrderService> { objAuditService, objPackingService };

            //Lấy về kho nhận hàng của Khách
            var customer = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == CustomerState.Id);
            int? warehouseId = 0;
            if (customer != null)
            {
                warehouseId = customer.WarehouseId ?? 0;
            }
            var jsonSerialiser = new JavaScriptSerializer();
            ViewBag.listService = jsonSerialiser.Serialize(listService);
            ViewBag.ListWardDelivery = JsonConvert.SerializeObject(listWarehouseDelivery);
            ViewBag.ActiveBuyOrder = "cl_on";
            ViewBag.Rate = ExchangeRate();
            ViewBag.WarehouseId = warehouseId;
            return View();
        }

        /// <summary>
        /// Lấy giá trị tiền cần đặt cọc
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMoneyLevel(int orderId)
        {
            var model = new OrderAdvanceMoneyItem();
            if (CustomerState == null)
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                model = UnitOfWork.OrderRepo.GetMoneyLevelByLinq(orderId, CustomerState.Id);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật trạng thái hủy đơn  khi chưa đặt cọc tiền
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> UpdateStatus(int orderId)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var obj =
                    await
                        UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    if (obj.Status <= (byte)OrderStatus.WaitOrder)
                    {
                        obj.Status = (byte)OrderStatus.Cancel;
                        result = await UnitOfWork.OrderRepo.SaveAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Check điều kiện đặt cọc đơn hàng, cụ thể là tiền trong tài khoản khách hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<int> ConfirmUpdateBalance(int orderId, int type)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                    x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);
                var obj =
                    await
                        UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    var advanceMoney = 0M;
                    advanceMoney = UnitOfWork.OrderRepo.UpdateBalanceByLinq(orderId, CustomerState.Id, type,
                        (byte)OrderStatus.WaitOrder);
                    if (advanceMoney < customer.BalanceAvalible)
                        result = 1;
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Đặt cọc cho đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public async Task<int> UpdateBalance(int orderId, string pass, int type)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                    x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                var obj =
                    await
                        UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                            x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    var advanceMoney = 0M;
                    advanceMoney = UnitOfWork.OrderRepo.UpdateBalanceByLinq(orderId, CustomerState.Id, type,
                        (byte)OrderStatus.WaitOrder);
                    if (advanceMoney < customer.BalanceAvalible && obj.Status <= (byte)OrderStatus.WaitDeposit)
                    {
                        if (
                            !Common.PasswordEncrypt.PasswordEncrypt.EncodePassword(pass.Trim(),
                                Common.Constant.PasswordSalt.FinGroupApiCustomer).Equals(CustomerState.Password))
                        {
                            result = -1;
                        }
                        else
                        {
                            var exchangeRate = ExchangeRate();
                            var timeNow = DateTime.Now;
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
                                ModeName =
                                    OrderExchangeType.Product.GetAttributeOfType<DescriptionAttribute>().Description,
                                Note = Resource.TTDatCocDonHang +// $"Thanh toán đặt cọc đơn hàng " +
                                    string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"),
                                        "{0:###,###}", advanceMoney),
                                OrderId = orderId,
                                TotalPrice = advanceMoney,
                                Status = (byte)OrderExchangeStatus.Approved,
                                OrderType = (byte)OrderExchangeOrderType.Order
                            });

                            // Thêm lịch sử thay đổi trạng thái
                            // Todo: Giỏi bổ xung thêm loại đơn hàng để phân biệt đơn hàng order, ký gửi, tìm nguồn
                            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                            {
                                CreateDate = timeNow,
                                Content =
                                    Resource.Title_DepositFromCart +
                                    string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"),
                                        "{0:###,###}", advanceMoney),
                                CustomerId = CustomerState.Id,
                                CustomerName = CustomerState.FullName,
                                OrderId = obj.Id,
                                Status = obj.Status,
                                Type = obj.Type
                            });
                            UnitOfWork.OrderHistoryRepo.Save();

                            UnitOfWork.OrderExchangeRepo.SaveNoCheck();
                            // Gửi thông báo Notification cho khách hàng
                            var notification = new Notification()
                            {
                                SystemId = SystemId,
                                SystemName = SystemName,
                                CustomerId = CustomerState.Id,
                                CustomerName = CustomerState.FullName,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                OrderId = obj.Id,
                                OrderType = 0, // Thông báo giành cho thay đổi ví kế toán
                                IsRead = false,
                                Title = Resource.Title_DepositFromCart + string.Format("{0:###,###}", advanceMoney),
                                Description = Resource.SoDuVDTBiTru +// "Số dư ví điện tử của bạn bị trừ: " +
                                    string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"),
                                        "{0:###,###}", advanceMoney) + " BAHT"
                            };

                            UnitOfWork.NotificationRepo.Add(notification);
                            // Tài khoản đang hoạt động
                            if (customer != null)
                            {
                                //sua lai theo cach tinh moi
                                var processRechargeBillResult =
                                    UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                                    {
                                        CustomerId = customer.Id,
                                        CurrencyFluctuations = advanceMoney,
                                        OrderId = orderId,
                                        TreasureIdd = (int)TreasureCustomerWallet.AdvanceOrder
                                    });
                            }
                            obj.TotalPayed = advanceMoney;
                            obj.Debt = obj.Total - obj.TotalPayed;
                            obj.TotalAdvance = advanceMoney;
                            obj.Created = DateTime.Now;
                            obj.Status = (byte)OrderStatus.WaitOrder;
                            UnitOfWork.NotificationRepo.Save();

                            if (customer != null)
                            {
                                CustomerState.FromUser(customer);
                            }
                            result = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        #region payment detail

        /// <summary>
        /// Check điều kiện số tiền đặt cọc và số tiền trong ví điện tử
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public int ConfirmPaymentBalance(int orderId)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                result = UnitOfWork.OrderDetailRepo.CheckDepositByLinq(orderId, CustomerState.Id);
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Thanh toán tiền hàng, tiền dịch vụ còn thiếu sau khi đặt cọc, có thể phải tính lại số tiền còn thiếu
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pass"></param>
        /// <param name="moneyMiss"></param>
        /// <returns></returns>
        public async Task<int> PaymentBalance(int orderId, string pass, string moneyMiss)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var tmpCheck = UnitOfWork.OrderDetailRepo.CheckDepositByLinq(orderId, CustomerState.Id);
                if (tmpCheck == 1)
                {
                    if (
                        !Common.PasswordEncrypt.PasswordEncrypt.EncodePassword(pass.Trim(),
                            Common.Constant.PasswordSalt.FinGroupApiCustomer).Equals(CustomerState.Password))
                    {
                        result = -1;
                    }
                    else
                    {
                        var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderId);
                        decimal tmpMiss = 0;
                        decimal.TryParse(moneyMiss.Replace(",", ""), out tmpMiss);
                        var exchangeRate = ExchangeRate();
                        var timeNow = DateTime.Now;
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
                            Note = Resource.TTTienConThieu + " " +// $"Thanh toán tiền còn thiếu " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}",
                                    tmpMiss) + " BAHT",
                            OrderId = orderId,
                            TotalPrice = tmpMiss,
                            Status = (byte)OrderExchangeStatus.Approved,
                            OrderType = (byte)OrderExchangeOrderType.Order
                        });

                        // Thêm lịch sử thay đổi trạng thái
                        // Todo: Giỏi bổ xung thêm loại đơn hàng để phân biệt đơn hàng order, ký gửi, tìm nguồn
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = Resource.TTTienConThieu + " " +//"Thanh toán tiền còn thiếu " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}",
                                    tmpMiss) + " BAHT",
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            OrderId = order.Id,
                            Status = order.Status,
                            Type = order.Type
                        });
                        UnitOfWork.OrderHistoryRepo.Save();

                        UnitOfWork.OrderExchangeRepo.SaveNoCheck();
                        //cap nhat tien tai khoan
                        UnitOfWork.OrderRepo.PaymentBalance(orderId, CustomerState.Id, tmpMiss);

                        var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                            x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            SystemId = SystemId,
                            SystemName = SystemName,
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderType = 0, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = Resource.TTTienConThieu + " " +// "Thanh toán tiền còn thiếu " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}",
                                    tmpMiss) + " ให้แก่ออเดอร์  #ORD" + order.Code.ToString(),
                            Description = Resource.SoDuVDTBiTru +//"Số dư ví điện tử của bạn bị trừ: " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}",
                                    tmpMiss) + " BAHT"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);

                        // Tài khoản không bị khóa
                        if (customer != null)
                        {
                            // Thanh toán tiền
                            var processRechargeBillResult =
                                UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                                {
                                    CustomerId = customer.Id,
                                    CurrencyFluctuations = tmpMiss,
                                    OrderId = order.Id,
                                    TreasureIdd = (int)EnumAccountantSubject.Customer
                                });
                        }
                        //tinh toan lai
                        order.TotalPayed = order.TotalPayed - tmpMiss;
                        order.TotalRefunded = order.TotalRefunded + tmpMiss;
                        order.Debt = order.Total - order.TotalPayed;
                        UnitOfWork.NotificationRepo.Save();
                        UnitOfWork.OrderHistoryRepo.Save();

                        if (customer != null)
                        {
                            CustomerState.FromUser(customer);
                        }
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Cập nhật hủy đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public int UpdateCancelOrder(int orderId)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderId);
                if (order != null)
                {
                    if (order.Status <= (byte)OrderStatus.WaitOrder)
                    {
                        var tmpMiss = UnitOfWork.OrderRepo.RestoreBalanceByLinq(orderId, CustomerState.Id);

                        var exchangeRate = ExchangeRate();
                        var timeNow = DateTime.Now;

                        // Thêm giao dịch trong đơn hàng
                        UnitOfWork.OrderExchangeRepo.Add(new OrderExchange()
                        {
                            Created = timeNow,
                            Updated = timeNow,
                            Currency = Currency.VND.ToString(),
                            ExchangeRate = exchangeRate,
                            IsDelete = false,
                            Type = (byte)OrderExchangeType.Product,
                            Mode = (byte)OrderExchangeMode.Import,
                            ModeName = OrderExchangeType.Product.GetAttributeOfType<DescriptionAttribute>().Description,
                            Note = Resource.HoanTienDatCoc + $" " +//  $"Hoàn tiền đặt cọc " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:#,###}",
                                    tmpMiss),
                            OrderId = orderId,
                            TotalPrice = tmpMiss,
                            Status = (byte)OrderExchangeStatus.Approved,
                            OrderType = (byte)OrderExchangeOrderType.Order
                        });

                        // Thêm lịch sử thay đổi trạng thái
                        // Todo: Giỏi bổ xung thêm loại đơn hàng để phân biệt đơn hàng order, ký gửi, tìm nguồn
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = Resource.HoanTienDatCoc + $" " +//"Hoàn tiền đặt cọc " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:#,###}",
                                    tmpMiss),
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            OrderId = order.Id,
                            Status = order.Status,
                            Type = order.Type
                        });
                        UnitOfWork.OrderHistoryRepo.Save();

                        var customer = UnitOfWork.CustomerRepo.SingleOrDefault(
                            x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                        // Tài khoản không bị khóa
                        if (customer != null)
                        {
                            if (tmpMiss > 0)
                            {
                                var processRechargeBillResult =
                                    UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                                    {
                                        CustomerId = customer.Id,
                                        CurrencyFluctuations = tmpMiss,
                                        OrderId = order.Id,
                                        TreasureIdd = (int)TreasureMustReturn.CancelOrder
                                    });
                            }
                            else
                            {
                                order.Status = (byte)OrderStatus.Cancel;
                            }
                            // Thanh toán tiền
                        }
                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            SystemId = SystemId,
                            SystemName = SystemName,
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderId = order.Id,
                            OrderType = 0, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = Resource.HoanTienDatCoc + $" " + string.Format("{0:###,###}", tmpMiss),// Title = "Hoàn tiền đặt cọc " + string.Format("{0:###,###}", tmpMiss),
                            Description = Resource.SoDuVDTBiTru +//"Số dư ví điện tử của bạn được cộng: " +
                                string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}",
                                    tmpMiss) + " BAHT"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);
                        UnitOfWork.OrderExchangeRepo.SaveNoCheck();
                        if (customer != null)
                        {
                            CustomerState.FromUser(customer);
                        }
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        #endregion payment detail

        #region Comment

        [HttpPost]
        public int SaveComment(int orderId, string desc, byte orderType)
        {
            var result = 0;

            if (CustomerState == null)
            {
                return result;
            }

            if (string.IsNullOrEmpty(CustomerState.Email))
            {
                return result;
            }

            try
            {
                var checkExists =
                    UnitOfWork.OrderRepo.SingleOrDefaultAsync(
                        x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (checkExists != null)
                {
                    OrderComment item = new OrderComment();
                    item.OrderId = orderId;
                    item.Description = desc;
                    item.OrderType = orderType;
                    item.CustomerId = CustomerState.Id;
                    item.CustomerName = CustomerState.FullName;
                    item.SystemId = SystemId;
                    item.SystemName = SystemName;
                    item.CreateDate = DateTime.Now;
                    item.IsRead = false;
                    result = UnitOfWork.OrderCommentRepo.Insert(item);
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        #endregion Comment

        #region Detail

        public ActionResult Detail(int id)
        {
            var model = UnitOfWork.OrderDetailRepo.GetAllByLinq(id, (byte)OrderType.Order, CustomerState.Id);
            ViewBag.CustomerName = CustomerState.FullName;
            return View(model);
        }

        public ActionResult GetMorePackage(int packageId)
        {
            if (CustomerState == null)
            {
                return PartialView(new System.Collections.Generic.List<HistoryPackage>());
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return PartialView(new System.Collections.Generic.List<HistoryPackage>());
                }
            }
            var model =
                UnitOfWork.HistoryPackageRepo.Find(m => m.OrderPackage == packageId).OrderBy(m => m.CreateDate).ToList();
            return PartialView(model);
        }

        public string UpdateQuantity(int orderDetailId, int quantity)
        {
            if (CustomerState == null)
            {
                return "0|0|0";
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return "0|0|0";
                }
            }
            //dung ham khac
            //var beginAmount = 0;
            //var result = UnitOfWork.OrderDetailRepo.UpdateQuantity(orderDetailId, quantity, ref beginAmount);

            var product = UnitOfWork.OrderDetailRepo.SingleOrDefault(x => x.Id == orderDetailId && !x.IsDelete);

            if (product == null)
                return "-1|0|สินค้านี้ไม่มีอยู่หรือถูกลบออก";// return "-1|0|Sản phẩm này không tồn tại hoặc đã bị xóa";

            var order = UnitOfWork.OrderRepo.SingleOrDefault(x => x.Id == product.OrderId && x.IsDelete == false);

            if (quantity > product.Max)
            {
                return string.Format("-1|0|"+Resource.SoLuongShopCon + "{0}." +Resource.BanVuiLongNhapSoLuong,
                //return string.Format("-1|0|Số lượng sản phẩm Shop còn là {0}. Bạn vui lòng nhập lại số lượng.",
                    product.Max);
            }
            var result = 0;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var productDetails = UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == orderDetailId && x.IsDelete == false && x.Link == product.Link)
                        .ToList();

                    var totalQuantity = productDetails.Sum(x => x.Quantity);

                    if (product.BeginAmount.HasValue && totalQuantity < product.BeginAmount.Value)
                        return string.Format("-1|0|ขนาดเล็กจำนวนจำนวนขั้นต่ำ ({0}) ความต้องการของตลาด",
                            // return string.Format("-1|0|Số lượng nhỏ hơn số lượng tối thiểu ({0}) yêu cầu của Shop",
                            product.BeginAmount.Value);

                    var dateTime = DateTime.Now;

                    var isUpdateQuantity = false;
                    if (quantity != product.Quantity)
                    {
                        product.Quantity = quantity;
                        product.QuantityBooked = quantity;
                        product.TotalPrice = product.Price * product.Quantity;
                        product.TotalExchange = product.TotalPrice * product.ExchangeRate;
                        product.AuditPrice = OrderRepository.OrderAudit(product.Quantity, product.Price);
                        isUpdateQuantity = true;
                    }
                    product.LastUpdate = dateTime;

                    var rs = UnitOfWork.OrderDetailRepo.Save();

                    // Cập nhật lại giá và nội dung liên quan
                    if (rs > 0 && isUpdateQuantity)
                    {
                        // Cập nhật lại giá khi sửa số lượng sản phẩm
                        if (!string.IsNullOrEmpty(product.Prices) && !string.IsNullOrWhiteSpace(product.ProId))
                        {
                            var priceRangers = JsonConvert.DeserializeObject<List<PriceMeta>>(product.Prices);

                            var price = priceRangers.SingleOrDefault(x => (x.End == null && totalQuantity >= x.Begin)
                                                                          ||
                                                                          (totalQuantity >= x.Begin &&
                                                                           totalQuantity <= x.End));
                            var totalQuantityProduct = UnitOfWork.OrderDetailRepo.Entities
                                .Where(x => x.OrderId == order.Id && x.IsDelete == false).Select(x => x.Quantity)
                                .ToList().Sum(x => x);

                            if (price != null)
                            {
                                foreach (var pd in productDetails)
                                {
                                    pd.Price = price.Price;
                                    pd.ExchangePrice = pd.Price * pd.ExchangeRate;
                                    pd.TotalPrice = pd.Price * pd.Quantity;
                                    pd.TotalExchange = pd.Price * pd.ExchangeRate * pd.Quantity;
                                    pd.AuditPrice = OrderRepository.OrderAudit(totalQuantityProduct, pd.Price);
                                }

                                UnitOfWork.OrderDetailRepo.Save();
                            }
                        }

                        order.TotalExchange = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .Sum(x => x.TotalExchange);

                        order.TotalPrice = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .Sum(x => x.TotalPrice);

                        order.ProductNo = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete)
                            .Sum(x => x.Quantity);

                        UnitOfWork.OrderRepo.Save();

                        //--------- Cập nhật lại giá dịch vụ -----------
                        // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                        var totalExchange = order.TotalExchange;
                        var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                        // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                        if (order.TotalExchange < 2000000)
                        {
                            totalPrice = 150000;
                        }

                        //var service = UnitOfWork.OrderServiceRepo.SingleOrDefault(
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
                        //       // $"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                        //}

                        // Cập nhật dịch vụ kiểm đếm
                        var serviceAudit = UnitOfWork.OrderServiceRepo.SingleOrDefault(
                            x => x.ServiceId == (byte)OrderServices.Audit && x.OrderId == order.Id && !x.IsDelete);

                        serviceAudit.LastUpdate = dateTime;

                        var totalAuditPrice = UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                        && x.Status == (byte)OrderDetailStatus.Order)
                            .Sum(x => x.AuditPrice.Value * x.Quantity);

                        serviceAudit.Value = totalAuditPrice;
                        serviceAudit.TotalPrice = totalAuditPrice;

                        UnitOfWork.OrderServiceRepo.Save();

                        // Cập nhật tổng tiền đơn hàng
                        var totalService = UnitOfWork.OrderServiceRepo.Entities
                            .Where(x => x.OrderId == order.Id && !x.IsDelete && x.Checked).Sum(x => x.TotalPrice);

                        order.LastUpdate = dateTime;
                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total;

                        result = UnitOfWork.OrderServiceRepo.Save();
                    }

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return result.ToString() + "|" + product.BeginAmount.Value + "|0";
        }

        public int UpdateNote(int orderId, string note)
        {
            if (CustomerState == null)
            {
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return 0;
                }
            }
            var result = 0;
            var tmpOrder = UnitOfWork.OrderRepo.FirstOrDefault(m => m.Id == orderId);
            if (tmpOrder != null)
            {
                tmpOrder.Note = note;
                result = UnitOfWork.OrderRepo.SaveNoCheck();
            }
            return result;
        }

        public int UpdatePrivateNote(int detailId, string note)
        {
            if (CustomerState == null)
            {
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return 0;
                }
            }
            var result = 0;
            var tmpOrder = UnitOfWork.OrderDetailRepo.FirstOrDefault(m => m.Id == detailId);
            if (tmpOrder != null)
            {
                tmpOrder.PrivateNote = note;
                result = UnitOfWork.OrderDetailRepo.SaveNoCheck();
            }
            return result;
        }

        /// <summary>
        /// Cập nhật số tiền sử dụng dịch vụ
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="serviceId"></param>
        /// <param name="isCheck"></param>
        /// <returns></returns>
        public async Task<int> UpdateService(int orderId, int serviceId, bool isCheck)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return 0;
                }
            }
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            if (order != null)
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        switch (serviceId)
                        {
                            //case (byte)OrderServices.Order:
                            //    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                            //    var totalExchange = order.TotalExchange;
                            //    var totalPrice = totalExchange *
                            //                     OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                            //    // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                            //    if (order.TotalExchange < 2000000)
                            //    {
                            //        totalPrice = 150000;
                            //    }

                            //    var service = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            //        x => x.ServiceId == serviceId && x.OrderId == order.Id && !x.IsDelete);
                            //    service.Checked = isCheck;
                            //    service.LastUpdate = DateTime.Now;
                            //    if (isCheck)
                            //    {
                            //        service.Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange);
                            //        service.TotalPrice = totalPrice < 5000 ? 5000 : totalPrice;

                            //        // Triết khấu phí mua hàng
                            //        var discount = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Order;
                            //        if (discount > 0)
                            //        {
                            //            service.TotalPrice -= service.TotalPrice * discount / 100;
                            //            service.Note =
                            //                $"ซื้อในราคาสุดพิเศษ {discount.ToString("N2", CultureInfo)}%";
                            //                //$"Phí mua hàng đã được triết khấu {discount.ToString("N2", CultureInfo)}%";
                            //        }
                            //    }
                            //    else
                            //    {
                            //        service.Value = 0;
                            //        service.TotalPrice = 0;
                            //        service.Note = $"ลูกค้าที่ใช้บริการยกเลิก";
                            //        //service.Note = $"Khách hàng hủy sử dụng dịch vụ";
                            //    }

                            //    var aa = await UnitOfWork.OrderServiceRepo.SaveAsync();
                            //    break;

                            case (byte)OrderServices.Audit:
                                // Cập nhật dịch vụ kiểm đếm
                                // phai tinh lai dua tren: gia tri san pham
                                var serviceAudit = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => x.ServiceId == serviceId && x.OrderId == orderId);
                                serviceAudit.Checked = isCheck;
                                serviceAudit.LastUpdate = DateTime.Now;
                                if (isCheck)
                                {
                                    var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                                        .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null
                                                    && x.Status != (byte)OrderDetailStatus.Cancel)
                                        .SumAsync(x => x.AuditPrice.Value * x.Quantity);

                                    serviceAudit.Value = totalAuditPrice;
                                    serviceAudit.TotalPrice = totalAuditPrice;
                                }
                                else
                                {
                                    serviceAudit.Value = 0;
                                    serviceAudit.TotalPrice = 0;
                                    serviceAudit.Note = Resource.KHHuySuDungDV;
                                    //serviceAudit.Note = $"Khách hàng hủy sử dụng dịch vụ";
                                }

                                await UnitOfWork.OrderServiceRepo.SaveAsync();
                                break;

                            case (byte)OrderServices.Packing:
                                // Cập nhật dịch vụ đóng kiện
                                // phai tinh lai dua tren: gia tri san pham
                                var servicePacking = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                    x => x.ServiceId == serviceId && x.OrderId == orderId);
                                servicePacking.Checked = isCheck;
                                servicePacking.LastUpdate = DateTime.Now;

                                await UnitOfWork.OrderServiceRepo.SaveAsync();
                                break;
                                //case (byte)OrderServices.FastDelivery:
                                //    // Cập nhật dịch vụ di nhanh
                                //    // phai tinh lai dua tren: gia tri san pham
                                //    var tmpFast = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                //        x => x.ServiceId == serviceId && x.OrderId == orderId);
                                //    tmpFast.Checked = isCheck;
                                //    tmpFast.LastUpdate = DateTime.Now;

                                //    await UnitOfWork.OrderServiceRepo.SaveAsync();
                                //    break;
                        }
                        // Cập nhật số lượng tổng
                        var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                            x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                        order.Total = totalService + order.TotalExchange;
                        order.Debt = order.Total;

                        await UnitOfWork.OrderRepo.SaveAsync();
                        result = 1;

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        result = 0;
                        throw;
                    }
                }
            }
            return result;
        }

        #endregion Detail

        //TODO chi tiết đơn hàng order
        public ActionResult DetailOrder()
        {
            ViewBag.ActiveBuyOrder = "cl_on";
            return View();
        }

        //TODO Lay danh sach cac don hang Order
        [HttpPost]
        public JsonResult GetAllOrderList(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new OrderExhibitionModel();
            model = GetData(seachInfor, pageInfor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public OrderExhibitionModel GetData(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new OrderExhibitionModel();
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm",
                    CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm",
                    CultureInfo.InvariantCulture);
            }
            seachInfor.SystemId = SystemId;
            seachInfor.CustomerId = CustomerState.Id;
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" ||
                seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            if (string.IsNullOrEmpty(seachInfor.Keyword))
            {
                seachInfor.Keyword = "";
            }
            seachInfor.OrderType = (byte)OrderType.Order;
            model = UnitOfWork.ExhibitionRepo.GetAllByLinq(pageInfor, seachInfor);
            return model;
        }

        public string DetailCount(int orderId)
        {
            var result = "0 " + Resource.Dashboard_Product;
            var model = new OrderDetailCountItem();

            model = UnitOfWork.ExhibitionRepo.GetDetailCountByOrderId(orderId);
            if (model != null)
            {
                result = string.Format("({0}/{1}/{2} - " + Resource.OrderGiveProducts + ")", model.ProductCount,
                   model.QuantityBooked, model.QuantityActually);

                //result = string.Format("({0}/{1}/{2} - Khách đặt/Đặt được/Thực nhận)", model.ProductCount,
                //    model.QuantityBooked, model.QuantityActually);
            }
            return result;
        }

        public void ExportOrder(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetData(seachInfor, pageInfor);
            var sb = new StringBuilder();
            sb.Append("<table border='1px' cellpadding='1' cellspacing='1' >");
            sb.Append("<tr>");
            sb.Append("<td>#</td>");
            sb.Append("<td>" + Resource.TransactionHistory_Time + "</td>");//sb.Append("<td>Thời gian</td>");
            sb.Append("<td>" + Resource.DetaiOrder_CodeOrder + "</td>");//sb.Append("<td>Mã đơn hàng</td>");
            sb.Append("<td>" + Resource.Order_LinkShop + " </td>");//sb.Append("<td>Link shop</td>");
            sb.Append("<td>" + Resource.TongSoSanPham + "</td>");//sb.Append("<td>Tổng số sản phẩm</td>");
            sb.Append("<td>" + Resource.Product_TotalMoney + " " + Resource.Currency + "</td>");// sb.Append("<td>Tổng tiền VND</td>");
            sb.Append("<td>" + Resource.Product_TotalMoney + " CNY</td>");//sb.Append("<td>Tổng tiền NDT</td>");
            sb.Append("<td>" + Resource.Order_Missing + " </td>");// sb.Append("<td>Còn thiếu</td>");
            sb.Append("<td>" + Resource.Order_Status + "</td>");//sb.Append("<td>Trạng thái</td>");
            sb.Append("</tr>");
            var index = 0;
            if (model.ListItems.Any())
            {
                var email = "";
                for (int i = 0; i < model.ListItems.Count(); i++)
                {
                    var item = model.ListItems[i];
                    var tmpMiss = "0";
                    if (item.Status == (byte)Common.Emums.OrderStatus.Cancel ||
                        item.Status == (byte)Common.Emums.OrderStatus.Lost)
                    {
                        tmpMiss = "0";
                    }
                    else
                    {
                        if (item.TotalMiss >= 1)
                        {
                            tmpMiss = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###}", item.TotalMiss);
                        }
                        else
                        {
                            tmpMiss = "0";
                        }
                    }

                    var statusText = "";
                    if (item.Status == (byte)Common.Emums.OrderStatus.WaitPrice)
                    {
                        statusText = Resource.Order_StatusWaitQuotes;// statusText = "Chờ báo giá";
                    }
                    if (item.Status == (byte)Common.Emums.OrderStatus.WaitDeposit)
                    {
                        statusText = Resource.Order_StatusPendingDeposit;// statusText = "Chờ đặt cọc";
                    }
                    if (item.Status == (byte)Common.Emums.OrderStatus.WaitOrder)
                    {
                        statusText = Resource.Order_StatusWaitOrder;// statusText = "Chờ đặt hàng";
                    }

                    if (item.Status >= (byte)Common.Emums.OrderStatus.Order &&
                        item.Status <= (byte)Common.Emums.OrderStatus.AccountantProcessing)
                    {
                        statusText = Resource.Order_StatusOnOrder;// statusText = "Đang đặt hàng";
                    }
                    if (item.Status == (byte)Common.Emums.OrderStatus.OrderSuccess)
                    {
                        statusText = Resource.Order_StatusOrdersComplete;//statusText = "Đặt hàng xong";
                    }

                    if (item.Status >= (byte)Common.Emums.OrderStatus.OrderSuccess &&
                        item.Status <= (byte)Common.Emums.OrderStatus.DeliveryShop)
                    {
                        statusText = Resource.Order_StatusOrdersComplete;//statusText = "Đặt hàng xong";
                    }
                    switch (item.Status)
                    {
                        case (byte)Common.Emums.OrderStatus.InWarehouse:
                            statusText = Resource.Deposit_GoodsInStorage;// statusText = "Hàng trong kho";
                            break;

                        case (byte)Common.Emums.OrderStatus.Shipping:
                            statusText = Resource.Order_StatusTransport;// statusText = "Đang vận chuyển";
                            break;

                        case (byte)Common.Emums.OrderStatus.Pending:
                            statusText = Resource.Order_StatusWaitDelivery;// statusText = "Chờ giao hàng";
                            break;

                        case (byte)Common.Emums.OrderStatus.GoingDelivery:
                            statusText = Resource.Order_StatusDelivered;//  statusText = "Đang giao hàng";
                            break;

                        case (byte)Common.Emums.OrderStatus.Finish:
                            statusText = Resource.Order_StatusComplete;//  statusText = "Hoàn thành";
                            break;

                        case (byte)Common.Emums.OrderStatus.Cancel:
                            statusText = Resource.Order_StatusDestroy;// statusText = "Đã hủy";
                            break;

                        case (byte)Common.Emums.OrderStatus.Lost:
                            statusText = Resource.Order_StatusThroat;// statusText = "Mất hỏng";
                            break;
                    }
                    var link = item.ShopLink.IndexOf('?') > 0
                        ? item.ShopLink.Substring(0, item.ShopLink.IndexOf('?'))
                        : item.ShopLink;

                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++index);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0:dd/MM/yyyy}\")", item.created));
                    sb.AppendFormat("<td>{0}</td>", item.code);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=HYPERLINK(\"{0}\",\"{1}\")", item.ShopLink, link));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.ProductCount));
                    sb.AppendFormat("<td>{0}</td>",
                        string.Format("=TEXT(\"{0}\",\"#,###\")",
                            string.Format(CultureInfo.InvariantCulture, "{0:#,###}", item.TotalExchange)));
                    sb.AppendFormat("<td>{0}</td>",
                        string.Format("=LOWER(\"{0}\")",
                            string.Format(CultureInfo.InvariantCulture, "{0:#,###.0}", item.TotalPrice)));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", tmpMiss));
                    sb.AppendFormat("<td>{0}</td>", statusText);
                    sb.AppendFormat("</tr>").AppendLine();
                }
            }
            sb.Append("</table>");
            var fileName = string.Format("DonOrder_{0:ddMMyyyy}.xls", DateTime.Now);
            var attachment = "attachment; filename=" + fileName + "";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel"; //office 2003
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Lấy thông tin danh sách khiếu nại trong đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetTicketByOrder(int orderId)
        {
            var lstTicket =
                await
                    UnitOfWork.ComplainRepo.FindAsync(
                        x => !x.IsDelete && x.OrderId == orderId && x.CustomerId == CustomerState.Id);

            return Json(new { status = Result.Succeed, lstTicket }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy thông tin danh sách lịch sử giao dịch trong đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetTransactionHistoryByOrder(int orderId)
        {
            var lstTransaction =
                await
                    UnitOfWork.RechargeBillRepo.FindAsync(
                        x => !x.IsDelete && x.OrderId == orderId && x.CustomerId == CustomerState.Id);

            return Json(new { status = Result.Succeed, lstTransaction }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy thông tin danh sách kiện hàng trong đơn hàng
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetPackageByOrder(int orderId)
        {
            var lstTransaction =
                await
                    UnitOfWork.OrderPackageRepo.FindAsync(
                        x => !x.IsDelete && x.OrderId == orderId && x.CustomerId == CustomerState.Id);

            return Json(new { status = Result.Succeed, lstTransaction }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thêm mới đơn hàng
        /// </summary>
        /// <param name="listOrderDetails"></param>
        /// <param name="wardDeliveryId"></param>
        /// <param name="listService"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveOrder(List<OrderDetail> listOrderDetails, int wardDeliveryId,
            List<OrderService> listService)
        {
            var dateTime = DateTime.Now;
            var order = new Order();

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //lấy thông tin khách hàng
                    var cus = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(x => x.Id == CustomerState.Id);
                    var wardDelivery =
                        await
                            UnitOfWork.OfficeRepo.FirstOrDefaultAsync(
                                x =>
                                    !x.IsDelete && x.Type == (byte)OfficeType.Warehouse &&
                                    x.Status == (byte)OfficeStatus.Use && x.Culture == "VN" && x.Id == wardDeliveryId);

                    order = new Order
                    {
                        Code = string.Empty,
                        Type = (byte)OrderType.Order,
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
                        WarehouseDeliveryId = wardDelivery.Id,
                        WarehouseDeliveryName = wardDelivery.Name,
                        CustomerId = CustomerState.Id,
                        CustomerName = CustomerState.FullName,
                        CustomerEmail = CustomerState.Email,
                        CustomerPhone = CustomerState.Phone,
                        CustomerAddress = CustomerState.Address,
                        Status = (byte)OrderStatus.WaitPrice,
                        OrderInfoId = 0,
                        FromAddressId = 0,
                        ToAddressId = 0,
                        DepositPercent = UnitOfWork.OrderRepo.CustomerVipLevel(CustomerState.LevelId).Deposit,
                        SystemId = SystemId,
                        SystemName = SystemName,
                        ServiceType = (byte)ServicePack.Business,
                        Created = dateTime,
                        LastUpdate = dateTime
                    };

                    //var firstOrDefault = listOrderDetails.FirstOrDefault();
                    //if (firstOrDefault != null)
                    //{
                    //    if (firstOrDefault.ShopLink != "NULL")
                    //    {
                    //        order.ShopLink = firstOrDefault.ShopLink;
                    //        order.ShopName = firstOrDefault.ShopName;
                    //        order.WebsiteName = MyCommon.GetDomain(firstOrDefault.ShopLink);
                    //    }
                    //}

                    foreach (var item in listOrderDetails)
                    {
                        if (string.IsNullOrEmpty(item.ShopName))
                        {
                            order.ShopLink = item.ShopLink;
                            order.ShopName = item.ShopName?.Trim() ?? "";
                            break;
                        }
                    }

                    //Lấy thông tin website
                    foreach (var item in listOrderDetails)
                    {
                        if (CheckWebsite(item.Link) != -1)
                        {
                            order.WebsiteName = MyCommon.GetDomain(item.Link);
                            break;
                        }
                        order.WebsiteName = MyCommon.GetDomain(item.Link);
                    }

                    UnitOfWork.OrderRepo.Add(order);
                    // Submit thêm order
                    await UnitOfWork.OrderRepo.SaveAsync();
                    List<OrderDetail> listOrderDetailAdd = new List<OrderDetail>();

                    foreach (var item in listOrderDetails)
                    {
                        var shop = await UnitOfWork.ShopRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Url == item.ShopLink);
                        if (item.ShopLink == "NULL")
                        {
                            shop = new Shop();
                        }
                        else
                        {
                            if (shop == null)
                            {
                                shop = new Shop()
                                {
                                    Name = item.ShopName?.Trim() ?? "",
                                    Url = item.ShopLink,
                                    Website = MyCommon.GetDomain(item.ShopLink),
                                    CreateDate = dateTime,
                                    UpdateDate = dateTime,
                                };

                                UnitOfWork.ShopRepo.Add(shop);
                                await UnitOfWork.ShopRepo.SaveAsync();
                            }
                        }

                        var name = item.Name ?? " ";

                        var orderDetail = new OrderDetail()
                        {
                            OrderId = order.Id,
                            UniqueCode = string.Empty,
                            Name = name,
                            Image = item.Image,
                            Quantity = item.Quantity,
                            QuantityBooked = item.Quantity,
                            BeginAmount = item.BeginAmount,
                            Price = item.Price,
                            ExchangeRate = ExchangeRate(),
                            ExchangePrice = item.Price * ExchangeRate(),
                            TotalPrice = item.Price * item.Quantity,
                            TotalExchange = (item.Price * ExchangeRate()) * item.Quantity,
                            AuditPrice = OrderRepository.OrderAudit(item.Quantity, item.Price),
                            ShopId = shop.Id,
                            ShopLink = shop.Url,
                            ShopName = shop.Name?.Trim() ?? "",
                            Note = item.Note,
                            Status = (byte)OrderDetailStatus.Order,
                            Link = item.Link,
                            Properties = item.Properties,
                            Color = item.Color,
                            Size = item.Size,
                            Prices = string.Empty,
                            Min = 1,
                            Max = Int32.MaxValue,
                            SkullId = string.Empty,
                            ProId = string.Empty,
                            Created = dateTime,
                            LastUpdate = dateTime
                        };

                        UnitOfWork.OrderDetailRepo.Add(orderDetail);

                        listOrderDetailAdd.Add(orderDetail);
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // Cập nhật lại Mã cho đơn hàng và tổng tiền
                    var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == cus.Id && x.Id <= order.Id);
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == cus.Id);
                    order.Code = $"{customer.Code}-{orderNo}";

                    order.UnsignName = MyCommon.Ucs2Convert(
                        $"{order.Code} {MyCommon.ReturnCode(order.Code)} {order.CustomerName} {order.CustomerEmail} {order.CustomerPhone} {order.CustomerAddress}");

                    order.LinkNo = await UnitOfWork.OrderDetailRepo.Entities
                        .CountAsync(x => x.OrderId == order.Id && !x.IsDelete);

                    order.ProductNo = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete)
                        .SumAsync(x => x.Quantity);

                    order.TotalExchange = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete)
                        .SumAsync(x => x.TotalExchange);

                    order.TotalPrice = await UnitOfWork.OrderDetailRepo.Entities
                        .Where(x => x.OrderId == order.Id && !x.IsDelete)
                        .SumAsync(x => x.TotalPrice);

                    // Submit cập nhật order
                    await UnitOfWork.OrderRepo.SaveAsync();

                    #region Thêm các dịch vụ cho đơn hàng

                    // Thêm dịch vụ bắt buộc "Mua hàng hộ" cho đơn hàng
                    var totalExchange = order.TotalExchange;
                    var totalPrice = totalExchange * OrderRepository.OrderPrice(order.ServiceType, totalExchange) / 100;

                    // Đơn hàng nhỏ hơn 2 triệu bị tính 150.000 vnđ
                    if (order.TotalExchange < 2000000)
                    {
                        totalPrice = 150000;
                    }

                    //// DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                    //var orderServcie = new OrderService()
                    //{
                    //    OrderId = order.Id,
                    //    ServiceId = (byte)OrderServices.Order,
                    //    ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                    //    ExchangeRate = ExchangeRate(),
                    //    IsDelete = false,
                    //    Created = dateTime,
                    //    LastUpdate = dateTime,
                    //    HashTag = string.Empty,
                    //    Value = OrderRepository.OrderPrice(order.ServiceType, totalExchange),
                    //    Currency = Currency.VND.ToString(),
                    //    Type = (byte)UnitType.Percent,
                    //    //TotalPrice = totalPrice < 5000 ? 5000 : totalPrice,
                    //    TotalPrice = 0,
                    //    Mode = (byte)OrderServiceMode.Required,
                    //    Checked = true
                    //};

                    //UnitOfWork.OrderServiceRepo.Add(orderServcie);

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

                    var orderShopShippingService = new OrderService()
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

                    // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                    var autditService = new OrderService()
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
                        Checked = listService.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Audit).Checked,
                        Created = dateTime,
                        LastUpdate = dateTime
                    };
                    UnitOfWork.OrderServiceRepo.Add(autditService);

                    if (autditService != null)
                    {
                        var totalAuditPrice = await UnitOfWork.OrderDetailRepo.Entities
                            .Where(x => !x.IsDelete && x.OrderId == order.Id && x.AuditPrice != null)
                            .SumAsync(x => x.AuditPrice.Value * x.Quantity);

                        autditService.Value = totalAuditPrice;
                        autditService.TotalPrice = totalAuditPrice;
                    }

                    foreach (var listGroupt in listOrderDetailAdd.GroupBy(x => x.Link))
                    {
                        var productDetails =
                            listOrderDetailAdd.Where(
                                x =>
                                    x.OrderId == order.Id && x.IsDelete == false &&
                                    x.Link == listGroupt.FirstOrDefault().Link).ToList();

                        foreach (var pd in productDetails)
                        {
                            pd.AuditPrice = OrderRepository.OrderAudit(order.ProductNo, pd.Price);
                        }
                    }

                    await UnitOfWork.OrderDetailRepo.SaveAsync();

                    // DỊCH VỤ ĐÓNG KIỆN HÀNG HÓA --------------------------------------------------------------------------
                    var packingService = new OrderService()
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
                        Checked = listService.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Packing).Checked,
                        Created = dateTime,
                        LastUpdate = dateTime
                    };
                    UnitOfWork.OrderServiceRepo.Add(packingService);

                    // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                    var outSideShippingService = new OrderService()
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

                    //// DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                    //var fastDeliveryService = new OrderService()
                    //{
                    //    OrderId = order.Id,
                    //    ServiceId = (byte)OrderServices.FastDelivery,
                    //    ServiceName =
                    //        OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
                    //    ExchangeRate = ExchangeRate(),
                    //    Value = 0,
                    //    Currency = Currency.VND.ToString(),
                    //    Type = (byte)UnitType.Money,
                    //    TotalPrice = 0,
                    //    Mode = (byte)OrderServiceMode.Option,
                    //    Checked =
                    //        listService.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.FastDelivery).Checked,
                    //    Created = dateTime,
                    //    LastUpdate = dateTime
                    //};
                    //UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                    var shipToHomeService = new OrderService()
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

                    //Check phí hàng lẻ
                    if (!CheckWebsiteName(order.WebsiteName))
                    {
                        var retailChargeService = new OrderService()
                        {
                            OrderId = order.Id,
                            ServiceId = (byte)OrderServices.RetailCharge,
                            ServiceName = OrderServices.RetailCharge.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = order.ExchangeRate,
                            Value = 50,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            //TotalPrice = 50 * order.ExchangeRate,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = true,
                            Created = dateTime,
                            LastUpdate = dateTime
                        };
                        UnitOfWork.OrderServiceRepo.Add(retailChargeService);

                        order.IsRetail = true;
                        // Submit cập nhật order
                        await UnitOfWork.OrderRepo.SaveAsync();
                    }

                    #endregion Thêm các dịch vụ cho đơn hàng

                    // Submit thêm OrderService
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    // Cập nhật số lượng tổng
                    var totalService = await UnitOfWork.OrderServiceRepo.Entities.Where(
                        x => x.OrderId == order.Id && !x.IsDelete && x.Checked).SumAsync(x => x.TotalPrice);

                    order.Total = totalService + order.TotalExchange;
                    order.Debt = order.Total;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { status = MsgType.Error, msg = Resource.KhongTheThemDonHang },
                        // return Json(new { status = MsgType.Error, msg = "Không thể thêm đơn hàng!" },
                        JsonRequestBehavior.AllowGet);
                    //throw;
                }
            }

            return Json(new { status = MsgType.Success, msg =Resource.ThemThanhCongDonHang + $" #ORD{order.Code}!" },
            //return Json(new { status = MsgType.Success, msg = $"Thêm thành công đơn hàng #ORD{order.Code}!" },
                JsonRequestBehavior.AllowGet);
        }

        public bool CheckWebsiteName(string website)
        {
            if (website == null)
                return false;
            if (website.Contains("taobao.com"))
                return true;
            if (website.Contains("1688.com"))
                return true;
            if (website.Contains("tmall.com"))
                return true;
            return false;
        }

        public JsonResult AddLink(string url)
        {
            Product pro;
            var website = CheckWebsite(url);
            HtmlWeb htmlWeb = new HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.GetEncoding("gbk"),
            };

            try
            {
                HtmlDocument document = htmlWeb.Load(url);

                switch (website)
                {
                    case (int)Website.Taobao:
                        pro = AddProductTaoBao(document, url);
                        break;

                    case (int)Website.W1688:
                        pro = AddProduct1688(document, url);
                        break;

                    case (int)Website.Tmall:
                        pro = AddProductTmall(document, url);
                        break;

                    default:
                        return Json(new { status = MsgType.Error, msg = Resource.KhongTheLayThongTinDonHang},
                        //return Json(new { status = MsgType.Error, msg = $"Không thể lấy thông tin sản phẩm!" },
                            JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = MsgType.Error, msg = Resource.KhongTheLayThongTinDonHang },
                //return Json(new { status = MsgType.Error, msg = $"Không thể lấy thông tin sản phẩm!" },
                    JsonRequestBehavior.AllowGet);
                //throw;
            }

            return Json(new { status = MsgType.Success, msg = Resource.ThemLinkThanhCong, pro, website },
            //return Json(new { status = MsgType.Success, msg = $"Thêm link thành công!", pro, website },
                JsonRequestBehavior.AllowGet);
        }

        public int CheckWebsite(string url)
        {
            if (url.Contains(EnumHelper.GetEnumDescription<Website>((byte)Website.Taobao)))
                return (int)Website.Taobao;
            if (url.Contains(EnumHelper.GetEnumDescription<Website>((byte)Website.W1688)))
                return (int)Website.W1688;
            if (url.Contains(EnumHelper.GetEnumDescription<Website>((byte)Website.Tmall)))
                return (int)Website.Tmall;
            return -1;
        }

        public string RemoveNumber(string str)
        {
            str = str.Replace(",", "");
            return str;
        }

        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(pText);
        }

        public string ReplaceSizeImg(string img)
        {
            img = img.Replace("20x20", "400x400");
            img = img.Replace("30x30", "400x400");
            img = img.Replace("32x32", "400x400");
            img = img.Replace("40x40", "400x400");
            img = img.Replace("50x50", "400x400");
            img = img.Replace("60x60", "400x400");
            img = img.Replace("70x70", "400x400");
            img = img.Replace("80x80", "400x400");
            img = img.Replace("90x90", "400x400");
            img = img.Replace("100x100", "400x400");
            return img;
        }

        public bool CheckColor(string title)
        {
            string listColor = "顏色 颜色分类 颜色";
            return listColor.Contains(title);
        }

        public bool CheckSize(string title)
        {
            string listSize = "尺碼 尺寸 尺码 参考身高 鞋码 大小描述";
            return listSize.Contains(title);
        }

        /// <summary>
        /// Lấy data website tmall
        /// </summary>
        /// <param name="document"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Product AddProductTmall(HtmlDocument document, string url)
        {
            //1.Khởi tạo dữ liệu
            var product = new Product();
            var note = document.DocumentNode;

            var left = Regex.Split(note.InnerText, @"TShop\.Setup")[1];
            var dataJson = Regex.Split(left, @"\}\)\(\)\;")[0];

            ////2.Gán dữ liệu
            product.DataJson = dataJson.Replace(");", ")");
            product.ProId = (DateTime.UtcNow.Subtract(DateTime.Now)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            product.SkullId = string.Empty;
            product.Rate = ExchangeRate();
            product.ProLink = url;
            product.BeginAmount = "1";

            //lấy ảnh đại diện
            product.Image = note.QuerySelector("img#J_ImgBooth").Attributes["src"].Value;
            //lấy tên sản phẩm
            product.Name = note.QuerySelector("img#J_ImgBooth").Attributes["alt"].Value;
            //lấy list ảnh sản phẩm
            var noteImg = note.QuerySelectorAll("#J_UlThumb img");
            var listImg = new List<dynamic>();
            foreach (var img in noteImg)
            {
                var imgSrc = "";
                imgSrc = img.Attributes["src"] == null ? img.Attributes["data-src"].Value : img.Attributes["src"].Value;
                listImg.Add(new { img = ReplaceSizeImg(imgSrc) });
            }

            var jsonSerialiser = new JavaScriptSerializer();
            product.ListImage = jsonSerialiser.Serialize(listImg);
            //tên, link shop
            if (note.QuerySelector(".hd-shop-name a") != null)
            {
                product.ShopNick = note.QuerySelector(".hd-shop-name a").InnerText;
                product.ShopLink = note.QuerySelector(".hd-shop-name a").Attributes["href"].Value;
            }

            //lấy thuộc tính sản phẩm
            var listProperty = new List<dynamic>();
            var propertyNote = note.QuerySelectorAll("#J_SKU dl, .tb-sku .tm-sale-prop, #J_isku .tb-skin .tb-prop");
            var htmlNodes = propertyNote as HtmlNode[] ?? propertyNote.ToArray();
            foreach (var item in htmlNodes)
            {
                var property = new Property { Type = "other" };

                var title = item.QuerySelector("dt").InnerText;
                property.Title = title;

                if (CheckColor(title))
                {
                    property.Type = "color";
                }
                if (CheckSize(title))
                {
                    property.Type = "size";
                }

                var listChilProperty = new List<ChilProperty>();
                foreach (var chil in item.QuerySelectorAll("dd li"))
                {
                    var chilProperty = new ChilProperty()
                    {
                        Title = chil.QuerySelector("a").QuerySelector("span").InnerText
                    };

                    chilProperty.Properties = chil.Attributes["data-value"].Value;

                    if (chil.QuerySelector("a").Attributes["style"] == null)
                    {
                        chilProperty.Type = "text";
                    }
                    else
                    {
                        chilProperty.Type = "img";
                        chilProperty.Img =
                            ReplaceSizeImg(
                                chil.QuerySelector("a").Attributes["style"].Value.Replace("background:url(", "")
                                    .Replace(") center no-repeat;", ""));
                    }

                    listChilProperty.Add(chilProperty);
                }
                property.ChilProperty = listChilProperty;
                listProperty.Add(property);
            }

            product.ListProperty = jsonSerialiser.Serialize(listProperty);

            //3.Trả dữ liệu
            return product;
        }

        /// <summary>
        /// Lấy data website 1688
        /// </summary>
        /// <param name="document"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Product AddProduct1688(HtmlDocument document, string url)
        {
            //1.Khởi tạo dữ liệu
            var product = new Product();
            var note = document.DocumentNode;

            var left = Regex.Split(note.InnerText, @"iDetailData\ \=")[1];
            var dataJson = Regex.Split(left, @"iDetailData\.allTagIds")[0];
            dataJson = dataJson.Replace(";", "");

            ////2.Gán dữ liệu
            product.DataJson = dataJson;
            product.ProId = (DateTime.UtcNow.Subtract(DateTime.Now)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            product.SkullId = string.Empty;
            product.Rate = ExchangeRate();
            product.ProLink = url;

            //lấy ảnh đại diện
            product.Image = note.QuerySelector(".mod-detail-gallery img").Attributes["src"].Value;
            //lấy tên sản phẩm
            product.Name = note.QuerySelector(".mod-detail-gallery img").Attributes["alt"].Value;
            //lấy list ảnh sản phẩm
            var noteImg = note.QuerySelectorAll("#dt-tab img");
            var listImg = new List<dynamic>();
            foreach (var img in noteImg)
            {
                var imgSrc = "";
                imgSrc = img.Attributes["src"] == null ? img.Attributes["data-src"].Value : img.Attributes["src"].Value;
                listImg.Add(new { img = ReplaceSizeImg(imgSrc) });
            }

            var jsonSerialiser = new JavaScriptSerializer();
            product.ListImage = jsonSerialiser.Serialize(listImg);
            //tên, link shop
            if (note.QuerySelector(".app-common_supplierInfoSmall, .app-import_supplierInfoSmall, .app-smt_supplierInfoSmall, .app-offerdetail_topbar, .app-yuan_supplierInfoSmall") != null)
            {
                try
                {
                    var str = note.QuerySelector(".app-common_supplierInfoSmall, .app-import_supplierInfoSmall, .app-smt_supplierInfoSmall, .app-offerdetail_topbar, .app-yuan_supplierInfoSmall").Attributes["data-view-config"].Value;

                    var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(str);
                    var nick = dict["nick"];
                    product.ShopNick = nick.ToString();
                }
                catch (Exception)
                {
                    if (note.QuerySelector("#usermidid") != null)
                    {
                        product.ShopNick = note.QuerySelector("#usermidid").InnerText;
                    }
                }
            }
            else
            {
                try
                {
                    var str = note.QuerySelector("meta[property='og:product:nick']").Attributes["content"].Value;

                    var nickArray = str.Split(';');
                    foreach (var item in nickArray)
                    {
                        var itemArray = item.Split('=');
                        if (itemArray[0] == "name")
                        {
                            product.ShopNick = itemArray[1];
                        }
                    }
                }
                catch (Exception)
                {
                    if (note.QuerySelector("#usermidid") != null)
                    {
                        product.ShopNick = note.QuerySelector("#usermidid").InnerText;
                    }
                }
            }

            if (note.QuerySelector(".ali-search form, .wp-search form") != null)
            {
                product.ShopLink = note.QuerySelector(".ali-search form, .wp-search form").Attributes["action"].Value;
            }

            //lấy thuộc tính sản phẩm
            var listProperty = new List<dynamic>();
            var propertyNote = note.QuerySelectorAll(".d-content");
            var htmlNodes = propertyNote as HtmlNode[] ?? propertyNote.ToArray();
            foreach (var item in htmlNodes)
            {
                if (item.QuerySelector("span.obj-title") != null)
                {
                    foreach (var itemProperty in item.QuerySelectorAll(".obj-leading, .obj-sku"))
                    {
                        var property = new Property { Type = "other" };

                        var title = itemProperty.QuerySelector("span.obj-title").InnerText;
                        property.Title = title;
                        var listChilProperty = new List<ChilProperty>();

                        if (CheckColor(title))
                        {
                            property.Type = "color";
                        }
                        if (CheckSize(title))
                        {
                            property.Type = "size";
                        }

                        foreach (var chil in itemProperty.QuerySelectorAll(".list-leading .unit-detail-spec-operator"))
                        {
                            var chilProperty = new ChilProperty()
                            {
                                Title = chil.QuerySelector("a").Attributes["title"].Value
                            };
                            if (chil.QuerySelector("a").QuerySelector("img") == null)
                            {
                                chilProperty.Type = "text";
                            }
                            else
                            {
                                chilProperty.Type = "img";
                                chilProperty.Img = ReplaceSizeImg(chil.QuerySelector("a").QuerySelector("img").Attributes["data-lazy-src"].Value);
                            }
                            chilProperty.Properties = chilProperty.Title;
                            listChilProperty.Add(chilProperty);
                        }

                        foreach (var chil in itemProperty.QuerySelectorAll(".table-sku .name"))
                        {
                            var chilProperty = new ChilProperty();

                            if (chil.QuerySelector("img") == null)
                            {
                                chilProperty.Type = "text";
                                chilProperty.Title = chil.QuerySelector("span").InnerText;
                            }
                            else
                            {
                                chilProperty.Type = "img";
                                chilProperty.Title = chil.QuerySelector("img").Attributes["alt"].Value;
                                chilProperty.Img = ReplaceSizeImg(chil.QuerySelector("img").Attributes["data-lazy-src"].Value);
                            }
                            chilProperty.Properties = chilProperty.Title;
                            listChilProperty.Add(chilProperty);
                        }

                        property.ChilProperty = listChilProperty;
                        listProperty.Add(property);
                    }
                }
            }

            //lấy khoảng giá của 1688
            var rangeNote = note.QuerySelector(".widget-custom-container .d-content");
            var listRange = new List<Range>();

            var priceNote = rangeNote.QuerySelectorAll("tr.price td").ToArray();
            var amountNote = rangeNote.QuerySelectorAll("tr.amount td").ToArray();

            for (int i = 1; i < priceNote.Length; i++)
            {
                var range = new Range();
                //khoảng giá
                var checkRange = priceNote[i].QuerySelector("div.price-discount-sku");
                if (checkRange != null)
                {
                    range.Type = "range";
                    foreach (var price in checkRange.QuerySelectorAll("span.value"))
                    {
                        if (string.IsNullOrEmpty(range.Price))
                        {
                            range.BeginPrice = decimal.Parse(price.InnerText, CultureInfo.InvariantCulture);
                            range.EndPrice = decimal.Parse(price.InnerText, CultureInfo.InvariantCulture);
                            range.Price = price.InnerText;
                            range.PriceExchange = (decimal.Parse(price.InnerText, CultureInfo.InvariantCulture) * ExchangeRate()).ToString("N2", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            range.EndPrice = decimal.Parse(price.InnerText, CultureInfo.InvariantCulture);
                            range.Price += " - " + price.InnerText;
                            range.PriceExchange += " - " + (decimal.Parse(price.InnerText, CultureInfo.InvariantCulture) * ExchangeRate()).ToString("N2", CultureInfo.InvariantCulture);
                        }
                    }

                    range.Amount = amountNote[i - 1].QuerySelector("span.value").InnerText;
                    var arrayRangeAmount = range.Amount.Split('-');
                    if (arrayRangeAmount.Length == 1)
                    {
                        range.BeginAmount = int.Parse(arrayRangeAmount[0].Replace("&ge;", ""));
                        range.EndAmount = Int32.MaxValue;
                    }
                    else
                    {
                        range.BeginAmount = int.Parse(arrayRangeAmount[0]);
                        range.EndAmount = int.Parse(arrayRangeAmount[1]);
                    }
                }
                else
                {
                    range.Type = "one";

                    range.Price = priceNote[i].QuerySelector("span.value").InnerText;
                    range.BeginPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);
                    range.EndPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);

                    range.PriceExchange = (decimal.Parse(priceNote[i].QuerySelector("span.value").InnerText, CultureInfo.InvariantCulture) * ExchangeRate()).ToString("N2", CultureInfo.InvariantCulture);

                    if (amountNote.LongCount() != priceNote.LongCount())
                    {
                        range.Amount = amountNote[i - 1].QuerySelector("span.value").InnerText;
                    }
                    else
                    {
                        range.Amount = amountNote[i].QuerySelector("span.value").InnerText;
                    }
                    var arrayRangeAmount = range.Amount.Split('-');
                    if (arrayRangeAmount.Length == 1)
                    {
                        range.BeginAmount = int.Parse(arrayRangeAmount[0].Replace("&ge;", ""));
                        range.EndAmount = Int32.MaxValue;
                    }
                    else
                    {
                        range.BeginAmount = int.Parse(arrayRangeAmount[0]);
                        range.EndAmount = int.Parse(arrayRangeAmount[1]);
                    }
                }

                listRange.Add(range);
            }

            //TH2
            priceNote = rangeNote.QuerySelectorAll("div.obj-price").ToArray();
            amountNote = note.QuerySelectorAll(".widget-custom-container div.obj-amount").ToArray();

            for (int i = 0; i < priceNote.Length; i++)
            {
                var range = new Range
                {
                    Type = "one",
                    Price = priceNote[i].QuerySelector("span.price-now").InnerText,
                    PriceExchange = (decimal.Parse(priceNote[i].QuerySelector("span.price-now").InnerText, CultureInfo.InvariantCulture) * ExchangeRate()).ToString("N2", CultureInfo.InvariantCulture),
                    Amount = "&ge;" + amountNote[i].FirstChild.InnerText.Replace("\"", "").Trim()
                };
                range.BeginAmount = int.Parse(range.Amount.Replace("&ge;", ""));
                range.EndAmount = Int32.MaxValue;
                range.BeginPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);
                range.EndPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);

                listRange.Add(range);
            }

            //TH3
            priceNote = rangeNote.QuerySelectorAll("div.obj-content.row1").ToArray();

            for (int i = 0; i < priceNote.Length; i++)
            {
                var range = new Range
                {
                    Type = "one",
                    Price = priceNote[i].QuerySelector("span.value").InnerText,
                    PriceExchange = (decimal.Parse(priceNote[i].QuerySelector("span.value").InnerText, CultureInfo.InvariantCulture) * ExchangeRate()).ToString("N2", CultureInfo.InvariantCulture),
                    Amount = "1"
                };
                range.BeginAmount = 1;
                range.EndAmount = Int32.MaxValue;
                range.BeginPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);
                range.EndPrice = decimal.Parse(range.Price, CultureInfo.InvariantCulture);

                listRange.Add(range);
            }

            if (note.QuerySelector(".mod-detail-purchasing.mod-detail-purchasing-single") != null)
            {
                product.SingerDataJson = note.QuerySelector(".mod-detail-purchasing.mod-detail-purchasing-single").Attributes["data-mod-config"].Value;
            }

            product.ListProperty = jsonSerialiser.Serialize(listProperty);
            product.ListRange = jsonSerialiser.Serialize(listRange);

            //3.Trả dữ liệu
            return product;
        }

        /// <summary>
        /// Lấy data website taobao
        /// </summary>
        /// <param name="document"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public Product AddProductTaoBao(HtmlDocument document, string url)
        {
            //1.Khởi tạo dữ liệu
            var product = new Product();
            var note = document.DocumentNode;
            if (note.QuerySelector("meta[charset]").Attributes["charset"].Value == "utf-8")
            {
                HtmlWeb htmlWeb = new HtmlWeb()
                {
                    AutoDetectEncoding = false,
                    OverrideEncoding = Encoding.UTF8,
                };

                document = htmlWeb.Load(url);
                note = document.DocumentNode;
            }

            //2.Gán dữ liệu
            if (url.Contains("world.taobao.com"))
            {
                var left = Regex.Split(note.InnerText, @"KISSY\.merge\(g_config\,")[1];
                var dataJson = Regex.Split(left, @"\}\)\;")[0] + "}";
                product.DataJson = dataJson;

                //lấy giá sản phẩm
                product.Price = note.QuerySelector("meta[property='product:price:amount']").Attributes["content"].Value;
            }
            else
            {
                var left = Regex.Split(note.InnerText, @"Hub\.config\.set\(\'sku\'\,")[1];
                var dataJson = Regex.Split(left, @"Hub\.config\.set\(\'desc\'\,")[0];
                product.DataJson = Regex.Split(dataJson, @"\)\;")[0]; ;

                //lấy giá sản phẩm
                product.Price = note.QuerySelector("input[name='current_price']").Attributes["value"].Value;
            }

            //lấy số lượng đặt tối thiểu
            product.BeginAmount = "1";
            product.ProId = (DateTime.UtcNow.Subtract(DateTime.Now)).TotalSeconds.ToString(CultureInfo.InvariantCulture);
            product.SkullId = string.Empty;
            product.Rate = ExchangeRate();
            product.ProLink = url;

            //lấy ảnh đại diện
            product.Image = note.QuerySelector("#J_ThumbView, #J_ImgBooth").Attributes["src"].Value;
            //lấy tên sản phẩm
            if (note.QuerySelector("#J_ThumbView, #J_ImgBooth").Attributes["alt"] == null)
            {
                product.Name = note.QuerySelector("#J_Title .tb-main-title").Attributes["data-title"].Value;
            }
            else
            {
                product.Name = note.QuerySelector("#J_ThumbView, #J_ImgBooth").Attributes["alt"].Value;
            }

            var noteImg = note.QuerySelectorAll("#J_ThumbNav img, #J_UlThumb img");

            //lấy list ảnh sản phẩm
            var listImg = new List<dynamic>();
            foreach (var img in noteImg)
            {
                var imgSrc = "";
                imgSrc = img.Attributes["src"] == null ? img.Attributes["data-src"].Value : img.Attributes["src"].Value;
                listImg.Add(new { img = ReplaceSizeImg(imgSrc) });
            }

            var jsonSerialiser = new JavaScriptSerializer();
            product.ListImage = jsonSerialiser.Serialize(listImg);
            //tên, link shop
            if (note.QuerySelector(".tb-shop-name a, .shop-name a.shop-name-link") != null)
            {
                product.ShopNick = note.QuerySelector(".tb-shop-name a, .shop-name a.shop-name-link").Attributes["title"].Value;
                product.ShopLink = note.QuerySelector(".tb-shop-name a, .shop-name a.shop-name-link").Attributes["href"].Value;
            }

            //lấy thuộc tính sản phẩm
            var listProperty = new List<dynamic>();
            var propertyNote = note.QuerySelectorAll("#J_SKU dl, .tb-sku .tm-sale-prop, #J_isku .tb-skin .tb-prop");
            var htmlNodes = propertyNote as HtmlNode[] ?? propertyNote.ToArray();
            foreach (var item in htmlNodes)
            {
                var property = new Property { Type = "other" };

                var title = item.QuerySelector("dt").InnerText;
                property.Title = title;

                if (CheckColor(title))
                {
                    property.Type = "color";
                }
                if (CheckSize(title))
                {
                    property.Type = "size";
                }

                var listChilProperty = new List<ChilProperty>();
                foreach (var chil in item.QuerySelectorAll("dd li"))
                {
                    var chilProperty = new ChilProperty()
                    {
                        Title = chil.QuerySelector("a").QuerySelector("span").InnerText
                    };

                    chilProperty.Properties = url.Contains("world.taobao.com") ? chil.Attributes["data-pv"].Value : chil.Attributes["data-value"].Value;

                    if (chil.QuerySelector("a").Attributes["style"] == null)
                    {
                        chilProperty.Type = "text";
                    }
                    else
                    {
                        chilProperty.Type = "img";
                        chilProperty.Img = ReplaceSizeImg(chil.QuerySelector("a").Attributes["style"].Value.Replace("background:url(", "").Replace(") center no-repeat;", ""));
                    }

                    listChilProperty.Add(chilProperty);
                }
                property.ChilProperty = listChilProperty;
                listProperty.Add(property);
            }

            product.ListProperty = jsonSerialiser.Serialize(listProperty);

            //3.Trả dữ liệu
            return product;
        }

        #endregion order

        #region Deposit

        //TODO Đơn hàng ký gửi

        public ActionResult DepositOrder()
        {
            ViewBag.ActiveDepositOrder = "cl_on";
            return View();
        }

        //TODO Tạo mới dơn hàng ký gửi

        public ActionResult CreateDeposit()
        {
            ViewBag.ListCategory = GetCatetgoryDepositJsTree();

            var listWarehouseTemp = new List<SelectListItem>();
            var listWarehouseTempDelivery = new List<SelectListItem>();
            var listWarehouse = new List<SelectListItem>();
            var listWarehouseDelivery = new List<SelectListItem>();
            var allWarehouse = UnitOfWork.OfficeRepo.FindAsNoTracking(
                   x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use && x.Culture == "CH");
            foreach (var item in allWarehouse)
            {
                listWarehouseTemp.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }

            listWarehouse.Add(new SelectListItem() { Text = Resource.DetailDeposit_StorageSendOD, Value = "-1" });
            //listWarehouse.Add(new SelectListItem() { Text = "Chọn kho gửi hàng", Value = "-1" });
            listWarehouse.AddRange(listWarehouseTemp.ToList());
            ViewBag.ListWardList = listWarehouse;
            ViewBag.ListWard = JsonConvert.SerializeObject(listWarehouse);

            var allWarehouseDelivery = UnitOfWork.OfficeRepo.FindAsNoTracking(
                  x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse && x.Status == (byte)OfficeStatus.Use && x.Culture == "VN");
            foreach (var item in allWarehouseDelivery)
            {
                listWarehouseTempDelivery.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            listWarehouseDelivery.Add(new SelectListItem() { Text = Resource.CreateOrder_SelectStorage, Value = "-1" });
            listWarehouseDelivery.AddRange(listWarehouseTempDelivery.ToList());
            ViewBag.ListWardDelivery = JsonConvert.SerializeObject(listWarehouseDelivery);

            ViewBag.ActiveDepositOrder = "cl_on";
            //var objFastDeliveryService = new OrderService()
            //{
            //    OrderId = 0,
            //    ServiceId = (byte)OrderServices.FastDelivery,
            //    ServiceName = OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
            //    ExchangeRate = ExchangeRate(),
            //    IsDelete = false,
            //    Created = DateTime.Now,
            //    LastUpdate = DateTime.Now,
            //    HashTag = string.Empty,
            //    Value = 0,
            //    Currency = Currency.VND.ToString(),
            //    Type = (byte)UnitType.Percent,
            //    TotalPrice = 0,
            //    Mode = (byte)OrderServiceMode.Option,
            //    Checked = false
            //};
            var objPackingService = new OrderService()
            {
                OrderId = 0,
                ServiceId = (byte)OrderServices.Packing,
                ServiceName = OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                ExchangeRate = ExchangeRate(),
                IsDelete = false,
                Created = DateTime.Now,
                LastUpdate = DateTime.Now,
                HashTag = string.Empty,
                Value = 0,
                Currency = Currency.VND.ToString(),
                Type = (byte)UnitType.Percent,
                TotalPrice = 0,
                Mode = (byte)OrderServiceMode.Option,
                Checked = false
            };

            //Lấy về kho nhận hàng của Khách
            var customer = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == CustomerState.Id);
            int? warehouseId = 0;
            if (customer != null)
            {
                warehouseId = customer.WarehouseId ?? 0;
            }
            var listService = new List<OrderService>();
            //listService.Add(objFastDeliveryService);
            listService.Add(objPackingService);
            var jsonSerialiser = new JavaScriptSerializer();
            ViewBag.listService = jsonSerialiser.Serialize(listService);
            ViewBag.WarehouseId = warehouseId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddDeposit(int wardId, int wardDeliveryId, List<DepositDetail> listProduct, List<OrderService> listService)
        {
            var result = 0;
            if (listProduct.Count > 0)
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var listTransport = "";
                        decimal totalWeight = 0M;
                        int totalpakage = 0;
                        foreach (var product in listProduct)
                        {
                            decimal tmpWeight = 0;
                            decimal.TryParse((string.IsNullOrEmpty(product.Weight.ToString()) ? "0" : product.Weight.ToString().Replace(".", ",")), out tmpWeight);
                            totalWeight += tmpWeight;

                            totalpakage += product.PacketNumber;
                        }
                        // Lấy về thông tin kho hàng
                        var listWarehouse = new List<SelectListItem>();
                        var allWarehouse = UnitOfWork.OfficeRepo.FindAsNoTracking(
                               x => !x.IsDelete && (x.Type == (byte)OfficeType.Warehouse) && (x.Status == (byte)OfficeStatus.Use));

                        var wardName = "";
                        var wardDeliveryName = "";
                        foreach (var item in allWarehouse)
                        {
                            if (item.Id == wardId)
                            {
                                wardName = item.Name;
                                break;
                            }
                        }
                        foreach (var item in allWarehouse)
                        {
                            if (item.Id == wardDeliveryId)
                            {
                                wardDeliveryName = item.Name;
                                break;
                            }
                        }
                        //TODO[Giỏi]: Bổ xung thêm phần giá fix của dịch vụ ký gửi theo khách hàng
                        var customer = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == CustomerState.Id);
                        if (customer != null)
                        {
                            var deposit = new Order()
                            {
                                Code = string.Empty,
                                Type = (byte)OrderType.Deposit,
                                LevelId = customer.LevelId,
                                LevelName = customer.LevelName,
                                Currency = Currency.CNY.ToString(),
                                ExchangeRate = ExchangeRate(),
                                WarehouseId = wardId,
                                WarehouseName = wardName,
                                WarehouseDeliveryId = wardDeliveryId,
                                WarehouseDeliveryName = wardDeliveryName,
                                CustomerId = customer.Id,
                                CustomerName = customer.FullName,
                                CustomerPhone = customer.Phone,
                                CustomerAddress = customer.Address,
                                CustomerEmail = customer.Email,
                                Status = (byte)DepositStatus.WaitDeposit,
                                SystemId = SystemId,
                                SystemName = SystemName,
                                Note = "",
                                Created = DateTime.Now,
                                LastUpdate = DateTime.Now,
                                ContactAddress = customer.Address,
                                ContactName = customer.FullName,
                                ContactPhone = customer.Phone,
                                ProvisionalMoney = 0,
                                TotalWeight = totalWeight,
                                IsDelete = false,
                                PacketNumber = totalpakage,
                                ApprovelPrice = customer.DepositPrice
                            };
                            UnitOfWork.DepositRepo.Add(deposit);

                            //lấy thông tin phòng của nhân viên phụ trách khách hàng
                            if (customer.UserId != null)
                            {
                                var office = UnitOfWork.OfficeRepo.FirstOrDefault(x => !x.IsDelete && x.Type == (byte)OfficeType.Deposit);
                                if (office != null)
                                {
                                    var userPosition = UnitOfWork.UserPositionRepo.FirstOrDefault(x => x.UserId == customer.UserId.Value && x.OfficeId == office.Id);
                                    deposit.UserId = userPosition.UserId;
                                    deposit.UserFullName = customer.UserFullName;
                                    deposit.OfficeId = userPosition.OfficeId;
                                    deposit.OfficeName = userPosition.OfficeName;
                                    deposit.OfficeIdPath = userPosition.OfficeIdPath;
                                }
                            }

                            // Submit thêm order
                            UnitOfWork.DepositRepo.SaveNoCheck();
                            var totalShip = 0M;
                            foreach (var product in listProduct)
                            {
                                //Lấy về thông tin ngành hàng
                                var category = UnitOfWork.CategoryRepo.FirstOrDefault(s => !s.IsDelete && s.Id == product.CategoryId);
                                if (category == null)
                                {
                                    return Json(new { status = MsgType.Error, msg = "Ngành hàng không tồn tại hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
                                }
                                //var host = Request.Url.Scheme + "://" + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                                //if (!string.IsNullOrEmpty(product.PacketCode))
                                //    listTransport += product.PacketCode + ";";
                                //float tmpWeight = 0;
                                //if (!string.IsNullOrEmpty(product.Image))
                                //{
                                //    product.Image = host + product.Image.Replace(host, "");
                                //}
                                //float.TryParse(product.Weight, out tmpWeight);

                                //Tạo chuỗi mã vận đơn
                                var listCode = "";
                                for (int i = 0; i < product.ListContractCode.Count(); i++)
                                {
                                    if (i > 0)
                                    {
                                        listCode += ";" + product.ListContractCode[i].Code;
                                    }
                                    else
                                    {
                                        listCode += product.ListContractCode[i].Code;
                                    }
                                }
                                var p = new DepositDetail()
                                {
                                    Image = product.Image,
                                    Quantity = product.Quantity,
                                    Note = product.Note,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    DepositId = deposit.Id,
                                    IsDelete = false,
                                    Weight = product.Weight,
                                    CategoryId = product.CategoryId,
                                    CategoryName = category.Name,
                                    ProductName = product.ProductName,
                                    Size = product.Size,
                                    PacketNumber = product.PacketNumber,
                                    Long = product.Long,
                                    High = product.High,
                                    Wide = product.Wide,
                                    ListCode = listCode,
                                    ShipTq = product.ShipTq != null ? (decimal)product.ShipTq : 0
                                };
                                totalShip += product.ShipTq != null ? (decimal)product.ShipTq : 0;
                                UnitOfWork.DepositDetailRepo.Add(p);
                            }
                            if (listTransport.Length > 0)
                            {
                                listTransport = listTransport.TrimEnd(';');
                            }
                            // Submit thêm sản phẩm
                            UnitOfWork.DepositDetailRepo.SaveNoCheck();

                            // Cập nhật lại Mã cho đơn hàng và tổng tiền
                            var depositOfDay = UnitOfWork.DepositRepo.Count(x =>
                                                                             x.Created.Year == deposit.Created.Year
                                                                             && x.Created.Month == deposit.Created.Month
                                                                             && x.Created.Day == deposit.Created.Day
                                                                             && x.Id <= deposit.Id);
                            var tmpCode = $"{depositOfDay}{deposit.Created:ddMMyy}{(byte)OrderType.Deposit}";
                            deposit.Code = tmpCode;
                            deposit.UnsignName = MyCommon.Ucs2Convert(
                        $"{deposit.Code} {MyCommon.ReturnCode(deposit.Code)} {deposit.CustomerName} {deposit.CustomerEmail} {deposit.CustomerPhone} {deposit.CustomerAddress}");
                            var objShipService = new OrderService()
                            {
                                OrderId = deposit.Id,
                                ServiceId = (byte)OrderServices.ShopShipping,
                                ServiceName = OrderServices.ShopShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                IsDelete = false,
                                Created = DateTime.Now,
                                LastUpdate = DateTime.Now,
                                HashTag = string.Empty,
                                Value = totalShip,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Percent,
                                //TotalPrice = totalShip * ExchangeRate(),
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = false
                            };

                            var objOrderService = new OrderService();
                            var objPackingService = new OrderService();
                            foreach (var item in listService)
                            {
                                //if (item.ServiceId == (byte)OrderServices.FastDelivery)
                                //{
                                //    item.OrderId = deposit.Id;
                                //    item.Created = DateTime.Now;
                                //    item.LastUpdate = DateTime.Now;
                                //    UnitOfWork.OrderServiceRepo.Add(item);
                                //}
                                //else
                                //{
                                item.OrderId = deposit.Id;
                                item.Created = DateTime.Now;
                                item.LastUpdate = DateTime.Now;
                                UnitOfWork.OrderServiceRepo.Add(item);
                                //}
                            }

                            var objPackingChargeService = new OrderService()
                            {
                                OrderId = deposit.Id,
                                ServiceId = (byte)OrderServices.PackingCharge,
                                ServiceName = OrderServices.PackingCharge.GetAttributeOfType<DescriptionAttribute>().Description,
                                ExchangeRate = ExchangeRate(),
                                IsDelete = false,
                                Created = DateTime.Now,
                                LastUpdate = DateTime.Now,
                                HashTag = string.Empty,
                                Value = 0,
                                Currency = Currency.VND.ToString(),
                                Type = (byte)UnitType.Percent,
                                TotalPrice = 0,
                                Mode = (byte)OrderServiceMode.Option,
                                Checked = false
                            };
                            UnitOfWork.OrderServiceRepo.Add(objShipService);

                            UnitOfWork.OrderServiceRepo.Add(objPackingChargeService);

                            UnitOfWork.OrderServiceRepo.SaveNoCheck();

                            UnitOfWork.DepositDetailRepo.SaveNoCheck();
                            var arrList = listTransport.Split(';');
                            for (int i = 0; i < arrList.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(arrList[i]))
                                {
                                    var orderPackage = new OrderPackage()
                                    {
                                        Code = string.Empty,
                                        Status = 0,
                                        OrderId = deposit.Id,
                                        OrderCode = deposit.Code,
                                        CustomerId = CustomerState.Id,
                                        CustomerName = deposit.CustomerName,
                                        CustomerUserName = CustomerState.Email,
                                        CustomerLevelId = deposit.LevelId,
                                        CustomerLevelName = deposit.LevelName,
                                        CustomerWarehouseId = deposit.WarehouseId,
                                        CustomerWarehouseName = deposit.WarehouseName,
                                        CustomerWarehouseIdPath = "",
                                        TransportCode = arrList[i],
                                        WarehouseId = deposit.WarehouseId,
                                        WarehouseName = deposit.WarehouseName,
                                        WarehouseIdPath = "",
                                        SystemId = deposit.SystemId,
                                        SystemName = deposit.SystemName,
                                        Created = DateTime.Now,
                                        LastUpdate = DateTime.Now,
                                        HashTag = string.Empty,
                                        PackageNo = 0,
                                        UnsignedText = string.Empty,
                                        OrderType = 1,
                                        IsDelete = false
                                    };

                                    UnitOfWork.OrderPackageRepo.Add(orderPackage);
                                    UnitOfWork.OrderPackageRepo.SaveNoCheck();
                                    // Cập nhật lại Mã cho đơn hàng và tổng tiền
                                    var packageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                                    x.Created.Year == orderPackage.Created.Year && x.Created.Month == orderPackage.Created.Month &&
                                    x.Created.Day == orderPackage.Created.Day && x.Id <= orderPackage.Id);

                                    orderPackage.Code = $"{packageOfDay}{orderPackage.Created:ddMMyy}";
                                    UnitOfWork.DepositDetailRepo.SaveNoCheck();
                                }
                            }
                            result = deposit.Id;
                        }
                        transaction.Commit();

                        Session["DepositAddItem"] = null;
                    }
                    catch (Exception ex)
                    {
                        //OutputLog.WriteOutputLog(ex);
                        transaction.Rollback();
                        return Json(new { status = MsgType.Error, msg = Resource.KhongTheThemDHKyGui , excep = ex }, JsonRequestBehavior.AllowGet);
                        //return Json(new { status = MsgType.Error, msg = "Không thể thêm đơn hàng ký gửi!", excep = ex }, JsonRequestBehavior.AllowGet);
                        //throw;
                    }
                }
            }

            return Json(new { status = MsgType.Success, msg = Resource.ThemThanhCongDHKG  }, JsonRequestBehavior.AllowGet);
            //return Json(new { status = MsgType.Success, msg = $"Thêm thành công đơn hàng ký gửi!" }, JsonRequestBehavior.AllowGet);
        }

        //TODO Lay danh sach cac don hang ký gửi
        [HttpPost]
        public async Task<JsonResult> GetAllDepositList(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new DepositModel();
            model = GetDataDeposit(seachInfor, pageInfor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public DepositModel GetDataDeposit(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new DepositModel();
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            seachInfor.SystemId = SystemId;
            seachInfor.CustomerId = CustomerState.Id;
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" || seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            if (string.IsNullOrEmpty(seachInfor.Keyword))
            {
                seachInfor.Keyword = "";
            }
            seachInfor.OrderType = (byte)OrderType.Deposit;
            model = UnitOfWork.DepositRepo.GetAllByLinq(pageInfor, seachInfor);
            return model;
        }

        [HttpPost]
        public int UpdateStatusDeposit(int orderId, int status, int type)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var obj = UnitOfWork.OrderRepo.SingleOrDefault(x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    if (type == 0)
                    {
                        obj.Status = (byte)DepositStatus.Cancel;
                    }
                    else
                    {
                        if (obj.Status == (byte)DepositStatus.WaitOrder)
                        {
                            obj.Status = (byte)DepositStatus.Order;
                        }
                    }

                    result = UnitOfWork.OrderRepo.SaveNoCheck();
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        #region Comment

        public async Task<int> PaymentBalanceDeposit(int orderId, string pass, string moneyMiss)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var tmpCheck = UnitOfWork.OrderDetailRepo.CheckDeposit(orderId, CustomerState.Id);
                if (tmpCheck == 1)
                {
                    if (!Common.PasswordEncrypt.PasswordEncrypt.EncodePassword(pass.Trim(), Common.Constant.PasswordSalt.FinGroupApiCustomer).Equals(CustomerState.Password))
                    {
                        result = -1;
                    }
                    else
                    {
                        var order = UnitOfWork.OrderRepo.FirstOrDefault(x => x.Id == orderId);
                        decimal tmpMiss;
                        decimal.TryParse(moneyMiss.Replace(",", ""), out tmpMiss);
                        var exchangeRate = ExchangeRate();
                        var timeNow = DateTime.Now;
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
                            Note =Resource.ThanhToanTienTamTinhDKG  + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss),
                            //Note = $"Thanh toán tiền tạm tính đơn ký gửi " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss),
                            OrderId = orderId,
                            TotalPrice = tmpMiss,
                            Status = (byte)OrderExchangeStatus.Approved,
                            OrderType = (byte)OrderExchangeOrderType.Order
                        });

                        // Thêm lịch sử thay đổi trạng thái
                        // Todo: Giỏi bổ xung thêm loại đơn hàng để phân biệt đơn hàng order, ký gửi, tìm nguồn
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeNow,
                            Content = Resource.ThanhToanTienTamTinhDKG + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss),
                            // Content = "Thanh toán tiền tạm tính đơn ký gửi " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss),
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            OrderId = order.Id,
                            Status = order.Status,
                            Type = order.Type
                        });
                        UnitOfWork.OrderHistoryRepo.Save();

                        UnitOfWork.OrderExchangeRepo.SaveNoCheck();
                        UnitOfWork.DepositRepo.PaymentBalance(CustomerState.Id, tmpMiss);

                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            SystemId = SystemId,
                            SystemName = SystemName,
                            CustomerId = CustomerState.Id,
                            CustomerName = CustomerState.FullName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderType = 1, // Thông báo giành cho thay đổi ví kế toán
                            IsRead = false,
                            Title = Resource.ThanhToanTienTamTinhDKG + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss) + " cho đơn hàng" + order.Id.ToString(),
                            //Title = "Thanh toán tiền tạm tính đơn ký gửi  " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss) + " cho đơn hàng" + order.Id.ToString(),
                            Description = Resource.SoDuVDTBiTru + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss) + " BAHT"
                            //Description = "Số dư ví điện tử của bạn bị trừ: " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMiss) + " VNĐ"
                        };

                        UnitOfWork.NotificationRepo.Add(notification);
                        var customer = await UnitOfWork.CustomerRepo.SingleOrDefaultAsync(
                            x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                        // Tài khoản đang bị khóa
                        if (customer != null)
                        {
                            var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                            {
                                CustomerId = customer.Id,
                                CurrencyFluctuations = tmpMiss,
                                OrderId = order.Id,
                                TreasureIdd = (int)EnumAccountantSubject.Customer
                            });
                        }
                        UnitOfWork.NotificationRepo.Save();

                        if (customer != null)
                        {
                            CustomerState.FromUser(customer);
                        }
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        public async Task<ActionResult> UpdateServiceDeposit(int orderId, int serviceId, bool isCheck)
        {
            if (CustomerState == null)
            {
                return PartialView(new OrderDetailModel());
            }

            if (string.IsNullOrEmpty(CustomerState.Email))
            {
                return PartialView(new OrderDetailModel());
            }

            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            if (order != null)
            {
                switch (serviceId)
                {
                    case (byte)OrderServices.Packing:
                        // Cập nhật dịch vụ đóng kiện
                        // phai tinh lai dua tren: gia tri san pham
                        var servicePacking = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.ServiceId == serviceId && x.OrderId == orderId);
                        servicePacking.Checked = isCheck;
                        servicePacking.LastUpdate = DateTime.Now;
                        await UnitOfWork.OrderServiceRepo.SaveAsync();
                        break;
                }
            }
            var model = UnitOfWork.OrderDetailRepo.GetByUpdateService(orderId);
            return PartialView(model);
        }

        #endregion Comment

        public void ExportDeposit(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetDataDeposit(seachInfor, pageInfor);
            var ngay = "";

            var sb = new StringBuilder();
            sb.Append("<table border='1px' cellpadding='1' cellspacing='1' >");
            sb.Append("<tr>");
            sb.Append("<td>#</td>");
            sb.Append("<td>" + Resource.TransactionHistory_Time + "</td>");//sb.Append("<td>Thời gian</td>");
            sb.Append("<td>" + Resource.Deposit_NumberPackage + "</td>");//sb.Append("<td>Số kiện</td>");
            sb.Append("<td>" + Resource.Deposit_TotalWeight + "</td>");// sb.Append("<td>Tổng trọng lượng (Kg)</td>");
            sb.Append("<td>" + Resource.Deposit_ProvisionalAmount + "</td>");// sb.Append("<td>Tiền tạm tính</td>");
            sb.Append("<td>" + Resource.Order_Status + "</td>");// sb.Append("<td>Trạng thái</td>");
            sb.Append("</tr>");
            var index = 0;
            if (model.ListItems.Any())
            {
                for (int i = 0; i < model.ListItems.Count(); i++)
                {
                    var item = model.ListItems[i];
                    var tmpMiss = "0";
                    if (item.Status == (byte)Common.Emums.DepositStatus.Cancel)
                    {
                        tmpMiss = "0";
                    }
                    else
                    {
                        if (item.ProvisionalMoney >= 1)
                        {
                            tmpMiss = string.Format(CultureInfo.GetCultureInfo("en-US"), "{0:#,###}", item.ProvisionalMoney);
                        }
                        else
                        {
                            tmpMiss = "0";
                        }
                    }
                    var tmpStatus = "";
                    switch (item.Status)
                    {
                        case (byte)Common.Emums.DepositStatus.WaitDeposit:
                            tmpStatus = Resource.Order_StatusWaitQuotes;//tmpStatus = "Chờ báo giá";
                            break;

                        case (byte)Common.Emums.DepositStatus.Processing:
                            tmpStatus =Resource.Sourcing_WaitingForProgressing ;
                            // tmpStatus = "Chờ xử lý";
                            break;

                        case (byte)Common.Emums.DepositStatus.PendingPrice:
                            tmpStatus = Resource.Sourcing_WaitingForProgressing;
                            //tmpStatus = "Chờ xử lý";
                            break;

                        case (byte)Common.Emums.DepositStatus.WaitOrder:
                            tmpStatus = Resource.Deposit_StatusPendingApplication; 
                            //tmpStatus = "Chờ kết đơn";
                            break;

                        case (byte)Common.Emums.DepositStatus.Order:
                            tmpStatus = "รอสินค้าเข้าโกดัง";
                            // tmpStatus = "Chờ hàng về kho";
                            break;

                        case (byte)Common.Emums.DepositStatus.InWarehouse:
                            tmpStatus = Resource.Deposit_GoodsInStorage;
                            //  tmpStatus = "Hàng trong kho";
                            break;

                        case (byte)Common.Emums.DepositStatus.Shipping:
                            tmpStatus = Resource.Order_StatusTransport ;
                            // tmpStatus = "Đang vận chuyển";
                            break;

                        case (byte)Common.Emums.DepositStatus.Pending:
                            tmpStatus = Resource.Order_StatusWaitDelivery;
                            //   tmpStatus = "Chờ giao hàng";
                            break;

                        case (byte)Common.Emums.DepositStatus.GoingDelivery:
                            tmpStatus = Resource.Order_StatusDelivered;
                            //  tmpStatus = "Đang giao hàng";
                            break;

                        case (byte)Common.Emums.DepositStatus.Finish:
                            tmpStatus = Resource.Order_StatusComplete;
                            // tmpStatus = "Hoàn thành";
                            break;

                        case (byte)Common.Emums.DepositStatus.Cancel:
                            tmpStatus = Resource.Order_StatusDestroy;
                            // tmpStatus = "Đã hủy";
                            break;

                        default:
                            break;
                    }
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++index);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.code));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0:dd/MM/yyyy}\")", item.CreateDate));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.PacketNumber));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=TEXT(\"{0}\",\"#,###\")", string.Format(CultureInfo.InvariantCulture, "{0:#,###}", item.TotalWeight)));
                    sb.AppendFormat("<td>{0}</td>", tmpMiss);
                    sb.AppendFormat("<td>{0}</td>", tmpStatus);
                    sb.AppendFormat("</tr>").AppendLine();
                }
            }
            sb.Append("</table>");
            var fileName = string.Format("DonKyGui_{0:ddMMyyyy}.xls", DateTime.Now);
            var attachment = "attachment; filename=" + fileName + "";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel"; //office 2003
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        #endregion Deposit

        #region Source

        public ActionResult Sourcing()
        {
            ViewBag.ActiveSourcing = "cl_on";
            return View();
        }

        //TODO tạo mới đơn hàng tìm nguồn

        public ActionResult CreateSourcing()
        {
            ViewBag.ListCategory = GetCatetgoryDepositJsTree();
            ViewBag.ActiveCreateSourcing = "cl_on";
            return View();
        }

        public async Task<JsonResult> SaveSourcing(List<SourceDetail> sourceDetail)
        {
            var timeDate = DateTime.Now;
            var source = new Source();

            ////check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == CustomerState.Id && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Khách hàng không tồn tại!" }, JsonRequestBehavior.AllowGet);
            }

            if (sourceDetail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Chưa có chi tiết đơn hàng!" }, JsonRequestBehavior.AllowGet);
            }
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    source.Id = 0;
                    source.Code = string.Empty;
                    source.SystemId = customer.SystemId;
                    source.SystemName = customer.SystemName;
                    source.WarehouseId = customer.WarehouseId ?? 0;
                    source.WarehouseName = customer.WarehouseName;
                    source.CustomerId = customer.Id;
                    source.CustomerName = customer.FullName;
                    source.CustomerEmail = customer.Email;
                    source.CustomerPhone = customer.Phone;
                    source.CustomerAddress = customer.Address;
                    source.Status = (byte)SourceStatus.WaitProcess;
                    source.CreateDate = timeDate;
                    source.UpdateDate = timeDate;
                    source.TypeService = 0;
                    source.ServiceMoney = 0;
                    source.IsDelete = false;
                    source.ShipMoney = 0m;
                    source.Type = (byte)OrderType.Source;
                    source.UnsignName = string.Empty;

                    UnitOfWork.SourceRepo.Add(source);
                    await UnitOfWork.SourceRepo.SaveAsync();

                    //cập nhật lại mã code
                    var sourceOfDay = UnitOfWork.SourceRepo.Count(
                        x => x.CreateDate.Year == source.CreateDate.Year
                            && x.CreateDate.Month == source.CreateDate.Month
                            && x.CreateDate.Day == source.CreateDate.Day
                            && x.Id <= source.Id
                    );
                    var code = $"{sourceOfDay}{source.CreateDate:ddMMyy}{source.Type}";
                    source.Code = code;
                    source.UnsignName = MyCommon.Ucs2Convert($"{source.Code} {MyCommon.ReturnCode(source.Code)} {source.CustomerName} {source.WarehouseName}").ToLower();

                    await UnitOfWork.SourceRepo.SaveAsync();

                    //chi tiết đơn hàng
                    foreach (var item in sourceDetail)
                    {
                        var category = UnitOfWork.CategoryRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.CategoryId);
                        if (category == null)
                        {
                            return Json(new { status = MsgType.Error, msg = "Ngành hàng không tồn tại hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
                        }
                        item.Created = timeDate;
                        item.LastUpdate = timeDate;
                        item.SourceId = source.Id;
                        item.ExchangeRate = ExchangeRate();
                        item.IsDelete = false;
                        item.CategoryName = category.Name;

                        UnitOfWork.SourceDetailRepo.Add(item);
                    }

                    await UnitOfWork.SourceDetailRepo.SaveAsync();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    //throw;

                    return Json(new { status = MsgType.Error, msg = Resource.KhongTheThemDonHang }, JsonRequestBehavior.AllowGet);
                    // return Json(new { status = MsgType.Error, msg = "Không thể thêm đơn hàng này!" }, JsonRequestBehavior.AllowGet);
                }
            }

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Thêm phiếu tìm nguồn thành công", list = sourceDetail }, JsonRequestBehavior.AllowGet);
        }

        //TODO CHI TIẾT ĐƠN HÀNG TÌM NGUỒN
        public ActionResult DetailSourcing()
        {
            ViewBag.ActiveSourcing = "cl_on";
            return View();
        }

        //TODO Lay danh sach cac don hang tìm nguồn
        [HttpPost]
        public JsonResult GetAllResourceList(SearchInfor seachInfor, PageItem pageInfor)
        {
            return Json(GetDataSource(seachInfor, pageInfor), JsonRequestBehavior.AllowGet);
        }

        public SourceModel GetDataSource(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new SourceModel();
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            seachInfor.SystemId = SystemId;
            seachInfor.CustomerId = CustomerState.Id;
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" || seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            if (string.IsNullOrEmpty(seachInfor.Keyword))
            {
                seachInfor.Keyword = "";
            }
            model = UnitOfWork.SourceRepo.GetAllByLinq(pageInfor, seachInfor);
            return model;
        }

        public ActionResult Detail(long id)
        {
            if (CustomerState == null)
            {
                return RedirectToAction("Login", "Account", new { @returnUrl = $"/{System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()}/tai-khoan/chi-tiet-tim-nguon-" + id.ToString() });
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return RedirectToAction("Login", "Account", new { @returnUrl = $"/{System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant()}/tai-khoan/chi-tiet-tim-nguon-" + id.ToString() });
                }
            }
            var model = UnitOfWork.SourceDetailRepo.GetAllByLinq(id, (byte)OrderType.Source);
            ViewBag.CustomerName = CustomerState.FullName;
            return View(model);
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> UpdateSupplier(long sourceId, long supplierId)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return 0;
                }
            }
            try
            {
                var obj = await UnitOfWork.SourceRepo.SingleOrDefaultAsync(x => x.Id == sourceId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    if (obj.Status != (byte)Common.Emums.SourceStatus.Success || obj.Status != (byte)Common.Emums.SourceStatus.Cancel)
                    {
                        obj.SourceSupplierId = supplierId;
                        result = await UnitOfWork.SourceRepo.SaveAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        /// <summary>
        /// Thanh toán tiền cho đơn hàng tìm nguồn
        /// </summary>
        /// <param name="sourceServiceId"></param>
        /// <returns></returns>
        public int UpdateBalanceAvalible(int sourceServiceId)
        {
            if (CustomerState == null)
            {
                return 0;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return 0;
                }
            }
            var result = 0;
            var tmpMoney = UnitOfWork.SourceServiceRepo.UpdateBalanceAvalible(CustomerState.Id, sourceServiceId);
            if (tmpMoney > 0)
            {
                var exchangeRate = ExchangeRate();
                var timeNow = DateTime.Now;
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
                    Note = $"Thanh toán tiền tìm nguồn " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMoney) + " BAHT",
                    OrderId = sourceServiceId,
                    TotalPrice = tmpMoney,
                    Status = (byte)OrderExchangeStatus.Approved,
                    OrderType = (byte)OrderExchangeOrderType.Order
                });

                // Gửi thông báo Notification cho khách hàng
                var notification = new Notification()
                {
                    SystemId = SystemId,
                    SystemName = SystemName,
                    CustomerId = CustomerState.Id,
                    CustomerName = CustomerState.FullName,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    OrderType = 2, // Thông báo giành cho thay đổi ví kế toán
                    IsRead = false,
                    Title = "Thanh toán tiền tìm nguồn  " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMoney) + " BAHT",
                    Description = Resource.SoDuVDTBiTru + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMoney) + " BAHT"
                    //Description = "Số dư ví điện tử của bạn bị trừ: " + string.Format(System.Globalization.CultureInfo.GetCultureInfo("en-US"), "{0:###,###}", tmpMoney) + " VNĐ"
                };

                UnitOfWork.NotificationRepo.Add(notification);
                var customer = UnitOfWork.CustomerRepo.SingleOrDefault(
                            x => x.IsActive && !x.IsDelete && !x.IsLockout && x.Id == CustomerState.Id);

                // Tài khoản đang bị khóa
                if (customer != null)
                {
                    // Thanh toán tiền
                    var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                    {
                        CustomerId = customer.Id,
                        CurrencyFluctuations = tmpMoney,
                        OrderId = sourceServiceId,
                        TreasureIdd = (int)EnumAccountantSubject.Customer
                    });
                }
                UnitOfWork.NotificationRepo.Save();

                result = 1;
            }

            return result;
        }

        [HttpPost]
        public async Task<int> UpdateStatusSource(int orderId)
        {
            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            try
            {
                var obj = await UnitOfWork.SourceRepo.SingleOrDefaultAsync(x => x.Id == orderId && !x.IsDelete && x.CustomerId == CustomerState.Id);
                if (obj != null)
                {
                    obj.Status = (byte)SourceStatus.Cancel;
                    result = await UnitOfWork.SourceRepo.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        public void ExportSource(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = GetDataSource(seachInfor, pageInfor);
            var ngay = "";

            var sb = new StringBuilder();
            sb.Append("<table border='1px' cellpadding='1' cellspacing='1' >");
            sb.Append("<tr>");
            sb.Append("<td>STT</td>");
            sb.Append("<td>Mã đơn</td>");
            sb.Append("<td>" + Resource.TransactionHistory_Time + "</td>");
            sb.Append("<td>Số lượng</td>");
            sb.Append("<td>Gói dịch vụ</td>");
            sb.Append("<td>" + Resource.Order_Status + "</td>");
            sb.Append("</tr>");
            var index = 0;
            if (model.ListItems.Any())
            {
                for (int i = 0; i < model.ListItems.Count(); i++)
                {
                    var item = model.ListItems[i];

                    var tmpStatus = "";
                    switch (item.Status)
                    {
                        case (byte)Common.Emums.SourceStatus.WaitProcess:
                            tmpStatus = "รอจัดการ";
                            // tmpStatus = "Chờ xử lý";
                            break;

                        case (byte)Common.Emums.SourceStatus.Process:
                            tmpStatus = "Đang xử lý";
                            break;

                        case (byte)Common.Emums.SourceStatus.WaitingChoice:
                            tmpStatus = "Chờ khách chọn NCC";
                            break;

                        case (byte)Common.Emums.SourceStatus.Success:
                            tmpStatus = Resource.Order_StatusComplete;
                            //tmpStatus = "Hoàn thành";
                            break;

                        case (byte)Common.Emums.SourceStatus.Cancel:

                            tmpStatus = "ยกเลิกแล้ว"; //tmpStatus = "Đã hủy";
                            break;

                        default:
                            break;
                    }
                    sb.AppendFormat("<tr>");
                    sb.AppendFormat("<td>{0}</td>", ++index);
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.code));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0:dd/MM/yyyy}\")", item.CreateDate));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.Quantity));
                    sb.AppendFormat("<td>{0}</td>", string.Format("=LOWER(\"{0}\")", item.TypeServiceName));
                    sb.AppendFormat("<td>{0}</td>", tmpStatus);
                    sb.AppendFormat("</tr>").AppendLine();
                }
            }
            sb.Append("</table>");
            var fileName = string.Format("DonTimNguon_{0:ddMMyyyy}.xls", DateTime.Now);
            var attachment = "attachment; filename=" + fileName + "";
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/ms-excel"; //office 2003
            Response.ContentEncoding = Encoding.Unicode;
            Response.BinaryWrite(Encoding.Unicode.GetPreamble());
            Response.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        #endregion Source

        //TODO Lấy danh sách các kiện hàng
        [HttpPost]
        public async Task<JsonResult> GetAllOrderPackageList(int page, int pageSize, string keyword, int status, DateTime? dateStart, DateTime? dateEnd)
        {
            ////todo khai bao
            long totalRecord;

            var orderPackageModal = await UnitOfWork.OrderPackageRepo.FindAsync(
                                                                                    out totalRecord,
                                                                                    s => !s.IsDelete
                                                                                    && s.Code.Contains(keyword)
                                                                                    && (status == -1) || (s.Status == status)
                                                                                    && (dateStart == null) || s.Created >= dateStart
                                                                                    && (dateEnd == null) || s.Created <= dateEnd,
                                                                                    s => s.OrderByDescending(x => x.Created),
                                                                                    page,
                                                                                    pageSize
                                                                                    );
            return Json(CustomerState == null ?
                new { status = Result.Failed, msg = "รับข้อผิดพลาดข้อมูล !", totalRecord, orderPackageModal }
                //new { status = Result.Failed, msg = "Lấy thông tin lỗi !", totalRecord, orderPackageModal }
                : new { status = Result.Succeed, msg = "ได้รับความสำเร็จข้อมูล !", totalRecord, orderPackageModal },
                //: new { status = Result.Succeed, msg = "Lấy thông tin thành công !", totalRecord, orderPackageModal },
                JsonRequestBehavior.AllowGet);
        }

        #region Detail

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetOrderDetails(int orderId)
        {
            //var model = UnitOfWork.OrderDetailRepo.GetAllByLinq(orderId, (byte)OrderType.Order, CustomerState.Id);
            //ViewBag.CustomerName = CustomerState.FullName;

            //0. Khai báo modal chi tiết đơn hàng được phép hiển thị
            var orderDetailModel = new OrderDetailModel();

            //1. Lấy thông tin đơn hàng đơn hàng
            var orderDetail = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => x.Id == orderId && !x.IsDelete);
            if (orderDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "สั่งซื้อสินค้าที่ไม่มีอยู่หรือถูกลบออก !" }, JsonRequestBehavior.AllowGet);
                // return Json(new { status = Result.Failed, msg = "Đơn hàng không tồn tại, hoặc đã bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra xem đơn hàng có phải là của người dùng này hay không
            if (orderDetail.CustomerId != CustomerState.Id)
            {
                return Json(new { status = Result.Failed, msg = "คุณไม่สามารถเข้าถึงเมนูประจำวัน!" }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = Result.Failed, msg = "Bạn không có quyền truy nhập đơn hàng này!" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy địa chỉ giao hàng cho khách
            var orderAddress = await UnitOfWork.OrderAddressRepo.FirstOrDefaultAsync(x => x.Id == orderDetail.ToAddressId);

            //4. Lấy thông tin chi tiết sản phẩm trong đơn hàng
            var orderProducts = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == orderId && x.CustomerId == CustomerState.Id);

            //5. Lấy danh sách sản phẩm trong đơn hàng
            var listOrderDetail = await UnitOfWork.OrderDetailRepo.FindAsync(x => x.OrderId == orderId && !x.IsDelete);

            //5.1. Lấy thông tin chi tiết dịch vụ sử dụng trong đơn hàng
            var orderServicesView = await UnitOfWork.OrderServiceRepo.FindAsync(x => !x.IsDelete && x.OrderId == orderId);

            //5.2. Lấy thông tin chi tiết dịch vụ sử dụng trong đơn hàng
            //bo di bay
            //var orderServices = orderServicesView.Where(x => x.ServiceId == (byte)Common.Emums.OrderServices.Audit || x.ServiceId == (byte)Common.Emums.OrderServices.Packing || x.ServiceId == (byte)Common.Emums.OrderServices.FastDelivery).ToList();
            var orderServices = orderServicesView.Where(x => x.ServiceId == (byte)Common.Emums.OrderServices.Audit || x.ServiceId == (byte)Common.Emums.OrderServices.Packing).ToList();

            //6. Lấy thông tin thanh toán trong đơn hàng
            var orderExchange = await UnitOfWork.OrderExchangeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.OrderId == orderId && x.Type == (byte)OrderExchangeType.Product);

            //7. Lấy thông tin các kiện hàng trong đơn hàng
            var orderPackage = UnitOfWork.OrderPackageRepo.GetOrderPackageForDetail(orderId, orderDetail.Type);
            //8. Lấy mã khiếu nại
            // luon tao khieu nai, khong check va refer sang chi tiet don
            //var tmpComplain = UnitOfWork.ComplainRepo.Find(x => x.OrderId == orderId && !x.IsDelete).OrderByDescending(m => m.CreateDate).FirstOrDefault();
            //var isComplain = 0L;
            //if (tmpComplain != null)
            //{
            //    isComplain = tmpComplain.Id;
            //}
            var isComplain = 0;
            //9. lay danh sach lich su giao dich
            var recharge = UnitOfWork.RechargeBillRepo.Find(m => m.CustomerId == CustomerState.Id && !m.IsDelete && m.OrderId == orderId).OrderByDescending(m => m.Created).ToList();
            //10: lay danh sach khieu nai
            var complains = UnitOfWork.ComplainRepo.Find(m => m.CustomerId == CustomerState.Id && !m.IsDelete && m.OrderId == orderId).OrderByDescending(m => m.CreateDate).ToList();

            return Json(new { status = Result.Succeed, orderDetail, orderAddress, orderProducts, listOrderDetail, orderServices, orderServicesView, orderExchange, orderPackage, isComplain, recharge, complains }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPackageHistory(long id)
        {
            var model = new List<PackageHistory>();
            var objPack = UnitOfWork.PackageHistoryRepo.FirstOrDefault(m => m.PackageId == id && m.CustomerId == CustomerState.Id);
            if (objPack != null)
            {
                model = UnitOfWork.PackageHistoryRepo.Find(m => m.PackageId == objPack.PackageId).OrderBy(m => m.CreateDate).ToList();
            }
            return PartialView(model);
        }

        public ActionResult GetPackageWallet(int id)
        {
            var model = new List<OrderPackageWalletItem>();
            var objPack = UnitOfWork.WalletDetailRepo.FirstOrDefault(m => m.PackageId == id);
            if (objPack != null)
            {
                model = UnitOfWork.WalletDetailRepo.Find(m => m.WalletId == objPack.WalletId)
                    .Select(m => new OrderPackageWalletItem()
                    {
                        Status = m.Status,
                        PackageCode = m.PackageCode
                    })
                    .ToList();
            }
            return PartialView(model);
        }

        public ActionResult GetServiceOther(int id)
        {
            var model = new List<OrderServiceOther>();
            model = UnitOfWork.OrderServiceOtherRepo.Find(m => m.OrderId == id).OrderBy(m => m.Created).ToList();
            return PartialView(model);
        }

        //TODO chi tiết đơn hàng order
        public ActionResult DetailDeposit()
        {
            ViewBag.ActiveBuyOrder = "cl_on";
            return View();
        }

        //Chi tiết đơn hàng ký gửi
        [HttpPost]
        public ActionResult DetailDeposit(int orderId)
        {
            var model = UnitOfWork.DepositRepo.GetDetailByLinq(orderId, (byte)OrderType.Deposit);
            //8. Lấy mã khiếu nại
            //var tmpComplain = UnitOfWork.ComplainRepo.Find(x => x.OrderId == orderId && !x.IsDelete).OrderByDescending(m => m.CreateDate).FirstOrDefault();
            //var isComplain = 0L;
            //if (tmpComplain != null)
            //{
            //    isComplain = tmpComplain.Id;
            //}
            var isComplain = 0;
            ViewBag.modelDeposit = JsonConvert.SerializeObject(model);

            ViewBag.CustomerName = CustomerState.FullName;
            return Json(new { model, isComplain }, JsonRequestBehavior.AllowGet);
        }

        //Chi tiết đơn hàng tìm nguồn
        [HttpPost]
        public ActionResult DetailSourcing(int orderId)
        {
            var typeSourcing = (byte)OrderType.Source;
            var model = UnitOfWork.SourceDetailRepo.GetAll(orderId, (byte)OrderType.Source);
            ViewBag.CustomerName = CustomerState.FullName;
            return Json(new { model, typeSourcing }, JsonRequestBehavior.AllowGet);
        }

        #endregion Detail

        [HttpPost]
        public JsonResult FileUploadDeposit()
        {
            var result = "";
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i]; //Uploaded file
                if (file != null)
                {
                    int iFileSize = file.ContentLength;
                    if (iFileSize > 2097152)  // 2MB
                    {
                        break;
                    }
                    var extension = Path.GetExtension(file.FileName);
                    var firstName = Path.GetFileNameWithoutExtension(file.FileName);
                    string fileName = file.FileName;
                    string path = @"/Areas/Cms/Content/Deposit/" + CustomerState.Email.Substring(0, CustomerState.Email.IndexOf('@')) + "/";
                    if (!Directory.Exists(Server.MapPath(path)))
                    {
                        Directory.CreateDirectory(Server.MapPath(path));
                    }
                    string filePath = path + fileName;
                    if (System.IO.File.Exists(Server.MapPath(filePath)))
                    {
                        Random rnd = new Random();
                        int number = rnd.Next(1000);
                        filePath = path + firstName + number.ToString() + extension;
                    }
                    fileName = Server.MapPath(filePath);
                    file.SaveAs(fileName);
                    result = filePath;
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }

    public class Product
    {
        public string ProId { get; set; }
        public string SkullId { get; set; }
        public decimal Rate { get; set; }
        public string ProLink { get; set; }
        public string Image { get; set; }
        public string ListImage { get; set; }
        public string ListProperty { get; set; }
        public string ListRange { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string PriceArr { get; set; }
        public string Size { get; set; }
        public string Sizetxt { get; set; }
        public string Color { get; set; }
        public string Colortxt { get; set; }
        public string Amount { get; set; }
        public string BeginAmount { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public string ShopNick { get; set; }
        public string ShopLink { get; set; }
        public string Site { get; set; }
        public string Domain { get; set; }
        public string Note { get; set; }
        public string Count { get; set; }
        public string Method { get; set; }
        public string DataJson { get; set; }
        public string SingerDataJson { get; set; }
    }

    public class Property
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public List<ChilProperty> ChilProperty { get; set; }
    }

    public class ChilProperty
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Properties { get; set; }
        public string Img { get; set; }
    }

    public class Range
    {
        public decimal BeginPrice { get; set; }
        public decimal EndPrice { get; set; }
        public string Price { get; set; }
        public int BeginAmount { get; set; }
        public int EndAmount { get; set; }
        public string Amount { get; set; }
        public string PriceExchange { get; set; }
        public string Type { get; set; }
    }
}