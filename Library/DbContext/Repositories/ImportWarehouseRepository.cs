using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ImportWarehouseRepository : Repository<ImportWarehouse>
    {
        public ImportWarehouseRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }

        public Task<List<ImportWarehouseResult>> Search(Expression<Func<ImportWarehouse, bool>> predicate, 
            Expression<Func<ImportWarehouseResult, bool>> predicate3, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.ImportWarehouse.Where(predicate)
                .Join(Db.ImportWarehouseDetails.Where(x=> x.IsDelete == false && x.Type == 0), i => i.Id, 
                    id => id.ImportWarehouseId, (imp, impD) => new {imp, impD})
                .Join(Db.OrderPackages, arg => arg.impD.PackageId, p => p.Id,
                    (arg, p) => new ImportWarehouseResult
                    {
                        Id = arg.imp.Id, // Id (Primary key)
                        Code = arg.imp.Code, // Code (length: 20)
                        Status = arg.imp.Status, // Status
                        PackageNumber = arg.imp.PackageNumber, // PackageNumber
                        WalletNumber = arg.imp.WalletNumber, // WalletNumber
                        WarehouseId = arg.imp.WarehouseId, // WarehouseId
                        WarehouseIdPath = arg.imp.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseName = arg.imp.WarehouseName, // WarehouseName (length: 300)
                        WarehouseAddress = arg.imp.WarehouseAddress, // WarehouseAddress (length: 300)
                        ShipperName = arg.imp.ShipperName, // ShipperName (length: 300)
                        ShipperPhone = arg.imp.ShipperPhone, // ShipperPhone (length: 300)
                        ShipperAddress = arg.imp.ShipperAddress, // ShipperAddress (length: 300)
                        ShipperEmail = arg.imp.ShipperEmail, // ShipperEmail (length: 300)
                        UserId = arg.imp.UserId, // UserId
                        UserName = arg.imp.UserName, // UserName (length: 30)
                        UserFullName = arg.imp.UserFullName, // UserFullName (length: 300)
                        WarehouseManagerId = arg.imp.WarehouseManagerId, // WarehouseManagerId
                        WarehouseManagerCode = arg.imp.WarehouseManagerCode, // WarehouseManagerCode (length: 20)
                        WarehouseManagerFullName = arg.imp.WarehouseManagerFullName,
                        // WarehouseManagerFullName (length: 300)
                        WarehouseAccountantId = arg.imp.WarehouseAccountantId, // WarehouseAccountantId
                        WarehouseAccountantCode = arg.imp.WarehouseAccountantCode,
                        // WarehouseAccountantCode (length: 20)
                        WarehouseAccountantFullName = arg.imp.WarehouseAccountantFullName,
                        // WarehouseAccountantFullName (length: 300)
                        Note = arg.imp.Note, // Note (length: 500)
                        UnsignedText = arg.imp.UnsignedText, // Note (length: 500)
                        Created = arg.imp.Created, // Created
                        Updated = arg.imp.Updated, // Updated
                        PackageId = p.Id, // Id (Primary key)
                        PackageCode = p.Code, // Code (length: 30)
                        PackageStatus = p.Status, // Status
                        PackageNote = p.Note, // Note (length: 600)
                        PackageOrderId = p.OrderId, // OrderId
                        PackageOrderCode = p.OrderCode, // OrderCode (length: 50)
                        PackageOrderServices = p.OrderServices, // OrderServices (length: 500)
                        PackageCustomerId = p.CustomerId, // CustomerId
                        PackageCustomerName = p.CustomerName, // CustomerName (length: 300)
                        PackageCustomerUserName = p.CustomerUserName, // CustomerUserName (length: 300)
                        PackageCustomerLevelId = p.CustomerLevelId, // CustomerLevelId
                        PackageCustomerLevelName = p.CustomerLevelName, // CustomerLevelName (length: 300)
                        PackageCustomerWarehouseId = p.CustomerWarehouseId, // CustomerWarehouseId
                        PackageCustomerWarehouseAddress = p.CustomerWarehouseAddress,
                        // CustomerWarehouseAddress (length: 500)
                        PackageCustomerWarehouseName = p.CustomerWarehouseName, // CustomerWarehouseName (length: 300)
                        PackageCustomerWarehouseIdPath = p.CustomerWarehouseIdPath,
                        // CustomerWarehouseIdPath (length: 300)
                        PackageTransportCode = p.TransportCode, // TransportCode (length: 50)
                        PackageWeight = p.Weight, // Weight
                        PackageWeightConverted = p.WeightConverted, // WeightConverted
                        PackageWeightActual = p.WeightActual, // WeightActual
                        PackageWeightWapperPercent = p.WeightWapperPercent, // WeightWapperPercent
                        PackageWeightWapper = p.WeightWapper, // WeightWapper
                        PackageTotalPriceWapper = p.TotalPriceWapper, // TotalPriceWapper
                        PackageVolume = p.Volume, // Volume
                        PackageVolumeActual = p.VolumeActual, // VolumeActual
                        PackageVolumeWapperPercent = p.VolumeWapperPercent, // VolumeWapperPercent
                        PackageVolumeWapper = p.VolumeWapper, // VolumeWapper
                        PackageSize = p.Size, // Size (length: 500)
                        PackageWidth = p.Width, // Width
                        PackageHeight = p.Height, // Height
                        PackageLength = p.Length, // Length
                        PackageTotalPrice = p.TotalPrice, // TotalPrice
                        PackageWarehouseId = p.WarehouseId, // WarehouseId
                        PackageWarehouseName = p.WarehouseName, // WarehouseName (length: 300)
                        PackageWarehouseIdPath = p.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        PackageWarehouseAddress = p.WarehouseAddress, // WarehouseAddress (length: 300)
                        PackageUserId = p.UserId, // UserId
                        PackageUserFullName = p.UserFullName, // UserFullName (length: 300)
                        PackageSystemId = p.SystemId, // SystemId
                        PackageSystemName = p.SystemName, // SystemName (length: 300)
                        PackageCreated = p.Created, // Created
                        PackageLastUpdate = p.LastUpdate, // LastUpdate
                        PackageHashTag = p.HashTag, // HashTag
                        PackageForcastDate = p.ForcastDate, // ForcastDate
                        PackagePackageNo = p.PackageNo, // PackageNo
                        PackageUnsignedText = p.UnsignedText, // UnsignedText (length: 500)
                        PackageCurrentLayoutId = p.CurrentLayoutId, // CurrentLayoutId
                        PackageCurrentLayoutName = p.CurrentLayoutName, // CurrentLayoutName (length: 300)
                        PackageCurrentLayoutIdPath = p.CurrentLayoutIdPath, // CurrentLayoutIdPath (length: 300)
                        PackageCurrentWarehouseId = p.CurrentWarehouseId, // CurrentWarehouseId
                        PackageCurrentWarehouseName = p.CurrentWarehouseName, // CurrentWarehouseName (length: 300)
                        PackageCurrentWarehouseIdPath = p.CurrentWarehouseIdPath,
                        // CurrentWarehouseIdPath (length: 300)
                        PackageCurrentWarehouseAddress = p.CurrentWarehouseAddress,
                        // CurrentWarehouseAddress (length: 300)
                        PackageIsDelete = p.IsDelete, // IsDelete
                        PackageOrderCodes = p.OrderCodes, // OrderCodes (length: 1000)
                        PackagePackageCodes = p.PackageCodes, // PackageCodes (length: 1000)
                        PackageCustomers = p.Customers, // Customers (length: 1000)
                        PackageOrderCodesUnsigned = p.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                        PackagePackageCodesUnsigned = p.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                        PackageCustomersUnsigned = p.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                        PackageOrderType = p.OrderType, // OrderType
                    }).Where(predicate3);

            totalRecord = query.LongCount();

            return query.OrderByDescending(x => new {x.Id, x.PackageId})
                    .Skip((currentPage - 1)*recordPerPage)
                    .Take(recordPerPage)
                    .ToListAsync();
        }

        public Task<ImportWarehouse> GetImportWarehouse(string orderCode)
        {
            return Db.OrderPackages.Where(x => x.IsDelete == false && x.OrderCode == orderCode)
                .Join(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 0), x => x.Id,
                    d => d.PackageId, (package, detail) => new {package, detail})
                .Join(Db.ImportWarehouse.Where(x => x.IsDelete == false && x.WarehouseId == 10),
                    arg => arg.detail.ImportWarehouseId, warehouse => warehouse.Id, (agr, import) => import)
                 .Distinct()
                .FirstOrDefaultAsync();
        }

        public Task<List<ImportWarehouseWalletResult>> Search(Expression<Func<ImportWarehouse, bool>> predicate,
            Expression<Func<ImportWarehouseWalletResult, bool>> predicate3, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.ImportWarehouse.Where(predicate)
                .Join(Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.Type == 1), i => i.Id, 
                    id => id.ImportWarehouseId, (imp, impD) => new { imp, impD })
                .Join(Db.Wallet, arg => arg.impD.PackageId, p => p.Id,
                    (arg, w) => new ImportWarehouseWalletResult
                    {
                        Id = arg.imp.Id, // Id (Primary key)
                        Code = arg.imp.Code, // Code (length: 20)
                        Status = arg.imp.Status, // Status
                        PackageNumber = arg.imp.PackageNumber, // PackageNumber
                        WalletNumber = arg.imp.WalletNumber, // WalletNumber
                        WarehouseId = arg.imp.WarehouseId, // WarehouseId
                        WarehouseIdPath = arg.imp.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseName = arg.imp.WarehouseName, // WarehouseName (length: 300)
                        WarehouseAddress = arg.imp.WarehouseAddress, // WarehouseAddress (length: 300)
                        ShipperName = arg.imp.ShipperName, // ShipperName (length: 300)
                        ShipperPhone = arg.imp.ShipperPhone, // ShipperPhone (length: 300)
                        ShipperAddress = arg.imp.ShipperAddress, // ShipperAddress (length: 300)
                        ShipperEmail = arg.imp.ShipperEmail, // ShipperEmail (length: 300)
                        UserId = arg.imp.UserId, // UserId
                        UserName = arg.imp.UserName, // UserName (length: 30)
                        UserFullName = arg.imp.UserFullName, // UserFullName (length: 300)
                        WarehouseManagerId = arg.imp.WarehouseManagerId, // WarehouseManagerId
                        WarehouseManagerCode = arg.imp.WarehouseManagerCode, // WarehouseManagerCode (length: 20)
                        WarehouseManagerFullName = arg.imp.WarehouseManagerFullName,
                        // WarehouseManagerFullName (length: 300)
                        WarehouseAccountantId = arg.imp.WarehouseAccountantId, // WarehouseAccountantId
                        WarehouseAccountantCode = arg.imp.WarehouseAccountantCode,
                        // WarehouseAccountantCode (length: 20)
                        WarehouseAccountantFullName = arg.imp.WarehouseAccountantFullName,
                        // WarehouseAccountantFullName (length: 300)
                        Note = arg.imp.Note, // Note (length: 500)
                        UnsignedText = arg.imp.UnsignedText, // Note (length: 500)
                        Created = arg.imp.Created, // Created
                        Updated = arg.imp.Updated, // Updated
                        WalletId = w.Id,  // Id (Primary key)
                        WalletCode = w.Code,  // Code (length: 20)
                        WalletStatus = w.Status,  // Status
                        WalletWidth = w.Width,  // Width
                        WalletLength = w.Length,  // Length
                        WalletHeight = w.Height,  // Height
                        WalletSize = w.Size,  // Size (length: 500)
                        WalletTotalWeight = w.TotalWeight,  // TotalWeight
                        WalletTotalWeightConverted = w.TotalWeightConverted,  // TotalWeightConverted
                        WalletTotalWeightActual = w.TotalWeightActual,  // TotalWeightActual
                        WalletTotalVolume = w.TotalVolume,  // TotalVolume
                        WalletWeight = w.Weight,  // Weight
                        WalletWeightConverted = w.WeightConverted,  // WeightConverted
                        WalletWeightActual = w.WeightActual,  // WeightActual
                        WalletVolume = w.Volume,  // Volume
                        WalletTotalValue = w.TotalValue,  // TotalValue
                        WalletPackageNo = w.PackageNo,  // PackageNo
                        WalletCreatedWarehouseId = w.CreatedWarehouseId,  // CreatedWarehouseId
                        WalletCreatedWarehouseIdPath = w.CreatedWarehouseIdPath,  // CreatedWarehouseIdPath (length: 300)
                        WalletCreatedWarehouseName = w.CreatedWarehouseName,  // CreatedWarehouseName (length: 300)
                        WalletCreatedWarehouseAddress = w.CreatedWarehouseAddress,  // CreatedWarehouseAddress (length: 500)
                        WalletCurrentWarehouseId = w.CurrentWarehouseId,  // CurrentWarehouseId
                        WalletCurrentWarehouseIdPath = w.CurrentWarehouseIdPath,  // CurrentWarehouseIdPath (length: 300)
                        WalletCurrentWarehouseName = w.CurrentWarehouseName,  // CurrentWarehouseName (length: 300)
                        WalletCurrentWarehouseAddress = w.CurrentWarehouseAddress,  // CurrentWarehouseAddress (length: 500)
                        WalletTargetWarehouseId = w.TargetWarehouseId,  // TargetWarehouseId
                        WalletTargetWarehouseIdPath = w.TargetWarehouseIdPath,  // TargetWarehouseIdPath (length: 300)
                        WalletTargetWarehouseName = w.TargetWarehouseName,  // TargetWarehouseName (length: 300)
                        WalletTargetWarehouseAddress = w.TargetWarehouseAddress,  // TargetWarehouseAddress (length: 500)
                        WalletUserId = w.UserId,  // UserId
                        WalletUserName = w.UserName,  // UserName (length: 50)
                        WalletUserFullName = w.UserFullName,  // UserFullName (length: 300)
                        WalletCreated = w.Created,  // Created
                        WalletUpdated = w.Updated,  // Updated
                        WalletUnsignedText = w.UnsignedText,  // UnsignedText (length: 500)
                        WalletNote = w.Note,  // Note (length: 500)
                        WalletOrderCodes = w.OrderCodes,  // OrderCodes (length: 1000)
                        WalletPackageCodes = w.PackageCodes,  // PackageCodes (length: 1000)
                        WalletCustomers = w.Customers,  // Customers (length: 1000)
                        WalletOrderCodesUnsigned = w.OrderCodesUnsigned,  // OrderCodesUnsigned (length: 1000)
                        WalletPackageCodesUnsigned = w.PackageCodesUnsigned,  // PackageCodesUnsigned (length: 1000)
                        WalletCustomersUnsigned = w.CustomersUnsigned,  // CustomersUnsigned (length: 1000)
                        WalletMode = w.Mode,  // Mode
                        WalletPartnerId = w.PartnerId,  // PartnerId
                        WalletPartnerName = w.PartnerName,  // PartnerName (length: 300)
                        WalletPartnerUpdate = w.PartnerUpdate,  // PartnerUpdate
                    }).Where(predicate3);

            totalRecord = query.LongCount();

            return query.OrderBy(x => new {x.Id, x.WalletId})
                .Skip((currentPage - 1)*recordPerPage)
                .Take(recordPerPage)
                .ToListAsync();
        }
    }
}
