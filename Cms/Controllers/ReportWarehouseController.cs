using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cms.Controllers
{
    public class ReportWarehouseController : Controller
    {
        // GET: ReportWarehouse
        public ActionResult Product()
        {
            return View();
        }

        public ActionResult Inventory()
        {
            return View();
        }

        public ActionResult Import()
        {
            return View();
        }

        public ActionResult Export()
        {
            return View();
        }

        public ActionResult FlowCoordinator()
        {
            return View();
        }
    }
}