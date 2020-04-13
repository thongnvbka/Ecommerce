using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PayReceivableRepository : Repository<PayReceivable>
    {
        public PayReceivableRepository(ProjectXContext dbContext) : base(dbContext)
        {
        }
        public int CountPayReceivables(int id)
        {
            return Db.PayReceivables.Where(t => t.ParentId == id).Count();
        }
        public Task<List<PayReceivable>> GetPayReceivableList()
        {
            return Db.PayReceivables.ToListAsync();
        }
        public bool Operator(int id)
        {
            var x = Db.PayReceivables.SingleOrDefault(d => d.Id == id).Operator;
            return (bool)x;
        }
    }
}
