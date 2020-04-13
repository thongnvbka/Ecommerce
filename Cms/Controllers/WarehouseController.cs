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
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    public class WarehouseController : BaseController
    {

        #region Xử lý phiếu yêu cầu nhập kho
        // GET: Warehouse
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// POST: /Warehouse/GetAllImportWarehouseList
        /// Lấy danh sách phiếu nhập kho
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetAllImportWarehouseList(int page, int pageSize, ImportWarehouseSearchModal searchModal)
        {
            var importWarehouseModal = new List<ImportWarehouse>();
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ImportWarehouseSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var DateStart = DateTime.Parse(searchModal.DateStart);
                var DateEnd = DateTime.Parse(searchModal.DateEnd);

                importWarehouseModal = await UnitOfWork.ImportWarehouseRepo.FindAsync(
                    out totalRecord,
                    x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.UserId == -1 || x.UserId == searchModal.UserId) && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId) && x.Created >= DateStart && x.Created <= DateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                importWarehouseModal = await UnitOfWork.ImportWarehouseRepo.FindAsync(
                    out totalRecord,
                    x => x.Code.Contains(searchModal.Keyword) && (searchModal.Status == -1 || x.Status == searchModal.Status) && (searchModal.UserId == -1 || x.UserId == searchModal.UserId) && (searchModal.WarehouseId == -1 || x.WarehouseId == searchModal.WarehouseId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, importWarehouseModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Detail phiếu nhập kho
        /// </summary>
        /// <param name="importWarehouseId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetImportWarehouseDetail(int importWarehouseId)
        {
            var result = true;

            var importWarehouseModal = await UnitOfWork.ImportWarehouseRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == importWarehouseId);
            if (importWarehouseModal == null)
            {
                result = false;
            }

            return Json(new { result, importWarehouseModal }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Lấy các thông tin Search trên phiếu nhập kho
        /// </summary>
        /// <returns></returns>
        public JsonResult GetImportWarehouseSearchData()
        {
            var listStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listWarehouse = new List<SearchMeta>();
            var listUser = new List<SearchMeta>();

            // Lấy các trạng thái Status
            foreach (ImportWarehouseStatus importWarehouseStatus in Enum.GetValues(typeof(ImportWarehouseStatus)))
            {
                if (importWarehouseStatus >= 0)
                {
                    listStatus.Add(new { Text = importWarehouseStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)importWarehouseStatus });
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

        #endregion

        #region Danh sách các package hóa    
        #endregion
    }
}