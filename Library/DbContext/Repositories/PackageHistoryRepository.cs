using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PackageHistoryRepository : Repository<PackageHistory>
    {
        public PackageHistoryRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
