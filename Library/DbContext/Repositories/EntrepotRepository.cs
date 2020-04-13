using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class EntrepotRepository : Repository<Entrepot>
    {
        public EntrepotRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
