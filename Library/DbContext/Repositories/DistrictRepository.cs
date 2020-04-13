using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class DistrictRepository : Repository<District>
    {
        public DistrictRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
