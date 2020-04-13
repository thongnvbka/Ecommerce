using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class AccountantSubjectRepository : Repository<AccountantSubject>
    {
        public AccountantSubjectRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
