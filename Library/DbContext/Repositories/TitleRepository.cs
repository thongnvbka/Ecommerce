using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class TitleRepository : Repository<Title>
    {
        public TitleRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<Title>> Suggettion(int officeId, string keyword, int size)
        {
            return Db.Titles.Where(x => x.UnsignedName.Contains(keyword) && !x.IsDelete && x.Status < 2)
                .GroupJoin(Db.Positions.Where(x => x.OfficeId == officeId), title => title.Id,
                    position => position.TitleId,
                    (title, position) => new {Title = title, Position = position.FirstOrDefault()})
                .Where(x => x.Position == null).Select(x => x.Title)
                .Take(size)
                .ToListAsync();

        }
    }
}
