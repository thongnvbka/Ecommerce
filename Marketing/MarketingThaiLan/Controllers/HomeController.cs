using System.Web.Mvc;

namespace MarketingThaiLan.Controllers
{
    // GET: Home
    public class HomeController : Controller
    {
        /// <summary>
        /// MENU HEDER
        /// </summary>
        /// <returns></returns>
        // GET: Home
        public ActionResult Index()//Todo Trang chu
        {
            return View();
        }
        public ActionResult PriceList()//Todo bang gia
        {
            ViewBag.ActivePriceList = "active";
            return View();
        }

        public ActionResult WarehouseInterface()//Todo kho giao dien
        {
            ViewBag.WarehouseInterface = "active";
            return View();
        }
        public ActionResult Help()//todo tro giup
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        public ActionResult Blog()//todo blog
        {
            ViewBag.Blog = "active";
            return View();
        }
        public ActionResult AboutUs()//todo gioi thieu
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }
        public ActionResult ContactUs()//todo gioi thieu
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        /// <summary>
        /// MENU LIST
        /// </summary>
        /// <returns></returns>
        public ActionResult SalientFeatures()//Todo tinh nang noi bat
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        public ActionResult SellBest()//Todo ban hang uu viet
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        public ActionResult ManagerStore()//Todo quan ly cua hang
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }
        public ActionResult CustomTheme()//Todo tuy chinh giao dien
        {
            ViewBag.SalientFeatures = "active";
            return View();
        } 
        public ActionResult MarketingSeo()//Todo marketing seo
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        public ActionResult ManagerProduct()//Todo quan ly san pham
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }

        public ActionResult Security()//Todo bao mat
        {
            ViewBag.SalientFeatures = "active";
            return View();
        }
   
    }
}