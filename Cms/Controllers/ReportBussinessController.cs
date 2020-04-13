using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cms.Controllers
{
    public class ReportBussinessController : Controller
    {
        // GET: Bussiness
        public ActionResult Customer()
        {
            return View();
        }

        public ActionResult Staff()
        {
            return View();
        }
    }
}