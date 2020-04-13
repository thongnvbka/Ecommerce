using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public partial class CustomerLevelRepository : Repository<CustomerLevel>
    {
        public CustomerLevelRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}