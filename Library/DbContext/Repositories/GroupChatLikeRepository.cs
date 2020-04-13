using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class GroupChatLikeRepository : Repository<GroupChatLike>
    {
        public GroupChatLikeRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
