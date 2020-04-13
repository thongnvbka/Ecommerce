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

namespace Cms.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        [LogTracker(EnumAction.View, EnumPage.Dashboard)]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Thống kê tình hình tài khoản người dùng trong ngày
        /// </summary>
        /// <param name="day"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportUser()
        {
            decimal totalCustomer = 0;
            decimal totalPotentialCustomer = 0;
            decimal totalStaff = 0;

            //1. Lây danh sách Account
            totalCustomer = UnitOfWork.CustomerRepo.FindAsNoTracking(x => !x.IsDelete).Count();

            //2. Lây danh sách Account tiềm năng
            totalPotentialCustomer = UnitOfWork.PotentialCustomerRepo.FindAsNoTracking(x => !x.IsDelete).Count();

            //2. Lấy danh sách tài khoản nhân viên
            totalStaff = UnitOfWork.UserRepo.FindAsNoTracking(x => !x.IsDelete).Count();

            return Json(new { totalCustomer, totalPotentialCustomer, totalStaff }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: Tình hình xử lý Orders Order trong ngày
        /// </summary>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportOrderOfDay(DateTime? day, DateTime? end)
        {
            decimal expectedRevenueOrder = 0;
            decimal expectedProfitOrder = 0;

            //1. Lây danh sach Orders xử lý trong ngày
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            //Lấy danh sách lịch sử Orders đặt hàng thành công   

            var listOrderSuccess = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete && x.Type == (byte)OrderType.Order)
                                                                     .Join(
                                                                             UnitOfWork.OrderHistoryRepo.Entities.Where(x =>x.CreateDate >= startOfDay && x.CreateDate <= endOfDay),
                                                                             order => order.Id,
                                                                             orderH => orderH.OrderId,
                                                                             (o, oh) => new
                                                                             {
                                                                                 oh.Status,
                                                                                 o.Total,
                                                                                 o.ExchangeRate,
                                                                                 o.UserId,
                                                                                 o.UserFullName,
                                                                                 PriceBargain = (o.PriceBargain == null ? 0 : o.PriceBargain)* o.ExchangeRate
                                                                             }
                                                                             )
                                                                             .OrderBy(x => x.Status)
                                                                             .ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                var status = (int)orderStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = orderStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listOrderSuccess.Count(x => x.Status == status)
                };

                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailOrder = new List<int>();
            var detailPrice = new List<dynamic>();

            foreach (var order in listOrderSuccess.Where(x => x.Status == (int)OrderStatus.OrderSuccess).GroupBy(x => x.UserId).ToList())
            {
                var firstOrDefault = order.FirstOrDefault();
                if (firstOrDefault != null) detailName.Add(firstOrDefault.UserFullName);
                detailOrder.Add(order.Count());
                detailPrice.Add(order.Sum(x => x.Total));

                expectedRevenueOrder += order.Sum(x => x.Total);
                expectedProfitOrder += order.Sum(s => s.PriceBargain ?? 0);
                //if(order.Count() > 0)
                //{
                //    foreach(var item in order)
                //    {
                //        item.o.PriceBargain = item.o.PriceBargain * item.o.ExchangeRate;
                //    }
                //    expectedProfitOrder += order.Sum(s => s.o.PriceBargain ?? 0);
                //}

            }

            //4. Tính toán ESTIMATED REVENUE BY ORDER


            //5. Trả kết quả lên view
            return Json(new { overview, detailName, detailOrder, detailPrice, expectedRevenueOrder, expectedProfitOrder}, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: Tình hình xử lý Orders Thương Mại trong ngày
        /// </summary>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportCommerceOfDay(DateTime? day, DateTime? end)
        {
            decimal expectedRevenueOrder = 0;
            decimal expectedProfitOrder = 0;

            //1. Lây danh sach Orders xử lý trong ngày
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            
            var listOrderSuccess = UnitOfWork.OrderRepo.FindAsNoTracking(x => !x.IsDelete && x.Type == (byte)OrderType.Commerce)
                                                                     .Join(
                                                                             UnitOfWork.OrderHistoryRepo.Entities.Where(x => x.Status == (int)OrderStatus.OrderSuccess && x.CreateDate >= startOfDay
                                                                                                                                                                       && x.CreateDate <= endOfDay),
                                                                             order => order.Id,
                                                                             orderH => orderH.OrderId,
                                                                             (o, oh) => new { o, oh }
                                                                             )
                                                                             .OrderBy(x => x.o.Status)
                                                                             .ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                var status = (int)orderStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = orderStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listOrderSuccess.Count(x => x.oh.Status == status)
                };

                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailOrder = new List<int>();
            var detailPrice = new List<dynamic>();

            foreach (var order in listOrderSuccess.Where(x => x.oh.Status == (int)OrderStatus.OrderSuccess).GroupBy(x => x.o.UserId).ToList())
            {
                var firstOrDefault = order.FirstOrDefault();
                if (firstOrDefault != null) detailName.Add(firstOrDefault.o.UserFullName);
                detailOrder.Add(order.Count());
                detailPrice.Add(order.Sum(x => x.o.Total));

                expectedRevenueOrder += order.Sum(x => x.o.Total);

            }

            //4. Tính toán ESTIMATED REVENUE BY ORDER


            //5. Trả kết quả lên view
            return Json(new { overview, detailName, detailOrder, detailPrice, expectedRevenueOrder, expectedProfitOrder }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: Tình hình xử lý Orders kí gửi trong ngày
        /// </summary>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportDepositOfDay(DateTime? day, DateTime? end)
        {
            decimal expectedRevenueDeposit = 0;
            decimal expectedProfitDeposit = 0;

            //1. Lây danh sach Orders xử lý trong ngày
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            
            var listDeposit = UnitOfWork.OrderRepo.Entities.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Deposit)
                                                                     .Join(
                                                                             UnitOfWork.OrderHistoryRepo.Entities.Where(x=>x.Status == (int)DepositStatus.Pending
                                                                                                                            && x.CreateDate >= startOfDay
                                                                                                                            && x.CreateDate <= endOfDay),
                                                                             deposit => deposit.Id,
                                                                             depositH => depositH.OrderId,
                                                                             (d, dh) => new { d, dh }
                                                                             )
                                                                             .OrderBy(x => x.d.Status)
                                                                             .ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (DepositStatus depositStatus in Enum.GetValues(typeof(DepositStatus)))
            {
                var status = (int)depositStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = depositStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listDeposit.Count(x => x.dh.Status == status)
                };

                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailDeposit = new List<int>();
            var detailPrice = new List<dynamic>();

            foreach (var deposit in listDeposit.Where(x => x.dh.Status == (int)DepositStatus.Pending).GroupBy(x => x.d.UserId).ToList())
            {
                var firstOrDefault = deposit.FirstOrDefault();
                if (firstOrDefault != null) detailName.Add(firstOrDefault.d.UserFullName);
                detailDeposit.Add(deposit.Count());
                detailPrice.Add(deposit.Sum(x => x.d.Total));
                
                expectedRevenueDeposit += deposit.Sum(x => x.d.Total);
            }

            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailDeposit, detailPrice, expectedRevenueDeposit, expectedProfitDeposit }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: THE DEALING OF DAILY HANDLING APPLICATIONS DURING THE DAY
        /// </summary>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportSourcingOfDay(DateTime? day, DateTime? end)
        {
            decimal expectedRevenueSourcing = 0;
            decimal expectedProfitSourcing = 0;

            //1. Lây danh sach Orders xử lý trong ngày
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            //Lấy danh sách lịch sử Orders
           
            var listSourcing = UnitOfWork.OrderRepo.Entities.Where(x => !x.IsDelete && x.Type == (byte)OrderType.Source)
                                                                     .Join(
                                                                             UnitOfWork.OrderHistoryRepo.Entities.Where(x => x.CreateDate >= startOfDay
                                                                                                                            && x.CreateDate <= endOfDay),
                                                                             deposit => deposit.Id,
                                                                             depositH => depositH.OrderId,
                                                                             (s, sh) => new { s, sh }
                                                                             )
                                                                             .OrderBy(x => x.s.Status)
                                                                             .ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (SourceStatus sourceStatus in Enum.GetValues(typeof(SourceStatus)))
            {
                var status = (int)sourceStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = sourceStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listSourcing.Count(x => x.sh.Status == status)
                };

                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailSourcing = new List<int>();
            var detailPrice = new List<dynamic>();

            foreach (var source in listSourcing.Where(x => x.sh.Status == (int)SourceStatus.Success).GroupBy(x => x.s.UserId).ToList())
            {
                var firstOrDefault = source.FirstOrDefault();
                if (firstOrDefault != null) detailName.Add(firstOrDefault.s.UserFullName);
                detailSourcing.Add(source.Count());
                detailPrice.Add(source.Sum(x => x.s.Total));

                expectedRevenueSourcing += source.Sum(x => x.s.Total);

            }

            //4. Tính toán ESTIMATED REVENUE BY ORDER


            //5. Trả kết quả lên view
            return Json(new { overview, detailName, detailSourcing, detailPrice, expectedRevenueSourcing, expectedProfitSourcing }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: Tình hình tài chính trong ngày
        /// </summary>
        /// <returns></returns>
        [CheckPermission(EnumAction.View, EnumPage.Dashboard)]
        public JsonResult ReportAccountantOfDay(DateTime? day, DateTime? end)
        {
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            //DateTime startOfDay = GetStartOfDay(DateTime.Now);
            //DateTime endOfDay = GetEndOfDay(DateTime.Now);

            decimal totalFundBillVNAddOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.VND.ToString(), startOfDay, endOfDay).Increase ?? 0;
            decimal totalFundBillVNMinusOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.VND.ToString(), startOfDay, endOfDay).Diminishe ?? 0;

            decimal totalFundBillCNAddOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.CNY.ToString(), startOfDay, endOfDay).Increase ?? 0;
            decimal totalFundBillCNMinusOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.CNY.ToString(), startOfDay, endOfDay).Diminishe ?? 0;

            decimal totalFundBillALPAddOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.ALP.ToString(), startOfDay, endOfDay).Increase ?? 0;
            decimal totalFundBillALPMinusOfDay = UnitOfWork.FundBillRepo.FundBillFinanceFund(Currency.ALP.ToString(), startOfDay, endOfDay).Diminishe ?? 0;

            decimal totalRechargeBillAddOfDay = UnitOfWork.FundBillRepo.RechargeBillFinanceFund(startOfDay, endOfDay).Increase ?? 0;
            decimal totalRechargeBillMinusOfDay = UnitOfWork.FundBillRepo.RechargeBillFinanceFund(startOfDay, endOfDay).Diminishe ?? 0;

            ////1. Lấy danh sách các phiếu nạp quỹ thành công trong Today
            //var fundbillOfDay = await UnitOfWork.FundBillRepo.FindAsNoTrackingAsync(
            //    x => !x.IsDelete
            //         && x.Status == (int)FundBillStatus.Approved
            //         && x.Created >= startOfDay && x.Created <= endOfDay
            //);

            //if (fundbillOfDay.Count > 0)
            //{
            //    //2. Thống kê nạp tiền Baht
            //    foreach (var item in fundbillOfDay)
            //    {
            //        //Nạp tiền
            //        if (item.Type == (byte)FundBillType.Increase)
            //        {
            //            totalFundBillVNAddOfDay += item.CurrencyFluctuations;
            //        }
            //        //Chi tiền
            //        if (item.Type == (byte)FundBillType.Diminishe)
            //        {
            //            totalFundBillVNMinusOfDay += item.CurrencyFluctuations;
            //        }
            //    }
            //}

            ////2. Lấy danh sách các phiếu ví thành công trong Today
            //var rechargeBillOfDay = await UnitOfWork.RechargeBillRepo.FindAsNoTrackingAsync(
            //    x => !x.IsDelete
            //         && x.Status == (int)RechargeBillStatus.Approved
            //         && x.Created >= startOfDay && x.Created <= endOfDay
            //);
            //if (rechargeBillOfDay.Count > 0)
            //{
            //    //2. Thống kê nạp tiền
            //    foreach (var item in rechargeBillOfDay)
            //    {
            //        //Nạp tiền ví
            //        if (item.Type == (byte)RechargeBillType.Increase)
            //        {
            //            totalRechargeBillAddOfDay += item.CurrencyFluctuations;
            //        }
            //        //Chi tiền ví
            //        if (item.Type == (byte)RechargeBillType.Diminishe)
            //        {
            //            totalRechargeBillMinusOfDay += item.CurrencyFluctuations;
            //        }
            //    }
            //}

            return Json(new { totalFundBillVNAddOfDay, totalFundBillVNMinusOfDay, totalRechargeBillAddOfDay, totalRechargeBillMinusOfDay, totalFundBillCNAddOfDay, totalFundBillCNMinusOfDay, totalFundBillALPAddOfDay, totalFundBillALPMinusOfDay }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê Tình hình xử lý ticket trong ngày
        /// </summary>
        /// <returns></returns>
        public JsonResult ReportTicketOfDay(DateTime? day, DateTime? end)
        {
            //1. Lây danh sach ticket xử lý trong ngày
            var startOfDay = GetStartOfDay(day ?? DateTime.Now);
            var endOfDay = GetEndOfDay(end ?? DateTime.Now);
            var listTicket = UnitOfWork.ComplainRepo.FindAsNoTracking(x =>
                                                                            x.LastUpdateDate >= startOfDay
                                                                            && x.LastUpdateDate <= endOfDay
                                                                            && !x.IsDelete)
                                                                            .OrderBy(x => x.Status
                                                                            ).ToList();

            //2. Lọc theo trạng thái
            var overview = new List<dynamic>();
            foreach (ComplainStatus ticketStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                var status = (int)ticketStatus;
                if (status == 0) continue;
                var data = new dynamic[] { ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, listTicket.Count(x => x.Status == status) };
                overview.Add(data);
            }

            //3. Tạo các dữ liệu theo báo cáo
            var detailName = new List<string>();
            var detailTicket = new List<int>();
            var listTicket1 = new List<ComplainUser>();
            var complainUser = UnitOfWork.ComplainUserRepo.Find(s=> s.IsCare == true);
            foreach (var item in listTicket.Where(s => s.Status == (int)ComplainStatus.Success))
            {
                var x = complainUser.FirstOrDefault(s => s.ComplainId == item.Id);
                if(x!= null)
                {
                    listTicket1.Add(x);
                }
                
            };
            if(listTicket1.Count() >0)
            {
                foreach (var ticket in listTicket1.GroupBy(x => x.UserId).ToList())
                {
                    var firstOrDefault = ticket.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        detailName.Add(firstOrDefault.UserName);
                        detailTicket.Add(ticket.Count());
                    }
                        
                }
            }
            

            //4. Trả kết quả lên view
            return Json(new { overview, detailName, detailTicket }, JsonRequestBehavior.AllowGet);
        }

    }
}