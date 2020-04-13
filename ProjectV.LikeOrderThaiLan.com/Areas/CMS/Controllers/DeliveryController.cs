using ProjectV.LikeOrderThaiLan.com.Controllers;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class DeliveryController : BaseController
    {
        // GET: CMS/Delivery
   
        public ActionResult CreateDelivery()
        { 
            ViewBag.ActiveCreateDelivery = "cl_on";
            return View();
        }
    }
}