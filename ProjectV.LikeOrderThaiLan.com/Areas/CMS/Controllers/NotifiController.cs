using ProjectV.LikeOrderThaiLan.com.Controllers;
using ResourcesLikeOrderThaiLan;
using System.Web.Mvc;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    public class NotifiController : BaseController
    {
        // GET: CMS/Notifi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OrderNotifiTop()
        {
            var tmpTop = 0;
            var model = UnitOfWork.NotificationRepo.GetTopNewByLinq(SystemId, 5, CustomerState.Id, ref tmpTop);
            ViewBag.CountNotifi = tmpTop;
            return PartialView(model);
        }

        public ActionResult NotifiTopCommon()
        {
            var tmpTop = 0;
            var model = UnitOfWork.NotifiCommonRepo.GetTopNewByLinq(SystemId, 5, CustomerState.Id, ref tmpTop);
            ViewBag.CountNotifiCommon = tmpTop;
            return PartialView(model);
        }

        public void UpdateNotifiTopCommon()
        {
            UnitOfWork.NotifiCommonRepo.UpdateTopNewByLinq(SystemId, CustomerState.Id);
        }

        public ActionResult NotifiTopOrder()
        {
            var tmpTop = 0;
            byte tmpStatus = 2;
            tmpStatus = (byte)Common.Emums.OrderStatus.New;
            var model = UnitOfWork.OrderRepo.GetTopNotifiByLinq(SystemId, 5, CustomerState.Id, tmpStatus, ref tmpTop);
            ViewBag.CountOrderNotifi = tmpTop;
            return PartialView(model);
        }

        public void UpdateNotifiTopOrder()
        {
            UnitOfWork.NotificationRepo.UpdateTopNewByLinq(SystemId, CustomerState.Id);
        }

        public string GetWallet()
        {
            var result = "";
            var tmpCustomer = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == CustomerState.Id);
            if (tmpCustomer != null)
            {
                result = $"{tmpCustomer.BalanceAvalible: ###,###}" + " " + Resource.Currency;
            }
            else
            {
                result = "0" + " " + Resource.Currency;
            }
            return result;
        }
    }
}