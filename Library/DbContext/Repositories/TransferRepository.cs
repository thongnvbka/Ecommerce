using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TransferRepository : Repository<Transfer>
    {
        public TransferRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
