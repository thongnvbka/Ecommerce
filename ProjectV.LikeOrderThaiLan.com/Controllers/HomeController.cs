using System.Web.Mvc;
using Common.Translate;

namespace ProjectV.LikeOrderThaiLan.com.Controllers
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// Trang chính
        /// </summary>
        /// <returns></returns>

        public ActionResult Index()
        {
            var tmpTop = 0;
            byte tmpStatus = 2;
            tmpStatus = (byte)Common.Emums.OrderStatus.New;
            var model = UnitOfWork.OrderRepo.GetTopNotifiByLinq(SystemId, 5, CustomerState.Id, tmpStatus, ref tmpTop);
            ViewBag.CountOrderNotifi = tmpTop; 
            return View();
        }

        [Route("{culture}/Home/FAQ")]
        public ActionResult FAQ()
        {
            return View();
        }

        [Route("{culture}/Home/TermsOfService")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [Route("{culture}/Home/Guide")]
        public ActionResult Guide()
        {
            ViewBag.ActiveGuide = "cl_on";
            return View();
        }

        public ActionResult SearchProduct()
        {
            return View();
        }

        public ActionResult Notification()
        {
            return View();
        }



        [HttpPost]
        public string SearchIndex(string keyword, string web)
        {
            Translator t = new Translator();
            string translation = t.Translate(keyword, "Thai", "Chinese");
            string result = "";
            switch (web.ToUpper())
            {
                case "1688.COM":
                    result = string.Format("https://s.1688.com/selloffer/offer_search.htm?keywords={0}", translation);
                    break;
                case "TAOBAO.COM":
                    result = string.Format("https://world.taobao.com/search/search.htm?_ksTS=1480480097245_41&spm=a21bp.7806943.a214x9l.1.n0Fubf&_input_charset=utf-8&navigator=all&json=on&q={0}&callback=__jsonp_cb&cna=RreMEFkQJBwCAXZGdLRuJDeu&abtest=_AB-LR517-LR854-LR895-PR517-PR854-PR895", translation);
                    break;
                case "TMALL.COM":
                    result = string.Format("https://list.tmall.com/search_product.htm?q={0}&click_id={0}&from=mallfp..pc_1.0_hq&spm=875.7931836%2FA.a1z5h.1.36s4mz", translation);
                    break;
            }
            return result;
        }


 
    }
}