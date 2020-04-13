using ProjectV.LikeOrderThaiLan.com.Controllers;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class WarehouseController : BaseController
    {
        // GET: CMS/Warehouse
        //TODO DANH SACH DON HANG LUU KHO
        public ActionResult StorageRequirements()
        {
           
            ViewBag.ActiveStorageRequirements = "cl_on";
            return View();
        }

        //TODO TAO MOI DON HANG LUU KHO
        public ActionResult CreateStorageRequirements()
        {
            
            ViewBag.ActiveStorageRequirements = "cl_on";
            return View();
        }

        //TODO DANH SACH SAN PHAM TRONG KHO

        public ActionResult ListProduct()
        {
            
            ViewBag.ActiveListProduct = "cl_on";
            return View();
        }
    }
}