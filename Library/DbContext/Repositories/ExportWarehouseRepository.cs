using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ExportWarehouseRepository : Repository<ExportWarehouse>
    {
        public ExportWarehouseRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
    }
}
