using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class MessageUserRepository : Repository<MessageUser>
    {
        public MessageUserRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
