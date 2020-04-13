using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TrackerRepository : Repository<Tracker>
    {
        public TrackerRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
