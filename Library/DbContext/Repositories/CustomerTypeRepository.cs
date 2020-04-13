
using System;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
   public class CustomerTypeRepository : Repository<CustomerType>
    {
        public CustomerTypeRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }

        public void Update(Customer customer)
        {
            throw new NotImplementedException();
        }
    }
}
