using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class AttachmentMessageRepository : Repository<AttachmentMessage>
    {
        public AttachmentMessageRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
