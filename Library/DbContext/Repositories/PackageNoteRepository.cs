using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PackageNoteRepository : Repository<PackageNote>
    {
        public PackageNoteRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
