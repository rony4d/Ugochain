using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoSever.Models;
using UgoChain.PeerTwo.Features;

namespace UgoChain.Api.PeerTwoSever.Hubs
{
    public class PeerTwoHub:Hub
    {
        IBlockchain _blockchain;
        public PeerTwoHub(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }

        public override Task OnConnectedAsync()
        {
            ConnectionData connectionData = new ConnectionData();
            connectionData.ConnectionId = Context.ConnectionId;
            connectionData.ConnectionTime = DateTime.Now;
            connectionData.PeerCode = (int)PeersEnum.PeerTwo;

            var httpContext = Context.GetHttpContext();
            connectionData.Payload = $"Local Port: {httpContext.Connection.LocalPort} \n" +
                $" Local IP Address: {httpContext.Connection.LocalIpAddress} \n" +
                $" Connection Id: { httpContext.Connection.Id} \n" +
                $" Project Server Name: PeerTwoHub";
            ConnectionList.AddUser(connectionData);

            //share this peers' info with the connected client 
            Clients.All.SendAsync("ActiveConnections", ConnectionList.GetActiveConnections());

            //share this peers' blockchain with the connected client
            return Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerTwo, _blockchain.Chain);
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }


        /// <summary>
        /// Update this peers' blockchain with new chain from another peer and update 
        /// all other connected peers
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public void SyncChain(List<Block> chain)
        {

            //return Clients.All.SendAsync("ActiveConnections", ConnectionList.GetActiveConnections());

            (bool, string) response = _blockchain.ReplaceChain(new Blockchain() { Chain = chain });

            if (response.Item1)
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.PeerOne, $"Peer two - {response.Item2}");

            }
            else
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.PeerOne, $"Peer two - {response.Item2}");

            }


            //Clients.All.SendAsync("AnnouncePeerSync", (int)PeerColorsEnum.PeerTwo, "Peer Two Sync Duo Memento!!!");

            //if (response.Item1)
            //{
            //    Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.Main, _blockchain.Chain);

            //}
        }
    }
}
