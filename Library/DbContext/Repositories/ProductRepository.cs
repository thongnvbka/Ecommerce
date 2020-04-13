using Library.DbContext.Results;
using Library.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Library.DbContext.Repositories
{
    public class ProductRepository : Repository<ProductResult>
    {
        public ProductRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<ProductResult>> GetListAsync(out long totalRecord, Expression<Func<ProductResult, bool>> filter = null, Func<IQueryable<ProductResult>, IQueryable<ProductResult>> orderBy = null, int currentPage = 1,
            int pageSize = 20)
        {
            var list = Db.OrderDetails.GroupBy(x => x.Link).Select(x => new ProductResult
            {
                Id = x.FirstOrDefault().Id,
                Name = x.FirstOrDefault().Name,
                Images = x.Select(y => y.Image).Distinct().ToList(),
                Quantity = x.Sum(y => y.Quantity),
                Link = x.FirstOrDefault().Link,
                Properties = x.Select(y => y.Properties).Distinct().ToList(),
                Created = x.OrderBy(y => y.Created).FirstOrDefault().Created,
                LastUpdate = x.OrderByDescending(y => y.Created).FirstOrDefault().Created,
                CategoryId = x.FirstOrDefault().CategoryId,
                CategoryName = x.FirstOrDefault().CategoryName
            });

            IQueryable<ProductResult> query = list;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            totalRecord = query.LongCount();

            if (orderBy == null) return query.ToListAsync();

            query = orderBy(query);

            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 20;
            }

            return query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        }
    }
}
