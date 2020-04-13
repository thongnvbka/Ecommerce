
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupChatReadRepository : Repository<GroupChatRead>
    {
        public GroupChatReadRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
