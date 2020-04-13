using System;
using System.Collections.Generic;
using System.Linq;
using Common.Helper;
using Ganss.XSS;
using Library.Models;
using Library.UnitOfWork;
using Newtonsoft.Json;

namespace Cms.Helpers
{
    public class MessageHelper
    {

        /// <summary>
        /// Hệ thông gửi email cho nhân viên
        /// </summary>
        /// <param name="toIds">Ids to</param>
        /// <param name="title">Title</param>
        /// <param name="content">Nội dung</param>
        /// <param name="userState">UserId hiện tại (Null lấy User system)</param>
        /// <param name="ccIds">Ids cc</param>
        /// <param name="bccId">Ids bcc</param>
        /// <returns>Mã lỗi</returns>
        public static int SystemSendMessage(List<int> toIds, string title, string content, UserState userState = null, List<int> ccIds = null, List<int> bccId = null)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                int userId;
                string avatar;
                string fullName;
                string userName;

                if (userState == null)
                {
                    var systemUser = unitOfWork.UserRepo.FirstOrDefault(x => x.IsDelete == false && x.IsSystem);

                    if (systemUser == null)
                        return -1;

                    userId = systemUser.Id;
                    avatar = systemUser.Avatar;
                    fullName = systemUser.FullName;
                    userName = systemUser.UserName;
                }
                else
                {
                    userId = userState.UserId;
                    avatar = userState.Avatar;
                    fullName = userState.FullName;
                    userName = userState.UserName;
                }


                if (toIds == null)
                    return -2;

                var fromUser =
                    JsonConvert.SerializeObject(
                        new
                        {
                            fullName,
                            userName,
                            userId
                        });

                var messageId = unitOfWork.MessageRealTimeRepo.InsertTemp(fromUser, userId, avatar,
                    $"{fullName} {userName}", null);

                var strToIds = $";{string.Join(";", toIds)};";
                var strCcIds = ccIds == null ? "" : $";{string.Join(";", ccIds)};";
                var strBccIds = bccId == null ? "" : $";{string.Join(";", bccId)};";

                var listUserTo = unitOfWork.UserRepo.GetBySpec(
                    u => new { userId = u.Id, userName = u.UserName, fullName = u.FullName, email = u.Email },
                    u => !u.IsDelete && strToIds.Contains(";" + u.Id + ";"));

                var listUserCc = unitOfWork.UserRepo.GetBySpec(
                    u => new { userId = u.Id, userName = u.UserName, fullName = u.FullName, email = u.Email },
                    u => !u.IsDelete && strCcIds.Contains(";" + u.Id + ";"));

                var listUserBcc = unitOfWork.UserRepo.GetBySpec(
                    u => new { userId = u.Id, userName = u.UserName, fullName = u.FullName, email = u.Email },
                    u => !u.IsDelete && strBccIds.Contains(";" + u.Id + ";"));

                var allUser = listUserTo.Concat(listUserCc).Concat(listUserBcc).ToList();

                var unsignTitle = MyCommon.Ucs2Convert(
                    $"{title} {userName} {fullName}" +
                    $" {string.Join(" ", allUser.Select(u => u.userName).ToList())}" +
                    $" {string.Join(" ", allUser.Select(u => u.fullName).ToList())}");

                int result;

                var htmlSanitizer = new HtmlSanitizer();
                htmlSanitizer.AllowedAttributes.Add("class");
                htmlSanitizer.AllowedAttributes.Add("style");

                var messageUser = unitOfWork.MessageRealTimeRepo.Send(messageId, userId,
                    JsonConvert.SerializeObject(listUserTo),
                    listUserCc.Any() ? JsonConvert.SerializeObject(listUserCc) : null,
                    listUserBcc.Any() ? JsonConvert.SerializeObject(listUserBcc) : null,
                    title, unsignTitle, htmlSanitizer.Sanitize(content),
                    allUser.Select(x => x.userId).ToList(), out result);

                if (result > 0)
                {
                    NotifyHelper.SendMessageToClient(messageUser, fullName, title,
                        avatar, DateTime.Now);
                }
            }
            return 1;
        }
    }
}