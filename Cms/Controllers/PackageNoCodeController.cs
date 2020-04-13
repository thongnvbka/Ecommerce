using System.Threading.Tasks;
using System.Web.Mvc;
using System;
using System.Linq;
using Common.Helper;
using Common.Emums;
using System.Linq.Expressions;
using Cms.Attributes;
using Library.DbContext.Entities;
using Library.DbContext.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Cms.Controllers
{
    public class PackageNoCodeController : BaseController
    {
        // GET: PackageNoCode
        [LogTracker(EnumAction.View, EnumPage.PackageNoCode)]
        public ActionResult Index()
        {
            ViewBag.Mode = 0;


            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
               .Cast<OrderType>()
               .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderType>((int)v) })
               .ToList(), jsonSerializerSettings);

            return View();
        }

        [LogTracker(EnumAction.View, EnumPage.PackageNoCodeApprovel)]
        public ActionResult Approvel()
        {
            ViewBag.Mode = 1;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.States = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);

            ViewBag.OrderTypes = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                .Cast<OrderType>()
                .Select(v => new {Id = (byte) v, Name = EnumHelper.GetEnumDescription<OrderType>((int) v)})
                .ToList(), jsonSerializerSettings);
            ViewBag.packageStatuss = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(v => new { Id = (byte)v, Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)v) })
                .ToList(), jsonSerializerSettings);
            return View();
        }

        [CheckPermission(EnumAction.View, EnumPage.PackageNoCode, EnumPage.PackageNoCodeApprovel)]
        public async Task<ActionResult> Search(byte? tabMode, byte? status, string statusPackage, DateTime? fromDate, DateTime? toDate, 
            byte? orderType, string keyword = "", int currentPage = 1, int recordPerPage = 20, bool isFirstRequest = false, byte mode = 0)
        {
            keyword = MyCommon.Ucs2Convert(keyword);

            int totalRecord;

            Expression<Func<PackageNoCode, bool>> queryNoCode = x => x.Mode == mode &&
                    ((fromDate == null && toDate == null)
                        || (fromDate != null && toDate != null && x.Created >= fromDate && x.Created <= toDate)
                        || (fromDate == null && toDate.HasValue && x.Created <= toDate)
                        || (toDate == null && fromDate.HasValue && x.Created >= fromDate)) &&
                     (status == null || x.Status == status.Value);

            Expression<Func<PackageNoCodeResult, bool>> query = x => (tabMode == 0
                         || tabMode == 1 && x.CreateUserId == UserState.UserId
                         || tabMode == 2 && x.UpdateUserId == UserState.UserId) 
                         && (orderType == null || x.OrderType == orderType) && x.UnsignedText.Contains(keyword);

            var items = await UnitOfWork.PackageNoCodeRepo.Search(queryNoCode, query, currentPage, recordPerPage, out totalRecord, statusPackage);

            int allNo;
            int createdNo;
            int approvelNo;

            switch (tabMode)
            {
                case 0:
                    allNo = totalRecord;
                    createdNo = await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => x.CreateUserId == UserState.UserId);
                    approvelNo = await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => x.UpdateUserId == UserState.UserId);
                    break;
                case 1:
                    allNo = await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => true);
                    createdNo = totalRecord;
                    approvelNo =
                        await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => x.UpdateUserId == UserState.UserId);
                    break;
                default:
                    allNo = await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => true);
                    createdNo = await UnitOfWork.PackageNoCodeRepo.Count(queryNoCode, x => x.CreateUserId == UserState.UserId);
                    approvelNo = totalRecord;
                    break;
            }
            // Không phải là lần Request đầu tiên
            if (isFirstRequest == false)
                return JsonCamelCaseResult(new { items, totalRecord, allNo, createdNo, approvelNo },
                    JsonRequestBehavior.AllowGet);

            var packageStatus = Enum.GetValues(typeof(OrderPackageStatus))
                .Cast<OrderPackageStatus>()
                .Select(x => new
                {
                    Id = (byte)x,
                    Name = EnumHelper.GetEnumDescription<OrderPackageStatus>((int)x),
                    Checked = false
                })
                .ToList();
           
            return JsonCamelCaseResult(new { items, totalRecord, allNo, createdNo, approvelNo, packageStatus }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.PackageNoCode, EnumPage.PackageNoCodeApprovel)]
        public async Task<ActionResult> UpdateCommentNo(int id)
        {
            var packageNoCode = await UnitOfWork.PackageNoCodeRepo.SingleOrDefaultAsync(x => x.Id == id);

            if(packageNoCode == null)
                return Json(new { Status = -1, Text = " this package lost code does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            packageNoCode.CommentNo = packageNoCode.CommentNo + 1;

            await UnitOfWork.PackageNoCodeRepo.SaveAsync();

            return Json(new { Status = 1, Text = "Update quatuly of comment to success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.PackageNoCode, EnumPage.PackageNoCodeApprovel)]
        public async Task<ActionResult> Remove(int id)
        {
            var packageNoCode = await UnitOfWork.PackageNoCodeRepo.SingleOrDefaultAsync(x => x.Id == id);

            if (packageNoCode == null)
                return JsonCamelCaseResult(new { Status = -1, Text = " this package lost code does not exist or has been deleted" }, JsonRequestBehavior.AllowGet);

            if (packageNoCode.Status != 0)
                return JsonCamelCaseResult(new { Status = -1, Text = "Unable to delete package for lost code" }, JsonRequestBehavior.AllowGet);

            UnitOfWork.PackageNoCodeRepo.Remove(packageNoCode);

            await UnitOfWork.PackageNoCodeRepo.SaveAsync();

            return JsonCamelCaseResult(new { Status = 1, Text = "Deleted successfully" }, JsonRequestBehavior.AllowGet);
        }
    }
}