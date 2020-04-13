using ProjectV.LikeOrderThaiLan.com.Controllers;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class StatisticalController : BaseController
    {
        // GET: CMS/Statistical-thống kê
        //TODO Thống kê đơn hàng mua
        public ActionResult OrdersStatistics()
        { 
            ViewBag.ActiveBuyOrdersStatistics = "cl_on";
            var model = UnitOfWork.CustomerRepo.GetLevel(CustomerState.Id);
            return View(model);
        }

        //TODO thống kê luu kho
        public ActionResult WarehouseStatistics()
        { 
            ViewBag.ActiveWarehouseStatistics = "cl_on";
            return View();
        }

        //TODO Thống kê ships hàng
        public ActionResult DeliveryStatistics()
        { 
            ViewBag.ActiveDeliveryStatistics = "cl_on";
            return View();
        }
    }
}