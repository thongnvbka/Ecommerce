using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MarketingThaiLan.Controllers
{
    public class MarketingController : Controller
    {
        // GET: Marketing
        public ActionResult Index()
        {
            ViewBag.MarketingIndex = "active";
            return View();
        }
        public ActionResult IndexPromotion()
        {
            ViewBag.IndexPromotion = "active";
            return View();
        }

        public ActionResult DesignWebsite()
        {
            return View();
        }

        public ActionResult SellOnFaceBook()
        {
            return View();
        }
    }
}