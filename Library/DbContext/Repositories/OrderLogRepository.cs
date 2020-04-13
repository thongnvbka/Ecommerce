using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderLogRepository : Repository<OrderLog>
    {
        public OrderLogRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
