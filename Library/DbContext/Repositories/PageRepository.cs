using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Library.UnitOfWork;

namespace Library.DbContext.Repositories
{
    public class PageRepository : Repository<Page>
    {
        public PageRepository(ProjectXContext context) : base(context)
        {
        }

        public Task<List<PageResult>> GetAll()
        {
            return Db.Pages.Where(x => !x.IsDelete).Select(
                x => new PageResult()
                {
                    AppId = x.AppId,
                    AppName = x.AppName,
                    Created = x.Created,
                    Description = x.Description,
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    ModuleName = x.ModuleName,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Url = x.Url,
                    Icon = x.Icon
                }).ToListAsync();
        }

        public Task<List<PageResult>> GetByAppId(int appId)
        {
            return Db.Pages.Where(x => !x.IsDelete && x.AppId == appId).Select(
                x => new PageResult()
                {
                    AppId = x.AppId,
                    AppName = x.AppName,
                    Created = x.Created,
                    Description = x.Description,
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    ModuleName = x.ModuleName,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Url = x.Url,
                    Icon = x.Icon
                }).ToListAsync();
        }

        public Task<List<PageResult>> GetByModuleId(int moduleId)
        {
            return Db.Pages.Where(x => !x.IsDelete && x.ModuleId == moduleId).Select(
                x => new PageResult()
                {
                    AppId = x.AppId,
                    AppName = x.AppName,
                    Created = x.Created,
                    Description = x.Description,
                    Id = x.Id,
                    ModuleId = x.ModuleId,
                    ModuleName = x.ModuleName,
                    Name = x.Name,
                    OrderNo = x.OrderNo,
                    Url = x.Url,
                    Icon = x.Icon
                }).ToListAsync();
        }
    }
}
