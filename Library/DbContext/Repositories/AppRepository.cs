
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.DbContext.Results;

namespace Library.DbContext.Repositories
{
    public class AppRepository : Repository<App>
    {
        public AppRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<AppResult>> GetAll()
        {
            return Db.Apps.Where(x => !x.IsDelete)
                .Select(x => new AppResult()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Created = x.Created,
                    Description = x.Description,
                    Icon = x.Icon,
                    OrderNo = x.OrderNo,
                    Url = x.Url
                }).ToListAsync();
        }
    }
}
