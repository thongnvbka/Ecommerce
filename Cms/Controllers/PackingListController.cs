using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.DbContext.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using Library.ViewModels;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using Cms.Attributes;
using Library.UnitOfWork;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class PackingListController : BaseController
    {
        // GET: Packing
        [LogTracker(EnumAction.View, EnumPage.PackingList)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetAllPackingList(int page, int pageSize, PackingListSearchModal searchModal)
        {
            var packingListModal = new List<PackingList>();
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new PackingListSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var DateStart = DateTime.Parse(searchModal.DateStart);
                var DateEnd = DateTime.Parse(searchModal.DateEnd);

                packingListModal = await UnitOfWork.PackingListRepo.FindAsync(
                    out totalRecord,
                    x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.UserId == -1 || x.UserId == searchModal.UserId) && (searchModal.WarehouseSourceId == -1 || x.WarehouseSourceId == searchModal.WarehouseSourceId) && (searchModal.WarehouseDesId == -1 || x.WarehouseDesId == searchModal.WarehouseDesId) && x.Created >= DateStart && x.Created <= DateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                packingListModal = await UnitOfWork.PackingListRepo.FindAsync(
                    out totalRecord,
                    x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.UserId == -1 || x.UserId == searchModal.UserId) && (searchModal.WarehouseSourceId == -1 || x.WarehouseSourceId == searchModal.WarehouseSourceId) && (searchModal.WarehouseDesId == -1 || x.WarehouseDesId == searchModal.WarehouseDesId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, packingListModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetPackingListDetail(int packingListId)
        {
            var result = true;

            var packingModal = await UnitOfWork.PackingListRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == packingListId);
            if (packingModal == null)
            {
                result = false;
            }

            return Json(new { result, packingModal }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPackingListSearchData()
        {
            var listStatus = new List<dynamic>() { new { Text = "All", Value = -1 } };
            var listWarehouse = new List<SearchMeta>();
            var listUser = new List<SearchMeta>();

            // Lấy các trạng thái Status
            foreach (PackingListStatus packingListStatus in Enum.GetValues(typeof(PackingListStatus)))
            {
                if (packingListStatus >= 0)
                {
                    listStatus.Add(new { Text = packingListStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)packingListStatus });
                }
            }

            // Lấy danh sách các kho
            var warehouse = UnitOfWork.WarehouseRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempWarehouseList = from p in warehouse
                                    select new SearchMeta() { Text = p.Name, Value = p.Id };
            listWarehouse.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listWarehouse.AddRange(tempWarehouseList.ToList());

            // Lấy danh sách người tạo phiếu - Danh sách nhân viên trong công ty
            var user = UnitOfWork.UserRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempUserList = from p in user
                               select new SearchMeta() { Text = p.FullName, Value = p.Id };

            listUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listUser.AddRange(tempUserList.ToList());

            return Json(new { listStatus, listWarehouse, listUser }, JsonRequestBehavior.AllowGet);
        }
    }

    
}