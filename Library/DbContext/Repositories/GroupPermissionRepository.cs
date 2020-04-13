using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupPermissionRepository : Repository<GroupPermision>
    {
        public GroupPermissionRepository(ProjectXContext dbContext) : base(dbContext)
        {
    }
}
}
