using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class RecentRepository : Repository<Recent>
    {
        public RecentRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
