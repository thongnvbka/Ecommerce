using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderInfoRepository : Repository<OrderInfo>
    {
        public OrderInfoRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
