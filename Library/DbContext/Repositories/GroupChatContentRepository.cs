using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupChatContentRepository : Repository<GroupChatContent>
    {
        public GroupChatContentRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
