using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.DbContext.Results;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;

namespace Cms.Controllers
{
    [RoutePrefix("module")]
    public class ModuleController : BaseController
    {
        /// <summary>
        /// Lấy tất cả các Module
        /// </summary>
        /// <returns></returns>
        [Route("all")]
        public async Task<ActionResult> GetAll()
        {
            var modules = await UnitOfWork.ModuleRepo.GetAll();

            var listModule = new List<ModuleResult>();

            foreach (var m in modules.Where(x=> x.Level == 0))
            {
                listModule.Add(m);
                var childs = modules.Where(x => x.ParentId == m.Id);
                foreach (var c in childs)
                {
                    c.Name = "-" + c.Name;
                }
                listModule.AddRange(childs);
            }

            return JsonCamelCaseResult(listModule, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy Module theo AppId
        /// </summary>
        /// <returns></returns>
        [Route("app-id{appId}")]
        public async Task<ActionResult> GetByAppId(int appId)
        {
            var modules = await UnitOfWork.ModuleRepo.GetByAppId(appId);

            return JsonCamelCaseResult(modules, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy Module theo AppUrl
        /// </summary>
        /// <returns></returns>
        [Route("app-url{appId}")]
        public async Task<ActionResult> GetByAppUrl(string appUrl)
        {
            appUrl = appUrl.ToLower();

            var modules = await UnitOfWork.ModuleRepo.GetByAppUrl(appUrl);

            return JsonCamelCaseResult(modules, JsonRequestBehavior.AllowGet);
        }
    }
}