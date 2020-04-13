using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class LogActionRepository : Repository<LogAction>
    {
        public LogActionRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
