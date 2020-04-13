using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ImportWarehouseDetailRepository : Repository<ImportWarehouseDetail>
    {
        public ImportWarehouseDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<ImportWarehouseDetailResult>> GetByImportWarehouseId(int importWarehouseId)
        {
            return Db.ImportWarehouseDetails.Where(x => x.IsDelete == false && x.ImportWarehouseId == importWarehouseId)
                .OrderBy(x => x.OrderCode)
                .Select(x => new ImportWarehouseDetailResult()
                {
                    Id = x.Id,
                    ImportWarehouseId = x.ImportWarehouseId,
                    ImportWarehouseCode = x.ImportWarehouseCode,
                    CustomerId = x.CustomerId,
                    CustomerName = x.CustomerName,
                    CustomerUserName = x.CustomerUserName,
                    Type = x.Type,
                    PackageId = x.PackageId,
                    PackageCode = x.PackageCode,
                    OrderPackageNo = x.OrderPackageNo,
                    OrderServices = x.OrderServices,
                    Note = x.Note,
                    OrderCode = x.OrderCode,
                    OrderId = x.OrderId,
                    OrderType = x.OrderType,
                    TransportCode = x.TransportCode,
                    Status = x.Status,
                    WarehouseIdPath = x.WarehouseIdPath,
                    WarehouseName = x.WarehouseName,
                    WarehouseAddress = x.WarehouseAddress,
                    WarehouseId = x.WarehouseId,
                    IsDelete = x.IsDelete,
                    Created = x.Created,
                    Updated = x.Updated,
                }).ToListAsync();
        }

        public Task<List<ImportWarehouseDetailWalletResult>> SearchWallet(int importWarehouseId)
        {
            return Db.ImportWarehouseDetails.Where(
                    x => x.ImportWarehouseId == importWarehouseId && x.IsDelete == false && x.Type == 1)
                .Join(Db.Wallet, detail => detail.PackageId, wallet => wallet.Id, (d, w) =>
                    new ImportWarehouseDetailWalletResult
                    {
                        Id = d.Id, // Id (Primary key)
                        ImportWarehouseId = d.ImportWarehouseId, // ImportWarehouseId
                        ImportWarehouseCode = d.ImportWarehouseCode, // ImportWarehouseCode (length: 30)
                        CustomerId = d.CustomerId, // CustomerId
                        CustomerName = d.CustomerName, // CustomerName (length: 300)
                        CustomerUserName = d.CustomerUserName, // CustomerUserName (length: 300)
                        Type = d.Type, // Type
                        PackageId = d.PackageId, // PackageId
                        PackageCode = d.PackageCode, // packageCode (length: 50)
                        OrderPackageNo = d.OrderPackageNo, // OrderPackageNo
                        OrderServices = d.OrderServices, // OrderServices (length: 500)
                        Note = d.Note, // Note (length: 500)
                        OrderCode = d.OrderCode, // OrderCode (length: 50)
                        OrderId = d.OrderId, // OrderId
                        OrderType = d.OrderType, // OrderType
                        TransportCode = d.TransportCode, // TransportCode (length: 50)
                        Status = d.Status, // Status
                        WarehouseIdPath = d.WarehouseIdPath, // WarehouseIdPath (length: 300)
                        WarehouseName = d.WarehouseName, // WarehouseName (length: 300)
                        WarehouseAddress = d.WarehouseAddress, // WarehouseAddress (length: 300)
                        WarehouseId = d.WarehouseId, // WarehouseId
                        Created = d.Created, // Created
                        Updated = d.Updated, // Updated
                        Width = w.Width, // Width
                        Length = w.Length, // Length
                        Height = w.Height, // Height
                        Size = w.Size, // Size (length: 500)
                        Weight = w.Weight, // Weight
                        WeightConverted = w.WeightConverted, // WeightConverted
                        WeightActual = w.WeightActual, // WeightActual
                        Volume = w.Volume, // Volume
                        TotalValue = w.TotalValue, // TotalValue
                        PackageNo = w.PackageNo // PackageNo
                    }).ToListAsync();
        }
    }
}

