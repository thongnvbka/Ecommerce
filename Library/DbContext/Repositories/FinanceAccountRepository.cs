using Library.DbContext.Entities;
using Library.UnitOfWork;


namespace Library.DbContext.Repositories
{
    public class FinanceAccountRepository : Repository<FinanceAccount>
    {
        public FinanceAccountRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
