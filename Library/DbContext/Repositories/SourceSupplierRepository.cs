using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class SourceSupplierRepository : Repository<SourceSupplier>
    {
        public SourceSupplierRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
