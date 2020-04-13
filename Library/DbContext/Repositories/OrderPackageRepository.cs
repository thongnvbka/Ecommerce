using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.ViewModels.Account;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Common.Items;
using Library.DbContext.Results;
using Library.Models;
using Library.ViewModels.Items;

namespace Library.DbContext.Repositories
{
    public class OrderPackageRepository : Repository<OrderPackage>
    {
        public OrderPackageRepository(ProjectXContext context) : base(context)
        {
        }

        /// <summary>
        /// Lấy ra các yều cầu xử lý mất mã được tạo trong khoảng thời gian
        /// </summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <param name="mode">0: Hàng mất mã, 1 Xác nhận hàng mất mã</param>
        /// <returns></returns>
        public Task<Dictionary<int, int>> GetPackageNoCodeCreatedAsync(DateTime? fromDate, DateTime? toDate,
            byte? mode)
        {
            return Db.PackageNoCodes
                .Where(x => (mode == null || x.Mode == mode) &&
                            (fromDate == null && toDate == null
                             || fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate
                             || fromDate == null && toDate.HasValue && x.Created <= toDate
                             || toDate == null && fromDate.HasValue && x.Created >= fromDate))
                .GroupBy(x=> x.Created.Day)
                .ToDictionaryAsync(x=> x.Key, x=> x.Count());
        }


        public Task<List<PackageNoCodeForReportResult>> GetPackageNoCodesCreatedAsync(DateTime time,
            byte? mode, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.PackageNoCodes.Where(x => (mode == null || x.Mode == mode) && x.Created.Day == time.Day && x.Created.Month == time.Month && x.Created.Year == time.Year)
                .Join(Db.OrderPackages.Where(x=> x.IsDelete == false), x=> x.PackageId, y=> y.Id, (p, package) => new PackageNoCodeForReportResult{
                    Id = p.Id,
                    PackageId = p.PackageId,
                    PackageCode = p.PackageCode,
                    TransportCode = package.TransportCode,
                    Note = p.Note,
                    UnsignText = p.UnsignText,
                    Status = p.Status,
                    Mode = p.Mode,
                    ImageJson = p.ImageJson,
                    Created = p.Created,
                    Updated = p.Updated,
                    CreateUserId = p.CreateUserId,
                    CreateUserFullName = p.CreateUserFullName,
                    CreateUserName = p.CreateUserName,
                    CreateOfficeId = p.CreateOfficeId,
                    CreateOfficeName = p.CreateOfficeName,
                    CreateOfficeIdPath = p.CreateOfficeIdPath,
                    UpdateUserId = p.UpdateUserId,
                    UpdateUserFullName = p.UpdateUserFullName,
                    UpdateUserName = p.UpdateUserName,
                    UpdateOfficeId = p.UpdateOfficeId,
                    CommentNo = p.CommentNo,
                    UpdateOfficeName = p.UpdateOfficeName,
                    UpdateOfficeIdPath = p.UpdateOfficeIdPath,
                });

            totalRecord = query.LongCount();
           return query.OrderBy(x=> x.Id).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        /// <summary>
        /// Lấy ra các thông tin mất mã được xử lý
        /// </summary>
        /// <param name="fromDate">Từ ngày</param>
        /// <param name="toDate">Đến ngày</param>
        /// <param name="mode">0: Hàng mất mã, 1 Xác nhận hàng mất mã</param>
        /// <returns></returns>
        public Task<Dictionary<int, int>> GetPackageNoCodeUpdateAsync(DateTime? fromDate, DateTime? toDate,
            byte? mode)
        {
            return Db.PackageNoCodes
                .Where(x => (mode == null || x.Mode == mode) && x.Status == 1 &&
                            (fromDate == null && toDate == null
                             || fromDate != null && toDate != null && x.Updated >= fromDate && x.Updated <= toDate
                             || fromDate == null && toDate.HasValue && x.Updated <= toDate
                             || toDate == null && fromDate.HasValue && x.Updated >= fromDate))
                .GroupBy(x => x.Updated.Day)
                .ToDictionaryAsync(x => x.Key, x => x.Count());
        }

        public Task<List<PackageNoCodeForReportResult>> GetPackageNoCodesUpdateAsync(DateTime time,
            byte? mode, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.PackageNoCodes
                .Where(x => (mode == null || x.Mode == mode) && x.Status == 1 &&
                            x.Updated.Day == time.Day && x.Updated.Month == time.Month && x.Created.Year == time.Year)
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false), x => x.PackageId, y => y.Id,
                    (p, package) => new PackageNoCodeForReportResult
                    {
                        Id = p.Id,
                        PackageId = p.PackageId,
                        PackageCode = p.PackageCode,
                        TransportCode = package.TransportCode,
                        Note = p.Note,
                        UnsignText = p.UnsignText,
                        Status = p.Status,
                        Mode = p.Mode,
                        ImageJson = p.ImageJson,
                        Created = p.Created,
                        Updated = p.Updated,
                        CreateUserId = p.CreateUserId,
                        CreateUserFullName = p.CreateUserFullName,
                        CreateUserName = p.CreateUserName,
                        CreateOfficeId = p.CreateOfficeId,
                        CreateOfficeName = p.CreateOfficeName,
                        CreateOfficeIdPath = p.CreateOfficeIdPath,
                        UpdateUserId = p.UpdateUserId,
                        UpdateUserFullName = p.UpdateUserFullName,
                        UpdateUserName = p.UpdateUserName,
                        UpdateOfficeId = p.UpdateOfficeId,
                        CommentNo = p.CommentNo,
                        UpdateOfficeName = p.UpdateOfficeName,
                        UpdateOfficeIdPath = p.UpdateOfficeIdPath,
                    });

            totalRecord = query.LongCount();

            return query.OrderBy(x=> x.Id).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        /// <summary>
        /// Lấy ra kiện nhập, xuất, tồn theo thời gian
        /// </summary>
        /// <param name="fromDate">Ngày bắt đầu</param>
        /// <param name="toDate">Ngày kết thúc</param>
        /// <param name="mode">0: Nhập kho (Tính từ lúc putaway), 1: Xuất kho (đang vận chuyển, đang đi giao), 2: Tồn kho, 3: mât hàng</param>
        /// <param name="warehouseId">Kho hàng</param>
        /// <param name="warehouseCulture">= "CH" kho TQ, khác là kho VN</param>
        /// <returns></returns>
        public Task<Dictionary<int, int>> GetPackageForReportAsync(DateTime? fromDate, DateTime? toDate, 
            byte mode, int warehouseId, string warehouseCulture)
        {
            // Query bao hàng được tạo trong kho
            Expression<Func<OrderPackage, bool>> packageQuery = x =>
                x.IsDelete == false &&
                (warehouseCulture == "CH" && x.WarehouseId == warehouseId ||
                 warehouseCulture != "CH" && x.CustomerWarehouseId == warehouseId);

           // Query bao hàng được tạo trong kho
            Expression<Func<PackageHistory, bool>> packageHistory = x =>
                mode == 0 && warehouseCulture == "CH" && x.Status == (byte) OrderPackageStatus.ChinaReceived
                || mode == 0 && warehouseCulture != "CH" && x.Status == (byte) OrderPackageStatus.Received
                || mode == 1 && warehouseCulture == "CH" && x.Status == (byte) OrderPackageStatus.PartnerDelivery
                || mode == 1 && warehouseCulture != "CH" && x.Status == (byte) OrderPackageStatus.GoingDelivery
                || mode == 2 && warehouseCulture == "CH" && (x.Status == (byte) OrderPackageStatus.ChinaInStock
                                                             || x.Status == (byte) OrderPackageStatus.ChinaExport)
                || mode == 2 && warehouseCulture != "CH" && x.Status == (byte) OrderPackageStatus.InStock
                || mode == 3 && warehouseCulture != "CH" && x.Status == (byte) OrderPackageStatus.Lost;

            return Db.OrderPackages.Where(packageQuery).Join(Db.PackageHistories
                .Where(packageHistory), x => x.Id,
                    h => h.PackageId, (p, h) => new PackageReportResult
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Status = p.Status,
                        Note = p.Note,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        OrderServices = p.OrderServices,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        CustomerUserName = p.CustomerUserName,
                        CustomerLevelId = p.CustomerLevelId,
                        CustomerLevelName = p.CustomerLevelName,
                        CustomerWarehouseId = p.CustomerWarehouseId,
                        CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                        CustomerWarehouseName = p.CustomerWarehouseName,
                        CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                        TransportCode = p.TransportCode,
                        Weight = p.Weight,
                        WeightConverted = p.WeightConverted,
                        WeightActual = p.WeightActual,
                        WeightActualPercent = p.WeightActualPercent,
                        WeightWapperPercent = p.WeightWapperPercent,
                        OtherService = p.OtherService,
                        WeightWapper = p.WeightWapper,
                        TotalPriceWapper = p.TotalPriceWapper,
                        Volume = p.Volume,
                        VolumeActual = p.VolumeActual,
                        VolumeWapperPercent = p.VolumeWapperPercent,
                        VolumeWapper = p.VolumeWapper,
                        Size = p.Size,
                        Width = p.Width,
                        Height = p.Height,
                        Length = p.Length,
                        TotalPrice = p.TotalPrice,
                        WarehouseId = p.WarehouseId,
                        WarehouseName = p.WarehouseName,
                        WarehouseIdPath = p.WarehouseIdPath,
                        WarehouseAddress = p.WarehouseAddress,
                        UserId = p.UserId,
                        UserFullName = p.UserFullName,
                        SystemId = p.SystemId,
                        SystemName = p.SystemName,
                        Created = p.Created,
                        LastUpdate = p.LastUpdate,
                        HashTag = p.HashTag,
                        ForcastDate = p.ForcastDate,
                        PackageNo = p.PackageNo,
                        CurrentLayoutId = p.CurrentLayoutId,
                        CurrentLayoutName = p.CurrentLayoutName,
                        CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                        CurrentWarehouseId = p.CurrentWarehouseId,
                        OrderType = p.OrderType,
                        Mode = p.Mode,
                        SameCodeStatus = p.SameCodeStatus,
                        CreateDate = h.CreateDate
                    }).Where(x=> fromDate == null && toDate == null
                    || fromDate != null && toDate != null && x.CreateDate >= fromDate && x.CreateDate <= toDate
                    || fromDate == null && toDate.HasValue && x.CreateDate <= toDate
                    || toDate == null && fromDate.HasValue && x.CreateDate >= fromDate)
                    .GroupBy(x=> x.CreateDate.Day)
                    .ToDictionaryAsync(x=> x.Key, x=> x.Count());
        }

        public Task<List<PackageReportResult>> GetPackagesForReportAsync(DateTime time,
            byte mode, int warehouseId, string warehouseCulture, int currentPage, int recordPerPage, out long totalRecord)
        {
            // Query bao hàng được tạo trong kho
            Expression<Func<OrderPackage, bool>> packageQuery = x =>
                x.IsDelete == false &&
                (warehouseCulture == "CH" && x.WarehouseId == warehouseId ||
                 warehouseCulture != "CH" && x.CustomerWarehouseId == warehouseId);

            // Query bao hàng được tạo trong kho
            Expression<Func<PackageHistory, bool>> packageHistory = x =>
                mode == 0 && warehouseCulture == "CH" && x.Status == (byte)OrderPackageStatus.ChinaReceived
                || mode == 0 && warehouseCulture != "CH" && x.Status == (byte)OrderPackageStatus.Received
                || mode == 1 && warehouseCulture == "CH" && x.Status == (byte)OrderPackageStatus.PartnerDelivery
                || mode == 1 && warehouseCulture != "CH" && x.Status == (byte)OrderPackageStatus.GoingDelivery
                || mode == 2 && warehouseCulture == "CH" && (x.Status == (byte)OrderPackageStatus.ChinaInStock
                                                             || x.Status == (byte)OrderPackageStatus.ChinaExport)
                || mode == 2 && warehouseCulture != "CH" && x.Status == (byte)OrderPackageStatus.InStock
                || mode == 3 && warehouseCulture != "CH" && x.Status == (byte)OrderPackageStatus.Lost;

            var query = Db.OrderPackages.Where(packageQuery).Join(Db.PackageHistories
                .Where(packageHistory), x => x.Id,
                    h => h.PackageId, (p, h) => new PackageReportResult
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Status = p.Status,
                        Note = p.Note,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        OrderServices = p.OrderServices,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        CustomerUserName = p.CustomerUserName,
                        CustomerLevelId = p.CustomerLevelId,
                        CustomerLevelName = p.CustomerLevelName,
                        CustomerWarehouseId = p.CustomerWarehouseId,
                        CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                        CustomerWarehouseName = p.CustomerWarehouseName,
                        CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                        TransportCode = p.TransportCode,
                        Weight = p.Weight,
                        WeightConverted = p.WeightConverted,
                        WeightActual = p.WeightActual,
                        WeightActualPercent = p.WeightActualPercent,
                        WeightWapperPercent = p.WeightWapperPercent,
                        OtherService = p.OtherService,
                        WeightWapper = p.WeightWapper,
                        TotalPriceWapper = p.TotalPriceWapper,
                        Volume = p.Volume,
                        VolumeActual = p.VolumeActual,
                        VolumeWapperPercent = p.VolumeWapperPercent,
                        VolumeWapper = p.VolumeWapper,
                        Size = p.Size,
                        Width = p.Width,
                        Height = p.Height,
                        Length = p.Length,
                        TotalPrice = p.TotalPrice,
                        WarehouseId = p.WarehouseId,
                        WarehouseName = p.WarehouseName,
                        WarehouseIdPath = p.WarehouseIdPath,
                        WarehouseAddress = p.WarehouseAddress,
                        UserId = p.UserId,
                        UserFullName = p.UserFullName,
                        SystemId = p.SystemId,
                        SystemName = p.SystemName,
                        Created = p.Created,
                        LastUpdate = p.LastUpdate,
                        HashTag = p.HashTag,
                        ForcastDate = p.ForcastDate,
                        PackageNo = p.PackageNo,
                        CurrentLayoutId = p.CurrentLayoutId,
                        CurrentLayoutName = p.CurrentLayoutName,
                        CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                        CurrentWarehouseId = p.CurrentWarehouseId,
                        OrderType = p.OrderType,
                        Mode = p.Mode,
                        SameCodeStatus = p.SameCodeStatus,
                        CreateDate = h.CreateDate
                    }).Where(x => x.CreateDate.Day == time.Day && x.CreateDate.Month == time.Month && x.CreateDate.Year == time.Year);


            totalRecord = query.LongCount();

            return query.OrderBy(x=> x.Id).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<string>> GetPackageSameCode(out long totalRecord, byte? status, DateTime? fromDate, DateTime? toDate,
            byte? orderType, string keyword, int currentPage, int recordPerPage)
        {
            var query = Db.OrderPackages.Where(
                    x => x.Mode != null && x.IsDelete == false && (status == null || x.SameCodeStatus == status) && x.OrderId > 0 &&
                    (orderType == null || x.OrderType == orderType.Value) && 
                    (keyword == "" || x.UnsignedText.Contains(keyword)) &&
                     (fromDate == null && toDate == null 
                     || fromDate != null && toDate != null && x.ForcastDate >= fromDate && x.ForcastDate <= toDate 
                     || fromDate == null && toDate.HasValue && x.ForcastDate <= toDate 
                     || toDate == null && fromDate.HasValue && x.ForcastDate >= fromDate))
                .Select(x => x.TransportCode).Distinct();

            totalRecord = query.LongCount();

            return query.OrderBy(x => x).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync(); 
        }

        public Task<List<OrderPackage>> GetPackageSameCode(string transportCodes)
        {
            return Db.OrderPackages.Where(
                        x => x.Mode != null && x.IsDelete == false &&
                            transportCodes.Contains(";" + x.TransportCode + ";") &&
                            x.OrderId > 0)
                    .OrderBy(x => new {x.TransportCode, x.CustomerWarehouseId, x.OrderId, x.Id})
                    .ToListAsync();
        }

        /// <summary>
        /// Tính tổng cân nặng tính tiền của các kiện hàng xuất kho tại TQ
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalActualWeight(int orderId)
        {
            var query = Db.Delivery.Where(x => x.IsDelete == false && x.Status != (byte) DeliveryStatus.Cancel)
                .Join(Db.DeliveryDetails.Where(x => x.IsDelete == false), d => d.Id, dd => dd.DeliveryId,
                    (delivery, detail) => new {delivery, detail})
                .Join(Db.OrderPackages.Where(x => x.IsDelete && x.WeightActual != null && x.OrderId == orderId),
                    arg => arg.detail.PackageId,
                    p => p.Id, (arg, p) => new {arg.delivery, p})
                .Where(x => x.delivery.CreatedOfficeId == x.p.WarehouseId)
                .Select(x => x.p)
                .Distinct();

            return !query.Any() ? 0 : query.Sum(x=> x.WeightActual.Value);
        }

        public List<OrderPackageOverDayResult> OrderPackageOverDay(int day)
        {
            var timeNow = DateTime.Now.AddDays(day * -1);

            return Db.OrderPackages.Where(
                    x => !x.IsDelete && x.Status == (byte) OrderPackageStatus.ShopDelivery && x.Created <= timeNow)
                .Join(Db.Orders, p => p.OrderId, o => o.Id, (p, o) => new {p, o})
                .Select(x=> new OrderPackageOverDayResult
                {
                    OrderId = x.p.OrderId,
                    OrderCode = x.p.OrderCode,
                    OrderType = x.p.OrderType,
                    PackageCode = x.p.Code,
                    UserId = x.o.UserId ?? 0,
                    UserFullName = x.o.UserFullName
                }).ToList();
        }

        public Task<List<PackageForTrackingResult>> GetOrderPackageForTracking(string keyword, byte? searchType, string statusText, 
            string warehouseIdText, string orderStatusText, string statusDepositText, string orderTypeText, string orderServiceText, DateTime? fromDate, DateTime? toDate, 
            string timeTypeText, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.OrderPackages.Where(
                    x => x.IsDelete == false && (searchType == null && x.UnsignedText.Contains(keyword) 
                    || searchType == 0 && x.TransportCode == keyword
                    || searchType == 1 && x.OrderCode == keyword
                    || searchType == 2 && x.Code == keyword
                    || searchType == 3 && x.CustomerUserName == keyword)  &&
                         (statusText == "" || statusText.Contains(";" + x.Status + ";")) &&
                         (warehouseIdText == "" || warehouseIdText.Contains(";" + x.CustomerWarehouseId + ";")) &&
                         (orderTypeText == "" || orderTypeText.Contains(";" + x.OrderType + ";")))
                 .Join(Db.OrderServices.Where(x=> x.IsDelete == false && x.Checked && (orderServiceText == "" || orderServiceText.Contains(";"+x.ServiceId+";"))), p=> p.OrderId, s=> s.OrderId, (p, s)=> p)
                 .Join(Db.PackageHistories.Where(x=> timeTypeText == "" 
                 || timeTypeText.Contains(";" + x.Status + ";") && 
                    (fromDate == null && toDate == null 
                     || fromDate != null && toDate != null && x.CreateDate >= fromDate && x.CreateDate <= toDate 
                     || fromDate == null && toDate.HasValue && x.CreateDate <= toDate 
                     || toDate == null && fromDate.HasValue && x.CreateDate >= fromDate)), 
                 x=> x.Id, x=> x.PackageId, (package, history) => package)
                .Join(Db.Orders.Where(
                        x => x.IsDelete == false && (x.Type != (byte)OrderType.Deposit && (statusDepositText == "" || statusDepositText.Contains(";" + x.Status + ";"))
                    || x.Type == (byte)OrderType.Deposit && (statusDepositText == "" || statusDepositText.Contains(";" + x.Status + ";")))),
                    p => p.OrderId, o => o.Id, (p, o) => new PackageForTrackingResult()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Status = p.Status,
                        Note = p.Note,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        OrderServices = p.OrderServices,
                        OrderType = p.OrderType,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        CustomerPhone = o.CustomerPhone,
                        CustomerUserName = p.CustomerUserName,
                        CustomerLevelId = p.CustomerLevelId,
                        CustomerLevelName = p.CustomerLevelName,
                        CustomerWarehouseId = p.CustomerWarehouseId,
                        CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                        CustomerWarehouseName = p.CustomerWarehouseName,
                        CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                        TransportCode = p.TransportCode,
                        Weight = p.Weight,
                        WeightConverted = p.WeightConverted,
                        WeightActual = p.WeightActual,
                        WeightActualPercent = p.WeightActualPercent,
                        WeightWapperPercent = p.WeightWapperPercent,
                        OtherService = p.OtherService,
                        WeightWapper = p.WeightWapper,
                        TotalPriceWapper = p.TotalPriceWapper,
                        Volume = p.Volume,
                        VolumeActual = p.VolumeActual,
                        VolumeWapperPercent = p.VolumeWapperPercent,
                        VolumeWapper = p.VolumeWapper,
                        Size = p.Size,
                        Width = p.Width,
                        Height = p.Height,
                        Length = p.Length,
                        TotalPrice = p.TotalPrice,
                        WarehouseId = p.WarehouseId,
                        WarehouseName = p.WarehouseName,
                        WarehouseIdPath = p.WarehouseIdPath,
                        WarehouseAddress = p.WarehouseAddress,
                        UserId = p.UserId,
                        UserFullName = p.UserFullName,
                        SystemId = p.SystemId,
                        SystemName = p.SystemName,
                        Created = p.Created,
                        LastUpdate = p.LastUpdate,
                        HashTag = p.HashTag,
                        ForcastDate = p.ForcastDate,
                        PackageNo = p.PackageNo,
                        UnsignedText = p.UnsignedText,
                        CurrentLayoutId = p.CurrentLayoutId,
                        CurrentLayoutName = p.CurrentLayoutName,
                        CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                        CurrentWarehouseId = p.CurrentWarehouseId,
                        CurrentWarehouseName = p.CurrentWarehouseName,
                        CurrentWarehouseIdPath = p.CurrentWarehouseIdPath,
                        CurrentWarehouseAddress = p.CurrentWarehouseAddress,
                        IsDelete = p.IsDelete,
                        OrderCodes = p.OrderCodes,
                        PackageCodes = p.PackageCodes,
                        Customers = p.Customers,
                        OrderCodesUnsigned = p.OrderCodesUnsigned,
                        PackageCodesUnsigned = p.PackageCodesUnsigned,
                        CustomersUnsigned = p.CustomersUnsigned,
                    }).Distinct();

            totalRecord = query.LongCount();

            return query.OrderBy(x => x.OrderId)
                .Skip((currentPage - 1) * recordPerPage)
                .Take(recordPerPage)
                .ToListAsync();
        }

        /// <summary>
        /// Tính tiền phí lưu kho của các kiện hàng
        /// </summary>
        /// <param name="packageIds">Id kiện hàng</param>
        /// <param name="userState">Nhân viên đang đăng nhập</param>
        /// <returns></returns>
        public Task<List<OrderPackageDatePutAwayResult>> GetPriceStoredInWarehouse(string packageIds, UserState userState)
        {
            return Db.PutAwayDetails.Where(x => packageIds.Contains(";" + x.PackageId + ";"))
                .Join(Db.PutAways
                    .Where(x => x.WarehouseId == userState.OfficeId), p => p.PutAwayId, p => p.Id, (d, p) => new { d.PackageId, p.Created })
                .GroupBy(x => x.PackageId)
                .Select(g => g.OrderByDescending(c => c.Created).FirstOrDefault())
                .Join(Db.OrderPackages, arg => arg.PackageId, p => p.Id, (g, p) => new OrderPackageDatePutAwayResult
                {
                    Id = p.Id,
                    Code = p.Code,
                    Status = p.Status,
                    Note = p.Note,
                    OrderId = p.OrderId,
                    OrderCode = p.OrderCode,
                    OrderServices = p.OrderServices,
                    CustomerId = p.CustomerId,
                    CustomerName = p.CustomerName,
                    CustomerUserName = p.CustomerUserName,
                    CustomerLevelId = p.CustomerLevelId,
                    CustomerLevelName = p.CustomerLevelName,
                    CustomerWarehouseId = p.CustomerWarehouseId,
                    CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                    CustomerWarehouseName = p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                    TransportCode = p.TransportCode,
                    Weight = p.Weight,
                    WeightConverted = p.WeightConverted,
                    WeightActual = p.WeightActual,
                    WeightActualPercent = p.WeightActualPercent,
                    WeightWapperPercent = p.WeightWapperPercent,
                    OtherService = p.OtherService,
                    WeightWapper = p.WeightWapper,
                    TotalPriceWapper = p.TotalPriceWapper,
                    Volume = p.Volume,
                    VolumeActual = p.VolumeActual,
                    VolumeWapperPercent = p.VolumeWapperPercent,
                    VolumeWapper = p.VolumeWapper,
                    Size = p.Size,
                    Width = p.Width,
                    Height = p.Height,
                    Length = p.Length,
                    TotalPrice = p.TotalPrice,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.WarehouseName,
                    WarehouseIdPath = p.WarehouseIdPath,
                    WarehouseAddress = p.WarehouseAddress,
                    UserId = p.UserId,
                    UserFullName = p.UserFullName,
                    SystemId = p.SystemId,
                    SystemName = p.SystemName,
                    Created = p.Created,
                    LastUpdate = p.LastUpdate,
                    PutAwayTime = g.Created,
                    HashTag = p.HashTag,
                    ForcastDate = p.ForcastDate,
                    PackageNo = p.PackageNo,
                    UnsignedText = p.UnsignedText,
                    CurrentLayoutId = p.CurrentLayoutId,
                    CurrentLayoutName = p.CurrentLayoutName,
                    CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                    CurrentWarehouseId = p.CurrentWarehouseId,
                    CurrentWarehouseName = p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = p.CurrentWarehouseAddress,
                    IsDelete = p.IsDelete,
                    OrderCodes = p.OrderCodes,
                    PackageCodes = p.PackageCodes,
                    Customers = p.Customers,
                    OrderCodesUnsigned = p.OrderCodesUnsigned,
                    PackageCodesUnsigned = p.PackageCodesUnsigned,
                    CustomersUnsigned = p.CustomersUnsigned,
                    OrderType = p.OrderType
                }).ToListAsync();
        }

        public Task<List<OrderPackageWapResult>> GetByPackageIds(string packageIds)
        {
            var walletQuery = Db.Wallet.Where(x => x.IsDelete == false && x.Mode == 0)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false), w => w.Id, d => d.WalletId,
                    (w, d) => new { d.WalletCode, d.PackageId }).Distinct();

            return Db.OrderPackages.Where(x => packageIds.Contains(";" + x.Id + ";"))
                .Join(Db.Orders, x => x.OrderId, o => o.Id, (p, o) => new { p, o })
                .GroupJoin(walletQuery, arg => arg.p.Id, wd => wd.PackageId, (arg, wd) => new { arg.p, arg.o, wd })
                .SelectMany(x => x.wd.DefaultIfEmpty(), (arg, wd) => new OrderPackageWapResult()
                {
                    Id = arg.p.Id,
                    Code = arg.p.Code,
                    Status = arg.p.Status,
                    Note = arg.p.Note,
                    OrderId = arg.p.OrderId,
                    OrderCode = arg.p.OrderCode,
                    OrderServices = arg.p.OrderServices,
                    CustomerId = arg.p.CustomerId,
                    CustomerName = arg.p.CustomerName,
                    CustomerUserName = arg.p.CustomerUserName,
                    CustomerLevelId = arg.p.CustomerLevelId,
                    CustomerLevelName = arg.p.CustomerLevelName,
                    CustomerWarehouseId = arg.p.CustomerWarehouseId,
                    CustomerWarehouseAddress = arg.p.CustomerWarehouseAddress,
                    CustomerWarehouseName = arg.p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = arg.p.CustomerWarehouseIdPath,
                    TransportCode = arg.p.TransportCode,
                    Weight = arg.p.Weight,
                    WeightConverted = arg.p.WeightConverted,
                    WeightActual = arg.p.WeightActual,
                    WeightActualPercent = arg.p.WeightActualPercent,
                    WeightWapperPercent = arg.p.WeightWapperPercent,
                    OtherService = arg.p.OtherService,
                    WeightWapper = arg.p.WeightWapper,
                    TotalPriceWapper = arg.p.TotalPriceWapper,
                    TotalPriceWapperExchange = arg.p.TotalPriceWapper * arg.o.ExchangeRate,
                    Volume = arg.p.Volume,
                    VolumeActual = arg.p.VolumeActual,
                    VolumeWapperPercent = arg.p.VolumeWapperPercent,
                    VolumeWapper = arg.p.VolumeWapper,
                    Size = arg.p.Size,
                    Width = arg.p.Width,
                    Height = arg.p.Height,
                    Length = arg.p.Length,
                    TotalPrice = arg.p.TotalPrice,
                    WarehouseId = arg.p.WarehouseId,
                    WarehouseName = arg.p.WarehouseName,
                    WarehouseIdPath = arg.p.WarehouseIdPath,
                    WarehouseAddress = arg.p.WarehouseAddress,
                    UserId = arg.p.UserId,
                    UserFullName = arg.p.UserFullName,
                    SystemId = arg.p.SystemId,
                    SystemName = arg.p.SystemName,
                    Created = arg.p.Created,
                    LastUpdate = arg.p.LastUpdate,
                    HashTag = arg.p.HashTag,
                    ForcastDate = arg.p.ForcastDate,
                    PackageNo = arg.p.PackageNo,
                    UnsignedText = arg.p.UnsignedText,
                    CurrentLayoutId = arg.p.CurrentLayoutId,
                    CurrentLayoutName = arg.p.CurrentLayoutName,
                    CurrentLayoutIdPath = arg.p.CurrentLayoutIdPath,
                    CurrentWarehouseId = arg.p.CurrentWarehouseId,
                    CurrentWarehouseName = arg.p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = arg.p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = arg.p.CurrentWarehouseAddress,
                    IsDelete = arg.p.IsDelete,
                    OrderCodes = arg.p.OrderCodes,
                    PackageCodes = arg.p.PackageCodes,
                    Customers = arg.p.Customers,
                    OrderCodesUnsigned = arg.p.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.p.PackageCodesUnsigned,
                    CustomersUnsigned = arg.p.CustomersUnsigned,
                    OrderType = arg.p.OrderType,
                    Mode = arg.p.Mode,
                    SameCodeStatus = arg.p.SameCodeStatus,
                    WalletCode = wd.WalletCode
                })
                .OrderBy(x => x.OrderId)
                .ToListAsync();
        }

        public List<OrderPackage> GetByOrderIdAndImportWarehouseId(int orderId, int importWarehouseId)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false && x.OrderId == orderId)
                .Join(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.ImportWarehouseId == importWarehouseId),
                    p => p.Id, d => d.PackageId, (p, d) => p).ToList();
        }

        public Task<List<OrderPackage>> GetByOrderIdAndImportWarehouseIdAsync(int orderId, int importWarehouseId)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false && x.OrderId == orderId)
                .Join(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.ImportWarehouseId == importWarehouseId),
                    p => p.Id, d => d.PackageId, (p, d) => p).ToListAsync();
        }

        public Task<List<OrderPackage>> GetByOrderIdAndWalletIdAsync(int orderId, int walletId)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false && x.OrderId == orderId)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false && x.WalletId == walletId),
                    p => p.Id, d => d.PackageId, (p, d) => p).ToListAsync();
        }

        public List<OrderPackage> GetByOrderIdAndWalletId(int orderId, int walletId)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false && x.OrderId == orderId)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false && x.WalletId == walletId),
                    p => p.Id, d => d.PackageId, (p, d) => p).ToList();
        }

        public Task<List<OrderPackage>> GetByWalletId(int walletId)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false && x.WalletId == walletId),
                    p => p.Id, d => d.PackageId, (p, d) => p).ToListAsync();
        }

        public Task<List<SuggetionOrderPackageResult>> SuggetionForImportWarehouse(string packageCodes, string warehouseIdPath, string term, int size, bool isPaste)
        {
            if (isPaste)
            {
                term = term.Trim();
            }

            // Các kiện hàng trong phiếu nhập kho
            var importWarehouseDetails = Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 0)
                .Join(Db.ImportWarehouse.Where(x => x.IsDelete == false), iwd => iwd.ImportWarehouseId, iw => iw.Id,
                    (detail, warehouse) => detail);

            // Danh sách các kiện hàng thuộc hàng mát mã chưa được xử lý
            var packageNoCode = Db.PackageNoCodes.Where(x => x.Status == 0);

            // Kiện hàng chưa được nhập kho
            var packages = Db.OrderPackages.Where(
                    x => x.Status == (byte) OrderPackageStatus.ShopDelivery && x.IsDelete == false &&
                         x.WarehouseIdPath == warehouseIdPath && x.OrderId != 0 &&
                         x.TransportCode != "" &&
                         ((isPaste && x.TransportCode == term) || (isPaste == false &&
                          (x.Code.Contains(term) || x.TransportCode.Contains(term) || x.OrderCode.Contains(term)))) &&
                         (packageCodes == "" || !packageCodes.Contains(";" + x.Code + ";")))
                .GroupJoin(importWarehouseDetails, op => op.Id, iwd => iwd.PackageId,
                    (op, iwd) => new {op, iwd})
                .SelectMany(x => x.iwd.DefaultIfEmpty(), (op, iwd) => new {op.op, iwd})
                .Where(x => x.iwd == null)
                .Select(x => x.op);

            // Kiện hàng chưa đc nhập kho và không phải hàng mất mã chưa đc xử lý
            return packages.GroupJoin(packageNoCode, p => p.Id, pn => pn.PackageId, (p, pn) => new { p, pn })
                .SelectMany(x => x.pn.DefaultIfEmpty(), (p, pn) => new { p.p, pn })
                .Where(x => x.pn == null)
                .Select(x => new SuggetionOrderPackageResult()
                {
                    Id = x.p.Id,
                    Code = x.p.Code,
                    Status = x.p.Status,
                    Note = x.p.Note,
                    OrderId = x.p.OrderId,
                    OrderCode = x.p.OrderCode,
                    OrderServices = x.p.OrderServices,
                    CustomerId = x.p.CustomerId,
                    CustomerName = x.p.CustomerName,
                    CustomerUserName = x.p.CustomerUserName,
                    CustomerLevelId = x.p.CustomerLevelId,
                    CustomerLevelName = x.p.CustomerLevelName,
                    CustomerWarehouseId = x.p.CustomerWarehouseId,
                    CustomerWarehouseAddress = x.p.CustomerWarehouseAddress,
                    CustomerWarehouseName = x.p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = x.p.CustomerWarehouseIdPath,
                    TransportCode = x.p.TransportCode,
                    Weight = x.p.Weight,
                    WeightConverted = x.p.WeightConverted,
                    WeightActual = x.p.WeightActual,
                    WeightActualPercent = x.p.WeightActualPercent,
                    WeightWapperPercent = x.p.WeightWapperPercent,
                    OtherService = x.p.OtherService,
                    WeightWapper = x.p.WeightWapper,
                    TotalPriceWapper = x.p.TotalPriceWapper,
                    Volume = x.p.Volume,
                    VolumeActual = x.p.VolumeActual,
                    VolumeWapperPercent = x.p.VolumeWapperPercent,
                    VolumeWapper = x.p.VolumeWapper,
                    Size = x.p.Size,
                    Width = x.p.Width,
                    Height = x.p.Height,
                    Length = x.p.Length,
                    TotalPrice = x.p.TotalPrice,
                    WarehouseId = x.p.WarehouseId,
                    WarehouseName = x.p.WarehouseName,
                    WarehouseIdPath = x.p.WarehouseIdPath,
                    WarehouseAddress = x.p.WarehouseAddress,
                    UserId = x.p.UserId,
                    UserFullName = x.p.UserFullName,
                    SystemId = x.p.SystemId,
                    SystemName = x.p.SystemName,
                    Created = x.p.Created,
                    LastUpdate = x.p.LastUpdate,
                    HashTag = x.p.HashTag,
                    ForcastDate = x.p.ForcastDate,
                    PackageNo = x.p.PackageNo,
                    UnsignedText = x.p.UnsignedText,
                    CurrentLayoutId = x.p.CurrentLayoutId,
                    CurrentLayoutName = x.p.CurrentLayoutName,
                    CurrentLayoutIdPath = x.p.CurrentLayoutIdPath,
                    CurrentWarehouseId = x.p.CurrentWarehouseId,
                    CurrentWarehouseName = x.p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = x.p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = x.p.CurrentWarehouseAddress,
                    IsDelete = x.p.IsDelete,
                    OrderCodes = x.p.OrderCodes,
                    PackageCodes = x.p.PackageCodes,
                    Customers = x.p.Customers,
                    OrderCodesUnsigned = x.p.OrderCodesUnsigned,
                    PackageCodesUnsigned = x.p.PackageCodesUnsigned,
                    CustomersUnsigned = x.p.CustomersUnsigned,
                    OrderType = x.p.OrderType,
                    Mode = x.p.Mode,
                    SameCodeStatus = x.p.SameCodeStatus
                })
                .OrderBy(x => x.Id)
                .Take(size)
                .ToListAsync();
        }

        public Task<List<SuggetionOrderPackageResult>> Suggetion(string term, string packageCodes, int size, int? targetWarehouseId, bool isPaste)
        {
            if (isPaste)
                term = term.Trim();

            var walletDetails = Db.WalletDetails.Where(x => x.IsDelete == false)
                .Join(Db.Wallet.Where(x => x.Mode == 0 && x.IsDelete == false), wd => wd.WalletId, w => w.Id,
                    (wd, w) => wd);

            return Db.OrderPackages.Where(x => (x.Status == (byte) OrderPackageStatus.ChinaInStock
                                                || x.Status == (byte) OrderPackageStatus.LoseCode) && x.OrderId != 0 &&
                                               //(targetWarehouseId == null ||
                                               // x.CustomerWarehouseId == targetWarehouseId.Value) &&
                                               x.CurrentLayoutId != null && !x.IsDelete &&
                                               x.TransportCode != "" &&
                                               (isPaste && x.TransportCode == term 
                                               || isPaste == false && (x.Code.Contains(term) 
                                               || x.TransportCode.Contains(term) 
                                               || x.OrderCode.Contains(term))) && (packageCodes == "" || !packageCodes.Contains(";" + x.Code + ";")))
                .GroupJoin(walletDetails, op => op.Id, wd => wd.PackageId, (op, wd) => new {op, wd})
                .SelectMany(x => x.wd.DefaultIfEmpty(), (op, wd) => new { p = op.op, wd})
                .Where(x => x.wd == null)
                .Select(x => new SuggetionOrderPackageResult()
                {
                    Id = x.p.Id,
                    Code = x.p.Code,
                    Status = x.p.Status,
                    Note = x.p.Note,
                    OrderId = x.p.OrderId,
                    OrderCode = x.p.OrderCode,
                    OrderServices = x.p.OrderServices,
                    CustomerId = x.p.CustomerId,
                    CustomerName = x.p.CustomerName,
                    CustomerUserName = x.p.CustomerUserName,
                    CustomerLevelId = x.p.CustomerLevelId,
                    CustomerLevelName = x.p.CustomerLevelName,
                    CustomerWarehouseId = x.p.CustomerWarehouseId,
                    CustomerWarehouseAddress = x.p.CustomerWarehouseAddress,
                    CustomerWarehouseName = x.p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = x.p.CustomerWarehouseIdPath,
                    TransportCode = x.p.TransportCode,
                    Weight = x.p.Weight,
                    WeightConverted = x.p.WeightConverted,
                    WeightActual = x.p.WeightActual,
                    WeightActualPercent = x.p.WeightActualPercent,
                    WeightWapperPercent = x.p.WeightWapperPercent,
                    OtherService = x.p.OtherService,
                    WeightWapper = x.p.WeightWapper,
                    TotalPriceWapper = x.p.TotalPriceWapper,
                    Volume = x.p.Volume,
                    VolumeActual = x.p.VolumeActual,
                    VolumeWapperPercent = x.p.VolumeWapperPercent,
                    VolumeWapper = x.p.VolumeWapper,
                    Size = x.p.Size,
                    Width = x.p.Width,
                    Height = x.p.Height,
                    Length = x.p.Length,
                    TotalPrice = x.p.TotalPrice,
                    WarehouseId = x.p.WarehouseId,
                    WarehouseName = x.p.WarehouseName,
                    WarehouseIdPath = x.p.WarehouseIdPath,
                    WarehouseAddress = x.p.WarehouseAddress,
                    UserId = x.p.UserId,
                    UserFullName = x.p.UserFullName,
                    SystemId = x.p.SystemId,
                    SystemName = x.p.SystemName,
                    Created = x.p.Created,
                    LastUpdate = x.p.LastUpdate,
                    HashTag = x.p.HashTag,
                    ForcastDate = x.p.ForcastDate,
                    PackageNo = x.p.PackageNo,
                    UnsignedText = x.p.UnsignedText,
                    CurrentLayoutId = x.p.CurrentLayoutId,
                    CurrentLayoutName = x.p.CurrentLayoutName,
                    CurrentLayoutIdPath = x.p.CurrentLayoutIdPath,
                    CurrentWarehouseId = x.p.CurrentWarehouseId,
                    CurrentWarehouseName = x.p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = x.p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = x.p.CurrentWarehouseAddress,
                    IsDelete = x.p.IsDelete,
                    OrderCodes = x.p.OrderCodes,
                    PackageCodes = x.p.PackageCodes,
                    Customers = x.p.Customers,
                    OrderCodesUnsigned = x.p.OrderCodesUnsigned,
                    PackageCodesUnsigned = x.p.PackageCodesUnsigned,
                    CustomersUnsigned = x.p.CustomersUnsigned,
                    OrderType = x.p.OrderType,
                    Mode = x.p.Mode,
                    SameCodeStatus = x.p.SameCodeStatus
                })
                .OrderBy(x => x.Id)
                .Take(size)
                .ToListAsync();
        }

        public Task<List<PackageWitWalletResult>> SuggetionForTransfer(string term, string packageCodes, int size, int targetWarehouseId, bool isPaste)
        {
            if (isPaste)
                term = term.Trim();

            var walletDetails = Db.WalletDetails.Where(x => x.IsDelete == false)
                .Join(Db.Wallet.Where(x => x.Mode == 0 && x.IsDelete == false), wd => wd.WalletId, w => w.Id,
                    (wd, w) => wd);

            return Db.OrderPackages.Where(x => (x.Status == (byte)OrderPackageStatus.Received || x.Status == (byte) OrderPackageStatus.InStock) 
            && x.OrderId != 0 && x.CurrentWarehouseId == targetWarehouseId && !x.IsDelete &&
                                               x.TransportCode != "" && (isPaste && x.TransportCode == term ||
                                                (isPaste == false && (x.Code.Contains(term) || x.TransportCode.Contains(term) ||
                                                  x.OrderCode.Contains(term)))) &&
                                               (packageCodes == "" || !packageCodes.Contains(";" + x.Code + ";")))
                .Join(walletDetails, op => op.Id, wd => wd.PackageId, (p, wd) => new { p, wd })
                .OrderBy(x => x.p.Id)
                .Take(size)
                .Select(arg => new PackageWitWalletResult()
                {
                    Id = arg.p.Id,
                    Code = arg.p.Code,
                    Status = arg.p.Status,
                    Note = arg.p.Note,
                    OrderId = arg.p.OrderId,
                    OrderCode = arg.p.OrderCode,
                    OrderServices = arg.p.OrderServices,
                    CustomerId = arg.p.CustomerId,
                    CustomerName = arg.p.CustomerName,
                    CustomerUserName = arg.p.CustomerUserName,
                    CustomerLevelId = arg.p.CustomerLevelId,
                    CustomerLevelName = arg.p.CustomerLevelName,
                    CustomerWarehouseId = arg.p.CustomerWarehouseId,
                    CustomerWarehouseAddress = arg.p.CustomerWarehouseAddress,
                    CustomerWarehouseName = arg.p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = arg.p.CustomerWarehouseIdPath,
                    TransportCode = arg.p.TransportCode,
                    Weight = arg.p.Weight,
                    WeightConverted = arg.p.WeightConverted,
                    WeightActual = arg.p.WeightActual,
                    WeightActualPercent = arg.p.WeightActualPercent,
                    WeightWapperPercent = arg.p.WeightWapperPercent,
                    OtherService = arg.p.OtherService,
                    WeightWapper = arg.p.WeightWapper,
                    TotalPriceWapper = arg.p.TotalPriceWapper,
                    Volume = arg.p.Volume,
                    VolumeActual = arg.p.VolumeActual,
                    VolumeWapperPercent = arg.p.VolumeWapperPercent,
                    VolumeWapper = arg.p.VolumeWapper,
                    Size = arg.p.Size,
                    Width = arg.p.Width,
                    Height = arg.p.Height,
                    Length = arg.p.Length,
                    TotalPrice = arg.p.TotalPrice,
                    WarehouseId = arg.p.WarehouseId,
                    WarehouseName = arg.p.WarehouseName,
                    WarehouseIdPath = arg.p.WarehouseIdPath,
                    WarehouseAddress = arg.p.WarehouseAddress,
                    UserId = arg.p.UserId,
                    UserFullName = arg.p.UserFullName,
                    SystemId = arg.p.SystemId,
                    SystemName = arg.p.SystemName,
                    Created = arg.p.Created,
                    LastUpdate = arg.p.LastUpdate,
                    HashTag = arg.p.HashTag,
                    ForcastDate = arg.p.ForcastDate,
                    PackageNo = arg.p.PackageNo,
                    UnsignedText = arg.p.UnsignedText,
                    CurrentLayoutId = arg.p.CurrentLayoutId,
                    CurrentLayoutName = arg.p.CurrentLayoutName,
                    CurrentLayoutIdPath = arg.p.CurrentLayoutIdPath,
                    CurrentWarehouseId = arg.p.CurrentWarehouseId,
                    CurrentWarehouseName = arg.p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = arg.p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = arg.p.CurrentWarehouseAddress,
                    IsDelete = arg.p.IsDelete,
                    OrderCodes = arg.p.OrderCodes,
                    PackageCodes = arg.p.PackageCodes,
                    Customers = arg.p.Customers,
                    OrderCodesUnsigned = arg.p.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.p.PackageCodesUnsigned,
                    CustomersUnsigned = arg.p.CustomersUnsigned,
                    OrderType = arg.p.OrderType,
                    WalletId = arg.wd.WalletId,
                    WalletCode = arg.wd.WalletCode,
                })
                .ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageCodes"></param>
        /// <returns></returns>
        public Task<List<OrderPackageForPackingResult>> Suggetion2(string packageCodes)
        {
            return Db.OrderPackages.Where(x => (x.Status == (byte) OrderPackageStatus.ChinaInStock
                                                || x.Status == (byte) OrderPackageStatus.LoseCode) && x.OrderId != 0 &&
                                               x.Weight != null && !x.IsDelete &&
                                               x.TransportCode != "" && packageCodes.Contains(";" + x.Code + ";"))
                .OrderBy(x => x.Id)
                .Select(p => new OrderPackageForPackingResult
                {
                    Id = p.Id,
                    Code = p.Code,
                    Status = p.Status,
                    Note = p.Note,
                    OrderId = p.OrderId,
                    OrderCode = p.OrderCode,
                    OrderServices = p.OrderServices,
                    CustomerId = p.CustomerId,
                    CustomerName = p.CustomerName,
                    CustomerUserName = p.CustomerUserName,
                    CustomerLevelId = p.CustomerLevelId,
                    CustomerLevelName = p.CustomerLevelName,
                    CustomerWarehouseId = p.CustomerWarehouseId,
                    CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                    CustomerWarehouseName = p.CustomerWarehouseName,
                    CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                    TransportCode = p.TransportCode,
                    Weight = p.Weight,
                    WeightConverted = p.WeightConverted,
                    WeightActual = p.WeightActual,

                    WeightActualPercent = p.WeightActualPercent,
                    WeightWapperPercent = p.WeightWapperPercent,
                    OtherService = p.OtherService,
                    WeightWapper = p.WeightWapper,
                    TotalPriceWapper = p.TotalPriceWapper,
                    Volume = p.Volume,
                    VolumeActual = p.VolumeActual,
                    VolumeWapperPercent = p.VolumeWapperPercent,
                    VolumeWapper = p.VolumeWapper,
                    Size = p.Size,
                    Width = p.Width,
                    Height = p.Height,
                    Length = p.Length,
                    TotalPrice = p.TotalPrice,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.WarehouseName,
                    WarehouseIdPath = p.WarehouseIdPath,
                    WarehouseAddress = p.WarehouseAddress,
                    UserId = p.UserId,
                    UserFullName = p.UserFullName,
                    SystemId = p.SystemId,
                    SystemName = p.SystemName,
                    Created = p.Created,
                    LastUpdate = p.LastUpdate,
                    HashTag = p.HashTag,
                    ForcastDate = p.ForcastDate,
                    PackageNo = p.PackageNo,
                    UnsignedText = p.UnsignedText,
                    CurrentLayoutId = p.CurrentLayoutId,
                    CurrentLayoutName = p.CurrentLayoutName,
                    CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                    CurrentWarehouseId = p.CurrentWarehouseId,
                    CurrentWarehouseName = p.CurrentWarehouseName,
                    CurrentWarehouseIdPath = p.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = p.CurrentWarehouseAddress,
                    IsDelete = p.IsDelete,
                    OrderCodes = p.OrderCodes,
                    PackageCodes = p.PackageCodes,
                    Customers = p.Customers,
                    OrderCodesUnsigned = p.OrderCodesUnsigned,
                    PackageCodesUnsigned = p.PackageCodesUnsigned,
                    CustomersUnsigned = p.CustomersUnsigned,
                    OrderType = p.OrderType,
                })
                .ToListAsync();
        }

        public Task<List<SuggetionOrderPackageResult>> SuggetionForPutAway(string term, string packageCodes, int size, int currentWarehouseId, bool isPaste)
        {
            if (isPaste)
                term = term.Trim();

            return Db.OrderPackages.Where(
                    x => (x.Status == (byte) OrderPackageStatus.ChinaReceived ||
                         x.Status == (byte) OrderPackageStatus.Received || x.Status == (byte)OrderPackageStatus.Lost) && x.CurrentWarehouseId == currentWarehouseId &&
                        x.CurrentLayoutId == null && !x.IsDelete && x.OrderId != 0 &&
                        x.TransportCode != "" && (isPaste && x.TransportCode == term ||
                         isPaste == false && (x.Code.Contains(term) || x.TransportCode.Contains(term) || x.OrderCode.Contains(term))) &&
                        (packageCodes == "" || !packageCodes.Contains(";" + x.Code + ";")))
                .Select(x=> new SuggetionOrderPackageResult()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Status = x.Status,
                    Note = x.Note,
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    OrderServices = x.OrderServices,
                    CustomerId = x.CustomerId,
                    CustomerName = x.CustomerName,
                    CustomerUserName = x.CustomerUserName,
                    CustomerLevelId = x.CustomerLevelId,
                    CustomerLevelName = x.CustomerLevelName,
                    CustomerWarehouseId = x.CustomerWarehouseId,
                    CustomerWarehouseAddress = x.CustomerWarehouseAddress,
                    CustomerWarehouseName = x.CustomerWarehouseName,
                    CustomerWarehouseIdPath = x.CustomerWarehouseIdPath,
                    TransportCode = x.TransportCode,
                    Weight = x.Weight,
                    WeightConverted = x.WeightConverted,
                    WeightActual = x.WeightActual,
                    WeightActualPercent = x.WeightActualPercent,
                    WeightWapperPercent = x.WeightWapperPercent,
                    OtherService = x.OtherService,
                    WeightWapper = x.WeightWapper,
                    TotalPriceWapper = x.TotalPriceWapper,
                    Volume = x.Volume,
                    VolumeActual = x.VolumeActual,
                    VolumeWapperPercent = x.VolumeWapperPercent,
                    VolumeWapper = x.VolumeWapper,
                    Size = x.Size,
                    Width = x.Width,
                    Height = x.Height,
                    Length = x.Length,
                    TotalPrice = x.TotalPrice,
                    WarehouseId = x.WarehouseId,
                    WarehouseName = x.WarehouseName,
                    WarehouseIdPath = x.WarehouseIdPath,
                    WarehouseAddress = x.WarehouseAddress,
                    UserId = x.UserId,
                    UserFullName = x.UserFullName,
                    SystemId = x.SystemId,
                    SystemName = x.SystemName,
                    Created = x.Created,
                    LastUpdate = x.LastUpdate,
                    HashTag = x.HashTag,
                    ForcastDate = x.ForcastDate,
                    PackageNo = x.PackageNo,
                    UnsignedText = x.UnsignedText,
                    CurrentLayoutId = x.CurrentLayoutId,
                    CurrentLayoutName = x.CurrentLayoutName,
                    CurrentLayoutIdPath = x.CurrentLayoutIdPath,
                    CurrentWarehouseId = x.CurrentWarehouseId,
                    CurrentWarehouseName = x.CurrentWarehouseName,
                    CurrentWarehouseIdPath = x.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = x.CurrentWarehouseAddress,
                    IsDelete = x.IsDelete,
                    OrderCodes = x.OrderCodes,
                    PackageCodes = x.PackageCodes,
                    Customers = x.Customers,
                    OrderCodesUnsigned = x.OrderCodesUnsigned,
                    PackageCodesUnsigned = x.PackageCodesUnsigned,
                    CustomersUnsigned = x.CustomersUnsigned,
                    OrderType = x.OrderType,
                    Mode = x.Mode,
                    SameCodeStatus = x.SameCodeStatus
                })
                .OrderBy(x => x.Id)
                .Take(size)
                .ToListAsync();
        }

        public Task<List<OrderPackage>> SuggetionForPutAway(int walletId, int currentWarehouseId)
        {
            return Db.WalletDetails.Where(x => x.WalletId == walletId)
                .Join(Db.OrderPackages.Where(
                    x => (x.Status == (byte) OrderPackageStatus.ChinaReceived ||
                         x.Status == (byte) OrderPackageStatus.Received)
                        && x.CurrentWarehouseId == currentWarehouseId &&
                        x.CurrentLayoutId == null && !x.IsDelete && x.OrderId != 0 &&
                        x.TransportCode != ""), d => d.PackageId, p => p.Id, (d, p) => p)
                .OrderBy(x => x.Id)
                .ToListAsync();
        }

        public Task<List<OrderPackage>> GetForDelivery(string codeOrder)
        {
            return Db.OrderPackages.Where(x => x.Status == (byte)OrderPackageStatus.InStock && !x.IsDelete && x.OrderCode == codeOrder)
                .GroupJoin(Db.DeliveryDetails.Where(x => !x.IsDelete), p => p.Id, d => d.PackageId,
                    (orderPackage, deliveryDetail) => new { orderPackage, deliveryDetail })
                .Where(x => !x.deliveryDetail.Any())
                .Select(x => x.orderPackage)
                .ToListAsync();
        }

        public List<string> GetWarehouseInWallet(string orderCodes)
        {
            return Db.Orders.Where(x => orderCodes.Contains(";" + x.Code + ";"))
                .Join(Db.Customers, order => order.CustomerId, customer => customer.Id,
                    (order, customer) => customer.WarehouseName).Distinct().ToList();
        }

        public Task<Dictionary<string, decimal>> SumWeightByOrderCodes(string ordersCode)
        {
            return Db.OrderPackages.Where(
                    x => ordersCode.Contains(";" + x.OrderCode + ";") && x.IsDelete == false && x.WeightActual != null)
                .GroupBy(x => x.OrderCode).Select(g => new
                {
                    OrderCode = g.Key,
                    ActualWeight = g.Sum(x => x.WeightActual ?? 0)
                }).ToDictionaryAsync(k => k.OrderCode, i => i.ActualWeight);
        }

        public Task<List<OrderPackage>> OrderPackageByWalletId(int walletId)
        {
            return Db.OrderPackages.Where(x => !x.IsDelete && Db.WalletDetails.Any(w => w.PackageId == x.Id && w.WalletId == walletId)).ToListAsync();
        }

        public Task<List<OrderPackage>> OrderPackageByOrderCode(string orderCode)
        {
            return Db.OrderPackages.Where(x => x.Status == (byte) OrderPackageStatus.ChinaInStock
                                               && !x.IsDelete && x.OrderCode == orderCode &&
                                               !Db.ExportWarehouseDetails.Any(w => w.PackageId == x.Id)).ToListAsync();
        }

        public Task<List<PackingResult>> SearchPacking(out int totalRecord, out IQueryable<PackingResult> queryOut, Expression<Func<OrderPackage, bool>> predicate1,
             Expression<Func<PackingResult, bool>> predicate2, int currentPage, int recordPerPage)
        {
            var walletDetail = Db.WalletDetails.Where(x => x.IsDelete == false)
                .Join(Db.Wallet.Where(
                    x =>
                        x.Mode == 1 && x.Status != (byte) WalletStatus.Complete &&
                        x.Status != (byte) WalletStatus.Lose), wd => wd.WalletId, w => w.Id, (wd, w) => new
                {
                    w.Id,
                    w.Weight,
                    w.WeightConverted,
                    w.Code,
                    w.Size,
                    w.Note,
                    w.Height,
                    w.Length,
                    w.WeightActual,
                    w.Created,
                    w.UserFullName,
                    w.UserId,
                    w.UserName,
                    w.Width,
                    wd.PackageId
                });

            var query = Db.OrderPackages.Where(predicate1)
                .Join(Db.OrderServices.Where(
                        x => x.IsDelete == false && x.ServiceId == (byte) OrderServices.Packing && x.Checked),
                    p => p.OrderId, os => os.OrderId, (p, os) => new {p, os})
                .GroupJoin(walletDetail, arg => arg.p.Id, wd => wd.PackageId, (arg, wd) => new {arg.p, arg.os, wd})
                .SelectMany(x => x.wd.DefaultIfEmpty(), (arg, w) => new PackingResult
                {
                    Created = arg.p.Created,
                    WalletId = w.Id,
                    Status = arg.p.Status,
                    Code = arg.p.Code,
                    Id = arg.p.Id,
                    Size = arg.p.Size,
                    WalletWeight = w.Weight,
                    OrderServices = arg.p.OrderServices,
                    WeightActual = arg.p.WeightActual,
                    Volume = arg.p.Volume,
                    WarehouseName = arg.p.WarehouseName,
                    OrderId = arg.p.OrderId,
                    Weight = arg.p.Weight,
                    CurrentWarehouseId = arg.p.CurrentWarehouseId,
                    OrderCodes = arg.p.OrderCodes,
                    WalletWeightConverted = w.WeightConverted,
                    WeightConverted = arg.p.WeightConverted,
                    WalletCode = w.Code,
                    WalletSize = w.Size,
                    WalletNote = w.Note,
                    PackageNo = arg.p.PackageNo,
                    CurrentLayoutId = arg.p.CurrentLayoutId,
                    CurrentLayoutIdPath = arg.p.CurrentLayoutIdPath,
                    CurrentLayoutName = arg.p.CurrentLayoutName,
                    CurrentWarehouseAddress = arg.p.CurrentWarehouseAddress,
                    CurrentWarehouseIdPath = arg.p.CurrentWarehouseIdPath,
                    CurrentWarehouseName = arg.p.CurrentWarehouseName,
                    CustomerId = arg.p.CustomerId,
                    CustomerLevelId = arg.p.CustomerLevelId,
                    CustomerLevelName = arg.p.CustomerLevelName,
                    CustomerName = arg.p.CustomerName,
                    CustomerUserName = arg.p.CustomerUserName,
                    CustomerWarehouseAddress = arg.p.CustomerWarehouseAddress,
                    CustomerWarehouseId = arg.p.CustomerWarehouseId,
                    CustomerWarehouseIdPath = arg.p.CustomerWarehouseIdPath,
                    CustomerWarehouseName = arg.p.CustomerWarehouseName,
                    Customers = arg.p.Customers,
                    CustomersUnsigned = arg.p.CustomersUnsigned,
                    ForcastDate = arg.p.ForcastDate,
                    HashTag = arg.p.HashTag,
                    Height = arg.p.Height,
                    LastUpdate = arg.p.LastUpdate,
                    Length = arg.p.Length,
                    Note = arg.p.Note,
                    OrderCode = arg.p.OrderCode,
                    OrderCodesUnsigned = arg.p.OrderCodesUnsigned,
                    OrderType = arg.p.OrderType,
                    PackageCodes = arg.p.PackageCodes,
                    PackageCodesUnsigned = arg.p.PackageCodesUnsigned,
                    SystemId = arg.p.SystemId,
                    SystemName = arg.p.SystemName,
                    TotalPrice = arg.p.TotalPrice,
                    TotalPriceWapper = arg.p.TotalPriceWapper,
                    TransportCode = arg.p.TransportCode,
                    UnsignedText = arg.p.UnsignedText,
                    UserFullName = arg.p.UserFullName,
                    UserId = arg.p.UserId,
                    VolumeActual = arg.p.VolumeActual,
                    VolumeWapper = arg.p.VolumeWapper,
                    VolumeWapperPercent = arg.p.VolumeWapperPercent,
                    WalletHeight = w.Height,
                    WalletLength = w.Length,
                    WalletWeightActual = w.WeightActual,
                    WalletCreated = w.Created,
                    WalletUserFullName = w.UserFullName,
                    WalletUserId = w.UserId,
                    WalletUserName = w.UserName,
                    WalletWidth = w.Width,
                    WarehouseAddress = arg.p.WarehouseAddress,
                    WarehouseId = arg.p.WarehouseId,
                    WarehouseIdPath = arg.p.WarehouseIdPath,
                    WeightWapper = arg.p.WeightWapper,
                    WeightWapperPercent = arg.p.WeightWapperPercent,
                    Width = arg.p.Width,
                });

            queryOut = query;

            query = query.Where(predicate2).Distinct();

            totalRecord = query.Count();

            return query.OrderBy(x => new { x.CustomerId, x.OrderId })
                    .Skip((currentPage - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToListAsync();
        }

        public List<OrderPackageItem> GetOrderPackageForDetail(int orderId, byte orderType)
        {
            int orderService = (int)OrderServices.OutSideShipping;
            var listObj = Db.OrderPackages.Where(x => x.OrderId == orderId && !x.IsDelete && x.OrderType == orderType)
                                        .Select(m => new OrderPackageItem()
                                        {
                                            Id = m.Id,
                                            Code = m.Code,
                                            Weight = m.Weight,
                                            WeightActual = m.WeightActual,
                                            TransportCode = m.TransportCode,
                                            CurrentWarehouseName = m.CurrentWarehouseName,
                                            Status = m.Status,
                                            Length = m.Length,
                                            Height = m.Height,
                                            Width = m.Width,
                                            ActualMoney = (((decimal)(Db.OrderServices.FirstOrDefault(z => z.ServiceId == orderService & z.OrderId == orderId).TotalPrice * m.WeightActualPercent)) / 100),
                                            IsGross = (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id) == null ? 0 : Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id).WalletId))) == null ? 1 : (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id) == null ? 0 : Db.WalletDetails.FirstOrDefault(z => z.PackageId == m.Id).WalletId))).PackageNo))
                                        }).ToList();
            
            return listObj;
        }

        public int CountPackageWallet(int packageId)
        {
            return (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails
            .FirstOrDefault(z => z.PackageId == packageId) == null ? 0 : 
            Db.WalletDetails.FirstOrDefault(z => z.PackageId == packageId).WalletId))) == null ? 1 : 
            (Db.Wallet.FirstOrDefault(y => y.Id == ((Db.WalletDetails.FirstOrDefault(z => z.PackageId == packageId) == null ? 0 : Db.WalletDetails.FirstOrDefault(z => z.PackageId == packageId).WalletId))).PackageNo));
        }

        public OrderPackageModel GetAllOrderPackageByLinq(PageItem pageInfor, SearchInfor searchInfor)
        {
            int orderService = (int)OrderServices.OutSideShipping;
            var query = Db.OrderPackages.Where(
                    x => (searchInfor.Status == -1 || (x.Status >= searchInfor.StartStatus && x.Status <= searchInfor.EndStatus))
                    && x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new OrderPackageListItem()
                {
                    ROW = 1,
                    Code = m.Code,
                    TransportCode = m.TransportCode,
                    Status = m.Status,
                    OrderId = m.OrderId,
                    OrderCode = m.OrderCode,
                    WeightActual = m.WeightActual,
                    Weight = m.Weight,
                    Size = m.Size,
                    Width = m.Width,
                    Height = m.Height,
                    Length = m.Length,
                    CurrentWarehouseName = m.CurrentWarehouseName,
                    Created = m.Created,
                    OrderType = m.OrderType,
                    ActualMoney = (((decimal)(Db.OrderServices.FirstOrDefault(z => z.ServiceId == orderService & z.OrderId == m.OrderId).TotalPrice * m.WeightActualPercent)) / 100),
                    IsGross = 1,
                    Id = m.Id
                });

            var queryCount = Db.OrderPackages.Where(
                    x => x.Code.Contains(searchInfor.Keyword)
                    && (searchInfor.AllTime == -1 || ((searchInfor.StartDate == null || x.Created >= searchInfor.StartDate)
                    && (searchInfor.FinishDate == null || x.Created <= searchInfor.FinishDate)))
                    && !x.IsDelete
                    && x.CustomerId == searchInfor.CustomerId
                    && x.SystemId == searchInfor.SystemId
                ).Select(m => new OrderPackageListItem()
                {
                    Status = m.Status,
                });
            pageInfor.CurrentPage = pageInfor.PageIndex;
            pageInfor.Total = query.Count();
            var tmpList = query.OrderByDescending(x => new { x.Created })
                    .Skip((pageInfor.CurrentPage - 1) * pageInfor.PageSize)
                    .Take(pageInfor.PageSize)
                    .ToList();
            var tmpStatus = new OrderPackageStatusItem()
            {
                shopTqPhatHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus1 && m.Status <= searchInfor.EndStatus1).Count(),
                khoTqNhanHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus2 && m.Status <= searchInfor.EndStatus2).Count(),
                trongKhoTq = queryCount.Where(m => m.Status >= searchInfor.StartStatus3 && m.Status <= searchInfor.EndStatus3).Count(),
                xuatKhoTq = queryCount.Where(m => m.Status >= searchInfor.StartStatus4 && m.Status <= searchInfor.EndStatus4).Count(),
                trenDuongVe = queryCount.Where(m => m.Status >= searchInfor.StartStatus5 && m.Status <= searchInfor.EndStatus5).Count(),
                trongKhoVn = queryCount.Where(m => m.Status >= searchInfor.StartStatus6 && m.Status <= searchInfor.StartStatus6).Count(),
                dangVanChuyen = queryCount.Where(m => m.Status >= searchInfor.StartStatus7 && m.Status <= searchInfor.StartStatus7).Count(),
                choGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus8 && m.Status <= searchInfor.StartStatus8).Count(),
                dangGiaoHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus9 && m.Status <= searchInfor.StartStatus9).Count(),
                daTraHang = queryCount.Where(m => m.Status >= searchInfor.StartStatus10 && m.Status <= searchInfor.StartStatus10).Count(),
                hangBiMat = queryCount.Where(m => m.Status >= searchInfor.StartStatus11 && m.Status <= searchInfor.StartStatus11).Count(),
            };
            var model = new OrderPackageModel()
            {
                Page = pageInfor,
                Search = searchInfor,
                ListItems = tmpList,
                OrderStatusItem = tmpStatus
            };
            return model;
        }

        public OrderPackageModel GetAllOrderPackage(PageItem pageInfor, SearchInfor searchInfor)
        {
            using (var context = new ProjectXContext())
            {
                var cmd = context.Database.Connection.CreateCommand();
                cmd.CommandText = "customer_orderPackage_select";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("PageIndex", pageInfor.PageIndex == 0 ? 1 : pageInfor.PageIndex));
                cmd.Parameters.Add(new SqlParameter("PageSize", pageInfor.PageIndex == 0 ? 25 : pageInfor.PageSize));
                cmd.Parameters.Add(new SqlParameter("Keyword", searchInfor.Keyword));
                cmd.Parameters.Add(new SqlParameter("StartDate", searchInfor.StartDate));
                cmd.Parameters.Add(new SqlParameter("FinishDate", searchInfor.FinishDate));
                cmd.Parameters.Add(new SqlParameter("AllTime", searchInfor.AllTime));
                cmd.Parameters.Add(new SqlParameter("Status", searchInfor.Status));
                cmd.Parameters.Add(new SqlParameter("SystemId", searchInfor.SystemId));
                cmd.Parameters.Add(new SqlParameter("CustomerId", searchInfor.CustomerId));
                cmd.Parameters.Add(new SqlParameter("IsSearch", searchInfor.IsSearch));
                SqlParameter outputCount = new SqlParameter("@Count", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputCount);
                try
                {
                    if (context.Database.Connection.State.Equals(ConnectionState.Closed))
                    {
                        context.Database.Connection.Open();
                    }
                    var reader = cmd.ExecuteReader();
                    var tmpList = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderPackageListItem>(reader).ToList();
                    reader.NextResult();
                    var tmpStatus = ((IObjectContextAdapter)context).ObjectContext.Translate<OrderPackageStatusItem>(reader).ToList().FirstOrDefault();
                    reader.Close();
                    pageInfor.CurrentPage = pageInfor.PageIndex;
                    pageInfor.Total = int.Parse(cmd.Parameters["@Count"].Value == null ? "0" : cmd.Parameters["@Count"].Value.ToString());

                    var model = new OrderPackageModel()
                    {
                        Page = pageInfor,
                        Search = searchInfor,
                        ListItems = tmpList,
                        OrderStatusItem = tmpStatus
                    };
                    return model;
                }
                finally
                {
                    context.Database.Connection.Close();
                }
            }

        }

        public List<PackageForTrackingResult> ExcelOrderPackageForTracking(string keyword, byte? searchType, string statusText, 
            string warehouseIdText, string orderStatusText, string statusDepositText, string orderTypeText, string orderServiceText, 
            DateTime? fromDate, DateTime? toDate, string timeTypeText)
        {
            var query = Db.OrderPackages.Where(
                x => x.IsDelete == false && (searchType == null && x.UnsignedText.Contains(keyword)
                    || searchType == 0 && x.TransportCode == keyword
                    || searchType == 1 && x.OrderCode == keyword
                    || searchType == 2 && x.Code == keyword
                    || searchType == 3 && x.CustomerUserName == keyword) &&
                         (statusText == "" || statusText.Contains(";" + x.Status + ";")) &&
                         (warehouseIdText == "" || warehouseIdText.Contains(";" + x.CustomerWarehouseId + ";")) &&
                         (orderTypeText == "" || orderTypeText.Contains(";" + x.OrderType + ";")))
                 .Join(Db.OrderServices.Where(x => x.IsDelete == false && x.Checked && (orderServiceText == "" || orderServiceText.Contains(";" + x.ServiceId + ";"))), p => p.OrderId, s => s.OrderId, (p, s) => p)
                 .Join(Db.PackageHistories.Where(x => timeTypeText == ""
                 || timeTypeText.Contains(";" + x.Status + ";") &&
                    (fromDate == null && toDate == null
                     || fromDate != null && toDate != null && x.CreateDate >= fromDate && x.CreateDate <= toDate
                     || fromDate == null && toDate.HasValue && x.CreateDate <= toDate
                     || toDate == null && fromDate.HasValue && x.CreateDate >= fromDate)), x => x.Id, x => x.PackageId, (package, history) => package)
                .Join(Db.Orders.Where(
                        x =>
                            x.IsDelete == false &&
                            (x.Type != (byte)OrderType.Deposit && (statusText == "" || statusText.Contains(";" + x.Status + ";"))
                    || x.Type == (byte)OrderType.Deposit && (statusDepositText == "" || statusDepositText.Contains(";" + x.Status + ";")))),
                    p => p.OrderId, o => o.Id, (p, o) => new PackageForTrackingResult()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Status = p.Status,
                        Note = p.Note,
                        OrderId = p.OrderId,
                        OrderCode = p.OrderCode,
                        OrderServices = p.OrderServices,
                        OrderType = p.OrderType,
                        CustomerId = p.CustomerId,
                        CustomerName = p.CustomerName,
                        CustomerPhone = o.CustomerPhone,
                        CustomerUserName = p.CustomerUserName,
                        CustomerLevelId = p.CustomerLevelId,
                        CustomerLevelName = p.CustomerLevelName,
                        CustomerWarehouseId = p.CustomerWarehouseId,
                        CustomerWarehouseAddress = p.CustomerWarehouseAddress,
                        CustomerWarehouseName = p.CustomerWarehouseName,
                        CustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                        TransportCode = p.TransportCode,
                        Weight = p.Weight,
                        WeightConverted = p.WeightConverted,
                        WeightActual = p.WeightActual,
                        WeightActualPercent = p.WeightActualPercent,
                        WeightWapperPercent = p.WeightWapperPercent,
                        OtherService = p.OtherService,
                        WeightWapper = p.WeightWapper,
                        TotalPriceWapper = p.TotalPriceWapper,
                        Volume = p.Volume,
                        VolumeActual = p.VolumeActual,
                        VolumeWapperPercent = p.VolumeWapperPercent,
                        VolumeWapper = p.VolumeWapper,
                        Size = p.Size,
                        Width = p.Width,
                        Height = p.Height,
                        Length = p.Length,
                        TotalPrice = p.TotalPrice,
                        WarehouseId = p.WarehouseId,
                        WarehouseName = p.WarehouseName,
                        WarehouseIdPath = p.WarehouseIdPath,
                        WarehouseAddress = p.WarehouseAddress,
                        UserId = p.UserId,
                        UserFullName = p.UserFullName,
                        SystemId = p.SystemId,
                        SystemName = p.SystemName,
                        Created = p.Created,
                        LastUpdate = p.LastUpdate,
                        HashTag = p.HashTag,
                        ForcastDate = p.ForcastDate,
                        PackageNo = p.PackageNo,
                        UnsignedText = p.UnsignedText,
                        CurrentLayoutId = p.CurrentLayoutId,
                        CurrentLayoutName = p.CurrentLayoutName,
                        CurrentLayoutIdPath = p.CurrentLayoutIdPath,
                        CurrentWarehouseId = p.CurrentWarehouseId,
                        CurrentWarehouseName = p.CurrentWarehouseName,
                        CurrentWarehouseIdPath = p.CurrentWarehouseIdPath,
                        CurrentWarehouseAddress = p.CurrentWarehouseAddress,
                        IsDelete = p.IsDelete,
                        OrderCodes = p.OrderCodes,
                        PackageCodes = p.PackageCodes,
                        Customers = p.Customers,
                        OrderCodesUnsigned = p.OrderCodesUnsigned,
                        PackageCodesUnsigned = p.PackageCodesUnsigned,
                        CustomersUnsigned = p.CustomersUnsigned,
                    }).Distinct();

            return query.ToList();
        }
    }
}
