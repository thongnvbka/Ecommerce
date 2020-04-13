using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderServiceRepository : Repository<OrderService>
    {
        public OrderServiceRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
