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
    public class DispatcherRepository : Repository<Dispatcher>
    {
        public DispatcherRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<DispatcherDetail>> DispatcherDetailOld(DateTime timeNow, string walletCodes)
        {
            return Db.Dispatchers.Where(x => x.IsDelete == false && x.Created < timeNow)
                .Join(Db.DispatcherDetails.Where(
                        x => x.IsDelete == false && x.Status != 2 && walletCodes.Contains(";" + x.WalletCode + ";")),
                    i => i.Id,
                    id => id.DispatcherId,
                    (d, dd) => dd).Distinct().ToListAsync();
        }

        public Task<List<DispatcherResult>> Search(Expression<Func<Dispatcher, bool>> predicate,
            Expression<Func<DispatcherResult, bool>> predicate3, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.Dispatchers.Where(predicate)
                .Join(Db.DispatcherDetails.Where(x => x.IsDelete == false), i => i.Id, id => id.DispatcherId, (d, dd) => new { d, dd })
                .Join(Db.Wallet, arg => arg.dd.WalletId, p => p.Id,
                    (arg, w) => new DispatcherResult
                    {
                        Id = arg.d.Id, // Id (Primary key)
                        Code = arg.d.Code, // Code (length: 20)
                        FromWarehouseId = arg.d.FromWarehouseId, // FromWarehouseId
                        FromWarehouseIdPath = arg.d.FromWarehouseIdPath, // FromWarehouseIdPath (length: 300)
                        FromWarehouseName = arg.d.FromWarehouseName, // FromWarehouseName (length: 300)
                        FromWarehouseAddress = arg.d.FromWarehouseAddress, // FromWarehouseAddress (length: 500)
                        Status = arg.d.Status, // Status
                        Amount = arg.d.Amount, // Amount
                        TotalWeight = arg.d.TotalWeight, // TotalWeight
                        TotalWeightActual = arg.d.TotalWeightActual, // TotalWeightActual
                        TotalWeightConverted = arg.d.TotalWeightConverted, // TotalWeightConverted
                        TotalVolume = arg.d.TotalVolume, // TotalVolume
                        TotalPackageNo = arg.d.TotalPackageNo, // TotalPackageNo
                        WalletNo = arg.d.WalletNo, // WalletNo
                        PriceType = arg.d.PriceType, // PriceType
                        Price = arg.d.Price, // Price
                        Value = arg.d.Value, // Value
                        UserId = arg.d.UserId, // UserId
                        UserName = arg.d.UserName, // UserName (length: 50)
                        UserFullName = arg.d.UserFullName, // UserFullName (length: 300)
                        Created = arg.d.Created, // Created
                        Updated = arg.d.Updated, // Updated
                        Note = arg.d.Note, // Note (length: 500)
                        UnsignedText = arg.d.UnsignedText, // UnsignedText (length: 500)
                        ForcastDate = arg.d.ForcastDate, // ForcastDate
                        ToWarehouseId = arg.d.ToWarehouseId, // ToWarehouseId
                        ToWarehouseIdPath = arg.d.ToWarehouseIdPath, // ToWarehouseIdPath (length: 300)
                        ToWarehouseName = arg.d.ToWarehouseName, // ToWarehouseName (length: 300)
                        ToWarehouseAddress = arg.d.ToWarehouseAddress, // ToWarehouseAddress (length: 500)
                        TransportPartnerId = arg.d.TransportPartnerId, // TransportPartnerId
                        TransportPartnerName = arg.d.TransportPartnerName, // TransportPartnerName (length: 300)
                        TransportMethodId = arg.d.TransportMethodId, // TransportMethodId
                        TransportMethodName = arg.d.TransportMethodName, // TransportMethodName (length: 300)
                        ContactName = arg.d.ContactName, // ContactName (length: 300)
                        ContactPhone = arg.d.ContactPhone, // ContactPhone (length: 20)
                        EntrepotId = arg.d.EntrepotId,
                        EntrepotName = arg.d.EntrepotName,
                        WalletId = w.Id, // Id (Primary key)
                        WalletCode = w.Code, // Code (length: 20)
                        WalletStatus = w.Status, // Status
                        WalletWidth = w.Width, // Width
                        WalletLength = w.Length, // Length
                        WalletHeight = w.Height, // Height
                        WalletSize = w.Size, // Size (length: 500)
                        WalletTotalWeight = w.TotalWeight, // TotalWeight
                        WalletTotalWeightConverted = w.TotalWeightConverted, // TotalWeightConverted
                        WalletTotalWeightActual = w.TotalWeightActual, // TotalWeightActual
                        WalletTotalVolume = w.TotalVolume, // TotalVolume
                        WalletWeight = w.Weight, // Weight
                        WalletWeightConverted = w.WeightConverted, // WeightConverted
                        WalletWeightActual = w.WeightActual, // WeightActual
                        WalletVolume = w.Volume, // Volume
                        WalletTotalValue = w.TotalValue, // TotalValue
                        WalletPackageNo = w.PackageNo, // PackageNo
                        WalletCreatedWarehouseId = w.CreatedWarehouseId, // CreatedWarehouseId
                        WalletCreatedWarehouseIdPath = w.CreatedWarehouseIdPath, // CreatedWarehouseIdPath (length: 300)
                        WalletCreatedWarehouseName = w.CreatedWarehouseName, // CreatedWarehouseName (length: 300)
                        WalletCreatedWarehouseAddress = w.CreatedWarehouseAddress, // CreatedWarehouseAddress (length: 500)
                        WalletCurrentWarehouseId = w.CurrentWarehouseId, // CurrentWarehouseId
                        WalletCurrentWarehouseIdPath = w.CurrentWarehouseIdPath, // CurrentWarehouseIdPath (length: 300)
                        WalletCurrentWarehouseName = w.CurrentWarehouseName, // CurrentWarehouseName (length: 300)
                        WalletCurrentWarehouseAddress = w.CurrentWarehouseAddress, // CurrentWarehouseAddress (length: 500)
                        WalletTargetWarehouseId = w.TargetWarehouseId, // TargetWarehouseId
                        WalletTargetWarehouseIdPath = w.TargetWarehouseIdPath, // TargetWarehouseIdPath (length: 300)
                        WalletTargetWarehouseName = w.TargetWarehouseName, // TargetWarehouseName (length: 300)
                        WalletTargetWarehouseAddress = w.TargetWarehouseAddress, // TargetWarehouseAddress (length: 500)
                        WalletUserId = w.UserId, // UserId
                        WalletUserName = w.UserName, // UserName (length: 50)
                        WalletUserFullName = w.UserFullName, // UserFullName (length: 300)
                        WalletCreated = w.Created, // Created
                        WalletUpdated = w.Updated, // Updated
                        WalletUnsignedText = w.UnsignedText, // UnsignedText (length: 500)
                        WalletNote = w.Note, // Note (length: 500)
                        WalletOrderCodes = w.OrderCodes, // OrderCodes (length: 1000)
                        WalletPackageCodes = w.PackageCodes, // PackageCodes (length: 1000)
                        WalletCustomers = w.Customers, // Customers (length: 1000)
                        WalletOrderCodesUnsigned = w.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                        WalletPackageCodesUnsigned = w.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                        WalletCustomersUnsigned = w.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                        WalletMode = w.Mode, // Mode
                        WalletPartnerId = w.PartnerId, // PartnerId
                        WalletPartnerName = w.PartnerName, // PartnerName (length: 300)
                        WalletPartnerUpdate = w.PartnerUpdate, // PartnerUpdate,
                        FromTransportPartnerId = arg.dd.FromTransportPartnerId,
                        FromTransportPartnerName = arg.dd.FromTransportPartnerName,
                        ToTransportPartnerId = arg.dd.ToTransportPartnerId,
                        ToTransportPartnerName = arg.dd.ToTransportPartnerName,
                        ToTransportPartnerTime = arg.dd.ToTransportPartnerTime,
                        FromDispatcherCode = arg.dd.FromDispatcherCode,
                        FromDispatcherId = arg.dd.FromDispatcherId,
                        FromEntrepotId = arg.dd.FromEntrepotId,
                        FromEntrepotName = arg.dd.FromEntrepotName,
                        FromTransportMethodId = arg.dd.FromTransportMethodId,
                        FromTransportMethodName = arg.dd.FromTransportMethodName,
                        ToDispatcherCode = arg.dd.ToDispatcherCode,
                        ToDispatcherId = arg.dd.ToDispatcherId,
                        ToEntrepotId = arg.dd.ToEntrepotId,
                        ToEntrepotName = arg.dd.ToEntrepotName,
                        ToTransportMethodId = arg.dd.ToTransportMethodId,
                        ToTransportMethodName = arg.dd.ToTransportMethodName,
                    }).Where(predicate3);

            totalRecord = query.LongCount();

            return query.OrderBy(x => new { x.Id, x.WalletId })
                    .Skip((currentPage - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToListAsync();
        }


        public Task<long> LongCount(Expression<Func<Dispatcher, bool>> predicate,
            Expression<Func<DispatcherResult, bool>> predicate3)
        {
            return Db.Dispatchers.Where(predicate)
                .Join(Db.DispatcherDetails.Where(x => x.IsDelete == false), i => i.Id, id => id.DispatcherId, (d, dd) => new { d, dd })
                .Join(Db.Wallet, arg => arg.dd.WalletId, p => p.Id,
                    (arg, w) => new DispatcherResult
                    {
                        Id = arg.d.Id, // Id (Primary key)
                        Code = arg.d.Code, // Code (length: 20)
                        FromWarehouseId = arg.d.FromWarehouseId, // FromWarehouseId
                        FromWarehouseIdPath = arg.d.FromWarehouseIdPath, // FromWarehouseIdPath (length: 300)
                        FromWarehouseName = arg.d.FromWarehouseName, // FromWarehouseName (length: 300)
                        FromWarehouseAddress = arg.d.FromWarehouseAddress, // FromWarehouseAddress (length: 500)
                        Status = arg.d.Status, // Status
                        Amount = arg.d.Amount, // Amount
                        TotalWeight = arg.d.TotalWeight, // TotalWeight
                        TotalWeightActual = arg.d.TotalWeightActual, // TotalWeightActual
                        TotalWeightConverted = arg.d.TotalWeightConverted, // TotalWeightConverted
                        TotalVolume = arg.d.TotalVolume, // TotalVolume
                        TotalPackageNo = arg.d.TotalPackageNo, // TotalPackageNo
                        WalletNo = arg.d.WalletNo, // WalletNo
                        PriceType = arg.d.PriceType, // PriceType
                        Price = arg.d.Price, // Price
                        Value = arg.d.Value, // Value
                        UserId = arg.d.UserId, // UserId
                        UserName = arg.d.UserName, // UserName (length: 50)
                        UserFullName = arg.d.UserFullName, // UserFullName (length: 300)
                        Created = arg.d.Created, // Created
                        Updated = arg.d.Updated, // Updated
                        Note = arg.d.Note, // Note (length: 500)
                        UnsignedText = arg.d.UnsignedText, // UnsignedText (length: 500)
                        ForcastDate = arg.d.ForcastDate, // ForcastDate
                        ToWarehouseId = arg.d.ToWarehouseId, // ToWarehouseId
                        ToWarehouseIdPath = arg.d.ToWarehouseIdPath, // ToWarehouseIdPath (length: 300)
                        ToWarehouseName = arg.d.ToWarehouseName, // ToWarehouseName (length: 300)
                        ToWarehouseAddress = arg.d.ToWarehouseAddress, // ToWarehouseAddress (length: 500)
                        TransportPartnerId = arg.d.TransportPartnerId, // TransportPartnerId
                        TransportPartnerName = arg.d.TransportPartnerName, // TransportPartnerName (length: 300)
                        TransportMethodId = arg.d.TransportMethodId, // TransportMethodId
                        TransportMethodName = arg.d.TransportMethodName, // TransportMethodName (length: 300)
                        ContactName = arg.d.ContactName, // ContactName (length: 300)
                        ContactPhone = arg.d.ContactPhone, // ContactPhone (length: 20)
                        EntrepotId = arg.d.EntrepotId,
                        EntrepotName = arg.d.EntrepotName,
                        WalletId = w.Id, // Id (Primary key)
                        WalletCode = w.Code, // Code (length: 20)
                        WalletStatus = w.Status, // Status
                        WalletWidth = w.Width, // Width
                        WalletLength = w.Length, // Length
                        WalletHeight = w.Height, // Height
                        WalletSize = w.Size, // Size (length: 500)
                        WalletTotalWeight = w.TotalWeight, // TotalWeight
                        WalletTotalWeightConverted = w.TotalWeightConverted, // TotalWeightConverted
                        WalletTotalWeightActual = w.TotalWeightActual, // TotalWeightActual
                        WalletTotalVolume = w.TotalVolume, // TotalVolume
                        WalletWeight = w.Weight, // Weight
                        WalletWeightConverted = w.WeightConverted, // WeightConverted
                        WalletWeightActual = w.WeightActual, // WeightActual
                        WalletVolume = w.Volume, // Volume
                        WalletTotalValue = w.TotalValue, // TotalValue
                        WalletPackageNo = w.PackageNo, // PackageNo
                        WalletCreatedWarehouseId = w.CreatedWarehouseId, // CreatedWarehouseId
                        WalletCreatedWarehouseIdPath = w.CreatedWarehouseIdPath, // CreatedWarehouseIdPath (length: 300)
                        WalletCreatedWarehouseName = w.CreatedWarehouseName, // CreatedWarehouseName (length: 300)
                        WalletCreatedWarehouseAddress = w.CreatedWarehouseAddress, // CreatedWarehouseAddress (length: 500)
                        WalletCurrentWarehouseId = w.CurrentWarehouseId, // CurrentWarehouseId
                        WalletCurrentWarehouseIdPath = w.CurrentWarehouseIdPath, // CurrentWarehouseIdPath (length: 300)
                        WalletCurrentWarehouseName = w.CurrentWarehouseName, // CurrentWarehouseName (length: 300)
                        WalletCurrentWarehouseAddress = w.CurrentWarehouseAddress, // CurrentWarehouseAddress (length: 500)
                        WalletTargetWarehouseId = w.TargetWarehouseId, // TargetWarehouseId
                        WalletTargetWarehouseIdPath = w.TargetWarehouseIdPath, // TargetWarehouseIdPath (length: 300)
                        WalletTargetWarehouseName = w.TargetWarehouseName, // TargetWarehouseName (length: 300)
                        WalletTargetWarehouseAddress = w.TargetWarehouseAddress, // TargetWarehouseAddress (length: 500)
                        WalletUserId = w.UserId, // UserId
                        WalletUserName = w.UserName, // UserName (length: 50)
                        WalletUserFullName = w.UserFullName, // UserFullName (length: 300)
                        WalletCreated = w.Created, // Created
                        WalletUpdated = w.Updated, // Updated
                        WalletUnsignedText = w.UnsignedText, // UnsignedText (length: 500)
                        WalletNote = w.Note, // Note (length: 500)
                        WalletOrderCodes = w.OrderCodes, // OrderCodes (length: 1000)
                        WalletPackageCodes = w.PackageCodes, // PackageCodes (length: 1000)
                        WalletCustomers = w.Customers, // Customers (length: 1000)
                        WalletOrderCodesUnsigned = w.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                        WalletPackageCodesUnsigned = w.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                        WalletCustomersUnsigned = w.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                        WalletMode = w.Mode, // Mode
                        WalletPartnerId = w.PartnerId, // PartnerId
                        WalletPartnerName = w.PartnerName, // PartnerName (length: 300)
                        WalletPartnerUpdate = w.PartnerUpdate, // PartnerUpdate
                        FromTransportPartnerId = arg.dd.FromTransportPartnerId,
                        FromTransportPartnerName = arg.dd.FromTransportPartnerName,
                        ToTransportPartnerId = arg.dd.ToTransportPartnerId,
                        ToTransportPartnerName = arg.dd.ToTransportPartnerName,
                        ToTransportPartnerTime = arg.dd.ToTransportPartnerTime,
                        FromDispatcherCode = arg.dd.FromDispatcherCode,
                        FromDispatcherId = arg.dd.FromDispatcherId,
                        FromEntrepotId = arg.dd.FromEntrepotId,
                        FromEntrepotName = arg.dd.FromEntrepotName,
                        FromTransportMethodId = arg.dd.FromTransportMethodId,
                        FromTransportMethodName = arg.dd.FromTransportMethodName,
                        ToDispatcherCode = arg.dd.ToDispatcherCode,
                        ToDispatcherId = arg.dd.ToDispatcherId,
                        ToEntrepotId = arg.dd.ToEntrepotId,
                        ToEntrepotName = arg.dd.ToEntrepotName,
                        ToTransportMethodId = arg.dd.ToTransportMethodId,
                        ToTransportMethodName = arg.dd.ToTransportMethodName,
                    }).Where(predicate3).LongCountAsync();
        }
    }
}

