using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderHistoryRepository : Repository<OrderHistory>
    {
        public OrderHistoryRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
