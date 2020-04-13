using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderReasonRepository : Repository<OrderReason>
    {
        public OrderReasonRepository(ProjectXContext context) : base(context)
        {
        }

    }
}
