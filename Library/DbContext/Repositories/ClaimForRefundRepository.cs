using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ClaimForRefundRepository : Repository<ClaimForRefund>
    {
        public ClaimForRefundRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
