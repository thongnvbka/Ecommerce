using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cms.Controllers
{
    public class LiabilitiesController : Controller
    {
        // GET: Liabilities
        public ActionResult CustomerLiabilities()
        {
            return View();
        }

        public ActionResult PartnerLiabilities()
        {
            return View();
        }
    }
}