using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class NotifiCustomerRepository : Repository<NotifiCustomer>
    {
        public NotifiCustomerRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
