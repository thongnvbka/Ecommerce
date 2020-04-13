using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class WalletDetailRepository : Repository<WalletDetail>
    {
        public WalletDetailRepository(ProjectXContext context) : base(context)
        {

        }


        public Task<List<WalletDetailResult>> Search(int walletId)
        {
            return Db.WalletDetails.Where(x => x.IsDelete == false && x.WalletId == walletId)
                .Join(Db.OrderPackages, d => d.PackageId, p => p.Id, (d, p) => new WalletDetailResult
                {
                    Id = d.Id, // Id (Primary key)
                    WalletId = d.WalletId, // WalletId
                    WalletCode = d.WalletCode, // WalletCode (length: 30)
                    PackageId = d.PackageId, // PackageId
                    PackageCode = d.PackageCode, // packageCode (length: 50)
                    OrderId = d.OrderId, // OrderId
                    OrderCode = d.OrderCode, // OrderCode (length: 50)
                    OrderType = d.OrderType, // OrderType
                    OrderServices = d.OrderServices, // OrderServices (length: 500)
                    OrderPackageNo = d.OrderPackageNo, // OrderPackageNo
                    Amount = d.Amount, // Amount
                    TransportCode = d.TransportCode, // TransportCode (length: 50)
                    Note = d.Note, // Note (length: 500)
                    Status = d.Status, // Status
                    Weight = d.Weight, // Weight
                    Volume = d.Volume, // Volume
                    ConvertedWeight = d.ConvertedWeight, // ConvertedWeight
                    ActualWeight = d.ActualWeight, // ActualWeight
                    Created = d.Created, // Created
                    Updated = d.Updated, // Updated
                    OrderCodes = d.OrderCodes, // OrderCodes (length: 1000)
                    PackageCodes = d.PackageCodes, // PackageCodes (length: 1000)
                    Customers = d.Customers, // Customers (length: 1000)
                    OrderCodesUnsigned = d.OrderCodesUnsigned, // OrderCodesUnsigned (length: 1000)
                    PackageCodesUnsigned = d.PackageCodesUnsigned, // PackageCodesUnsigned (length: 1000)
                    CustomersUnsigned = d.CustomersUnsigned, // CustomersUnsigned (length: 1000)
                    CustomerName = p.CustomerName, // CustomerName (length: 300)
                    CustomerUserName = p.CustomerUserName, // CustomerUserName (length: 300)
                    CustomerLevelId = p.CustomerLevelId, // CustomerLevelId
                    CustomerLevelName = p.CustomerLevelName, // CustomerLevelName (length: 300)
                    CustomerWarehouseId = p.CustomerWarehouseId, // CustomerWarehouseId
                    CustomerWarehouseAddress = p.CustomerWarehouseAddress, // CustomerWarehouseAddress (length: 500)
                    CustomerWarehouseName = p.CustomerWarehouseName, // CustomerWarehouseName (length: 300)
                    CustomerWarehouseIdPath = p.CustomerWarehouseIdPath, // CustomerWarehouseIdPath (length: 300)
                    WeightWapper = p.WeightWapper
                }).OrderBy(x => new {x.OrderId, x.PackageId}).ToListAsync();
        }
    }
}
