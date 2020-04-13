using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DispatcherDetailRepository : Repository<DispatcherDetail>
    {
        public DispatcherDetailRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

