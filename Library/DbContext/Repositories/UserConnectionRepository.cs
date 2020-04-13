using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class UserConnectionRepository : Repository<UserConnection>
    {
        public UserConnectionRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
