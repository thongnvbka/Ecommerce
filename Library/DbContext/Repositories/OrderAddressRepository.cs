using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderAddressRepository : Repository<OrderAddress>
    {
        public OrderAddressRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

