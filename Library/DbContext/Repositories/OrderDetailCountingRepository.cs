using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderDetailCountingRepository : Repository<OrderDetailCounting>
    {
        public OrderDetailCountingRepository(ProjectXContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy ra kiểm đếm sai chưa có Refund
        /// </summary>
        /// <param name="orderId">Id đơn hàng</param>
        /// <param name="orderDetailCountingIds">Chuỗ id của kiểm đếm sai (vd: ";id1;id2;")</param>
        /// <returns>OrderDetailCounting: kiểm đếm sai</returns>
        public Task<List<OrderDetailCounting>> OrderDetailCountingNotHaveRefund(int orderId, string orderDetailCountingIds)
        {
            var orderRefund = Db.OrderRefunds.Where(x => x.IsDelete == false)
                .Join(Db.OrderRefundDetails.Where(x => x.IsDelete == false), x => x.Id, d => d.OrderRefundId,
                    (r, d) => d);

            return Db.OrderDetailCountings.Where(x => x.OrderId == orderId && x.IsDelete == false && orderDetailCountingIds.Contains(";" + x.Id + ";"))
                .GroupJoin(orderRefund, c => c.Id, rd => rd.OrderDetailCountingId, (c, rd) => new { c, rd })
                .SelectMany(x => x.rd.DefaultIfEmpty(), (x, rd) => new { x.c, rd })
                .Where(x => x.rd == null)
                .Select(x => x.c)
                .ToListAsync();
        }

        public Task<List<OrderDetailCounting>> OrderDetailCountingNotHaveRefund(int orderId)
        {
            var orderRefund = Db.OrderRefunds.Where(x => x.IsDelete == false)
                .Join(Db.OrderRefundDetails.Where(x => x.IsDelete == false), x => x.Id, d => d.OrderRefundId,
                    (r, d) => d);

            return Db.OrderDetailCountings.Where(x => x.OrderId == orderId && x.IsDelete == false)
                .GroupJoin(orderRefund, c => c.Id, rd => rd.OrderDetailCountingId, (c, rd) => new { c, rd })
                .SelectMany(x => x.rd.DefaultIfEmpty(), (x, rd) => new { x.c, rd })
                .Where(x => x.rd == null)
                .Select(x => x.c)
                .ToListAsync();
        }

        public Task<List<AcountingLoseResult>> GetOrderCountingLose(out int totalRecord, Expression<Func<OrderDetailCounting, bool>> predicate0,
            Expression<Func<AcountingLoseResult, bool>> predicate, 
            byte? status, DateTime? fromDate, DateTime? toDate, string keyword, int currentPage, int recordPerPage)
        {
            var query = Db.OrderDetailCountings.Where(
                    x => x.IsDelete == false && (fromDate == null && toDate == null
                                                 ||
                                                 fromDate != null && (toDate != null) &&
                                                 x.Created >= fromDate && x.Created <= toDate
                                                 ||
                                                 fromDate == null && toDate.HasValue && x.Created <= toDate
                                                 ||
                                                 toDate == null && fromDate.HasValue && x.Created >= fromDate) &&
                         (status == null || x.Status == status.Value)).Where(predicate0)
                .Join(Db.Orders.Where(x => x.IsDelete == false && x.UnsignName.Contains(keyword)), d => d.OrderId,
                    o => o.Id,
                    (d, o) => new AcountingLoseResult
                    {
                        Id = o.Id,
                        Code = o.Code,
                        Type = o.Type,
                        WebsiteName = o.WebsiteName,
                        ShopId = o.ShopId,
                        ShopName = o.ShopName,
                        ShopLink = o.ShopLink,
                        ProductNo = o.ProductNo,
                        PackageNo = o.PackageNo,
                        ContractCode = o.ContractCode,
                        ContractCodes = o.ContractCodes,
                        LevelId = o.LevelId,
                        LevelName = o.LevelName,
                        TotalWeight = o.TotalWeight,
                        DiscountType = o.DiscountType,
                        DiscountValue = o.DiscountValue,
                        GiftCode = o.GiftCode,
                        CreatedTool = o.CreatedTool,
                        Currency = o.Currency,
                        ExchangeRate = o.ExchangeRate,
                        TotalExchange = o.TotalExchange,
                        TotalPrice = o.TotalPrice,
                        Total = o.Total,
                        HashTag = o.HashTag,
                        WarehouseId = o.WarehouseId,
                        WarehouseName = o.WarehouseName,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName,
                        CustomerEmail = o.CustomerEmail,
                        CustomerPhone = o.CustomerPhone,
                        CustomerAddress = o.CustomerAddress,
                        Status = o.Status,
                        UserId = o.UserId,
                        UserFullName = o.UserFullName,
                        OfficeId = o.OfficeId,
                        OfficeName = o.OfficeName,
                        OfficeIdPath = o.OfficeIdPath,
                        CreatedOfficeIdPath = o.CreatedOfficeIdPath,
                        CreatedUserId = o.CreatedUserId,
                        CreatedUserFullName = o.CreatedUserFullName,
                        CreatedOfficeId = o.CreatedOfficeId,
                        CreatedOfficeName = o.CreatedOfficeName,
                        OrderInfoId = o.OrderInfoId,
                        FromAddressId = o.FromAddressId,
                        ToAddressId = o.ToAddressId,
                        SystemId = o.SystemId,
                        SystemName = o.SystemName,
                        ServiceType = o.ServiceType,
                        Note = o.Note,
                        PrivateNote = o.PrivateNote,
                        LinkNo = o.LinkNo,
                        Created = o.Created,
                        LastUpdate = o.LastUpdate,
                        ExpectedDate = o.ExpectedDate,
                        TotalPurchase = o.TotalPurchase,
                        TotalAdvance = o.TotalAdvance,
                        ReasonCancel = o.ReasonCancel,
                        PriceBargain = o.PriceBargain,
                        PaidShop = o.PaidShop,
                        FeeShip = o.FeeShip,
                        FeeShipBargain = o.FeeShipBargain,
                        IsPayWarehouseShip = o.IsPayWarehouseShip,
                        UserNote = o.UserNote,
                        PackageNoInStock = o.PackageNoInStock,
                        UnsignName = o.UnsignName,
                        PacketNumber = o.PacketNumber,
                        Description = o.Description,
                        ProvisionalMoney = o.ProvisionalMoney,
                        DepositType = o.DepositType,
                        WarehouseDeliveryId = o.WarehouseDeliveryId,
                        WarehouseDeliveryName = o.WarehouseDeliveryName,
                        ApprovelUnit = o.ApprovelUnit,
                        ApprovelPrice = o.ApprovelPrice,
                        ContactName = o.ContactName,
                        ContactPhone = o.ContactPhone,
                        ContactAddress = o.ContactAddress,
                        ContactEmail = o.ContactEmail,
                        CustomerCareUserId = o.CustomerCareUserId,
                        CustomerCareFullName = o.CustomerCareFullName,
                        CustomerCareOfficeId = o.CustomerCareOfficeId,
                        CustomerCareOfficeName = o.CustomerCareOfficeName,
                        CustomerCareOfficeIdPath = o.CustomerCareOfficeIdPath,
                        CommentNo = d.CommentNo,
                        RequestStatus = d.Status,
                        NoteProcess = d.Note,
                        QuantityLose = Db.OrderDetailCountings.Where(x => x.OrderId == o.Id && x.IsDelete == false).Sum(x=> x.QuantityLose),
                        TotalPriceLose = Db.OrderDetailCountings.Where(x => x.OrderId == o.Id && x.IsDelete == false).Sum(x => x.TotalPrice),
                        RequestNo = Db.OrderDetailCountings.Count(x => x.OrderId == o.Id && x.IsDelete == false)
                    }).Where(predicate).Distinct().OrderBy(x => x.Created);

            totalRecord = query.Count();

            return query.Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<int> GetOrderCountingLoseCount(Expression<Func<OrderDetailCounting, bool>> predicate0, Expression<Func<AcountingLoseResult, bool>> predicate, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword)
        {
            var query = Db.OrderDetailCountings.Where(
                    x => x.IsDelete == false && (fromDate == null && toDate == null
                                                 ||
                                                 fromDate != null && (toDate != null) &&
                                                 x.Created >= fromDate && x.Created <= toDate
                                                 ||
                                                 fromDate == null && toDate.HasValue && x.Created <= toDate
                                                 ||
                                                 toDate == null && fromDate.HasValue && x.Created >= fromDate) &&
                         (status == null || x.Status == status.Value)).Where(predicate0)
                .Join(Db.Orders.Where(x => x.IsDelete == false && x.UnsignName.Contains(keyword)), d => d.OrderId,
                    o => o.Id,
                    (d, o) => new AcountingLoseResult
                    {
                        Id = o.Id,
                        Code = o.Code,
                        Type = o.Type,
                        WebsiteName = o.WebsiteName,
                        ShopId = o.ShopId,
                        ShopName = o.ShopName,
                        ShopLink = o.ShopLink,
                        ProductNo = o.ProductNo,
                        PackageNo = o.PackageNo,
                        ContractCode = o.ContractCode,
                        ContractCodes = o.ContractCodes,
                        LevelId = o.LevelId,
                        LevelName = o.LevelName,
                        TotalWeight = o.TotalWeight,
                        DiscountType = o.DiscountType,
                        DiscountValue = o.DiscountValue,
                        GiftCode = o.GiftCode,
                        CreatedTool = o.CreatedTool,
                        Currency = o.Currency,
                        ExchangeRate = o.ExchangeRate,
                        TotalExchange = o.TotalExchange,
                        TotalPrice = o.TotalPrice,
                        Total = o.Total,
                        HashTag = o.HashTag,
                        WarehouseId = o.WarehouseId,
                        WarehouseName = o.WarehouseName,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName,
                        CustomerEmail = o.CustomerEmail,
                        CustomerPhone = o.CustomerPhone,
                        CustomerAddress = o.CustomerAddress,
                        Status = o.Status,
                        UserId = o.UserId,
                        UserFullName = o.UserFullName,
                        OfficeId = o.OfficeId,
                        OfficeName = o.OfficeName,
                        OfficeIdPath = o.OfficeIdPath,
                        CreatedOfficeIdPath = o.CreatedOfficeIdPath,
                        CreatedUserId = o.CreatedUserId,
                        CreatedUserFullName = o.CreatedUserFullName,
                        CreatedOfficeId = o.CreatedOfficeId,
                        CreatedOfficeName = o.CreatedOfficeName,
                        OrderInfoId = o.OrderInfoId,
                        FromAddressId = o.FromAddressId,
                        ToAddressId = o.ToAddressId,
                        SystemId = o.SystemId,
                        SystemName = o.SystemName,
                        ServiceType = o.ServiceType,
                        Note = o.Note,
                        PrivateNote = o.PrivateNote,
                        LinkNo = o.LinkNo,
                        Created = o.Created,
                        LastUpdate = o.LastUpdate,
                        ExpectedDate = o.ExpectedDate,
                        TotalPurchase = o.TotalPurchase,
                        TotalAdvance = o.TotalAdvance,
                        ReasonCancel = o.ReasonCancel,
                        PriceBargain = o.PriceBargain,
                        PaidShop = o.PaidShop,
                        FeeShip = o.FeeShip,
                        FeeShipBargain = o.FeeShipBargain,
                        IsPayWarehouseShip = o.IsPayWarehouseShip,
                        UserNote = o.UserNote,
                        PackageNoInStock = o.PackageNoInStock,
                        UnsignName = o.UnsignName,
                        PacketNumber = o.PacketNumber,
                        Description = o.Description,
                        ProvisionalMoney = o.ProvisionalMoney,
                        DepositType = o.DepositType,
                        WarehouseDeliveryId = o.WarehouseDeliveryId,
                        WarehouseDeliveryName = o.WarehouseDeliveryName,
                        ApprovelUnit = o.ApprovelUnit,
                        ApprovelPrice = o.ApprovelPrice,
                        ContactName = o.ContactName,
                        ContactPhone = o.ContactPhone,
                        ContactAddress = o.ContactAddress,
                        ContactEmail = o.ContactEmail,
                        CustomerCareUserId = o.CustomerCareUserId,
                        CustomerCareFullName = o.CustomerCareFullName,
                        CustomerCareOfficeId = o.CustomerCareOfficeId,
                        CustomerCareOfficeName = o.CustomerCareOfficeName,
                        CustomerCareOfficeIdPath = o.CustomerCareOfficeIdPath,
                        QuantityLose = 0,
                        TotalPriceLose = 0,
                        CommentNo = 0,
                        RequestNo = 0,
                        RequestStatus = 0,
                        NoteProcess = ""
                    }).Where(predicate).Distinct().OrderBy(x => x.Created);

            return query.CountAsync();
        }

        public Task<List<OrderDetailCountingResult>> Search(out int totalRecord, string warehouseIdPath, bool isManager,
            int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword,
            int currentPage, int recordPerPage)
        {
            var query = Db.Orders.Where(x => !x.IsDelete && (x.Status >= (byte)OrderStatus.InWarehouse && x.Status != (byte)OrderStatus.Cancel) && x.UnsignName.Contains(keyword))
                .Join(Db.OrderServices.Where(x => x.Checked && (x.ServiceId == (byte) OrderServices.Audit)), o => o.Id, s => s.OrderId, (o, s) => new {o, s})
                .Join(Db.OrderDetails.Where(
                        x =>
                            !x.IsDelete && x.Status == (byte) OrderDetailStatus.Order && 
                            ((status == null) 
                                || ((status == 0) && (x.QuantityActuallyReceived == null))
                                || ((status == 1) && (x.QuantityActuallyReceived != null))
                                || ((status == 2) && (x.QuantityActuallyReceived != x.Quantity))) &&
                            (((fromDate == null) && (toDate == null))
                             ||
                             ((fromDate != null) && (toDate != null) &&
                              (x.CountingTime >= fromDate) && (x.CountingTime <= toDate))
                             ||
                             ((fromDate == null) && toDate.HasValue && (x.CountingTime <= toDate))
                             ||
                             ((toDate == null) && fromDate.HasValue && (x.CountingTime >= fromDate))) &&
                            ((userId == null) || (x.CountingUserId == userId)) //&&
                        //((x.CountingOfficeIdPath == null) ||
                        // (isManager && ((x.CountingOfficeIdPath == warehouseIdPath) ||
                        //                x.CountingOfficeIdPath.StartsWith(warehouseIdPath + "."))) ||
                        // (!isManager && (x.CountingOfficeIdPath == warehouseIdPath)))
                    ),
                    o => o.o.Id,
                    d => d.OrderId, (o, d) => new
                    {
                        d.BeginAmount,
                        d.Created,
                        Updated = d.LastUpdate,
                        o.o.CustomerAddress,
                        o.o.CustomerEmail,
                        o.o.CustomerId,
                        o.o.CustomerName,
                        o.o.CustomerPhone,
                        d.ExchangePrice,
                        d.ExchangeRate,
                        d.Image,
                        d.Link,
                        Mode = 0,
                        d.Name,
                        d.Note,
                        NotePrivate = d.UserNote,
                        o.o.OfficeId,
                        o.o.OfficeIdPath,
                        o.o.OfficeName,
                        o.o.UserId,
                        o.o.UserFullName,
                        o.o.WarehouseId,
                        WarehouseName = o.o.WebsiteName,
                        o.o.WebsiteName,
                        OrderStatus = o.o.Status,
                        OrderCode = o.o.Code,
                        OrderId = o.o.Id,
                        OrderDetailId = d.Id,
                        d.Price,
                        o.o.LinkNo,
                        d.Properties,
                        d.Quantity,
                        d.QuantityBooked,
                        d.QuantityRecived,
                        d.QuantityIncorrect,
                        QuantityActual = d.QuantityActuallyReceived,
                        o.o.ShopId,
                        o.o.ShopLink,
                        o.o.ShopName,
                        d.TotalExchange,
                        d.TotalPrice,
                        d.CountingTime,
                        d.CountingUserName,
                        d.CountingUserId,
                        d.CountingFullName,
                        d.CountingOfficeId,
                        d.CountingOfficeName,
                        o.o.WarehouseDeliveryId,
                        o.o.WarehouseDeliveryName,
                        RequestLose = Db.OrderDetailCountings.Count(x=> x.OrderDetailId == d.Id && x.IsDelete == false)
                    });

            totalRecord = query.Select(x => x.OrderId)
                .Distinct().Count();

            var orders = query.Select(x => x.OrderCode)
                .Distinct()
                .OrderBy(x => x)
                .Skip((currentPage - 1)*recordPerPage)
                .Take(recordPerPage).ToList();

            var codes = $";{string.Join(";", orders)};";

            return query.Where(x => codes.Contains(";" + x.OrderCode + ";"))
                .Select(x => new OrderDetailCountingResult
                {
                    BeginAmount = x.BeginAmount,
                    Created = x.Created,
                    Updated = x.Updated,
                    CustomerAddress = x.CustomerAddress,
                    CustomerEmail = x.CustomerEmail,
                    CustomerId = x.CustomerId,
                    CustomerName = x.CustomerName,
                    CustomerPhone = x.CustomerPhone,
                    ExchangePrice = x.ExchangePrice,
                    ExchangeRate = x.ExchangeRate,
                    Image = x.Image,
                    Link = x.Link,
                    Mode = 0,
                    Name = x.Name,
                    Note = x.Note,
                    NotePrivate = x.NotePrivate,
                    OfficeId = x.OfficeId,
                    OfficeIdPath = x.OfficeIdPath,
                    OfficeName = x.OfficeName,
                    UserId = x.UserId,
                    UserFullName = x.UserFullName,
                    WarehouseId = x.WarehouseId,
                    WarehouseName = x.WebsiteName,
                    WebsiteName = x.WebsiteName,
                    OrderCode = x.OrderCode,
                    OrderId = x.OrderId,
                    OrderStatus = x.OrderStatus,
                    OrderDetailId = x.OrderDetailId,
                    Price = x.Price,
                    ProductNo = x.LinkNo,
                    Properties = x.Properties,
                    Quantity = x.Quantity,
                    QuantityBooked = x.QuantityBooked,
                    QuantityRecived = x.QuantityRecived,
                    QuantityIncorrect = x.QuantityIncorrect,
                    QuantityActual = x.QuantityActual,
                    ShopId = x.ShopId,
                    ShopLink = x.ShopLink,
                    ShopName = x.ShopName,
                    TotalExchange = x.TotalExchange,
                    TotalPrice = x.TotalPrice,
                    CountingUserId = x.CountingUserId,
                    CountingUserName = x.CountingUserName,
                    CountingFullName = x.CountingFullName,
                    CountingTime = x.CountingTime,
                    CountingOfficeId    = x.CountingOfficeId,
                    CountingOfficeName = x.CountingOfficeName,
                    WarehouseDeliveryId = x.WarehouseDeliveryId ?? 0,
                    WarehouseDeliveryName = x.WarehouseDeliveryName,
                    RequestLose = x.RequestLose
                }).Distinct().ToListAsync();
        }
    }
}

