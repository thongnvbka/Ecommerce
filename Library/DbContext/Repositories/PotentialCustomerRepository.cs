using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PotentialCustomerRepository : Repository<PotentialCustomer>
    {
        public PotentialCustomerRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}