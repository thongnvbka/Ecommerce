using System.Web.Mvc;
using Cms.Attributes;
using Common.Emums;
using Library.Models;
using Library.Settings;
using Library.UnitOfWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        // GET: Setting
        [LogTracker(EnumAction.View, EnumPage.Setting)]
        public ActionResult Index()
        {
            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.Setting)]
        public ActionResult GetSetting(byte officeType)
        {
            var settingProvider = new SettingProvider<NotifySetting>($"OfficeType_{officeType}");

            return JsonCamelCaseResult(settingProvider.Settings, JsonRequestBehavior.AllowGet);
        }

        [CheckPermission(EnumAction.Update, EnumPage.Setting)]
        public ActionResult SaveNotifySetting(NotifySettingMeta model)
        {
            if (!ModelState.IsValid)
                return JsonCamelCaseResult(new { Status = -1, Text = "Data format is incorrect" },//*Data format is incorrect*/
                    JsonRequestBehavior.AllowGet);

            var settingProvider = new SettingProvider<NotifySetting>($"OfficeType_{model.OfficeType}");

            settingProvider.SaveSettings(new NotifySetting
            {
                IsFollow = model.IsFollow,
                UsersString = JsonConvert.SerializeObject(model.Users, new JsonSerializerSettings(){ ContractResolver = new CamelCasePropertyNamesContractResolver()})
            });

            return JsonCamelCaseResult(new {Status = 1, Text = "successfully saved" }, JsonRequestBehavior.AllowGet);//Luu thanh cong
        }
    }
}