using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupChatUserRepository : Repository<GroupChatUser>
    {
        public GroupChatUserRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
