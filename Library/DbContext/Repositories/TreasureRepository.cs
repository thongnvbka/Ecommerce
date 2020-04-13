using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TreasureRepository : Repository<Treasure>
    {
        public TreasureRepository(ProjectXContext context) : base(context)
        {
        }
        public int CountTreasure(int id)
        {
            return Db.Treasures.Where(t => t.ParentId==id).Count();
        }
        public Task<List<Treasure>> GetTreasureList()
        {
            return Db.Treasures.ToListAsync();
        }
        public bool Operator(int id)
        {
            var x = Db.Treasures.SingleOrDefault(d => d.Id == id).Operator;
            return (bool)x;
        }
        
    }
}
