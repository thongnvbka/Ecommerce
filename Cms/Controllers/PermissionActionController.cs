using System.Threading;
using Common.Helper;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cms.Controllers
{
    [RoutePrefix("permission-action")]
    [Authorize]
    public class PermissionActionController : BaseController
    {
        /// <summary>
        /// Lấy tất cả các Page
        /// </summary>
        /// <returns></returns>
        [Route("by-page-{pageId}")]
        [HttpGet]
        public async Task<ActionResult> GetByPageId(int pageId)
        {
            var permissionActions = await UnitOfWork.PermissionActionRepo.FindAsync(
                x => !x.IsDelete && x.PageId == pageId && x.GroupPermisionId == null);

            return JsonCamelCaseResult(permissionActions, JsonRequestBehavior.AllowGet);
        }

        [Route("get-by-permission")]
        [HttpGet]
        public async Task<ActionResult> GetByPermissionId(int permissionId, string keyword = "")
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            var items = await UnitOfWork.PermissionActionRepo.FindAsync(
                x => !x.IsDelete && x.GroupPermisionId == permissionId && keyword.Contains(keyword));

            var actions = await UnitOfWork.RoleActionRepo.GetAll();

            return JsonCamelCaseResult(new { items, actions }, JsonRequestBehavior.AllowGet);
        }
    }
}