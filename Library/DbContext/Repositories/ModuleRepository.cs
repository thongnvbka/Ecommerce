using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Library.DbContext.Results;

namespace Library.DbContext.Repositories
{
    public class ModuleRepository : Repository<Module>
    {
        public ModuleRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<ModuleResult>> GetAll()
        {
            return Db.Modules.Where(x => !x.IsDelete)
                .Select(x => new ModuleResult()
                {
                    AppId = x.AppId,
                    Created = x.Created,
                    Description = x.Description,
                    Icon = x.Icon,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Id = x.Id,
                    ParentId = x.ParentId,
                    ParentName = x.ParentName,
                    Level = x.Level
                }).ToListAsync();
        }

        public Task<List<ModuleResult>> GetByAppId(int appId)
        {
            return Db.Modules.Where(x => !x.IsDelete && x.AppId == appId)
                .Select(x => new ModuleResult()
                {
                    AppId = x.AppId,
                    Created = x.Created,
                    Description = x.Description,
                    Icon = x.Icon,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Id = x.Id,
                    ParentId = x.ParentId,
                    ParentName = x.ParentName,
                    Level = x.Level
                }).ToListAsync();
        }

        public Task<List<ModuleResult>> GetByAppUrl(string url)
        {
            return Db.Modules.Where(x => !x.IsDelete)
                .Join(Db.Apps.Where(app => !app.IsDelete && app.Url.Equals(url)), m => m.AppId, app => app.Id,
                    (x, app) => new ModuleResult()
                    {
                        AppId = x.AppId,
                        Created = x.Created,
                        Description = x.Description,
                        Icon = x.Icon,
                        Name = x.Name,
                        OrderNo = x.OrderNo,
                        Id = x.Id,
                        ParentId = x.ParentId,
                        ParentName = x.ParentName,
                        Level = x.Level
                    }).ToListAsync();
        }
    }
}
