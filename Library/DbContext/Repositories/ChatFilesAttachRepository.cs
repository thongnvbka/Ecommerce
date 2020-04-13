using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ChatFilesAttachRepository : Repository<ChatFilesAttach>
    {
        public ChatFilesAttachRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
