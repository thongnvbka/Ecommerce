using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ClaimForRefundDetailRepository : Repository<ClaimForRefundDetail>
    {
        public ClaimForRefundDetailRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
