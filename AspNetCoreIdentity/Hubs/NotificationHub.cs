using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace AspNetCoreIdentity.Hubs
{
    [HubName("server-notif")]
    public class NotificationHub : Hub
    {
        public override Task OnConnected()
        {
            var userName = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, userName);
            return base.OnConnected();
        }

        public void Join()
        {
            var userName = Context.User.Identity.Name;
            var userId = Context.User.Identity.GetUserId();
            //Clients.Group(userName).logInfo($"User '{userName}' is connected.");
            Clients.User(userName).logInfo($"User '{userName}' is connected. ID: {userId}");
        }

        public void RefreshPage(string userName)
        {
            //Clients.Group(userId).refreshPage();
            Clients.User(userName).refreshPage();
        }

    }
}
