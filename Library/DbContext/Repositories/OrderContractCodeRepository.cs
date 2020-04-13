using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class OrderContractCodeRepository : Repository<OrderContractCode>
    {
        public OrderContractCodeRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
