using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class CustomerConfigLevelRepository : Repository<CustomerConfigLevel>
    {
        public CustomerConfigLevelRepository(ProjectXContext dbContext) : base(dbContext)
        {

        }
    }
}