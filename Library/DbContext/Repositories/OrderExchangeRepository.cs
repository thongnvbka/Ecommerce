using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderExchangeRepository : Repository<OrderExchange>
    {
        public OrderExchangeRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

