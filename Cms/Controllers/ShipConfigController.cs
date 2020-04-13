using Cms.Attributes;
using Common.Emums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cms.Controllers
{
    [Authorize]
    public class ShipConfigController : BaseController
    {
        // GET: ShipConfig
        [LogTracker(EnumAction.View, EnumPage.ShipPrice)]
        public ActionResult ShipPrice()
        {
            return View();
        }
    }
}