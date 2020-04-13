using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.Models;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DeliveryRepository : Repository<Delivery>
    {
        public DeliveryRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }

        // Lấy ra các kiện hàng đang có phiếu xuất kho
        public Task<List<DeliveryPackageResult>> GetPackageInDelivery(string packageIds)
        {
            return Db.Delivery.Where(x => x.IsDelete == false)
                .Join(
                    Db.DeliveryDetails.Where(x => x.IsDelete == false && packageIds.Contains(";" + x.PackageId + ";")),
                    x => x.Id, x => x.DeliveryId,
                    (delivery, detail) => new DeliveryPackageResult()
                    {
                        TransportCode = detail.TransportCode,
                        PackageCode = detail.PackageCode,
                        DeliveryCode = detail.DeliveryCode,
                        PackageId = detail.PackageId,
                        DeliveryId = detail.DeliveryId
                    })
                .ToListAsync();
        }

        /// <summary>
        /// Lấy ra các đơn hàng đã có phiếu xuất kho
        /// </summary>
        /// <param name="orderIds">Id đơn hàng (dạng: ";orderId1;orderId2;orderIdN;")</param>
        /// <returns></returns>
        public Task<List<int>> GetOrderHaveDelivery(string orderIds)
        {
            return Db.Delivery.Where(x => x.IsDelete == false)
                .Join(Db.DeliveryDetails.Where(x => x.IsDelete == false && orderIds.Contains(";" + x.OrderId + ";")),
                    d => d.Id, dd => dd.DeliveryId, (d, dd) => dd.OrderId)
                .Distinct()
                .ToListAsync();
        }

        public Task<List<OrderPackage>> GetPackageByDeliveryId(int deliveryId)
        {
            return Db.Delivery.Where(x => x.IsDelete == false && x.Id == deliveryId)
                .Join(Db.DeliveryDetails.Where(x => x.IsDelete == false),
                    d => d.Id, dd => dd.DeliveryId, (d, dd) => dd)
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false), d => d.PackageId, p => p.Id, (d, p) => p)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        ///     Lấy ra các khách hàng có kiện hàng đang chờ xuất kho
        /// </summary>
        /// <param name="user">Nhân viên hiện tại</param>
        /// <param name="systemId"></param>
        /// <param name="orderType"></param>
        /// <param name="keyword"></param>
        /// <param name="currentPage"></param>
        /// <param name="recordPerPage"></param>
        /// <param name="totalRecord"></param>
        /// <returns></returns>
        public Task<List<Customer>> Search(UserState user, byte? systemId, byte? orderType, string keyword,
            int currentPage, int recordPerPage, out int totalRecord)
        {
            var query = Db.Customers
                .Join(Db.OrderPackages
                        .Where(x => x.IsDelete == false && x.CurrentLayoutId != null &&
                                    x.CurrentWarehouseId == user.OfficeId &&
                                    x.UnsignedText.Contains(keyword) &&
                                    (systemId == null || x.SystemId == systemId) &&
                                    (orderType == null || x.OrderType == orderType)), x => x.Id,
                    p => p.CustomerId, (c, p) => new {c, p})
                .GroupJoin(Db.DeliveryDetails.Where(x => x.IsDelete == false), arg => arg.p.Id, dd => dd.PackageId,
                    (arg, dd) => new {arg.p, arg.c, dd})
                .SelectMany(x => x.dd.DefaultIfEmpty(), (arg, dd) => new {arg.p, arg.c, dd})
                .GroupJoin(Db.Delivery.Where(x => x.IsDelete == false && x.WarehouseIdPath == user.OfficeIdPath),
                    arg => arg.dd.DeliveryId, d => d.Id, (arg, d) => new
                    {
                        arg.p,
                        arg.c,
                        arg.dd,
                        d
                    })
                .SelectMany(x => x.d.DefaultIfEmpty(), (arg, d) => new
                {
                    arg.p,
                    arg.c,
                    arg.dd,
                    d
                })
                .Where(x => x.d == null)
                .Select(x => x.c)
                .Distinct();

            totalRecord = query.Count();

            return query.OrderBy(x => x.LevelId)
                .Skip((currentPage - 1) * recordPerPage)
                .Take(recordPerPage)
                .ToListAsync();
        }

        /// <summary>
        ///     Lấy ra các kiện chờ xuất kho của khách hàng
        /// </summary>
        /// <param name="user"></param>
        /// <param name="systemId"></param>
        /// <param name="orderType"></param>
        /// <param name="customerIds">Id khách hàng (dạng: ";customerId1;customerId2;customerIdN;")</param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<List<OrderPackagDeliveryResult>> Search(UserState user, byte? systemId, byte? orderType, string customerIds,
            string keyword = "")
        {
            var walletQuery = Db.Wallet.Where(x => x.IsDelete == false && x.Mode == 0)
                .Join(Db.WalletDetails.Where(x => x.IsDelete == false), w => w.Id, d => d.WalletId,
                    (w, d) => new {d.WalletCode, d.PackageId}).Distinct();

            return Db.OrderPackages.Where(x => x.IsDelete == false && x.CurrentLayoutId != null &&
                                               x.CurrentWarehouseId == user.OfficeId &&
                                               x.UnsignedText.Contains(keyword) &&
                                               (systemId == null || x.SystemId == systemId) &&
                                               (orderType == null || x.OrderType == orderType) &&
                                               customerIds.Contains(";" + x.CustomerId + ";"))
                .GroupJoin(Db.DeliveryDetails.Where(x => x.IsDelete == false), arg => arg.Id, dd => dd.PackageId,
                    (p, dd) => new {p, dd})
                .SelectMany(x => x.dd.DefaultIfEmpty(), (arg, dd) => new {arg.p, dd})
                .GroupJoin(Db.Delivery.Where(x => x.IsDelete == false && x.WarehouseIdPath == user.OfficeIdPath),
                    arg => arg.dd.DeliveryId, d => d.Id, (arg, d) => new
                    {
                        arg.p,
                        arg.dd,
                        d
                    })
                .SelectMany(x => x.d.DefaultIfEmpty(), (arg, d) => new
                {
                    arg.p,
                    arg.dd,
                    d
                })
                .Where(x => x.d == null)
                .Select(x => x.p)
                .Distinct()
                .GroupJoin(walletQuery, p => p.Id, wd => wd.PackageId, (d, wd) => new {d, wd})
                .SelectMany(x => x.wd.DefaultIfEmpty(), (arg, wd) => new OrderPackagDeliveryResult()
                {
                    Id = arg.d.Id,
                    Code = arg.d.Code,
                    Status = arg.d.Status,
                    Note = arg.d.Note,
                    OrderId = arg.d.OrderId,
                    OrderCode = arg.d.OrderCode,
                    OrderServices = arg.d.OrderServices,
                    CustomerId = arg.d.CustomerId,
                    CustomerName = arg.d.CustomerName,
                    CustomerUserName = arg.d.CustomerUserName,
                    CustomerLevelId = arg.d.CustomerLevelId,
                    CustomerLevelName = arg.d.CustomerLevelName,
                    CustomerWarehouseId = arg.d.CustomerWarehouseId,
                    CustomerWarehouseAddress = arg.d.CustomerWarehouseAddress,
                    CustomerWarehouseName = arg.d.CustomerWarehouseName,
                    CustomerWarehouseIdPath = arg.d.CustomerWarehouseIdPath,
                    TransportCode = arg.d.TransportCode,
                    Weight = arg.d.Weight,
                    WeightConverted = arg.d.WeightConverted,
                    WeightActual = arg.d.WeightActual,
                    WeightActualPercent = arg.d.WeightActualPercent,
                    WeightWapperPercent = arg.d.WeightWapperPercent,
                    OtherService = arg.d.OtherService,
                    WeightWapper = arg.d.WeightWapper,
                    TotalPriceWapper = arg.d.TotalPriceWapper,
                    Volume = arg.d.Volume,
                    VolumeActual = arg.d.VolumeActual,
                    VolumeWapperPercent = arg.d.VolumeWapperPercent,
                    VolumeWapper = arg.d.VolumeWapper,
                    Size = arg.d.Size,
                    Width = arg.d.Width,
                    Height = arg.d.Height,
                    Length = arg.d.Length,
                    TotalPrice = arg.d.TotalPrice,
                    WarehouseId = arg.d.WarehouseId,
                    WarehouseName = arg.d.WarehouseName,
                    WarehouseIdPath = arg.d.WarehouseIdPath,
                    WarehouseAddress = arg.d.WarehouseAddress,
                    UserId = arg.d.UserId,
                    UserFullName = arg.d.UserFullName,
                    SystemId = arg.d.SystemId,
                    SystemName = arg.d.SystemName,
                    Created = arg.d.Created,
                    LastUpdate = arg.d.LastUpdate,
                    HashTag = arg.d.HashTag,
                    ForcastDate = arg.d.ForcastDate,
                    PackageNo = arg.d.PackageNo,
                    UnsignedText = arg.d.UnsignedText,
                    CurrentLayoutId = arg.d.CurrentLayoutId,
                    CurrentLayoutName = arg.d.CurrentLayoutName,
                    CurrentLayoutIdPath = arg.d.CurrentLayoutIdPath,
                    CurrentWarehouseId = arg.d.CurrentWarehouseId,
                    CurrentWarehouseName = arg.d.CurrentWarehouseName,
                    CurrentWarehouseIdPath = arg.d.CurrentWarehouseIdPath,
                    CurrentWarehouseAddress = arg.d.CurrentWarehouseAddress,
                    IsDelete = arg.d.IsDelete,
                    OrderCodes = arg.d.OrderCodes,
                    PackageCodes = arg.d.PackageCodes,
                    Customers = arg.d.Customers,
                    OrderCodesUnsigned = arg.d.OrderCodesUnsigned,
                    PackageCodesUnsigned = arg.d.PackageCodesUnsigned,
                    CustomersUnsigned = arg.d.CustomersUnsigned,
                    WalletCode = wd.WalletCode,
                })
                .ToListAsync();
        }
    }
}
