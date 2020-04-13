using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cms.Controllers
{
    [RoutePrefix("app")]
    public class AppController : BaseController
    {
        /// <summary>
        /// Lấy tất cả các App
        /// </summary>
        /// <returns></returns>
        [Route("all")]
        public async Task<ActionResult> GetAll()
        {
            var apps = await UnitOfWork.AppRepo.GetAll();

            return JsonCamelCaseResult(apps, JsonRequestBehavior.AllowGet);
        }
    }
}