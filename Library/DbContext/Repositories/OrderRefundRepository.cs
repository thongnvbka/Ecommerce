using System;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Library.DbContext.Results;

namespace Library.DbContext.Repositories
{
    public class OrderRefundRepository : Repository<OrderRefund>
    {
        public OrderRefundRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<OrderRefundResult>> Search(byte? status, byte mode, DateTime? fromDate, DateTime? toDate,
            string keyword, int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.OrderRefunds.Where(x => x.IsDelete == false && (status == null || x.Status == status.Value) &&
                                       (((fromDate == null) && (toDate == null))
                                        ||
                                        ((fromDate != null) && (toDate != null) && (x.Created >= fromDate) &&
                                         (x.Created <= toDate))
                                        || ((fromDate == null) && toDate.HasValue && (x.Created <= toDate))
                                        || ((toDate == null) && fromDate.HasValue && (x.Created >= fromDate))) &&
                                       (x.UnsignText.Contains(keyword)) && x.Mode == mode)
                .Join(Db.Orders, x => x.OrderId, x => x.Id, (r, o) => new OrderRefundResult()
                {
                    Id = r.Id,
                    Code = r.Code,
                    OrderId = r.OrderId,
                    LinkNo = r.LinkNo,
                    ProductNo = r.ProductNo,
                    UnsignText = r.UnsignText,
                    Status = r.Status,
                    Mode = r.Mode,
                    Note = r.Note,
                    CreateUserId = r.CreateUserId,
                    CreateUserFullName = r.CreateUserFullName,
                    CreateUserName = r.CreateUserName,
                    CreateOfficeId = r.CreateOfficeId,
                    CreateOfficeName = r.CreateOfficeName,
                    CreateOfficeIdPath = r.CreateOfficeIdPath,
                    UpdateUserId = r.UpdateUserId,
                    UpdateUserFullName = r.UpdateUserFullName,
                    UpdateUserName = r.UpdateUserName,
                    UpdateOfficeId = r.UpdateOfficeId,
                    UpdateOfficeName = r.UpdateOfficeName,
                    UpdateOfficeIdPath = r.UpdateOfficeIdPath,
                    CommentNo = r.CommentNo,
                    Amount = r.Amount,
                    AmountActual = r.AmountActual,
                    TotalAcount = r.TotalAcount,
                    Percent = r.Percent,
                    Created = r.Created,
                    Updated = r.Updated,
                    OrderCode = o.Code,
                    CustomerId = o.CustomerId,
                    CustomerName = o.CustomerName,
                    CustomerEmail = o.CustomerEmail,
                    CustomerPhone = o.CustomerPhone,
                });

            totalRecord = query.LongCount();

            return query.OrderBy(x=> x.Created).Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<OrderRefundResult> GetById(int id)
        {
            var query = Db.OrderRefunds.Where(x => x.IsDelete == false && x.Id == id)
                .Join(Db.Orders, x => x.OrderId, x => x.Id, (r, o) => new OrderRefundResult()
                {
                    Id = r.Id,
                    Code = r.Code,
                    OrderId = r.OrderId,
                    LinkNo = r.LinkNo,
                    ProductNo = r.ProductNo,
                    UnsignText = r.UnsignText,
                    Status = r.Status,
                    Mode = r.Mode,
                    Note = r.Note,
                    CreateUserId = r.CreateUserId,
                    CreateUserFullName = r.CreateUserFullName,
                    CreateUserName = r.CreateUserName,
                    CreateOfficeId = r.CreateOfficeId,
                    CreateOfficeName = r.CreateOfficeName,
                    CreateOfficeIdPath = r.CreateOfficeIdPath,
                    UpdateUserId = r.UpdateUserId,
                    UpdateUserFullName = r.UpdateUserFullName,
                    UpdateUserName = r.UpdateUserName,
                    UpdateOfficeId = r.UpdateOfficeId,
                    UpdateOfficeName = r.UpdateOfficeName,
                    UpdateOfficeIdPath = r.UpdateOfficeIdPath,
                    CommentNo = r.CommentNo,
                    Amount = r.Amount,
                    AmountActual = r.AmountActual,
                    TotalAcount = r.TotalAcount,
                    Percent = r.Percent,
                    Created = r.Created,
                    Updated = r.Updated,
                    OrderCode = o.Code,
                    CustomerId = o.CustomerId,
                    CustomerName = o.CustomerName,
                    CustomerEmail = o.CustomerEmail,
                    CustomerPhone = o.CustomerPhone,
                });

            return query.SingleOrDefaultAsync();
        }
    }
}
