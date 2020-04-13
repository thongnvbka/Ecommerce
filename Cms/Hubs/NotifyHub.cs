using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common.Helper;
using Library.Models;
using Library.UnitOfWork;
using Microsoft.AspNet.SignalR;

namespace Cms.Hubs
{
    [Authorize]
    public class NotifyHub : Hub
    {
        //public string GetClaim(List<Claim> claims, string key)
        //{
        //    var claim = claims.FirstOrDefault(c => c.Type == key);

        //    return claim?.Value;
        //}

        public override Task OnConnected()
        {
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var unitOfWork = new UnitOfWork();

            unitOfWork.UserRepo.InsertUserConnection(userState.UserId, userState.UserName, userState.FullName, userState.OfficeId,
                userState.OfficeName, userState.TitleName, userState.Avatar, Context.ConnectionId,
               userState.SessionId, MyCommon.Ucs2Convert($"{userState.UserName} {userState.FullName}"));

            return base.OnConnected();
        }

        // Attach file to single instant message
        public void AttachFileToSingleChat(object itemArray, string userId)
        {
            Clients.Group(userId).ListenAttachFileToSingleChat(itemArray);
        }
        
        public override Task OnReconnected()
        {
            Clients.All.LogReconnected("reconnected");
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userState = new UserState();

            if (Context.User is ClaimsPrincipal)
            {
                var user = Context.User as ClaimsPrincipal;
                var claims = user.Claims.ToList();

                //var userStateString = GetClaim(claims, "userState");

                var claim = claims.FirstOrDefault(c => c.Type == "userState");

                var userStateString = claim?.Value;

                if (!string.IsNullOrEmpty(userStateString))
                    userState.FromString(userStateString);
            }

            var unitOfWork = new UnitOfWork();

            unitOfWork.UserRepo.DeleteUserConnection(Context.ConnectionId);


            unitOfWork.UserRepo.GetUserConnectionByUserId(userState.UserId, u => u.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }
    }
}