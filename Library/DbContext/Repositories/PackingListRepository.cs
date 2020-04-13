using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PackingListRepository : Repository<PackingList>
    {
        public PackingListRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
