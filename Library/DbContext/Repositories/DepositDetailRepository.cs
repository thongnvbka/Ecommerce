using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DepositDetailRepository : Repository<DepositDetail>
    {
        public DepositDetailRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
