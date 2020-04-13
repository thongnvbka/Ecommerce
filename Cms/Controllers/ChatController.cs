using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Emums;
using Library.DbContext.Entities;
using Library.UnitOfWork;
using Common.Constant;

namespace Cms.Controllers
{
    public class ChatController : BaseController
    {
        [HttpPost]
        public async Task<JsonResult> GetChat(int orderId, int orderType)
        {
            var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType);

            var listChatCustomer = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType && (bool)!x.IsRead && x.CustomerId != null);
            foreach (var item in listChatCustomer)
            {
                item.IsRead = true;
            }
            await UnitOfWork.OrderCommentRepo.SaveAsync();

            return Json(listChat, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> AddChat(int orderId, int orderType, string text, byte? type)
        {
            if (UserState.OfficeType == (byte) OfficeType.Warehouse)
            {
                return Json(new { status = MsgType.Error, msg = "Warehouse staff are not chatting with customer!" }, JsonRequestBehavior.AllowGet);
            }

            dynamic order = null;
            var typeOrderComplain = 0;
            switch (orderType)
            {
                case (int)OrderType.Order:
                    order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Order);
                    typeOrderComplain = 0;
                    break;
                case (int)OrderType.Deposit:
                    order = await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Deposit);
                    typeOrderComplain = 1;
                    break;
                case (int)OrderType.Source:
                    order = await UnitOfWork.SourceRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId) ??
                            (dynamic)await UnitOfWork.OrderRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == orderId && x.Type == (byte)OrderType.Source);
                    typeOrderComplain = 2;
                    break;
            }

            type = type ?? (byte)CommentType.Text;

            var user = await UnitOfWork.UserRepo.FirstOrDefaultAsNoTrackingAsync(x => x.Id == UserState.UserId);

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
                UserId = UserState.UserId,
                UserOffice = UserState.OfficeIdPath,
                UserPhone = user.Phone,
                UserName = UserState.FullName,
                IsRead = false,
                SystemId = order.SystemId,
                SystemName = order.SystemName,
                CommentType = type
            };
            // Gửi thông báo Notification cho khách hàng
            var notification = new Notification()
            {
                SystemId = order.SystemId,
                SystemName = order.SystemName,
                CustomerId = order.CustomerId,
                CustomerName = order.CustomerName,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                OrderId = order.Id,
                OrderType = typeOrderComplain,
                IsRead = false,
                Title = string.Format("{0} ข้อความใหม่ของออเกอร์ {1}: \"{2}\"",""/* UserState.FullName*/, order.Code, (text.Length > 30 ? text.Substring(0,30) : text)),
                Description = string.Format("{0} ข้อความใหม่ของออเกอร์ {1}: \"{2}\"","" /*UserState.FullName*/, order.Code, text)
            };
            UnitOfWork.NotificationRepo.Add(notification);
            int groupId;
            var check = CheckGroup(orderId, orderType, out groupId);
            if (check)
            {
                chat.GroupId = groupId;
            }

            UnitOfWork.OrderCommentRepo.Add(chat);
            await UnitOfWork.OrderRepo.SaveAsync();

            if (!check)
            {
                chat.GroupId = (int)chat.Id;
                await UnitOfWork.OrderRepo.SaveAsync();
            }

            var listChat = await UnitOfWork.OrderCommentRepo.FindAsync(x => x.OrderId == orderId && x.OrderType == orderType);

            return Json(new { status = MsgType.Success, msg = "Success!", listChat }, JsonRequestBehavior.AllowGet);
        }

        private bool CheckGroup(int orderId, int orderType, out int groupId)
        {
            var timeOld = DateTime.Now.AddMinutes(-2);

            var chat = UnitOfWork.OrderCommentRepo.Entities.Where(x => x.OrderId == orderId && x.OrderType == orderType && x.UserId > 0 && x.UserId == UserState.UserId && x.CreateDate >= timeOld).OrderByDescending(x => x.Id).FirstOrDefault();
            groupId = chat?.GroupId ?? 0;
            return chat?.UserId != null;
        }
    }
}