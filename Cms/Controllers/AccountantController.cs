using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Library.DbContext.Entities;
using Library.Models;
using System;
using System.Collections.Generic;
using Common.Helper;
using Common.Emums;
using System.ComponentModel;
using Common.Constant;
using Cms.Attributes;
using Library.ViewModels.Complains;
using Library.ViewModels.Items;
using System.Web.Script.Serialization;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Globalization;
using Cms.Helpers;
using System.Runtime.ExceptionServices;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    public class AccountantController : BaseController
    {
        [LogTracker(EnumAction.View, EnumPage.FundBill, EnumPage.RechargeBill, EnumPage.WithDrawal, EnumPage.AccountantOrder, EnumPage.Debit, EnumPage.AccountantSupportTicket, EnumPage.ExecuteClaimForRefund, EnumPage.AccountantFindCustomer, EnumPage.AccountantReportFundBill, EnumPage.AccountantReportAll, EnumPage.AccountantReportMust)]
        public async Task<ActionResult> Index()
        {

            ViewBag.TreasureAddTree = await GetTreasureJsTree(oper: true);
            ViewBag.TreasureMinusTree = await GetTreasureJsTree(oper: false);

            ViewBag.FinanceFundTree = await GetFinanceFundJsTree(Currency.ALP.ToString());

            //Định khoản công nợ
            ViewBag.TreasureReturnJsTree = await GetTreasureReturnJsTree();
            ViewBag.TreasureCollectJsTree = await GetTreasureCollectJsTree();
            ViewBag.PayReceiveSearchTree = await PayReceiveSearchTree();

            ViewBag.TreasureSearchTree = await GetTreasureJsTreeSearch();
            ViewBag.FinanceFundSearchTree = await GetFinanceFundJsTreeSearch(Currency.ALP.ToString());

            //Thanh toán
            ViewBag.FinanceFundJsAccountTree = await GetFinanceFundJsAccountTree(Currency.ALP.ToString());

            //Định khoản ví điện tử
            ViewBag.TreasureWalletSearchTree = await GetTreasureWalletJsTreeSearch();
            ViewBag.TreasureWalletAddTree = await GetTreasureWalletJsTree(oper: true);
            ViewBag.TreasureWalletMinusTree = await GetTreasureWalletJsTree(oper: false);

            //Thống kê
            ViewBag.FinanceFundJsAccountantSearch = await GetFinanceFundJsAccountantSearch();

            //Hàm cập nhật các bản ghi quỹ FundBill để cập nhật lại thông tin Orders.




            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetRenderSystem()
        {
            //FundBill
            var listFundBillType = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listFinanceFund = new List<SearchMeta>();
            var listAccountantSubject = new List<SearchMeta>();
            var listFundBillStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            //RechargeBill
            var listRechargeBillType = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listCustomer = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listRechargeBillStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            //Debit
            var listDebitStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listSubjectType = new List<SearchMeta>();
            var listSubject = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listOffice = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            var listDebitType = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            //MustReturn - phai tra
            var listMustReturnStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            //WithDrawal
            var listWithDrawalStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            var listSystem = new List<dynamic>() { new { Text = "- All -", Value = -1, Class = "active", ClassChild = "label-danger" } };

            var listTicketStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            var listStatusRefund = new List<dynamic>() { new { Text = "- All -", Value = -1 } };

            var listUser = new List<SearchMeta>();

            #region FundBill

            // Lấy kiểu giao dịch với quỹ
            foreach (FundBillType funBillType in Enum.GetValues(typeof(FundBillType)))
            {
                if (funBillType >= 0)
                {
                    listFundBillType.Add(new { Text = funBillType.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillType });
                }
            }

            foreach (TicketStatus ticketStatus in Enum.GetValues(typeof(TicketStatus)))
            {
                if (ticketStatus >= 0)
                {
                    listTicketStatus.Add(new { Text = ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)ticketStatus });
                }
            }

            // Lấy danh sách đối tượng quỹ
            // TODO: Cần lấy danh sách từ bảng FinanceFund
            listFinanceFund.Add(new SearchMeta() { Text = "- All -", Value = -1 });


            // Lấy danh sách các Type đối tượng quỹ
            // TODO: Cần lấy danh sách từ bảng AccountantSubject
            var accountantSubject = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            var tempaccountantSubjectList = from p in accountantSubject
                                            select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            listAccountantSubject.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listAccountantSubject.AddRange(tempaccountantSubjectList.ToList());

            // Lấy các trạng thái Status
            foreach (FundBillStatus funBillStatus in Enum.GetValues(typeof(FundBillStatus)))
            {
                if (funBillStatus >= 0)
                {
                    listFundBillStatus.Add(new { Text = funBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillStatus });
                }
            }
            #region [Gom ví theo type, status]
            var listWallet = UnitOfWork.FundBillRepo.Find(x => !x.IsDelete).ToList();
            var listFundBillStatus1 = new List<dynamic>();
            var listFundBillType1 = new List<dynamic>();
            //5. Lấy danh sách ví theo trạng thái
            var listSystemWallet = new List<dynamic>()
            {
                new
                {
                   Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listWallet.Count,
                    ClassChild = "label-danger"
                }
            };

            // Lấy các trạng thái Status
            foreach (FundBillStatus funBillStatus in Enum.GetValues(typeof(FundBillStatus)))
            {
                if (funBillStatus >= 0)
                {
                    listFundBillStatus1.Add(new { Text = funBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillStatus });
                }
            }
            foreach (FundBillType funBillType in Enum.GetValues(typeof(FundBillType)))
            {
                if (funBillType >= 0)
                {
                    listFundBillType1.Add(new { Text = funBillType.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillType });
                }
            }

            //foreach (var item in listFundBillStatus1)
            //{
            //    listSystemWallet.Add(new
            //    {
            //        Text = item.Text,
            //        Value = item.Value,
            //        Class = "",
            //        Total = listWallet.Count(x => x.Status == item.Value),
            //        ClassChild = "label-primary"
            //    });
            //}
            foreach (var item in listFundBillType1)
            {
                listSystemWallet.Add(new
                {
                    Text = item.Text,
                    Value = item.Value,
                    Class = "",
                    Total = listWallet.Count(x => x.Type == item.Value),
                    ClassChild = "label-primary"
                });
            }
            #endregion

            #endregion

            #region [Withdrawal]

            // Lấy các trạng thái Status
            foreach (WithDrawalStatus funBillStatus in Enum.GetValues(typeof(WithDrawalStatus)))
            {
                if (funBillStatus >= 0)
                {
                    listWithDrawalStatus.Add(new { Text = funBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillStatus });
                }
            }
            var listWithdrawal = UnitOfWork.DbContext.Draws.ToList();
            var listSystemWithdrawal1 = new List<dynamic>();
            var listSystemWithdrawal = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listWithdrawal.Count,
                    ClassChild = "label-danger"
                }
            };
            foreach (WithDrawalStatus funBillStatus in Enum.GetValues(typeof(WithDrawalStatus)))
            {
                if (funBillStatus >= 0)
                {
                    listSystemWithdrawal1.Add(new { Text = funBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)funBillStatus });
                }
            }

            foreach (var item in listSystemWithdrawal1)
            {
                listSystemWithdrawal.Add(new
                {
                    Text = item.Text,
                    Value = item.Value,
                    Class = "",
                    Total = listWithdrawal.Count(x => x.Status == item.Value),
                    ClassChild = "label-primary"
                });
            }

            #endregion

            #region RechargeBill
            // Lấy kiểu giao dịch với ví
            foreach (RechargeBillType rechargeBillType in Enum.GetValues(typeof(RechargeBillType)))
            {
                if (rechargeBillType >= 0)
                {
                    listRechargeBillType.Add(new { Text = rechargeBillType.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)rechargeBillType });
                }
            }
            // hien thi Tab loc danh sach
            var listRechargeWallet = UnitOfWork.RechargeBillRepo.Find(x => !x.IsDelete).ToList();
            var listRechargeBillType1 = new List<dynamic>();
            foreach (RechargeBillType rechargeBillType in Enum.GetValues(typeof(RechargeBillType)))
            {
                if (rechargeBillType >= 0)
                {
                    listRechargeBillType1.Add(new { Text = rechargeBillType.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)rechargeBillType });
                }
            }

            //5. Lấy danh sách ví theo  giao dich
            var listSystemRechargeWallet = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listRechargeWallet.Count,
                    ClassChild = "label-danger"
                }
            };

            foreach (var item in listRechargeBillType1)
            {
                listSystemRechargeWallet.Add(new
                {
                    Text = item.Text,
                    Value = item.Value,
                    Class = "",
                    Total = listRechargeWallet.Count(x => x.Type == item.Value),
                    ClassChild = "label-primary"
                });
            }


            // Lấy các trạng thái Status
            foreach (RechargeBillStatus rechargeBillStatus in Enum.GetValues(typeof(RechargeBillStatus)))
            {
                if (rechargeBillStatus >= 0)
                {
                    listRechargeBillStatus.Add(new { Text = rechargeBillStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)rechargeBillStatus });
                }
            }


            #endregion

            #region Debit

            // Lấy các Type Công nợ
            foreach (DebitHistoryType mustCollectStatus in Enum.GetValues(typeof(DebitHistoryType)))
            {
                if (mustCollectStatus >= 0)
                {
                    listDebitType.Add(new { Text = mustCollectStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)mustCollectStatus });
                }
            }

            // Lấy các trạng thái Status
            foreach (MustCollectStatus mustCollectStatus in Enum.GetValues(typeof(MustCollectStatus)))
            {
                if (mustCollectStatus >= 0)
                {
                    listDebitStatus.Add(new { Text = mustCollectStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)mustCollectStatus });
                }
            }

            // hien thi Tab loc danh sach
            var listMustCollect = UnitOfWork.DebitRepo.Find(x => !x.IsDelete).ToList();
            var listDebitStatus1 = new List<dynamic>();
            foreach (DebitHistoryStatus mustCollectStatus in Enum.GetValues(typeof(DebitHistoryStatus)))
            {
                if (mustCollectStatus >= 0)
                {
                    listDebitStatus1.Add(new { Text = mustCollectStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)mustCollectStatus });
                }
            }

            //5. Lấy danh sách công nợ theo  giao dich
            var listSystemDebit = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listMustCollect.Count,
                    ClassChild = "label-danger"
                }
            };

            foreach (var item in listDebitStatus1)
            {
                listSystemDebit.Add(new
                {
                    Text = item.Text,
                    Value = item.Value,
                    Class = "",
                    Total = listMustCollect.Count(x => x.Status == item.Value),
                    ClassChild = "label-primary"
                });
            }

            //5. Lấy danh công nợ của Order theo  giao dich
            var listSystemDebitOrder = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listMustCollect.Count,
                    ClassChild = "label-danger"
                }
            };

            // Lấy danh sách các đối tượng
            var subjectType = UnitOfWork.AccountantSubjectRepo.FindAsNoTracking(x => x.Id > 0 && !x.IsDelete).ToList();
            var tempSubjectList = from p in subjectType
                                  select new SearchMeta() { Text = p.SubjectName, Value = p.Id };
            listSubjectType.Add(new SearchMeta() { Text = "- All -", Value = -1 });
            listSubjectType.AddRange(tempSubjectList.ToList());
            #endregion

            #region MustReturn
            // Lấy các trạng thái Status
            foreach (MustReturnStatus mustReturnStatus in Enum.GetValues(typeof(MustReturnStatus)))
            {
                if (mustReturnStatus >= 0)
                {
                    listMustReturnStatus.Add(new { Text = mustReturnStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)mustReturnStatus });
                }
            }

            //2. Lấy danh sách System trên system
            // TODO: Cần lấy danh sách từ bảng System
            //var systemDb = UnitOfWork.SystemRepo.FindAsNoTracking(x => x.Id > 0).ToList();
            //var tempSystemDb = from p in systemDb
            //                   select new SearchMeta() { Text = p.Domain, Value = p.Id };

            //listSystem.Add(new SearchMeta() { Text = "- Tất cả -", Value = -1 });
            //listSystem.AddRange(tempSystemDb.ToList());



            //3. Lấy danh sách system
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    ClassChild = "label-primary"
                });
            }
            #endregion

            #region User
            //3. Lấy ra danh sách nhân viên
            if (UserState.Type != null && UserState.OfficeId != null)
            {
                var listUserTemp = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, (byte)UserState.Type, UserState.OfficeIdPath, (int)UserState.OfficeId);
                var tempUserList = from p in listUserTemp
                                   select new SearchMeta() { Text = p.UserName + " - " + p.FullName, Value = p.Id };

                listUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
                listUser.AddRange(tempUserList.ToList());
            }

            //4. Lấy danh sách nhân viên hỗ trợ
            var listUserDetail = UnitOfWork.DbContext.Users.Where(s => s.Id != UserState.UserId).Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
            foreach (var item in listUserDetail)
            {
                item.Text = item.Text + '-' + Position(int.Parse(item.Value));
            }

            //5. Lấy danh sách trạng thái Refund
            foreach (ClaimForRefundStatus refundStatus in Enum.GetValues(typeof(ClaimForRefundStatus)))
            {
                if (refundStatus >= 0)
                {
                    listStatusRefund.Add(new { Text = refundStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)refundStatus });
                }
            }
            #endregion

            return Json(new
            {
                listFundBillType,
                listFinanceFund,
                listAccountantSubject,
                listFundBillStatus,

                listRechargeBillType,
                listRechargeBillStatus,
                listCustomer,

                listDebitStatus,
                listSubjectType,
                listSubject,
                listOffice,

                listMustReturnStatus,
                listWithDrawalStatus,
                listSystem,
                listTicketStatus,
                listUser,
                listUserDetail,
                listSystemWallet,
                listSystemRechargeWallet,

                listSystemDebit,
                listSystemWithdrawal,
                listDebitType,
                listStatusRefund

            }, JsonRequestBehavior.AllowGet);
        }
        //Lấy về vị trí  công tác
        public string Position(int id)
        {
            var x = "";
            var user = UnitOfWork.DbContext.UserPositions.FirstOrDefault(d => d.UserId == id && d.IsDefault);
            if (user != null)
            {
                x = user.TitleName;
            }
            return x;
        }
        // Khởi tạo form
        [HttpPost]
        public async Task<JsonResult> GetInit()
        {
            var totalFundBill = 0;
            var totalRechargeBill = 0;
            var totalRequestMoney = 0;
            var totalDebit = 0;
            var totalDebitOrder = 0;
            var totalMustReturn = 0;
            var totalComplain = 0;
            var totalClaimForRefund = 0;
            long totalAccountantOrder = 0;

            //1. Tính toán số FullBill chưa được duyệt
            totalFundBill = await UnitOfWork.FundBillRepo.CountAsync(x => x.IsDelete == false);

            //2. Tính toán phiếu nạp/trừ tiền ví điện tử chưa được duyệt
            totalRechargeBill = await UnitOfWork.RechargeBillRepo.CountAsync(x => x.IsDelete == false);
            
            //3. Tính toán số phiếu yêu cầu rút tiền điện tử của khách hàng
            totalRequestMoney = UnitOfWork.DbContext.Draws.Count();

            //4. Tính toán số phải thu chưa xử lý
            totalDebit = await UnitOfWork.DebitRepo.CountAsync(x => !x.IsDelete);

            //6. Tính toán số khiếu nại cần hỗ trợ
            totalComplain = await UnitOfWork.ComplainRepo.TicketSupportCountAsync(UserState);

            //7. Tính toán Sum số Customer Refund Request
            //var listClaimForRefund = UnitOfWork.DbContext.ClaimForRefund;
            //x => !x.IsDelete
            //&& x.Status == (int)ClaimForRefundStatus.OrderWait
            //&& x.AccountantId == UserState.UserId

            totalClaimForRefund = UnitOfWork.ClaimForRefundRepo.Count();

            //TODO(Giỏi): bổ xung thêm hiển thị số lượng Contract phải thanh toán
            //8. Tính số lượng Contract phải thanh toán
            decimal totalPrice;
            var list = await UnitOfWork.OrderRepo.GetOrderContractCode(out totalPrice, out totalAccountantOrder, 1, 
                0, "", (byte)ContractCodeType.AwaitingPayment, -1, null, null, null, null);

            return Json(new { totalFundBill, totalRechargeBill, totalRequestMoney, totalDebit,
                totalMustReturn, totalComplain, totalClaimForRefund, totalAccountantOrder, totalDebitOrder }, JsonRequestBehavior.AllowGet);
        }

        #region [Thông tin hỗ trợ khiếu nại]
        [HttpPost]

        public async Task<JsonResult> GetAllSearchData()
        {
            var listComplainStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listComplainSystem = new List<dynamic>() { new { Text ="All", Value = -1 } };

            // Lấy các trạng thái Status của Complain
            foreach (ComplainStatus ticketStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (ticketStatus >= 0)
                {
                    listComplainStatus.Add(new { Text = ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)ticketStatus });
                }
            }

            // Lấy danh sách System
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listComplainSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }
            var count = 0;
            //foreach (var item in UnitOfWork.ComplainRepo.Find(s => !s.IsDelete))
            //{
            //    if (CountUser(item.Id) > 0)
            //    {
            //        count = count + 1;
            //    }
            //}
            return Json(new { count, listComplainStatus, listComplainSystem }, JsonRequestBehavior.AllowGet);
        }

        public int CountUser(long complainId)
        {
            var userId = UserState.UserId;
            var count = UnitOfWork.ComplainUserRepo.Find(d => d.ComplainId == complainId && d.UserId == userId && d.IsCare == false).Count();
            return count;

        }

        [HttpPost]
        public async Task<JsonResult> GetRenderSystemTab(string active)
        {
            var listComplainSystem = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listSystemDb1 = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb1)
            {
                listComplainSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }

            var listStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };
            foreach (ComplainStatus complainStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (complainStatus != 0)
                    listStatus.Add(new { Value = (int)complainStatus, Text = complainStatus.GetAttributeOfType<DescriptionAttribute>().Description });
            }

            var listComplain = new List<SystemMeta>();
            var listcomplainuser = new List<TicketComplain>();

            if (active == "ticket-support")
            {
                listcomplainuser = UnitOfWork.ComplainRepo.SystemTicketSupport(UserState);

            }

            listComplain = listcomplainuser.Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();

            //5. Lấy danh sách system
            var listSystem = new List<dynamic>()
            {
                new
                {
                    Text ="All",
                    Value = -1,
                    Class = "active",
                    Total = listComplain.Count,
                    ClassChild = "label-danger"
                }
            };
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);

            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                    Class = "",
                    Total = listComplain.Count(x => x.SystemId == item.Id),
                    ClassChild = "label-primary"
                });
            }


            return Json(new { listSystem, listStatus, listComplainSystem }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public async Task<JsonResult> GetAllTicketListByStaff(int page, int pageSize, ComplainSearchModal searchModal)
        {
            //var ticketModal = new List<TicketComplain>();
            //var office = (byte)OfficeType.Accountancy;
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }

            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var dateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var dateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            var ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketSupportOfficeList(out totalRecord, page, pageSize, searchModal.Keyword,
                searchModal.Status, searchModal.SystemId, dateStart, dateEnd, UserState);

            return Json(new { totalRecord, ticketModal }, JsonRequestBehavior.AllowGet);
        }

        #region Detail KHIẾU NẠI
        [HttpPost]
        public async Task<JsonResult> GetTicketDetail(int ticketId)
        {
            var customer = new Customer();
            var complainuserlist = new List<ComplainDetail>();
            var list = new List<ComplainDetail>();
            var usersupportlist = new List<User>();
            var usersupport = new User();
            var ticketModal = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(p => p.Id == ticketId);
            //var complainuser = UnitOfWork.DbContext.ComplainUsers.Where(s => s.ComplainId == ticketId).OrderBy(b=>b.CreateDate);
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == ticketId
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );

            //Lọc Staff handling
            var complainusersupport = (from d in UnitOfWork.DbContext.ComplainUsers
                                       where (d.ComplainId == ticketId && d.IsCare == false)
                                       orderby d.CreateDate
                                       select new { d.UserId, d.UserName }).Distinct().ToList();
            //Sum complainusser theo complainId
            foreach (var item in complainuser)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {

                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    if (posit != null)
                    {
                        userPosition = posit.TitleName;
                    }

                }

                complainuserlist.Add(new ComplainDetail
                {
                    Id = item.Id,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = item.ComplainId,
                    Content = item.Content,
                    AttachFile = item.AttachFile,
                    CreateDate = item.CreateDate,
                    UpdateDate = item.UpdateDate,
                    UserRequestId = item.UserRequestId,
                    UserRequestName = item.UserRequestName,
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    IsRead = item.IsRead,
                    IsCare = item.IsCare
                });
            }

            //Lọc người xử lý(hỗ trợ)
            foreach (var item in complainusersupport)
            {
                var posit = new UserPosition();
                var userPosition = "";
                if (item.UserId != null)
                {
                    posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    userPosition = posit.TitleName;
                }
                list.Add(new ComplainDetail
                {
                    Id = 0,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = 0
                });
            }

            //Lấy ra nhân viên chịu trách nhiệm chính
            var complainusermain = complainuserlist.SingleOrDefault(s => s.IsCare == true);
            var hascustomer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(p => p.Id == ticketModal.CustomerId);
            customer = hascustomer;
            //}

            return Json(new { ticketModal, customer, complainuserlist, complainusermain, list }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllUser()
        {
            var listUser =
                UnitOfWork.DbContext.Users.Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" })
                    .ToList();
            return Json(new { listUser }, JsonRequestBehavior.AllowGet);
        }

        //Phản hồi cho khách hàng

        [HttpPost]
        public JsonResult feedbackComplain(string content, int complainId)
        {
            var com = UnitOfWork.ComplainUserRepo.FirstOrDefault(d => d.ComplainId == complainId && d.UserId == UserState.UserId);
            var complainback = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            var complain = new ComplainUser();
            complain.UserId = UserState.UserId;
            complain.UserName = UserState.UserName;
            if (com != null)
            {
                complain.UserRequestId = com.UserRequestId;
                complain.UserRequestName = com.UserRequestName;
            }
            complain.Content = content;
            complain.ComplainId = complainId;
            complain.CreateDate = DateTime.Now;
            complain.CustomerId = complainback.CustomerId;
            complain.CustomerName = complainback.CustomerName;
            UnitOfWork.ComplainUserRepo.Add(complain);
            UnitOfWork.ComplainUserRepo.Save();

            return Json(complainback, JsonRequestBehavior.AllowGet);
        }
        //Hoàn thành ticket
        [HttpPost]
        public JsonResult finishComplain(int complainId)
        {

            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == complainId);
            complain.Status = (byte)ComplainStatus.Success;
            complain.LastUpdateDate = DateTime.Now;
            UnitOfWork.ComplainRepo.Save();
            return Json(complain, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #endregion

        #region Xử lý yêu cầu Refund cho khách hàng

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claimForRefund"></param>
        /// <param name="model"></param>
        /// <returns></returns>

        //Lấy danh sách yêu cầu Refund
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund, EnumPage.OrderClaimForRefund)]
        public async Task<JsonResult> GetClaimForRefundList(int page, int pageSize, ClaimForRefundSearchModal searchModal)
        {
            List<ClaimForRefund> claimForRefundModal;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ClaimForRefundSearchModal();
            }

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var dateStart = DateTime.Parse(searchModal.DateStart);
                var dateEnd = DateTime.Parse(searchModal.DateEnd);

                claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword)
                    || x.OrderCode == searchModal.Keyword
                    || x.CustomerFullName.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword))

                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.AccountantId == searchModal.UserId)
                         && x.Created >= dateStart && x.Created <= dateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            else
            {
                claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FindAsync(
                    out totalRecord,
                    x => (x.Code.Contains(searchModal.Keyword)
                    || x.OrderCode == searchModal.Keyword
                    || x.CustomerFullName.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword))
                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId),
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }

            return Json(new { totalRecord, claimForRefundModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> ExecuteClaimForRefund(ClaimForRefund claimForRefund)
        {
            //1. Kiểm tra Ticket còn tồn tại hay không thì mới được Refund.
            var ticketDetail = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == claimForRefund.TicketId);
            if (ticketDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket Does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra phiếu yêu cầu Refund cho khách hàng có tồn tại hay không ?
            var claimForRefundDetail = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(x => !x.IsDelete && x.Id == claimForRefund.Id);
            if (claimForRefundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.ClaimForRefundIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra trạng thái có phải là Waiting Refund
            if (claimForRefundDetail.Status != (int)ClaimForRefundStatus.AccountantWait)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.ClaimForRefundIsNotWating }, JsonRequestBehavior.AllowGet);
            }

            //4. Lấy thông tin Detail đối tượng nhập là khách hàng
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == claimForRefundDetail.CustomerId);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //5. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //1. Kiểm tra xem có thêm yêu cầu tạo một phiếu nạp/trừ tiền ví điện tử khách hàng tương ứng hay không ?
                    //var rechargeBill = new RechargeBill();

                    //var rechargeBillOfDay = UnitOfWork.RechargeBillRepo.Count(x =>
                    //                        x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                    //                        x.Created.Day == DateTime.Now.Day);
                    //rechargeBill.Code = $"{rechargeBillOfDay}{DateTime.Now:ddMMyy}";

                    //rechargeBill.CustomerId = customerDetail.Id;
                    //rechargeBill.CustomerCode = customerDetail.Code;
                    //rechargeBill.CustomerPhone = customerDetail.Phone;
                    //rechargeBill.CustomerName = customerDetail.FullName;
                    //rechargeBill.CustomerEmail = customerDetail.Email;
                    //rechargeBill.CustomerAddress = customerDetail.Address;

                    ////Thông tin nhân viên tạo phiếu
                    //rechargeBill.UserId = UserState.UserId;
                    //rechargeBill.UserName = UserState.UserName;

                    ////Thông tin số dư
                    //rechargeBill.CurrencyFluctuations = (decimal)claimForRefundDetail.RealTotalRefund;
                    //rechargeBill.Increase = (decimal)claimForRefundDetail.RealTotalRefund;
                    //rechargeBill.Diminishe = 0;

                    //rechargeBill.CurencyStart = customerDetail.BalanceAvalible;
                    //rechargeBill.CurencyEnd = customerDetail.BalanceAvalible + (decimal)claimForRefundDetail.RealTotalRefund;

                    //rechargeBill.Type = 0;
                    //rechargeBill.Status = (byte)RechargeBillStatus.Approved;

                    //rechargeBill.Note = "Refund e-wallet from complaint handling process. Complaint handling code: " + claimForRefundDetail.Code;

                    //UnitOfWork.RechargeBillRepo.Add(rechargeBill);
                    //UnitOfWork.RechargeBillRepo.Save();

                    var processRechargeBillResult = UnitOfWork.RechargeBillRepo.ProcessRechargeBill(new AutoProcessRechargeBillModel()
                    {
                        CustomerId = customerDetail.Id,
                        CurrencyFluctuations = (decimal)claimForRefundDetail.RealTotalRefund,
                        OrderId = claimForRefundDetail.OrderId,
                        Note = "Refund e-wallet from complaint handling process. Complaint handling code: " + claimForRefundDetail.Code,
                        TreasureIdd = (int)TreasureCustomerWallet.ClaimForRefund
                    });

                    // Lỗi trong quá tình thực hiện thanh toán
                    if (processRechargeBillResult.Status < 0)
                    {
                        transaction.Rollback();

                        return Json(new { status = Result.Failed, msg = "Error in updating complaint handling refund !" }, JsonRequestBehavior.AllowGet);
                    }


                    //2. Cập nhật lại thông tin phiếu yêu cầu Refund cho khách là đã Refund
                    // Lấy thông tin Detail của nhân viên
                    var staffDetail = await UnitOfWork.UserRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == UserState.UserId);
                    if (staffDetail != null)
                    {
                        claimForRefundDetail.AccountantId = staffDetail.Id;
                        claimForRefundDetail.AccountantFullName = staffDetail.FullName;
                        claimForRefundDetail.AccountantEmail = staffDetail.Email;
                    }

                    claimForRefundDetail.LastUpdated = DateTime.Now;
                    claimForRefundDetail.Status = (byte)ClaimForRefundStatus.Success;

                    UnitOfWork.ClaimForRefundRepo.Save();

                    // Cập nhật trạng thái cho ticket là hoàn thành
                    ticketDetail.Status = (byte)ComplainStatus.AccountantFinish;
                    ticketDetail.LastUpdateDate = DateTime.Now;

                    UnitOfWork.ComplainRepo.Save();
                    //// Gửi Notify cho nhân viên CSKH
                    //if (claimForRefundDetail.SupportId > 0)
                    //{
                    //    NotifyHelper.CreateAndSendNotifySystemToClient((int)claimForRefundDetail.OrderUserId,
                    //        "Xác nhận Refund cho khiếu nại " + ticketDetail.Code,
                    //        EnumNotifyType.Info,
                    //        $" <a href=\"" + "/Ticket/#CFRF" + claimForRefundDetail.Code + "\" target=\"_blank\">" + claimForRefundDetail.Code + "</a>",
                    //        "Xác nhận Refund cho khiếu nại: " + ticketDetail.Code,
                    //         Url.Action("Index", "Ticket"));
                    //    //NotifyHelper.CreateAndSendNotifySystemToClient((int)claimForRefundDetail.SupportId, "Xác nhận Refund cho khiếu nại #CFRF" + ticketDetail.Code, EnumNotifyType.Info, "Xác nhận Refund cho Phiếu Refund:" + claimForRefundDetail.Code);
                    //}

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = ticketDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = ticketDetail.CustomerId;
                    conplainHistory.CustomerName = ticketDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.AccountantFinish;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Changing status to : " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.AccountantFinish);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    //4. TODO: Cập nhật lên thông báo cho khách hàng

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }

                try
                {
                    // Gửi thông báo Notification cho khách hàng
                    var notification = new Notification()
                    {
                        SystemId = customerDetail.SystemId,
                        SystemName = customerDetail.SystemName,
                        CustomerId = customerDetail.Id,
                        CustomerName = customerDetail.FullName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderType = 4, // Thông báo giành cho thay đổi ví kế toán
                        IsRead = false,
                        Title = "The balance of your e-wallet has changed",
                        Description = "The balance of your e-wallet has changed: " + (decimal)claimForRefundDetail.RealTotalRefund + " Refund e-wallet from complaint handling process. Complaint handling code: " + claimForRefundDetail.TicketCode
                    };

                    UnitOfWork.NotificationRepo.Add(notification);
                    UnitOfWork.NotificationRepo.Save();

                    ////4. Gửi thư thông báo cho khách hàng thông tin thay đổi số dư ví điện tử
                    //var body = System.IO.File.ReadAllText(Server.MapPath("~/EmailTemplate/Accounting/rechargeInfo.html"));

                    //body = body.Replace("{{Note}}", rechargeBillDetail.Note);
                    //body = body.Replace("{{UserFullname}}", customerDetail.FullName);
                    //body = body.Replace("{{CurrencyFluctuations}}", rechargeBillDetail.CurrencyFluctuations.ToString());
                    //body = body.Replace("{{CurencyStart}}", rechargeBillDetail.CurencyStart.ToString());
                    //body = body.Replace("{{CurencyEnd}}", rechargeBillDetail.CurencyEnd.ToString());
                    //MailHelper.SendMail(customerDetail.Email, "The balance of your e-wallet has changed", body, false);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }



            return Json(new { status = Result.Succeed, msg = ConstantMessage.ExecuteClaimForRefundIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Tra cứu thông tin khách hàng]
        //TRA CỨU THÔNG TIN KHÁCH HÀNG
        [HttpPost]
        public async Task<JsonResult> GetCustomerInfo(int customerId)
        {
            var customer = new Customer();
            if (customerId > 0)
            {

                customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == customerId);

            }
            return Json(new { customer }, JsonRequestBehavior.AllowGet);
        }
        //Lịch sử hỗ trợ khách hàng
        [HttpPost]
        public JsonResult SupportHistory(int customerId)
        {
            var customer = (from s in UnitOfWork.DbContext.Complains
                            where s.CustomerId == customerId && !s.IsDelete
                            orderby s.CreateDate
                            select new { s.CreateDate }).Distinct();
            var customerinfo = from s in UnitOfWork.DbContext.Complains
                               where s.CustomerId == customerId && !s.IsDelete
                               select s;
            return Json(new { customer, customerinfo }, JsonRequestBehavior.AllowGet);
        }

        //Lịch sử Orders
        [HttpPost]
        public async Task<JsonResult> OrderHistory(int? customerId, int page, int pageSize)
        {
            long totalRecord;
            int userId = UserState.UserId;
            var customer = new List<Order>();
            customer = await UnitOfWork.OrderRepo.FindAsync(
                  out totalRecord,
                  s => s.CustomerId == customerId && !s.IsDelete,
                    x => x.OrderByDescending(y => y.Id),
                    page,
                    pageSize);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        //Sao kê ví điện tử
        [HttpPost]
        public async Task<JsonResult> OrderMoney(int customerId, int page, int pageSize)
        {
            int userId = UserState.UserId;
            var customer = new List<RechargeBill>();
            customer = await UnitOfWork.RechargeBillRepo.FindAsync(s => s.CustomerId == customerId && !s.IsDelete);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thống kê quỹ
        /// <summary>
        /// Thống kê: Tồn quỹ tại thời điểm hiện tại
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> FinanceFundReport()
        {
            var fundName = new List<string>();
            var fundData = new List<decimal>();

            //1. Lấy danh sách các quỹ
            var financeFundList = await UnitOfWork.FinaceFundRepo.FindAsNoTrackingAsync(x => !x.IsDelete);
            foreach (var item in financeFundList)
            {
                fundName.Add(item.Name);
                fundData.Add(item.Balance);
            }

            return Json(new { fundName, fundData }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Thống kê: Thu/Chi quỹ trong ngày
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> FundReport()
        {
            var fundName = new List<string>();
            var fundAdd = new List<decimal>();
            var fundMinus = new List<decimal>();

            decimal addValue = 0;
            decimal minusValue = 0;

            DateTime startOfDay = Common.Func.Extensions.GetStartOfDay(DateTime.Now);
            DateTime endOfDay = Common.Func.Extensions.GetEndOfDay(DateTime.Now);

            //1. Lấy thông tin danh sách quỹ
            var financeFundList = await UnitOfWork.FinaceFundRepo.FindAsNoTrackingAsync(x => !x.IsDelete);
            foreach (var item in financeFundList)
            {
                fundName.Add(item.Name);
            }

            //2. Tính toán số tiền phát sinh trong quỹ trong ngày
            var fundbillOfDay = await UnitOfWork.FundBillRepo.FindAsNoTrackingAsync(x => !x.IsDelete
                                                                                   && x.Status == (byte)FundBillStatus.Approved
                                                                                   && x.LastUpdated >= startOfDay && x.LastUpdated <= endOfDay);

            foreach (var item in financeFundList)
            {
                addValue = 0;
                minusValue = 0;

                foreach (var bill in fundbillOfDay)
                {
                    if (bill.FinanceFundId == item.Id && bill.Type == (int)FundBillType.Increase)
                    {
                        addValue += bill.CurrencyFluctuations;
                    }

                    if (bill.FinanceFundId == item.Id && bill.Type == (int)FundBillType.Diminishe)
                    {
                        minusValue += bill.CurrencyFluctuations;
                    }
                }

                fundAdd.Add(addValue);
                fundMinus.Add(minusValue);
            }

            return Json(new { fundName, fundAdd, fundMinus }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FinanceTreasureOfDay()
        {
            //1. Lấy danh sách
            var dateNow = DateTime.Now;
            var listOrder = UnitOfWork.OrderRepo.FindAsNoTracking(x => x.LastUpdate.Year == dateNow.Year && x.LastUpdate.Month == dateNow.Month
                && x.LastUpdate.Day == dateNow.Day && !x.IsDelete && x.Type == (byte)OrderType.Order).OrderBy(x => x.Status).ToList();

            //2. Lọc theo trạng thái
            var overview = new List<ReportMeta>();
            foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                var status = (int)orderStatus;
                if (status == 0) continue;

                var data = new ReportMeta
                {
                    name = orderStatus.GetAttributeOfType<DescriptionAttribute>().Description,
                    y = listOrder.Count(x => x.Status == status)
                };

                overview.Add(data);
            }

            //5. Trả kết quả lên view
            return Json(new { overview }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        //Thống kê hoạt động quỹ trong nghiệp vụ tài chính
        [HttpPost]
        public ActionResult ExportExcelFund(DateTime? dateInput)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");
                var colorTd = ColorTranslator.FromHtml("#92D050");
                var colorFooter = ColorTranslator.FromHtml("#FFC000");

                dateInput = dateInput ?? DateTime.Now;

                var listFund = UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsParent && !x.IsDelete).ToList();
                var listTreasure = UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
                var listFundBill = UnitOfWork.FundBillRepo.Entities.Where(x => !x.IsDelete && x.Status == (byte)FundBillStatus.Approved).ToList();
                var arrayCount = listTreasure.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + arrayCount, "ACCOUNT", ExcelHorizontalAlignment.Left, true);
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col, row + 1, col + arrayCount, "Balance", ExcelHorizontalAlignment.Left, true, colorTd);
                ExcelHelper.CreateHeaderTable(sheet, row + 2, col, row + 2, col + arrayCount, "Activity", ExcelHorizontalAlignment.Left, true);
                col = col + arrayCount;
                col++;
                foreach (var fund in listFund)
                {
                    var sum = fund.Balance;
                    var fullbill = listFundBill.Where(s => s.FinanceFundId == fund.Id && s.LastUpdated.Date >= dateInput.Value.Date);
                    foreach (var item in fullbill)
                    {
                        if (item.Type == (byte)FundBillType.Increase)
                        {
                            sum -= item.CurrencyFluctuations;
                        }
                        else
                        {
                            sum += item.CurrencyFluctuations;
                        }
                    }

                    ExcelHelper.CreateHeaderTable(sheet, row, col, fund.Name, ExcelHorizontalAlignment.Center, true);
                    ExcelHelper.CreateHeaderTable(sheet, row + 1, col, sum, ExcelHorizontalAlignment.Center, true, colorTd);
                    ExcelHelper.CreateCellTable(sheet, row + 1, col, row + 1, col, sum, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0"
                    });
                    ExcelHelper.CreateHeaderTable(sheet, row + 2, col, null, ExcelHorizontalAlignment.Center, true);
                    col++;
                }

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 1, 1, row - 1, col - 1, $"FUND ACTIVITY REPORT IN DAY {dateInput.Value.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Left,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });
                #endregion

                var no = row + 3;

                if (listTreasure.Any())
                {
                    foreach (var treasure in listTreasure)
                    {
                        col = 1;
                        var count = treasure.IdPath.Split('.').Count() - 1;
                        if (treasure.ParentId == 0)
                        {
                            var str = treasure.Operator == false ? "-" : "+";
                            ExcelHelper.CreateCellTable(sheet, no, col + count, $"{treasure.Name.ToUpper()}({str})", ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, treasure.Name, ExcelHorizontalAlignment.Left, true);
                        }
                        col = col + arrayCount;
                        col++;
                        foreach (var fund in listFund)
                        {
                            if (treasure.Operator == false)
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.FinanceFundId == fund.Id && x.TreasureId == treasure.Id && x.LastUpdated.Date == dateInput.Value.Date).Sum(x => x.Diminishe), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            else
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.FinanceFundId == fund.Id && x.TreasureId == treasure.Id && x.LastUpdated.Date == dateInput.Value.Date).Sum(x => x.Increase), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            col++;
                        }
                        no++;
                    }
                }

                col = 1;
                ExcelHelper.CreateHeaderTable(sheet, no, col, no, col + arrayCount, "Left", ExcelHorizontalAlignment.Left, true, colorFooter);
                col = col + arrayCount;
                col++;
                foreach (var fund in listFund)
                {
                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, fund.Balance, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0",
                        BackgroundColor = colorFooter
                    });
                    col++;
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"FUND_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //Thống kê hoạt động ví điện tử trong nghiệp vụ tài chính
        [HttpPost]
        public ActionResult ExportExcelWallet(DateTime? dateInput)
        {
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");
                var colorTd = ColorTranslator.FromHtml("#92D050");
                var colorFooter = ColorTranslator.FromHtml("#FFC000");

                dateInput = dateInput ?? DateTime.Now;

                var listFund = UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsParent && !x.IsDelete).ToList();
                var listTreasure = UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
                var listFundBill = UnitOfWork.FundBillRepo.Entities.Where(x => !x.IsDelete && x.Status == (byte)FundBillStatus.Approved).ToList();
                var arrayCount = listTreasure.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + arrayCount, "ACCOUNT", ExcelHorizontalAlignment.Left, true);
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col, row + 1, col + arrayCount, "Balance", ExcelHorizontalAlignment.Left, true, colorTd);
                ExcelHelper.CreateHeaderTable(sheet, row + 2, col, row + 2, col + arrayCount, "Activity", ExcelHorizontalAlignment.Left, true);
                col = col + arrayCount;
                col++;
                foreach (var fund in listFund)
                {
                    var sum = fund.Balance;
                    //if (DateTime.Now.Date != date.Value.Date)
                    //{
                    //    var fullbill = listFundBill.Where(s => s.FinanceFundId == fund.Id && s.Created.Date >= date.Value.Date);
                    //    foreach (var item in fullbill)
                    //    {
                    //        var objectTest = listTreasure.FirstOrDefault(s => s.Id == item.TreasureId);
                    //        if (objectTest.Operator.Value)
                    //        {
                    //            sum -= item.CurrencyFluctuations;
                    //        }
                    //        else
                    //        {
                    //            sum += item.CurrencyFluctuations;
                    //        }
                    //    }
                    //}

                    var fullbill = listFundBill.Where(s => s.FinanceFundId == fund.Id && s.Created.Date > dateInput.Value.Date);
                    foreach (var item in fullbill)
                    {

                        if (item.Type == (byte)FundBillType.Increase)
                        {
                            sum -= item.CurrencyFluctuations;
                        }
                        else
                        {
                            sum += item.CurrencyFluctuations;
                        }
                    }

                    ExcelHelper.CreateHeaderTable(sheet, row, col, fund.Name, ExcelHorizontalAlignment.Center, true);
                    ExcelHelper.CreateHeaderTable(sheet, row + 1, col, sum, ExcelHorizontalAlignment.Center, true, colorTd);
                    ExcelHelper.CreateCellTable(sheet, row + 1, col, row + 1, col, sum, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0"
                    });
                    ExcelHelper.CreateHeaderTable(sheet, row + 2, col, null, ExcelHorizontalAlignment.Center, true);
                    col++;
                }

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 1, 1, row - 1, col - 1, $"REPORT OF E-WALLET ACTIVITY IN DAY {dateInput.Value.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Left,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });
                #endregion

                var no = row + 3;

                if (listTreasure.Any())
                {
                    foreach (var treasure in listTreasure)
                    {
                        col = 1;
                        var count = treasure.IdPath.Split('.').Count() - 1;
                        if (treasure.ParentId == 0)
                        {
                            var str = treasure.Operator == false ? "-" : "+";
                            ExcelHelper.CreateCellTable(sheet, no, col + count, $"{treasure.Name.ToUpper()}({str})", ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, treasure.Name, ExcelHorizontalAlignment.Left, true);
                        }
                        col = col + arrayCount;
                        col++;
                        foreach (var fund in listFund)
                        {
                            if (treasure.Operator == false)
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.FinanceFundId == fund.Id && x.TreasureId == treasure.Id && x.Created.Date == dateInput.Value.Date).Sum(x => x.Diminishe), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            else
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.FinanceFundId == fund.Id && x.TreasureId == treasure.Id && x.Created.Date == dateInput.Value.Date).Sum(x => x.Increase), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            col++;
                        }
                        no++;
                    }
                }

                col = 1;
                ExcelHelper.CreateHeaderTable(sheet, no, col, no, col + arrayCount, "Left", ExcelHorizontalAlignment.Left, true, colorFooter);
                col = col + arrayCount;
                col++;
                foreach (var fund in listFund)
                {
                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, fund.Balance, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0",
                        BackgroundColor = colorFooter
                    });
                    col++;
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"FUND_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        #region [Thống kê tình hình thu chi định khoản quỹ theo thời gian]
        [HttpPost]
        public JsonResult GetAccountSituation(DateTime? startDay, DateTime? endDay, int? financeFundId)
        {
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var now = GetEndOfDay(DateTime.Now);

            var inc = (byte)FundBillType.Increase;
            var dic = (byte)FundBillType.Diminishe;

            var account = UnitOfWork.FundBillRepo.GetAccountSituationOnTime(start, end, now, (int)FundBillStatus.Approved, financeFundId, inc, dic);

            //1. Tạo các dữ liệu theo báo cáo
            var diminishe = new List<decimal>();
            var increase = new List<decimal>();
            var balance = new List<decimal>();
            var day = new List<string>();
            foreach (var or in account)
            {
                day.Add(or.Created);
                diminishe.Add(or.Diminishe ?? 0);
                increase.Add(or.Increase ?? 0);
                balance.Add(or.Balance ?? 0);
            }
            //2. Trả kết quả lên view
            return Json(new { day, diminishe, increase, balance }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAccount(int page, int pagesize, string keyword, DateTime? startDay, DateTime? endDay, int? customerId)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "";
            }

            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var now = GetEndOfDay(DateTime.Now);

            //1. Tạo các dữ liệu theo báo cáo
            var diminishe = new List<decimal>();//trừ
            var increase = new List<decimal>();//nạp
            var balance = new List<decimal>();//số dư
            var before = new List<decimal>();//trước kỳ
            var after = new List<decimal>();//sau kỳ
            var name = new List<string>();//Full name

            //2. tạo dữ liệu
            long totalRecord;
            var listCustomer = UnitOfWork.CustomerRepo.Find(
                  out totalRecord,
                  x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Email.Contains(keyword)),
                  x => x.OrderBy(y => y.FullName),
                  page,
                  pagesize
              );

            var listCustomerWallet = UnitOfWork.CustomerWalletRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
            var listRechargeBill = UnitOfWork.RechargeBillRepo.Entities.Where(h => !h.IsDelete).ToList();

            var enumerable = listCustomer as Customer[] ?? listCustomer.ToArray();
            foreach (var cus in enumerable)
            {
                var b = (decimal)0;
                var a = (decimal)0;

                //lấy tên khách hàng
                name.Add($"{cus.FullName} ({cus.Email})");

                //lấy Balances
                balance.Add(cus.BalanceAvalible);

                //lấy số dư
                b = cus.BalanceAvalible;
                a = cus.BalanceAvalible;
                if (start <= now)
                {
                    //trước kỳ
                    var fullbill = listRechargeBill.Where(s => s.CustomerId == cus.Id && s.Created.Date >= start.Date);
                    foreach (var item in fullbill)
                    {
                        var objectTest = listCustomerWallet.FirstOrDefault(s => s.Id == item.TreasureId);
                        if (item.Type == (byte)RechargeBillType.Increase)
                        {
                            b -= item.Increase ?? 0;
                        }
                        else
                        {
                            b += item.Diminishe ?? 0;
                        }
                    }
                    //sau kỳ
                    fullbill = listRechargeBill.Where(s => s.CustomerId == cus.Id && s.Created.Date > end.Date);
                    foreach (var item in fullbill)
                    {
                        var objectTest = listCustomerWallet.FirstOrDefault(s => s.Id == item.TreasureId);
                        if (objectTest.Operator.Value)
                        {
                            a -= item.Increase ?? 0;
                        }
                        else
                        {
                            a += item.Diminishe ?? 0;
                        }
                    }
                }
                before.Add(b);
                after.Add(a);
            }

            //nạp trừ ví
            if (listCustomerWallet.Any())
            {
                foreach (var cus in enumerable)
                {
                    var d = (decimal)0;
                    var i = (decimal)0;
                    foreach (var treasure in listCustomerWallet)
                    {
                        if (treasure.Operator == false)
                        {
                            var sum = listRechargeBill.Where(x => x.TreasureId == treasure.Id && x.CustomerId == cus.Id && x.Created.Date >= start.Date && x.Created.Date <= end.Date).Sum(x => x.Diminishe);
                            if (sum != null)
                                d += sum.Value;
                        }
                        else
                        {
                            var sum = listRechargeBill.Where(x => x.TreasureId == treasure.Id && x.CustomerId == cus.Id && x.Created.Date >= start.Date && x.Created.Date <= end.Date).Sum(x => x.Increase);
                            if (sum != null)
                                i += sum.Value;
                        }
                    }

                    diminishe.Add(d);
                    increase.Add(i);
                }
            }

            //3. Trả kết quả lên view
            return Json(new { name, diminishe, increase, balance, before, after, totalRecord }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        //Xuất thống kê hoạt động thu chi quỹ trong nghiệp vụ tài chính
        [HttpPost]
        public ActionResult ExportAccountExcelFund(DateTime? startDay, DateTime? endDay, int financeFundId)
        {
            var id = financeFundId == 0 ? 1 : financeFundId;
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");
                var colorTd = ColorTranslator.FromHtml("#92D050");
                var colorFooter = ColorTranslator.FromHtml("#FFC000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var now = GetEndOfDay(DateTime.Now);

                var listDate = new List<DateTime>();
                DateTime tmpDate = start;
                do
                {
                    listDate.Add(tmpDate);
                    tmpDate = tmpDate.AddDays(1);
                } while (tmpDate <= end);

                var Fund = UnitOfWork.FinaceFundRepo.Entities.FirstOrDefault(x => x.Id == id);

                var listTreasure = UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
                var listFundBill = UnitOfWork.FundBillRepo.Entities.Where(h => !h.IsDelete
                                                && h.FinanceFundId == id && h.Status == (byte)FundBillStatus.Approved).ToList();
                var arrayCount = listTreasure.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

                var sum = Fund.Balance;


                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + arrayCount, "ACCOUNT", ExcelHorizontalAlignment.Left, true);
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col, row + 1, col + arrayCount, "Balance", ExcelHorizontalAlignment.Left, true, colorTd);
                ExcelHelper.CreateHeaderTable(sheet, row + 2, col, row + 2, col + arrayCount, "Activity", ExcelHorizontalAlignment.Left, true);
                col = col + arrayCount;
                col++;
                foreach (var date in listDate)
                {
                    if (date.Date <= now)
                    {
                        sum = Fund.Balance;
                        var fullbill = listFundBill.Where(s => s.LastUpdated.Date >= date.Date && s.Status == (byte)FundBillStatus.Approved);
                        foreach (var item in fullbill)
                        {

                            if (item.Type == (byte)FundBillType.Increase)
                            {
                                sum -= item.Increase ?? 0;
                            }
                            else
                            {
                                sum += item.Diminishe ?? 0;
                            }
                        }

                        ExcelHelper.CreateHeaderTable(sheet, row, col, date.ToShortDateString(), ExcelHorizontalAlignment.Center, true);
                        ExcelHelper.CreateHeaderTable(sheet, row + 1, col, sum, ExcelHorizontalAlignment.Center, true, colorTd);
                        ExcelHelper.CreateCellTable(sheet, row + 1, col, row + 1, col, sum, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        ExcelHelper.CreateHeaderTable(sheet, row + 2, col, null, ExcelHorizontalAlignment.Center, true);
                        col++;
                    }

                }

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col - 1, $"FUND ACTIVITY REPORT FROM DATE {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });
                var sumday = Fund.Balance;
                ExcelHelper.CreateCellTable(sheet, row - 1, 1, row - 1, col - 1, $"Fund balance: {sumday} ", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    NumberFormat = "#,##0",
                    BackgroundColor = colorHeader
                });
                #endregion

                var no = row + 3;

                if (listTreasure.Any())
                {
                    foreach (var treasure in listTreasure)
                    {
                        col = 1;
                        var count = treasure.IdPath.Split('.').Count() - 1;
                        if (treasure.ParentId == 0)
                        {
                            var str = treasure.Operator == false ? "-" : "+";
                            ExcelHelper.CreateCellTable(sheet, no, col + count, $"{treasure.Name.ToUpper()}({str})", ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, treasure.Name, ExcelHorizontalAlignment.Left, true);
                        }
                        col = col + arrayCount;
                        col++;
                        foreach (var date in listDate)
                        {
                            if (date.Date <= now)
                            {

                                if (treasure.Operator == false)
                                {
                                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.TreasureId == treasure.Id && x.LastUpdated.Date == date.Date).Sum(x => x.Diminishe), new CustomExcelStyle
                                    {
                                        IsMerge = false,
                                        IsBold = false,
                                        Border = ExcelBorderStyle.Thin,
                                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                                        NumberFormat = "#,##0"
                                    });
                                }
                                else
                                {
                                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, listFundBill.Where(x => x.TreasureId == treasure.Id && x.LastUpdated.Date == date.Date).Sum(x => x.Increase), new CustomExcelStyle
                                    {
                                        IsMerge = false,
                                        IsBold = false,
                                        Border = ExcelBorderStyle.Thin,
                                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                                        NumberFormat = "#,##0"
                                    });
                                }
                                col++;
                            }
                        }
                        no++;
                    }
                }

                col = 1;
                ExcelHelper.CreateHeaderTable(sheet, no, col, no, col + arrayCount, "Left", ExcelHorizontalAlignment.Left, true, colorFooter);
                col = col + arrayCount;
                col++;
                var sum1 = Fund.Balance;
                foreach (var date in listDate)
                {
                    if (date.Date <= now)
                    {

                        sum1 = Fund.Balance;
                        var fullbill = listFundBill.Where(s => s.LastUpdated.Date > date.Date && s.Status == (byte)FundBillStatus.Approved);
                        foreach (var item in fullbill)
                        {
                            if (item.Type == (byte)FundBillType.Increase)
                            {
                                sum1 -= item.Increase ?? 0;
                            }
                            else
                            {
                                sum1 += item.Diminishe ?? 0;
                            }
                        }
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, sum1, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0",
                            BackgroundColor = colorFooter
                        });
                        col++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"FUND_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }


        [HttpPost]
        public ActionResult ExportAccountExcel(string keyword, DateTime? startDay, DateTime? endDay, int? customerId)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                keyword = "";
            }
            //var id = financeFundId == 0 ? 1 : financeFundId;
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");
                var colorTd = ColorTranslator.FromHtml("#92D050");
                var colorFooter = ColorTranslator.FromHtml("#FFC000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var now = GetEndOfDay(DateTime.Now);

                long totalRecord;
                var listCustomer = UnitOfWork.CustomerRepo.Find(
                      out totalRecord,
                      x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Email.Contains(keyword)),
                      x => x.OrderBy(y => y.FullName),
                      1,
                      int.MaxValue
                  );

                var listCustomerWallet = UnitOfWork.CustomerWalletRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
                var listRechargeBill = UnitOfWork.RechargeBillRepo.Entities.Where(h => !h.IsDelete).ToList();
                if (customerId != null)
                {
                    listCustomer = listCustomer.Where(x => x.Id == customerId.Value).ToList();
                }

                var arrayCount = listCustomerWallet.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

                var sum = decimal.Parse("0");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + arrayCount, "ACCOUNT", ExcelHorizontalAlignment.Left, true);
                ExcelHelper.CreateHeaderTable(sheet, row + 1, col, row + 1, col + arrayCount, "Opening balance", ExcelHorizontalAlignment.Left, true, colorTd);
                ExcelHelper.CreateHeaderTable(sheet, row + 2, col, row + 2, col + arrayCount, "Activity", ExcelHorizontalAlignment.Left, true);
                col = col + arrayCount;
                col++;
                foreach (var cus in listCustomer)
                {
                    sum = cus.BalanceAvalible;
                    if (start <= now)
                    {
                        var fullbill = listRechargeBill.Where(s => s.CustomerId == cus.Id && s.Created.Date >= start.Date);

                        foreach (var item in fullbill)
                        {
                            var objectTest = listCustomerWallet.FirstOrDefault(s => s.Id == item.TreasureId);
                            if (item.Type == (byte)RechargeBillType.Increase)
                            {
                                sum -= item.Increase ?? 0;
                            }
                            else
                            {
                                sum += item.Diminishe ?? 0;
                            }

                        }
                    }

                    ExcelHelper.CreateHeaderTable(sheet, row, col, $"{cus.FullName} ({cus.Email})", ExcelHorizontalAlignment.Center, true);
                    ExcelHelper.CreateHeaderTable(sheet, row + 1, col, sum, ExcelHorizontalAlignment.Center, true, colorTd);
                    ExcelHelper.CreateCellTable(sheet, row + 1, col, row + 1, col, sum, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0"
                    });
                    ExcelHelper.CreateHeaderTable(sheet, row + 2, col, null, ExcelHorizontalAlignment.Center, true);
                    col++;

                }

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col - 1, $"REPORT OF E-WALLET ACTIVITY FROM DATE {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Left,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });
                #endregion

                var no = row + 3;

                if (listCustomerWallet.Any())
                {
                    foreach (var treasure in listCustomerWallet)
                    {
                        col = 1;
                        var count = treasure.IdPath.Split('.').Count() - 1;
                        if (treasure.ParentId == 0)
                        {
                            var str = treasure.Operator == false ? "-" : "+";
                            ExcelHelper.CreateCellTable(sheet, no, col + count, $"{treasure.Name.ToUpper()}({str})", ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, treasure.Name, ExcelHorizontalAlignment.Left, true);
                        }
                        col = col + arrayCount;
                        col++;
                        foreach (var cus in listCustomer)
                        {

                            if (treasure.Operator == false)
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listRechargeBill.Where(x => x.TreasureId == treasure.Id && x.CustomerId == cus.Id && x.Created.Date >= start.Date && x.Created.Date <= end.Date).Sum(x => x.Diminishe), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            else
                            {
                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, listRechargeBill.Where(x => x.TreasureId == treasure.Id && x.CustomerId == cus.Id && x.Created.Date >= start.Date && x.Created.Date <= end.Date).Sum(x => x.Increase), new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });
                            }
                            col++;
                        }
                        no++;
                    }
                }

                col = 1;
                ExcelHelper.CreateHeaderTable(sheet, no, col, no, col + arrayCount, "Ending balance", ExcelHorizontalAlignment.Left, true, colorTd);
                col = col + arrayCount;
                col++;

                foreach (var cus in listCustomer)
                {
                    sum = cus.BalanceAvalible;
                    if (start <= now)
                    {
                        var fullbill = listRechargeBill.Where(s => s.CustomerId == cus.Id && s.Created.Date > end.Date);
                        foreach (var item in fullbill)
                        {
                            var objectTest = listCustomerWallet.FirstOrDefault(s => s.Id == item.TreasureId);
                            if (item.Type == (byte)RechargeBillType.Increase)
                            {
                                sum -= item.Increase ?? 0;
                            }
                            else
                            {
                                sum += item.Diminishe ?? 0;
                            }
                        }
                    }

                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, sum, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0",
                        BackgroundColor = colorTd
                    });
                    col++;
                }
                no++;

                col = 1;
                ExcelHelper.CreateHeaderTable(sheet, no, col, no, col + arrayCount, "Current balance", ExcelHorizontalAlignment.Left, true, colorFooter);
                col = col + arrayCount;
                col++;

                foreach (var cus in listCustomer)
                {
                    ExcelHelper.CreateCellTable(sheet, no, col, no, col, cus.BalanceAvalible, new CustomExcelStyle
                    {
                        IsMerge = false,
                        IsBold = false,
                        Border = ExcelBorderStyle.Thin,
                        HorizontalAlign = ExcelHorizontalAlignment.Right,
                        NumberFormat = "#,##0",
                        BackgroundColor = colorFooter
                    });
                    col++;
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"RECHARGE_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //[HttpPost]
        //public ActionResult ExportAccountExcel(string keyword, DateTime? startDay, DateTime? endDay, int? customerId)
        //{
        //    if (string.IsNullOrEmpty(keyword))
        //    {
        //        keyword = "";
        //    }
        //    //var id = financeFundId == 0 ? 1 : financeFundId;
        //    using (var xls = new ExcelPackage())
        //    {
        //        var sheet = xls.Workbook.Worksheets.Add("Sheet1");
        //        var colorHeader = ColorTranslator.FromHtml("#FF0000");
        //        var colorTd = ColorTranslator.FromHtml("#92D050");
        //        var colorFooter = ColorTranslator.FromHtml("#FFC000");

        //        var start = startDay ?? DateTime.Now;
        //        var end = endDay ?? DateTime.Now;
        //        var now = GetEndOfDay(DateTime.Now);

        //        long totalRecord;
        //        var listCustomer = UnitOfWork.CustomerRepo.Find(
        //              out totalRecord,
        //              x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Email.Contains(keyword)),
        //              x => x.OrderBy(y => y.FullName),
        //              1,
        //              int.MaxValue
        //          );

        //        var listCustomerWallet = UnitOfWork.CustomerWalletRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
        //        var listRechargeBill = UnitOfWork.RechargeBillRepo.Entities.Where(h => !h.IsDelete).ToList();
        //        if (customerId != null)
        //        {
        //            listCustomer = listCustomer.Where(x => x.Id == customerId.Value).ToList();
        //        }

        //        var arrayCount = listCustomerWallet.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

        //        var sum = decimal.Parse("0");

        //        var col = 1;
        //        var row = 4;

        //        ExcelHelper.CreateHeaderTable(sheet, row, col, row + arrayCount, col, "ACCOUNT", ExcelHorizontalAlignment.Left, true);
        //        ExcelHelper.CreateHeaderTable(sheet, row, col + 1, row + arrayCount, col + 1, "Opening balance", ExcelHorizontalAlignment.Left, true, colorTd);
        //        ExcelHelper.CreateHeaderTable(sheet, row, col + 2, row, col + 2, "Activity", ExcelHorizontalAlignment.Left, true);
        //        ExcelHelper.CreateHeaderTable(sheet, row, col + 3, row + arrayCount, col + 3, "Ending balance", ExcelHorizontalAlignment.Left, true);
        //        ExcelHelper.CreateHeaderTable(sheet, row, col + 4, row + arrayCount, col + 4, "Còn (hiện có)", ExcelHorizontalAlignment.Left, true);
        //        row = 10;
        //        if (listCustomerWallet.Any())
        //        {
        //            col = 3;
        //            foreach (var treasure in listCustomerWallet)
        //            {
        //                var count = treasure.IdPath.Split('.').Count() - 1;

        //                var countChild = listCustomerWallet.Count(x => x.ParentId == treasure.Id);
        //                if (treasure.ParentId == 0)
        //                {
        //                    var str = treasure.Operator == false ? "-" : "+";
        //                    ExcelHelper.CreateCellTable(sheet, row + count, col, row + count, col + countChild, $"{treasure.Name.ToUpper()}({str})", ExcelHorizontalAlignment.Left, true);
        //                }
        //                //else
        //                //{
        //                //    ExcelHelper.CreateCellTable(sheet, row + count, col, row + count, col + countChild, treasure.Name, ExcelHorizontalAlignment.Left, true);
        //                //}
        //                col = col + countChild;
        //                col++;
        //            }
        //        }

        //        sheet.Cells.AutoFitColumns();
        //        sheet.Row(4).Height = 30;

        //        return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"RECHARGE_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
        //    }
        //}

        //Thống kê tồn quỹ theo thời gian

        [HttpPost]
        public ActionResult FinanceFundBalanceExcelSituationReport(DateTime? startDay, DateTime? endDay)
        {

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#FF0000");
                var colorTd = ColorTranslator.FromHtml("#92D050");
                var colorFooter = ColorTranslator.FromHtml("#FFC000");

                var start = startDay ?? DateTime.Now;
                var end = endDay ?? DateTime.Now;
                var now = GetEndOfDay(DateTime.Now);

                var listDate = new List<DateTime>();
                DateTime tmpDate = start;
                do
                {
                    listDate.Add(tmpDate);
                    tmpDate = tmpDate.AddDays(1);
                } while (tmpDate <= end);

                var Fund = UnitOfWork.FinaceFundRepo.Entities.Where(h => !h.IsDelete).OrderBy(x => x.IdPath).ToList();


                var listFundBill = UnitOfWork.FundBillRepo.Entities.Where(h => !h.IsDelete && h.Status == (byte)FundBillStatus.Approved).ToList();
                var arrayCount = Fund.Select(x => x.IdPath.Split('.').Count()).OrderByDescending(x => x).FirstOrDefault() - 1;

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, row, col + arrayCount, "FUND", ExcelHorizontalAlignment.Left, true);
                col = col + arrayCount;
                col++;
                foreach (var date in listDate)
                {
                    if (date.Date <= now)
                    {
                        ExcelHelper.CreateHeaderTable(sheet, row, col, date.ToShortDateString(), ExcelHorizontalAlignment.Center, true);
                        col++;

                    }

                }

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col - 1, $"REPORT OF FUND BALANCE FROM DATE {start.ToShortDateString()} TO DATE {end.ToShortDateString()}", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 18,
                    BackgroundColor = colorHeader
                });

                #endregion

                var no = row + 1;

                if (Fund.Any())
                {
                    foreach (var fund in Fund)
                    {
                        var listTreasure = UnitOfWork.TreasureRepo.Entities.Where(x => !x.IsDelete).OrderBy(x => x.IdPath).ToList();
                        col = 1;
                        var listChild = UnitOfWork.FinaceFundRepo.Entities.Where(x => !x.IsDelete && !x.IsParent && x.IdPath.StartsWith(fund.IdPath.ToString())).ToList();
                        var count = fund.IdPath.Split('.').Count() - 1;
                        if (fund.ParentId == 0)
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, $"{fund.Name.ToUpper()}", ExcelHorizontalAlignment.Left, true);
                        }
                        else
                        {
                            ExcelHelper.CreateCellTable(sheet, no, col + count, fund.Name, ExcelHorizontalAlignment.Left, true);
                        }
                        col = col + arrayCount;
                        col++;
                        foreach (var date in listDate)
                        {
                            if (date.Date <= now)
                            {
                                decimal sum = 0;
                                if (fund.IsParent)
                                {
                                    sum = fund.Balance;
                                }
                                foreach (var item in listChild)
                                {
                                    sum += item.Balance;
                                    var fullbill = listFundBill.Where(s => s.LastUpdated.Date > date.Date && s.FinanceFundId == item.Id && s.Status == (byte)FundBillStatus.Approved);
                                    foreach (var item1 in fullbill)
                                    {
                                        //var objectTest = listTreasure.FirstOrDefault(s => s.Id == item1.TreasureId);
                                        if (item1.Type == (byte)FundBillType.Increase)
                                        {
                                            sum -= item1.Increase ?? 0;
                                        }
                                        else
                                        {
                                            sum += item1.Diminishe ?? 0;
                                        }
                                    }
                                }

                                ExcelHelper.CreateCellTable(sheet, no, col, no, col, sum, new CustomExcelStyle
                                {
                                    IsMerge = false,
                                    IsBold = false,
                                    Border = ExcelBorderStyle.Thin,
                                    HorizontalAlign = ExcelHorizontalAlignment.Right,
                                    NumberFormat = "#,##0"
                                });

                                col++;
                            }

                        }
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"FUND_{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //Quay lại trạng thái trước đó
        [HttpPost]
        public ActionResult ViewRefundMoneyModalBack( int claimId)
        {
            //1. Xác nhận Position, phòng ban
            var isOrder = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Order);
            var isCustomerCare = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.CustomerCare);
            var isAccountant = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Accountancy);
            var isDirectorate = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Directorate);
            
            //2. Kiểm tra phiếu Refund có tồn tại?
            var claim = UnitOfWork.ClaimForRefundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == claimId);
            if (claim == null)
            {
                return Json(new { status = Result.Failed, msg = "Refund slip has been cancelled or does not exist" },
                        JsonRequestBehavior.AllowGet);
            }
            
            //3. Kiểm tra khiếu nại có tồn tại?
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(x => !x.IsDelete && x.Id == claim.TicketId);
            if(complain == null)
            {
                return Json(new { status = Result.Failed, msg = "Complaint has been cancelled or does not exist" },
                        JsonRequestBehavior.AllowGet);
            }
            
            //4. Lấy danh sách lịch sử trạng thái khiếu nại
            var listClaim = UnitOfWork.ComplainHistoryRepo.FindAsNoTracking(s => s.ComplainId == claim.TicketId).OrderByDescending(x=>x.CreateDate).ToList();
            //5. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
            var conplainHistory = new ComplainHistory();

            //5.1 Trường hợp khiếu nại đã cố phiếu Refund nhưng bị hủy nên tạo lại phiếu Refund
            if (isOrder)
            {
                //Mặc định là chuyển về cho CSKH
                conplainHistory.Status = (byte)ComplainStatus.CustomerCareWait;
            }
            else
            {
                if(listClaim[1].Status > listClaim[0].Status)
                {
                    if (listClaim[0].Status == (byte)ComplainStatus.CustomerCareWait && listClaim[1].Status == (byte)ComplainStatus.ApprovalWait)
                    {
                        conplainHistory.Status = listClaim[1].Status;
                    }
                    else
                    {
                        return Json(new { status = Result.Failed, msg = "Cannot give feedback about status!" },
                        JsonRequestBehavior.AllowGet);
                    }
                    
                }
                
                conplainHistory.Status = listClaim[1].Status;
            }
            
            conplainHistory.ComplainId = complain.Id;
            conplainHistory.CreateDate = DateTime.Now;
            
            conplainHistory.UserId = UserState.UserId;
            conplainHistory.UserFullName = UserState.FullName;
            conplainHistory.Content = "Return to previous status: " + EnumHelper.GetEnumDescription<ClaimForRefundStatus>(listClaim[1].Status);
            conplainHistory.CustomerId = complain.CustomerId;
            conplainHistory.CustomerName = complain.CustomerName;

            UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
            UnitOfWork.ComplainHistoryRepo.Save();

            complain.Status = conplainHistory.Status;
            complain.LastUpdateDate = DateTime.Now;
            UnitOfWork.ComplainRepo.Save();

            if (isOrder)
            {
                claim.Status = (byte)ClaimForRefundStatus.CustomerCareWait;
            }
            else
            {
                claim.Status = (complain.Status ?? 0 )- 2;
            }
                
            claim.LastUpdated = DateTime.Now;
            UnitOfWork.ComplainRepo.Save();
            
            string st = "Status is returned to " + EnumHelper.GetEnumDescription<ClaimForRefundStatus>(claim.Status);
            return Json(new { status = Result.Succeed, msg = st }, JsonRequestBehavior.AllowGet);
        }
    }
}