using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderServiceOtherRepository : Repository<OrderServiceOther>
    {
        public OrderServiceOtherRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<OrderServiceOtherResult>> Search(out long totalRecord, bool isManager, string warehouseIdPath, byte? mode, int? systemId, byte? orderType,
            DateTime? fromDate, DateTime? toDate, string keyword = "", int currentPage = 1,
           int recordPerPage = 20)
        {
            var query = Db.OrderServiceOthers.Where(x => x.UnsignText.Contains(keyword) &&
                                                         (mode == null || x.Mode == mode) &&
                                                         //(isManager && (x.CreatedOfficeIdPath == warehouseIdPath
                                                         // || x.CreatedOfficeIdPath.StartsWith(warehouseIdPath + "."))
                                                         // || !isManager && x.CreatedOfficeIdPath == warehouseIdPath) &&
                                                         (fromDate == null && toDate == null
                                                          || fromDate != null && toDate != null && x.Created >= fromDate && 
                                                              x.Created <= toDate || fromDate == null && 
                                                              toDate.HasValue && x.Created <= toDate
                                                          || toDate == null && fromDate.HasValue && x.Created >= fromDate))
                .Join(Db.Orders.Where(x => x.IsDelete == false && (systemId == null || x.SystemId == systemId) &&
                                           (orderType == null || x.Type == orderType)),
                    x => x.OrderId, x => x.Id, (other, order) => new
                        OrderServiceOtherResult
                        {
                            Id = other.Id,
                            OrderId = other.OrderId,
                            OrderType = order.Type,
                            OrderCode = other.OrderCode,
                            ExchangeRate = other.ExchangeRate,
                            Currency = other.Currency,
                            Value = other.Value,
                            TotalPrice = other.TotalPrice,
                            Mode = other.Mode,
                            ObjectId = other.ObjectId,
                            Type = other.Type,
                            Note = other.Note,
                            Created = other.Created,
                            CreatedUserId = other.CreatedUserId,
                            CreatedUserFullName = other.CreatedUserFullName,
                            CreatedUserUserName = other.CreatedUserUserName,
                            CreatedUserTitleId = other.CreatedUserTitleId,
                            CreatedUserTitleName = other.CreatedUserTitleName,
                            CreatedOfficeId = other.CreatedOfficeId,
                            CreatedOfficeName = other.CreatedOfficeName,
                            CreatedOfficeIdPath = other.CreatedOfficeIdPath,
                            PackageNo = other.PackageNo,
                            TotalWeightActual = other.TotalWeightActual,
                            PackageCodes = other.PackageCodes,
                            DataJson = other.DataJson,
                        });

            totalRecord = query.LongCount();

            return query.OrderByDescending(x => new {x.Id})
                .Skip((currentPage - 1) * recordPerPage)
                .Take(recordPerPage)
                .ToListAsync();
        }
    }
}
