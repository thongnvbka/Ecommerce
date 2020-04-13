using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PositionRepository : Repository<Position>
    {
        public PositionRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<Position>> GetPositionByOfficeIdPath(string officeIdPath)
        {
            return Db.Positions.Join(Db.Offices.Where(office => office.IdPath.StartsWith(officeIdPath + ".")),
                position => position.OfficeId, office => office.Id, (position, office) => position).ToListAsync();
        }
    }
}
