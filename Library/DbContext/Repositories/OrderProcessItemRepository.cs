using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderProcessItemRepository : Repository<OrderProcessItem>
    {
        public OrderProcessItemRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
