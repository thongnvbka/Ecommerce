using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ExportWarehouseDetailRepository : Repository<ExportWarehouseDetail>
    {
        public ExportWarehouseDetailRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<CustomerInfoResult>> GetCustomerHasExportWarehouse(out int totalRecord, bool isManager,
            string warehouseIdPath, int? userId, byte? status, DateTime? fromDate,
            DateTime? toDate, string keyword = "", int currentPage = 1, int recordPerPage = 20)
        {
            var query = Db.ExportWarehouseDetails.Where(x => !x.IsDelete && ((status == null) || (x.Status == status)))
                .Join(Db.ExportWarehouse.Where(x => (((fromDate == null) && (toDate == null))
                                                     ||
                                                     ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) &&
                                                      (x.Created <= toDate))
                                                     || ((fromDate == null) && toDate.HasValue && (x.Created <= toDate))
                                                     ||
                                                     ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate))) &&
                                                    ((isManager &&
                                                      ((x.WarehouseIdPath == warehouseIdPath) ||
                                                       x.WarehouseIdPath.StartsWith(warehouseIdPath + ".")))
                                                     || (!isManager && (x.WarehouseIdPath == warehouseIdPath))) &&
                                                    ((userId == null) || (x.UserId == userId.Value)) &&
                                                    x.UnsignedText.Contains(keyword)), d => d.ExportWarehouseId,
                    e => e.Id, (detail, warehouse) => detail)
                .Join(Db.Customers, e => e.CustomerId, c => c.Id, (detail, customer) => new {detail, customer})
                .Select(
                    x =>
                        new CustomerInfoResult
                        {
                            CustomerId = x.detail.CustomerId,
                            CustomerFullName = x.customer.FullName,
                            CustomerEmail = x.customer.Email,
                            CustomerBalanceAvalible = x.customer.BalanceAvalible,
                            CustomerPhone = x.customer.Phone
                        })
                .Distinct().OrderBy(x=> x.CustomerId);

            totalRecord = query.Count();

            return query.Skip((currentPage - 1)*recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<ExportWarehouseDetailResult>> GetByCustomerIds(string customerIds, byte? status)
        {
            return Db.ExportWarehouseDetails.Where(x => !x.IsDelete &&
                                                        ((status == null) || (x.Status == status)) &&
                                                        customerIds.Contains(";" + x.CustomerId + ";"))
                .Join(Db.Orders, e => e.OrderId, o => o.Id, (detail, order) => new {detail, order})
                .Select(x => new ExportWarehouseDetailResult
                {
                    Id = x.detail.Id,
                    ExportWarehouseId = x.detail.ExportWarehouseId,
                    ExportWarehouseCode = x.detail.ExportWarehouseCode,
                    PackageId = x.detail.PackageId,
                    PackageCode = x.detail.PackageCode,
                    PackageWeight = x.detail.PackageWeight,
                    PackageWeightConverted = x.detail.PackageWeightConverted,
                    PackageWeightActual = x.detail.PackageWeightActual,
                    PackageTransportCode = x.detail.PackageTransportCode,
                    Note = x.detail.Note,
                    PackageSize = x.detail.PackageSize,
                    OrderId = x.detail.OrderId,
                    OrderCode = x.detail.OrderCode,
                    OrderWeight = x.detail.OrderWeight,
                    OrderWeightConverted = x.detail.OrderWeightConverted,
                    OrderWeightActual = x.detail.OrderWeightActual,
                    OrderShip = x.detail.OrderShip,
                    OrderShipActual = x.detail.OrderShipActual,
                    OrderPackageNo = x.detail.OrderPackageNo,
                    OrderTotalPackageNo = x.detail.OrderTotalPackageNo,
                    OrderNote = x.detail.OrderNote,
                    CustomerId = x.detail.CustomerId,
                    CustomerUserName = x.detail.CustomerUserName,
                    CustomerFullName = x.detail.CustomerFullName,
                    CustomerPhone = x.detail.CustomerPhone,
                    CustomerAddress = x.detail.CustomerAddress,
                    CustomerOrderNo = x.detail.CustomerOrderNo,
                    CustomerDistance = x.detail.CustomerDistance,
                    CustomerWeight = x.detail.CustomerWeight,
                    CustomerWeightConverted = x.detail.CustomerWeightConverted,
                    CustomerWeightActual = x.detail.CustomerWeightActual,
                    Created = x.detail.Created,
                    Updated = x.detail.Updated,
                    Status = x.detail.Status,
                    Total = x.order.Total,
                    TotalProductPrice = x.order.TotalExchange,
                    PayedPrice =
                        Db.OrderExchanges.Where(
                                s =>
                                    (s.OrderId == x.order.Id) && (s.Status == (byte) OrderExchangeStatus.Approved) &&
                                    ((s.Mode == (byte) OrderExchangeMode.Export) & !s.IsDelete))
                            .Sum(s => s.TotalPrice ?? 0),
                    TotalServicePrice = Db.OrderServices.Where(s => (s.OrderId == x.order.Id) && s.Checked && !s.IsDelete)
                            .Sum(s => s.TotalPrice)
                }).ToListAsync();
        }
    }
}
