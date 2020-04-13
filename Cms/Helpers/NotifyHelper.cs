using System;
using System.Collections.Generic;
using System.Linq;
using Cms.Hubs;
using Common.Emums;
using Library.DbContext.Results;
using Library.UnitOfWork;
using Microsoft.AspNet.SignalR;

namespace Cms.Helpers
{
    public static class NotifyHelper
    {

        #region Notify System To Client
        /// <summary>
        /// Tạo và gửi thông báo từ system xuống người dùng
        /// </summary>
        /// <param name="userId">UserId người nhận</param>
        /// <param name="title">Title thông báo</param>
        /// <param name="type">Type thông báo</param>
        /// <param name="content">Nội dung thông báo</param>
        /// <param name="url">Đường link khi click vào thông báo</param>
        public static void CreateAndSendNotifySystemToClient(int userId, string title, EnumNotifyType type, string content, string url = "")
        {
            var unitOfWork = new UnitOfWork();

            var now = DateTime.Now;

            var notifyId = unitOfWork.NotifyRealTimeRepo.InsertNotifySystem(userId, title, type, content, now, url);

            if (notifyId > 0)
            {
                SendNotifySystemToClient(userId, notifyId, title, type, now, url);
            }
        }

        /// <summary>
        /// Tạo và gửi thông báo từ system xuống người dùng 
        /// </summary>
        /// <param name="userId">UserId người nhận</param>
        /// <param name="title">Title thông báo</param>
        /// <param name="type">Type thông báo</param>
        /// <param name="content">Nội dung thông báo</param>
        /// <param name="group">Nhóm của thông báo</param>
        /// <param name="url">Đường link khi click vào thông báo</param>
        public static void CreateAndSendNotifySystemToClient(int userId, string title, EnumNotifyType type, string content, string group, string url = "")
        {
            var now = DateTime.Now;
            bool isInserted;

            var unitOfWork = new UnitOfWork();

            var notifyId = unitOfWork.NotifyRealTimeRepo.InsertNotifySystem(userId, title, type, content, now, group, url, out isInserted);

            if (notifyId > 0 && isInserted)
            {
                SendNotifySystemToClient(userId, notifyId, title, type, now, url);
            }
        }


        /// <summary>
        /// Gửi thông báo từ system xuống người dùng 
        /// </summary>
        /// <param name="userId">UserId người nhận</param>
        /// <param name="notifyId">Id thông báo</param>
        /// <param name="title">Title thông báo</param>
        /// <param name="type">Type thông báo</param>
        /// <param name="sendTime">Thời gian gửi</param>
        /// <param name="url">Đường link khi click vào thông báo</param>
        public static void SendNotifySystemToClient(int userId, long notifyId, string title, EnumNotifyType type, DateTime sendTime, string url)
        {
            var unitOfWork = new UnitOfWork();

            var userConnections = unitOfWork.UserRepo.GetUserConnectionByUserId(userId);

            if (!userConnections.Any())
            {
                return;
            }
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connection in userConnections)
            {
                hubContext.Clients.Client(connection.ConnectionId).notifySystem(notifyId, title, (byte)type, sendTime, url);
            }
        }


        /// <summary>
        /// Cập nhật lại số lượng thông báo chưa đọc
        /// </summary>
        /// <param name="userId">UserId người nhận</param>
        /// <param name="notifyIdReaded">Id thông báo</param>
        public static void UpdateTotalNotifySystemUnreadToClient(int userId, long notifyIdReaded)
        {
            var unitOfWork = new UnitOfWork();

            var userConnections = unitOfWork.UserRepo.GetUserConnectionByUserId(userId);

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connection in userConnections)
            {
                hubContext.Clients.Client(connection.ConnectionId).updateTotalNotifySystemUnread(notifyIdReaded);
            }
        }
        #endregion

        #region Message To Client

        public static void SendMessageToClient(int userId, long notifyId, string fullName, string title, string avatar, DateTime sendTime)
        {
            var unitOfWork = new UnitOfWork();
            var userConnections = unitOfWork.UserRepo.GetUserConnectionByUserId(userId);

            if (!userConnections.Any())
            {
                return;
            }
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connection in userConnections)
            {
                hubContext.Clients.Client(connection.ConnectionId).notifyMessage(notifyId, fullName, title, avatar, sendTime);
            }
        }

        /// <summary>
        /// Gửi thông báo xuống Client
        /// Cập nhật số lượng Message chưa đọc
        /// </summary>
        /// <param name="userId">Id người nhận</param>
        /// <param name="notifyIdReaded"></param>
        public static void UpdateTotalMessageToClient(int userId, long notifyIdReaded)
        {
            var unitOfWork = new UnitOfWork();
            var userConnections = unitOfWork.UserRepo.GetUserConnectionByUserId(userId);

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connection in userConnections)
            {
                hubContext.Clients.Client(connection.ConnectionId).updateTotalMessageUnread(notifyIdReaded);
            }
        }
        /// <summary>
        /// Thông báo gửi Message đến client
        /// Có Email gửi đến
        /// </summary>
        /// <param name="messageUser">Danh sách User cần gửi đến</param>
        /// <param name="fullName">Họ và tên người gửi</param>
        /// <param name="title">Title của thông báo</param>
        /// <param name="avatar">Ảnh người gửi</param>
        /// <param name="sendTime">Thời gian gửi</param>
        public static void SendMessageToClient(List<MessageSendResults> messageUser, string fullName, string title, string avatar, DateTime sendTime)
        {
            var unitOfWork = new UnitOfWork();

            var userConnections = unitOfWork.UserRepo.GetUserConnectionByUserId(messageUser.Select(m => m.UserId).ToList());

            if (!userConnections.Any())
            {
                return;
            }
            var dictionary = messageUser.ToDictionary(m => m.UserId.ToString(), m => m.MessageId);

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            foreach (var connection in userConnections)
            {
                hubContext.Clients.Client(connection.ConnectionId).notifyMessage(dictionary[connection.UserId.ToString()], fullName, title, avatar, sendTime);
            }
        }
        #endregion
    }
}