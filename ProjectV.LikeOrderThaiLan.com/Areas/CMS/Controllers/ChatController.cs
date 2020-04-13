using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using ProjectV.LikeOrderThaiLan.com.Controllers;
using ResourcesLikeOrderThaiLan;

namespace ProjectV.LikeOrderThaiLan.com.Areas.CMS.Controllers
{
    public class ChatController : BaseController
    {
        // GET: CMS/Chat
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetChat(int orderId, int orderType)
        {
            var listChatDetail = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType);

            var listChatCustomer = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType && (bool)!x.IsRead && x.UserId != null);
            var listChat = new List<ChatDetail>();
            foreach (var item in listChatCustomer)
            {
                item.IsRead = true;
            }
            foreach (var item in listChatDetail)
            {
                string avatarCustomer = "";
                string avatarUser = "";
                if (item.UserId > 0)
                {
                    var user = UnitOfWork.UserRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.UserId);
                    if (user != null)
                    {
                        avatarUser = user.Avatar;
                    }
                }
                if (item.CustomerId > 0)
                {
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.CustomerId);
                    if (customer != null)
                    {
                        avatarCustomer = customer.Avatar;
                    }
                }
                listChat.Add(new ChatDetail
                {
                    Id = item.Id,
                    Description = item.Description,
                    CustomerId = item.CustomerId,
                    UserId = item.UserId,
                    CreateDate = item.CreateDate,
                    IsRead = item.IsRead,
                    CustomerName = item.CustomerName,
                    UserName = item.UserName,
                    OrderType = item.OrderType,
                    CommentType = item.CommentType,
                    UserOffice = item.UserOffice,
                    GroupId = item.GroupId,
                    AvatarCustomer = avatarCustomer,
                    AvatarUser = avatarUser
                });
            }
            await UnitOfWork.OrderCommentRepo.SaveAsync();

            return Json(listChat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetTicketChat(int complainId)
        {
            var listChatDetail = await UnitOfWork.ComplainUserRepo.FindAsync(x =>
                                                                                x.ComplainId == complainId
                                                                                && x.GroupId != null);

            var listChatCustomer = await UnitOfWork.ComplainUserRepo.FindAsync(x =>
                                                                                x.ComplainId == complainId
                                                                                && (bool)!x.IsRead
                                                                                && ((bool)!x.IsCare || x.IsCare == null)
                                                                                && (bool)!x.IsInHouse
                                                                                );
            var listChat = new List<ChatDetail>();
            foreach (var item in listChatCustomer)
            {
                item.IsRead = true;
            }
            foreach (var item in listChatDetail)
            {
                string avatarCustomer = "";
                string avatarUser = "";
                if (item.UserId > 0)
                {
                    var user = UnitOfWork.UserRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.UserId);
                    if (user != null)
                    {
                        avatarUser = user.Avatar;
                    }
                }
                if (item.CustomerId > 0)
                {
                    var customer = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.CustomerId);
                    if (customer != null)
                    {
                        avatarCustomer = customer.Avatar;
                    }
                }
                listChat.Add(new ChatDetail
                {
                    Id = item.Id,
                    Description = item.Content,
                    CustomerId = item.CustomerId,
                    UserId = item.UserId,
                    CreateDate = item.CreateDate,
                    IsRead = item.IsRead,
                    CustomerName = item.CustomerName,
                    UserName = item.UserName,
                    OrderType = 0,
                    CommentType = item.CommentType,
                    UserOffice = item.OfficeIdPath,
                    GroupId = item.GroupId,
                    AvatarCustomer = avatarCustomer,
                    AvatarUser = avatarUser
                });
            }
            await UnitOfWork.OrderCommentRepo.SaveAsync();

            return Json(listChat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddChat(int orderId, int orderType, string text, byte? type)
        {
            dynamic order = null;
            switch (orderType)
            {
                case (int)OrderType.Order:
                    order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Order);
                    break;

                case (int)OrderType.Deposit:
                    order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Deposit);
                    break;

                case (int)OrderType.Source:
                    order = await UnitOfWork.SourceRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId) ??
                            (dynamic)await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Source);
                    break;
            }

            type = type ?? (byte)CommentType.Text;

            var user = await UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == CustomerState.Id);

            if (order == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var chat = new OrderComment()
            {
                OrderId = orderId,
                OrderType = (byte)orderType,
                Description = text,
                CreateDate = DateTime.Now,
                CustomerId = CustomerState.Id,
                CustomerName = CustomerState.FullName,
                IsRead = false,
                SystemId = order.SystemId,
                SystemName = order.SystemName,
                CommentType = type
            };

            int groupId;
            var check = CheckGroup(orderId, orderType, out groupId);
            if (check)
            {
                chat.GroupId = groupId;
            }

            UnitOfWork.OrderCommentRepo.Add(chat);
            await UnitOfWork.OrderCommentRepo.SaveAsync();

            if (!check)
            {
                chat.GroupId = (int)chat.Id;
                await UnitOfWork.OrderCommentRepo.SaveAsync();
            }

            var listChat = new List<ChatDetail>();
            var listChatDetail = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType);
            foreach (var item in listChatDetail)
            {
                string avatarCustomer = "";
                string avatarUser = "";
                if (item.UserId > 0)
                {
                    var user1 = UnitOfWork.UserRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.UserId);
                    if (user1 != null)
                    {
                        avatarUser = user1.Avatar;
                    }
                }
                if (item.CustomerId > 0)
                {
                    var customer1 = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.CustomerId);
                    if (customer1 != null)
                    {
                        avatarCustomer = customer1.Avatar;
                    }
                }
                listChat.Add(new ChatDetail
                {
                    Id = item.Id,
                    Description = item.Description,
                    CustomerId = item.CustomerId,
                    UserId = item.UserId,
                    CreateDate = item.CreateDate,
                    IsRead = item.IsRead,
                    CustomerName = item.CustomerName,
                    UserName = item.UserName,
                    OrderType = item.OrderType,
                    CommentType = item.CommentType,
                    UserOffice = item.UserOffice,
                    GroupId = item.GroupId,
                    AvatarCustomer = avatarCustomer,
                    AvatarUser = avatarUser
                });
            }
            //Them notifi o duoi cms
            var tmpTitle = "";
            var tmpContent = "";
            var tmpGroup = "";
            var tmpUrl = "";

            var now = DateTime.Now;
            bool isInserted;
            if (order.UserId != null)
            {
                switch (orderType)
                {
                    case (int)OrderType.Order:
                        tmpTitle = $"{CustomerState.FullName}" + Resource.CommentDonHang + "ORD{order.Code}";
                        //tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng ORD{order.Code}";
                        tmpUrl = $"ORD{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Purchase/Order#" + tmpUrl + "\" target=\"_blank\"> " + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"ORD{order.Code}";
                        break;

                    case (int)OrderType.Deposit:
                        tmpTitle = $"{CustomerState.FullName} " + Resource.CommentDonHang + $" DEP{order.Code}";
                        // tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng DEP{order.Code}";
                        tmpUrl = $"DEP{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Purchase/Order#" + tmpUrl + "\" target=\"_blank\"> " + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"DEP{order.Code}";
                        break;

                    case (int)OrderType.Source:
                        tmpTitle = $"{CustomerState.FullName} " + Resource.CommentDonHang + $"SRC{order.Code}";
                        // tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng SRC{order.Code}";
                        tmpUrl = $"SRC{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Purchase/Order#" + tmpUrl + "\" target=\"_blank\"> " + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"SRC{order.Code}";
                        break;
                }
                UnitOfWork.NotifyRealTimeRepo.InsertNotifySystem(order.UserId, tmpTitle, EnumNotifyType.Info, tmpContent, now, tmpGroup, "/Purchase/Order#" + tmpUrl, out isInserted);
            }
            if (order.CustomerCareUserId != null)
            {
                switch (orderType)
                {
                    case (int)OrderType.Order:
                        tmpTitle = $"{CustomerState.FullName} " + Resource.CommentDonHang + $" ORD{order.Code}";
                        //tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng ORD{order.Code}";
                        tmpUrl = $"ORD{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Ticket#" + tmpUrl + "\" target=\"_blank\">" + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"ORD{order.Code}";
                        break;

                    case (int)OrderType.Deposit:
                        tmpTitle = $"{CustomerState.FullName} " + Resource.CommentDonHang + $" DEP{order.Code}";
                        //tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng DEP{order.Code}";
                        tmpUrl = $"DEP{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Ticket#" + tmpUrl + "\" target=\"_blank\">" + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"ORD{order.Code}";
                        break;

                    case (int)OrderType.Source:
                        tmpTitle = $"{CustomerState.FullName} " + Resource.CommentDonHang + $" SRC{order.Code}";
                        //tmpTitle = $"{CustomerState.FullName} comment cho đơn hàng SRC{order.Code}";
                        tmpUrl = $"SRC{order.Code}";
                        tmpContent = text + ".<a href=\"" + "/Ticket#" + tmpUrl + "\" target=\"_blank\">" + Resource.XemChiTiet + "</a>";
                        //tmpGroup = $"ORD{order.Code}";
                        break;
                }
                UnitOfWork.NotifyRealTimeRepo.InsertNotifySystem(order.CustomerCareUserId, tmpTitle, EnumNotifyType.Info, tmpContent, now, tmpGroup, "/Ticket#" + tmpUrl, out isInserted);
            }
            return Json(listChat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddTicketChat(int complainId, string text, byte? type)
        {
            dynamic complain = await UnitOfWork.ComplainRepo.FirstOrDefaultAsync(x => x.Id == complainId);

            type = type ?? (byte)CommentType.Text;

            var user = await UnitOfWork.CustomerRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == CustomerState.Id);

            if (complain == null)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var chat = new ComplainUser()
            {
                ComplainId = complainId,
                Content = text,
                CustomerId = CustomerState.Id,
                CustomerName = CustomerState.FullName,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                IsRead = false,
                CommentType = type,
                SystemId = complain.SystemId,
                SystemName = complain.SystemName,
            };

            int groupId;
            var check = CheckTicketGroup(complainId, out groupId);
            if (check)
            {
                chat.GroupId = groupId;
            }
            UnitOfWork.ComplainUserRepo.Add(chat);
            await UnitOfWork.ComplainUserRepo.SaveAsync();
            if (!check)
            {
                chat.GroupId = (int)chat.Id;
            }

            await UnitOfWork.ComplainUserRepo.SaveAsync();

            var listChat = new List<ChatDetail>();
            var listChatDetail = await UnitOfWork.ComplainUserRepo.FindAsync(x => x.ComplainId == complainId
                                                                                && x.GroupId != null
                                                                                );
            foreach (var item in listChatDetail)
            {
                string avatarCustomer = "";
                string avatarUser = "";
                if (item.UserId > 0)
                {
                    var user1 = UnitOfWork.UserRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.UserId);
                    if (user1 != null)
                    {
                        avatarUser = user1.Avatar;
                    }
                }
                if (item.CustomerId > 0)
                {
                    var customer1 = UnitOfWork.CustomerRepo.FirstOrDefault(s => !s.IsDelete && s.Id == item.CustomerId);
                    if (customer1 != null)
                    {
                        avatarCustomer = customer1.Avatar;
                    }
                }
                listChat.Add(new ChatDetail
                {
                    Id = item.Id,
                    Description = item.Content,
                    CustomerId = item.CustomerId,
                    UserId = item.UserId,
                    CreateDate = item.CreateDate,
                    IsRead = item.IsRead,
                    CustomerName = item.CustomerName,
                    UserName = item.UserName,
                    CommentType = item.CommentType,
                    UserOffice = item.OfficeIdPath,
                    GroupId = item.GroupId,
                    AvatarCustomer = avatarCustomer,
                    AvatarUser = avatarUser
                });
            }
            //Them notifi o duoi cms
            var tmpTitle = "";
            var tmpContent = "";
            var tmpGroup = "";
            tmpTitle = $"{CustomerState.FullName} " + Resource.CommentChoKN + $" {complain.Code}";
            //tmpTitle = $"{CustomerState.FullName} comment cho ticket {complain.Code}";
            tmpContent = text + ".<a href=\"" + "/Ticket/#TK" + complain.Code + "\" target=\"_blank\"> " + Resource.XemChiTiet + "</a>";
            //tmpGroup = $"Ticket_{complain.Code}";

            var now = DateTime.Now;
            bool isInserted;

            //Check điều kiện có phải là người xử lý chính hay không
            var tmpComplainUser = UnitOfWork.ComplainUserRepo.Find(m => m.ComplainId == complainId && m.UserId != null).Select(m => m.UserId).Distinct().ToList();
            foreach (var item in tmpComplainUser)
            {
                if (item.Value != null && item.Value > 0)
                {
                    UnitOfWork.NotifyRealTimeRepo.InsertNotifySystem(item.Value, tmpTitle, EnumNotifyType.Info, tmpContent, now, tmpGroup, "/Ticket/#TK" + complain.Code, out isInserted);
                }
            }
            return Json(listChat, JsonRequestBehavior.AllowGet);
        }

        private bool CheckGroup(int orderId, int orderType, out int groupId)
        {
            var timeOld = DateTime.Now.AddMinutes(-2);

            var chat = UnitOfWork.OrderCommentRepo.Entities.Where(x => x.OrderId == orderId && x.OrderType == orderType && x.CustomerId > 0 && x.CreateDate >= timeOld).OrderByDescending(x => x.Id).FirstOrDefault();
            groupId = chat?.GroupId ?? 0;
            return chat?.CustomerId != null;
        }

        private bool CheckTicketGroup(int complainId, out int groupId)
        {
            var timeOld = DateTime.Now.AddMinutes(-2);

            var chat = UnitOfWork.ComplainUserRepo.Entities.Where(x => x.ComplainId == complainId && x.UserId == null && x.CreateDate >= timeOld).OrderByDescending(x => x.Id).FirstOrDefault();
            groupId = chat?.GroupId ?? 0;
            return chat?.CustomerId != null;
        }
    }
}