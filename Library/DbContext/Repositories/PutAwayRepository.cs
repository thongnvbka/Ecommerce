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
    public class PutAwayRepository : Repository<PutAway>
    {
        public PutAwayRepository(ProjectXContext context) : base(context)
        {
        }

        public object CountByStatus(Expression<Func<PutAway, bool>> query)
        {
            return Entities.Where(query).GroupBy(p => p.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToDictionary(k => k.Status, i => i.Count);
        }

        public Task<List<PutAwayResult>> Search(Expression<Func<PutAway, bool>> predicate,
            Expression<Func<PutAwayResult, bool>> predicate3, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.PutAways.Where(predicate)
                .Join(Db.PutAwayDetails.Where(x => x.IsDelete == false), i => i.Id, id => id.PutAwayId, (put, impD) => new { put, impD })
                .Join(Db.OrderPackages, arg => arg.impD.PackageId, p => p.Id,
                    (arg, p) => new PutAwayResult
                    {
                        Id = arg.put.Id, // Id (Primary key)
                        Code = arg.put.Code, // Code (length: 20)
                        Status = arg.put.Status, // Status
                        TotalWeight = arg.put.TotalWeight, // TotalWeight
                        TotalActualWeight = arg.put.TotalActualWeight, // TotalActualWeight
                        TotalConversionWeight = arg.put.TotalConversionWeight, // TotalConversionWeight
                        PackageNo = arg.put.PackageNo, // PackageNo
                        WarehouseId = arg.put.WarehouseId, // WarehouseId
                        WarehouseIdPath = arg.put.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseName = arg.put.WarehouseName, // WarehouseName (length: 300)
                        WarehouseAddress = arg.put.WarehouseAddress, // WarehouseAddress (length: 500)
                        UserId = arg.put.UserId, // UserId
                        UserName = arg.put.UserName, // UserName (length: 50)
                        UserFullName = arg.put.UserFullName, // UserFullName (length: 300)
                        Created = arg.put.Created, // Created
                        Updated = arg.put.Updated, // Updated
                        UnsignedText = arg.put.UnsignedText, // UnsignedText (length: 500)
                        Note = arg.put.Note, // Note (length: 500)
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

            return query.OrderByDescending(x => new { x.Id, x.PackageId })
                    .Skip((currentPage - 1) * recordPerPage)
                    .Take(recordPerPage)
                    .ToListAsync();
        }
    }
}