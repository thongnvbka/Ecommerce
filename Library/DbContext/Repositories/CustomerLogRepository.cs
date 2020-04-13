using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class CustomerLogRepository : Repository<CustomerLogRepository>
    {
        public CustomerLogRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
