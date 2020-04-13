using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Emums;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DebitReportRepository : Repository<DebitReport>
    {
        public DebitReportRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<OrderResult>> GetByOrderAll(Expression<Func<DebitReport, bool>> predic, Expression<Func<Order, bool>> predic1,
    int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.DebitReports
                .Where(predic)
                .Join(Db.Orders.Where(predic1), d => d.OrderId, dd => dd.Id,
                    (report, o) => new OrderResult
                    {
                        Id = o.Id,
                        Type = o.Type,
                        Status = o.Status,
                        Code = o.Code,
                        CustomerPhone = o.CustomerPhone,
                        CustomerEmail = o.CustomerEmail,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName
                    })
                .Distinct().OrderBy(x => x.Id);

            totalRecord = query.LongCount();

            return query.Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<OrderResult>> GetByOrder(Expression<Func<DebitReport, bool>> predic, Expression<Func<Order, bool>> predic1, 
            int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.DebitReports
                .Where(x => x.PackageId == null)
                .Where(predic)
                .Join(Db.Orders.Where(predic1), d => d.OrderId, dd => dd.Id,
                    (report, o) => new OrderResult
                    {
                        Id = o.Id,
                        Type = o.Type,
                        Status = o.Status,
                        Code = o.Code,
                        CustomerPhone = o.CustomerPhone,
                        CustomerEmail = o.CustomerEmail,
                        CustomerId = o.CustomerId,
                        CustomerName = o.CustomerName
                    })
                .Distinct().OrderBy(x => x.Id);

            totalRecord = query.LongCount();

            return query.Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<OrderResult>> GetByPackage(Expression<Func<DebitReport, bool>> predic, Expression<Func<OrderPackage, bool>> predic1,
            int currentPage, int recordPerPage, out long totalRecord)
        {
            var query = Db.DebitReports
                .Where(x => x.PackageId != null)
                .Where(predic)
                .Join(Db.OrderPackages.Where(predic1), d => d.PackageId, dd => dd.Id, (report, p)=> new { report, p})
                .Join(Db.Orders, arg=> arg.p.OrderId, o=> o.Id, (arg, o)=> new OrderResult
                {
                    Id = o.Id,
                    Type = o.Type,
                    Status = o.Status,
                    Code = o.Code,
                    CustomerPhone = o.CustomerPhone,
                    CustomerEmail = o.CustomerEmail,
                    CustomerId = o.CustomerId,
                    CustomerName = o.CustomerName
                })
                .Distinct().OrderBy(x => x.Id);

            totalRecord = query.LongCount();

            return query.Skip((currentPage - 1) * recordPerPage).Take(recordPerPage).ToListAsync();
        }

        public Task<List<PackageForDebitResult>> GetDetailPackage(Expression<Func<DebitReport, bool>> predic, Expression<Func<OrderPackage, bool>> predic1)
        {
            return Db.DebitReports
                .Where(x => x.PackageId != null)
                .Where(predic)
                .Join(Db.OrderPackages.Where(predic1), d => d.PackageId, dd => dd.Id, (report, p) => new PackageForDebitResult
                {
                    Id = p.Id,
                    Code = p.Code,
                    Status = p.Status,
                    OrderId = p.OrderId,
                    OrderCode = p.OrderCode,
                    TransportCode = p.TransportCode,
                }).Distinct().OrderBy(x => x.Id).ToListAsync();
        }

        public Task<Dictionary<int, List<DebitReport>>> GetByOrderQuery(Expression<Func<DebitReport, bool>> predic, 
            Expression<Func<Order, bool>> predic1, string orderIds)
        {
            return Db.DebitReports.Where(x => x.PackageId == null && orderIds.Contains(";"+x.OrderId+";"))
                .Where(predic)
                .Join(Db.Orders.Where(predic1), d => d.OrderId, dd => dd.Id,
                    (report, order) => report)
                .GroupBy(x => x.OrderId)
                .ToDictionaryAsync(x => x.Key, reports => reports.ToList());
        }

        public Task<Dictionary<int, List<DebitReport>>> GetByPackageQuery(Expression<Func<DebitReport, bool>> predic, 
            Expression<Func<OrderPackage, bool>> predic1, string orderIds)
        {
            return Db.DebitReports.Where(x => x.PackageId != null && orderIds.Contains(";" + x.OrderId + ";"))
                .Where(predic)
                .Join(Db.OrderPackages.Where(predic1), d => d.PackageId, dd => dd.Id,
                    (report, package) => report)
                .GroupBy(x => x.OrderId)
                .ToDictionaryAsync(x => x.Key, reports => reports.ToList());
        }

        public Task<Dictionary<int, List<DebitReportPackageResult>>> GetByPackageQuery2(Expression<Func<DebitReport, bool>> predic,
    Expression<Func<OrderPackage, bool>> predic1, string orderIds)
        {
            return Db.DebitReports.Where(x => x.PackageId != null && orderIds.Contains(";" + x.OrderId + ";"))
                .Where(predic)
                .Join(Db.OrderPackages.Where(predic1), d => d.PackageId, dd => dd.Id,
                    (report, package) => new DebitReportPackageResult()
                    {
                        Id = report.Id,
                        PackageId = report.PackageId,
                        PackageCode = report.PackageCode,
                        OrderId = report.OrderId,
                        OrderCode = report.OrderCode,
                        ServiceId = report.ServiceId,
                        Price = report.Price,
                        CustomerId = report.CustomerId,
                        CustomerEmail = report.CustomerEmail,
                        CustomerPhone = report.CustomerPhone,
                        Status = package.Status,
                        TransportCode = package.TransportCode,
                        Weight = package.Weight,
                        WeightConverted = package.WeightConverted,
                        WeightActual = package.WeightActual,
                    })
                .GroupBy(x => x.OrderId)
                .ToDictionaryAsync(x => x.Key, reports => reports.ToList());
        }

        public Task<Dictionary<byte, decimal>> GetByOrderQuery(Expression<Func<Order, bool>> query)
        {
            return Db.DebitReports.Where(x => x.PackageId == null)
                .Join(Db.Orders.Where(query), d => d.OrderId, dd => dd.Id,
                    (report, order) => report)
                .GroupBy(x => x.ServiceId)
                .ToDictionaryAsync(x => x.Key, reports => reports.Sum(x => x.Price));
        }

        public Task<Dictionary<byte, decimal>> GetByPackageQuery(Expression<Func<OrderPackage, bool>> query)
        {
            return Db.DebitReports.Where(x => x.PackageId != null)
                .Join(Db.OrderPackages.Where(query), d => d.PackageId, dd => dd.Id,
                    (report, order) => report)
                .GroupBy(x => x.ServiceId)
                .ToDictionaryAsync(x => x.Key, reports => reports.Sum(x => x.Price));
        }
    }
}
