using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class HistoryPackageRepository : Repository<HistoryPackage>
    {
        public HistoryPackageRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
