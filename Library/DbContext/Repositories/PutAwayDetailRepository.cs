using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PutAwayDetailRepository : Repository<PutAwayDetail>
    {
        public PutAwayDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<PutAwayDetailResult1>> GetByPutawayId(int putAwayId)
        {
            return Db.PutAwayDetails.Where(x => x.IsDelete == false && x.PutAwayId == putAwayId)
                .Join(Db.OrderPackages, pd => pd.PackageId, op => op.Id, (pd, p) => new PutAwayDetailResult1()
                {
                    Id = pd.Id, // Id (Primary key)
                    PutAwayId = pd.PutAwayId, // PutAwayId
                    PutAwayCode = pd.PutAwayCode, // PutAwayCode (length: 30)
                    PackageId = pd.PackageId, // PackageId
                    PackageCode = pd.PackageCode, // PackageCode (length: 50)
                    TransportCode = pd.TransportCode, // TransportCode (length: 50)
                    OrderId = pd.OrderId, // OrderId
                    OrderCode = pd.OrderCode, // OrderCode (length: 50)
                    OrderType = pd.OrderType, // OrderType
                    OrderServices = pd.OrderServices, // OrderServices (length: 500)
                    OrderPackageNo = pd.OrderPackageNo, // OrderPackageNo
                    CustomerId = pd.CustomerId, // CustomerId
                    CustomerName = pd.CustomerName, // CustomerName (length: 300)
                    CustomerUserName = pd.CustomerUserName, // CustomerUserName (length: 300)
                    Note = pd.Note, // Note (length: 500)
                    Status = pd.Status, // Status
                    Length = pd.Length, // Length
                    Weight = pd.Weight, // Weight
                    Width = pd.Width, // Width
                    Size = pd.Size, // Size (length: 500)
                    Height = pd.Height, // Height
                    LayoutId = pd.LayoutId, // LayoutId
                    LayoutName = pd.LayoutName, // LayoutName (length: 300)
                    LayoutIdPath = pd.LayoutIdPath, // LayoutIdPath (length: 300)
                    LayoutNamePath = pd.LayoutNamePath, // LayoutNamePath
                    ConvertedWeight = pd.ConvertedWeight, // ConvertedWeight
                    ActualWeight = pd.ActualWeight, // ActualWeight
                    Created = pd.Created, // Created
                    Updated = pd.Updated, // Updated
                    CustomerWarehouseId = p.CustomerWarehouseId, // CustomerWarehouseId
                    CustomerWarehouseAddress = p.CustomerWarehouseAddress, // CustomerWarehouseAddress (length: 500)
                    CustomerWarehouseName = p.CustomerWarehouseName, // CustomerWarehouseName (length: 300)
                    CustomerWarehouseIdPath = p.CustomerWarehouseIdPath, // CustomerWarehouseIdPath (length: 300)
                }).ToListAsync();
        }
    }
}