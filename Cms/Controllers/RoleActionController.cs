using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cms.Controllers
{
    [RoutePrefix("role-action")]
    [Authorize]
    public class RoleActionController : BaseController
    {
        [Route("all")]
        public async Task<ActionResult> GetAll()
        {
            var roleAction = await UnitOfWork.RoleActionRepo.GetAll();

            return JsonCamelCaseResult(roleAction, JsonRequestBehavior.AllowGet);
        }
    }
}