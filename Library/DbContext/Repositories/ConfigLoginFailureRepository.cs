using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class ConfigLoginFailureRepository : Repository<ConfigLoginFailure>
    {
        public ConfigLoginFailureRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
