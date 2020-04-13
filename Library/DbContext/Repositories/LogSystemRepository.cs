using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class LogSystemRepository : Repository<LogSystem>
    {
        public LogSystemRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
