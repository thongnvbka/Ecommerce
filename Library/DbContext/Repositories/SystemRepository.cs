using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class SystemRepository : Repository<Entities.System>
    {
        public SystemRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

