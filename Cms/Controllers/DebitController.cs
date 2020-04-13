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
using Common.Constant;
using AutoMapper;
using Cms.Attributes;
using Library.UnitOfWork;
using Library.ViewModels.Complains;
using Library.DbContext.Results;
using Library.ViewModels.Ticket;
using Cms.ViewEngine;
using Cms.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.ExceptionServices;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class DebitController : BaseController
    {
        // GET: Debit
        [CheckPermission(EnumAction.Export, EnumPage.Debit)]
        public ActionResult Index()
        {
            return View();
        }

        #region [Debit]

        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.MustCollect)]
        public async Task<JsonResult> GetAllDebitList(int page, int pageSize, DebitSearchModal searchModal)
        {
            // Tính Total money thu/ chi theo danh sách
            decimal collectMoney = 0;
            decimal returnMoney = 0;

            var debitModal = new List<DebitDetailHistory>();

            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new DebitSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);

            //2. Lấy ra dữ liệu
            debitModal = await UnitOfWork.DebitRepo.GetAllDebitList(
                out totalRecord,
                page,
                pageSize,
                searchModal.Keyword,
                searchModal.Status,
                DateStart,
                DateEnd,
                searchModal.UserId,
                searchModal.SubjectId,
                searchModal.SubjectTypeId,
                searchModal.TreasureId,
                searchModal.FinanceFundId,
                searchModal.Type,
                UserState);
            collectMoney = debitModal.Sum(x => x.MustCollectMoney ?? 0);
            returnMoney = debitModal.Sum(x => x.MustReturnMoney ?? 0);
            return Json(new { totalRecord, debitModal, collectMoney, returnMoney }, JsonRequestBehavior.AllowGet);
        }

        //Danh sach cong no cua don hàng
        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.MustCollect)]
        public async Task<JsonResult> GetAllDebitOrderList(int page, int pageSize, DebitSearchModal searchModal)
        {

            var debitModal = new List<DebitDetailHistory>();

            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new DebitSearchModal();
            }

            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);

            //2. Lấy ra dữ liệu
            debitModal = await UnitOfWork.DebitRepo.GetAllDebitList(
                out totalRecord,
                page,
                pageSize,
                searchModal.Keyword,
                searchModal.Status,
                DateStart,
                DateEnd,
                searchModal.UserId,
                searchModal.SubjectId,
                searchModal.SubjectTypeId,
                searchModal.TreasureId,
                searchModal.FinanceFundId,
                searchModal.Type,
                UserState);

            return Json(new { totalRecord, debitModal }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.MustCollect)]
        public async Task<JsonResult> GetDebitDetail(int debitId)
        {
            var result = true;

            var debitModal = await UnitOfWork.DebitRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == debitId);
            if (debitModal == null)
            {
                result = false;
            }

            return Json(new { result, debitModal }, JsonRequestBehavior.AllowGet);
        }

        //Thong tin chi tiet DebitHistory
        [HttpPost]
        //[CheckPermission(EnumAction.View, EnumPage.MustCollect)]
        public async Task<JsonResult> GetDebitHistoryDetail(int debitId)
        {
            var debitModal = await UnitOfWork.DebitHistoryRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == debitId);
            if (debitModal == null)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities detail does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { debitModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.Debit)]
        public JsonResult CreateNewHandDebit(DebitHistoryMeta model)
        {
            var manualUpdateDebitModel = new ManualUpdateDebitModel();

            ModelState.Remove("Id");
            ModelState.Remove("Idd");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra thông tin đối tượng
            //var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == model.SubjectId && x.Email == model.SubjectEmail && x.Code == model.SubjectCode);
            //if (customerDetail == null)
            //{
            //    var staffDetail = UnitOfWork.UserRepo.FirstOrDefault(x => x.Id == model.SubjectId && x.Email == model.SubjectEmail);
            //    if (staffDetail != null)
            //    {
            //        manualUpdateDebitModel.SubjectId = staffDetail.Id;
            //        manualUpdateDebitModel.SubjectEmail = staffDetail.Email;
            //    }
            //}
            //else
            //{
            //    manualUpdateDebitModel.SubjectId = customerDetail.Id;
            //    manualUpdateDebitModel.SubjectCode = customerDetail.Code;
            //    manualUpdateDebitModel.SubjectEmail = customerDetail.Email;
            //}
            manualUpdateDebitModel.SubjectId = (int)model.SubjectId;
            manualUpdateDebitModel.SubjectCode = model.SubjectCode;
            manualUpdateDebitModel.SubjectEmail = model.SubjectEmail;

            manualUpdateDebitModel.Money = (decimal)model.Money;
            manualUpdateDebitModel.Note = model.Note;
            manualUpdateDebitModel.PayReceivableId = (int)model.PayReceivableId;
            manualUpdateDebitModel.Status = (byte)DebitHistoryStatus.Incomplete;


            var data = UpdateDebitManual(manualUpdateDebitModel);
            return Json(new { status = data.status, msg = data.msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.Debit)]
        public JsonResult EditDebit(Debit model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            var customerDetail = new Customer();
            var staffDetail = new User();

            // Chặn nếu số tiền thu thực tế âm.
            //if (model.CurrencyFluctuations < model.CurrencyDiscount)
            //{
            //    return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            //}

            // Lấy thông tin Detail MustCollect
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.Id);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Chặn nếu trạng thái phiếu nạp/trừ tiền đã được duyệt thì ko thể Edit
            if (debitDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.EditRechargeBillStatusIsNotImpossible }, JsonRequestBehavior.AllowGet);
            }

            // Lấy thông tin Detail Type đối tượng kế toán.
            var accountantSubjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.AccountantSubjectId);
            if (accountantSubjectDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Kiểm tra thông tin khách hàng
            customerDetail = UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.SubjectId);
            if (customerDetail == null)
            {
                staffDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId);
                if (staffDetail == null)
                {
                    return Json(new { status = Result.Failed, msg = "You have not entered subject information or subject does not exist !" }, JsonRequestBehavior.AllowGet);
                }
            }

            // Lấy thông tin Detail quỹ
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.FinanceFundId);

            // Lấy thông tin Detail Type định khoản
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == model.TreasureId);
            if (treasureDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin phiếu nạp/trừ tiền khách hàng
                    Mapper.Map(model, debitDetail);

                    // Tính toán lại số tiền phải thu thực tế
                    //debitDetail.CurrencyReal = debitDetail.CurrencyFluctuations - debitDetail.CurrencyDiscount;

                    if (financeFundDetail != null)
                    {
                        // Map lại các thông tin để lưu
                        debitDetail.FinanceFundId = financeFundDetail.Id;
                        debitDetail.FinanceFundName = financeFundDetail.Name;
                        debitDetail.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                        debitDetail.FinanceFundDepartment = financeFundDetail.Department;
                        debitDetail.FinanceFundNameBank = financeFundDetail.NameBank;
                        debitDetail.FinanceFundUserFullName = financeFundDetail.UserFullName;
                        debitDetail.FinanceFundUserPhone = financeFundDetail.UserPhone;
                        debitDetail.FinanceFundUserEmail = financeFundDetail.UserEmail;
                    }

                    debitDetail.AccountantSubjectId = accountantSubjectDetail.Id;
                    debitDetail.AccountantSubjectName = accountantSubjectDetail.SubjectName;

                    if (customerDetail != null)
                    {
                        debitDetail.SubjectId = customerDetail.Id;
                        debitDetail.SubjectCode = customerDetail.Code;
                        debitDetail.SubjectName = customerDetail.FullName;
                        debitDetail.SubjectPhone = customerDetail.Phone;
                        debitDetail.SubjectEmail = customerDetail.Email;
                    }
                    else
                    {
                        debitDetail.SubjectId = staffDetail.Id;
                        debitDetail.SubjectCode = "";
                        debitDetail.SubjectName = staffDetail.FullName;
                        debitDetail.SubjectPhone = staffDetail.Phone;
                        debitDetail.SubjectEmail = staffDetail.Email;
                    }

                    debitDetail.TreasureId = treasureDetail.Id;
                    debitDetail.TreasureName = treasureDetail.Name;

                    // Lưu lại vào database
                    UnitOfWork.DebitRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.EditMustCollectIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.Debit)]
        public JsonResult DeleteDebit(int mustCollectId)
        {
            // Kiểm tra thông tin phiếu công nợ phải thu
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.Id == mustCollectId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            // Nếu được duyệt thì không cho xóa
            if (debitDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.MustCollectIsApproval }, JsonRequestBehavior.AllowGet);
            }

            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lấy lại thông tin để thực hiện lưu
                    debitDetail.IsDelete = true;

                    //Lưu xuống Database
                    UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteMustCollectIsSuccess }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.Debit)]
        public JsonResult ApprovalDebit(int mustCollectId)
        {
            //1. Kiểm tra thông tin phiếu
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.Id == mustCollectId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }
            if (debitDetail.Status == 1)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin quỹ thực tế trong công nợ phải thu
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitDetail.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.FinanceFundIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //3. Ghi nhận thông tin thu tiền vào qũy
            // Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //======================== Cập nhật lại thông tin vào công nợ phải thu
                    debitDetail.Status = 1; // Trạng thái phiếu công nợ được xác nhận Approved

                    debitDetail.UserApprovalId = UserState.UserId;
                    debitDetail.UserApprovalName = UserState.UserName;

                    //UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    //======================== Cộng tiền vão quỹ đã thu thực tế của khách hàng
                    var fundBillDetail = new FundBill();

                    fundBillDetail.Status = 1; // Xác nhận được duyệt
                    fundBillDetail.Type = 0;

                    fundBillDetail.Code = string.Empty;

                    var fundBillOfDay = UnitOfWork.FundBillRepo.Count(x =>
                                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                        x.Created.Day == DateTime.Now.Day);
                    fundBillDetail.Code = $"{fundBillOfDay}{DateTime.Now:ddMMyy}";

                    // Map lại các thông tin vào FundBill
                    fundBillDetail.FinanceFundId = financeFundDetail.Id;
                    fundBillDetail.FinanceFundName = financeFundDetail.Name;
                    fundBillDetail.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillDetail.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillDetail.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillDetail.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillDetail.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillDetail.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    fundBillDetail.AccountantSubjectId = debitDetail.AccountantSubjectId;
                    fundBillDetail.AccountantSubjectName = debitDetail.AccountantSubjectName;

                    fundBillDetail.SubjectId = debitDetail.SubjectId;
                    fundBillDetail.SubjectCode = debitDetail.SubjectCode;
                    fundBillDetail.SubjectName = debitDetail.SubjectName;
                    fundBillDetail.SubjectPhone = debitDetail.SubjectPhone;
                    fundBillDetail.SubjectEmail = debitDetail.SubjectEmail;

                    fundBillDetail.TreasureId = debitDetail.TreasureId;
                    fundBillDetail.TreasureName = debitDetail.TreasureName;

                    /*fundBillDetail.CurrencyFluctuations = (decimal)debitDetail.CurrencyReal;   */// Cập nhật số tiền mới vào trong quỹ
                    fundBillDetail.CurencyStart = financeFundDetail.Balance;                         // Số tiền đầu tiên đang có trong quỹ
                    /*fundBillDetail.CurencyEnd = financeFundDetail.Balance + (decimal)debitDetail.CurrencyReal; */           // Số tiền mới trong quỹ

                    // Gắn thông tin người tạo tương đồng người tạo phiếu phải thu.
                    fundBillDetail.UserId = debitDetail.UserId;
                    fundBillDetail.UserName = debitDetail.UserName;
                    fundBillDetail.UserCode = debitDetail.UserCode;

                    // Gắn thông tin người duyệt
                    fundBillDetail.UserApprovalId = UserState.UserId;
                    fundBillDetail.UserApprovalName = UserState.UserName;

                    // Gắn thông tin ghi chú
                    fundBillDetail.Note = "Collecting liabilities note No.: " + debitDetail.Code;

                    UnitOfWork.FundBillRepo.Add(fundBillDetail);
                    UnitOfWork.FundBillRepo.Save();

                    //======================== Trừ tiền thực tế vào quỹ
                    //financeFundDetail.Balance += (decimal)debitDetail.CurrencyReal;

                    UnitOfWork.FinaceFundRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = ConstantMessage.SystemError }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.ApprovalMustCollectIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetDebitSearchData()
        {
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listSubjectType = new List<SearchMeta>();
            var listSubject = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listOffice = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            // Lấy các trạng thái Status
            foreach (MustCollectStatus mustCollectStatus in Enum.GetValues(typeof(MustCollectStatus)))
            {
                if (mustCollectStatus >= 0)
                {
                    listStatus.Add(new { Text = mustCollectStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)mustCollectStatus });
                }
            }

            // Lấy danh sách các đối tượng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => x.Id > 0 && !x.IsDelete).ToList();
            var tempWarehouseList = from p in subjectType
                                    select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            listSubjectType.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listSubjectType.AddRange(tempWarehouseList.ToList());

            return Json(new { listStatus, listSubjectType, listSubject, listOffice }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.Debit)]
        public JsonResult GetDebitInitCreateOrEdit()
        {
            // Lấy danh sách các đối tượng nhập
            var accounttantSubject = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => !x.IsDelete && x.Id > 0).ToList();
            var tempAcounttantSubject = from p in accounttantSubject
                                        select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            var listAccountantSubject = tempAcounttantSubject.ToList();

            //// Lấy danh sách quỹ
            //var listFinanceFund = UnitOfWork.DbContext.FinanceFunds.Where(x => !x.IsDelete && x.Id > 0).Select(o => new
            //{
            //    id = o.Id.ToString(),
            //    text = o.Name,
            //    parent = o.ParentId.ToString(),
            //    idPath = o.IdPath,
            //}).ToList();
            //listFinanceFund.Add(new { id = "0", text = "- Select fund -", parent = "#", idPath = "0" });

            // Lấy danh sách định khoản
            var listTreasure = UnitOfWork.DbContext.Treasures.Where(x => !x.IsDelete && x.Id > 0).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
            }).ToList();
            listTreasure.Add(new { id = "0", text = "Fund management", parent = "#", idPath = "0" });

            return Json(new { listAccountantSubject, listTreasure }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [DebitHistory]

        /// <summary>
        /// Edit công nợ
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Update, EnumPage.Debit)]
        public JsonResult EditDebitHistory(DebitHistoryMeta model)
        {
            var debitHistory = new DebitHistory();
            var timeNow = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //1. Kiểm tra AMOUNT REQUESTED cập nhật phải > 0
            if (model.Money <= 0)
            {
                return Json(new { status = Result.Failed, msg = "Amount entered must be > 0" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra debitHistory có tồn tại hay không ?
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.Id == model.Id);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Amount of liabilities you entered was not correct" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin Debit
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.Id == debitHistoryDetail.DebitId && x.SubjectId == debitHistoryDetail.SubjectId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Amount of liabilities you entered was not correct !" }, JsonRequestBehavior.AllowGet);
            }

            //4. Cập nhật lại vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Nếu là công nợ phải thu
                    if (debitHistoryDetail.DebitType == (byte)DebitType.Collect)
                    {
                        //Cập nhật lại số tiền trong Debit
                        debitDetail.MustCollectMoney = debitDetail.MustCollectMoney + (model.Money - debitHistoryDetail.Money);
                        debitDetail.LastUpdated = timeNow;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();
                    }
                    if (debitHistoryDetail.DebitType == (byte)DebitType.Return)
                    {

                        // Nếu là công nợ phải trả
                        debitDetail.MustReturnMoney = debitDetail.MustReturnMoney + (model.Money - debitHistoryDetail.Money);
                        debitDetail.LastUpdated = timeNow;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();
                    }

                    // Cập nhật công nợ DebitHistory
                    debitHistoryDetail.Money = model.Money;
                    debitHistoryDetail.LastUpdated = timeNow;
                    debitHistoryDetail.Note = model.Note;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi cập nhật công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}
            }

            return Json(new { status = Result.Succeed, msg = "Liabilities updated successfully!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Xóa công nợ, cần thực hiện xóa và cập nhật lại thông tin trong Debit
        /// </summary>
        /// <param name="debitHistoryId">Id công nợ</param>
        /// <returns></returns>
        [CheckPermission(EnumAction.Delete, EnumPage.Debit)]
        public JsonResult DeleteDebitHistory(int debitHistoryId)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem DebitHistory có tồn tại hay không 
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.Id == debitHistoryId);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Amount of liabilities you entered was not correct !" }, JsonRequestBehavior.AllowGet);
            }
            if (debitHistoryDetail.Status == (byte)DebitHistoryStatus.Completed)
            {
                return Json(new { status = Result.Failed, msg = "Cannot delete completed liabilities!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin Debit
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.Id == debitHistoryDetail.DebitId && x.SubjectId == debitHistoryDetail.SubjectId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Amount of liabilities you entered was not correct !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Xóa công nợ
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Nếu là công nợ phải thu
                    if (debitHistoryDetail.DebitType == (byte)DebitType.Collect)
                    {
                        //Cập nhật lại số tiền trong Debit
                        debitDetail.MustCollectMoney = debitDetail.MustCollectMoney - debitHistoryDetail.Money;
                        debitDetail.LastUpdated = timeNow;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();

                    }
                    if (debitHistoryDetail.DebitType == (byte)DebitType.Return)
                    {

                        // Nếu là công nợ phải trả
                        debitDetail.MustReturnMoney = debitDetail.MustCollectMoney - debitHistoryDetail.Money;
                        debitDetail.LastUpdated = timeNow;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();

                    }

                    // Cập nhật công nợ DebitHistory
                    UnitOfWork.DebitHistoryRepo.Remove(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi xóa công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = Result.Succeed, msg = "Liabilities deleted successfully !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission(EnumAction.Approvel, EnumPage.Debit)]
        public JsonResult ExecuteDebitMustCollect(DebitHistoryMeta model)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem debithistory có chính xác hay không ?
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.IsSystem == false && x.Id == model.Id);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (debitHistoryDetail.Status == (byte)DebitHistoryStatus.Completed)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities has been completed !" }, JsonRequestBehavior.AllowGet);
            }
            //if (model.DebitType == (byte)DebitHistoryType.mustCollect)
            //{
            //    return Json(new { status = Result.Failed, msg = "Type công nợ processing request không chính xác !" }, JsonRequestBehavior.AllowGet);
            //}

            //2. Kiểm tra xem debit cha có tồn tại hay không ?
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.Id == debitHistoryDetail.DebitId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Parent liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin quỹ có tồn tại hay không ?
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "The fund you have just selected is not correct or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //4. Kiểm tra thông tin định khoản có chính xác hay không ?
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (treasureDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Account balancing information is not correct !" }, JsonRequestBehavior.AllowGet);
            }
            if (treasureDetail.IsIdSystem == true)
            {
                return Json(new { status = Result.Failed, msg = "Không thể chọn định khoản tự động của hệ thống  !" }, JsonRequestBehavior.AllowGet);
            }
            if (treasureDetail.Operator != true)
            {
                return Json(new { status = Result.Failed, msg = "Loại định khoản yêu cầu không chính xác  !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Cập nhật
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1. Cập nhật lại vào Debit
                    debitDetail.MustCollectMoney -= debitHistoryDetail.Money;
                    debitDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    //3.2. Cập nhật lại vào DebitHistory
                    debitHistoryDetail.Status = (byte)DebitHistoryStatus.Completed;
                    debitHistoryDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.3. Cập nhật công nợ DebitHistory
                    debitHistoryDetail.Money = model.Money;
                    debitHistoryDetail.LastUpdated = timeNow;
                    debitHistoryDetail.Note = model.Note;

                    debitHistoryDetail.FinanceFundId = financeFundDetail.Id;
                    debitHistoryDetail.FinanceFundName = financeFundDetail.Name;
                    debitHistoryDetail.FinanceFundBankAccountNumber = financeFundDetail.CardId;
                    debitHistoryDetail.FinanceFundDepartment = financeFundDetail.CardBranch;
                    debitHistoryDetail.FinanceFundNameBank = financeFundDetail.CardBank;


                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.4. Cập nhật bản ghi FundBill
                    var fundBillOfDay = UnitOfWork.FundBillRepo.Count(x =>
                                                                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                        x.Created.Day == DateTime.Now.Day);

                    var fundBillDetail = new FundBill();
                    var customerDetail = new Customer();
                    var userDetail = new User();

                    // Kiểm tra lại thông tin đối tượng
                    customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitHistoryDetail.SubjectId && x.Code == debitHistoryDetail.SubjectCode);
                    if (customerDetail == null)
                    {
                        userDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitHistoryDetail.SubjectId);
                        if (userDetail != null)
                        {
                            fundBillDetail.AccountantSubjectId = userDetail.TypeId;
                            fundBillDetail.AccountantSubjectName = userDetail.TypeName;

                            fundBillDetail.SubjectId = userDetail.Id;
                            fundBillDetail.SubjectEmail = userDetail.Email;
                            fundBillDetail.SubjectPhone = userDetail.Phone;
                            fundBillDetail.SubjectName = userDetail.FullName;
                        }
                    }
                    else
                    {
                        fundBillDetail.AccountantSubjectId = customerDetail.TypeId;
                        fundBillDetail.AccountantSubjectName = customerDetail.TypeName;

                        fundBillDetail.SubjectId = customerDetail.Id;
                        fundBillDetail.SubjectCode = customerDetail.Code;
                        fundBillDetail.SubjectEmail = customerDetail.Email;
                        fundBillDetail.SubjectPhone = customerDetail.Phone;
                        fundBillDetail.SubjectName = customerDetail.FullName;
                    }

                    //3.5. Cập nhật vào phiếu nạp/Subtract funds tự động

                    fundBillDetail.Code = $"{fundBillOfDay}{DateTime.Now:ddMMyy}";
                    fundBillDetail.Type = (byte)FundBillType.Increase;
                    fundBillDetail.Status = (byte)FundBillStatus.Approved;

                    // Số tiền
                    fundBillDetail.CurrencyFluctuations = (decimal)debitHistoryDetail.Money;
                    fundBillDetail.Increase = (decimal)debitHistoryDetail.Money;
                    fundBillDetail.Diminishe = 0;

                    fundBillDetail.CurencyStart = financeFundDetail.Balance;
                    fundBillDetail.CurencyEnd = financeFundDetail.Balance + (decimal)debitHistoryDetail.Money;

                    //Thông tin quỹ
                    fundBillDetail.FinanceFundId = financeFundDetail.Id;
                    fundBillDetail.FinanceFundName = financeFundDetail.Name;
                    fundBillDetail.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillDetail.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillDetail.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillDetail.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillDetail.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillDetail.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    //Thông tin định khoản
                    fundBillDetail.TreasureId = treasureDetail.Id;
                    fundBillDetail.TreasureName = treasureDetail.Name;

                    // Gắn thông tin người tạo
                    fundBillDetail.UserId = UserState.UserId;
                    fundBillDetail.UserName = UserState.UserName;

                    UnitOfWork.FundBillRepo.Add(fundBillDetail);
                    UnitOfWork.FundBillRepo.Save();

                    //3.4. Cập nhật lại số dư quỹ (Tăng quỹ)
                    financeFundDetail.Balance += (decimal)debitHistoryDetail.Money;

                    UnitOfWork.FinaceFundRepo.Update(financeFundDetail);
                    UnitOfWork.FinaceFundRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi cập nhật công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = Result.Succeed, msg = "Account receivable processed successfully! Respective amount has been added to fund !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckPermission(EnumAction.Approvel, EnumPage.Debit)]
        public JsonResult ExecuteDebitMustReturn(DebitHistoryMeta model)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem debithistory có chính xác hay không ?
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.IsSystem == false && x.Id == model.Id);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (debitHistoryDetail.Status == (byte)DebitHistoryStatus.Completed)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities has been completed !" }, JsonRequestBehavior.AllowGet);
            }
            //if(model.DebitType == (byte)DebitHistoryType.mustReturn)
            //{
            //    return Json(new { status = Result.Failed, msg = "Type công nợ processing request không chính xác !" }, JsonRequestBehavior.AllowGet);
            //}

            //2. Kiểm tra xem debit cha có tồn tại hay không ?
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.Id == debitHistoryDetail.DebitId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Parent liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin quỹ có tồn tại hay không ?
            var financeFundDetail = UnitOfWork.FinaceFundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.FinanceFundId);
            if (financeFundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "The fund you have just selected is not correct or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (financeFundDetail.Balance < model.Money)
            {
                return Json(new { status = Result.Failed, msg = "Fund balance is not enough to execute payment !" }, JsonRequestBehavior.AllowGet);
            }

            //4. Kiểm tra thông tin định khoản có chính xác hay không ?
            var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.TreasureId);
            if (treasureDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Account balancing information is not correct !" }, JsonRequestBehavior.AllowGet);
            }
            if (treasureDetail.IsIdSystem == true)
            {
                return Json(new { status = Result.Failed, msg = "Cannot select auto balancing account from the system  !" }, JsonRequestBehavior.AllowGet);
            }
            if (treasureDetail.Operator != false)
            {
                return Json(new { status = Result.Failed, msg = "Requested type of account balancing is not correct  !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Cập nhật
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1. Cập nhật lại vào Debit
                    debitDetail.MustReturnMoney -= debitHistoryDetail.Money;
                    debitDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    //3.2. Cập nhật lại vào DebitHistory
                    debitHistoryDetail.Status = (byte)DebitHistoryStatus.Completed;
                    debitHistoryDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.3. Cập nhật công nợ DebitHistory
                    debitHistoryDetail.Money = model.Money;
                    debitHistoryDetail.LastUpdated = timeNow;
                    debitHistoryDetail.Note = model.Note;

                    debitHistoryDetail.FinanceFundId = financeFundDetail.Id;
                    debitHistoryDetail.FinanceFundName = financeFundDetail.Name;
                    debitHistoryDetail.FinanceFundBankAccountNumber = financeFundDetail.CardId;
                    debitHistoryDetail.FinanceFundDepartment = financeFundDetail.CardBranch;
                    debitHistoryDetail.FinanceFundNameBank = financeFundDetail.CardBank;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.4. Cập nhật bản ghi FundBill
                    var fundBillOfDay = UnitOfWork.FundBillRepo.Count(x =>
                                                                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                        x.Created.Day == DateTime.Now.Day);

                    var fundBillDetail = new FundBill();
                    var customerDetail = new Customer();
                    var userDetail = new User();

                    // Kiểm tra lại thông tin đối tượng
                    customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitHistoryDetail.SubjectId && x.Code == debitHistoryDetail.SubjectCode);
                    if (customerDetail == null)
                    {
                        userDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitHistoryDetail.SubjectId);
                        if (userDetail != null)
                        {
                            fundBillDetail.AccountantSubjectId = userDetail.TypeId;
                            fundBillDetail.AccountantSubjectName = userDetail.TypeName;

                            fundBillDetail.SubjectId = userDetail.Id;
                            fundBillDetail.SubjectEmail = userDetail.Email;
                            fundBillDetail.SubjectPhone = userDetail.Phone;
                            fundBillDetail.SubjectName = userDetail.FullName;
                        }
                    }
                    else
                    {
                        fundBillDetail.AccountantSubjectId = customerDetail.TypeId;
                        fundBillDetail.AccountantSubjectName = customerDetail.TypeName;

                        fundBillDetail.SubjectId = customerDetail.Id;
                        fundBillDetail.SubjectCode = customerDetail.Code;
                        fundBillDetail.SubjectEmail = customerDetail.Email;
                        fundBillDetail.SubjectPhone = customerDetail.Phone;
                        fundBillDetail.SubjectName = customerDetail.FullName;
                    }

                    //3.5. Cập nhật vào phiếu nạp/Subtract funds tự động

                    fundBillDetail.Code = $"{fundBillOfDay}{DateTime.Now:ddMMyy}";
                    fundBillDetail.Type = (byte)FundBillType.Diminishe;
                    fundBillDetail.Status = (byte)FundBillStatus.Approved;

                    // Số tiền
                    fundBillDetail.CurrencyFluctuations = (decimal)debitHistoryDetail.Money;
                    fundBillDetail.Diminishe = (decimal)debitHistoryDetail.Money;
                    fundBillDetail.Increase = 0;

                    fundBillDetail.CurencyStart = financeFundDetail.Balance;
                    fundBillDetail.CurencyEnd = financeFundDetail.Balance - (decimal)debitHistoryDetail.Money;

                    //Thông tin quỹ
                    fundBillDetail.FinanceFundId = financeFundDetail.Id;
                    fundBillDetail.FinanceFundName = financeFundDetail.Name;
                    fundBillDetail.FinanceFundBankAccountNumber = financeFundDetail.BankAccountNumber;
                    fundBillDetail.FinanceFundDepartment = financeFundDetail.Department;
                    fundBillDetail.FinanceFundNameBank = financeFundDetail.NameBank;
                    fundBillDetail.FinanceFundUserFullName = financeFundDetail.UserFullName;
                    fundBillDetail.FinanceFundUserPhone = financeFundDetail.UserPhone;
                    fundBillDetail.FinanceFundUserEmail = financeFundDetail.UserEmail;

                    //Thông tin định khoản
                    fundBillDetail.TreasureId = treasureDetail.Id;
                    fundBillDetail.TreasureName = treasureDetail.Name;

                    // Gắn thông tin người tạo
                    fundBillDetail.UserId = UserState.UserId;
                    fundBillDetail.UserName = UserState.UserName;

                    UnitOfWork.FundBillRepo.Add(fundBillDetail);
                    UnitOfWork.FundBillRepo.Save();

                    //3.4. Cập nhật lại số dư quỹ (Tăng quỹ)
                    financeFundDetail.Balance -= (decimal)debitHistoryDetail.Money;

                    UnitOfWork.FinaceFundRepo.Update(financeFundDetail);
                    UnitOfWork.FinaceFundRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi cập nhật công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = Result.Succeed, msg = "Account payable handled successfully ! Fund has been deducted corresponding amount !" }, JsonRequestBehavior.AllowGet);
            }
        }


        #endregion

        #region [Các hàm xử lý công nợ phải thu - phải trả]
        public ReturnFunc UpdateDebitManual(ManualUpdateDebitModel model)
        {
            var result = new ReturnFunc();

            var debitHistory = new DebitHistory();

            var customerDetail = new Customer();
            var staffDetail = new User();
            var accountantSubjectDetail = new AccountantSubject();
            debitHistory.Note = model.Note;
            //1. Kiểm tra AMOUNT REQUESTED cập nhật phải > 0
            if (model.Money <= 0)
            {
                result.status = Result.Failed;
                result.msg = "Please enter the amount  > 0 !";
                return result;
            }
            else
            {
                debitHistory.Money = model.Money;
            }

            //2. Kiểm tra định khoản công nợ không phải định khoản của system
            var PayReceivableDetail = UnitOfWork.PayReceivableRepo.FirstOrDefault(x => !x.IsDelete && x.IsIdSystem == false && x.Id == model.PayReceivableId);
            if (PayReceivableDetail == null)
            {
                result.status = Result.Failed;
                result.msg = "Account balancing entered was not correct !";
                return result;
            }
            else
            {
                debitHistory.PayReceivableId = PayReceivableDetail.Id;
                debitHistory.PayReceivableIName = PayReceivableDetail.Name;
            }

            //3. Lấy thông tin Detail đối tượng nhập là khách hàng
            if (model.SubjectCode != null)
            {
                customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId && x.Code == model.SubjectCode && x.Email == model.SubjectEmail);
                if (customerDetail != null)
                {
                    debitHistory.SubjectId = customerDetail.Id;
                    debitHistory.SubjectCode = customerDetail.Code;
                    debitHistory.SubjectName = customerDetail.FullName;
                    debitHistory.SubjectPhone = customerDetail.Phone;
                    debitHistory.SubjectEmail = customerDetail.Email;
                    debitHistory.SubjectAddress = customerDetail.Address;

                    accountantSubjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Id == customerDetail.TypeId);
                    if (accountantSubjectDetail == null)
                    {
                        result.status = Result.Failed;
                        result.msg = "Vui lòng kiểm tra kiểu đối tượng người dùng trên hệ thống !";
                        return result;
                    }
                }
            }
            else
            {
                staffDetail = UnitOfWork.UserRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId && x.Email == model.SubjectEmail);
                if (staffDetail == null)
                {
                    result.status = Result.Failed;
                    result.msg = "You have not entered subject information or subject does not exist !";
                    return result;
                }
                else
                {
                    debitHistory.SubjectId = staffDetail.Id;
                    debitHistory.SubjectName = staffDetail.FullName;
                    debitHistory.SubjectPhone = staffDetail.Phone;
                    debitHistory.SubjectEmail = staffDetail.Email;

                    accountantSubjectDetail = UnitOfWork.AccountantSubjectRepo.FirstOrDefault(x => x.Id == staffDetail.TypeId);
                    if (accountantSubjectDetail == null)
                    {
                        result.status = Result.Failed;
                        result.msg = "Please check type of user object in the system !";
                        return result;
                    }
                }
            }

            //5. Yêu cầu cập nhật phải thu - phải trả
            if (PayReceivableDetail.Operator == true)
            {
                //Cập nhật phải thu vào system
                if (ManualMustCollectDebit(debitHistory, accountantSubjectDetail.Id, accountantSubjectDetail.SubjectName) == false)
                {
                    result.status = Result.Failed;
                    result.msg = "There is an error in updating account receivable!";
                    return result;
                }
            }
            if (PayReceivableDetail.Operator == false)
            {
                //Cập nhật phải trả vào system
                if (ManualMustReturnDebit(debitHistory, accountantSubjectDetail.Id, accountantSubjectDetail.SubjectName) == false)
                {
                    result.status = Result.Failed;
                    result.msg = "There is an error in updating account payable !";
                    return result;
                }
            }
            result.status = Result.Succeed;
            result.msg = "New liabilities added successfully !";
            return result;
        }

        public bool ManualMustCollectDebit(DebitHistory debitHistoryModel, int subjectId, string subjectName)
        {
            var timeNow = DateTime.Now;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //1. Kiểm tra xem đã có phát sinh công nợ của khách hàng này với Orders này hay chưa
                    var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.SubjectId == debitHistoryModel.SubjectId);
                    if (debitDetail == null)
                    {
                        var debitUpdate = new Debit();

                        //1.1. Thêm trong Debit
                        var debitCode = UnitOfWork.DebitRepo.Count(x =>
                                                                 x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                 x.Created.Day == DateTime.Now.Day);

                        debitUpdate.Code = $"{DateTime.Now:ddMMyy}{debitCode}";

                        debitUpdate.Status = 0;

                        debitUpdate.MustCollectMoney = debitHistoryModel.Money;
                        debitUpdate.MustReturnMoney = 0;

                        debitUpdate.SubjectId = debitHistoryModel.SubjectId;
                        debitUpdate.SubjectCode = debitHistoryModel.SubjectCode;
                        debitUpdate.SubjectName = debitHistoryModel.SubjectName;
                        debitUpdate.SubjectEmail = debitHistoryModel.SubjectEmail;
                        debitUpdate.SubjectAddress = debitHistoryModel.SubjectAddress;
                        debitUpdate.SubjectPhone = debitHistoryModel.SubjectPhone;

                        debitUpdate.SubjectTypeId = subjectId;
                        debitUpdate.SubjectTypeName = subjectName;

                        debitUpdate.OrderId = debitHistoryModel.OrderId;
                        debitUpdate.OrderCode = debitHistoryModel.OrderCode;
                        debitUpdate.OrderType = debitHistoryModel.OrderType;
                        debitUpdate.IsSystem = false;

                        UnitOfWork.DebitRepo.Add(debitUpdate);
                        UnitOfWork.DebitRepo.Save();
                        //1.2. Cập nhật thêm vào DebitHistory
                        var debitHistoryCode = UnitOfWork.DebitHistoryRepo.Count(x =>
                                                                              x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                              x.Created.Day == DateTime.Now.Day);

                        debitHistoryModel.Code = $"{debitHistoryCode}{DateTime.Now:ddMMyy}";
                        debitHistoryModel.IsSystem = false;
                        debitHistoryModel.Status = (byte)DebitHistoryStatus.Incomplete;

                        debitHistoryModel.DebitType = (byte)DebitHistoryType.mustCollect;
                        debitHistoryModel.DebitId = debitUpdate.Id;
                        debitHistoryModel.DebitCode = debitUpdate.Code;

                        UnitOfWork.DebitHistoryRepo.Add(debitHistoryModel);
                        UnitOfWork.DebitHistoryRepo.Save();
                    }
                    else
                    {
                        //1.1. Thêm dữ liệu mới vào DebitHistory
                        var debitHistoryCode = UnitOfWork.DebitHistoryRepo.Count(x =>
                                                                               x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                               x.Created.Day == DateTime.Now.Day);

                        debitHistoryModel.Code = $"{debitHistoryCode}{DateTime.Now:ddMMyy}";

                        debitHistoryModel.IsSystem = false;
                        debitHistoryModel.Status = (byte)DebitHistoryStatus.Incomplete;

                        debitHistoryModel.DebitType = (byte)DebitHistoryType.mustCollect;
                        debitHistoryModel.DebitId = debitDetail.Id;
                        debitHistoryModel.DebitCode = debitDetail.Code;

                        UnitOfWork.DebitHistoryRepo.Add(debitHistoryModel);
                        UnitOfWork.DebitHistoryRepo.Save();

                        //1.2. Cập nhật trong Debit
                        debitDetail.LastUpdated = timeNow;
                        debitDetail.MustCollectMoney = debitDetail.MustCollectMoney + (decimal)debitHistoryModel.Money;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return false;
                //}
            }
            return true;
        }

        public bool ManualMustReturnDebit(DebitHistory debitHistoryModel, int subjectId, string subjectName)
        {
            var timeNow = DateTime.Now;

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //1. Kiểm tra xem đã có phát sinh công nợ của khách hàng này với Orders này hay chưa
                    var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == false && x.SubjectId == debitHistoryModel.SubjectId);
                    if (debitDetail == null)
                    {
                        var debitUpdate = new Debit();

                        //1.1. Thêm trong Debit
                        var debitCode = UnitOfWork.DebitRepo.Count(x =>
                                                                 x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                 x.Created.Day == DateTime.Now.Day);

                        debitUpdate.Code = $"{DateTime.Now:ddMMyy}{debitCode}";

                        debitUpdate.Status = 0;

                        debitUpdate.MustReturnMoney = debitHistoryModel.Money;
                        debitUpdate.MustCollectMoney = 0;

                        debitUpdate.SubjectId = debitHistoryModel.SubjectId;
                        debitUpdate.SubjectCode = debitHistoryModel.SubjectCode;
                        debitUpdate.SubjectName = debitHistoryModel.SubjectName;
                        debitUpdate.SubjectEmail = debitHistoryModel.SubjectEmail;
                        debitUpdate.SubjectAddress = debitHistoryModel.SubjectAddress;
                        debitUpdate.SubjectPhone = debitHistoryModel.SubjectPhone;

                        debitUpdate.AccountantSubjectId = subjectId;
                        debitUpdate.AccountantSubjectName = subjectName;

                        debitUpdate.OrderId = debitHistoryModel.OrderId;
                        debitUpdate.OrderCode = debitHistoryModel.OrderCode;
                        debitUpdate.OrderType = debitHistoryModel.OrderType;

                        debitUpdate.IsSystem = false;

                        UnitOfWork.DebitRepo.Add(debitUpdate);
                        UnitOfWork.DebitRepo.Save();

                        //1.2. Cập nhật thêm vào DebitHistory
                        var debitHistoryCode = UnitOfWork.DebitHistoryRepo.Count(x =>
                                                                               x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                               x.Created.Day == DateTime.Now.Day);

                        debitHistoryModel.Code = $"{debitHistoryCode}{DateTime.Now:ddMMyy}";

                        debitHistoryModel.IsSystem = false;
                        debitHistoryModel.Status = (byte)DebitHistoryStatus.Incomplete;

                        debitHistoryModel.DebitType = (byte)DebitHistoryType.mustReturn;
                        debitHistoryModel.DebitId = debitUpdate.Id;
                        debitHistoryModel.DebitCode = debitUpdate.Code;

                        UnitOfWork.DebitHistoryRepo.Add(debitHistoryModel);
                        UnitOfWork.DebitHistoryRepo.Save();
                    }
                    else
                    {
                        //1.1. Thêm dữ liệu mới vào DebitHistory
                        var debitHistoryCode = UnitOfWork.DebitHistoryRepo.Count(x =>
                                                                               x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                               x.Created.Day == DateTime.Now.Day);

                        debitHistoryModel.Code = $"{debitHistoryCode}{DateTime.Now:ddMMyy}";


                        debitHistoryModel.IsSystem = false;
                        debitHistoryModel.Status = (byte)DebitHistoryStatus.Incomplete;

                        debitHistoryModel.DebitType = (byte)DebitHistoryType.mustReturn;
                        debitHistoryModel.DebitId = debitDetail.Id;
                        debitHistoryModel.DebitCode = debitDetail.Code;

                        UnitOfWork.DebitHistoryRepo.Add(debitHistoryModel);
                        UnitOfWork.DebitHistoryRepo.Save();

                        //1.2. Cập nhật trong Debit
                        debitDetail.LastUpdated = timeNow;
                        debitDetail.MustReturnMoney = debitDetail.MustReturnMoney + (decimal)debitHistoryModel.Money;

                        UnitOfWork.DebitRepo.Update(debitDetail);
                        UnitOfWork.DebitRepo.Save();
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return false;
                //}
            }
            return true;
        }

        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.Debit)]
        public JsonResult ExecuteDebitWalletMustCollect(DebitHistoryMeta model)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem debithistory có chính xác hay không ?
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.IsSystem == true && x.Id == model.Id);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (debitHistoryDetail.Status == (byte)DebitHistoryStatus.Completed)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities has been completed !" }, JsonRequestBehavior.AllowGet);
            }
            //if (model.DebitType == (byte)DebitHistoryType.mustCollect)
            //{
            //    return Json(new { status = Result.Failed, msg = "Type công nợ processing request không chính xác !" }, JsonRequestBehavior.AllowGet);
            //}

            //2. Kiểm tra xem debit cha có tồn tại hay không ?
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == true && x.Id == debitHistoryDetail.DebitId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Parent liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin khách hàng có tồn tại hay không
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId && x.Email == model.SubjectEmail);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if(customerDetail.BalanceAvalible < debitHistoryDetail.Money)
            {
                return Json(new { status = Result.Failed, msg = "The balance of e-wallet customers is not enough to settle the debt !" }, JsonRequestBehavior.AllowGet);
            }

            //5. Kiểm tra định khoản ví điện tử tương ứng đã tồn tại hay chưa
            var customerWalletDetail = UnitOfWork.CustomerWalletRepo.FirstOrDefault(x => !x.IsDelete && x.Idd == debitHistoryDetail.PayReceivableIdd);
            if (customerWalletDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "The corresponding e-wallet debt settlement does not exist !" }, JsonRequestBehavior.AllowGet);
            }

            //6. Cập nhật vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1. Cập nhật lại vào Debit
                    debitDetail.MustCollectMoney -= debitHistoryDetail.Money;
                    debitDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    //3.2. Cập nhật lại vào DebitHistory
                    debitHistoryDetail.Status = (byte)DebitHistoryStatus.Completed;
                    debitHistoryDetail.LastUpdated = timeNow;

                    debitHistoryDetail.Money = model.Money;
                    debitHistoryDetail.LastUpdated = timeNow;
                    debitHistoryDetail.Note = model.Note;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.4. Cập nhật bản ghi RechargeBill
                    var reachargeBillOfDay = UnitOfWork.RechargeBillRepo.Count(x =>
                                                                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                        x.Created.Day == DateTime.Now.Day);

                    var recharBillDetail = new RechargeBill();
                    // Kiểm tra lại thông tin đối tượng

                    recharBillDetail.CustomerId = customerDetail.Id;
                    recharBillDetail.CustomerEmail = customerDetail.Email;
                    recharBillDetail.CustomerPhone = customerDetail.Phone;
                    recharBillDetail.CustomerName = customerDetail.FullName;

                    //3.5. Cập nhật vào phiếu nạp/Subtract funds tự động
                    recharBillDetail.Code = $"{reachargeBillOfDay}{DateTime.Now:ddMMyy}";
                    recharBillDetail.Type = (byte)RechargeBillType.Diminishe;
                    recharBillDetail.Status = (byte)RechargeBillStatus.Approved;

                    // Số tiền
                    recharBillDetail.CurrencyFluctuations = (decimal)debitHistoryDetail.Money;
                    recharBillDetail.Diminishe = (decimal)debitHistoryDetail.Money;
                    recharBillDetail.Increase = 0;

                    recharBillDetail.CurencyStart = customerDetail.BalanceAvalible;
                    recharBillDetail.CurencyEnd = customerDetail.BalanceAvalible - (decimal)debitHistoryDetail.Money;

                    //Thông tin định khoản
                    recharBillDetail.TreasureId = customerWalletDetail.Id;
                    recharBillDetail.TreasureName = customerWalletDetail.Name;

                    // Gắn thông tin người tạo
                    recharBillDetail.UserId = UserState.UserId;
                    recharBillDetail.UserName = UserState.UserName;
                    // Gắn thông tin người duyệt
                    recharBillDetail.UserApprovalId = UserState.UserId;
                    recharBillDetail.UserApprovalName = UserState.UserName;

                    // Gắn lại thông tin Orders
                    recharBillDetail.OrderId = debitHistoryDetail.OrderId;
                    recharBillDetail.OrderCode = debitHistoryDetail.OrderCode;
                    recharBillDetail.OrderType = debitHistoryDetail.OrderType;

                    UnitOfWork.RechargeBillRepo.Add(recharBillDetail);
                    UnitOfWork.RechargeBillRepo.Save();

                    //3.4. Cập nhật lại số dư ví điện tử khách hàng
                    customerDetail.BalanceAvalible -= (decimal)debitHistoryDetail.Money;

                    UnitOfWork.CustomerRepo.Update(customerDetail);
                    UnitOfWork.CustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi cập nhật công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = Result.Succeed, msg = "Account receivable handled successfully ! Customer e-wallet has been deposited !" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.Debit)]
        public JsonResult ExecuteDebitWalletMustReturn(DebitHistoryMeta model)
        {
            var timeNow = DateTime.Now;

            //1. Kiểm tra xem debithistory có chính xác hay không ?
            var debitHistoryDetail = UnitOfWork.DebitHistoryRepo.FirstOrDefault(x => x.IsSystem == true && x.Id == model.Id);
            if (debitHistoryDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (debitHistoryDetail.Status == (byte)DebitHistoryStatus.Completed)
            {
                return Json(new { status = Result.Failed, msg = "Liabilities has been completed !" }, JsonRequestBehavior.AllowGet);
            }
            //if (model.DebitType == (byte)DebitHistoryType.mustCollect)
            //{
            //    return Json(new { status = Result.Failed, msg = "Type công nợ processing request không chính xác !" }, JsonRequestBehavior.AllowGet);
            //}

            //2. Kiểm tra xem debit cha có tồn tại hay không ?
            var debitDetail = UnitOfWork.DebitRepo.FirstOrDefault(x => !x.IsDelete && x.IsSystem == true && x.Id == debitHistoryDetail.DebitId);
            if (debitDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Parent liabilities do not exist or have been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra thông tin khách hàng có tồn tại hay không
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == model.SubjectId && x.Email == model.SubjectEmail);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Customer does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (customerDetail.BalanceAvalible < debitHistoryDetail.Money)
            {
                return Json(new { status = Result.Failed, msg = "The balance of customer e-wallet is not enough to settle the debt !" }, JsonRequestBehavior.AllowGet);
            }

            //4. Kiểm tra thông tin định khoản có chính xác hay không ?
            //var treasureDetail = UnitOfWork.TreasureRepo.FirstOrDefault(x => !x.IsDelete && x.Id == debitHistoryDetail.PayReceivableId);
            //if (treasureDetail == null)
            //{
            //    return Json(new { status = Result.Failed, msg = "Account balancing information is not correct !" }, JsonRequestBehavior.AllowGet);
            //}
            //if (treasureDetail.IsIdSystem == false)
            //{
            //    return Json(new { status = Result.Failed, msg = "Cannot select auto balancing account from the system  !" }, JsonRequestBehavior.AllowGet);
            //}
            //if (treasureDetail.Operator != false)
            //{
            //    return Json(new { status = Result.Failed, msg = "Type định khoản không phải công nợ phải trả !" }, JsonRequestBehavior.AllowGet);
            //}

            //5. Kiểm tra định khoản ví điện tử tương ứng đã tồn tại hay chưa
            var customerWalletDetail = UnitOfWork.CustomerWalletRepo.FirstOrDefault(x => !x.IsDelete && x.Idd == debitHistoryDetail.PayReceivableIdd);
            if (customerWalletDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "The corresponding e - wallet debt settlement does not exist !" }, JsonRequestBehavior.AllowGet);
            }

            //6. Cập nhật vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1. Cập nhật lại vào Debit
                    debitDetail.MustReturnMoney -= debitHistoryDetail.Money;
                    debitDetail.LastUpdated = timeNow;

                    UnitOfWork.DebitRepo.Update(debitDetail);
                    UnitOfWork.DebitRepo.Save();

                    //3.2. Cập nhật lại vào DebitHistory
                    debitHistoryDetail.Status = (byte)DebitHistoryStatus.Completed;
                    debitHistoryDetail.LastUpdated = timeNow;

                    debitHistoryDetail.Money = model.Money;
                    debitHistoryDetail.LastUpdated = timeNow;
                    debitHistoryDetail.Note = model.Note;

                    UnitOfWork.DebitHistoryRepo.Update(debitHistoryDetail);
                    UnitOfWork.DebitHistoryRepo.Save();

                    //3.4. Cập nhật bản ghi RechargeBill
                    var reachargeBillOfDay = UnitOfWork.RechargeBillRepo.Count(x =>
                                                                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                                                                        x.Created.Day == DateTime.Now.Day);

                    var recharBillDetail = new RechargeBill();
                    // Kiểm tra lại thông tin đối tượng

                    recharBillDetail.CustomerId = customerDetail.Id;
                    recharBillDetail.CustomerEmail = customerDetail.Email;
                    recharBillDetail.CustomerPhone = customerDetail.Phone;
                    recharBillDetail.CustomerName = customerDetail.FullName;

                    //3.5. Cập nhật vào phiếu nạp/Subtract funds tự động
                    recharBillDetail.Code = $"{reachargeBillOfDay}{DateTime.Now:ddMMyy}";
                    recharBillDetail.Type = (byte)RechargeBillType.Increase;
                    recharBillDetail.Status = (byte)RechargeBillStatus.Approved;

                    // Số tiền
                    recharBillDetail.CurrencyFluctuations = (decimal)debitHistoryDetail.Money;
                    recharBillDetail.Increase = (decimal)debitHistoryDetail.Money;
                    recharBillDetail.Diminishe = 0;

                    recharBillDetail.CurencyStart = customerDetail.BalanceAvalible;
                    recharBillDetail.CurencyEnd = customerDetail.BalanceAvalible + (decimal)debitHistoryDetail.Money;

                    //Thông tin định khoản
                    recharBillDetail.TreasureId = customerWalletDetail.Id;
                    recharBillDetail.TreasureName = customerWalletDetail.Name;

                    // Gắn thông tin người tạo
                    recharBillDetail.UserId = UserState.UserId;
                    recharBillDetail.UserName = UserState.UserName;

                    // Gắn lại thông tin Orders
                    recharBillDetail.OrderId = debitHistoryDetail.OrderId;
                    recharBillDetail.OrderCode = debitHistoryDetail.OrderCode;
                    recharBillDetail.OrderType = debitHistoryDetail.OrderType;

                    // Gắn thông tin người duyệt
                    recharBillDetail.UserApprovalId = UserState.UserId;
                    recharBillDetail.UserApprovalName = UserState.UserName;

                    UnitOfWork.RechargeBillRepo.Add(recharBillDetail);
                    UnitOfWork.RechargeBillRepo.Save();

                    //3.4. Cập nhật lại số dư ví điện tử khách hàng
                    customerDetail.BalanceAvalible += (decimal)debitHistoryDetail.Money;

                    UnitOfWork.CustomerRepo.Update(customerDetail);
                    UnitOfWork.CustomerRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
                //catch (Exception)
                //{
                //    transaction.Rollback();
                //    return Json(new { status = Result.Failed, msg = "Lỗi cập nhật công nợ, vui lòng thử lại !" }, JsonRequestBehavior.AllowGet);
                //}

                return Json(new { status = Result.Succeed, msg = "Account payable handled successfully! Customer e-wallet has been deposited !" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}