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
    public class WalletRepository : Repository<Wallet>
    {
        public WalletRepository(ProjectXContext context) : base(context)
        {

        }


        public bool HasPackageGoDelivery(int walletId)
        {
            var query = Db.Wallet.Where(x => x.IsDelete == false)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false), w => w.Id, d => d.WalletId,
                    (w, d) => new {d.PackageId})
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false), arg => arg.PackageId, p => p.Id,
                    (arg, p) => new {arg.PackageId, p.Status});

            return query.Any() && !query.Any(x => x.Status == (byte) OrderPackageStatus.GoingDelivery ||
                           x.Status == (byte) OrderPackageStatus.Completed);
        }

        public Task<List<WalletResult>> SearchAsync(Expression<Func<Wallet, bool>> exp1, Expression<Func<WalletResult, bool>> exp2, 
            int pageIndex, int recordPerPage, out long totalRecord)
        {
            var query = Db.Wallet.Where(exp1)
                .GroupJoin(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 1), wallet => wallet.Id,
                    detail => detail.PackageId, (wallet, details) => new {wallet, details})
                .SelectMany(arg => arg.details.DefaultIfEmpty(), (arg, detail) => new WalletResult()
                {
                    Id = arg.wallet.Id,
                    Code = arg.wallet.Code,
                    Status = arg.wallet.Status,
                    Width = arg.wallet.Width,
                    Length = arg.wallet.Length,
                    Height = arg.wallet.Height,
                    Size = arg.wallet.Size,
                    TotalWeight = arg.wallet.TotalWeight,
                    TotalWeightConverted = arg.wallet.TotalWeightConverted,
                    TotalWeightActual = arg.wallet.TotalWeightActual,
                    TotalVolume = arg.wallet.TotalVolume,
                    Weight = arg.wallet.Weight,
                    WeightConverted = arg.wallet.WeightConverted,
                    WeightActual = arg.wallet.WeightActual,
                    Volume = arg.wallet.Volume,
                    TotalValue = arg.wallet.TotalValue,
                    PackageNo = arg.wallet.PackageNo,
                    CreatedWarehouseId = arg.wallet.CreatedWarehouseId,
                    CreatedWarehouseIdPath = arg.wallet.CreatedWarehouseIdPath,
                    CreatedWarehouseName = arg.wallet.CreatedWarehouseName,
                    CreatedWarehouseAddress = arg.wallet.CreatedWarehouseAddress,
                    CurrentWarehouseId = arg.wallet.CurrentWarehouseId,
                    CurrentWarehouseIdPath = arg.wallet.CurrentWarehouseIdPath,
                    CurrentWarehouseName = arg.wallet.CurrentWarehouseName,
                    CurrentWarehouseAddress = arg.wallet.CurrentWarehouseAddress,
                    TargetWarehouseId = arg.wallet.TargetWarehouseId,
                    TargetWarehouseIdPath = arg.wallet.TargetWarehouseIdPath,
                    TargetWarehouseName = arg.wallet.TargetWarehouseName,
                    TargetWarehouseAddress = arg.wallet.TargetWarehouseAddress,
                    UserId = arg.wallet.UserId,
                    UserName = arg.wallet.UserName,
                    UserFullName = arg.wallet.UserFullName,
                    Created = arg.wallet.Created,
                    Updated = arg.wallet.Updated,
                    UnsignedText = arg.wallet.UnsignedText,
                    Note = arg.wallet.Note,
                    IsDelete = arg.wallet.IsDelete,
                    OrderCodes = arg.wallet.OrderCodes,
                    PackageCodes = arg.wallet.PackageCodes,
                    Customers = arg.wallet.Customers,
                    OrderCodesUnsigned = arg.wallet.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.wallet.PackageCodesUnsigned,
                    CustomersUnsigned = arg.wallet.CustomersUnsigned,
                    Mode = arg.wallet.Mode,
                    PartnerId = arg.wallet.PartnerId,
                    PartnerName = arg.wallet.PartnerName,
                    PartnerUpdate = arg.wallet.PartnerUpdate,
                    EntrepotId = arg.wallet.EntrepotId,
                    EntrepotName = arg.wallet.EntrepotName,
                    OrderServices = arg.wallet.OrderServices,
                    OrderServicesJson = arg.wallet.OrderServicesJson,
                    ImportedTime = detail.Created,
                }).Where(exp2);

            totalRecord = query.Count();

            return query.OrderBy(x => x.Id)
                    .Skip((pageIndex - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="toDate"></param>
        /// <param name="mode">0: Tạo bao, 1 Nhập bao</param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public Task<Dictionary<int, int>> WalletReportAsync(DateTime? fromDate, DateTime? toDate, byte mode, int warehouseId)
        {
            var query = Db.Wallet.Where(x=> !x.IsDelete  && (x.CreatedWarehouseId == warehouseId || x.TargetWarehouseId == warehouseId))
                .GroupJoin(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 1), wallet => wallet.Id,
                    detail => detail.PackageId, (wallet, details) => new { wallet, details })
                .SelectMany(arg => arg.details.DefaultIfEmpty(), (arg, detail) => new WalletResult()
                {
                    Id = arg.wallet.Id,
                    Code = arg.wallet.Code,
                    Status = arg.wallet.Status,
                    Width = arg.wallet.Width,
                    Length = arg.wallet.Length,
                    Height = arg.wallet.Height,
                    Size = arg.wallet.Size,
                    TotalWeight = arg.wallet.TotalWeight,
                    TotalWeightConverted = arg.wallet.TotalWeightConverted,
                    TotalWeightActual = arg.wallet.TotalWeightActual,
                    TotalVolume = arg.wallet.TotalVolume,
                    Weight = arg.wallet.Weight,
                    WeightConverted = arg.wallet.WeightConverted,
                    WeightActual = arg.wallet.WeightActual,
                    Volume = arg.wallet.Volume,
                    TotalValue = arg.wallet.TotalValue,
                    PackageNo = arg.wallet.PackageNo,
                    CreatedWarehouseId = arg.wallet.CreatedWarehouseId,
                    CreatedWarehouseIdPath = arg.wallet.CreatedWarehouseIdPath,
                    CreatedWarehouseName = arg.wallet.CreatedWarehouseName,
                    CreatedWarehouseAddress = arg.wallet.CreatedWarehouseAddress,
                    CurrentWarehouseId = arg.wallet.CurrentWarehouseId,
                    CurrentWarehouseIdPath = arg.wallet.CurrentWarehouseIdPath,
                    CurrentWarehouseName = arg.wallet.CurrentWarehouseName,
                    CurrentWarehouseAddress = arg.wallet.CurrentWarehouseAddress,
                    TargetWarehouseId = arg.wallet.TargetWarehouseId,
                    TargetWarehouseIdPath = arg.wallet.TargetWarehouseIdPath,
                    TargetWarehouseName = arg.wallet.TargetWarehouseName,
                    TargetWarehouseAddress = arg.wallet.TargetWarehouseAddress,
                    UserId = arg.wallet.UserId,
                    UserName = arg.wallet.UserName,
                    UserFullName = arg.wallet.UserFullName,
                    Created = arg.wallet.Created,
                    Updated = arg.wallet.Updated,
                    UnsignedText = arg.wallet.UnsignedText,
                    Note = arg.wallet.Note,
                    IsDelete = arg.wallet.IsDelete,
                    OrderCodes = arg.wallet.OrderCodes,
                    PackageCodes = arg.wallet.PackageCodes,
                    Customers = arg.wallet.Customers,
                    OrderCodesUnsigned = arg.wallet.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.wallet.PackageCodesUnsigned,
                    CustomersUnsigned = arg.wallet.CustomersUnsigned,
                    Mode = arg.wallet.Mode,
                    PartnerId = arg.wallet.PartnerId,
                    PartnerName = arg.wallet.PartnerName,
                    PartnerUpdate = arg.wallet.PartnerUpdate,
                    EntrepotId = arg.wallet.EntrepotId,
                    EntrepotName = arg.wallet.EntrepotName,
                    OrderServices = arg.wallet.OrderServices,
                    OrderServicesJson = arg.wallet.OrderServicesJson,
                    ImportedTime = detail.Created,
                }).Where(x=> mode == 0 && (
                fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate
                || fromDate == null && toDate.HasValue && x.Created <= toDate
                || toDate == null && fromDate.HasValue && x.Created >= fromDate)
                || mode == 1 &&
                (fromDate == null && toDate == null
                || fromDate != null && toDate != null && x.ImportedTime >= fromDate && x.ImportedTime <= toDate
                || fromDate == null && toDate.HasValue && x.ImportedTime <= toDate
                || toDate == null && fromDate.HasValue && x.ImportedTime >= fromDate));

            if (mode == 0)
                return query.GroupBy(x => x.Created.Day)
                    .ToDictionaryAsync(x => x.Key, x => x.Count());

            return query.GroupBy(x => x.ImportedTime.Value.Day)
                    .ToDictionaryAsync(x => x.Key, x => x.Count());
        }

        public Task<List<WalletResult>> SearchForExportAsync(Expression<Func<Wallet, bool>> exp1, Expression<Func<WalletResult, bool>> exp2)
        {
            return Db.Wallet.Where(exp1)
                .GroupJoin(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 1), wallet => wallet.Id,
                    detail => detail.PackageId, (wallet, details) => new {wallet, details})
                .SelectMany(arg => arg.details.DefaultIfEmpty(), (arg, detail) => new WalletResult()
                {
                    Id = arg.wallet.Id,
                    Code = arg.wallet.Code,
                    Status = arg.wallet.Status,
                    Width = arg.wallet.Width,
                    Length = arg.wallet.Length,
                    Height = arg.wallet.Height,
                    Size = arg.wallet.Size,
                    TotalWeight = arg.wallet.TotalWeight,
                    TotalWeightConverted = arg.wallet.TotalWeightConverted,
                    TotalWeightActual = arg.wallet.TotalWeightActual,
                    TotalVolume = arg.wallet.TotalVolume,
                    Weight = arg.wallet.Weight,
                    WeightConverted = arg.wallet.WeightConverted,
                    WeightActual = arg.wallet.WeightActual,
                    Volume = arg.wallet.Volume,
                    TotalValue = arg.wallet.TotalValue,
                    PackageNo = arg.wallet.PackageNo,
                    CreatedWarehouseId = arg.wallet.CreatedWarehouseId,
                    CreatedWarehouseIdPath = arg.wallet.CreatedWarehouseIdPath,
                    CreatedWarehouseName = arg.wallet.CreatedWarehouseName,
                    CreatedWarehouseAddress = arg.wallet.CreatedWarehouseAddress,
                    CurrentWarehouseId = arg.wallet.CurrentWarehouseId,
                    CurrentWarehouseIdPath = arg.wallet.CurrentWarehouseIdPath,
                    CurrentWarehouseName = arg.wallet.CurrentWarehouseName,
                    CurrentWarehouseAddress = arg.wallet.CurrentWarehouseAddress,
                    TargetWarehouseId = arg.wallet.TargetWarehouseId,
                    TargetWarehouseIdPath = arg.wallet.TargetWarehouseIdPath,
                    TargetWarehouseName = arg.wallet.TargetWarehouseName,
                    TargetWarehouseAddress = arg.wallet.TargetWarehouseAddress,
                    UserId = arg.wallet.UserId,
                    UserName = arg.wallet.UserName,
                    UserFullName = arg.wallet.UserFullName,
                    Created = arg.wallet.Created,
                    Updated = arg.wallet.Updated,
                    UnsignedText = arg.wallet.UnsignedText,
                    Note = arg.wallet.Note,
                    IsDelete = arg.wallet.IsDelete,
                    OrderCodes = arg.wallet.OrderCodes,
                    PackageCodes = arg.wallet.PackageCodes,
                    Customers = arg.wallet.Customers,
                    OrderCodesUnsigned = arg.wallet.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.wallet.PackageCodesUnsigned,
                    CustomersUnsigned = arg.wallet.CustomersUnsigned,
                    Mode = arg.wallet.Mode,
                    PartnerId = arg.wallet.PartnerId,
                    PartnerName = arg.wallet.PartnerName,
                    PartnerUpdate = arg.wallet.PartnerUpdate,
                    EntrepotId = arg.wallet.EntrepotId,
                    EntrepotName = arg.wallet.EntrepotName,
                    OrderServices = arg.wallet.OrderServices,
                    OrderServicesJson = arg.wallet.OrderServicesJson,
                    ImportedTime = detail.Created,
                }).Where(exp2).ToListAsync();
        }

        public Task<long> CountAsync(Expression<Func<Wallet, bool>> exp1, Expression<Func<WalletResult, bool>> exp2)
        {
            return Db.Wallet.Where(exp1)
                .GroupJoin(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 1), wallet => wallet.Id,
                    detail => detail.PackageId, (wallet, details) => new { wallet, details })
                .SelectMany(arg => arg.details.DefaultIfEmpty(), (arg, detail) => new WalletResult()
                {
                    Id = arg.wallet.Id,
                    Code = arg.wallet.Code,
                    Status = arg.wallet.Status,
                    Width = arg.wallet.Width,
                    Length = arg.wallet.Length,
                    Height = arg.wallet.Height,
                    Size = arg.wallet.Size,
                    TotalWeight = arg.wallet.TotalWeight,
                    TotalWeightConverted = arg.wallet.TotalWeightConverted,
                    TotalWeightActual = arg.wallet.TotalWeightActual,
                    TotalVolume = arg.wallet.TotalVolume,
                    Weight = arg.wallet.Weight,
                    WeightConverted = arg.wallet.WeightConverted,
                    WeightActual = arg.wallet.WeightActual,
                    Volume = arg.wallet.Volume,
                    TotalValue = arg.wallet.TotalValue,
                    PackageNo = arg.wallet.PackageNo,
                    CreatedWarehouseId = arg.wallet.CreatedWarehouseId,
                    CreatedWarehouseIdPath = arg.wallet.CreatedWarehouseIdPath,
                    CreatedWarehouseName = arg.wallet.CreatedWarehouseName,
                    CreatedWarehouseAddress = arg.wallet.CreatedWarehouseAddress,
                    CurrentWarehouseId = arg.wallet.CurrentWarehouseId,
                    CurrentWarehouseIdPath = arg.wallet.CurrentWarehouseIdPath,
                    CurrentWarehouseName = arg.wallet.CurrentWarehouseName,
                    CurrentWarehouseAddress = arg.wallet.CurrentWarehouseAddress,
                    TargetWarehouseId = arg.wallet.TargetWarehouseId,
                    TargetWarehouseIdPath = arg.wallet.TargetWarehouseIdPath,
                    TargetWarehouseName = arg.wallet.TargetWarehouseName,
                    TargetWarehouseAddress = arg.wallet.TargetWarehouseAddress,
                    UserId = arg.wallet.UserId,
                    UserName = arg.wallet.UserName,
                    UserFullName = arg.wallet.UserFullName,
                    Created = arg.wallet.Created,
                    Updated = arg.wallet.Updated,
                    UnsignedText = arg.wallet.UnsignedText,
                    Note = arg.wallet.Note,
                    IsDelete = arg.wallet.IsDelete,
                    OrderCodes = arg.wallet.OrderCodes,
                    PackageCodes = arg.wallet.PackageCodes,
                    Customers = arg.wallet.Customers,
                    OrderCodesUnsigned = arg.wallet.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.wallet.PackageCodesUnsigned,
                    CustomersUnsigned = arg.wallet.CustomersUnsigned,
                    Mode = arg.wallet.Mode,
                    PartnerId = arg.wallet.PartnerId,
                    PartnerName = arg.wallet.PartnerName,
                    PartnerUpdate = arg.wallet.PartnerUpdate,
                    EntrepotId = arg.wallet.EntrepotId,
                    EntrepotName = arg.wallet.EntrepotName,
                    OrderServices = arg.wallet.OrderServices,
                    OrderServicesJson = arg.wallet.OrderServicesJson,
                    ImportedTime = detail.Created,
                }).Where(exp2).LongCountAsync();
        }

        public Task<List<OrderServiceResult>> GetOrderServiceByWalletId(int walletId)
        {
            return Db.WalletDetails.Where(x => x.IsDelete == false && x.WalletId == walletId)
                .Join(Db.OrderServices.Where(
                        x => x.IsDelete == false && x.Checked && x.Mode == (byte) OrderServiceMode.Option),
                    x => x.OrderId, x => x.OrderId, (detail, service) => new OrderServiceResult
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName
                    }).Distinct().ToListAsync();
        }

        public Task<List<WalletPackageResult>> GetWalletByPackageCodes(string packageCodes, byte walletMode = 0)
        {
            return Db.Wallet.Where(x => x.IsDelete == false && x.Mode == walletMode)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false && packageCodes.Contains(";" + x.PackageCode + ";")),
                    w => w.Id, d => d.WalletId, (w, wd) => new WalletPackageResult
                    {
                        Id = w.Id,
                        Code = w.Code,
                        Status = w.Status,
                        Width = w.Width,
                        Length = w.Length,
                        Height = w.Height,
                        Size = w.Size,
                        TotalWeight = w.TotalWeight,
                        TotalWeightConverted = w.TotalWeightConverted,
                        TotalWeightActual = w.TotalWeightActual,
                        TotalVolume = w.TotalVolume,
                        Weight = w.Weight,
                        WeightConverted = w.WeightConverted,
                        WeightActual = w.WeightActual,
                        Volume = w.Volume,
                        TotalValue = w.TotalValue,
                        PackageNo = w.PackageNo,
                        CreatedWarehouseId = w.CreatedWarehouseId,
                        CreatedWarehouseIdPath = w.CreatedWarehouseIdPath,
                        CreatedWarehouseName = w.CreatedWarehouseName,
                        CreatedWarehouseAddress = w.CreatedWarehouseAddress,
                        CurrentWarehouseId = w.CurrentWarehouseId,
                        CurrentWarehouseIdPath = w.CurrentWarehouseIdPath,
                        CurrentWarehouseName = w.CurrentWarehouseName,
                        CurrentWarehouseAddress = w.CurrentWarehouseAddress,
                        TargetWarehouseId = w.TargetWarehouseId,
                        TargetWarehouseIdPath = w.TargetWarehouseIdPath,
                        TargetWarehouseName = w.TargetWarehouseName,
                        TargetWarehouseAddress = w.TargetWarehouseAddress,
                        UserId = w.UserId,
                        UserName = w.UserName,
                        UserFullName = w.UserFullName,
                        Created = w.Created,
                        Updated = w.Updated,
                        UnsignedText = w.UnsignedText,
                        Note = w.Note,
                        IsDelete = w.IsDelete,
                        OrderCodes = w.OrderCodes,
                        PackageCodes = w.PackageCodes,
                        Customers = w.Customers,
                        OrderCodesUnsigned = w.OrderCodesUnsigned,
                        PackageCodesUnsigned = w.PackageCodesUnsigned,
                        CustomersUnsigned = w.CustomersUnsigned,
                        Mode = w.Mode,
                        PartnerId = w.PartnerId,
                        PartnerName = w.PartnerName,
                        PartnerUpdate = w.PartnerUpdate,
                        EntrepotId = w.EntrepotId,
                        EntrepotName = w.EntrepotName,
                        PackageId = wd.PackageId,
                    }).ToListAsync();
        }

        public Task<List<WalletPackageResult>> GetWalletPackage(string packageIds)
        {
            return Db.Wallet.Where(x => x.IsDelete == false)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false && packageIds.Contains(";" + x.PackageId + ";")),
                    w => w.Id, wd => wd.WalletId, (w, wd) => new WalletPackageResult()
                    {
                        Id = w.Id,
                        Code = w.Code,
                        Status = w.Status,
                        Width = w.Width,
                        Length = w.Length,
                        Height = w.Height,
                        Size = w.Size,
                        TotalWeight = w.TotalWeight,
                        TotalWeightConverted = w.TotalWeightConverted,
                        TotalWeightActual = w.TotalWeightActual,
                        TotalVolume = w.TotalVolume,
                        Weight = w.Weight,
                        WeightConverted = w.WeightConverted,
                        WeightActual = w.WeightActual,
                        Volume = w.Volume,
                        TotalValue = w.TotalValue,
                        PackageNo = w.PackageNo,
                        CreatedWarehouseId = w.CreatedWarehouseId,
                        CreatedWarehouseIdPath = w.CreatedWarehouseIdPath,
                        CreatedWarehouseName = w.CreatedWarehouseName,
                        CreatedWarehouseAddress = w.CreatedWarehouseAddress,
                        CurrentWarehouseId = w.CurrentWarehouseId,
                        CurrentWarehouseIdPath = w.CurrentWarehouseIdPath,
                        CurrentWarehouseName = w.CurrentWarehouseName,
                        CurrentWarehouseAddress = w.CurrentWarehouseAddress,
                        TargetWarehouseId = w.TargetWarehouseId,
                        TargetWarehouseIdPath = w.TargetWarehouseIdPath,
                        TargetWarehouseName = w.TargetWarehouseName,
                        TargetWarehouseAddress = w.TargetWarehouseAddress,
                        UserId = w.UserId,
                        UserName = w.UserName,
                        UserFullName = w.UserFullName,
                        Created = w.Created,
                        Updated = w.Updated,
                        UnsignedText = w.UnsignedText,
                        Note = w.Note,
                        IsDelete = w.IsDelete,
                        OrderCodes = w.OrderCodes,
                        PackageCodes = w.PackageCodes,
                        Customers = w.Customers,
                        OrderCodesUnsigned = w.OrderCodesUnsigned,
                        PackageCodesUnsigned = w.PackageCodesUnsigned,
                        CustomersUnsigned = w.CustomersUnsigned,
                        Mode = w.Mode,
                        PartnerId = w.PartnerId,
                        PartnerName = w.PartnerName,
                        PartnerUpdate = w.PartnerUpdate,
                        EntrepotId = w.EntrepotId,
                        EntrepotName = w.EntrepotName,
                        PackageId = wd.PackageId,
                    }).ToListAsync();
        }

        public Task<List<Wallet>> WalletByDispatcherId(int dispatcherId)
        {
            return Db.Wallet.Where(x => !x.IsDelete && Db.DispatcherDetails.Any(w => w.WalletId == x.Id && w.DispatcherId == dispatcherId)).ToListAsync();
        }

        public Task<List<WalletTrackerResult>> DispatcherDetailByWalletId(int walletId)
        {
            return
                Db.Dispatchers.Where(x => x.IsDelete == false)
                    .Join(Db.DispatcherDetails.Where(x => x.IsDelete == false && x.WalletId == walletId), d => d.Id,
                        dd => dd.DispatcherId, (d, dd) => new {d, dd})
                    .Join(Db.Wallet, arg => arg.dd.WalletId, w => w.Id, (arg, w) => new {arg.d, arg.dd, w})
                    .Select(x => new WalletTrackerResult()
                    {
                        WalletId = x.w.Id,
                        WalletCode = x.w.Code,
                        WalletCreated = x.w.Created,
                        WalletEntrepotId = x.w.EntrepotId,
                        WalletEntrepotName = x.w.EntrepotName,
                        WalletCreatedWarehouseAddress = x.w.CreatedWarehouseAddress,
                        WalletCreatedWarehouseId = x.w.CreatedWarehouseId,
                        WalletCreatedWarehouseIdPath = x.w.CreatedWarehouseIdPath,
                        WalletCreatedWarehouseName = x.w.CreatedWarehouseName,
                        WalletCurrentWarehouseAddress = x.w.CurrentWarehouseAddress,
                        WalletCurrentWarehouseId = x.w.CurrentWarehouseId,
                        WalletCurrentWarehouseIdPath = x.w.CurrentWarehouseIdPath,
                        WalletCurrentWarehouseName = x.w.CurrentWarehouseName,
                        WalletHeight = x.w.Height,
                        WalletLength = x.w.Length,
                        WalletNote = x.w.Note,
                        WalletPackageNo = x.w.PackageNo,
                        WalletPartnerId = x.w.PartnerId,
                        WalletPartnerName = x.w.PartnerName,
                        WalletPartnerUpdate = x.w.PartnerUpdate,
                        WalletSize = x.w.Size,
                        WalletTargetWarehouseAddress = x.w.TargetWarehouseAddress,
                        WalletTargetWarehouseId = x.w.TargetWarehouseId,
                        WalletTargetWarehouseIdPath = x.w.TargetWarehouseIdPath,
                        WalletTargetWarehouseName = x.w.TargetWarehouseName,
                        WalletTotalValue = x.w.TotalValue,
                        WalletTotalVolume = x.w.TotalVolume,
                        WalletTotalWeight = x.w.TotalWeight,
                        WalletTotalWeightActual = x.w.TotalWeightActual,
                        WalletTotalWeightConverted = x.w.TotalWeightConverted,
                        WalletUserFullName = x.w.UserFullName,
                        WalletUserId = x.w.UserId,
                        WalletUserName = x.w.UserName,
                        WalletVolume = x.w.Volume,
                        WalletWeight = x.w.Weight,
                        WalletWeightActual = x.w.WeightActual,
                        WalletWeightConverted = x.w.WeightConverted,
                        WalletWidth = x.w.Width,
                        DispatcherDetailId = x.dd.Id,
                        DispatcherDetailActualWeight = x.dd.WeightActual,
                        DispatcherDetailConvertedWeight = x.dd.WeightConverted,
                        DispatcherDetailDescription = x.dd.Description,
                        DispatcherDetailNote = x.dd.Note,
                        DispatcherDetailStatus = x.dd.Status,
                        DispatcherDetailVolume = x.dd.Volume,
                        DispatcherDetailWeight = x.dd.Weight,
                        DispatcherDetailValue = x.dd.Value,
                        FromTransportPartnerId = x.dd.FromTransportPartnerId,
                        FromTransportPartnerName = x.dd.FromTransportPartnerName,
                        FromTransportMethodId = x.dd.FromTransportMethodId,
                        FromTransportMethodName = x.dd.FromTransportMethodName,
                        FromEntrepotId = x.dd.FromEntrepotId,
                        FromEntrepotName = x.dd.FromEntrepotName,
                        ToTransportPartnerId = x.dd.ToTransportPartnerId,
                        ToTransportPartnerName = x.dd.ToTransportPartnerName,
                        ToTransportPartnerTime = x.dd.ToTransportPartnerTime,
                        ToTransportMethodId = x.dd.ToTransportMethodId,
                        ToTransportMethodName = x.dd.ToTransportMethodName,
                        ToEntrepotId = x.dd.ToEntrepotId,
                        ToEntrepotName = x.dd.ToEntrepotName,
                        FromDispatcherId = x.dd.FromDispatcherId,
                        FromDispatcherCode = x.dd.FromDispatcherCode,
                        ToDispatcherId = x.dd.ToDispatcherId,
                        ToDispatcherCode = x.dd.ToDispatcherCode,
                        EntrepotId = x.dd.EntrepotId,
                        EntrepotName = x.dd.EntrepotName,
                        DispatcherId = x.d.Id,
                        DispatcherCode = x.d.Code,
                        DispatcherCreatedUserId = x.d.UserId,
                        DispatcherCreatedUserFullName = x.d.UserFullName,
                        DispatcherCreatedUserName = x.d.UserName,
                        DispatcherCreated = x.d.Created,
                        DispatcherPriceType = x.d.PriceType,
                        DispatcherValue = x.d.Value,
                        TransportMethodId = x.d.TransportMethodId,
                        TransportMethodName = x.d.TransportMethodName,
                        TransportPartnerId = x.d.TransportPartnerId,
                        TransportPartnerName = x.d.TransportPartnerName,
                        DispatcherEntrepotId = x.d.EntrepotId,
                        DispatcherEntrepotName = x.d.EntrepotName
                    })
                    .OrderBy(x => x.DispatcherCreated)
                    .ToListAsync();
        }

        public Task<List<WalletTrackerResult>> WalletTracker(Expression<Func<Wallet, bool>> spec, Expression<Func<DispatcherDetail, 
            bool>> spec1, Expression<Func<Dispatcher, bool>> spec2, Expression<Func<WalletTrackerResult, bool>> spec3, int currentPage, 
            int recordPerPage, out int totalRecord, out IQueryable<WalletTrackerResult> outQuery)
        {
            var query = Db.Wallet.Where(spec)
                .GroupJoin(Db.DispatcherDetails.Where(spec1), w => new {Key1 = w.Id, Key2 = w.PartnerId.Value },
                dd => new {Key1 = dd.WalletId, Key2 = dd.TransportPartnerId },
                    (wallet, detail) => new {wallet, detail})
                .SelectMany(x => x.detail.DefaultIfEmpty(), (arg, detail) => new {arg.wallet, detail})
                .GroupJoin(Db.Dispatchers.Where(spec2), arg => arg.detail.DispatcherId, d => d.Id,
                    (arg, d) => new { w = arg.wallet, dd = arg.detail, d})
                .SelectMany(x=> x.d.DefaultIfEmpty(), (x, d) => new WalletTrackerResult()
                {
                    WalletId = x.w.Id,
                    WalletCode = x.w.Code,
                    WalletCreated = x.w.Created,
                    WalletEntrepotId = x.w.EntrepotId,
                    WalletEntrepotName = x.w.EntrepotName,
                    WalletCreatedWarehouseAddress = x.w.CreatedWarehouseAddress,
                    WalletCreatedWarehouseId = x.w.CreatedWarehouseId,
                    WalletCreatedWarehouseIdPath = x.w.CreatedWarehouseIdPath,
                    WalletCreatedWarehouseName = x.w.CreatedWarehouseName,
                    WalletCurrentWarehouseAddress = x.w.CurrentWarehouseAddress,
                    WalletCurrentWarehouseId = x.w.CurrentWarehouseId,
                    WalletCurrentWarehouseIdPath = x.w.CurrentWarehouseIdPath,
                    WalletCurrentWarehouseName = x.w.CurrentWarehouseName,
                    WalletHeight = x.w.Height,
                    WalletLength = x.w.Length,
                    WalletNote = x.w.Note,
                    WalletPackageNo = x.w.PackageNo,
                    WalletPartnerId = x.w.PartnerId,
                    WalletPartnerName = x.w.PartnerName,
                    WalletPartnerUpdate = x.w.PartnerUpdate,
                    WalletSize = x.w.Size,
                    WalletTargetWarehouseAddress = x.w.TargetWarehouseAddress,
                    WalletTargetWarehouseId = x.w.TargetWarehouseId,
                    WalletTargetWarehouseIdPath = x.w.TargetWarehouseIdPath,
                    WalletTargetWarehouseName = x.w.TargetWarehouseName,
                    WalletTotalValue = x.w.TotalValue,
                    WalletTotalVolume = x.w.TotalVolume,
                    WalletTotalWeight = x.w.TotalWeight,
                    WalletTotalWeightActual = x.w.TotalWeightActual,
                    WalletTotalWeightConverted = x.w.TotalWeightConverted,
                    WalletUserFullName = x.w.UserFullName,
                    WalletUserId = x.w.UserId,
                    WalletUserName = x.w.UserName,
                    WalletVolume = x.w.Volume,
                    WalletWeight = x.w.Weight,
                    WalletWeightActual = x.w.WeightActual,
                    WalletWeightConverted = x.w.WeightConverted,
                    WalletWidth = x.w.Width,
                    DispatcherDetailId = x.dd.Id,
                    DispatcherDetailActualWeight = x.dd.WeightActual,
                    DispatcherDetailConvertedWeight = x.dd.WeightConverted,
                    DispatcherDetailDescription = x.dd.Description,
                    DispatcherDetailNote = x.dd.Note,
                    DispatcherDetailStatus = x.dd.Status,
                    DispatcherDetailVolume = x.dd.Volume,
                    DispatcherDetailWeight = x.dd.Weight,
                    DispatcherDetailValue = x.dd.Value,
                    FromTransportPartnerId = x.dd.FromTransportPartnerId,
                    FromTransportPartnerName = x.dd.FromTransportPartnerName,
                    FromTransportMethodId = x.dd.FromTransportMethodId,
                    FromTransportMethodName = x.dd.FromTransportMethodName,
                    FromEntrepotId = x.dd.FromEntrepotId,
                    FromEntrepotName = x.dd.FromEntrepotName,
                    ToTransportPartnerId = x.dd.ToTransportPartnerId,
                    ToTransportPartnerName = x.dd.ToTransportPartnerName,
                    ToTransportPartnerTime = x.dd.ToTransportPartnerTime,
                    ToTransportMethodId = x.dd.ToTransportMethodId,
                    ToTransportMethodName = x.dd.ToTransportMethodName,
                    ToEntrepotId = x.dd.ToEntrepotId,
                    ToEntrepotName = x.dd.ToEntrepotName,
                    FromDispatcherId = x.dd.FromDispatcherId,
                    FromDispatcherCode = x.dd.FromDispatcherCode,
                    ToDispatcherId = x.dd.ToDispatcherId,
                    ToDispatcherCode = x.dd.ToDispatcherCode,
                    EntrepotId = x.dd.EntrepotId,
                    EntrepotName = x.dd.EntrepotName,
                    DispatcherId = d.Id,
                    DispatcherCode = d.Code,
                    DispatcherCreatedUserId = d.UserId,
                    DispatcherCreatedUserFullName = d.UserFullName,
                    DispatcherCreatedUserName = d.UserName,
                    DispatcherCreated = d.Created,
                    DispatcherPriceType = d.PriceType,
                    DispatcherValue = d.Value,
                    TransportMethodId = d.TransportMethodId,
                    TransportMethodName = d.TransportMethodName,
                    TransportPartnerId = d.TransportPartnerId,
                    TransportPartnerName = d.TransportPartnerName,
                    DispatcherEntrepotId = d.EntrepotId,
                    DispatcherEntrepotName = d.EntrepotName
                }).Where(spec3);


            outQuery = query;

            totalRecord = query.Count();

            return query.OrderBy(x=> new {x.DispatcherId, x.DispatcherCreatedUserId, x.WalletUserId }).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<WalletTrackerResult>> WalletTrackerByPartner(Expression<Func<Wallet, bool>> spec, Expression<Func<DispatcherDetail,
            bool>> spec1, Expression<Func<Dispatcher, bool>> spec2, Expression<Func<WalletTrackerResult, bool>> spec3, int currentPage,
            int recordPerPage, out int totalRecord, out IQueryable<WalletTrackerResult> outQuery)
        {
            var query = Db.Wallet.Where(spec)
                .Join(Db.DispatcherDetails.Where(spec1), w => new { Key1 = w.Id },
                dd => new { Key1 = dd.WalletId },
                    (wallet, detail) => new { wallet, detail })
                .Select(arg => new { arg.wallet, arg.detail })
                .Join(Db.Dispatchers.Where(spec2), arg => arg.detail.DispatcherId, d => d.Id,
                    (arg, d) => new { w = arg.wallet, dd = arg.detail, d })
                .Select(x => new WalletTrackerResult()
                {
                    WalletId = x.w.Id,
                    WalletCode = x.w.Code,
                    WalletCreated = x.w.Created,
                    WalletEntrepotId = x.w.EntrepotId,
                    WalletEntrepotName = x.w.EntrepotName,
                    WalletCreatedWarehouseAddress = x.w.CreatedWarehouseAddress,
                    WalletCreatedWarehouseId = x.w.CreatedWarehouseId,
                    WalletCreatedWarehouseIdPath = x.w.CreatedWarehouseIdPath,
                    WalletCreatedWarehouseName = x.w.CreatedWarehouseName,
                    WalletCurrentWarehouseAddress = x.w.CurrentWarehouseAddress,
                    WalletCurrentWarehouseId = x.w.CurrentWarehouseId,
                    WalletCurrentWarehouseIdPath = x.w.CurrentWarehouseIdPath,
                    WalletCurrentWarehouseName = x.w.CurrentWarehouseName,
                    WalletHeight = x.w.Height,
                    WalletLength = x.w.Length,
                    WalletNote = x.w.Note,
                    WalletPackageNo = x.w.PackageNo,
                    WalletPartnerId = x.w.PartnerId,
                    WalletPartnerName = x.w.PartnerName,
                    WalletPartnerUpdate = x.w.PartnerUpdate,
                    WalletSize = x.w.Size,
                    WalletTargetWarehouseAddress = x.w.TargetWarehouseAddress,
                    WalletTargetWarehouseId = x.w.TargetWarehouseId,
                    WalletTargetWarehouseIdPath = x.w.TargetWarehouseIdPath,
                    WalletTargetWarehouseName = x.w.TargetWarehouseName,
                    WalletTotalValue = x.w.TotalValue,
                    WalletTotalVolume = x.w.TotalVolume,
                    WalletTotalWeight = x.w.TotalWeight,
                    WalletTotalWeightActual = x.w.TotalWeightActual,
                    WalletTotalWeightConverted = x.w.TotalWeightConverted,
                    WalletUserFullName = x.w.UserFullName,
                    WalletUserId = x.w.UserId,
                    WalletUserName = x.w.UserName,
                    WalletVolume = x.w.Volume,
                    WalletWeight = x.w.Weight,
                    WalletWeightActual = x.w.WeightActual,
                    WalletWeightConverted = x.w.WeightConverted,
                    WalletWidth = x.w.Width,
                    DispatcherDetailId = x.dd.Id,
                    DispatcherDetailActualWeight = x.dd.WeightActual,
                    DispatcherDetailConvertedWeight = x.dd.WeightConverted,
                    DispatcherDetailDescription = x.dd.Description,
                    DispatcherDetailNote = x.dd.Note,
                    DispatcherDetailStatus = x.dd.Status,
                    DispatcherDetailVolume = x.dd.Volume,
                    DispatcherDetailWeight = x.dd.Weight,
                    DispatcherDetailValue = x.dd.Value,
                    FromTransportPartnerId = x.dd.FromTransportPartnerId,
                    FromTransportPartnerName = x.dd.FromTransportPartnerName,
                    FromTransportMethodId = x.dd.FromTransportMethodId,
                    FromTransportMethodName = x.dd.FromTransportMethodName,
                    FromEntrepotId = x.dd.FromEntrepotId,
                    FromEntrepotName = x.dd.FromEntrepotName,
                    ToTransportPartnerId = x.dd.ToTransportPartnerId,
                    ToTransportPartnerName = x.dd.ToTransportPartnerName,
                    ToTransportPartnerTime = x.dd.ToTransportPartnerTime,
                    ToTransportMethodId = x.dd.ToTransportMethodId,
                    ToTransportMethodName = x.dd.ToTransportMethodName,
                    ToEntrepotId = x.dd.ToEntrepotId,
                    ToEntrepotName = x.dd.ToEntrepotName,
                    FromDispatcherId = x.dd.FromDispatcherId,
                    FromDispatcherCode = x.dd.FromDispatcherCode,
                    ToDispatcherId = x.dd.ToDispatcherId,
                    ToDispatcherCode = x.dd.ToDispatcherCode,
                    EntrepotId = x.dd.EntrepotId,
                    EntrepotName = x.dd.EntrepotName,
                    DispatcherId = x.d.Id,
                    DispatcherCode = x.d.Code,
                    DispatcherCreatedUserId = x.d.UserId,
                    DispatcherCreatedUserFullName = x.d.UserFullName,
                    DispatcherCreatedUserName = x.d.UserName,
                    DispatcherCreated = x.d.Created,
                    DispatcherPriceType = x.d.PriceType,
                    DispatcherValue = x.d.Value,
                    TransportMethodId = x.d.TransportMethodId,
                    TransportMethodName = x.d.TransportMethodName,
                    TransportPartnerId = x.d.TransportPartnerId,
                    TransportPartnerName = x.d.TransportPartnerName,
                    DispatcherEntrepotId = x.d.EntrepotId,
                    DispatcherEntrepotName = x.d.EntrepotName,
                }).Where(spec3);


            outQuery = query;

            totalRecord = query.Count();

            return query.OrderBy(x => new { x.ToDispatcherId, x.DispatcherDetailStatus })
                .Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<Wallet>> SuggetionForPutAway(Expression<Func<Wallet, bool>> predicate, string packageCodes, int size)
        {
            return Db.Wallet.Where(predicate)
                    .Join(Db.WalletDetails.Where(x => packageCodes == "" || !packageCodes.Contains(";" + x.PackageCode + ";")),
                        w => w.Id, d => d.WalletId, (w, d) => new {w, d})
                     .Join(Db.OrderPackages.Where(x=> x.IsDelete == false && x.Status == (byte)OrderPackageStatus.Received), arg=> arg.d.PackageId, p=> p.Id, (arg, p)=> arg.w)
                    .Distinct()
                    .OrderBy(x => x.Id)
                    .Take(size).ToListAsync();
        }
    }
}
