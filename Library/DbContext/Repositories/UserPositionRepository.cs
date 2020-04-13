using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class UserPositionRepository : Repository<UserPosition>
    {
        public UserPositionRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
