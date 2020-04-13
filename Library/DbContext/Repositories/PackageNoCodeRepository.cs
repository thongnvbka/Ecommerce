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
    public class PackageNoCodeRepository : Repository<PackageNoCode>
    {
        public PackageNoCodeRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<PackageNoCodeResult>> Search(Expression<Func<PackageNoCode, bool>> predicate, 
            Expression<Func<PackageNoCodeResult, bool>> predicate1, 
            int pageIndex, int recordPerPage, out int totalRecord, string statusPackage)
        {
            var query = Db.PackageNoCodes.Where(predicate)
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false && (statusPackage == "" || statusPackage.Contains(";" + x.Status + ";"))), pnc => pnc.PackageId, p => p.Id,
                    (pnc, p) => new {pnc, p}).Select(x => new PackageNoCodeResult()
                {
                        Id= x.p.Id, // Id (Primary key)
                        Code= x.p.Code, // Code (length: 30)
                        Status= x.p.Status, // Status
                        Note= x.p.Note, // Note (length: 600)
                        OrderId= x.p.OrderId, // OrderId
                        OrderCode= x.p.OrderCode, // OrderCode (length: 50)
                        OrderServices= x.p.OrderServices, // OrderServices (length: 500)
                        CustomerId= x.p.CustomerId, // CustomerId
                        CustomerName= x.p.CustomerName, // CustomerName (length: 300)
                        CustomerUserName= x.p.CustomerUserName, // CustomerUserName (length: 300)
                        CustomerLevelId= x.p.CustomerLevelId, // CustomerLevelId
                        CustomerLevelName= x.p.CustomerLevelName, // CustomerLevelName (length: 300)
                        CustomerWarehouseId= x.p.CustomerWarehouseId, // CustomerWarehouseId
                        CustomerWarehouseAddress= x.p.CustomerWarehouseAddress, // CustomerWarehouseAddress (length: 500)
                        CustomerWarehouseName= x.p.CustomerWarehouseName, // CustomerWarehouseName (length: 300)
                        CustomerWarehouseIdPath= x.p.CustomerWarehouseIdPath, // CustomerWarehouseIdPath (length: 300)
                        TransportCode= x.p.TransportCode, // TransportCode (length: 50)
                        Weight= x.p.Weight, // Weight
                        WeightConverted= x.p.WeightConverted, // WeightConverted
                        WeightActual= x.p.WeightActual, // WeightActual
                        WeightWapperPercent= x.p.WeightWapperPercent, // WeightWapperPercent
                        WeightWapper= x.p.WeightWapper, // WeightWapper
                        TotalPriceWapper= x.p.TotalPriceWapper, // TotalPriceWapper
                        Volume= x.p.Volume, // Volume
                        VolumeActual= x.p.VolumeActual, // VolumeActual
                        VolumeWapperPercent= x.p.VolumeWapperPercent, // VolumeWapperPercent
                        VolumeWapper= x.p.VolumeWapper, // VolumeWapper
                        Size= x.p.Size, // Size (length: 500)
                        Width= x.p.Width, // Width
                        Height= x.p.Height, // Height
                        Length= x.p.Length, // Length
                        TotalPrice= x.p.TotalPrice, // TotalPrice
                        WarehouseId= x.p.WarehouseId, // WarehouseId
                        WarehouseName= x.p.WarehouseName, // WarehouseName (length: 300)
                        WarehouseIdPath= x.p.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseAddress= x.p.WarehouseAddress, // WarehouseAddress (length: 300)
                        UserId= x.p.UserId, // UserId
                        UserFullName= x.p.UserFullName, // UserFullName (length: 300)
                        SystemId= x.p.SystemId, // SystemId
                        SystemName= x.p.SystemName, // SystemName (length: 300)
                        Created= x.p.Created, // Created
                        LastUpdate= x.p.LastUpdate, // LastUpdate
                        HashTag= x.p.HashTag, // HashTag
                        ForcastDate= x.p.ForcastDate, // ForcastDate
                        PackageNo= x.p.PackageNo, // PackageNo
                        UnsignedText= x.p.UnsignedText + " " + x.pnc.UnsignText + " " + x.p.TransportCode, // UnsignedText (length: 500)
                        CurrentLayoutId= x.p.CurrentLayoutId, // CurrentLayoutId
                        CurrentLayoutName= x.p.CurrentLayoutName, // CurrentLayoutName (length: 300)
                        CurrentLayoutIdPath= x.p.CurrentLayoutIdPath, // CurrentLayoutIdPath (length: 300)
                        CurrentWarehouseId= x.p.CurrentWarehouseId, // CurrentWarehouseId
                        CurrentWarehouseName= x.p.CurrentWarehouseName, // CurrentWarehouseName (length: 300)
                        CurrentWarehouseIdPath= x.p.CurrentWarehouseIdPath, // CurrentWarehouseIdPath (length: 300)
                        CurrentWarehouseAddress= x.p.CurrentWarehouseAddress, // CurrentWarehouseAddress (length: 300)
                        OrderCodes= x.p.OrderCodes, // OrderCodes (length: 1000)
                        PackageCodes= x.p.PackageCodes, // PackageCodes (length: 1000)
                        Customers= x.p.Customers, // Customers (length: 1000)
                        OrderCodesUnsigned= x.p.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                        PackageCodesUnsigned= x.p.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                        CustomersUnsigned= x.p.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                        OrderType= x.p.OrderType, // OrderType
                        PackageNoCodeNote = x.pnc.Note, // Note   
                        UnsignText = x.pnc.UnsignText, // UnsignText    
                        PackageNoCodeStatus = x.pnc.Status, // Status 
                        PackageNoCodeMode = x.pnc.Mode, // Mode
                        PackageNoCodeImageJson = x.pnc.ImageJson, // ImageJson
                        PackageNoCodeCreated = x.pnc.Created, // Created
                        PackageNoCodeUpdated = x.pnc.Updated, // Updated
                        CreateUserId = x.pnc.CreateUserId, // CreateUserId
                        CreateUserFullName = x.pnc.CreateUserFullName, // CreateUserFullName (length: 300)
                        CreateUserName = x.pnc.CreateUserName, // CreateUserName (length: 50)
                        CreateOfficeId = x.pnc.CreateOfficeId, // CreateOfficeId
                        CreateOfficeName = x.pnc.CreateOfficeName, // CreateOfficeName (length: 300)
                        CreateOfficeIdPath = x.pnc.CreateOfficeIdPath, // CreateOfficeIdPath (length: 300)
                        UpdateUserId = x.pnc.UpdateUserId, // UpdateUserId
                        UpdateUserFullName = x.pnc.UpdateUserFullName, // UpdateUserFullName (length: 300)
                        UpdateUserName = x.pnc.UpdateUserName, // UpdateUserName (length: 50)
                        UpdateOfficeId = x.pnc.UpdateOfficeId, // UpdateOfficeId
                        UpdateOfficeName = x.pnc.UpdateOfficeName, // UpdateOfficeName (length: 300)
                        UpdateOfficeIdPath = x.pnc.UpdateOfficeIdPath, // UpdateOfficeIdPath (length: 300)
                        PackageNoCodeId = x.pnc.Id,
                        PackageNoCodeCommentNo = x.pnc.CommentNo
                    }).Where(predicate1);

            totalRecord = query.Count();

            return query.OrderByDescending(x => x.Created)
                .Skip((pageIndex - 1)*recordPerPage).Take(recordPerPage)
                .ToListAsync();
        }


        public Task<int> Count(Expression<Func<PackageNoCode, bool>> predicate,
            Expression<Func<PackageNoCodeResult, bool>> predicate1)
        {
            var query = Db.PackageNoCodes.Where(predicate)
                .Join(Db.OrderPackages.Where(x => x.IsDelete == false), pnc => pnc.PackageId, p => p.Id,
                    (pnc, p) => new { pnc, p }).Select(x => new PackageNoCodeResult()
                    {
                        Id = x.p.Id, // Id (Primary key)
                        Code = x.p.Code, // Code (length: 30)
                        Status = x.p.Status, // Status
                        Note = x.p.Note, // Note (length: 600)
                        OrderId = x.p.OrderId, // OrderId
                        OrderCode = x.p.OrderCode, // OrderCode (length: 50)
                        OrderServices = x.p.OrderServices, // OrderServices (length: 500)
                        CustomerId = x.p.CustomerId, // CustomerId
                        CustomerName = x.p.CustomerName, // CustomerName (length: 300)
                        CustomerUserName = x.p.CustomerUserName, // CustomerUserName (length: 300)
                        CustomerLevelId = x.p.CustomerLevelId, // CustomerLevelId
                        CustomerLevelName = x.p.CustomerLevelName, // CustomerLevelName (length: 300)
                        CustomerWarehouseId = x.p.CustomerWarehouseId, // CustomerWarehouseId
                        CustomerWarehouseAddress = x.p.CustomerWarehouseAddress, // CustomerWarehouseAddress (length: 500)
                        CustomerWarehouseName = x.p.CustomerWarehouseName, // CustomerWarehouseName (length: 300)
                        CustomerWarehouseIdPath = x.p.CustomerWarehouseIdPath, // CustomerWarehouseIdPath (length: 300)
                        TransportCode = x.p.TransportCode, // TransportCode (length: 50)
                        Weight = x.p.Weight, // Weight
                        WeightConverted = x.p.WeightConverted, // WeightConverted
                        WeightActual = x.p.WeightActual, // WeightActual
                        WeightWapperPercent = x.p.WeightWapperPercent, // WeightWapperPercent
                        WeightWapper = x.p.WeightWapper, // WeightWapper
                        TotalPriceWapper = x.p.TotalPriceWapper, // TotalPriceWapper
                        Volume = x.p.Volume, // Volume
                        VolumeActual = x.p.VolumeActual, // VolumeActual
                        VolumeWapperPercent = x.p.VolumeWapperPercent, // VolumeWapperPercent
                        VolumeWapper = x.p.VolumeWapper, // VolumeWapper
                        Size = x.p.Size, // Size (length: 500)
                        Width = x.p.Width, // Width
                        Height = x.p.Height, // Height
                        Length = x.p.Length, // Length
                        TotalPrice = x.p.TotalPrice, // TotalPrice
                        WarehouseId = x.p.WarehouseId, // WarehouseId
                        WarehouseName = x.p.WarehouseName, // WarehouseName (length: 300)
                        WarehouseIdPath = x.p.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseAddress = x.p.WarehouseAddress, // WarehouseAddress (length: 300)
                        UserId = x.p.UserId, // UserId
                        UserFullName = x.p.UserFullName, // UserFullName (length: 300)
                        SystemId = x.p.SystemId, // SystemId
                        SystemName = x.p.SystemName, // SystemName (length: 300)
                        Created = x.p.Created, // Created
                        LastUpdate = x.p.LastUpdate, // LastUpdate
                        HashTag = x.p.HashTag, // HashTag
                        ForcastDate = x.p.ForcastDate, // ForcastDate
                        PackageNo = x.p.PackageNo, // PackageNo
                        UnsignedText = x.p.UnsignedText, // UnsignedText (length: 500)
                        CurrentLayoutId = x.p.CurrentLayoutId, // CurrentLayoutId
                        CurrentLayoutName = x.p.CurrentLayoutName, // CurrentLayoutName (length: 300)
                        CurrentLayoutIdPath = x.p.CurrentLayoutIdPath, // CurrentLayoutIdPath (length: 300)
                        CurrentWarehouseId = x.p.CurrentWarehouseId, // CurrentWarehouseId
                        CurrentWarehouseName = x.p.CurrentWarehouseName, // CurrentWarehouseName (length: 300)
                        CurrentWarehouseIdPath = x.p.CurrentWarehouseIdPath, // CurrentWarehouseIdPath (length: 300)
                        CurrentWarehouseAddress = x.p.CurrentWarehouseAddress, // CurrentWarehouseAddress (length: 300)
                        OrderCodes = x.p.OrderCodes, // OrderCodes (length: 1000)
                        PackageCodes = x.p.PackageCodes, // PackageCodes (length: 1000)
                        Customers = x.p.Customers, // Customers (length: 1000)
                        OrderCodesUnsigned = x.p.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                        PackageCodesUnsigned = x.p.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                        CustomersUnsigned = x.p.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                        OrderType = x.p.OrderType, // OrderType
                        PackageNoCodeNote = x.pnc.Note, // Note   
                        UnsignText = x.pnc.UnsignText, // UnsignText    
                        PackageNoCodeStatus = x.pnc.Status, // Status 
                        PackageNoCodeMode = x.pnc.Mode, // Mode
                        PackageNoCodeImageJson = x.pnc.ImageJson, // ImageJson
                        PackageNoCodeCreated = x.pnc.Created, // Created
                        PackageNoCodeUpdated = x.pnc.Updated, // Updated
                        CreateUserId = x.pnc.CreateUserId, // CreateUserId
                        CreateUserFullName = x.pnc.CreateUserFullName, // CreateUserFullName (length: 300)
                        CreateUserName = x.pnc.CreateUserName, // CreateUserName (length: 50)
                        CreateOfficeId = x.pnc.CreateOfficeId, // CreateOfficeId
                        CreateOfficeName = x.pnc.CreateOfficeName, // CreateOfficeName (length: 300)
                        CreateOfficeIdPath = x.pnc.CreateOfficeIdPath, // CreateOfficeIdPath (length: 300)
                        UpdateUserId = x.pnc.UpdateUserId, // UpdateUserId
                        UpdateUserFullName = x.pnc.UpdateUserFullName, // UpdateUserFullName (length: 300)
                        UpdateUserName = x.pnc.UpdateUserName, // UpdateUserName (length: 50)
                        UpdateOfficeId = x.pnc.UpdateOfficeId, // UpdateOfficeId
                        UpdateOfficeName = x.pnc.UpdateOfficeName, // UpdateOfficeName (length: 300)
                        UpdateOfficeIdPath = x.pnc.UpdateOfficeIdPath, // UpdateOfficeIdPath (length: 300)
                        PackageNoCodeCommentNo = x.pnc.CommentNo
                    }).Where(predicate1);

            return query.CountAsync();
        }
    }
}
