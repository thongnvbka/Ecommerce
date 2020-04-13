using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ComplainRecoupRepository : Repository<ComplainRecoup>
    {
        public ComplainRecoupRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
