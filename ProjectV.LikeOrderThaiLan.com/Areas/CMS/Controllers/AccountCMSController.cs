using Common.Constant;
using Common.Emums;
using Common.Host;
using Common.Items;
using Library.DbContext.Entities;
using Library.Models;
using Library.ViewModels.Account;
using Library.ViewModels.Items;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Globalization;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    [Authorize]
    public class AccountCMSController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.ActiveAccount = "cl_on";

            return View();
        }

        public async Task<JsonResult> SelectInforCustomer()
        {
            ViewBag.ActiveAccount = "cl_on";
            //var model = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.IsActive == true && x.IsDelete == false && x.Id == CustomerState.Id);
            //var topNotifi = UnitOfWork.NotifiCommonRepo.FirstOrDefault(x => x.IsRead == false);

            //return Json(CustomerState == null ?
            //    new
            //    {
            //        status = Result.Failed,
            //        msg = "Lấy thông tin lỗi !",
            //        model,
            //        topNotifi
            //    }
            //    : new
            //    {
            //        status = Result.Succeed,
            //        msg = "Lấy thông tin thành công !",
            //        model,
            //        topNotifi
            //    },
            //    JsonRequestBehavior.AllowGet);

            //1. Lấy thông tin khách hàng
            var userDetail = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.IsActive && x.Id == CustomerState.Id);
            if (userDetail == null)
            {
                return Json(new { status = Result.Failed, msg = Resource.KHKhongTonTai }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin các đơn hàng trong giỏ hàng của khách hàng này
            var order = UnitOfWork.OrderRepo.Find(x => !x.IsDelete && x.CustomerId == CustomerState.Id && x.SystemId == userDetail.SystemId).ToList();
            // Đơn hàng đang trong giỏ hàng
            var orderNew = order.Count(x => x.Status == (byte)OrderStatus.New && x.CustomerId == CustomerState.Id && x.SystemId == SystemId && !x.IsDelete && x.Type == (byte)OrderType.Order);
            // Đơn hàng đang chờ đặt cọc
            var orderAwait = order.Count(x => x.Status == (byte)OrderStatus.WaitDeposit && x.CustomerId == CustomerState.Id && x.SystemId == SystemId);

            //3. Lấy thông tin Notification chưa đọc
            var notification = UnitOfWork.NotificationRepo.Find(x => x.IsRead == false && x.CustomerId == CustomerState.Id && x.SystemId == SystemId).Count();

            //4. Show lên View
            return Json(new { status = Result.Succeed, msg = Resource.LayThongTinThanhCong, userDetail, orderNew, orderAwait, notification }, JsonRequestBehavior.AllowGet);
            //return Json(new { status = Result.Succeed, msg = "Lấy thông tin thành công !", userDetail, orderNew, orderAwait, notification }, JsonRequestBehavior.AllowGet);
        }

        #region lich su giao dich

        //todo danh sách lịch sử giao dịch
        public ActionResult ListTotalRecharge()
        { 
            ViewBag.ActiveLstRechange = "cl_on";
            return View();
        }

        //todo show ra danh sách lịch sử giao dịch
        [HttpPost]
        public async Task<JsonResult> GetListRecharge(int page, int pageSize)
        {
            long totalRecord;
            var rechargeModal = new List<RechargeBill>();

            rechargeModal = await UnitOfWork.RechargeBillRepo.FindAsync(
                out totalRecord,
                x => (x.CustomerId == CustomerState.Id)
                && (x.IsDelete == false),
                x => x.OrderByDescending(y => y.Created),
                page,
                pageSize
                );

            return Json(CustomerState == null ?
                new
                {
                    status = Result.Failed,
                    msg =Resource.LayThongTinLoi,
                    //msg = "Lấy thông tin lỗi !",
                    totalRecord,
                    rechargeModal
                }
            : new
            {
                status = Result.Succeed,
                msg = Resource.LayThongTinThanhCong,
                //msg = "Lấy thông tin thành công !",
                totalRecord,
                rechargeModal
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetInit()
        {
            var totalRecharge = 0;
            // Tính toán tổng số lịch sử giao dịch
            var listRecharge = await UnitOfWork.RechargeBillRepo.FindAsNoTrackingAsync(x => x.IsDelete == false && (x.CustomerId == CustomerState.Id));

            var totalNotification = 0;
            // Tính toán tổng số lịch sử giao dịch
            var listNotification = await UnitOfWork.NotificationRepo.FindAsNoTrackingAsync(x => x.CustomerId == CustomerState.Id && x.SystemId == SystemId);

            var totalNotificationNoRead = 0;
            // Tính toán tổng số lịch sử giao dịch
            var listNotificationNoRead = await UnitOfWork.NotificationRepo.FindAsNoTrackingAsync(
                     x => x.CustomerId == CustomerState.Id && !x.IsRead && x.SystemId == SystemId
                );
            if (CustomerState == null) return Json(new { totalRecharge, totalNotification, totalNotificationNoRead }, JsonRequestBehavior.AllowGet);

            totalNotification = listNotification.Count;
            totalRecharge = listRecharge.Count;
            totalNotificationNoRead = listNotificationNoRead.Count;

            return Json(new { totalRecharge, totalNotification, totalNotificationNoRead }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetInitCommon()
        {
            var totalNotificationCommonNoRead = 0;
            // Tính toán tổng số thong bao chung chua doc
            var listNotificationCommonNoRead = await UnitOfWork.NotifiCommonRepo.FindAsNoTrackingAsync(
                     x => (x.IsRead == false) && x.SystemId==SystemId && !x.Status
                );

            var totalNotificationCommon = 0;
            // Tính toán tổng số thong bao chung
            var listNotificationCommon = await UnitOfWork.NotifiCommonRepo.FindAsNoTrackingAsync(
                     x =>!x.Status && x.SystemId == SystemId
                );

            if (CustomerState == null)
                return Json(new { totalNotificationCommonNoRead, totalNotificationCommon }, JsonRequestBehavior.AllowGet);

            totalNotificationCommonNoRead = listNotificationCommonNoRead.Count;
            totalNotificationCommon = listNotificationCommon.Count;
            return Json(new { totalNotificationCommonNoRead, totalNotificationCommon }, JsonRequestBehavior.AllowGet);
        }

        #endregion lich su giao dich

        //todo  nạp tiền
        public ActionResult Rechange()
        { 
            ViewBag.ActiveRechange = "cl_on"; 
            var model = UnitOfWork.CustomerRepo.FirstOrDefault(x => x.Id == CustomerState.Id);
            return View(model);
        }

        //todo tích điểm
        public ActionResult Point()
        { 
            ViewBag.ActivePoint = "cl_on";
            return View();
        }

        //todo mã giảm giá
        public ActionResult DiscountCodes()
        { 
            ViewBag.ActiveDiscountCodes = "cl_on";
            return View();
        }

        #region Thong bao riêng cho khách hàng

        //todo thông báo
        public ActionResult Notification()
        {
           
            ViewBag.ActiveNotification = "cl_on";
            return View();
        }

        //todo show ra danh sách thông báo
        [HttpPost]
        public async Task<JsonResult> GetListNotification(SearchInfor seachInfor, int page, int pageSize)
        {
            long totalRecord;
            //0. Kiểm tra trạng thái đăng nhập của khách hàng
            if (CustomerState == null)
            {
                return Json(new { status = Result.Failed, msg = Resource.LayThongTinLoi }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = Result.Failed, msg = "Lấy thông tin lỗi !" }, JsonRequestBehavior.AllowGet);
            }
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" || seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            //1. Lấy thông tin thông báo cho khách hàng

            var notiCustomer = await UnitOfWork.NotificationRepo.FindAsync(
                    out totalRecord,
                    x => x.CustomerId == CustomerState.Id && x.SystemId == SystemId && (seachInfor.AllTime == -1 || ((seachInfor.StartDate == null || x.CreateDate >= seachInfor.StartDate) && (seachInfor.FinishDate == null || x.CreateDate <= seachInfor.FinishDate))),
                    x => x.OrderBy(z => z.IsRead).ThenByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );

            var totalNoti = notiCustomer.Count();

            //2. Tổng số thông báo đã đọc
            var totalReaded = notiCustomer.Where(x => x.IsRead);

            return Json(new { status = Result.Succeed, msg = Resource.LayThongTinThanhCong, totalRecord, totalReaded, notiCustomer }, JsonRequestBehavior.AllowGet);
            //return Json(new { status = Result.Succeed, msg = "Lấy thông tin thành công !", totalRecord, totalReaded, notiCustomer }, JsonRequestBehavior.AllowGet);
        }

        //todo xóa thông báo
        [HttpPost]
        public ActionResult DeleteNotitfiCation(long notiId)
        {
            //Todo lấy ra mã id của notification
            var tmpNoti = UnitOfWork.NotificationRepo.FirstOrDefault(x => x.Id == notiId);
            //Todo kiểm tra
            if (tmpNoti == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            //Todo xóa bỏ
            UnitOfWork.NotificationRepo.Remove(tmpNoti);
            var rs = UnitOfWork.NotificationRepo.SaveAsync();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DetailNotitication()
        {
            ViewBag.ActiveNotification = "cl_on";
            return View();
        }
        //Todo Chi tiết thông báo
        [HttpPost]
        public async Task<JsonResult> DetailNotitication(int notificationId)
        {
            var result = true;
            //todo lấy ra chi tiết thông báo
            var notification = await UnitOfWork.NotificationRepo.FirstOrDefaultAsync(p => p.Id == notificationId);
            if (notification == null)
            {
                result = false;
            }
            else
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        notification.IsRead = true;
                        notification.UpdateDate = DateTime.Now;
                        UnitOfWork.NotificationRepo.Save();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return Json(new { status = false, msg = "Error" }, JsonRequestBehavior.AllowGet);
                        //return Json(new { status = false, msg = "Lỗi" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result, notification }, JsonRequestBehavior.AllowGet);
        }

        #endregion Thong bao riêng cho khách hàng

        #region Thông báo chung cho khách hàng

        //todo thông báo
        public ActionResult NotificationCommon()
        { 
            ViewBag.ActiveNotificationCommon = "cl_on";
            return View();
        }

        //todo show ra danh sách thông báo
        [HttpPost]
        public async Task<JsonResult> GetListNotificationCommon(int page, int pageSize)
        {
            long totalRecord;
            //0. Kiểm tra trạng thái đăng nhập của khách hàng
            if (CustomerState == null)
            {
                return Json(new { status = Result.Failed, msg = Resource.LayThongTinLoi }, JsonRequestBehavior.AllowGet);
                //return Json(new { status = Result.Failed, msg = "Lấy thông tin lỗi !" }, JsonRequestBehavior.AllowGet);
            }

            //1. Lấy thông tin thông báo cho khách hàng
            var notification = await UnitOfWork.NotifiCommonRepo.FindAsync(
                    out totalRecord,
                    x => !x.Status && x.SystemId == SystemId,
                    x => x.OrderBy(z => z.IsRead).ThenByDescending(y => y.CreateDate),
                    page,
                    pageSize
                );

            var totalNoti = notification.Count();

            //2. Số danh sách đã đọc
            var totalRead = UnitOfWork.NotifiCustomerRepo.Find(x => !x.IsRead && x.CustomerId == CustomerState.Id && x.SystemId == SystemId).Count();

            //3. Số tin nhắn chưa đọc
            var totalNoRead = totalNoti - totalRead;

            //4. Show lên View
            return Json(new { status = Result.Succeed, msg = Resource.LayDSTBThanhCong , totalRecord, totalRead, totalNoRead, notification }, JsonRequestBehavior.AllowGet);
        }

        //todo xóa thông báo
        [HttpPost]
        public ActionResult DeleteNotificationCommon(long notiId)
        {
            //Todo lấy ra mã id của notification
            var tmpNoti = UnitOfWork.NotifiCommonRepo.FirstOrDefault(x => x.Id == notiId);
            //Todo kiểm tra
            if (tmpNoti == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            //Todo xóa bỏ
            UnitOfWork.NotifiCommonRepo.Remove(tmpNoti);
            var rs = UnitOfWork.NotifiCommonRepo.SaveAsync();
            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        //Todo Chi tiết thông báo
        [HttpPost]
        public async Task<JsonResult> DetailNotificationCommon(int notificationId)
        {
            var result = true;
            //todo lấy ra chi tiết thông báo
            var notification = await UnitOfWork.NotifiCommonRepo.FirstOrDefaultAsync(p => p.Id == notificationId);
            if (notification == null)
            {
                result = false;
            }
            else
            {
                using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        notification.IsRead = true;
                        notification.UpdateDate = DateTime.Now;
                        UnitOfWork.NotificationRepo.Save();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return Json(new { status = false, msg = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new { result, notification }, JsonRequestBehavior.AllowGet);
        }

        #endregion Thông báo chung cho khách hàng

        //todo trả lời tư vấn
        public ActionResult AnswerMessage()
        {
            
            ViewBag.ActiveAnswerMessage = "cl_on";
            return View();
        }

        //todo danh sách Công nợ
        public ActionResult Liabilitie(SearchInfor seachInfor, PageItem pageInfor)
        {
            
            ViewBag.ActiveLiabilitie = "cl_on";
            return View();
        }

        [HttpPost]
        public JsonResult GetAllDebitList(SearchInfor seachInfor, PageItem pageInfor)
        {
            var model = new DebitHistoryModelV2();
            if (!string.IsNullOrEmpty(seachInfor.StartDateS))
            {
                seachInfor.StartDate = DateTime.ParseExact(seachInfor.StartDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(seachInfor.FinishDateS))
            {
                seachInfor.FinishDate = DateTime.ParseExact(seachInfor.FinishDateS, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
            seachInfor.SystemId = SystemId;
            seachInfor.CustomerId = CustomerState.Id;
            if (seachInfor.StartDate.ToString("dd/MM/yyyy") == "01/01/0001" || seachInfor.FinishDate.ToString("dd/MM/yyyy") == "01/01/0001")
            {
                seachInfor.AllTime = -1;
            }
            if (string.IsNullOrEmpty(seachInfor.Keyword))
            {
                seachInfor.Keyword = "";
            }
            model = UnitOfWork.DebitHistoryRepo.GetAllByLinq(pageInfor, seachInfor);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllDebitHistoryList(int debitId)
        {
            byte debitType = (byte)DebitType.Return;
            var debitHistoryList = UnitOfWork.DebitHistoryRepo.Find(s => s.DebitId == debitId && s.DebitType == debitType && s.SubjectId == CustomerState.Id);
            return Json(new { debitHistoryList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDataByOrderId(int orderId)
        {
            var model = new List<DebitHistory>();
            if (CustomerState == null)
            {
                return PartialView(model);
            }
            else
            {
                byte debitType = (byte)DebitType.Return;
                model = UnitOfWork.DebitHistoryRepo.Find(m => m.DebitId == orderId && m.SubjectId == CustomerState.Id && m.DebitType == debitType).ToList();
                return PartialView(model);
            }
        }

        #region Claim

        //todo yêu cầu rút tiền
        public ActionResult Claim()
        { 
            ViewBag.ActiveClaim = "cl_on";
            var customerId = CustomerState.Id;
            var tmpCustomer = UnitOfWork.CustomerRepo.FirstOrDefault(m => m.Id == customerId);
            if (tmpCustomer != null)
            {
                ViewBag.BalanceAvalible = tmpCustomer.BalanceAvalible;
            }
            else
            {
                ViewBag.BalanceAvalible = 0;
            }

            return View();
        }

        [HttpPost]
        public async Task<int> SaveAdvance(Draw item)
        {

            var result = 0;
            if (CustomerState == null)
            {
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(CustomerState.Email))
                {
                    return result;
                }
            }
            item.CustomerId = CustomerState.Id;
            item.Status = 0;
            item.CreateDate = DateTime.Now;
            item.LastUpdate = DateTime.Now;
            item.CustomerName = CustomerState.FullName;
            item.CustomerEmail = CustomerState.Email;
            item.CustomerPhone = CustomerState.Phone;
            item.CustomerId = CustomerState.Id;
            item.SystemId = SystemId;
            item.SystemName = SystemName;
            item.Note = Resource.YCRutTien;
            UnitOfWork.DrawRepo.Add(item);
            var depositOfDay = UnitOfWork.DrawRepo.Count(x =>
                        x.CreateDate.Year == item.CreateDate.Year && x.CreateDate.Month == item.CreateDate.Month &&
                        x.CreateDate.Day == item.CreateDate.Day) + 1; 

            item.Code = $"{item.CreateDate:ddMMyy}{depositOfDay}";
            result = await UnitOfWork.DrawRepo.SaveAsync();
            return result;
        }

        public int CheckPass(string pass)
        {
            var result = 0;

            if (string.IsNullOrEmpty(CustomerState?.Email))
            {
                return 0;
            }

            try
            {
                var tmpPass = "";
                var customerId = CustomerState.Id;
                var tmpCustomer = UnitOfWork.CustomerRepo.FirstOrDefault(m => m.Id == customerId);
                if (tmpCustomer != null)
                {
                    tmpPass = tmpCustomer.Password;
                }
                if (!Common.PasswordEncrypt.PasswordEncrypt.EncodePassword(pass.Trim(), Common.Constant.PasswordSalt.FinGroupApiCustomer).Equals(tmpPass))
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                OutputLog.WriteOutputLog(ex);
            }
            return result;
        }

        //todo show ra danh sách lịch sử rút tiền
        [HttpPost]
        public async Task<JsonResult> GetListDraw(int page, int pageSize)
        {
            // Todo cần lấy danh sách lịch sử rút tiền theo mã khách hàng thuộc hệ thống likeorder{SystemId=5}
            long totalRecord;
            var listItems = new List<Draw>();

            listItems = await UnitOfWork.DrawRepo.FindAsync(
                out totalRecord,
                x => (x.SystemId == SystemId)
                     && (CustomerState.Id == x.CustomerId),
                x => x.OrderByDescending(y => y.CreateDate),
                page,
                pageSize
                );

            return Json(CustomerState == null ? new { status = Result.Failed, msg = Resource.LayThongTinLoi, totalRecord, listItems }
           // return Json(CustomerState == null ? new { status = Result.Failed, msg = "Lấy thông tin lỗi !", totalRecord, listItems }
            : new { status = Result.Succeed, msg = Resource.LayThongTinThanhCong, totalRecord, listItems }, JsonRequestBehavior.AllowGet);
           // : new { status = Result.Succeed, msg = "Lấy thông tin thành công !", totalRecord, listItems }, JsonRequestBehavior.AllowGet);
        }

        #endregion Claim

        #region Thong ke chung

        public JsonResult CustomerReport(int typeSearch)
        {
            var dateNow = DateTime.Now;
            var tmpOrder = 0;
            var tmpDeposit = 0;
            var tmpSource = 0;
            var tmpComplain = 0;
            switch (typeSearch)
            {
                case 0:
                    //don order
                    tmpOrder = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Day > (dateNow.Day - 8)
                                            && m.Created.Month == dateNow.Month
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Order
                                            );
                    //don ky gui
                    tmpDeposit = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Day > (dateNow.Day - 8)
                                            && m.Created.Month == dateNow.Month
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Deposit
                                            );
                    //don tim nguon
                    tmpSource = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Day > (dateNow.Day - 8)
                                            && m.Created.Month == dateNow.Month
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Source
                                            );
                    //khieu nai
                    tmpComplain = UnitOfWork.ComplainRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.CreateDate.Day > (dateNow.Day - 8)
                                            && m.CreateDate.Month == dateNow.Month
                                            && m.CreateDate.Year == dateNow.Year
                                            );
                    break;

                case 1:
                    //don order
                    tmpOrder = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 1)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Order
                                            );
                    //don ky gui
                    tmpDeposit = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 1)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Deposit
                                            );
                    //don tim nguon
                    tmpSource = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 1)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Source
                                            );
                    //khieu nai
                    tmpComplain = UnitOfWork.ComplainRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.CreateDate.Month >= (dateNow.Month - 1)
                                            && m.CreateDate.Year == dateNow.Year
                                            );
                    break;

                case 2:
                    //don order
                    tmpOrder = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 3)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Order
                                            );
                    //don ky gui
                    tmpDeposit = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 3)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Deposit
                                            );
                    //don tim nguon
                    tmpSource = UnitOfWork.OrderRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.Created.Month >= (dateNow.Month - 3)
                                            && m.Created.Year == dateNow.Year
                                            && m.Type == (byte)OrderType.Source
                                            );
                    //khieu nai
                    tmpComplain = UnitOfWork.ComplainRepo.Count(m => !m.IsDelete && m.SystemId == SystemId
                                            && m.CustomerId == CustomerState.Id
                                            && m.CreateDate.Month >= (dateNow.Month - 3)
                                            && m.CreateDate.Year == dateNow.Year
                                            );
                    break;
            }
            var model = new CustomerReporItem()
            {
                CountComplate = tmpComplain,
                CountDeposit = tmpDeposit,
                CountOrder = tmpOrder,
                CountSource = tmpSource
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #endregion Thong ke chung
    }
}