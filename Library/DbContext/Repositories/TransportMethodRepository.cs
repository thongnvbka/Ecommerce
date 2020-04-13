using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TransportMethodRepository : Repository<TransportMethod>
    {
        public TransportMethodRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
