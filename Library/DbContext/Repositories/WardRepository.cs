using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class WardRepository : Repository<Ward>
    {
        public WardRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

