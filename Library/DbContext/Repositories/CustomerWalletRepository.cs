using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class CustomerWalletRepository : Repository<CustomerWallet>
    {
        public CustomerWalletRepository(ProjectXContext context) : base(context)
        {
        }
        public int CountCustomerWallet(int id)
        {
            return Db.CustomerWallets.Where(t => t.ParentId == id).Count();
        }
        public Task<List<CustomerWallet>> GetCustomerWalletList()
        {
            return Db.CustomerWallets.ToListAsync();
        }
        public bool Operator(int id)
        {
            var x = Db.CustomerWallets.SingleOrDefault(d => d.Id == id).Operator;
            return (bool)x;
        }
    }
}
