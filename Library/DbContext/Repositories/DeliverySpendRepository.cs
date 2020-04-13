using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DeliverySpendRepository : Repository<DeliverySpend>
    {
        public DeliverySpendRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

