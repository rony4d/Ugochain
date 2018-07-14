using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.Models;

namespace UgoChain.Api.Hubs
{
    public class PeersHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            ConnectionData connectionData = new ConnectionData();
            connectionData.ConnectionId = Context.ConnectionId;
            connectionData.ConnectionTime = DateTime.Now;
            var httpContext = Context.GetHttpContext();
            connectionData.Payload = $"Local Port: {httpContext.Connection.LocalPort} \n" +
                $" Local IP Address: {httpContext.Connection.LocalIpAddress} \n" +
                $" Connection Id: { httpContext.Connection.Id} \n" +
                $" Project Server Name: PeersHub";
            ConnectionList.AddUser(connectionData);

            return Clients.All.SendAsync("ActiveConnections", ConnectionList.GetActiveConnections());
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
        //public Task SendMessage(ChatMessage message)
        //{
        //    string timestamp = DateTime.Now.ToShortTimeString();
        //    return Clients.All.SendAsync("ReceiveMessage", timestamp, message.User, message.Message);
        //}
    }
}
