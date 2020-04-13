using ProjectV.LikeOrderThaiLan.com.Controllers;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    public class GuideController : BaseController
    {
        // GET: CMS/Guide
        public ActionResult Guide()
        {
          
            ViewBag.ActiveGuide = "cl_on";
            return View();
        }
    }
}