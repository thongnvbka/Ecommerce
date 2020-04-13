using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ProvinceRepository : Repository<Province>
    {
        public ProvinceRepository(ProjectXContext context) : base(context)
        {
        }
    }
}

