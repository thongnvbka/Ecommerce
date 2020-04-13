using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class LayoutRepository : Repository<Layout>
    {
        public LayoutRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
