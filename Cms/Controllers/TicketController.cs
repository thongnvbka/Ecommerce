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
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;
using System.Runtime.ExceptionServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Common.Items;
using ResourcesLikeOrderThaiLan;

namespace Cms.Controllers
{
    [Authorize]
    public class TicketController : BaseController
    {
        // GET: Ticket
        [LogTracker(EnumAction.View, EnumPage.TicketAssign, EnumPage.TicketComplain, EnumPage.TicketForMe, EnumPage.TicketLast, EnumPage.TicketSupport, EnumPage.TicketSearchCustomer, EnumPage.TicketClaimforrefund, EnumPage.TicketReportForDay, EnumPage.OrderWaitNew, EnumPage.OrderWait, EnumPage.OrderCustomerCare)]
        public ActionResult Index()
        {
            //Lấy danh sách Type khiếu nại
            ViewBag.complainTypeJsTree = ComplainTypeActonJsTree();

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            //Danh sach loai order
            ViewBag.OrderType = JsonConvert.SerializeObject(Enum.GetValues(typeof(OrderType))
                    .Cast<OrderType>()
                    .ToDictionary(v => v.ToString(), v => (byte)v),
                jsonSerializerSettings);

            // Danh sách kho việt nam
            ViewBag.ListWarehouseVN = JsonConvert.SerializeObject(UnitOfWork.DbContext.Offices.Select(x => new { x.Id, x.Code, x.Name, x.Address, x.Culture, x.Type, x.IsDelete }).Where(x => x.Culture == "VN" && x.Type == (byte)OfficeType.Warehouse && !x.IsDelete));

            // Lấy danh sách Type khiếu nại
            var complainTypeService = new List<dynamic>();
            var listtype = UnitOfWork.ComplainTypeRepo.Find(x =>/* x.IsParent && */x.ParentId == 0 && !x.IsDelete).OrderBy(x => x.Index).ToList();
            foreach (var item in listtype)
            {
                complainTypeService.Add(new { Text = item.Name, Value = item.Id });
            }

            ViewBag.ListComplainTypeService = JsonConvert.SerializeObject(complainTypeService);

            //Danh sách Type khiếu nại cho CSKH chốt
            var listtypeClose = UnitOfWork.ComplainTypeRepo.Find(x => /*!x.IsParent && */!x.IsDelete).OrderBy(x => x.Index).ToList();
            var complainType = new List<SelectListItem>();
            complainType.Add(new SelectListItem { Text = "SELECT TYPE COMPLAINTS", Value = "0" });
            foreach (var item in listtypeClose)
            {
                complainType.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });
            }
            ViewBag.ListcomplainType = JsonConvert.SerializeObject(complainType);
            return View();
        }
        #region [Lấy ID ticket]
        [HttpPost]
        public async Task<JsonResult> GetTicketId(string code)
        {
            var ticket = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(x => x.Code == code && !x.IsDelete);

            return ticket == null ? Json(new { status = MsgType.Error, msg = "Complaint does not exist or has been deleted!" },
                JsonRequestBehavior.AllowGet) : Json(new { status = MsgType.Success, msg = "", id = ticket.Id },
                JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Tạo Ticket]
        // Tạo ticket
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.TicketAssign)]
        public async Task<JsonResult> CreateTicket(Complain complain, List<ComplainLinkDetail> list)
        {
            var Code = RemoveCode(complain.OrderCode);
            complain.OrderCode = Code;
            var ticket = new Complain();
            Mapper.Map(complain, ticket);
            var message = ConstantMessage.CreateComplainIsSuccess;

            var order = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Code == Code);
            var customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == complain.CustomerId);
            if (order == null)
            {
                return Json(new { status = Result.Failed, msg = "The customer has no Order entered!" }, JsonRequestBehavior.AllowGet);
            }
            //Lấy tên Type khiếu nại
            var typeComplain = UnitOfWork.ComplainTypeRepo.FirstOrDefault(s => !s.IsDelete && s.Id == complain.TypeService);
            if (typeComplain == null)
            {
                return Json(new { status = Result.Failed, msg = "No type of complaint exists or has been deleted!" }, JsonRequestBehavior.AllowGet);
            }
            ticket.TypeServiceName = typeComplain.Name;
            ticket.CustomerName = customer.FullName;
            ticket.OrderId = order.Id;
            ticket.OrderType = order.Type;
            ticket.CreateDate = DateTime.Now;
            ticket.LastUpdateDate = DateTime.Now;
            ticket.SystemId = customer.SystemId;
            ticket.SystemName = customer.SystemName;
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    UnitOfWork.ComplainRepo.Add(ticket);
                    UnitOfWork.ComplainRepo.Save();

                    //Gán ticket cho CSKH theo dõi Order
                    if (order.CustomerCareUserId != null)
                    {
                        ticket.Status = (byte)ComplainStatus.Process;
                        var complainuseradd = new ComplainUser
                        {
                            UserId = order.CustomerCareUserId,
                            UserName = order.CustomerCareName,
                            OfficeId = order.CustomerCareOfficeId,
                            OfficeIdPath = order.CustomerCareOfficeIdPath,
                            OfficeName = order.CustomerCareOfficeName,
                            UpdateDate = DateTime.Now,
                            CreateDate = DateTime.Now,
                            ComplainId = ticket.Id,
                            IsCare = true
                        };
                        //Thêm Detail khiếu nại
                        UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                        UnitOfWork.ComplainUserRepo.Save();

                        UnitOfWork.ComplainRepo.Save();

                        message = "Compain " + ticket.Code + " Assigned to customer care" + order.CustomerCareName + "Processed by customer care " + order.CustomerCareName + " Tracking Order" + order.Code;
                    }
                    //Gui Notify cho nguoi mac dinh nhan khieu nai
                    var description = ticket.Content == null ? "" : ticket.Content == "" ? "" : ticket.Content.Substring(0, ticket.Content.Length - 1);
                    if (description != "")
                    {
                        if (ticket.Content.Length > 500)
                        {
                            description = ticket.Content.Substring(0, 500);
                        }
                    }
                    var notification = new Notification()
                    {
                        SystemId = (int)ticket.SystemId,
                        SystemName = ticket.SystemName,
                        CustomerId = ticket.CustomerId,
                        CustomerName = ticket.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = (int)ticket.Id,
                        OrderType = 3,
                        IsRead = false,
                        Title = string.Format("{0} Complete complaint handling {1} cho Order {2}", ticket.SystemName, ticket.Code, ticket.OrderCode),
                        Description = string.Format("{0} Complete complaint handling {1} cho Order {2} with content: {3} ", ticket.SystemName, ticket.Code, ticket.OrderCode, description)
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    UnitOfWork.ComplainRepo.Save();
                    // Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = ticket.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = ticket.CustomerId;
                    conplainHistory.CustomerName = ticket.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Wait;
                    conplainHistory.Content = "Create compaint customer care";

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            if (item.Note != "undefined" && item.Note != null && item.Note != "")
                            {
                                var complainOrder = new ComplainOrder();
                                complainOrder.ComplainId = ticket.Id;
                                complainOrder.OrderDetailId = item.Id;
                                complainOrder.Note = item.Note;
                                complainOrder.LinkOrder = item.Index;
                                complainOrder.CreateDate = DateTime.Now;
                                UnitOfWork.ComplainOrderRepo.Add(complainOrder);
                                UnitOfWork.ComplainOrderRepo.Save();
                            }

                        }
                    }



                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            var totalWait = UnitOfWork.ComplainRepo.Count(x => !x.IsDelete && x.Status == (byte)ComplainStatus.Wait);
            return Json(new { status = Result.Succeed, msg = message, totalWait, list }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thông tin trang hiển thị
        public async Task<JsonResult> GetAllSearchData()
        {
            var listStatusRefund = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listStatus = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listSystem = new List<dynamic>() { new { Text ="All", Value = -1 } };
            var listUser = new List<SearchMeta>();

            //1. Lấy các trạng thái Status của Complain
            foreach (ComplainStatus ticketStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (ticketStatus >= 0)
                {
                    listStatus.Add(new { Text = ticketStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)ticketStatus });
                }
            }

            //2. Lấy danh sách System
            var listSystemDb = await UnitOfWork.SystemRepo.FindAsync(x => x.Id > 0);
            foreach (var item in listSystemDb)
            {
                listSystem.Add(new
                {
                    Text = item.Domain,
                    Value = item.Id,
                });
            }

            //3. Lấy ra danh sách nhân viên
            if (UserState.Type != null && UserState.OfficeId != null)
            {
                var listUserTemp = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, (byte)UserState.Type, UserState.OfficeIdPath, (int)UserState.OfficeId);
                var tempUserList = from p in listUserTemp
                                   select new SearchMeta() { Text = p.UserName + " - " + p.FullName, Value = p.Id };

                listUser.Add(new SearchMeta() { Text = "- All -", Value = -1 });
                listUser.AddRange(tempUserList.ToList());
            }
            //4. Lấy danh sách trạng thái Refund
            foreach (ClaimForRefundStatus refundStatus in Enum.GetValues(typeof(ClaimForRefundStatus)))
            {
                if (refundStatus >= 0)
                {
                    listStatusRefund.Add(new { Text = refundStatus.GetAttributeOfType<DescriptionAttribute>().Description, Value = (int)refundStatus });
                }
            }

            return Json(new { listStatus, listSystem, listUser, listStatusRefund }, JsonRequestBehavior.AllowGet);
        }
        //Lấy danh sách system
        [HttpPost]
        public async Task<JsonResult> GetRenderSystem(string active)
        {
            var userId = UserState.UserId;
            var dateDelay = DateTime.Now.AddDays(-1);
            var listStatus = new List<dynamic>() { new { Text = "- All -", Value = -1 } };
            foreach (ComplainStatus complainStatus in Enum.GetValues(typeof(ComplainStatus)))
            {
                if (complainStatus != 0)
                    listStatus.Add(new { Value = (int)complainStatus, Text = complainStatus.GetAttributeOfType<DescriptionAttribute>().Description });
            }

            List<SystemMeta> listComplain;
            if (active == "ticket")
            {
                listComplain = UnitOfWork.ComplainRepo.Entities.Where(x => /*!x.IsDelete &&*/ x.Status > (byte)ComplainStatus.Wait && x.Status < (byte)ComplainStatus.Success)
                                .Join(UnitOfWork.ComplainUserRepo.Entities.Where(x => x.UserId == userId && x.IsCare == true),
                                complain => complain.Id,
                                complainuser => complainuser.ComplainId,
                                (c, cu) => c).Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }
            else if (active == "support")
            {
                listComplain = UnitOfWork.ComplainRepo.Entities.Where(x => /*!x.IsDelete &&*/ x.Status > (byte)ComplainStatus.Wait)
                                 .Join(UnitOfWork.ComplainUserRepo.Entities.Where(x =>  x.IsCare == false),
                                 complain => complain.Id,
                                 complainuser => complainuser.ComplainId,
                                 (c, cu) => new { c.Id, c.SystemId }).Distinct().Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }
            else if(active == "last")
            {
                listComplain = UnitOfWork.ComplainRepo.Find(x => x.CreateDate <= dateDelay /*&& !x.IsDelete*/).Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }

            else if(active == "assign")
            {
                listComplain = UnitOfWork.ComplainRepo.Find(x => x.Status < (byte)ComplainStatus.Process /*&& !x.IsDelete*/).Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }
            else
            {
                listComplain = UnitOfWork.ComplainRepo.Find(x => x.Status > (byte)ComplainStatus.Wait /*&& !x.IsDelete*/).Select(x => new SystemMeta() { SystemId = (int)x.SystemId }).ToList();
            }
            
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

            return Json(new { listSystem, listStatus }, JsonRequestBehavior.AllowGet);
        }

        // Dữ liệu thông báo
        public async Task<JsonResult> GetInit()
        {
            var totalTicketWait = 0;
            var totalTicket = 0;
            var totalTicketNeedFix = 0;
            var totalTicketLate = 0;
            var totalTicketSupport = 0;
            var totalOrderWaitNew = 0;
            var totalOrderWait = 0;

            var ticketModal = new List<Complain>();
            var ticketModalsupport = new List<Complain>();

            //1. Tính Sum Ticket chưa nhận xử lý
            totalTicketWait = await UnitOfWork.ComplainRepo.CountTicketWait();

            //2. Tính Sum Ticket khiếu nại
            totalTicket = await UnitOfWork.ComplainRepo.CountTicketComplain(UserState);

            //3. Số ticket tôi cần xử lý
            totalTicketNeedFix = await UnitOfWork.ComplainRepo.CountTicketAssignForUser(UserState.UserId);

            //4. Số ticket trễ xử lý
            totalTicketLate = await UnitOfWork.ComplainRepo.CountTicketPending(UserState);

            //5. Số ticket có người hỗ trợ
            totalTicketSupport = await UnitOfWork.ComplainRepo.CountTicketHasSupporter(UserState);

            //6. Số phiếu Refund khiếu nại
            var totalClaimForRefund = UnitOfWork.ClaimForRefundRepo.Find(s => (UserState.Type > 0 || s.UserId == UserState.UserId)
            ).Count();

            //7.Lấy thông tin Order chờ báo giá
            totalOrderWaitNew = UnitOfWork.OrderRepo.Count(x => !x.IsDelete && x.CustomerCareUserId == null && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.WaitPrice);
            totalOrderWait = UnitOfWork.OrderRepo.Count(x => !x.IsDelete && x.CustomerCareUserId == UserState.UserId && x.Type == (byte)OrderType.Order && x.Status == (byte)OrderStatus.AreQuotes); ;

            return Json(new { totalTicket, totalTicketNeedFix, totalTicketWait, totalTicketLate, totalTicketSupport, totalClaimForRefund, totalOrderWaitNew, totalOrderWait }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Các hàm dùng chung

        public int CountUser(long complainId)
        {
            var userId = UserState.UserId;
            var count = UnitOfWork.ComplainUserRepo.Find(d => d.ComplainId == complainId && d.UserId == userId && d.IsCare != null).Count();
            return count;
        }

        //Lấy về vị trí  công tác
        public string Position(int id)
        {
            var x = "";
            //var user = UnitOfWork.DbContext.UserPositions.SingleOrDefault(d => d.UserId == id && d.IsDefault);
            var user = UnitOfWork.DbContext.UserPositions.FirstOrDefault(d => d.UserId == id && d.IsDefault);
            if (user != null)
            {
                x = user.TitleName;
            }
            return x;
        }

        //Lấy về danh sách User theo phòng 
        public JsonResult GetUserSearch(string keyword, int? page)
        {
            long totalRecord;
            var listUser = UnitOfWork.ComplainRepo.GetUserSearch(out totalRecord, keyword, page);
            return Json(new { incomplete_results = true, total_count = totalRecord, items = listUser.Select(x => new { id = x.Id, text = x.FullName, email = x.Email, phone = x.Phone, avatar = x.Avatar }) }, JsonRequestBehavior.AllowGet);
        }

        #region Tra cứu thông tin khách hàng
        //ALL CUSTEMOR
        [HttpPost]
        public JsonResult GetAllCustomer()
        {
            int userId = UserState.UserId;
            var customer = new List<Customer>();

            if (UserState.Type > 0)
            {
                customer = UnitOfWork.CustomerRepo.Find(s => s.UserId == userId).ToList();

            }
            return Json(customer, JsonRequestBehavior.AllowGet);
        }

        #region [Tra cứu thông tin khách hàng]
        //TRA CỨU THÔNG TIN KHÁCH HÀNG
        public JsonResult GetCustomerSearch(string keyword, int? page)
        {
            long totalRecord;
            keyword = MyCommon.Ucs2Convert(keyword);
            var listCustomer = new List<Customer>();
            var business = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Business);
            if (business)
            {
                listCustomer = UnitOfWork.CustomerRepo.Find(
                   out totalRecord,
                   x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.SystemName.Contains(keyword))
                   && (UserState.Type != 0 && UserState.OfficeId == x.OfficeId || x.UserId == UserState.UserId),
                   x => x.OrderByDescending(y => y.FullName),
                   page ?? 1,
                   10
              ).ToList();
            }
            else
            {
                listCustomer = UnitOfWork.CustomerRepo.Find(
                    out totalRecord,
                    x => !x.IsDelete && x.IsActive && (x.FullName.Contains(keyword) || x.Email.Contains(keyword) || x.Phone.Contains(keyword) || x.SystemName.Contains(keyword)),
                    x => x.OrderByDescending(y => y.FullName),
                    page ?? 1,
                    10
               ).ToList();
            }


            return Json(new { incomplete_results = true, total_count = totalRecord, items = listCustomer.Select(x => new { id = x.Id, code = x.Code, text = x.FullName, email = x.Email, phone = x.Phone, avatar = x.Avatar, systemName = x.SystemName, address = x.Address, depositPrice = x.DepositPrice }) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetCustomerInfo(int? customerId)
        {
            var customer = new Customer();
            var listOrder = new List<dynamic>();
            if (customerId > 0 && customerId != null)
            {
                customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == customerId);
                listOrder.AddRange(UnitOfWork.OrderRepo.Find(x => x.CustomerId == customer.Id && x.Status > (byte)OrderStatus.WaitOrder).Select(x => new { x.Id, x.Code, x.Type }).ToList());
            }
            return Json(new { customer, listOrder }, JsonRequestBehavior.AllowGet);
        }
        //Lịch sử hỗ trợ khách hàng
        [HttpPost]
        public JsonResult SupportHistory(int customerId)
        {
            //Lấy danh sách khiếu nại của khách hàng có mã customerId
            var complainlist = UnitOfWork.ComplainRepo.Find(s => s.CustomerId == customerId);


            var complainuserlist = new List<ComplainUser>();
            foreach (var item in complainlist)
            {
                var x = UnitOfWork.ComplainUserRepo.Find(s => s.ComplainId == item.Id);
                complainuserlist.AddRange(x);
            }
            var complainuserlist1 = complainuserlist.GroupBy(s => s.ComplainId);

            var customer = (from s in UnitOfWork.DbContext.Complains
                            where s.CustomerId == customerId && !s.IsDelete
                            orderby s.CreateDate
                            select new { s.CreateDate }).Distinct();
            var customerinfo = from s in UnitOfWork.DbContext.Complains
                               where s.CustomerId == customerId && !s.IsDelete
                               select s;
            return Json(new { customer, customerinfo }, JsonRequestBehavior.AllowGet);
        }

        //Lấy danh sách hỗ trợ khiếu nại 
        public async Task<JsonResult> CustomerListLookUp(int complainId)
        {
            var complainuserlist = new List<ComplainDetail>();
            var complainuserinternallist = new List<ComplainDetail>();
            //1. danh sách trao đổi với khách hàng
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == complainId
               && p.IsInHouse == false
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );
            //2. Nội dung danh sách trao đổi với khách hàng
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

            //3. Danh sách trao đổi nội bộ
            var complainuserinternal = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == complainId
               && p.IsInHouse == true
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );

            //4. Nội dung danh sách trao đổi nội bộ
            foreach (var item in complainuserinternal)
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

                complainuserinternallist.Add(new ComplainDetail
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
            return Json(new { complainuserlist, complainuserinternallist }, JsonRequestBehavior.AllowGet);
        }

        //Lịch sử Order
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
            return Json(new { totalRecord, customer }, JsonRequestBehavior.AllowGet);
        }
        //Sao kê ví điện tử
        [HttpPost]
        public async Task<JsonResult> OrderMoney(int customerId, int page, int pageSize)
        {
            int userId = UserState.UserId;
            long totalRecord;
            var customer = new List<RechargeBill>();
            customer = await UnitOfWork.RechargeBillRepo.FindAsync(
                 out totalRecord,
                 s => s.CustomerId == customerId && !s.IsDelete,
                   x => x.OrderByDescending(y => y.Id),
                   page,
                   pageSize);
            return Json(new { totalRecord, customer }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        // Thông tin hỗ trợ khách hàng khiếu nại
        [HttpPost]
        public async Task<JsonResult> listComplainSupport(int complainId)
        {
            var complainuserlist = new List<ComplainDetail>();
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
              p => p.ComplainId == complainId
              && p.UserId != null,
              null,
               x => x.OrderBy(y => y.CreateDate)
              );
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
            return Json(complainuserlist, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #endregion

        #region Ticket chưa nhận xử lý
        // LẤY DANH SÁCH TICKET CHƯA PHÂN VIỆC
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketAssign)]
        public async Task<JsonResult> GetAllTicketAssignList(int page, int pageSize, ComplainSearchModal searchModal)
        {

            var ticketModal = new List<TicketComplain>();
            long totalRecord;
            var userId = UserState.UserId;
            var complain = new Complain();

            //var complainuserlist = (from d in UnitOfWork.ComplainUserRepo.Find(s => s.UserId == userId).OrderByDescending(s => s.CreateDate)
            //                        select new { d.ComplainId }).Distinct().ToList();

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            if (!string.IsNullOrEmpty(searchModal.DateStart))
            {
                var DateStart = DateTime.Parse(searchModal.DateStart);
                var DateEnd = DateTime.Parse(searchModal.DateEnd);
                ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketAssignList(
                    out totalRecord, page, pageSize, searchModal.Keyword, searchModal.SystemId, DateStart, DateEnd);
            }
            else
            {
                ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketAssignList(
                    out totalRecord, page, pageSize, searchModal.Keyword, searchModal.SystemId, null, null);
            }

            //lấy list CustomerId
            var listCustomerId = ticketModal.Select(x => x.CustomerId).ToList();
            //Lấy danh sách khách hàng
            var listCustomer = UnitOfWork.CustomerRepo.Entities.Where(x => listCustomerId.Contains(x.Id)).Select(x => new { x.Id, x.Email, x.Phone, x.WarehouseName }).ToList();

            return Json(new { totalRecord, ticketModal, listCustomer }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Nhận Ticket về xử lý
        /// </summary>
        /// <param name="complainId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ReceiveTicket(int complainId)
        {

            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = await UnitOfWork.ComplainRepo.SingleOrDefaultAsync(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra xem đã có nhân viên nhận xử lý Ticket đó hay chưa
            var complainUser = await UnitOfWork.ComplainUserRepo.SingleOrDefaultAsync(x => x.ComplainId == complainId && x.IsCare == true);
            if (complainUser != null)
            {
                return Json(new { status = Result.Failed, msg = "There was staff handling this ticket !" }, JsonRequestBehavior.AllowGet);
            }
            //3. Lấy thông tin Detail don hang
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == complainDetail.OrderId);
            if (order == null)
            {
                return Json(new { status = Result.Failed, msg = "Order does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            //3. Lưu thông tin vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin người xử lý Ticket vào ComplainUser
                    var complainuseradd = new ComplainUser
                    {
                        UserId = UserState.UserId,
                        UserName = UserState.FullName,
                        OfficeId = UserState.OfficeId,
                        OfficeIdPath = UserState.OfficeIdPath,
                        OfficeName = UserState.OfficeName,
                        UpdateDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        ComplainId = complainId,
                        IsCare = true
                    };
                    //Thêm Detail khiếu nại
                    UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                    UnitOfWork.ComplainUserRepo.Save();

                    // Lưu thông tin Ticket có người đã nhận xử lý
                    complainDetail.Status = (byte)ComplainStatus.Process;
                    complainDetail.LastUpdateDate = DateTime.Now;
                    // Gửi thông báo Notification cho khách hàng
                    var description = complainDetail.Content == null ? "" : complainDetail.Content == "" ? "" : complainDetail.Content.Substring(0, complainDetail.Content.Length - 1);
                    if (description != "")
                    {
                        if (complainDetail.Content.Length > 500)
                        {
                            description = complainDetail.Content.Substring(0, 500);
                        }
                    }
                    var notification = new Notification()
                    {
                        SystemId = (int)complainDetail.SystemId,
                        SystemName = complainDetail.SystemName,
                        CustomerId = complainDetail.CustomerId,
                        CustomerName = complainDetail.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = complainId,
                        OrderType = 3,
                        IsRead = false,
                        Title = string.Format("{0} Receive complaint handling {1} cho Order {2}", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode),
                        Description = string.Format("{0} Receive complaint handling {1} cho Order {2} with content: {3} ", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode, description)
                    };

                    UnitOfWork.NotificationRepo.Add(notification);
                    // Cập nhật trạng thái khiếu nại
                    UnitOfWork.ComplainRepo.Save();

                    //Cap nhat thong tin don hang
                    if(order.CustomerCareUserId == null )
                    {
                        order.CustomerCareUserId = UserState.UserId;
                        order.CustomerCareName = UserState.FullName;
                        order.CustomerCareOfficeIdPath = UserState.OfficeIdPath;
                        order.CustomerCareFullName = UserState.FullName;
                        order.CustomerCareOfficeId = UserState.OfficeId;
                        order.CustomerCareOfficeName = UserState.OfficeName;
                        UnitOfWork.OrderRepo.Save();
                    }
                    

                    // Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complainDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complainDetail.CustomerId;
                    conplainHistory.CustomerName = complainDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Process;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Transfer status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Process);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    throw;
                    // return Json(new { status = Result.Failed, msg = "[Lỗi system] liên lạc nhân viên kĩ thuật để được trợ giúp !" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = Result.Succeed, msg = "Get Ticket Processing Successful !" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Hàm phân ticket giao việc cho nhân viên trong danh sách trễ xử lý
        /// </summary>
        /// <param name="complainId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.TicketAssign, EnumPage.TicketLast)]
        public async Task<JsonResult> AssignedComplain(int complainId, UserOfficeResult user)
        {
            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = await UnitOfWork.ComplainRepo.SingleOrDefaultAsync(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin Detail nhân viên
            var staffDetail = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == user.Id);
            if (staffDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Staff does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //4. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var complainUser = await UnitOfWork.ComplainUserRepo.SingleOrDefaultAsync(x => x.ComplainId == complainId && x.IsCare == true);
                    //Đã có người nhận xử lý
                    if (complainUser != null)
                    {
                        if (complainDetail.Status > (byte)ComplainStatus.Process && complainDetail.Status < (byte)ComplainStatus.Cancel)
                        {
                            var claim = await UnitOfWork.ClaimForRefundRepo.SingleOrDefaultAsync(x => x.TicketId == complainId && !x.IsDelete);
                            if (claim == null)
                            {
                                return Json(new { status = Result.Failed, msg = "Complaints not exist request a refund processor !" }, JsonRequestBehavior.AllowGet);
                            }
                            claim.UserId = staffDetail.Id;
                            claim.UserName = staffDetail.UserName;
                            claim.UserPhone = staffDetail.Phone;
                            claim.UserEmail = staffDetail.Email;
                            claim.OfficeId = user.OfficeId;
                            claim.OfficeIdPath = user.OfficeIdPath;
                            claim.OfficeName = user.OfficeName;
                            claim.LastUpdated = DateTime.Now;

                            //Cập nhật thông tin phiếu yêu cầu Refund
                            UnitOfWork.ClaimForRefundRepo.Save();

                        }
                        complainUser.UserId = staffDetail.Id;
                        complainUser.UserName = staffDetail.FullName;
                        complainUser.ComplainId = complainId;
                        complainUser.UpdateDate = DateTime.Now;
                        complainUser.OfficeId = user.OfficeId;
                        complainUser.OfficeIdPath = user.OfficeIdPath;
                        complainUser.OfficeName = user.OfficeName;


                        //Cập nhật thông tin Detail Complain
                        UnitOfWork.ComplainUserRepo.Save();
                    }
                    else
                    {
                        // Nếu chưa có người nhận xử lý
                        var complainuseradd = new ComplainUser
                        {
                            UserId = staffDetail.Id,
                            UserName = staffDetail.FullName,
                            ComplainId = complainId,
                            OfficeId = user.OfficeId,
                            OfficeIdPath = user.OfficeIdPath,
                            OfficeName = user.OfficeName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            IsCare = true
                        };

                        // Lưu thông tin Ticket có người đã nhận xử lý
                        complainDetail.Status = (byte)ComplainStatus.Process;
                        complainDetail.LastUpdateDate = DateTime.Now;
                        // Gửi thông báo Notification cho khách hàng
                        var description = complainDetail.Content == null ? "" : complainDetail.Content == "" ? "" : complainDetail.Content.Substring(0, complainDetail.Content.Length - 1);
                        if (description != "")
                        {
                            if (complainDetail.Content.Length > 500)
                            {
                                description = complainDetail.Content.Substring(0, 500);
                            }
                        }
                        var notification = new Notification()
                        {
                            SystemId = (int)complainDetail.SystemId,
                            SystemName = complainDetail.SystemName,
                            CustomerId = complainDetail.CustomerId,
                            CustomerName = complainDetail.CustomerName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderId = complainId,
                            OrderType = 3,
                            IsRead = false,
                            Title = string.Format("{0} Receive complaint handling {1} cho Order {2}", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode),
                            Description = string.Format("{0} Receive complaint handling {1} cho Order {2} Content: {3} ", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode, description)
                        };
                        UnitOfWork.NotificationRepo.Add(notification);
                        // Cập nhật thông tin Complain
                        UnitOfWork.ComplainRepo.Save();

                        //Thêm Detail Complain
                        UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                        UnitOfWork.ComplainUserRepo.Save();

                        // Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                        var conplainHistory = new ComplainHistory();
                        conplainHistory.ComplainId = complainDetail.Id;
                        conplainHistory.CreateDate = DateTime.Now;
                        conplainHistory.CustomerId = complainDetail.CustomerId;
                        conplainHistory.CustomerName = complainDetail.CustomerName;
                        conplainHistory.Status = (byte)ComplainStatus.Process;
                        conplainHistory.UserId = staffDetail.Id;
                        conplainHistory.UserFullName = staffDetail.FullName;
                        conplainHistory.Content = "Transfer status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Process);

                        UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                        UnitOfWork.ComplainHistoryRepo.Save();

                    }
                    // Gửi Notify cho nhân viên CSKH
                    NotifyHelper.CreateAndSendNotifySystemToClient(user.Id,
                        "Xử lý ticket ",
                        EnumNotifyType.Info,
                        $" <a href=\"" + "/Ticket/#TK" + complainDetail.Code + "\" target=\"_blank\">" + complainDetail.Code + "</a>",
                        "Yêu cầu xử lý ticket: " + complainDetail.Code,
                        Url.Action("Index", "Ticket"));
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = Result.Succeed, msg = "Staff work ticket assignment is successful!" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Phân Ticket cho Staff handling khi chưa nhận xử lý
        /// </summary>
        /// <param name="complainId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.TicketAssign)]
        public async Task<JsonResult> AssignedComplainNotGet(int complainId, UserOfficeResult user)
        {
            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = await UnitOfWork.ComplainRepo.SingleOrDefaultAsync(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra xem đã có nhân viên nhận xử lý Ticket đó hay chưa
            var complainUser = await UnitOfWork.ComplainUserRepo.SingleOrDefaultAsync(x => x.ComplainId == complainId && x.IsCare == true);
            if (complainUser != null)
            {
                return Json(new { status = Result.Failed, msg = "There was staff handling this ticket !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin Detail nhân viên
            var staffDetail = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == user.Id);
            if (staffDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Nhân viên does not exist hoặc đã bị xóa !" }, JsonRequestBehavior.AllowGet);
            }
            //3. Lấy thông tin Detail don hang
            var order = await UnitOfWork.OrderRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == complainDetail.OrderId);
            if (order == null)
            {
                return Json(new { status = Result.Failed, msg = "Don does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            //4. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin người xử lý Ticket vào ComplainUser
                    var complainuseradd = new ComplainUser
                    {
                        UserId = staffDetail.Id,
                        UserName = staffDetail.FullName,
                        ComplainId = complainId,
                        OfficeId = user.OfficeId,
                        OfficeIdPath = user.OfficeIdPath,
                        OfficeName = user.OfficeName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsCare = true
                    };
                    UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                    UnitOfWork.ComplainUserRepo.Save();

                    // Lưu thông tin Ticket có người đã nhận xử lý
                    complainDetail.Status = (byte)ComplainStatus.Process;
                    complainDetail.LastUpdateDate = DateTime.Now;

                    UnitOfWork.ComplainUserRepo.Save();

                    //Cap nhat thong tin don hang
                    if (order.CustomerCareUserId == null)
                    {
                        order.CustomerCareUserId = user.Id;
                        order.CustomerCareName = user.FullName;
                        order.CustomerCareOfficeIdPath = user.OfficeIdPath;
                        order.CustomerCareFullName = user.FullName;
                        order.CustomerCareOfficeId = user.OfficeId;
                        order.CustomerCareOfficeName = user.OfficeName;
                        UnitOfWork.OrderRepo.Save();
                    }
                
                    // Gửi Notify cho nhân viên CSKH
                    NotifyHelper.CreateAndSendNotifySystemToClient(user.Id,
                        "Xử lý ticket ",
                        EnumNotifyType.Info,
                        $" <a href=\"" + "/Ticket/#TK" + complainDetail.Code + "\" target=\"_blank\">" + complainDetail.Code + "</a>",
                        "Yêu cầu xử lý ticket: " + complainDetail.Code, Url.Action("Index", "Ticket"));


                    // Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complainDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complainDetail.CustomerId;
                    conplainHistory.CustomerName = complainDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Process;
                    conplainHistory.UserId = staffDetail.Id;
                    conplainHistory.UserFullName = staffDetail.FullName;
                    conplainHistory.Content = "Switch to status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Process);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }


            return Json(new { status = Result.Succeed, msg = "Staff work ticket assignment is successful !" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckPermission(EnumAction.Approvel, EnumPage.TicketAssign)]
        public async Task<JsonResult> AssignedComplainAuto(int complainId, int userId)
        {
            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = await UnitOfWork.ComplainRepo.SingleOrDefaultAsync(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra xem đã có nhân viên nhận xử lý Ticket đó hay chưa
            var complainUser = await UnitOfWork.ComplainUserRepo.SingleOrDefaultAsync(x => x.ComplainId == complainId && x.IsCare == true);
            if (complainUser != null)
            {
                return Json(new { status = Result.Failed, msg = "There was staff handling this ticket !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin Detail nhân viên
            var staffDetail = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == userId);
            if (staffDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Staff does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            //Position
            var office = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(x => x.UserId == userId);
            if (office == null)
            {
                return Json(new { status = Result.Failed, msg = "Staff no location!" }, JsonRequestBehavior.AllowGet);
            }
            //4. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin người xử lý Ticket vào ComplainUser
                    var complainuseradd = new ComplainUser
                    {
                        UserId = staffDetail.Id,
                        UserName = staffDetail.FullName,
                        ComplainId = complainId,
                        OfficeId = office.OfficeId,
                        OfficeIdPath = office.OfficeIdPath,
                        OfficeName = office.OfficeName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        IsCare = true
                    };
                    UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                    UnitOfWork.ComplainUserRepo.Save();

                    // Lưu thông tin Ticket có người đã nhận xử lý
                    complainDetail.Status = (byte)ComplainStatus.Process;
                    complainDetail.LastUpdateDate = DateTime.Now;

                    UnitOfWork.ComplainUserRepo.Save();

                    // Gửi Notify cho nhân viên CSKH
                    NotifyHelper.CreateAndSendNotifySystemToClient(userId,
                        "Xử lý ticket ",
                        EnumNotifyType.Info,
                        $" <a href=\"" + "/Ticket/#TK" + complainDetail.Code + "\" target=\"_blank\">" + complainDetail.Code + "</a>",
                        "Yêu cầu xử lý ticket: " + complainDetail.Code, Url.Action("Index", "Ticket"));


                    // Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complainDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complainDetail.CustomerId;
                    conplainHistory.CustomerName = complainDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Process;
                    conplainHistory.UserId = staffDetail.Id;
                    conplainHistory.UserFullName = staffDetail.FullName;
                    conplainHistory.Content = " status tranfer: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Process);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }


            return Json(new { status = Result.Succeed, msg = "assigning staff successfully handled ticket !" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ticket khiếu nại
        // LẤY DANH SÁCH TICKET KHIẾU NẠI
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketComplain)]
        public async Task<JsonResult> GetAllTicketComplainList(int page, int pageSize, ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            var complainusersupport = new List<Complain>();
            var ticketModal = new List<TicketComplain>();

            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketComplainList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);

            //lấy list OrderId
            var listOrderId = ticketModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listCustomerCase = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName, x.UserId, x.UserFullName }).ToList();
            //lấy list CustomerId
            var listCustomerId = ticketModal.Select(x => x.CustomerId).ToList();
            //Lấy danh sách khách hàng
            var listCustomer = UnitOfWork.CustomerRepo.Entities.Where(x => listCustomerId.Contains(x.Id)).Select(x => new { x.Id, x.Email, x.Phone, x.WarehouseName }).ToList();

            //Lấy danh sách listTicketId
            var listTicketId = ticketModal.Where(x => x.StatusClaimForRefund == (byte)ClaimForRefundStatus.Success).Select(x => x.Id).ToList();
            var listClaimId = UnitOfWork.ClaimForRefundRepo.Entities.Where(x => listTicketId.Contains(x.TicketId ?? 0)).Select(x => new { x.TicketId, x.Code, x.Id });

            return Json(new { totalRecord, ticketModal, listCustomerCase, listCustomer, listClaimId }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ticket tôi cần xử lý
        // LẤY DANH SÁCH TICKET KHIẾU NẠI TÔI CẦN XỬ LÝ
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketForMe)]
        public async Task<JsonResult> GetAllTicketList(int page, int pageSize, ComplainSearchModal searchModal, int? customerId, int? userId)
        {
            var ticketModal = new List<TicketComplain>();
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            //chuyển sang không dấu tiếng việt
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);

            //lấy list OrderId
            var listOrderId = ticketModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listCustomerCase = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName, x.UserId, x.UserFullName }).ToList();
            //lấy list CustomerId
            var listCustomerId = ticketModal.Select(x => x.CustomerId).ToList();
            //Lấy danh sách khách hàng
            var listCustomer = UnitOfWork.CustomerRepo.Entities.Where(x => listCustomerId.Contains(x.Id)).Select(x => new { x.Id, x.Email, x.Phone, x.WarehouseName }).ToList();

            //Lấy danh sách listTicketId
            var listTicketId = ticketModal.Where(x => x.StatusClaimForRefund == (byte)ClaimForRefundStatus.Success).Select(x => x.Id).ToList();
            var listClaimId = UnitOfWork.ClaimForRefundRepo.Entities.Where(x => listTicketId.Contains(x.TicketId ?? 0)).Select(x => new { x.TicketId, x.Code, x.Id });

            return Json(new { totalRecord, ticketModal, listCustomerCase, listCustomer, listClaimId }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ticket bị trễ xử lý
        // LẤY DANH SÁCH TICKET TRỄ XỬ LÝ
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketLast)]
        public async Task<JsonResult> GetAllTicketLastList(int page, int pageSize, ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            var ticketModal = new List<TicketComplain>();
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }

            var dateDelay = DateTime.Now.AddMinutes(-60);
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketLastList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);

            //lấy list OrderId
            var listOrderId = ticketModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listCustomerCase = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName, x.UserId, x.UserFullName }).ToList();
            //lấy list CustomerId
            var listCustomerId = ticketModal.Select(x => x.CustomerId).ToList();
            //Lấy danh sách khách hàng
            var listCustomer = UnitOfWork.CustomerRepo.Entities.Where(x => listCustomerId.Contains(x.Id)).Select(x => new { x.Id, x.Email, x.Phone, x.WarehouseName }).ToList();

            return Json(new { totalRecord, ticketModal, listCustomerCase, listCustomer }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ticket có người hỗ trợ
        // LẤY DANH SÁCH TICKET HỖ TRỢ
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketSupport)]
        public async Task<JsonResult> GetAllTicketSupportList(int page, int pageSize, ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            //1. khởi tạo dữ liệu
            var ticketModal = new List<TicketComplain>();
            long totalRecord;

            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);

            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            //2. Lấy ra dữ liệu
            ticketModal = await UnitOfWork.ComplainRepo.GetAllTicketSupportList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);

            //lấy list OrderId
            var listOrderId = ticketModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listCustomerCase = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName, x.UserId, x.UserFullName }).ToList();
            //lấy list CustomerId
            var listCustomerId = ticketModal.Select(x => x.CustomerId).ToList();
            //Lấy danh sách khách hàng
            var listCustomer = UnitOfWork.CustomerRepo.Entities.Where(x => listCustomerId.Contains(x.Id)).Select(x => new { x.Id, x.Email, x.Phone, x.WarehouseName }).ToList();

            return Json(new { totalRecord, ticketModal, listCustomerCase, listCustomer }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Thêm hỗ trợ

        //Lấy về danh sách user
        [HttpPost]
        public JsonResult GetAllUser()
        {
            var listUser = UnitOfWork.DbContext.Users.Where(s => !s.IsDelete && s.Id != UserState.UserId).Select(x => new SelectListItem { Text = x.FullName, Value = x.Id + "" }).ToList();
            foreach (var item in listUser)
            {
                item.Text = Position(int.Parse(item.Value)) + " - " + item.Text;
            }
            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        //Thêm người hỗ trợ
        [HttpPost]
        public async Task<JsonResult> AddUserSupport(int userSupportId, int complainId)
        {
            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = await UnitOfWork.ComplainRepo.SingleOrDefaultAsync(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }
            if (complainDetail.Status == (byte)ComplainStatus.Success)
            {
                return Json(new { status = Result.Failed, msg = "This ticket has been completed !" }, JsonRequestBehavior.AllowGet);
            }


            //2. Lấy thông tin Detail nhân viên
            var staffDetail = await UnitOfWork.UserRepo.SingleOrDefaultAsync(x => !x.IsDelete && x.Id == userSupportId);
            if (staffDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Staffdoes not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy phòng ban theo nhân viên
            var office = await UnitOfWork.UserPositionRepo.SingleOrDefaultAsync(x => x.UserId == userSupportId && x.IsDefault);
            if (office == null)
            {
                return Json(new { status = Result.Failed, msg = "Staff are not assigned !" }, JsonRequestBehavior.AllowGet);
            }

            //3. Lấy thông tin người phụ trách Ticket
            var complainUserDetail = await UnitOfWork.ComplainUserRepo.FirstOrDefaultAsync(x => x.ComplainId == complainId && x.IsCare == true);
            if (complainUserDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "There is no official ticket holder !" }, JsonRequestBehavior.AllowGet);
            }

            //4 Lấy thông tin người hỗ trợ Ticket
            var ticketSupport = await UnitOfWork.ComplainUserRepo.FindAsNoTrackingAsync(
                                x => x.ComplainId == complainId
                                && x.IsCare == false
                                && x.UserId == staffDetail.Id
                                && x.UserRequestId == complainUserDetail.UserId
                                );
            if (ticketSupport.Count > 0)
            {
                return Json(new { status = Result.Failed, msg = "This user is already a support member of this Ticket!" }, JsonRequestBehavior.AllowGet);
            }

            //4. Cập nhật vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lưu thông tin người xử lý Ticket vào ComplainUser
                    var complainuseradd = new ComplainUser
                    {
                        ComplainId = complainDetail.Id,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OfficeId = office.OfficeId,
                        OfficeIdPath = office.OfficeIdPath,
                        OfficeName = office.OfficeName,
                        UserId = staffDetail.Id,
                        UserName = staffDetail.FullName,
                        UserRequestId = complainUserDetail.UserId,
                        UserRequestName = complainUserDetail.UserName,

                        IsCare = false
                    };
                    UnitOfWork.ComplainUserRepo.Add(complainuseradd);
                    UnitOfWork.ComplainUserRepo.Save();

                    //Gửi thông báo cho Người hỗ trợ
                    NotifyHelper.CreateAndSendNotifySystemToClient(staffDetail.Id,
                        "Hỗ trợ xử lý ticket ",
                        EnumNotifyType.Info,
                         $" <a href=\"" + "/Ticket/#TK" + complainDetail.Code + "\" target=\"_blank\">" + complainDetail.Code + "</a>",
                        "Requires ticket processing support: " + complainDetail.Code,
                        Url.Action("Index", "Ticket"));

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = Result.Succeed, msg = "Add successful supporters !" }, JsonRequestBehavior.AllowGet);

        }

        //Xóa người hỗ trợ
        [HttpPost]
        public JsonResult DeleteSupport(int userId)
        {
            var complain = new Complain();

            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var complanUser =
                        UnitOfWork.ComplainUserRepo.FirstOrDefault(s => s.UserId == userId && s.IsCare == false);
                    complain = UnitOfWork.ComplainRepo.FirstOrDefault(d => d.Id == complanUser.ComplainId);

                    UnitOfWork.ComplainUserRepo.Remove(complanUser);
                    UnitOfWork.ComplainUserRepo.Save();

                    UnitOfWork.ComplainRepo.Save();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = Result.Succeed, complain, msg = "Delete successful supporters !" }, JsonRequestBehavior.AllowGet);
        }

        //Phản hồi cho khách hàng
        private bool CheckTicketGroup(int complainId, out int groupId)
        {
            var timeOld = DateTime.Now.AddMinutes(-2);

            var chat = UnitOfWork.ComplainUserRepo.Entities.Where(x => x.ComplainId == complainId && x.UserId > 0 && x.UserId == UserState.UserId && x.CreateDate >= timeOld).OrderByDescending(x => x.Id).FirstOrDefault();
            groupId = chat?.GroupId ?? 0;
            return chat?.UserId != null;
        }

        [HttpPost]
        public async Task<JsonResult> FeedbackComplain(string content, int complainId, bool objectChat, byte? type)
        {
            //1. Kiểm tra xem Ticket có tồn tại hay không
            var complainDetail = UnitOfWork.ComplainRepo.FirstOrDefault(x => x.Id == complainId && !x.IsDelete);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket không tồn tại hoặc bị xóa !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra nhân viên này có được quyền trả lời khiếu nại này hay không
            //var complainUser =  UnitOfWork.ComplainUserRepo.FirstOrDefault(d => d.ComplainId == complainId && d.UserId == UserState.UserId);
            //if (complainUser == null)
            //{
            //    return Json(new { status = Result.Failed, msg = " Bạn không phải người dùng trong danh sách xử lý của Ticket này!" }, JsonRequestBehavior.AllowGet);
            //}

            //4. Lấy thông tin khách hàng
            var customerDetail = UnitOfWork.CustomerRepo.FirstOrDefault(x => !x.IsDelete && x.Id == complainDetail.CustomerId);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = " Khách hàng trong Ticket này does not exist hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }

            //4. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var complainUserSave = new ComplainUser();
                    type = type ?? (byte)CommentType.Text;
                    //Trao đổi giữa các nhân viên
                    if (objectChat == true)
                    {
                        //Check điều kiện có phải là người xử lý chính hay không
                        var tmpComplainUser = UnitOfWork.ComplainUserRepo.Find(m => m.ComplainId == complainId && m.UserId != null).Select(m => m.UserId).Distinct().ToList();
                        foreach (var item in tmpComplainUser)
                        {
                            if (item.Value != UserState.UserId)
                            {

                                NotifyHelper.CreateAndSendNotifySystemToClient(item.Value,
                                                        $"{UserState.FullName} Feedback complaints {complainDetail.Code}",
                                                        EnumNotifyType.Info,
                                                        $"{UserState.FullName} phản hồi khiếu nại" +
                                                        $" <a href=\"" + "/Ticket/#TK" + complainDetail.Code + "\" target=\"_blank\">" + complainDetail.Code + "</a>" +
                                                        $" Xem khiếu nại {MyCommon.ReturnCode(complainDetail.Code)}",
                                                        $"Ticket_{complainDetail.Id}",
                                                        Url.Action("Index", "Ticket"));
                            }
                        }

                        complainUserSave.UserId = UserState.UserId;
                        complainUserSave.UserName = UserState.UserName;
                        complainUserSave.Content = content;
                        complainUserSave.ComplainId = complainId;
                        complainUserSave.CreateDate = DateTime.Now;
                        complainUserSave.IsInHouse = true;
                        complainUserSave.CustomerId = customerDetail.Id;
                        complainUserSave.CustomerName = customerDetail.FullName;
                        complainUserSave.IsRead = false;
                        complainUserSave.CommentType = type;
                        complainUserSave.SystemId = complainDetail.SystemId;
                        complainUserSave.SystemName = complainDetail.SystemName;
                        int groupId;
                        var check = CheckTicketGroup(complainId, out groupId);
                        if (check)
                        {
                            complainUserSave.GroupId = groupId;
                        }
                        UnitOfWork.ComplainUserRepo.Add(complainUserSave);
                        await UnitOfWork.ComplainUserRepo.SaveAsync();

                        if (!check)
                        {
                            complainUserSave.GroupId = (int)complainUserSave.Id;
                            await UnitOfWork.ComplainUserRepo.SaveAsync();
                        }
                    }
                    //Trao đổi với khách hàng
                    else
                    {


                        complainUserSave.UserId = UserState.UserId;
                        complainUserSave.UserName = UserState.FullName;
                        complainUserSave.Content = content;
                        complainUserSave.ComplainId = complainId;
                        complainUserSave.CreateDate = DateTime.Now;
                        complainUserSave.CustomerId = customerDetail.Id;
                        complainUserSave.CustomerName = customerDetail.FullName;
                        complainUserSave.IsInHouse = false;
                        complainUserSave.IsRead = false;
                        complainUserSave.CommentType = type;
                        complainUserSave.SystemId = complainDetail.SystemId;
                        complainUserSave.SystemName = complainDetail.SystemName;

                        int groupId;
                        var check = CheckTicketGroup(complainId, out groupId);
                        if (check)
                        {
                            complainUserSave.GroupId = groupId;
                        }
                        UnitOfWork.ComplainUserRepo.Add(complainUserSave);
                        await UnitOfWork.ComplainUserRepo.SaveAsync();

                        // Gửi thông báo Notification cho khách hàng
                        var notification = new Notification()
                        {
                            SystemId = (int)complainDetail.SystemId,
                            SystemName = complainDetail.SystemName,
                            CustomerId = customerDetail.Id,
                            CustomerName = customerDetail.FullName,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            OrderId = complainId,
                            OrderType = 3,
                            IsRead = false,
                            Title = string.Format("{0} Complaint response: \"{1}\"", UserState.UserName, (content.Length > 30 ? content.Substring(0, 30) : content)),
                            Description = string.Format("{0} Complaint response: \"{1}\"", UserState.UserName, content)
                        };
                        UnitOfWork.NotificationRepo.Add(notification);
                        await UnitOfWork.ComplainUserRepo.SaveAsync();

                        if (!check)
                        {
                            complainUserSave.GroupId = (int)complainUserSave.Id;
                            await UnitOfWork.ComplainUserRepo.SaveAsync();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = Result.Succeed, msg = "Phản hồi khiếu nại thành công !" }, JsonRequestBehavior.AllowGet);
        }
        //Hoàn thành ticket
        [HttpPost]
        public async Task<JsonResult> FinishComplain(int complainId)
        {
            //1. Kiểm tra Ticket có tồn tại và đã hoàn thành chưa
            var complainDetail = UnitOfWork.ComplainRepo.FirstOrDefault(x => !x.IsDelete && x.Id == complainId);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (complainDetail.Status == (byte)ComplainStatus.Success)
            {
                return Json(new { status = Result.Failed, msg = "Ticket was completed !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    complainDetail.Status = (byte)ComplainStatus.Success;
                    complainDetail.LastUpdateDate = DateTime.Now;
                    // Gửi thông báo Notification cho khách hàng
                    var description = complainDetail.Content == null ? "" : complainDetail.Content == "" ? "" : complainDetail.Content.Substring(0, complainDetail.Content.Length - 1);
                    if(description != "")
                    {
                        if (complainDetail.Content.Length > 500)
                        {
                            description = complainDetail.Content.Substring(0, 500);
                        }
                    }
                    
                    var notification = new Notification()
                    {
                        SystemId = (int)complainDetail.SystemId,
                        SystemName = complainDetail.SystemName,
                        CustomerId = complainDetail.CustomerId,
                        CustomerName = complainDetail.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = complainId,
                        OrderType = 3,
                        IsRead = false,
                        Title = string.Format("{0} Complete complaint handling {1} cho Order {2}", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode),
                        Description = string.Format("{0} Complete complaint handling {1} cho Order {2}Content: {3} ", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode, description)
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    UnitOfWork.ComplainRepo.Save();

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complainDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complainDetail.CustomerId;
                    conplainHistory.CustomerName = complainDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Success;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Switch to status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Success);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //3. Số ticket tôi cần xử lý
            var totalTicketNeedFix = await UnitOfWork.ComplainRepo.CountTicketAssignForUser(UserState.UserId);

            return Json(new { status = Result.Succeed, msg = "Confirmation of completion of the complaint !", totalTicketNeedFix = totalTicketNeedFix }, JsonRequestBehavior.AllowGet);
        }

        //Hủy ticket
        [HttpPost]
        public async Task<JsonResult> CancelComplain(int complainId)
        {
            //1. Kiểm tra Ticket có tồn tại và đã hoàn thành chưa
            var complainDetail = UnitOfWork.ComplainRepo.FirstOrDefault(x => !x.IsDelete && x.Id == complainId);
            if (complainDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "Ticket does not exist or has been deleted !" }, JsonRequestBehavior.AllowGet);
            }
            if (complainDetail.Status == (byte)ComplainStatus.Cancel)
            {
                return Json(new { status = Result.Failed, msg = "Ticket canceled !" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lưu dữ liệu vào database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    complainDetail.IsDelete = true;
                    complainDetail.Status = (byte)ComplainStatus.Cancel;
                    complainDetail.LastUpdateDate = DateTime.Now;
                    // Gửi thông báo Notification cho khách hàng
                    var description = complainDetail.Content == null ? "" : complainDetail.Content == "" ? "" : complainDetail.Content.Substring(0, complainDetail.Content.Length - 1);
                    if (description != "")
                    {
                        if (complainDetail.Content.Length > 500)
                        {
                            description = complainDetail.Content.Substring(0, 500);
                        }
                    }
                    var notification = new Notification()
                    {
                        SystemId = (int)complainDetail.SystemId,
                        SystemName = complainDetail.SystemName,
                        CustomerId = complainDetail.CustomerId,
                        CustomerName = complainDetail.CustomerName,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        OrderId = complainId,
                        OrderType = 3,
                        IsRead = false,
                        Title = string.Format("{0} Cancellation of complaint handling {1} cho Order {2}", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode),
                        Description = string.Format("{0} Cancellation of complaint handling {1} cho Order {2} Content: {3} ", complainDetail.SystemName, complainDetail.Code, complainDetail.OrderCode, description)
                    };
                    UnitOfWork.NotificationRepo.Add(notification);
                    UnitOfWork.ComplainRepo.Save();

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complainDetail.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complainDetail.CustomerId;
                    conplainHistory.CustomerName = complainDetail.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.Cancel;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Switch to status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.Cancel);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //3. Số ticket tôi cần xử lý
            var totalTicketNeedFix = await UnitOfWork.ComplainRepo.CountTicketAssignForUser(UserState.UserId);

            return Json(new { status = Result.Succeed, msg = "Confirm cancel the complaints !", totalTicketNeedFix = totalTicketNeedFix }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Lấy thông tin Detail ticket
        [HttpPost]
        public async Task<JsonResult> GetTicketDetail(int ticketId)
        {
            var customer = new Customer();
            var complainuserlist = new List<ComplainDetail>();
            var complainuserinternallist = new List<ComplainDetail>();
            var list = new List<ComplainDetail>();
            var usersupportlist = new List<User>();
            var usersupport = new User();
            var ticketModal = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(p => p.Id == ticketId);
            //Đánh dấu là đã đọc nội dung comment của khách hàng
            var listChatCustomer = await UnitOfWork.ComplainUserRepo.FindAsync(x =>
                                                                                x.ComplainId == ticketId
                                                                                && (bool)!x.IsRead
                                                                                && x.CustomerId != null
                                                                                && x.UserId == null
                                                                                );

            foreach (var item in listChatCustomer)
            {
                item.IsRead = true;

            }
            await UnitOfWork.ComplainUserRepo.SaveAsync();
            //Danh sách trao đổi với khách hàng
            var complainuser = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == ticketId
               && (p.IsInHouse == false || p.IsInHouse == null) && p.Content != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );


            //Lọc Staff handling
            var complainusersupport = (from d in UnitOfWork.DbContext.ComplainUsers
                                       where (d.ComplainId == ticketId && d.IsCare == false)
                                       orderby d.CreateDate
                                       select new { d.UserId, d.UserName, d.OfficeId, d.OfficeIdPath, d.OfficeName }).Distinct().ToList();
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
                    IsCare = item.IsCare,
                    OfficeId = item.OfficeId,
                    OfficeIdPath = item.OfficeIdPath,
                    OfficeName = item.OfficeName,
                    GroupId = item.GroupId,
                    CommentType = item.CommentType
                });
            }

            // Danh sách trao đổi nội bộ
            var complainuserinternal = await UnitOfWork.ComplainUserRepo.FindAsync(
               p => p.ComplainId == ticketId
               && p.IsInHouse == true
               && p.UserId != null,
               null,
                x => x.OrderBy(y => y.CreateDate)
               );

            //Nội dung danh sách trao đổi nội bộ
            foreach (var item in complainuserinternal)
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

                complainuserinternallist.Add(new ComplainDetail
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
                    IsCare = item.IsCare,
                    OfficeId = item.OfficeId,
                    OfficeIdPath = item.OfficeIdPath,
                    OfficeName = item.OfficeName,
                    GroupId = item.GroupId,
                    CommentType = item.CommentType
                });
            }

            //Lọc người xử lý(hỗ trợ)
            foreach (var item in complainusersupport)
            {
                var userPosition = "";
                if (item.UserId != null)
                {
                    var posit = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsync(d => d.UserId == item.UserId && d.IsDefault);
                    userPosition = posit.TitleName;
                }
                list.Add(new ComplainDetail
                {
                    Id = 0,
                    UserId = item.UserId,
                    UserName = item.UserName,
                    UserPosition = userPosition,
                    ComplainId = 0,
                    OfficeId = item.OfficeId,
                    OfficeIdPath = item.OfficeIdPath,
                    OfficeName = item.OfficeName
                });
            }

            //Lấy ra nhân viên chịu trách nhiệm chính
            var complainusermain = UnitOfWork.ComplainUserRepo.FirstOrDefault(s => s.ComplainId == ticketId && s.IsCare == true);
            var hascustomer = new Customer();
            if (ticketModal != null)
            {
                hascustomer = UnitOfWork.CustomerRepo.FirstOrDefault(p => p.Id == ticketModal.CustomerId);
            }
            //Lay danh sach san pham
            var listComplainOrder = UnitOfWork.ComplainOrderRepo.GetListOrderDetail(ticketId);
            customer = hascustomer;
            return Json(new { ticketModal, customer, complainuserlist, complainusermain, list, complainuserinternallist, listComplainOrder }, JsonRequestBehavior.AllowGet);
        }

        #region [Cập nhật - Xóa nội dung trao đổi trong Detail Ticket khiếu nại]
        // Hàm cập nhật nội dung trao đổi về Ticket khiếu nại
        [HttpPost]
        //[CheckPermission(EnumAction.Add, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund)]
        public async Task<ActionResult> UpdateContent(int complainDetailId, string complainContent)
        {

            //Thay đổi nội dung trao đổi
            var complainUser = await UnitOfWork.ComplainUserRepo.FirstOrDefaultAsync(s => s.Id == complainDetailId);
            if (complainUser != null)
            {
                complainUser.Content = complainContent;
            }
            else
            {
                return Json(new { status = Result.Failed, msg = "No exist detail ticket this complaint !" },
                        JsonRequestBehavior.AllowGet);
            }

            complainUser.UpdateDate = DateTime.Now;
            UnitOfWork.ComplainUserRepo.Save();

            return Json(new { status = Result.Succeed, msg = "Update the content of the successful response!" }, JsonRequestBehavior.AllowGet);
        }

        // Hàm xóa nội dung trao đổi về Ticket khiếu nại
        [HttpPost]
        //[CheckPermission(EnumAction.Add, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund)]
        public async Task<ActionResult> DeleteContent(int complainDetailId)
        {

            //Thay đổi nội dung trao đổi
            var complainUser = await UnitOfWork.ComplainUserRepo.FirstOrDefaultAsync(s => s.Id == complainDetailId);



            if (complainUser == null)
            {
                return Json(new { status = Result.Failed, msg = "No exist detail ticket this complaint !" },
                         JsonRequestBehavior.AllowGet);
            }
            UnitOfWork.ComplainUserRepo.Remove(complainUser);
            UnitOfWork.ComplainUserRepo.Save();

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteClaimForRefundIsDetail }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #endregion

        #region [Lấy danh sách nhân viên trong phòng mình]

        [HttpPost]
        public async Task<JsonResult> GetUserComplain()
        {
            var listUser = await UnitOfWork.UserRepo.GetUserToOffice(UserState.UserId, 3, UserState.OfficeIdPath, UserState.OfficeId.Value);

            return Json(listUser, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region [Thông tin Refund khiếu nại]

        [HttpPost]
        public JsonResult checkCustomerCare(int orderId)
        {
            var result = 0;
            var order = UnitOfWork.OrderRepo.FirstOrDefault(s => s.Id == orderId && s.IsDelete == false);
            if (order == null)
            {
                return Json(new { status = Result.Failed, msg = "Order does not exist or has been canceled!" }, JsonRequestBehavior.AllowGet);
            }
            if(order.CustomerCareUserId != null)
            {
                result = 1;
            }
            return Json(new { status = Result.Succeed, value = result, name = order.CustomerCareFullName, userId = order.CustomerCareUserId }, JsonRequestBehavior.AllowGet);
        }
        //Kiem tra de gan khieu nai cho nhan vien
        [HttpPost]
        public JsonResult CheckRefundTicket(int ticketId)
        {
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == ticketId);
            if (complain == null)
            {
                return Json(new { status = Result.Failed, msg = "Complaints do not exist!" }, JsonRequestBehavior.AllowGet);
            }
            var orderClaim = UnitOfWork.ClaimForRefundRepo.Find(s => s.OrderId == complain.OrderId);
            return Json(new { status = Result.Succeed, value = orderClaim.Count() }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> RefundTicket(int ticketId)
        {
            var claimForRefundVM = new ClaimForRefundViewModel();
            var listClaimForRefund = new List<ClaimForRefundDetail>();
            claimForRefundVM.ClaimForRefund = new ClaimForRefund();
            claimForRefundVM.LstClaimForRefundDetails = listClaimForRefund;

            var orderDetail = new Order();
            var customer = new Customer();


            //1. Lấy Detail thông tin Ticket
            var complain = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(s => s.Id == ticketId);
            if (complain == null)
            {
                return Json(new { status = Result.Failed, msg = "Complaints do not exist!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //Mapper.Map(complain, claimForRefund.ClaimForRefund);
                claimForRefundVM.ClaimForRefund.TicketId = ticketId;
                claimForRefundVM.ClaimForRefund.TicketCode = complain.Code;

                //2. Lấy thông tin Detail khách hàng
                customer = await UnitOfWork.CustomerRepo.FirstOrDefaultAsync(s => s.Id == complain.CustomerId);
                if (customer == null)
                {
                    return Json(new { status = Result.Failed, msg = "Customer does not exist!" }, JsonRequestBehavior.AllowGet); ;
                }
                else
                {
                    claimForRefundVM.ClaimForRefund.CustomerId = customer.Id;
                    claimForRefundVM.ClaimForRefund.CustomerCode = customer.Code;
                    claimForRefundVM.ClaimForRefund.CustomerEmail = customer.Email;
                    claimForRefundVM.ClaimForRefund.CustomerFullName = customer.FullName;
                    claimForRefundVM.ClaimForRefund.CustomerAddress = customer.Address;
                    claimForRefundVM.ClaimForRefund.CustomerOfficeId = customer.OfficeId;
                    claimForRefundVM.ClaimForRefund.CustomerOfficeName = customer.OfficeName;
                    claimForRefundVM.ClaimForRefund.CustomerOfficePath = customer.OfficeIdPath;
                }

                //3. Lấy thông tin Order
                orderDetail = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Id == complain.OrderId);
                if (orderDetail == null)
                {
                }
                else
                {
                    claimForRefundVM.ClaimForRefund.OrderId = orderDetail.Id;
                    claimForRefundVM.ClaimForRefund.OrderCode = orderDetail.Code;
                    claimForRefundVM.ClaimForRefund.OrderType = orderDetail.Type;
                    claimForRefundVM.ClaimForRefund.OrderUserId = orderDetail.UserId;
                    claimForRefundVM.ClaimForRefund.OrderUserFullName = orderDetail.UserFullName;
                    claimForRefundVM.ClaimForRefund.OrderUserOfficeId = orderDetail.OfficeId;
                    claimForRefundVM.ClaimForRefund.OrderUserOfficeName = orderDetail.OfficeName;
                    claimForRefundVM.ClaimForRefund.OrderUserOfficePath = orderDetail.OfficeIdPath;

                    //4. Lấy thông tin các sản phẩm trong OrderDetail
                    if (orderDetail.Type == (int)OrderType.Order)
                    {
                        var list = await UnitOfWork.OrderDetailRepo.FindAsNoTrackingAsync(s => s.OrderId == complain.OrderId && s.IsDelete == false);
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                claimForRefundVM.LstClaimForRefundDetails.Add(new ClaimForRefundDetail
                                {
                                    Id = 0,
                                    ProductId = item.Id,
                                    Name = item.Name,
                                    Link = item.Link,
                                    Image = item.Image,
                                    Quantity = item.Quantity,
                                    Size = item.Size,
                                    Color = item.Color,
                                    Price = item.Price,
                                    TotalPrice = item.TotalPrice,
                                    TotalExchange = item.TotalExchange,
                                    OrderId = item.OrderId,
                                    OrderType = (int)OrderType.Order,
                                    QuantityFailed = 0,
                                    Note = item.Note,
                                    ClaimId = 0,
                                    TotalQuantityFailed = 0,
                                    ClaimCode = ""
                                });
                            }
                        }
                    }
                    if (orderDetail.Type == (int)OrderType.Deposit)
                    {
                        var list = await UnitOfWork.DepositDetailRepo.FindAsync(s => s.DepositId == complain.OrderId && s.IsDelete == false);
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                claimForRefundVM.LstClaimForRefundDetails.Add(new ClaimForRefundDetail
                                {
                                    Id = 0,
                                    ProductId = item.Id,
                                    Name = item.ProductName,
                                    Link = "",
                                    Image = item.Image,
                                    Quantity = item.Quantity,
                                    Size = item.Size,
                                    Color = "",
                                    Price = 0,
                                    TotalPrice = 0,
                                    TotalExchange = 0,
                                    OrderId = item.DepositId,
                                    OrderType = (int)OrderType.Deposit,
                                    QuantityFailed = 0,
                                    Note = item.Note,
                                    ClaimId = 0,
                                    TotalQuantityFailed = 0,
                                    ClaimCode = ""
                                });
                            }
                        }
                    }
                    if (orderDetail.Type == (int)OrderType.Source)
                    {
                        var list = await UnitOfWork.SourceDetailRepo.FindAsync(s => s.SourceId == complain.OrderId && s.IsDelete == false);
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                claimForRefundVM.LstClaimForRefundDetails.Add(new ClaimForRefundDetail
                                {
                                    Id = 0,
                                    ProductId = item.Id,
                                    Name = item.Name,
                                    Link = item.Link,
                                    Image = item.ImagePath1,
                                    Quantity = item.Quantity,
                                    Size = item.Size,
                                    Color = item.Color,
                                    Price = item.Price,
                                    TotalPrice = item.TotalPrice,
                                    TotalExchange = item.TotalExchange,
                                    OrderId = int.Parse(item.SourceId.ToString()),
                                    OrderType = (int)OrderType.Order,
                                    QuantityFailed = 0,
                                    Note = item.Note,
                                    TotalQuantityFailed = 0,
                                    ClaimId = 0,
                                    ClaimCode = ""
                                });
                            }
                        }
                    }
                }
            }

            //5. Lấy thông tin Detail của Ticket
            var complainuser = await UnitOfWork.ComplainUserRepo.FirstOrDefaultAsync(s => s.ComplainId == ticketId && s.IsCare == true);
            if (complainuser == null)
            {
                ;
            }
            else
            {
                //6. Lấy thông tin Detail người chịu trách nhiệm xử lý Ticket
                var userDetail = await UnitOfWork.UserRepo.FirstOrDefaultAsync(s => s.Id == complainuser.UserId);
                if (userDetail == null)
                {
                    ;
                }
                else
                {
                    claimForRefundVM.ClaimForRefund.SupportId = userDetail.Id;
                    claimForRefundVM.ClaimForRefund.SupportEmail = userDetail.Email;
                    claimForRefundVM.ClaimForRefund.SupportFullName = userDetail.FullName;
                }
            }
            //7. Lấy thông tin dịch vụ Order
            var orderService = UnitOfWork.OrderServiceRepo.Find(s => !s.IsDelete && s.OrderId == complain.OrderId && s.Checked);

            //8. Lấy thông tin của cấp Level Vip
            var levelVip = new CustomerLevel();
            if (orderDetail != null)
            {
                levelVip = UnitOfWork.CustomerLevelRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderDetail.LevelId);
            }
            claimForRefundVM.LstClaimForRefundDetails = claimForRefundVM.LstClaimForRefundDetails.OrderBy(s => s.ProductId).ToList();
            return Json(new
            {
                claimForRefundVM,
                complain,
                orderDetail,
                customer,
                orderService,
                levelVip
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Xử lý yêu cầu Refund - ClaimForRefund



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
                    x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.TicketCode == searchModal.Keyword || x.CustomerFullName.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword) || x.UserPhone.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword)
                    || x.UserEmail.Contains(searchModal.Keyword) || x.UserName.Contains(searchModal.Keyword))
                         //&& (UserState.Type > 0 || x.UserId == UserState.UserId)
                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId || x.SupportId == searchModal.UserId)
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
                   x => (x.Code.Contains(searchModal.Keyword) || x.OrderCode == searchModal.Keyword
                    || x.TicketCode == searchModal.Keyword || x.CustomerFullName.Contains(searchModal.Keyword)
                    || x.CustomerEmail.Contains(searchModal.Keyword) || x.UserPhone.Contains(searchModal.Keyword)
                    || x.CustomerPhone.Contains(searchModal.Keyword)
                    || x.UserEmail.Contains(searchModal.Keyword) || x.UserName.Contains(searchModal.Keyword))
                         //&& (UserState.Type > 0 || x.UserId == UserState.UserId)
                         //&& !x.IsDelete
                         && (searchModal.CustomerId == -1 || x.CustomerId == searchModal.CustomerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (searchModal.UserId == -1 || x.UserId == searchModal.UserId || x.SupportId == searchModal.UserId),
                    //&& (UserState.Type != 0 || x.UserId == UserState.UserId),
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );
            }
            //lấy list OrderId
            var listOrderId = claimForRefundModal.Select(x => x.OrderId).ToList();
            //Lấy danh sách order
            var listOrder = UnitOfWork.OrderRepo.Entities.Where(x => listOrderId.Contains(x.Id)).Select(x => new { x.Id, x.CustomerCareUserId, x.CustomerCareFullName}).ToList();

            //lấy listComplainId
            var listComplainId = claimForRefundModal.Select(x => x.TicketId).ToList();
            //Lấy danh sách Complain
            var listComplain = UnitOfWork.ComplainUserRepo.Entities.Where(x => listComplainId.Contains((int)x.ComplainId) && x.IsCare == true).Select(x => new { x.Id, x.ComplainId, x.UserId, x.UserName }).ToList();
            return Json(new { totalRecord, claimForRefundModal, listOrder, listComplain }, JsonRequestBehavior.AllowGet);
        }

        //Lấy thông tin Detail yêu cầu khiếu nại
        [HttpPost]
        //[CheckPermission(EnumAction.Approvel, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund, EnumPage.OrderClaimForRefund)]
        public async Task<JsonResult> GetClaimForRefundDetail(int claimForRefundId)
        {

            var claimForRefundViewModel = new ClaimForRefundViewModel();

            //1. Kiểm tra phiếu xử lý yêu cầu Refund có tồn tại hay không
            var claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == claimForRefundId);
            if (claimForRefundModal == null)
            {
                return Json(new { status = Result.Failed, msg = "Compaint ticket has been canceled or does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            //2. Lấy thông tin Detail các sản phẩm bị khiếu nại
            var claimForRefundDetail = UnitOfWork.ClaimForRefundDetailRepo.Find(x => x.ClaimId == claimForRefundModal.Id).OrderBy(s => s.ProductId);

            //3. Lấy về thông tin khiếu nại
            var ticket = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(s => !s.IsDelete && s.Id == claimForRefundModal.TicketId);
            if (ticket == null)
            {
                return Json(new { status = Result.Failed, msg = "Compaint ticket has been canceled or does not exist!" }, JsonRequestBehavior.AllowGet);
            }


            //4. Map lại dữ liệu
            claimForRefundViewModel.ClaimForRefund = claimForRefundModal;
            claimForRefundViewModel.LstClaimForRefundDetails = claimForRefundDetail.OrderBy(s => s.ProductId).ToList();

            //5. Lấy về thông tin Order
            var orderDetail = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Id == claimForRefundModal.OrderId);

            //6. Lấy thông tin dịch vụ Order
            var orderService = UnitOfWork.OrderServiceRepo.Find(s => !s.IsDelete && s.OrderId == ticket.OrderId && s.Checked).ToList();

            //7. Lấy thông tin của cấp Level Vip
            var levelVip = new CustomerLevel();
            if (orderDetail != null)
            {
                levelVip = UnitOfWork.CustomerLevelRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderDetail.LevelId);
            }


            return Json(new { status = Result.Succeed, claimForRefundViewModel, ticket, orderDetail, orderService, levelVip }, JsonRequestBehavior.AllowGet);
        }

        // Tạo yêu cầu Refund
        [HttpPost]
        [CheckPermission(EnumAction.Add, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund)]
        public async Task<ActionResult> CreateNewClaimForRefund(ClaimForRefund claimForRefund, List<ClaimForRefundDetail> listClaimForRefundDetail, Complain complainFund)
        {
            var claimForRefundSave = new ClaimForRefund();

            //ModelState.Remove("Id");
            //if (!ModelState.IsValid)
            //{
            //    return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid },
            //        JsonRequestBehavior.AllowGet);
            //}
            //1. Kiểm tra số lượng hàng lỗi
            if (claimForRefund.MoneyRefund == 0 || claimForRefund.MoneyRefund == null)
            {
                return Json(new { status = Result.Failed, msg = "You will not be able to create a request unless you provide the number of errors !" },
                    JsonRequestBehavior.AllowGet);
            }

            Mapper.Map(claimForRefund, claimForRefundSave);

            //2. Kiểm tra xem đã tồn tại phiếu yêu cầu Refund nào đã tồn tại cho Ticket này trên system hay chưa
            var claimForRefundDetail =
                await UnitOfWork.ClaimForRefundRepo.FindAsNoTrackingAsync(x => (x.TicketId == claimForRefund.TicketId && x.IsDelete != true));

            if (claimForRefundDetail.Count > 0)
            {
                return Json(new { status = Result.Failed, msg = "Existing refund request exists for this Complaint Ticket !" },
                    JsonRequestBehavior.AllowGet);
            }

            //3. lấy thông tin Ticket
            var ticketDetail =
                UnitOfWork.ComplainRepo.FirstOrDefaultAsNoTracking(x => !x.IsDelete && x.Id == claimForRefund.TicketId);
            if (ticketDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.TicketIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                claimForRefundSave.TicketId = (int)ticketDetail.Id;
                claimForRefundSave.TicketCode = ticketDetail.Code;
                claimForRefundSave.TicketCreated = ticketDetail.CreateDate;
            }

            if (ticketDetail.Status == (byte)ComplainStatus.OrderWait)
            {
                return Json(new { status = Result.Failed, msg = "Has requested a refund exists for complaints!" },
                    JsonRequestBehavior.AllowGet);
            }

            //4. Lấy thông tin Detail đối tượng nhập là khách hàng
            var customerDetail =
                await
                    UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == claimForRefund.CustomerId);
            if (customerDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.CustomerIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                claimForRefundSave.CustomerId = customerDetail.Id;
                claimForRefundSave.CustomerCode = customerDetail.Code;
                claimForRefundSave.CustomerEmail = customerDetail.Email;
                claimForRefundSave.CustomerPhone = customerDetail.Phone;
                claimForRefundSave.CustomerFullName = customerDetail.FullName;
                claimForRefundSave.CustomerAddress = customerDetail.Address;
                claimForRefundSave.CustomerOfficeId = customerDetail.OfficeId;
                claimForRefundSave.CustomerOfficeName = customerDetail.OfficeName;
                claimForRefundSave.CustomerOfficePath = customerDetail.OfficeIdPath;
            }

            //5. Lấy thông tin order detail
            var orderDetail =
                    await
                        UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == claimForRefund.OrderId);
            if (orderDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.OrderIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                claimForRefundSave.OrderId = orderDetail.Id;
                claimForRefundSave.OrderCode = orderDetail.Code;
                claimForRefundSave.OrderType = orderDetail.Type;

                claimForRefundSave.OrderUserId = orderDetail.UserId;
                claimForRefundSave.OrderUserFullName = orderDetail.UserFullName;

                claimForRefundSave.OrderUserOfficeId = orderDetail.OfficeId;
                claimForRefundSave.OrderUserOfficeName = orderDetail.OfficeName;
                claimForRefundSave.OrderUserOfficePath = orderDetail.OfficeIdPath;
            }

            //6. Lấy thông tin Detail nhân viên CSKH
            var supportDetail = await UnitOfWork.UserRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == claimForRefund.SupportId);
            if (supportDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.UserIsNotValid },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                claimForRefundSave.SupportId = supportDetail.Id;
                claimForRefundSave.SupportEmail = supportDetail.Email;
                claimForRefundSave.SupportFullName = supportDetail.FullName;
            }

            //7. Lấy thông tin
            claimForRefundSave.Code = string.Empty;      // Khởi tạo code FundBill

            var claimForRefundOfDay = UnitOfWork.ClaimForRefundRepo.Count(x =>
                x.Created.Year == DateTime.Now.Year && x.Created.Month == DateTime.Now.Month &&
                x.Created.Day == DateTime.Now.Day);
            var code = $"{claimForRefundOfDay}{DateTime.Now:ddMMyy}";
            claimForRefundSave.Code = code;

            claimForRefundSave.ExchangeRate = ExchangeRate();
            //claimForRefundSave.MoneyRefund = claimForRefund.MoneyRefund;

            //8. Nhận về các khoản Refund
            claimForRefundSave.RealTotalRefund = claimForRefund.RealTotalRefund;
            claimForRefundSave.MoneyOrderRefund = claimForRefund.MoneyOrderRefund;
            claimForRefundSave.MoneyRefund = claimForRefund.MoneyRefund;
            claimForRefundSave.CurrencyDiscount = claimForRefund.CurrencyDiscount;
            claimForRefundSave.SupporterMoneyRequest = claimForRefund.SupporterMoneyRequest;

            //9. Lưu thông tin người tạo yêu cầu
            var userDetail = await UnitOfWork.UserRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == UserState.UserId);
            if (userDetail == null)
            {
                return Json(new { status = Result.Failed, msg = "No user or deleted user exists!" },
                    JsonRequestBehavior.AllowGet);
            }
            claimForRefundSave.UserId = userDetail.Id;
            claimForRefundSave.UserName = userDetail.UserName;
            claimForRefundSave.UserEmail = userDetail.Email;
            claimForRefundSave.UserPhone = userDetail.Phone;

            //10. Lưu thông tin phòng ban người tạo yêu cầu
            var userOffice = await UnitOfWork.UserPositionRepo.FirstOrDefaultAsNoTrackingAsync(x => x.UserId == UserState.UserId && x.IsDefault);
            if (userOffice == null)
            {
                return Json(new { status = Result.Failed, msg = "User has not been assigned!" },
                    JsonRequestBehavior.AllowGet);
            }
            claimForRefundSave.OfficeId = userOffice.OfficeId;
            claimForRefundSave.OfficeName = userOffice.OfficeName;
            claimForRefundSave.OfficeIdPath = userOffice.OfficeIdPath;

            //11. Lấy về tên Type khiếu nại chốt
            var typeComplain = await UnitOfWork.ComplainTypeRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == complainFund.TypeServiceClose && !x.IsDelete);
            if (typeComplain == null)
            {
                return Json(new { status = Result.Failed, msg = "Type Complaints do not exist!" },
                   JsonRequestBehavior.AllowGet);
            }
            //11. Lưu thông tin xuống database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //9.1 Thay đổi trạng thái Ticket sang Chờ đặt hàng xử lý
                    var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);
                    complain.Status = (byte)ComplainStatus.OrderWait;
                    complain.LastUpdateDate = DateTime.Now;
                    complain.TypeServiceClose = typeComplain.Id;
                    complain.TypeServiceCloseName = typeComplain.Name;
                    UnitOfWork.ComplainRepo.Save();

                    //9.2 Lưu thông tin phiếu yêu cầu Refund
                    claimForRefundSave.Status = (byte)ClaimForRefundStatus.OrderWait;
                    UnitOfWork.ClaimForRefundRepo.Add(claimForRefundSave);
                    UnitOfWork.ClaimForRefundRepo.Save();
                    var claimRefund = UnitOfWork.ClaimForRefundRepo.FirstOrDefault(s => s.Code == code);

                    //9.3 Lưu thông tin sản phẩm cần Refund
                    foreach (var item in listClaimForRefundDetail)
                    {
                        //if(item.)
                        item.ClaimId = claimRefund.Id;
                        item.ClaimCode = code;

                        UnitOfWork.ClaimForRefundDetailRepo.Add(item);
                        UnitOfWork.ClaimForRefundDetailRepo.Save();
                    }
                    //9.4 Gửi Notify cho nhân viên đặt hàng
                    var claim = UnitOfWork.ClaimForRefundRepo.FirstOrDefault(s => s.TicketId == claimForRefundSave.TicketId);
                    if (claimForRefundSave.OrderUserId > 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient((int)claimForRefundSave.OrderUserId,
                            "Xử lý yêu cầu Refund ",
                            EnumNotifyType.Info,
                            $" <a href=\"" + "/Ticket/#CFRF" + claim.Code + "\" target=\"_blank\">" + claim.Code + "</a>",
                            "Request to process a refund request for a request: " + claim.Code,
                             Url.Action("Index", "Ticket"));
                    }

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complain.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complain.CustomerId;
                    conplainHistory.CustomerName = complain.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.OrderWait;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Tranfer stataus: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.OrderWait);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            //6. Số phiếu Refund khiếu nại
            var totalClaimForRefund = UnitOfWork.ClaimForRefundRepo.Find(s => (UserState.Type > 0 || s.UserId == UserState.UserId)).Count();

            return Json(new { status = Result.Succeed, msg = ConstantMessage.CreateNewClaimForRefundIsSuccess, totalClaimForRefund = totalClaimForRefund }, JsonRequestBehavior.AllowGet);
        }

        // Lấy thông tin yêu cầu Refund
        [HttpPost]
        public async Task<JsonResult> GetClaimForRefund(int code)
        {

            //1. Kiểm tra phiếu xử lý yêu cầu Refund có tồn tại hay không
            var claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == code);
            if (claimForRefundModal == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = Result.Succeed, claimForRefundModal }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetClaimForRefundCode(string code)
        {

            //1. Kiểm tra phiếu xử lý yêu cầu Refund có tồn tại hay không
            var claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Code == code);
            if (claimForRefundModal == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { status = Result.Succeed, claimForRefundModal }, JsonRequestBehavior.AllowGet);
        }

        // Edit thông tin xử lý yêu cầu Refund
        [HttpPost]
        [CheckPermission(EnumAction.View, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund, EnumPage.OrderClaimForRefund)]
        public async Task<JsonResult> ClaimForRefundUpdate(int claimForRefundId)
        {
            var claimForRefundViewModel = new ClaimForRefundViewModel();
            var orderDetail = new Order();
            var claimForRefundDetail = new List<ClaimForRefundDetail>();
            var ticket = new Complain();
            var orderService = new List<OrderService>();
            var levelVip = new CustomerLevel();

            //1. Kiểm tra phiếu xử lý yêu cầu Refund có tồn tại hay không
            var claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsNoTrackingAsync(x => !x.IsDelete && x.Id == claimForRefundId);
            if (claimForRefundModal == null)
            {
                return Json(new { status = Result.Failed, msg = "Phiếu yêu cầu does not exist hoặc đã bị xóa!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //2. Lấy thông tin khiếu nại
                ticket = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(s => !s.IsDelete && s.Id == claimForRefundModal.TicketId);
                if (ticket == null)
                {
                    return Json(new { status = Result.Failed, msg = "Compain does not exist or has been deleted!" }, JsonRequestBehavior.AllowGet);
                }

                //3. Lấy về thông tin Order
                orderDetail = await UnitOfWork.OrderRepo.FirstOrDefaultAsync(s => s.Id == claimForRefundModal.OrderId);

                //4. Lấy thông tin Detail các sản phẩm bị khiếu nại
                claimForRefundDetail = await UnitOfWork.ClaimForRefundDetailRepo.FindAsync(x => x.ClaimId == claimForRefundModal.Id);

                //5. Map lại dữ liệu
                claimForRefundViewModel.ClaimForRefund = claimForRefundModal;
                claimForRefundViewModel.LstClaimForRefundDetails = claimForRefundDetail.OrderBy(s => s.ProductId).ToList();
                //6. Lấy thông tin dịch vụ Order
                orderService = UnitOfWork.OrderServiceRepo.Find(s => !s.IsDelete && s.OrderId == ticket.OrderId && s.Checked).ToList();
                //7. Lấy thông tin của cấp Level Vip


                if (orderDetail != null)
                {
                    levelVip = UnitOfWork.CustomerLevelRepo.FirstOrDefault(x => !x.IsDelete && x.Id == orderDetail.LevelId);
                }
            }
            return Json(new { status = Result.Succeed, claimForRefundViewModel, orderDetail, ticket, orderService, levelVip }, JsonRequestBehavior.AllowGet);
        }

        //----------------------------Đặt hàng tham gia xử lý Refund-------------------------------

        //Cập nhật thông tin xử lý yêu cầu Refund
        [HttpPost]
        public async Task<ActionResult> ClaimForRefundInfoUpdate(ClaimForRefund claimForRefund, List<ClaimForRefundDetail> listClaimForRefundDetail, Complain complainFund)
        {
            //ClaimForRefundStatus status = new ClaimForRefundStatus();
            var isOrder = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Order);
            var isCustomerCare = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.CustomerCare);

            //1. Kiểm tra phiếu Refund khiếu nại có tồn tại
            var claimForRefundSave = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(s => s.Code == claimForRefund.Code && !s.IsDelete);
            if (claimForRefundSave == null)
            {
                return Json(new { status = Result.Failed, msg = "There is no refund claim for this complaint ticket !" },
                   JsonRequestBehavior.AllowGet);
            }
            //2. Kiểm tra phiếu yêu cầu đã chuyển trạng thái chưa
            if (isOrder && (claimForRefundSave.Status > (byte)ClaimForRefundStatus.CustomerCareWait) && claimForRefundSave.RealTotalRefund == null)
            {
                return Json(new { status = Result.Failed, msg = "Could not update information because the most wanted to be changed to status " + EnumHelper.GetEnumDescription<ClaimForRefundStatus>(claimForRefundSave.Status) },
                   JsonRequestBehavior.AllowGet);
            }
            if (isCustomerCare && (claimForRefundSave.Status == (byte)ClaimForRefundStatus.CustomerCareWait && claimForRefundSave.ApproverId != null))
            {
                return Json(new { status = Result.Failed, msg = "Could not update information because the most wanted to be changed to status " + EnumHelper.GetEnumDescription<ClaimForRefundStatus>(claimForRefundSave.Status) },
                   JsonRequestBehavior.AllowGet);
            }


            //3. Kiểm tra Ticket có tồn tại
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);

            if (complain == null)
            {

                return Json(new { status = Result.Failed, msg = "This ticket does not exist !" },
                        JsonRequestBehavior.AllowGet);
            }
            //11. Lấy về tên Type khiếu nại chốt
            var typeComplain = await UnitOfWork.ComplainTypeRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == complainFund.TypeServiceClose && !x.IsDelete);
            if (typeComplain == null)
            {
                return Json(new { status = Result.Failed, msg = "You have not selected the type of claim!" },
                   JsonRequestBehavior.AllowGet);
            }

            //4. Lưu thông tin xuống database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    claimForRefund.Id = claimForRefundSave.Id;
                    Mapper.Map(claimForRefund, claimForRefundSave);
                    claimForRefundSave.LastUpdated = DateTime.Now;
                    //9.1 Thay đổi Type khiếu nại chốt
                    complain.LastUpdateDate = DateTime.Now;
                    complain.TypeServiceClose = typeComplain.Id;
                    complain.TypeServiceCloseName = typeComplain.Name;
                    UnitOfWork.ComplainRepo.Save();
                    //9.2 Cập nhật thông tin phiếu yêu cầu Refund
                    UnitOfWork.ClaimForRefundRepo.Save();

                    //9.3 Cập nhật thông tin sản phẩm cần Refund
                    if (isCustomerCare)
                    {
                        foreach (var item in listClaimForRefundDetail)
                        {
                            var claimForRefundDetail = await UnitOfWork.ClaimForRefundDetailRepo.FirstOrDefaultAsync(s => s.Id == item.Id);
                            if (claimForRefundDetail == null)
                            {
                                return Json(new { status = Result.Failed, msg = "Not exist detail refund for this complaint  this ticket !" },
                                    JsonRequestBehavior.AllowGet);
                            }
                            Mapper.Map(item, claimForRefundDetail);
                            UnitOfWork.ClaimForRefundDetailRepo.Save();
                        }
                    }


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = Result.Succeed, msg = ConstantMessage.UpdateClaimForRefundIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        //Cập nhật note Phiếu yêu cầu Refund
        [HttpPost]
        public async Task<ActionResult> ClaimForRefundNoteUpdate(ClaimForRefund claimForRefund)
        {

            //1. Kiểm tra phiếu Refund khiếu nại có tồn tại
            var claimForRefundSave = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(s => s.Code == claimForRefund.Code && !s.IsDelete);
            if (claimForRefundSave == null)
            {
                return Json(new { status = Result.Failed, msg = "Not exist refund claim for this claim ticket!" },
                   JsonRequestBehavior.AllowGet);
            }

            //3. Kiểm tra Ticket có tồn tại
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);

            if (complain == null)
            {

                return Json(new { status = Result.Failed, msg = "This ticket does not exist !" },
                        JsonRequestBehavior.AllowGet);
            }
            claimForRefundSave.NoteAccountanter = claimForRefund.NoteAccountanter;
            claimForRefundSave.NoteOrderer = claimForRefund.NoteOrderer;
            claimForRefundSave.NoteSupporter = claimForRefund.NoteSupporter;
            claimForRefundSave.LastUpdated = DateTime.Now;
            UnitOfWork.ClaimForRefundRepo.Save();

            return Json(new { status = Result.Succeed, msg = ConstantMessage.UpdateClaimForRefundIsSuccess }, JsonRequestBehavior.AllowGet);
        }

        //Chuyển sang cho CSKH xử lý tiếp sau đặt hàng hoặc sau phê duyệt
        [HttpPost]
        public async Task<ActionResult> ClaimForRefundForwardCareCustomer(ClaimForRefund claimForRefund)
        {
            //1. Kiểm tra phiếu Refund khiếu nại có tồn tại
            var claimForRefundSave = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(s => s.Code == claimForRefund.Code && !s.IsDelete);
            if (claimForRefundSave == null)
            {
                return Json(new { status = Result.Failed, msg = "Not exist refund claim for this claim ticket!" },
                   JsonRequestBehavior.AllowGet);
            }
            claimForRefund.Id = claimForRefundSave.Id;
            Mapper.Map(claimForRefund, claimForRefundSave);

            //2. Kiểm tra Ticket có tồn tại và thay đổi trạng thái của Ticket
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);

            if (complain == null)
            {

                return Json(new { status = Result.Failed, msg = "Not exist detail refund for this complaint  this ticket  !" },
                        JsonRequestBehavior.AllowGet);
            }

            if (complain.Status != (byte)ComplainStatus.OrderWait && complain.Status != (byte)ComplainStatus.ApprovalWait)
            {

                return Json(new { status = Result.Failed, msg = "Action can not be performed when a refund request has been made for customer care !" },
                        JsonRequestBehavior.AllowGet);
            }
            complain.Status = (byte)ComplainStatus.CustomerCareWait;
            complain.LastUpdateDate = DateTime.Now;

            var isOrder = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.Order);
            var isCustomerCare = UnitOfWork.OfficeRepo.CheckOfficeType(UserState.OfficeId.Value, (byte)OfficeType.CustomerCare);

            //3. Lưu thông tin xuống database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1 Cập nhật thông tin phiếu yêu cầu Refund
                    if (!isOrder)
                    {
                        //-------Nếu là trưởng phòng CSKH , giám đốc-------
                        complain.BigMoney = claimForRefundSave.RealTotalRefund;
                        claimForRefundSave.ApproverId = UserState.UserId;
                        claimForRefundSave.ApproverName = UserState.UserName;
                    }

                    claimForRefundSave.LastUpdated = DateTime.Now;
                    claimForRefundSave.Status = (byte)ClaimForRefundStatus.CustomerCareWait;
                    UnitOfWork.ClaimForRefundRepo.Save();

                    //3.2 Gửi Notify cho nhân viên CSKH
                    var claim = UnitOfWork.ClaimForRefundRepo.FirstOrDefault(s => s.TicketId == claimForRefundSave.TicketId);
                    if (claimForRefundSave.SupportId > 0)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient((int)claimForRefundSave.SupportId,
                            "Xử lý yêu cầu Refund",
                            EnumNotifyType.Info,
                            $" <a href=\"" + "/Ticket/#CFRF" + claim.Code + "\" target=\"_blank\">" + claim.Code + "</a>",
                            "Requires processing a request for a top up request: " + claim.Code,
                            Url.Action("Index", "Ticket"));
                    }

                    //3.3 Cập nhật trạng thái Ticket
                    UnitOfWork.ComplainRepo.Save();


                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complain.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complain.CustomerId;
                    conplainHistory.CustomerName = complain.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.CustomerCareWait;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Chuyển sang trạng thái: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.CustomerCareWait);
                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();


                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = Result.Succeed, msg = "Information will be forwarded to customer care for further processing" }, JsonRequestBehavior.AllowGet);
        }

        //------------------------CSKH tham gia xử lý Refund-------------------------------------

        //Cập nhật thông tin : sử dụng phương thức chung "ClaimForRefundInfoUpdate()"

        //Chuyển sang cho giám đốc, trưởng phòng CSKH phê duyệt
        [HttpPost]
        public async Task<ActionResult> ClaimForRefundForwardBoss(ClaimForRefund claimForRefund, List<ClaimForRefundDetail> listClaimForRefundDetail)
        {
            //1. Kiểm tra phiếu Refund khiếu nại có tồn tại
            var claimForRefundSave = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(s => s.Code == claimForRefund.Code && !s.IsDelete);
            if (claimForRefundSave == null)
            {
                return Json(new { status = Result.Failed, msg = "Not exist refund claim for this claim ticket!" },
                   JsonRequestBehavior.AllowGet);
            }
            claimForRefund.Id = claimForRefundSave.Id;
            Mapper.Map(claimForRefund, claimForRefundSave);

            //2. Kiểm tra Ticket có tồn tại và thay đổi trạng thái của Ticket
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);

            if (complain == null)
            {

                return Json(new { status = Result.Failed, msg = "This ticket does not exist !" },
                        JsonRequestBehavior.AllowGet);
            }

            if (complain.Status >= (byte)ComplainStatus.ApprovalWait)
            {

                return Json(new { status = Result.Failed, msg = "Can not operate! The refund request has been moved to approval!" },
                        JsonRequestBehavior.AllowGet);
            }

            complain.Status = (byte)ComplainStatus.ApprovalWait;
            complain.LastUpdateDate = DateTime.Now;

            //3. Lưu thông tin xuống database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    //3.1 Cập nhật thông tin phiếu yêu cầu Refund
                    claimForRefundSave.LastUpdated = DateTime.Now;
                    claimForRefundSave.Status = (byte)ClaimForRefundStatus.ApprovalWait;
                    UnitOfWork.ClaimForRefundRepo.Save();

                    //3.2 Cập nhật trạng thái Ticket
                    UnitOfWork.ComplainRepo.Save();

                    //3.3 Cập nhật thông tin sản phẩm cần Refund
                    foreach (var item in listClaimForRefundDetail)
                    {
                        var claimForRefundDetail = await UnitOfWork.ClaimForRefundDetailRepo.FirstOrDefaultAsync(s => s.Id == item.Id);
                        if (claimForRefundDetail == null)
                        {
                            return Json(new { status = Result.Failed, msg = "Not exist detail refund for this complaint  this ticket !" },
                                JsonRequestBehavior.AllowGet);
                        }
                        Mapper.Map(item, claimForRefundDetail);
                        UnitOfWork.ClaimForRefundDetailRepo.Save();
                    }

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complain.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complain.CustomerId;
                    conplainHistory.CustomerName = complain.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.ApprovalWait;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Switch to status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.ApprovalWait);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = Result.Succeed, msg = "nformation will be passed to the superiors for approval" }, JsonRequestBehavior.AllowGet);
        }

        //Chuyển sang cho kế toán xử lý tiếp
        [HttpPost]
        public async Task<ActionResult> ClaimForRefundForwardAccountant(ClaimForRefund claimForRefund, List<ClaimForRefundDetail> listClaimForRefundDetail)
        {
            //1. Kiểm tra phiếu Refund khiếu nại có tồn tại
            var claimForRefundSave = await UnitOfWork.ClaimForRefundRepo.FirstOrDefaultAsync(s => s.Code == claimForRefund.Code && !s.IsDelete);
            if (claimForRefundSave == null)
            {
                return Json(new { status = Result.Failed, msg = "Not exist refund claim for this claim ticket!" },
                   JsonRequestBehavior.AllowGet);
            }
            claimForRefund.Id = claimForRefundSave.Id;
            Mapper.Map(claimForRefund, claimForRefundSave);

            //2. Kiểm tra Ticket có tồn tại và thay đổi trạng thái của Ticket
            var complain = UnitOfWork.ComplainRepo.FirstOrDefault(s => s.Id == claimForRefund.TicketId);

            if (complain == null)
            {

                return Json(new { status = Result.Failed, msg = "This ticket does not exist !" },
                        JsonRequestBehavior.AllowGet);
            }
            //if (complain.Status == (byte)ComplainStatus.AccountantWait)
            //{

            //    return Json(new { status = Result.Failed, msg = "Không thể thao tác! Yêu cầu Refund đã chuyển sang kế toán xử lý !" },
            //            JsonRequestBehavior.AllowGet);
            //}
            if (complain.Status == (byte)ComplainStatus.Success)
            {
                return Json(new { status = Result.Failed, msg = "Failure to update refund processing when Complaint Handling is complete! " }, JsonRequestBehavior.AllowGet);
            }

            complain.Status = (byte)ComplainStatus.AccountantWait;
            complain.LastUpdateDate = DateTime.Now;

            //3. Lưu thông tin xuống database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {

                    //if (claimForRefundSave.ApproverId == null)
                    //{
                    //    claimForRefundSave.ApproverId = UserState.UserId;
                    //    claimForRefundSave.ApproverName = UserState.UserName;

                    //}

                    //3.1 Cập nhật thông tin phiếu yêu cầu Refund
                    claimForRefundSave.LastUpdated = DateTime.Now;
                    claimForRefundSave.Status = (byte)ClaimForRefundStatus.AccountantWait;
                    UnitOfWork.ClaimForRefundRepo.Save();

                    //3.2 Cập nhật trạng thái Ticket
                    UnitOfWork.ComplainRepo.Save();

                    //3.3 Cập nhật thông tin sản phẩm cần Refund
                    foreach (var item in listClaimForRefundDetail)
                    {
                        var claimForRefundDetail = await UnitOfWork.ClaimForRefundDetailRepo.FirstOrDefaultAsync(s => s.Id == item.Id);
                        if (claimForRefundDetail == null)
                        {
                            return Json(new { status = Result.Failed, msg = "Not exist detail refund for this complaint  this ticket !" },
                                JsonRequestBehavior.AllowGet);
                        }
                        Mapper.Map(item, claimForRefundDetail);
                        UnitOfWork.ClaimForRefundDetailRepo.Save();
                    }

                    var office = await UnitOfWork.OfficeRepo.FirstOrDefaultAsync(x => x.Type == (byte)OfficeType.Accountancy);
                    var listUser = await UnitOfWork.UserRepo.GetUserToOffice(0, 1, office.IdPath, office.Id);

                    foreach (var user in listUser)
                    {
                        NotifyHelper.CreateAndSendNotifySystemToClient(user.Id,
                            "Xử lý yêu cầu Refund",
                            EnumNotifyType.Info,
                            $" <a href=\"" + "/Accountant/#CFRF" + claimForRefundSave.Code + "\" target=\"_blank\">" + claimForRefundSave.Code + "</a>",
                            "Request a refund for the claim: " + claimForRefundSave.Code,
                            Url.Action("Index", "Accountant"));
                    }

                    //10. Lưu thông tin lịch sử cập nhật trạng thái khiếu nại
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ComplainId = complain.Id;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = complain.CustomerId;
                    conplainHistory.CustomerName = complain.CustomerName;
                    conplainHistory.Status = (byte)ComplainStatus.AccountantWait;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = "Switch to status: " + EnumHelper.GetEnumDescription<ComplainStatus>((byte)ComplainStatus.AccountantWait);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return Json(new { status = Result.Succeed, msg = "Information will be forwarded to further processing accountants!" }, JsonRequestBehavior.AllowGet);
        }

        //-------------------------Giám đốc, trưởng phòng CSKH tham gia phê duyệt-------------------------

        //sử dụng phương thức chung "ClaimForRefundForwardCareCustomer()"


        //Hàm hủy yêu cầu Refund
        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.TicketClaimforrefund, EnumPage.ExecuteClaimForRefund)]
        public JsonResult DeleteClaimForRefund(int claimForRefundId, string content)
        {
            //1. Kiểm tra thông tin phiếu yêu cầu Refund
            var claimForRefundDetail =
                UnitOfWork.ClaimForRefundRepo.FirstOrDefault(x => !x.IsDelete && x.Id == claimForRefundId);
            if (claimForRefundDetail == null)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.DataIsNotValid }, JsonRequestBehavior.AllowGet);
            }

            //2. Kiểm tra xem ClaimForRefund đã được xác nhận chưa
            if (claimForRefundDetail.Status == (byte)ClaimForRefundStatus.Success)
            {
                return Json(new { status = Result.Failed, msg = ConstantMessage.ClaimForRefundApproval }, JsonRequestBehavior.AllowGet);
            }

            //3. Khởi tạo transaction kết nối database
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    // Lấy lại thông tin để thực hiện lưu
                    claimForRefundDetail.IsDelete = true;
                    claimForRefundDetail.ReasonCancel = content;
                    claimForRefundDetail.Status = (byte)ClaimForRefundStatus.Cancel;
                    //Lưu xuống Database
                    UnitOfWork.ClaimForRefundRepo.Update(claimForRefundDetail);
                    UnitOfWork.ClaimForRefundRepo.Save();

                    //10. Lưu thông tin lịch sử cập nhật trạng thái phiếu yêu cầu Refund
                    var conplainHistory = new ComplainHistory();
                    conplainHistory.ClaimForRefundId = claimForRefundDetail.Id;
                    conplainHistory.ComplainId = claimForRefundDetail.TicketId ?? 0;
                    conplainHistory.CreateDate = DateTime.Now;
                    conplainHistory.CustomerId = claimForRefundDetail.CustomerId ?? 0;
                    conplainHistory.CustomerName = claimForRefundDetail.CustomerFullName;
                    conplainHistory.Status = (byte)ClaimForRefundStatus.Cancel;
                    conplainHistory.UserId = UserState.UserId;
                    conplainHistory.UserFullName = UserState.FullName;
                    conplainHistory.Content = " change refund status: " + EnumHelper.GetEnumDescription<ClaimForRefundStatus>((byte)ClaimForRefundStatus.Cancel);

                    UnitOfWork.ComplainHistoryRepo.Add(conplainHistory);
                    UnitOfWork.ComplainHistoryRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = Result.Succeed, msg = ConstantMessage.DeleteClaimForRefundIsSuccess }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Xuat file Excel]

        //Danh sách Ticket khiếu nại
        [HttpPost]
        public async Task<ActionResult> ExportExcelComplain(ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            int page = 1;
            int pageSize = Int32.MaxValue;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            DateStart = GetStartOfDay(DateStart);
            DateEnd = GetEndOfDay(DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            var listComplain = await UnitOfWork.ComplainRepo.GetAllTicketComplainList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);
            return ExportExcel(searchModal, listComplain, "DANH SÁCH KHIẾU NẠI ĐÃ ĐƯỢC PHÂN CÔNG XỬ LÝ");
        }

        //Danh sách Ticket tôi cần xử lý
        [HttpPost]
        public async Task<ActionResult> ExportExcelTicket(ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            int page = 1;
            int pageSize = Int32.MaxValue;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            DateStart = GetStartOfDay(DateStart);
            DateEnd = GetEndOfDay(DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            var listComplain = await UnitOfWork.ComplainRepo.GetAllTicketList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);
            return ExportExcel(searchModal, listComplain, "LIST OF COMPLAINTS I NEED TO HANDLE");
        }

        //Danh sách Ticket khiếu nại trễ xử lý
        [HttpPost]
        public async Task<ActionResult> ExportExcelTicketLate(ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            int page = 1;
            int pageSize = Int32.MaxValue;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            DateStart = GetStartOfDay(DateStart);
            DateEnd = GetEndOfDay(DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            var listComplain = await UnitOfWork.ComplainRepo.GetAllTicketLastList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);
            return ExportExcel(searchModal, listComplain, "LIST OF COMPLAINTS DELAY PROCESSED");
        }


        //Danh sách Ticket khiếu nại có người hỗ trợ
        [HttpPost]
        public async Task<ActionResult> ExportExcelTicketSupport(ComplainSearchModal searchModal, int? userId, int? customerId)
        {
            int page = 1;
            int pageSize = Int32.MaxValue;
            long totalRecord;
            if (searchModal == null)
            {
                searchModal = new ComplainSearchModal();
            }
            searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
            var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
            var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
            DateStart = GetStartOfDay(DateStart);
            DateEnd = GetEndOfDay(DateEnd);
            searchModal.Keyword = RemoveCode(searchModal.Keyword);
            var listComplain = await UnitOfWork.ComplainRepo.GetAllTicketSupportList(out totalRecord, page, pageSize, searchModal.Keyword, searchModal.Status, searchModal.SystemId, DateStart, DateEnd, userId, customerId, UserState);
            return ExportExcel(searchModal, listComplain, "LIST OF COMPLAINTS HAS SUPPORTER");
        }
        //Hàm Xuất file Excel dùng chung
        [HttpPost]
        public ActionResult ExportExcel(ComplainSearchModal searchModal, List<TicketComplain> list, string title)
        {


            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");


                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TICKET CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, " Order code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order Type", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "compain type", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, " COMPLAINTS TYPE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "AMOUNT REQUESTED", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "AMOUNT APPROVAL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF IN CHARGE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STAFF SUPPORT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CONTENT COMPAINT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "INTERNAL NOTES CUSTOMER CARE", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, title, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                var start = searchModal.DateStart == null ? "__" : DateTime.Parse(searchModal.DateStart).ToShortDateString();
                var end = searchModal.DateEnd == null ? "__" : DateTime.Parse(searchModal.DateEnd).ToShortDateString();
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"from the: {start} to they {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (list.Any())
                {
                    foreach (var ticket in list)
                    {
                        //var type = ticket.TypeService.Split(',');
                        //var typeService = EnumHelper.GetEnumDescription<ComplainTypeService>(byte.Parse(type[0]));
                        //for (int i = 0; i < type.Length; i++)
                        //{
                        //    if (type[i] != null && i > 0)
                        //    {
                        //        typeService += ',' + EnumHelper.GetEnumDescription<ComplainTypeService>(byte.Parse(type[i]));
                        //    }
                        //}
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        //Ma ticket
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //thoi gian
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CreateDate.Value.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Loai don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<OrderType>((byte)ticket.OrderType), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Loai khieu nai
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.TypeServiceName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.TypeServiceCloseName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //So tien yeu cau
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.RequestMoney, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        //So tien duyet
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.BigMoney, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        //Nhan vien xu ly
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Nhan vien ho tro
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserSupport, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Trang thai
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<ComplainStatus>((int)ticket.Status), ExcelHorizontalAlignment.Center, true);
                        col++;
                        //Noi dung
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Content, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ghi chu noi bo
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.ContentInternal, ExcelHorizontalAlignment.Left, true);
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"DANHSACHTICKET{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }

        //Danh sách phiếu yêu cầu Refund
        [HttpPost]
        public async Task<ActionResult> ExportExcelClaimForRefund(ClaimForRefundSearchModal searchModal, int? userId, int? customerId)
        {
            //long totalRecord;
            int page = 1;
            int pageSize = Int32.MaxValue;

            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                long totalRecord;
                List<ClaimForRefund> claimForRefundModal;
                if (searchModal == null)
                {
                    searchModal = new ClaimForRefundSearchModal();
                }
                searchModal.Keyword = MyCommon.Ucs2Convert(searchModal.Keyword);
                var DateStart = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateStart) ? DateTime.Now.AddYears(-100).ToString() : searchModal.DateStart);
                var DateEnd = DateTime.Parse(string.IsNullOrEmpty(searchModal.DateEnd) ? DateTime.Now.AddYears(1).ToString() : searchModal.DateEnd);
                DateStart = GetStartOfDay(DateStart);
                DateEnd = GetEndOfDay(DateEnd);
                searchModal.Keyword = String.IsNullOrEmpty(searchModal.Keyword) ? "" : searchModal.Keyword.Trim();

                claimForRefundModal = await UnitOfWork.ClaimForRefundRepo.FindAsync(
                    out totalRecord,
                    x => x.Code.Contains(searchModal.Keyword)
                    && (UserState.Type > 0 || x.UserId == UserState.UserId)
                         //&& !x.IsDelete
                         && (customerId == null || customerId == 0 || x.CustomerId == customerId)
                         && (searchModal.Status == -1 || x.Status == searchModal.Status)
                         && (userId == null || userId == 0 || x.UserId == userId)
                         && x.Created >= DateStart && x.Created <= DateEnd,
                    x => x.OrderByDescending(y => y.Created),
                    page,
                    pageSize
                );

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REQUEST CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TICKET CODE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Customer code", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "customer's full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER EMAIL", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDERING STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EMAIL ORDERING STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BUSINESS STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EMAIL BUSINESS STAFF", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "NV CSKH", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BUSINESS STAFF CUSTOMER CARE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "BUSINESS STAFF ACCOUNTANT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "EMAIL BUSINESS STAFF ACCOUNTANT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TIME", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL AMOUNT EXPECTED", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order requires shop (TT)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Order requires shop (MC)", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER CARE REQUEST", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "REDUCE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "TOTAL ACTUAL MONEY", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ORDER NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "CUSTOMER CARE NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "ACCOUNTANT NOTE", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "STATUS", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "DANH SÁCH YÊU CẦU HOÀN TIỀN KHIẾU NẠI", new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                var start = searchModal.DateStart == null ? "__" : DateTime.Parse(searchModal.DateStart).ToShortDateString();
                var end = searchModal.DateEnd == null ? "__" : DateTime.Parse(searchModal.DateEnd).ToShortDateString();

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"From: {start} to date {end}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (claimForRefundModal.Any())
                {
                    foreach (var ticket in claimForRefundModal)
                    {
                        col = 1;
                        //STT
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        //Ma yeu cau
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Code, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ma ticket
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.TicketCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ma don hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ma khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerCode, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //ten khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Mail khach hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //dat hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserName, ExcelHorizontalAlignment.Center, true);
                        col++;
                        //mail dat hang
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.UserEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //kinh dioanh
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderUserFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //mail kinh doanh
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.OrderUserEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //CSKH
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //mail CSKH
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.CustomerEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //Ke toan
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.AccountantFullName, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //mail ke toan
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.AccountantEmail, ExcelHorizontalAlignment.Left, true);
                        col++;
                        //ngay
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Left, true);
                        col++;
                        //
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.MoneyRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.MoneyOrderRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.MoneyOrderRefundDicker, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.SupporterMoneyRequest, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.CurrencyDiscount, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, ticket.RealTotalRefund, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.NoteOrderer, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.NoteSupporter, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, ticket.NoteAccountanter, ExcelHorizontalAlignment.Left, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, EnumHelper.GetEnumDescription<ClaimForRefundStatus>(ticket.Status), ExcelHorizontalAlignment.Center, true);
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;

                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"YEUCAUHOANTIENKHIEUNAI{DateTime.Now:yyyyMMddHHmmssfff}.xlsx");
            }
        }
        #endregion
        #region [Thống kê]
        //Thống kê
        [HttpPost]
        public JsonResult listViewReport(DateTime? dateStart, DateTime? dateEnd)
        {
            //1. Lây danh sach ticket xử lý trong ngày
            dateStart = dateStart ?? DateTime.Now;
            dateEnd = dateEnd ?? DateTime.Now;

            var listTicket = UnitOfWork.ComplainRepo.FindAsNoTracking(x => x.LastUpdateDate >= dateStart && x.LastUpdateDate <= dateEnd && !x.IsDelete)
                .Select(s => new { s.Id, s.Status }).OrderBy(x => x.Status).ToList();
            var complain = UnitOfWork.ComplainUserRepo.Find(s => s.IsCare == true);
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
            var detailTicket = new List<long>();
            var listTicket1 = new List<ComplainUser>();
            if (listTicket != null)
            {
                foreach (var item in listTicket.Where(s => s.Status == (int)ComplainStatus.Success))
                {
                    var x = complain.FirstOrDefault(s => s.ComplainId == item.Id);
                    if (x != null)
                    {
                        listTicket1.Add(x);
                    }

                };
            }

            int total = 0;
            if (listTicket1 != null)
            {
                foreach (var ticket in listTicket1.GroupBy(x => x.UserId).ToList())
                {
                    var firstOrDefault = ticket.FirstOrDefault();
                    if (firstOrDefault != null)
                    {
                        detailName.Add(firstOrDefault.UserName);
                        detailTicket.Add(ticket.Count());
                        total += ticket.Count();
                    }
                }
            }


            //4. Trả kết quả lên view
            return Json(new { total, overview, detailName, detailTicket }, JsonRequestBehavior.AllowGet);
        }

        //2. Thống kê ticket mà nhân viên tiếp nhân theo thời gian
        public JsonResult GetTicketReceiveSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketReceiveSituation(out total, start, end).GroupBy(x => x.UserId);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var user = new List<string>();
            var TotalMoney = new List<decimal>();
            foreach (var tk in ticket)
            {
                user.Add(tk.FirstOrDefault().UserName);
                totalTicket.Add(tk.Count());
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }

        //3. Thống kê ticket theo Type VIP theo thời gian
        public JsonResult GetTicketVIPSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketVIPSituation(out total, start, end).GroupBy(x => x.LevelId);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var user = new List<string>();
            var TotalMoney = new List<decimal>();
            foreach (var tk in ticket)
            {
                user.Add(tk.FirstOrDefault().LevelName);
                totalTicket.Add(tk.Count());
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }

        //4. Thống kê ticket theo Type Khiếu nại theo thời gian
        public JsonResult GetTicketTypeSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketTypeSituation(out total, start, end).GroupBy(x => x.TypeService);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var user = new List<string>();
            var TotalMoney = new List<decimal>();
            foreach (var tk in ticket)
            {
                user.Add(tk.FirstOrDefault().TypeServiceName);
                totalTicket.Add(tk.Count());
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }

        //5. Thống kê ticket xử lý xong theo thời gian
        public JsonResult GetTicketSuccessSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketSuccessSituation(out total, start, end);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var user = new List<string>();
            var TotalMoney = new List<decimal>();
            foreach (var tk in ticket)
            {
                user.Add(tk.Created);
                totalTicket.Add(tk.TotalOrder);
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }

        //6. Thống kê số tiền hoàn bồi/ ticket theo thời gian
        public JsonResult GetTicketClaimSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketClaimSituation(out total, start, end);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var TotalMoney = new List<decimal>();
            var user = new List<string>();
            foreach (var tk in ticket)
            {
                user.Add(tk.Created);
                totalTicket.Add(tk.TotalOrder);
                TotalMoney.Add(tk.TotalMoney ?? 0);
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }

        #region [Xuat Exel]
        [HttpPost]
        public FileContentResult ExcelTicketSituationReport(DateTime? startDay, DateTime? endDay, int contentReport)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var title = "Ticket statistics are generated over time";
            var fileName = "TICKET_TAOKHIEUNAI";

            //1. Tạo các dữ liệu theo báo cáo
            var listTicket = new List<dynamic>();

            //Theo loai khieu loai
            if (contentReport == 2)
            {
                title = "Ticket statistics by type of complaint";
                fileName = "TICKET_LOAIKHIEUNAI";
                var ticket2 = UnitOfWork.ComplainRepo.GetTicketTypeSituation(out total, start, end).GroupBy(x => x.TypeService);

                foreach (var tk in ticket2)
                {
                    listTicket.Add(new
                    {
                        text = tk.FirstOrDefault().TypeServiceName,
                        value = tk.Count()
                    });
                }
                return CommonExcelTicketReport(start, end, listTicket, "compain type", title, fileName);
            }
            //Theo loaj VIP
            else if (contentReport == 3)
            {
                title = "Ticket statistics by VIP type";
                fileName = "TICKET_LOAIVIP";
                var ticket3 = UnitOfWork.ComplainRepo.GetTicketVIPSituation(out total, start, end).GroupBy(x => x.LevelId);
                foreach (var tk in ticket3)
                {
                    listTicket.Add(new
                    {
                        text = tk.FirstOrDefault().LevelName,
                        value = tk.Count()
                    });
                }
                return CommonExcelTicketReport(start, end, listTicket, "VIP type", title, fileName);
            }
            //Xu ly xong
            else if (contentReport == 4)
            {
                title = "Tickets statistics have been processed";
                fileName = "TICKET_KHIEUNAITHANHCONG";
                var ticket4 = UnitOfWork.ComplainRepo.GetTicketSuccessSituation(out total, start, end);

                return CommonTicketReport(start, end, ticket4, title, fileName, contentReport);
            }
            //Nhan vien nhan
            else if (contentReport == 5)
            {
                title = "Ticket statistics according to staff handle";
                fileName = "TICKET_NHANVIENNHAN";
                var ticket5 = UnitOfWork.ComplainRepo.GetTicketReceiveSituation(out total, start, end).GroupBy(x => x.UserId);
                foreach (var tk in ticket5)
                {
                    listTicket.Add(new
                    {
                        text = tk.FirstOrDefault().UserName,
                        value = tk.Count()
                    });
                }
                return CommonExcelTicketReport(start, end, listTicket, "Staff", title, fileName);
            }
            //Tien hoan boi
            else if (contentReport == 6)
            {
                title = "THONG KE TICKET CUNG SO TIEN HOAN BOI";
                fileName = "TICKET_TIENHOANBOI";
                var ticket6 = UnitOfWork.ComplainRepo.GetTicketClaimSituation(out total, start, end);
                return CommonTicketReport(start, end, ticket6, title, fileName, contentReport);
            }
            //Ticket duoc tao
            var ticket = UnitOfWork.ComplainRepo.GetTicketSituationOnTime(out total, start, end);
            return CommonTicketReport(start, end, ticket, title, fileName, contentReport);
        }

        //Ham chung xuat Excel thong ke ticket
        public FileContentResult CommonExcelTicketReport(DateTime startDay, DateTime endDay, List<dynamic> listUserExcel, string Column, string Title, string fileName)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, Column, ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Số ticket", ExcelHorizontalAlignment.Center, true, colorHeader);


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, Title, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                ngay = "From: " + startDay + " to date " + endDay;
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.text, ExcelHorizontalAlignment.Right, true);
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.value, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;
                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"{fileName + DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }
        #endregion
        #endregion

        #region [Thống kê tình hình khiếu nại theo thời gian]
        [HttpPost]
        public JsonResult GetTicketSituation(DateTime? startDay, DateTime? endDay)
        {
            long total;
            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            var ticket = UnitOfWork.ComplainRepo.GetTicketSituationOnTime(out total, start, end);

            //1. Tạo các dữ liệu theo báo cáo
            var totalTicket = new List<long>();
            var user = new List<string>();
            var TotalMoney = new List<decimal>();
            foreach (var or in ticket)
            {
                user.Add(or.Created);
                totalTicket.Add(or.TotalOrder);
            }
            //2. Trả kết quả lên view
            return Json(new { total, user, totalTicket, TotalMoney }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public FileContentResult CommonTicketReport(DateTime startDay, DateTime endDay, List<ProfitDay> listUserExcel, string Title, string fileName, int contentReport)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Ngày", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Số ticket", ExcelHorizontalAlignment.Center, true, colorHeader);
                if (contentReport == 6)
                {
                    col++;
                    ExcelHelper.CreateHeaderTable(sheet, row, col, "Số tien hoan boi", ExcelHorizontalAlignment.Center, true, colorHeader);
                }


                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, Title, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });
                ngay = "From: " + startDay + " to date " + endDay;
                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.Created.ToString(CultureInfo.InvariantCulture), ExcelHorizontalAlignment.Right, true);
                        col++;

                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalOrder, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        if (contentReport == 6)
                        {
                            col++;

                            ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalMoney, new CustomExcelStyle
                            {
                                IsMerge = false,
                                IsBold = false,
                                Border = ExcelBorderStyle.Thin,
                                HorizontalAlign = ExcelHorizontalAlignment.Right,
                                NumberFormat = "#,##0"
                            });
                        }
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;
                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"{fileName + DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }

        [HttpPost]
        public JsonResult ExcelReportTicketSituation(DateTime? startDay, DateTime? endDay)
        {
            var ordered = UnitOfWork.OrderRepo.GetTotalProfitReportAllDay(startDay, endDay, UserState);

            //1. Tạo các dữ liệu theo báo cáo
            var totalOrder = new List<int>();
            var totalBargain = new List<dynamic>();
            var totalMoney = new List<dynamic>();
            var day = new List<string>();

            foreach (var or in ordered)
            {
                day.Add(or.Created);
                totalOrder.Add(or.TotalOrder);
                totalBargain.Add(or.TotalBargain);
                totalMoney.Add(or.TotalMoney);
            }
            //2. Trả kết quả lên view
            return Json(new { day, totalBargain, totalMoney, totalOrder }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTotalProfitBargainReportAllDay(DateTime? startDay, DateTime? endDay)
        {
            var status = (byte)OrderStatus.Finish;
            var ordered = UnitOfWork.OrderRepo.GetTotalProfitBargainReportAllDay(startDay, endDay, UserState, status);

            //1. Tạo các dữ liệu theo báo cáo
            var totalOrder = new List<int>();
            var totalBargain = new List<dynamic>();
            var totalMoney = new List<dynamic>();
            var day = new List<string>();

            foreach (var or in ordered)
            {
                day.Add(or.Created);
                totalOrder.Add(or.TotalOrder);
                totalBargain.Add(or.TotalBargain);
            }
            //2. Trả kết quả lên view
            return Json(new { day, totalBargain, totalMoney, totalOrder }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Thống kê Number orders of quotations theo đầu nhân viên]
        [HttpPost]
        public async Task<JsonResult> GetOrderWaitSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là đang báo giá
            var status = (byte)OrderStatus.AreQuotes;

            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.CustomerCare, UserState);
            var ordered = UnitOfWork.ComplainRepo.GetOrderWaitSituationAllDay(listUser, startDay, endDay, status);
            //1. Tạo các dữ liệu theo báo cáo
            var Name = new List<string>();
            var totalOrder = new List<int>();

            foreach (var or in ordered)
            {

                Name.Add(or.FullName);
                totalOrder.Add(or.TotalCusstomer);
            }

            var total = ordered.Sum(s => s.TotalCusstomer);
            //2. Trả kết quả lên view
            return Json(new { Name, totalOrder, total }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetOrderWaitSituationContinute(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là đang báo giá
            var status = (byte)OrderStatus.AreQuotes;

            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);
            
            var ordered = UnitOfWork.ComplainRepo.GetOrderWaitSituationContinuteAllDay(startDay, endDay, status);
            //1. Tạo các dữ liệu theo báo cáo
            var Name = new List<string>();
            var totalOrder = new List<int>();

            foreach (var or in ordered)
            {

                Name.Add(or.FullName);
                totalOrder.Add(or.TotalCusstomer);
            }

            var total = ordered.Sum(s => s.TotalCusstomer);
            //2. Trả kết quả lên view
            return Json(new { Name, totalOrder, total }, JsonRequestBehavior.AllowGet);
        }
        //Thống kê Order khách tạo theo thời gian (24h)
        [HttpPost]
        public JsonResult GetOrderCustomerSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là chờ báo giá
            var status = (byte)OrderStatus.WaitPrice;

            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);

            var ordered = UnitOfWork.ComplainRepo.GetOrderCustomerSituation(startDay, endDay, status);
            //1. Tạo các dữ liệu theo báo cáo
            var Name = new List<string>();
            var totalOrder = new List<int>();

            foreach (var or in ordered)
            {

                Name.Add(or.FullName);
                totalOrder.Add(or.TotalCusstomer);
            }

            var total = ordered.Sum(s => s.TotalCusstomer);
            //2. Trả kết quả lên view
            return Json(new { Name, totalOrder, total }, JsonRequestBehavior.AllowGet);
        }


        //Thống kê Order đặt hàng tạo theo thời gian (24h)
        [HttpPost]
        public JsonResult GetOrderUserSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là chờ báo giá
            var status = (byte)OrderStatus.WaitPrice;

            var start = GetStartOfDay(startDay ?? DateTime.Now);
            var end = GetEndOfDay(endDay ?? DateTime.Now);

            var ordered = UnitOfWork.ComplainRepo.GetOrderUserSituation(startDay, endDay, status);
            //1. Tạo các dữ liệu theo báo cáo
            var Name = new List<string>();
            var totalOrder = new List<int>();

            foreach (var or in ordered)
            {

                Name.Add(or.FullName);
                totalOrder.Add(or.TotalCusstomer);
            }

            var total = ordered.Sum(s => s.TotalCusstomer);
            //2. Trả kết quả lên view
            return Json(new { Name, totalOrder, total }, JsonRequestBehavior.AllowGet);
        }

        #region [Export Excel Thống kê tiền bargain Order theo đầu nhân viên và theo thời gian]
        [HttpPost]
        public async Task<FileContentResult> ExcelReportOrderWaitSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là đang báo giá
            var status = (byte)OrderStatus.AreQuotes;

            var listUser = await UnitOfWork.UserRepo.GetUserToOfficeType(OfficeType.CustomerCare, UserState);
            var ordered = UnitOfWork.ComplainRepo.GetOrderWaitSituationAllDay(listUser, startDay, endDay, status);

            return CommonOrderWaitReport(startDay, endDay, ordered, "Staff", "Quote by staff", 1);
        }
        [HttpPost]
        public FileContentResult ExcelReportOrderWaitSituationContinute(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là đang báo giá
            var status = (byte)OrderStatus.WaitPrice;
            var ordered = UnitOfWork.ComplainRepo.GetOrderWaitSituationContinuteAllDay(startDay, endDay, status);

            return CommonOrderWaitReport(startDay, endDay, ordered,"Time", "Quote over time", 2);
        }
        [HttpPost]
        public FileContentResult ExcelReportOrderCustomerSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là chờ báo giá
            var status = (byte)OrderStatus.WaitPrice;
            var ordered = UnitOfWork.ComplainRepo.GetOrderCustomerSituation(startDay, endDay, status);

            return CommonOrderWaitReport(startDay, endDay, ordered, "Time the customer creates", "Customers create quotes over time", 3);
        }
        [HttpPost]
        public FileContentResult ExcelReportOrderUserSituation(DateTime? startDay, DateTime? endDay)
        {
            // Trạng thái là chờ báo giá
            var status = (byte)OrderStatus.AreQuotes;
            var ordered = UnitOfWork.ComplainRepo.GetOrderUserSituation(startDay, endDay, status);

            return CommonOrderWaitReport(startDay, endDay, ordered, "Order time created", "Order quotes over time", 4);
        }
        public FileContentResult CommonOrderWaitReport(DateTime? startDay, DateTime? endDay, List<CustomerUser> listUserExcel, string title, string fileName, int type)
        {
            var ngay = "";
            using (var xls = new ExcelPackage())
            {
                var sheet = xls.Workbook.Worksheets.Add("Sheet1");
                var colorHeader = ColorTranslator.FromHtml("#70ad47");

                var col = 1;
                var row = 4;

                ExcelHelper.CreateHeaderTable(sheet, row, col, "STT", ExcelHorizontalAlignment.Center, true, colorHeader);
                col++;
                if(type == 1)
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col, "Full name", ExcelHorizontalAlignment.Center, true, colorHeader);
                }
                else if(type == 2)
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col, "Day quotes", ExcelHorizontalAlignment.Center, true, colorHeader);
                }
                else
                {
                    ExcelHelper.CreateHeaderTable(sheet, row, col, "Time to create a quotation", ExcelHorizontalAlignment.Center, true, colorHeader);
                }
                
                col++;
                ExcelHelper.CreateHeaderTable(sheet, row, col, "Number orders of quotations", ExcelHorizontalAlignment.Center, true, colorHeader);

                #region Title
                ExcelHelper.CreateCellTable(sheet, row - 3, 1, row - 3, col, "THỐNG KÊ Number orders of quotations THEO "+ title, new CustomExcelStyle
                {
                    IsBold = true,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 16
                });

                var start = startDay?.ToShortDateString() ?? "__";
                var end = endDay?.ToShortDateString() ?? "__";

                ngay = "From: " + start + " to date " + end;

                ExcelHelper.CreateCellTable(sheet, row - 2, 1, row - 2, col, $"{ngay}", new CustomExcelStyle
                {
                    IsBold = false,
                    IsMerge = true,
                    HorizontalAlign = ExcelHorizontalAlignment.Center,
                    FontSize = 9
                });
                #endregion

                var no = row + 1;

                if (listUserExcel.Any())
                {
                    foreach (var w in listUserExcel)
                    {
                        col = 1;
                        ExcelHelper.CreateCellTable(sheet, no, col, no - row, ExcelHorizontalAlignment.Center, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, w.FullName, ExcelHorizontalAlignment.Right, true);
                        col++;
                        ExcelHelper.CreateCellTable(sheet, no, col, no, col, w.TotalCusstomer, new CustomExcelStyle
                        {
                            IsMerge = false,
                            IsBold = false,
                            Border = ExcelBorderStyle.Thin,
                            HorizontalAlign = ExcelHorizontalAlignment.Right,
                            NumberFormat = "#,##0"
                        });
                        no++;
                    }
                }

                sheet.Cells.AutoFitColumns();
                sheet.Column(1).Width = 6;
                sheet.Row(4).Height = 30;


                return File(xls.GetAsByteArray(), System.Net.Mime.MediaTypeNames.Application.Octet, $"{fileName+ DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
        }
        #endregion
        #endregion

        #region [Chốt note]
        // Tạo ticket
        [HttpPost]
        public async Task<JsonResult> NoteClose(int complainId, string noteClose)
        {

            var ticket = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(s => s.Id == complainId && !s.IsDelete);
            if (ticket == null)
            {
                return Json(new { status = Result.Failed, msg = "No complaints exist or has been removed!" }, JsonRequestBehavior.AllowGet);
            }
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    ticket.ContentInternal = noteClose;
                    UnitOfWork.ComplainRepo.Save();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return Json(new { status = Result.Succeed, msg = "Fasten the note successfully!" }, JsonRequestBehavior.AllowGet);
        }

        //Chốt note trên danh sách
        [HttpPost]
        public async Task<JsonResult> NoteCloseCommon(TicketComplain complain)
        {

            var ticket = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(s => s.Id == complain.Id && !s.IsDelete);
            if (ticket == null)
            {
                return Json(new { status = Result.Failed, msg = "No complaints exist or has been removed!" }, JsonRequestBehavior.AllowGet);
            }

            ticket.ContentInternal = complain.ContentInternal;
            ticket.ContentInternalOrder = complain.ContentInternalOrder;
            UnitOfWork.ComplainRepo.Save();

            return Json(new { status = Result.Succeed, msg = "Fasten the note successfully!" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region [Cấu hình Type khiếu nại] 

        [LogTracker(EnumAction.View, EnumPage.ComplainType)]
        public async Task<ActionResult> ComplainType(ModelView<ComplainType, ComplainTypeViewModel> model)
        {

            long totalRecord;

            if (model.SearchInfo == null)
            {
                model.SearchInfo = new ComplainTypeViewModel();
            }
            model.Items = await UnitOfWork.ComplainTypeRepo.FindAsync(
               out totalRecord,
               x => !x.IsDelete && (x.IdPath == model.SearchInfo.Path || x.IdPath.StartsWith(model.SearchInfo.Path + ".")),
               x => x.OrderBy(y => y.ParentId),
               model.PageInfo.CurrentPage,
               model.PageInfo.PageSize
           );

            model.PageInfo.TotalRecord = (int)totalRecord;
            model.PageInfo.Name = "Type khiếu nại";
            model.PageInfo.Url = Url.Action("ComplainType", "Ticket");

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ListComplainType", model);
            }

            ViewBag.complainTypeJsTree = ComplainTypeJsTree();

            return View(model);
        }

        [CheckPermission(EnumAction.View, EnumPage.ComplainType)]
        public ActionResult CreateComplainType()
        {
            ViewBag.complainTypeJsTree = ComplainTypeActonJsTree();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Add, EnumPage.ComplainType)]
        public async Task<ActionResult> CreateComplainType(ComplainTypeMeta model)
        {
            ViewBag.complainTypeJsTree = ComplainTypeActonJsTree();
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return View();

            // Tên Type khiếu nại đã tồn tại
            if (await UnitOfWork.ComplainTypeRepo.AnyAsync(x => x.Name.Equals(model.Name) && !x.IsDelete))
            {
                ModelState.AddModelError("Name", $"Tên loại khiếu nại \"{model.Name }\" already exists");
                return View();
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var trea = Mapper.Map<ComplainType>(model);
                    var obj = new ComplainType();
                    if (trea.ParentId != 0)
                    {
                        obj = UnitOfWork.ComplainTypeRepo.SingleOrDefault(x => x.Id == trea.ParentId);
                        if (obj != null)
                        {
                            obj.IsParent = true;
                            trea.IdPath = string.Empty;
                            trea.NamePath = obj.NamePath + "/" + trea.Name;
                            trea.ParentId = obj.Id;
                            trea.ParentName = obj.Name;
                        }
                        else
                        {
                            TempData["Msg"] = $"does not exist hoặc đã xóa Type khiếu nại đã chọn";
                        }
                    }
                    else
                    {
                        trea.IdPath = string.Empty;
                        trea.NamePath = trea.Name;
                        trea.ParentId = 0;
                        trea.ParentName = "";
                    }

                    UnitOfWork.ComplainTypeRepo.Add(trea);
                    var rs = await UnitOfWork.ComplainTypeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View();
                    }
                    //Cập nhật lại IdPath
                    if (trea.ParentId != 0)
                    {
                        trea.IdPath = obj.IdPath + "." + trea.Id;
                    }
                    else
                    {
                        trea.IdPath = "0." + trea.Id;
                    }

                    await UnitOfWork.ComplainTypeRepo.SaveAsync();

                    TempData["Msg"] = $"Add successful Type khiếu nại \"<b>{trea.Name}</b>\"";
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return RedirectToAction("CreateComplainType");
        }

        [CheckPermission(EnumAction.View, EnumPage.ComplainType)]
        public async Task<ActionResult> EditComplainType(int id)
        {
            ViewBag.complainTypeJsTree = ComplainTypeActonJsTree();
            var trea = await UnitOfWork.ComplainTypeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return HttpNotFound($"Không có định khoản thu chi nào có Id là {id}");

            return View(Mapper.Map<ComplainTypeMeta>(trea));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckPermission(EnumAction.Update, EnumPage.ComplainType)]
        public async Task<ActionResult> EditComplainType(ComplainTypeMeta model)
        {
            ViewBag.complainTypeJsTree = ComplainTypeActonJsTree();
            if (!ModelState.IsValid)
                return View(model);

            //1. Kiểm tra Type khiếu nại có tồn tại hay không
            var trea = await UnitOfWork.ComplainTypeRepo.SingleOrDefaultAsync(x => x.Id == model.Id && !x.IsDelete);
            if (trea == null)
            {
                ModelState.AddModelError("NotExist", "Loại khiếu nại does not exist hoặc đã bị xóa");
                return View(model);
            }

            ComplainType treaParent = null;

            // Có thay đổi đơn vị cha
            if (model.ParentId != trea.ParentId)
            {
                // Kiểm tra đơn vị Cha có tồn tại hay không
                treaParent =
                    await UnitOfWork.ComplainTypeRepo.SingleOrDefaultAsync(x => x.Id == model.ParentId && !x.IsDelete);
                if (treaParent == null)
                {
                    ModelState.AddModelError("ParentId",
                        $"Loại khiếu nại cha \"{model.ParentName}\" does not exist hoặc đã bị xóa");
                    return View(model);
                }
                model.ParentName = treaParent.Name;
            }
            else
            {
                model.ParentName = trea.ParentName;
            }

            // Tên Type khiếu nại đã tồn tại
            if (
                await UnitOfWork.ComplainTypeRepo.AnyAsync(
                        x => x.Name.Equals(model.Name) && !x.IsDelete && x.ParentId == model.ParentId && x.Id != model.Id))
            {
                ModelState.AddModelError("Name", $"Tên loại khiếu nại \"{model.Name}\" đã tồn tại");
                return View(model);
            }

            // Khởi tạo transaction
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    var oldParentId = trea.ParentId;
                    var oldIdPath = trea.IdPath;
                    var oldNamePath = trea.NamePath;
                    trea = Mapper.Map(model, trea);

                    var rs = await UnitOfWork.ComplainTypeRepo.SaveAsync();

                    if (rs <= 0)
                    {
                        return View(model);
                    }

                    // Cập nhật lại IdPath và NamePath cho đơn vị
                    if (model.ParentId != oldParentId)
                    {
                        if (treaParent == null)
                        {
                            trea.IdPath = trea.Id.ToString();
                        }
                        else
                        {
                            trea.IdPath = $"{treaParent.IdPath}.{trea.Id}";
                            trea.NamePath = $"{treaParent.NamePath}/{trea.Name}";
                            treaParent.IsParent = true;

                            //check Type khiếu nại cha cũ
                            var parent = await UnitOfWork.ComplainTypeRepo.FirstOrDefaultAsync(x => x.Id == oldParentId);
                            if (parent != null)
                            {
                                var countChil = await UnitOfWork.ComplainTypeRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                                parent.IsParent = countChil != 0;
                                await UnitOfWork.ComplainTypeRepo.SaveAsync();
                            }
                        }

                        // Cập nhật lại IdPath của tất cả các đơn vị bên dưới
                        var listSubTrea = await UnitOfWork.ComplainTypeRepo.FindAsync(
                                    x => !x.IsDelete && x.IdPath.StartsWith(oldIdPath + "."));
                        if (listSubTrea != null)
                        {
                            listSubTrea.ForEach(o =>
                            {
                                o.IdPath = $"{trea.IdPath}{o.IdPath.Substring(oldIdPath.Length, o.IdPath.Length - oldIdPath.Length)}";
                                o.NamePath = $"{trea.NamePath}{o.NamePath.Substring(oldNamePath.Length, o.NamePath.Length - oldNamePath.Length)}";
                            });
                        }


                    }

                    await UnitOfWork.ComplainTypeRepo.SaveAsync();

                    TempData["Msg"] = $"Update successful Type khiếu nại \"<b>{trea.Name}</b>\"";

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }
            return RedirectToAction("ComplainType");
        }

        [HttpPost]
        [CheckPermission(EnumAction.Delete, EnumPage.ComplainType)]
        public async Task<ActionResult> DeleteComplainType(int id)
        {
            var rs = 1;
            var trea = await UnitOfWork.ComplainTypeRepo.SingleOrDefaultAsync(x => x.Id == id && !x.IsDelete);

            if (trea == null)
                return JsonCamelCaseResult(-1, JsonRequestBehavior.AllowGet);
            using (var transaction = UnitOfWork.DbContext.Database.BeginTransaction())
            {
                try
                {
                    trea.IsDelete = true;
                    UnitOfWork.ComplainTypeRepo.Update(trea);

                    rs = await UnitOfWork.ComplainTypeRepo.SaveAsync();

                    //check định khoản quỹ cha cũ
                    var parent = await UnitOfWork.ComplainTypeRepo.FirstOrDefaultAsync(x => x.Id == trea.ParentId);
                    if (parent != null)
                    {
                        var countChil =
                            await UnitOfWork.ComplainTypeRepo.CountAsync(x => x.ParentId == parent.Id && !x.IsDelete);
                        parent.IsParent = countChil != 0;
                        await UnitOfWork.ComplainTypeRepo.SaveAsync();
                    }
                    //Xóa định khoản quỹ con
                    if (trea.IsParent)
                    {
                        var listChil = await UnitOfWork.ComplainTypeRepo.FindAsync(x => x.IdPath.Contains(trea.IdPath) && !x.IsDelete);
                        foreach (var item in listChil)
                        {
                            item.IsDelete = true;
                        }
                        await UnitOfWork.ComplainTypeRepo.SaveAsync();
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ExceptionDispatchInfo.Capture(ex).Throw();
                    throw;
                }
            }

            return JsonCamelCaseResult(rs, JsonRequestBehavior.AllowGet);
        }

        public string ComplainTypeJsTree()
        {
            var list = UnitOfWork.DbContext.ComplainTypes.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId.ToString(),
                idPath = o.IdPath,
                state = new { opened = "", selected = "" },
            }).ToList();
            list.Add(new { id = "0", text = "Quản lý Type khiếu nại", parent = "#", idPath = "0", state = new { opened = "true", selected = "true" } });
            return JsonConvert.SerializeObject(list);
        }

        public string ComplainTypeActonJsTree()
        {

            var list = UnitOfWork.DbContext.ComplainTypes.Where(x => !x.IsDelete).Select(o => new
            {
                id = o.Id.ToString(),
                text = o.Name,
                parent = o.ParentId == 0 ? "#" : o.ParentId.ToString(),
                idPath = o.IdPath
            }).ToList();
            list.Add(new
            {
                id = "0",
                text = "QUẢN LÝ Type KHIẾU NẠI",
                parent = "#",
                idPath = "0"
            });
            return JsonConvert.SerializeObject(list);
        }
        #endregion

        [HttpPost]
        public JsonResult UpdateClaim(int productId, int orderType)
        {
            UnitOfWork.ComplainRepo.UpdateClaimForRefundDetail(productId, orderType);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderCustomerSearch(string keyword, int? page, int? customerId)
        {
            long totalRecord;
            keyword = String.IsNullOrEmpty(keyword) ? "" : keyword.Trim();
            //long totalStaff;
            List<dynamic> items = new List<dynamic>();
            var listOrder = UnitOfWork.OrderRepo.Find(
                out totalRecord,
                x => !x.IsDelete && x.Code.Contains(keyword) && (customerId == null || x.CustomerId == customerId),
                x => x.OrderByDescending(y => y.Created),
                 page ?? 1,
                   10
                ).ToList();

            items.AddRange(listOrder.Select(
                    x =>
                        new
                        {
                            id = x.Id,
                            code = x.Code,
                            text = x.CustomerName,
                            email = x.CustomerEmail,
                            phone = x.CustomerPhone,
                            systemName = x.SystemName,
                            websiteName = x.WebsiteName,
                            typeName = EnumHelper.GetEnumDescription<OrderType>(x.Type),
                            type = x.Type
                        }));

            return Json(new { incomplete_results = true, total_count = totalRecord, items }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetOrderDetail(string orderCodeCustomerId)
        {
            var orderCodeCustomer = "";
            var listOrderDetail = new List<OrderDetail>();
            if (orderCodeCustomerId != null)
            {
                orderCodeCustomer = RemoveCode(orderCodeCustomerId);

                var order = UnitOfWork.OrderRepo.FirstOrDefault(x => !x.IsDelete && x.Code == orderCodeCustomer);
                if (order == null)
                {

                }
                if (!orderCodeCustomerId.Contains("DEP"))
                {
                    listOrderDetail = UnitOfWork.OrderDetailRepo.Find(s => s.OrderId == order.Id && s.IsDelete == false).OrderBy(x=>x.Id).ToList();
                }
            }

            return Json(new { listOrderDetail }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult SaveNoteComplain(int complainOrderId, string note)
        //{

        //    var orderCodeCustomer = RemoveCode(orderCodeCustomerId);
        //    var listOrderDetail = new List<OrderDetail>();
        //    var order = UnitOfWork.OrderRepo.FirstOrDefault(x => !x.IsDelete && x.Code == orderCodeCustomer);
        //    if (order == null)
        //    {

        //    }
        //    if (!orderCodeCustomerId.Contains("DEP"))
        //    {
        //        listOrderDetail = UnitOfWork.OrderDetailRepo.Find(s => s.OrderId == order.Id).ToList();
        //    }

        //    return Json(new { listOrderDetail }, JsonRequestBehavior.AllowGet);
        //}
    }
}