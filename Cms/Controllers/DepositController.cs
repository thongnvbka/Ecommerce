using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cms.Attributes;
using Common.Constant;
using Common.Emums;
using Common.Helper;
using Library.DbContext.Entities;
using Library.Jobs;
using Library.DbContext.Repositories;

namespace Cms.Controllers
{
    [Authorize]
    public class DepositController : BaseController
    {
        #region [1. Orders ký gửi]
        /// <summary>
        /// lay danh sach don hang ky gui moi
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
        [LogTracker(EnumAction.View, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> GetOrderDepositNew(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart,
            DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (x.Status == (byte)DepositStatus.WaitDeposit)
                    && x.Type == (byte)OrderType.Deposit,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.Code == (keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (x.Status == (byte)DepositStatus.WaitDeposit)
                    && x.Type == (byte)OrderType.Deposit,
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

        /// <summary>
        /// Lay danh sach don hang ky gui
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
        [LogTracker(EnumAction.View, EnumPage.OrderDeposit)]
        public async Task<JsonResult> GetOrderDeposit(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart,
            DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && (x.Status > (byte)DepositStatus.WaitDeposit)
                    && x.Type == (byte)OrderType.Deposit
                    && x.UserId != null,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.Code == (keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && (x.Status > (byte)DepositStatus.WaitDeposit)
                    && x.Type == (byte)OrderType.Deposit
                    && x.UserId != null,
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

        /// <summary>
        /// lay danh sach don hang ky gui che xu ly
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
        [LogTracker(EnumAction.View, EnumPage.OrderDepositDelay)]
        public async Task<JsonResult> GetOrderDepositDelay(int page, int pageSize, string keyword, int status, int systemId, DateTime? dateStart,
            DateTime? dateEnd, int? userId, int? customerId, bool checkExactCode)
        {
            //1. Khởi tạo các biến
            long totalRecord;
            var dateDelay = DateTime.Now.AddMinutes(TimeDelay);

            //2. Lấy dữ liệu
            var listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.UnsignName.Contains(keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && (x.Status == (byte)DepositStatus.Order)
                    && x.Type == (byte)OrderType.Deposit
                    && (x.Created <= dateDelay)
                    && x.UserId != null,
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
            );
            if (checkExactCode)
            {
                keyword = RemoveCode(keyword);
                listOrder = await UnitOfWork.OrderRepo.FindAsync(
                out totalRecord,
                x => (status == -1 || x.Status == status)
                    && (systemId == -1 || x.SystemId == systemId)
                    && x.Code == (keyword)
                    && (dateStart == null || x.Created >= dateStart)
                    && (dateEnd == null || x.Created <= dateEnd)
                    && !x.IsDelete
                    && (customerId == null || x.CustomerId == customerId)
                    && (userId == null || x.UserId == userId)
                    && (UserState.Type == 0 || (x.OfficeIdPath == UserState.OfficeIdPath || x.OfficeIdPath.StartsWith(UserState.OfficeIdPath + ".")))
                    && (UserState.Type != 0 || x.UserId == UserState.UserId)
                    && (x.Status == (byte)DepositStatus.Order)
                    && x.Type == (byte)OrderType.Deposit
                    && (x.Created <= dateDelay)
                    && x.UserId != null,
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

        /// <summary>
        /// lay thong tin chi tiet don hang
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> GetDepositDetail(int id)
        {
            //1. Kiểm tra thông tin Orders ký gửi
            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == id);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == deposit.CustomerId && !x.IsDelete);
            var listDetail = await UnitOfWork.DepositDetailRepo.FindAsync(x => x.DepositId == deposit.Id && !x.IsDelete);
            var listPackageView = await UnitOfWork.OrderPackageRepo.FindAsync(
                x => x.OrderId == deposit.Id
                    && !x.IsDelete
                    && x.OrderType == deposit.Type
            );
            var listHistory = await UnitOfWork.OrderHistoryRepo.FindAsync(x => x.OrderId == deposit.Id && x.Type == deposit.Type, query => query.OrderByDescending(m => m.CreateDate), null);
            var userOrder = deposit.UserId != null ? await UnitOfWork.UserRepo.GetUserToOfficeOrder(deposit.UserId.Value) : null;
            var listOrderService = UnitOfWork.OrderServiceRepo.Find(x => !x.IsDelete && x.OrderId == deposit.Id /*&& x.ServiceId != (byte)OrderServices.Order*/ && x.ServiceId != (byte)OrderServices.Audit && x.Checked).ToList();
            var listOrderServiceOther = UnitOfWork.OrderServiceOtherRepo.Find(x => x.OrderId == deposit.Id).ToList();

            var listOrderServiceCheck = new List<OrderService>();

            //var fastDeliveryService = UnitOfWork.OrderServiceRepo.FirstOrDefault(x => !x.IsDelete && x.OrderId == deposit.Id && x.ServiceId == (byte)OrderServices.FastDelivery);
            //listOrderServiceCheck.Add(fastDeliveryService);

            var packingService = UnitOfWork.OrderServiceRepo.FirstOrDefault(x => !x.IsDelete && x.OrderId == deposit.Id && x.ServiceId == (byte)OrderServices.Packing);
            listOrderServiceCheck.Add(packingService);

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "", deposit, listDetail, customer, listPackageView, listHistory, userOrder, listOrderService, listOrderServiceCheck, listOrderServiceOther }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// lay cac thong tin co ban
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.View, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> GetData()
        {
            var timeDate = DateTime.Now;
            var exchangeRate = ExchangeRate();
            //kho ký gửi
            var listWarehouseDeposit = await UnitOfWork.OfficeRepo.FindAsync(
                x => !x.IsDelete && x.Type == (byte)OfficeType.Warehouse
                    && x.Culture == "CH"
            );
            //kho giao hàng
            var listWarehouseDelivery = await UnitOfWork.OfficeRepo.FindAsync(
                x => !x.IsDelete
                    && x.Type == (byte)OfficeType.Warehouse
                    && x.Culture == "VN"
            );

            //thêm dịch vụ
            var listOrderServiceCheck = new List<OrderService>();
            // DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------

            //var fastDeliveryService = new OrderService()
            //{
            //    OrderId = 0,
            //    ServiceId = (byte)OrderServices.FastDelivery,
            //    ServiceName =
            //            OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
            //    ExchangeRate = ExchangeRate(),
            //    Value = 0,
            //    Currency = Currency.VND.ToString(),
            //    Type = (byte)UnitType.Money,
            //    TotalPrice = 0,
            //    Mode = (byte)OrderServiceMode.Option,
            //    Checked = false,
            //    Created = timeDate,
            //    LastUpdate = timeDate
            //};
            //listOrderServiceCheck.Add(fastDeliveryService);

            var packingService = new OrderService()
            {
                OrderId = 0,
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
                Created = timeDate,
                LastUpdate = timeDate
            };
            listOrderServiceCheck.Add(packingService);


            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "", exchangeRate, listWarehouseDeposit, listWarehouseDelivery, listOrderServiceCheck }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// luu don hang them moi
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="listDetails"></param>
        /// <param name="orderInfoId"></param>
        /// <param name="depositType"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> Save(Order deposit, List<DepositDetail> listDetails, bool? orderInfoId, bool? depositType, List<OrderService> listOrderServiceCheck)
        {
            var timeDate = DateTime.Now;

            //check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == deposit.CustomerId && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Chưa chọn khách hàng!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho nhận ký gửi
            var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseId);

            if (warehouseStart == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select consignment warehouse!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho giao hàng
            var warehouseEnd = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseDeliveryId);

            if (warehouseEnd == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select warehouse to deliver!" }, JsonRequestBehavior.AllowGet);
            }

            if (!listDetails.Any())
            {
                return Json(new { status = MsgType.Error, msg = "Chưa có chi tiết đơn hàng!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    deposit.Id = 0;
                    deposit.Code = string.Empty;
                    deposit.Created = timeDate;
                    deposit.LastUpdate = timeDate;
                    deposit.LevelId = customer.LevelId;
                    deposit.LevelName = customer.LevelName;
                    deposit.UserId = UserState.UserId;
                    deposit.UserName = UserState.UserName;
                    deposit.UserFullName = UserState.FullName;
                    deposit.OfficeId = UserState.OfficeId;
                    deposit.OfficeName = UserState.OfficeName;
                    deposit.OfficeIdPath = UserState.OfficeIdPath;
                    deposit.Type = (byte)OrderType.Deposit;
                    deposit.PacketNumber = 0;
                    deposit.Status = (byte)DepositStatus.Processing;
                    deposit.SystemId = customer.SystemId;
                    deposit.SystemName = customer.SystemName;
                    deposit.ProvisionalMoney = 0;
                    deposit.TotalWeight = 0;
                    deposit.Currency = "VND";
                    deposit.ExchangeRate = ExchangeRate();
                    deposit.IsDelete = false;
                    deposit.WarehouseName = warehouseStart.Name;
                    deposit.WarehouseDeliveryName = warehouseEnd.Name;

                    UnitOfWork.OrderRepo.Add(deposit);
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Detail Orders
                    foreach (var item in listDetails)
                    {
                        item.CreateDate = timeDate;
                        item.UpdateDate = timeDate;
                        item.DepositId = deposit.Id;
                        item.Size = $"{item.Long}x{item.Wide}x{item.High}";
                        item.IsDelete = false;

                        UnitOfWork.DepositDetailRepo.Add(item);
                    }

                    await UnitOfWork.DepositDetailRepo.SaveAsync();

                    // Cập nhật lại Mã cho Orders
                    var orderNo = UnitOfWork.OrderRepo.Count(x => x.CustomerId == customer.Id && x.Id <= deposit.Id);
                    deposit.Code = $"{customer.Code}-{orderNo}";

                    deposit.UnsignName = MyCommon.Ucs2Convert($"{deposit.Code} {MyCommon.ReturnCode(deposit.Code)} {deposit.CustomerName} {deposit.WarehouseName} {deposit.WarehouseDeliveryName}").ToLower();
                    deposit.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);

                    if (deposit.ApprovelPrice != null)
                        deposit.ProvisionalMoney = (decimal)(deposit.TotalWeight * deposit.ApprovelPrice);

                    deposit.PacketNumber = listDetails.Sum(x => x.PacketNumber);

                    if (orderInfoId != null)
                    {
                        if (orderInfoId == true)
                        {
                            var detail =
                                await UnitOfWork.DepositDetailRepo.FirstOrDefaultAsNoTrackingAsync(
                                    x => x.DepositId == deposit.Id && !x.IsDelete);

                            var info = new OrderInfo()
                            {
                                CategoryName = detail.CategoryName,
                                IsDelete = 0
                            };
                            UnitOfWork.OrderInfoRepo.Add(info);
                            await UnitOfWork.OrderInfoRepo.SaveAsync();

                            deposit.OrderInfoId = info.Id;
                        }
                        else
                        {
                            deposit.OrderInfoId = 0;
                        }
                    }
                    else
                    {
                        deposit.OrderInfoId = 0;
                    }

                    deposit.DepositType = depositType != null ? depositType == true ? 1 : 0 : 0;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Thêm lịch sử Orders
                    if (deposit.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = $"Thêm đơn hàng ký gửi và nhận xử lý đơn hàng bởi {UserState.FullName}",
                            CustomerId = deposit.CustomerId.Value,
                            CustomerName = deposit.CustomerName,
                            OrderId = deposit.Id,
                            Status = deposit.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = deposit.Type
                        });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    #region  Thêm các dịch vụ cho đơn hàng

                    // DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                    //var orderServcie = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);
                    //if (orderServcie == null)
                    //{
                    //    orderServcie = new OrderService()
                    //    {
                    //        OrderId = deposit.Id,
                    //        ServiceId = (byte)OrderServices.Order,
                    //        ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                    //        ExchangeRate = deposit.ExchangeRate,
                    //        IsDelete = false,
                    //        Created = timeDate,
                    //        LastUpdate = timeDate,
                    //        HashTag = string.Empty,
                    //        Value = 0,
                    //        Currency = Currency.VND.ToString(),
                    //        Type = (byte)UnitType.Percent,
                    //        TotalPrice = 0,
                    //        Mode = (byte)OrderServiceMode.Required,
                    //        Checked = false
                    //    };

                    //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                    //}

                    // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
                    var orderShopShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.ShopShipping);
                    if (orderShopShippingService == null)
                    {
                        orderShopShippingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.ShopShipping,
                            ServiceName = (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = false,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                    }

                    // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                    var autditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Audit);
                    if (autditService == null)
                    {
                        autditService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.Audit,
                            ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = false,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(autditService);
                    }


                    // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                    var packingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Packing);
                    if (packingService == null)
                    {
                        packingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.Packing,
                            ServiceName =
                                OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Packing).Checked,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(packingService);
                    }
                    else
                    {
                        autditService.Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Packing).Checked;
                        autditService.LastUpdate = timeDate;
                    }

                    // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                    var outSideShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.OutSideShipping);
                    if (outSideShippingService == null)
                    {
                        outSideShippingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.OutSideShipping,
                            ServiceName =
                                OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                    }

                    // DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                    //var fastDeliveryService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.FastDelivery);
                    //if (fastDeliveryService == null)
                    //{
                    //    fastDeliveryService = new OrderService()
                    //    {
                    //        OrderId = deposit.Id,
                    //        ServiceId = (byte)OrderServices.FastDelivery,
                    //        ServiceName =
                    //            OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
                    //        ExchangeRate = deposit.ExchangeRate,
                    //        Value = 0,
                    //        Currency = Currency.VND.ToString(),
                    //        Type = (byte)UnitType.Money,
                    //        TotalPrice = 0,
                    //        Mode = (byte)OrderServiceMode.Option,
                    //        Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.FastDelivery).Checked,
                    //        Created = timeDate,
                    //        LastUpdate = timeDate
                    //    };
                    //    UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);
                    //}
                    //else
                    //{
                    //    autditService.Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.FastDelivery).Checked;
                    //    autditService.LastUpdate = timeDate;
                    //}

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                    var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.InSideShipping);
                    if (shipToHomeService == null)
                    {
                        shipToHomeService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.InSideShipping,
                            ServiceName =
                                OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                    }

                    #endregion

                    // Submit thêm OrderService
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

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Thêm thành công đơn hàng ký gửi!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// sua chi tiet don hang
        /// </summary>
        /// <param name="depositDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> UpdateDetail(DepositDetail depositDetail)
        {
            var timeDate = DateTime.Now;
            var detail = await UnitOfWork.DepositDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == depositDetail.Id);
            if (detail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Chi tiết đơn hàng does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == detail.DepositId);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    detail.UpdateDate = timeDate;
                    detail.Weight = depositDetail.Weight;
                    detail.CategoryId = depositDetail.CategoryId;
                    detail.CategoryName = depositDetail.CategoryName;
                    detail.ProductName = depositDetail.ProductName;
                    detail.Quantity = depositDetail.Quantity;
                    detail.Image = depositDetail.Image;
                    detail.Note = depositDetail.Note;
                    detail.PacketNumber = depositDetail.PacketNumber;
                    detail.Long = depositDetail.Long;
                    detail.High = depositDetail.High;
                    detail.Wide = depositDetail.Wide;
                    detail.Size = $"{detail.Long}x{detail.Wide}x{detail.High}";
                    detail.ListCode = depositDetail.ListCode;
                    detail.ShipTq = depositDetail.ShipTq;

                    await UnitOfWork.DepositDetailRepo.SaveAsync();

                    var listDetails = await UnitOfWork.DepositDetailRepo.FindAsync(x => !x.IsDelete && x.DepositId == deposit.Id);

                    deposit.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);

                    if (deposit.ApprovelPrice != null)
                        deposit.ProvisionalMoney = (decimal)(deposit.TotalWeight * deposit.ApprovelPrice);

                    deposit.PacketNumber = listDetails.Sum(x => x.PacketNumber);

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

            return Json(new { status = MsgType.Success, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// them moi chi tiet don hang
        /// </summary>
        /// <param name="id"></param>
        /// <param name="depositDetail"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Add, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> SaveDetail(int id, DepositDetail depositDetail)
        {
            var timeDate = DateTime.Now;

            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Đơn hàng does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var detail = new DepositDetail
                    {
                        Weight = depositDetail.Weight,
                        CategoryId = depositDetail.CategoryId,
                        CategoryName = depositDetail.CategoryName,
                        ProductName = depositDetail.ProductName,
                        Quantity = depositDetail.Quantity,
                        Image = depositDetail.Image,
                        Note = depositDetail.Note,
                        PacketNumber = depositDetail.PacketNumber,
                        Long = depositDetail.Long,
                        High = depositDetail.High,
                        Wide = depositDetail.Wide,
                        ListCode = depositDetail.ListCode,
                        CreateDate = timeDate,
                        UpdateDate = timeDate,
                        DepositId = deposit.Id,
                        LadingCode = depositDetail.LadingCode,
                        ShipTq = depositDetail.ShipTq
                    };

                    detail.Size = $"{detail.Long}x{detail.Wide}x{detail.High}";
                    detail.IsDelete = false;

                    UnitOfWork.DepositDetailRepo.Add(detail);
                    await UnitOfWork.DepositDetailRepo.SaveAsync();

                    var listDetails = await UnitOfWork.DepositDetailRepo.FindAsync(x => !x.IsDelete && x.DepositId == deposit.Id);

                    deposit.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);
                    if (deposit.ApprovelPrice != null)
                        deposit.ProvisionalMoney = deposit.TotalWeight * deposit.ApprovelPrice;
                    deposit.PacketNumber = listDetails.Sum(x => x.PacketNumber);

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

            return Json(new { status = MsgType.Success, msg = "Updated successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// cap nhat thong tin don hang ky gui
        /// </summary>
        /// <param name="deposit"></param>
        /// <param name="orderInfoId"></param>
        /// <param name="depositType"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> Update(Order deposit, bool? orderInfoId, bool? depositType, List<OrderService> listOrderServiceCheck)
        {
            var timeDate = DateTime.Now;

            var depositUpdate = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.Id);

            if (depositUpdate == null)
            {
                return Json(new { status = MsgType.Error, msg = "Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == depositUpdate.CustomerId && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Chưa chọn khách hàng!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho nhận ký gửi
            var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseId);

            if (warehouseStart == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select consignment warehouse!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho giao hàng
            var warehouseEnd = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseDeliveryId);

            if (warehouseEnd == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select warehouse to deliver!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    depositUpdate.LastUpdate = timeDate;
                    depositUpdate.LevelId = customer.LevelId;
                    depositUpdate.LevelName = customer.LevelName;
                    depositUpdate.SystemId = customer.SystemId;
                    depositUpdate.SystemName = customer.SystemName;
                    depositUpdate.ExchangeRate = deposit.ExchangeRate;
                    depositUpdate.CustomerId = customer.Id;
                    depositUpdate.CustomerName = customer.FullName;
                    depositUpdate.CustomerEmail = customer.Email;
                    depositUpdate.CustomerPhone = customer.Phone;
                    depositUpdate.CustomerAddress = customer.Address;
                    depositUpdate.Note = deposit.Note;
                    depositUpdate.ContactName = deposit.ContactName;
                    depositUpdate.ContactPhone = deposit.ContactPhone;
                    depositUpdate.ContactAddress = deposit.ContactAddress;
                    depositUpdate.ContactEmail = deposit.ContactEmail;
                    depositUpdate.Description = deposit.Description;
                    depositUpdate.WarehouseId = warehouseStart.Id;
                    depositUpdate.WarehouseName = warehouseStart.Name;
                    depositUpdate.WarehouseDeliveryId = warehouseEnd.Id;
                    depositUpdate.WarehouseDeliveryName = warehouseEnd.Name;
                    depositUpdate.ApprovelUnit = deposit.ApprovelUnit;
                    depositUpdate.ApprovelPrice = deposit.ApprovelPrice;
                    depositUpdate.FeeShip = deposit.FeeShip;

                    if (orderInfoId != null)
                    {
                        if (orderInfoId == true)
                        {
                            var detail =
                                await UnitOfWork.DepositDetailRepo.FirstOrDefaultAsNoTrackingAsync(
                                    x => x.DepositId == depositUpdate.Id && !x.IsDelete);

                            var info = new OrderInfo()
                            {
                                CategoryName = detail.CategoryName,
                                IsDelete = 0
                            };
                            UnitOfWork.OrderInfoRepo.Add(info);
                            await UnitOfWork.OrderInfoRepo.SaveAsync();

                            depositUpdate.OrderInfoId = info.Id;
                        }
                        else
                        {
                            depositUpdate.OrderInfoId = 0;
                        }
                    }
                    else
                    {
                        depositUpdate.OrderInfoId = 0;
                    }

                    depositUpdate.DepositType = depositType != null ? depositType == true ? 1 : 0 : 0;

                    var listDetails = await UnitOfWork.DepositDetailRepo.FindAsync(x => !x.IsDelete && x.DepositId == depositUpdate.Id);

                    depositUpdate.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);
                    if (depositUpdate.ApprovelPrice != null)
                        depositUpdate.ProvisionalMoney = (decimal)(depositUpdate.TotalWeight * depositUpdate.ApprovelPrice);
                    depositUpdate.PacketNumber = listDetails.Sum(x => x.PacketNumber);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    #region  Thêm các dịch vụ cho đơn hàng

                    // DỊCH VỤ MUA HÀNG HỘ --------------------------------------------------------------------------
                    //var orderServcie = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Order);
                    //if (orderServcie == null)
                    //{
                    //    orderServcie = new OrderService()
                    //    {
                    //        OrderId = deposit.Id,
                    //        ServiceId = (byte)OrderServices.Order,
                    //        ServiceName = OrderServices.Order.GetAttributeOfType<DescriptionAttribute>().Description,
                    //        ExchangeRate = deposit.ExchangeRate,
                    //        IsDelete = false,
                    //        Created = timeDate,
                    //        LastUpdate = timeDate,
                    //        HashTag = string.Empty,
                    //        Value = 0,
                    //        Currency = Currency.VND.ToString(),
                    //        Type = (byte)UnitType.Percent,
                    //        TotalPrice = 0,
                    //        Mode = (byte)OrderServiceMode.Required,
                    //        Checked = false
                    //    };

                    //    UnitOfWork.OrderServiceRepo.Add(orderServcie);
                    //}

                    // DỊCH VỤ SHOP TQ CHUYỂN HÀNG --------------------------------------------------------------------------
                    var orderShopShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.ShopShipping);
                    if (orderShopShippingService == null)
                    {
                        orderShopShippingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.ShopShipping,
                            ServiceName = (OrderServices.ShopShipping).GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = false,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(orderShopShippingService);
                    }

                    // DỊCH VỤ KIEERM ĐẾM HÀNG HÓA --------------------------------------------------------------------------
                    var autditService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Audit);
                    if (autditService == null)
                    {
                        autditService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.Audit,
                            ServiceName = OrderServices.Audit.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = false,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(autditService);
                    }


                    // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                    var packingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.Packing);
                    if (packingService == null)
                    {
                        packingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.Packing,
                            ServiceName =
                                OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Packing).Checked,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(packingService);
                    }
                    else
                    {
                        packingService.Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.Packing).Checked;
                        packingService.LastUpdate = timeDate;
                    }

                    // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                    var outSideShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.OutSideShipping);
                    if (outSideShippingService == null)
                    {
                        outSideShippingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.OutSideShipping,
                            ServiceName =
                                OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                    }

                    // DỊCH VỤ CHUYỂN HÀNG ĐƯỜNG HÀNG KHÔNG --------------------------------------------------------------------------
                    //var fastDeliveryService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.FastDelivery);
                    //if (fastDeliveryService == null)
                    //{
                    //    fastDeliveryService = new OrderService()
                    //    {
                    //        OrderId = deposit.Id,
                    //        ServiceId = (byte)OrderServices.FastDelivery,
                    //        ServiceName =
                    //            OrderServices.FastDelivery.GetAttributeOfType<DescriptionAttribute>().Description,
                    //        ExchangeRate = deposit.ExchangeRate,
                    //        Value = 0,
                    //        Currency = Currency.VND.ToString(),
                    //        Type = (byte)UnitType.Money,
                    //        TotalPrice = 0,
                    //        Mode = (byte)OrderServiceMode.Option,
                    //        Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.FastDelivery).Checked,
                    //        Created = timeDate,
                    //        LastUpdate = timeDate
                    //    };
                    //    UnitOfWork.OrderServiceRepo.Add(fastDeliveryService);
                    //}
                    //else
                    //{
                    //    fastDeliveryService.Checked = listOrderServiceCheck.FirstOrDefault(x => x.ServiceId == (byte)OrderServices.FastDelivery).Checked;
                    //    fastDeliveryService.LastUpdate = timeDate;
                    //}

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                    var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.ServiceId == (byte)OrderServices.InSideShipping);
                    if (shipToHomeService == null)
                    {
                        shipToHomeService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.InSideShipping,
                            ServiceName =
                                OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                    }

                    // Submit thêm OrderService
                    await UnitOfWork.OrderServiceRepo.SaveAsync();

                    var strCodeOrder = $";{depositUpdate.Code};";
                    var order = depositUpdate;
                    var weight = await UnitOfWork.OrderPackageRepo.SumWeightByOrderCodes(strCodeOrder);


                    //foreach (var order in orders)
                    //{
                        if (weight.ContainsKey(order.Code))
                        {
                            order.TotalWeight = weight[order.Code];
                        }

                        #region Chia tiền dịch vụ phát sinh cho các kiện hàng theo % cân nạng

                        // Tính toán chi phí phát sinh cảu Orders
                        var serviceOthers =
                            await UnitOfWork.OrderServiceOtherRepo.FindAsync(
                                x => x.OrderId == order.Id && x.Type == 0);

                        if (serviceOthers.Any())
                        {
                            var packageFirst = new List<int>();
                            // Cập nhật lại Sum cân nặng của đơn
                            foreach (var serviceOther in serviceOthers)
                            {
                                var packages = UnitOfWork.OrderPackageRepo.GetByOrderIdAndImportWarehouseId(order.Id,
                                    serviceOther.ObjectId);

                                serviceOther.TotalWeightActual = packages.Sum(x => x.WeightActual);

                                // Tính lại cân nặng trong kiên hàng
                                foreach (var p in packages)
                                {
                                    var percent = p.WeightActual * 100 / serviceOther.TotalWeightActual;

                                    if (packageFirst.Any(x => x == p.Id))
                                    {
                                        p.OtherService += percent * serviceOther.TotalPrice / 100;
                                    }
                                    else
                                    {
                                        p.OtherService = percent * serviceOther.TotalPrice / 100;
                                        packageFirst.Add(p.Id);
                                    }
                                }
                            }
                        }

                        #endregion

                        #region Update Goods shipping to Vietnam service

                        ///*var*/ fastDeliveryService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte)OrderServices.FastDelivery && x.Checked);

                        //var optimalService = await
                        //    UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                        //        x => !x.IsDelete && x.OrderId == order.Id &&
                        //             x.ServiceId == (byte)OrderServices.Optimal && x.Checked);

                        /*var*/ outSideShippingService = await
                            UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => !x.IsDelete && x.OrderId == order.Id &&
                                     x.ServiceId == (byte)OrderServices.OutSideShipping && x.Checked);

                        decimal serviceValue;

                        var vipLevel = UnitOfWork.OrderRepo.CustomerVipLevel(order.LevelId);

                        // Cân nặng các package được xuất giao tại TQ
                        var orderWeightIgnore = UnitOfWork.OrderPackageRepo.GetTotalActualWeight(order.Id);

                        // Sum cân nặng tính tiền vc của Orders
                        var orderWeight = order.TotalWeight - orderWeightIgnore;

                        decimal outSideShipping;

                        // Orders ký gửi
                        if (order.Type == (byte)OrderType.Deposit)
                        {
                            serviceValue = order.ApprovelPrice ?? 0;

                            if (orderWeight >= 50)
                            {
                                outSideShipping = orderWeight * serviceValue;
                            }
                            else
                            {
                                outSideShipping = (orderWeight - 1) * serviceValue + 100000;
                            }
                        }
                        else // Orders Order
                        {
                            //// VC tiết kiệm
                            //if (optimalService != null)
                            //{
                            //    serviceValue = OrderRepository.OptimalDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else if (fastDeliveryService != null) // VC nhanh
                            //{
                            //    serviceValue = OrderRepository.FastDelivery(orderWeight,
                            //        order.WarehouseDeliveryId ?? 0);
                            //}
                            //else // VC bình thường
                            //{
                                serviceValue = OrderRepository.ShippingOutSide(order.ServiceType, orderWeight,
                                    order.WarehouseDeliveryId ?? 0);
                            //}

                            outSideShipping = orderWeight * serviceValue;
                        }

                        if (outSideShippingService == null)
                        {
                            outSideShippingService = new OrderService()
                            {
                                IsDelete = false,
                                Checked = true,
                                Created = timeDate,
                                LastUpdate = timeDate,
                                Value = serviceValue,
                                Currency = Currency.VND.ToString(),
                                ExchangeRate = 0,
                                TotalPrice = outSideShipping,
                                HashTag = string.Empty,
                                Mode = (byte)OrderServiceMode.Required,
                                OrderId = order.Id,
                                ServiceId = (byte)OrderServices.OutSideShipping,
                                ServiceName =
                                    (OrderServices.OutSideShipping).GetAttributeOfType<DescriptionAttribute>()
                                    .Description,
                                Type = (byte)UnitType.Money,

                            };

                            // Triết khấu Vip cho Orders Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee {serviceValue.ToString("N2", CultureInfo)} baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";

                                UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                            }
                        }
                        else
                        {
                            outSideShippingService.LastUpdate = timeDate;
                            outSideShippingService.Value = serviceValue;
                            outSideShippingService.TotalPrice = outSideShipping;

                            // Triết khấu Vip cho Orders Order
                            if (order.Type == (byte)OrderType.Order)
                            {
                                // Trừ tiền triết khấu theo cấp Level Vip
                                outSideShippingService.TotalPrice -= vipLevel.Ship *
                                                                     outSideShippingService.TotalPrice / 100;
                                outSideShippingService.Note =
                                    $"Goods shipping service fee  {serviceValue.ToString("N2", CultureInfo)} baht/1kg" +
                                    $" and discounted {vipLevel.Ship.ToString("N2", CultureInfo)}%";
                            }
                        }

                        #endregion

                        await UnitOfWork.OrderServiceRepo.SaveAsync();

                        // Cập nhật lại Total money của Orders
                        var totalService = UnitOfWork.OrderServiceRepo.Find(x => x.OrderId == order.Id &&
                                                                                 x.IsDelete == false && x.Checked)
                            .ToList()
                            .Sum(x => x.TotalPrice);

                        order.PackageNo = await UnitOfWork.OrderPackageRepo.CountAsync(x => x.IsDelete == false && x.OrderId == order.Id);
                        order.PackageNoInStock = await UnitOfWork.OrderPackageRepo.CountAsync(x => x.IsDelete == false && x.OrderId == order.Id && x.Status >= (byte)OrderPackageStatus.ChinaReceived);

                        order.Total = order.TotalExchange + totalService;
                        order.Debt = order.Total - (order.TotalPayed - order.TotalRefunded);

                        order.LastUpdate = DateTime.Now;

                        await UnitOfWork.OrderRepo.SaveAsync();
                    //}

                    #endregion

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //2. Gửi dữ liệu lên view
            return Json(new { status = MsgType.Success, msg = "Sửa đơn hàng thành công" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// xoa chi tiet don hang
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Delete, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> DeleteDetail(int id)
        {
            var timeDate = DateTime.Now;
            var detail = await UnitOfWork.DepositDetailRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
            if (detail == null)
            {
                return Json(new { status = MsgType.Error, msg = "Detail Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == detail.DepositId);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    detail.UpdateDate = timeDate;
                    detail.IsDelete = true;

                    await UnitOfWork.DepositDetailRepo.SaveAsync();

                    var listDetails = await UnitOfWork.DepositDetailRepo.FindAsync(x => !x.IsDelete && x.DepositId == deposit.Id);

                    deposit.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);
                    if (deposit.ApprovelPrice != null)
                        deposit.ProvisionalMoney = (decimal)(deposit.TotalWeight * deposit.ApprovelPrice);
                    deposit.PacketNumber = listDetails.Sum(x => x.PacketNumber);

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

            return Json(new { status = MsgType.Success, msg = "Update deleted successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Kết đơn hộ khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> Singles(int id)
        {
            var timeDate = DateTime.Now;
            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Đơn hàng does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    deposit.LastUpdate = timeDate;
                    deposit.Status = (byte)DepositStatus.Order;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == deposit.CustomerId);

                    //Thêm lịch sử Orders
                    if (deposit.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = $"Employee {UserState.FullName} settle the order for customer",
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            OrderId = deposit.Id,
                            Status = deposit.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = deposit.Type
                        });

                    await UnitOfWork.OrderHistoryRepo.SaveAsync();

                    //Them dich vu don hang
                    // DỊCH VỤ ĐÓNG package HÓA --------------------------------------------------------------------------
                    var packingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                                x => x.OrderId == deposit.Id
                                && !x.IsDelete
                                && x.ServiceId == (byte)OrderServices.Packing);

                    if (packingService == null)
                    {
                        packingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.Packing,
                            ServiceName = OrderServices.Packing.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.CNY.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Option,
                            Checked = false,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(packingService);
                    }
                    // DỊCH VỤ CHUYỂN HÀNG VỀ VN --------------------------------------------------------------------------
                    var outSideShippingService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.OrderId == deposit.Id
                                && !x.IsDelete
                                && x.ServiceId == (byte)OrderServices.OutSideShipping);

                    if (outSideShippingService == null)
                    {
                        outSideShippingService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.OutSideShipping,
                            ServiceName = OrderServices.OutSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(outSideShippingService);
                    }

                    // DỊCH GIAO HÀNG TẬN NHÀ --------------------------------------------------------------------------
                    var shipToHomeService = await UnitOfWork.OrderServiceRepo.SingleOrDefaultAsync(
                            x => x.OrderId == deposit.Id
                            && !x.IsDelete
                            && x.ServiceId == (byte)OrderServices.InSideShipping);

                    if (shipToHomeService == null)
                    {
                        shipToHomeService = new OrderService()
                        {
                            OrderId = deposit.Id,
                            ServiceId = (byte)OrderServices.InSideShipping,
                            ServiceName = OrderServices.InSideShipping.GetAttributeOfType<DescriptionAttribute>().Description,
                            ExchangeRate = deposit.ExchangeRate,
                            Value = 0,
                            Currency = Currency.VND.ToString(),
                            Type = (byte)UnitType.Money,
                            TotalPrice = 0,
                            Mode = (byte)OrderServiceMode.Required,
                            Checked = true,
                            Created = timeDate,
                            LastUpdate = timeDate
                        };
                        UnitOfWork.OrderServiceRepo.Add(shipToHomeService);
                    }

                    //Them dia chi khach hang vao don hang
                    var toAddress = new OrderAddress()
                    {
                        Address = deposit.ContactAddress,
                        Phone = deposit.ContactPhone,
                        ProvinceId = 0,
                        ProvinceName = "",
                        DistrictId = 0,
                        DistrictName = "",
                        WardId = 0,
                        WardName = "",
                        FullName = deposit.ContactName,
                    };

                    // Tạo địa chỉ người đặt hàng
                    var fromAddress = new OrderAddress()
                    {
                        Address = customer.Address,
                        Phone = customer.Phone,
                        ProvinceId = customer.ProvinceId,
                        ProvinceName = customer.ProvinceName,
                        DistrictId = customer.DistrictId,
                        DistrictName = customer.DistrictName,
                        WardId = customer.WardId,
                        WardName = customer.WardsName,
                        FullName = customer.FullName
                    };

                    UnitOfWork.OrderAddressRepo.Add(fromAddress);
                    UnitOfWork.OrderAddressRepo.Add(toAddress);

                    await UnitOfWork.OrderAddressRepo.SaveAsync();

                    deposit.FromAddressId = fromAddress.Id;
                    deposit.ToAddressId = toAddress.Id;

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
            return Json(new { status = MsgType.Success, msg = "Order settled successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gửi duyệt giá
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> PendingPrice(Deposit deposit, bool? orderInfoId, bool? depositType)
        {
            var timeDate = DateTime.Now;

            var depositUpdate = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.Id);

            if (depositUpdate == null)
            {
                return Json(new { status = MsgType.Error, msg = "Orders Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            //check khách hàng
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => x.Id == depositUpdate.CustomerId && !x.IsDelete);
            if (customer == null)
            {
                return Json(new { status = MsgType.Error, msg = "Not customer selected yet!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho nhận ký gửi
            var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseId);

            if (warehouseStart == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select consignment warehouse!" }, JsonRequestBehavior.AllowGet);
            }

            //check kho giao hàng
            var warehouseEnd = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseDeliveryId);

            if (warehouseEnd == null)
            {
                return Json(new { status = MsgType.Error, msg = "Select warehouse to deliver!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    depositUpdate.LastUpdate = timeDate;
                    depositUpdate.LevelId = customer.LevelId;
                    depositUpdate.LevelName = customer.LevelName;
                    depositUpdate.SystemId = customer.SystemId;
                    depositUpdate.SystemName = customer.SystemName;
                    depositUpdate.ExchangeRate = deposit.ExchangeRate;
                    depositUpdate.CustomerId = customer.Id;
                    depositUpdate.CustomerName = customer.FullName;
                    depositUpdate.CustomerEmail = customer.Email;
                    depositUpdate.CustomerPhone = customer.Phone;
                    depositUpdate.CustomerAddress = customer.Address;
                    depositUpdate.Note = deposit.Note;
                    depositUpdate.ContactName = deposit.ContactName;
                    depositUpdate.ContactPhone = deposit.ContactPhone;
                    depositUpdate.ContactAddress = deposit.ContactAddress;
                    depositUpdate.ContactEmail = deposit.ContactEmail;
                    depositUpdate.Description = deposit.Description;
                    depositUpdate.WarehouseId = warehouseStart.Id;
                    depositUpdate.WarehouseName = warehouseStart.Name;
                    depositUpdate.WarehouseDeliveryId = warehouseEnd.Id;
                    depositUpdate.WarehouseDeliveryName = warehouseEnd.Name;
                    depositUpdate.ApprovelUnit = deposit.ApprovelUnit;
                    depositUpdate.ApprovelPrice = deposit.ApprovelPrice;

                    depositUpdate.Status = (byte)DepositStatus.PendingPrice;

                    if (orderInfoId != null)
                    {
                        if (orderInfoId == true)
                        {
                            var detail =
                                await UnitOfWork.DepositDetailRepo.FirstOrDefaultAsNoTrackingAsync(
                                    x => x.DepositId == depositUpdate.Id && !x.IsDelete);

                            var info = new OrderInfo()
                            {
                                CategoryName = detail.CategoryName,
                                IsDelete = 0
                            };
                            UnitOfWork.OrderInfoRepo.Add(info);
                            await UnitOfWork.OrderInfoRepo.SaveAsync();

                            depositUpdate.OrderInfoId = info.Id;
                        }
                        else
                        {
                            depositUpdate.OrderInfoId = 0;
                        }
                    }
                    else
                    {
                        depositUpdate.OrderInfoId = 0;
                    }

                    depositUpdate.DepositType = depositType != null ? depositType == true ? 1 : 0 : 0;

                    var listDetails = await UnitOfWork.DepositDetailRepo.FindAsync(x => !x.IsDelete && x.DepositId == depositUpdate.Id);

                    depositUpdate.TotalWeight = (decimal)listDetails.Sum(x => x.Weight);

                    if (depositUpdate.ApprovelPrice != null)
                    {
                        if (depositUpdate.TotalWeight < 50)
                        {
                            depositUpdate.ProvisionalMoney = (decimal)(depositUpdate.TotalWeight * (depositUpdate.ApprovelPrice - 1) + 100000);
                        }
                        else
                        {
                            depositUpdate.ProvisionalMoney = (decimal)(depositUpdate.TotalWeight * depositUpdate.ApprovelPrice);
                        }
                    }

                    depositUpdate.PacketNumber = listDetails.Sum(x => x.PacketNumber);

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Thêm lịch sử Orders
                    if (depositUpdate.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = "Send price approval",
                            CustomerId = depositUpdate.CustomerId.Value,
                            CustomerName = depositUpdate.CustomerName,
                            OrderId = depositUpdate.Id,
                            Status = depositUpdate.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = depositUpdate.Type
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

                //using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                //{
                //    try
                //    {
                //        deposit.LastUpdate = timeDate;
                //        deposit.Status = (byte)DepositStatus.PendingPrice;

                //        await UnitOfWork.OrderRepo.SaveAsync();

                //        //Thêm lịch sử Orders
                //        if (deposit.CustomerId != null)
                //            UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                //            {
                //                CreateDate = timeDate,
                //                Content = "Gửi duyệt giá",
                //                CustomerId = deposit.CustomerId.Value,
                //                CustomerName = deposit.CustomerName,
                //                OrderId = deposit.Id,
                //                Status = deposit.Status,
                //                UserId = UserState.UserId,
                //                UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                //                Type = deposit.Type
                //            });

                //        await UnitOfWork.OrderHistoryRepo.SaveAsync();

                //        transaction.Commit();
                //    }
                //    catch (Exception)
                //    {
                //        transaction.Rollback();
                //        //throw;

                //        return Json(new { status = MsgType.Error, msg = "Không thể thực hiện thao tác này!" }, JsonRequestBehavior.AllowGet);
                //    }
            }
            return Json(new { status = MsgType.Success, msg = "Action executed successfully" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// duyet gia cho don hang
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [LogTracker(EnumAction.Approvel, EnumPage.OrderDepositDelay, EnumPage.OrderDeposit, EnumPage.OrderDepositNew)]
        public async Task<JsonResult> Quotes(int id)
        {
            var timeDate = DateTime.Now;
            var deposit = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (deposit == null)
            {
                return Json(new { status = MsgType.Error, msg = "Đơn hàng does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    deposit.LastUpdate = timeDate;
                    deposit.Status = (byte)DepositStatus.WaitOrder;

                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Thêm lịch sử Orders
                    if (deposit.CustomerId != null)
                        UnitOfWork.OrderHistoryRepo.Add(new OrderHistory()
                        {
                            CreateDate = timeDate,
                            Content = "Duyệt giá cho đơn hàng",
                            CustomerId = deposit.CustomerId.Value,
                            CustomerName = deposit.CustomerName,
                            OrderId = deposit.Id,
                            Status = deposit.Status,
                            UserId = UserState.UserId,
                            UserFullName = $"{UserState.FullName} - {UserState.TitleName}",
                            Type = deposit.Type
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
            return Json(new { status = MsgType.Success, msg = "Action executed successfully" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [2. Actions with waybill code]

        //Add the tracking code vào Orders
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDeposit, EnumPage.OrderDepositDelay, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<JsonResult> AddContractCode(int id, string codePackage, string note)
        {
            var timeNow = DateTime.Now;
            if (!string.IsNullOrEmpty(codePackage))
            {
                codePackage = codePackage.Trim();
            }

            var deposit = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (deposit == null) //does not exist Orders hoặc Orders bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Đơn hàng does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //Tạo mã vận đơn
                    //lấy thông tin khách hàng
                    var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.CustomerId);
                    if (customer == null) //does not exist Orders hoặc Orders bị xóa
                    {
                        return Json(new { status = MsgType.Error, msg = "There is no customer information!" }, JsonRequestBehavior.AllowGet);
                    }

                    //lấy thông tin kho nguồn
                    var warehouseStart = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseId);

                    if (warehouseStart == null)
                    {
                        return Json(new { status = MsgType.Error, msg = "Chưa chọn kho nhận hàng!" }, JsonRequestBehavior.AllowGet);
                    }

                    //lấy thông tin kho đích
                    var warehouseEnd = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.WarehouseDeliveryId);

                    if (warehouseEnd == null)
                    {
                        return Json(new { status = MsgType.Error, msg = "Warehoue to deliver has not been selected!" }, JsonRequestBehavior.AllowGet);
                    }

                    var checkOrderPackage = await
                   UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(
                       x => !x.IsDelete && x.OrderId == deposit.Id && x.TransportCode == codePackage);

                    if (checkOrderPackage != null)
                    {
                        return Json(new { status = MsgType.Error, msg = $" Waybill code coincided '#{codePackage}'!" }, JsonRequestBehavior.AllowGet);
                    }

                    var listService = await UnitOfWork.OrderServiceRepo.FindAsync(
                        x => x.OrderId == deposit.Id && x.Mode == (byte)OrderServiceMode.Option
                            && x.Checked
                            && !x.IsDelete
                    );

                    if (deposit.PacketNumber != null)
                    {
                        var depositPackage = new OrderPackage()
                        {
                            Code = string.Empty,
                            Status = 0,
                            OrderId = deposit.Id,
                            OrderCode = deposit.Code,
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            CustomerUserName = customer.Email,
                            CustomerLevelId = customer.LevelId,
                            CustomerLevelName = customer.LevelName,
                            CustomerWarehouseId = warehouseEnd.Id,
                            CustomerWarehouseName = warehouseEnd.Name,
                            CustomerWarehouseIdPath = warehouseEnd.IdPath,
                            CustomerWarehouseAddress = warehouseEnd.Address,
                            TransportCode = codePackage,
                            WarehouseId = warehouseStart.Id,
                            WarehouseName = warehouseStart.Name,
                            WarehouseIdPath = warehouseStart.IdPath,
                            WarehouseAddress = warehouseStart.Address,
                            SystemId = deposit.SystemId,
                            SystemName = deposit.SystemName,
                            Created = timeNow,
                            LastUpdate = timeNow,
                            HashTag = string.Empty,
                            PackageNo = deposit.PacketNumber.Value,
                            UnsignedText = string.Empty,
                            ForcastDate = DateTime.Now.AddDays(2),
                            OrderType = deposit.Type,
                            Note = note
                        };

                        if (listService.Any())
                        {
                            depositPackage.OrderServices = string.Join(", ", listService.Select(x => x.ServiceName.Replace("Phí dịch", "Dịch")).ToList());
                        }

                        UnitOfWork.OrderPackageRepo.Add(depositPackage);
                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //cập nhật lại mã code
                        var depositPackageOfDay = UnitOfWork.OrderPackageRepo.Count(x =>
                            x.Created.Year == depositPackage.Created.Year && x.Created.Month == depositPackage.Created.Month &&
                            x.Created.Day == depositPackage.Created.Day && x.Id <= depositPackage.Id);
                        depositPackage.Code = $"{depositPackageOfDay}{depositPackage.Created:ddMMyy}";
                        depositPackage.UnsignedText = MyCommon.Ucs2Convert($"{depositPackage.OrderCode} {MyCommon.ReturnCode(depositPackage.OrderCode)} {depositPackage.Code} {depositPackage.TransportCode} {deposit.CustomerName} {deposit.CustomerEmail} {deposit.CustomerPhone} {depositPackage.WarehouseName}").ToLower();

                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //Check trùng mã vận đơn
                        var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == codePackage && x.OrderId > 0);

                        if (checkPackageCount > 1)
                        {
                            PackageJob.UpdateSameTransportCode(depositPackage.TransportCode, depositPackage.Code, UnitOfWork, UserState);
                        }

                        //Ghi chú toàn system cho package, Orders
                        var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                              x =>
                              x.PackageId == depositPackage.Id && x.OrderId == depositPackage.OrderId && x.ObjectId == null &&
                              x.Mode == (byte)PackageNoteMode.Order);

                        if (packageNote == null && !string.IsNullOrWhiteSpace(depositPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                            {
                                OrderId = depositPackage.OrderId,
                                OrderCode = depositPackage.OrderCode,
                                PackageId = depositPackage.Id,
                                PackageCode = depositPackage.Code,
                                UserId = UserState.UserId,
                                UserFullName = UserState.FullName,
                                Time = DateTime.Now,
                                ObjectId = null,
                                ObjectCode = string.Empty,
                                Mode = (byte)PackageNoteMode.Order,
                                Content = depositPackage.Note
                            });
                        }
                        else if (packageNote != null && !string.IsNullOrWhiteSpace(depositPackage.Note))
                        {
                            packageNote.Content = depositPackage.Note;
                        }
                        else if (packageNote != null && string.IsNullOrWhiteSpace(depositPackage.Note))
                        {
                            UnitOfWork.PackageNoteRepo.Remove(packageNote);
                        }

                        // Thêm lịch sử cho package
                        var packageHistory = new PackageHistory()
                        {
                            PackageId = depositPackage.Id,
                            PackageCode = depositPackage.Code,
                            OrderId = deposit.Id,
                            OrderCode = deposit.Code,
                            Type = deposit.Type,
                            Status = (int)OrderPackageStatus.ShopDelivery,
                            Content = EnumHelper.GetEnumDescription(OrderPackageStatus.ShopDelivery),
                            CustomerId = customer.Id,
                            CustomerName = customer.FullName,
                            UserId = UserState.UserId,
                            UserName = UserState.UserName,
                            UserFullName = UserState.FullName,
                            CreateDate = DateTime.Now,
                        };

                        UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                        await UnitOfWork.PackageHistoryRepo.SaveAsync();
                    }

                    //Lây ra số package trong Orders
                    var listPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == deposit.Id && x.OrderType == deposit.Type);

                    //Tính trung bình số giá trị package
                    var totalPrice = deposit.ProvisionalMoney / listPackage.Count();
                    foreach (var item in listPackage)
                    {
                        item.TotalPrice = totalPrice;
                        item.LastUpdate = timeNow;
                        item.PackageNo = listPackage.Count;
                    }

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    deposit.PackageNo = listPackage.Count;
                    deposit.LastUpdate = timeNow;

                    //Cập nhật unsignName
                    deposit.UnsignName = deposit.UnsignName + " " + codePackage.ToLower();

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

            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.OrderType == deposit.Type);
            return Json(new { status = MsgType.Success, msg = "Waybill code added successfully!", list }, JsonRequestBehavior.AllowGet);
        }

        //Delete waybill code
        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDeposit, EnumPage.OrderDepositDelay, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<JsonResult> DeleteContractCode(int id)
        {
            var timeNow = DateTime.Now;

            var depositPackage = await UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (depositPackage == null) //package đã bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "package Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var deposit = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == depositPackage.OrderId && !x.IsDelete);
            if (deposit == null) //does not exist Orders hoặc Orders bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var array = deposit.UnsignName.Split(' ');
                    array = array.Where(x => x != depositPackage.TransportCode.ToLower()).ToArray();
                    deposit.UnsignName = string.Join(" ", array);
                    await UnitOfWork.OrderRepo.SaveAsync();

                    depositPackage.IsDelete = true;
                    depositPackage.LastUpdate = timeNow;

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    deposit.PackageNo = UnitOfWork.OrderPackageRepo.Count(x => x.OrderId == deposit.Id && !x.IsDelete);
                    deposit.LastUpdate = timeNow;
                    await UnitOfWork.OrderRepo.SaveAsync();

                    //Lây ra số package trong Orders
                    var listPackage = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.OrderId == deposit.Id && x.OrderType == deposit.Type);

                    //Tính trung bình số giá trị package
                    if (listPackage.Any())
                    {
                        var totalPrice = deposit.ProvisionalMoney / listPackage.Count();
                        foreach (var item in listPackage)
                        {
                            item.TotalPrice = totalPrice;
                            item.LastUpdate = timeNow;
                        }

                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }

                    //xóa trùng mã vận đơn
                    var listP = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.TransportCode == depositPackage.TransportCode && x.OrderId > 0);
                    if (listP.Count() == 1)
                    {
                        var item = listP.FirstOrDefault();
                        item.Note = MyCommon.RemoveHash(MyCommon.RemoveHash(item.Note, "[TM]"), "[XL]");
                        item.Mode = null;
                        item.SameCodeStatus = 0;

                        await UnitOfWork.OrderPackageRepo.SaveAsync();
                    }
                    else
                    {
                        //Check trùng mã vận đơn
                        var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == depositPackage.TransportCode && x.OrderId > 0);

                        if (checkPackageCount > 1)
                        {
                            PackageJob.UpdateSameTransportCode(depositPackage.TransportCode, listP.FirstOrDefault().Code, UnitOfWork, UserState);
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
            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.OrderType == deposit.Type);
            return Json(new { status = MsgType.Success, msg = "Action executed successfully!", list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogTracker(EnumAction.Update, EnumPage.OrderDeposit, EnumPage.OrderDepositDelay, EnumPage.ImportWarehouse, EnumPage.ImportWarehouseWallet)]
        public async Task<JsonResult> EditContractCode(int packageId, string packageName, DateTime date, decimal? weight, decimal? width, decimal? height, decimal? length)
        {
            var timeNow = DateTime.Now;

            if (!string.IsNullOrEmpty(packageName))
            {
                packageName = packageName.Trim();
            }

            var depositPackage = await UnitOfWork.OrderPackageRepo.FirstOrDefaultAsync(x => x.Id == packageId && !x.IsDelete);
            if (depositPackage == null) //package đã bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "package Does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            var deposit = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => x.Id == depositPackage.OrderId && !x.IsDelete);
            if (deposit == null) //does not exist Orders hoặc Orders bị xóa
            {
                return Json(new { status = MsgType.Error, msg = "Order does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var pOld = depositPackage.TransportCode;

                    var array = deposit.UnsignName.Split(' ');
                    array = array.Where(x => x != depositPackage.TransportCode.ToLower()).ToArray();
                    deposit.UnsignName = string.Join(" ", array);
                    await UnitOfWork.OrderRepo.SaveAsync();

                    depositPackage.TransportCode = packageName;
                    depositPackage.LastUpdate = timeNow;
                    depositPackage.ForcastDate = date;
                    depositPackage.Weight = weight;
                    depositPackage.Width = width;
                    depositPackage.Height = height;
                    depositPackage.Length = length;
                    depositPackage.Size = $"{length}x{width}x{height}";

                    await UnitOfWork.OrderPackageRepo.SaveAsync();

                    //Check trùng mã vận đơn
                    var checkPackageCount = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == packageName && x.OrderId > 0);

                    if (checkPackageCount > 1)
                    {
                        PackageJob.UpdateSameTransportCode(depositPackage.TransportCode, depositPackage.Code, UnitOfWork, UserState);
                    }

                    //Ghi chú toàn system cho package, Orders
                    var packageNote = await UnitOfWork.PackageNoteRepo.SingleOrDefaultAsync(
                          x =>
                          x.PackageId == depositPackage.Id && x.OrderId == depositPackage.OrderId && x.ObjectId == null &&
                          x.Mode == (byte)PackageNoteMode.Order);

                    if (packageNote == null && !string.IsNullOrWhiteSpace(depositPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Add(new PackageNote()
                        {
                            OrderId = depositPackage.OrderId,
                            OrderCode = depositPackage.OrderCode,
                            PackageId = depositPackage.Id,
                            PackageCode = depositPackage.Code,
                            UserId = UserState.UserId,
                            UserFullName = UserState.FullName,
                            Time = DateTime.Now,
                            ObjectId = null,
                            ObjectCode = string.Empty,
                            Mode = (byte)PackageNoteMode.Order,
                            Content = depositPackage.Note
                        });
                    }
                    else if (packageNote != null && !string.IsNullOrWhiteSpace(depositPackage.Note))
                    {
                        packageNote.Content = depositPackage.Note;
                    }
                    else if (packageNote != null && string.IsNullOrWhiteSpace(depositPackage.Note))
                    {
                        UnitOfWork.PackageNoteRepo.Remove(packageNote);
                    }

                    var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == deposit.CustomerId);

                    // Thêm lịch sử cho package
                    var packageHistory = new PackageHistory()
                    {
                        PackageId = depositPackage.Id,
                        PackageCode = depositPackage.Code,
                        OrderId = deposit.Id,
                        OrderCode = deposit.Code,
                        Type = deposit.Type,
                        Status = (int)OrderPackageStatus.ShopDelivery,
                        Content = EnumHelper.GetEnumDescription(OrderPackageStatus.ShopDelivery),
                        CustomerId = customer.Id,
                        CustomerName = customer.FullName,
                        UserId = UserState.UserId,
                        UserName = UserState.UserName,
                        UserFullName = UserState.FullName,
                        CreateDate = DateTime.Now,
                    };

                    UnitOfWork.PackageHistoryRepo.Add(packageHistory);

                    await UnitOfWork.PackageHistoryRepo.SaveAsync();

                    if (pOld != depositPackage.TransportCode)
                    {
                        var arrayP = depositPackage.UnsignedText.Split(' ');
                        arrayP = arrayP.Where(x => x != pOld.ToLower()).ToArray();

                        depositPackage.UnsignedText = string.Join(" ", arrayP) + $" { depositPackage.TransportCode.ToLower() }";
                        await UnitOfWork.OrderPackageRepo.SaveAsync();

                        //xóa trùng mã vận đơn
                        var listP = await UnitOfWork.OrderPackageRepo.FindAsync(x => !x.IsDelete && x.TransportCode == pOld && x.OrderId > 0);
                        if (listP.Count() == 1)
                        {
                            var item = listP.FirstOrDefault();
                            item.Note = MyCommon.RemoveHash(MyCommon.RemoveHash(item.Note, "[TM]"), "[XL]");
                            item.Mode = null;
                            item.SameCodeStatus = 0;

                            await UnitOfWork.OrderPackageRepo.SaveAsync();
                        }
                        else
                        {
                            //Check trùng mã vận đơn
                            var checkPackageCount2 = await UnitOfWork.OrderPackageRepo.CountAsync(x => !x.IsDelete && x.TransportCode == pOld && x.OrderId > 0);

                            if (checkPackageCount2 > 1)
                            {
                                PackageJob.UpdateSameTransportCode(pOld, listP.FirstOrDefault().Code, UnitOfWork, UserState);
                            }
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
            var list = await UnitOfWork.OrderPackageRepo.FindAsync(x => x.OrderId == deposit.Id && !x.IsDelete && x.OrderType == deposit.Type);
            return Json(new { status = MsgType.Success, msg = "Thực hiện thao tác thành công!", list }, JsonRequestBehavior.AllowGet);
        }

        #endregion [5. Actions with waybill code]
    }
}