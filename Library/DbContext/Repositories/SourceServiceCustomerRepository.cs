using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.DbContext.Results;

namespace Library.DbContext.Repositories
{
    public class SourceServiceCustomerRepository : Repository<SourceServiceCustomer>
    {
        public SourceServiceCustomerRepository(ProjectXContext context) : base(context)
        {
        }
    }
}
