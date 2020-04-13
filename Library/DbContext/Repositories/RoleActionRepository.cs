using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.DbContext.Results;

namespace Library.DbContext.Repositories
{
    public class RoleActionRepository : Repository<RoleAction>
    {
        public RoleActionRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<RoleActionResult>> GetAll()
        {
            return Db.RoleActions.Where(x => !x.IsDelete)
                .Select(x => new RoleActionResult() { Description = x.Description, Id = x.Id, Name = x.Name })
                .ToListAsync();
        }
    }
}
