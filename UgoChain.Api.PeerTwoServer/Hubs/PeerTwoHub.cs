using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoSever.Models;
using UgoChain.PeerTwo.Features;
using UgoChain.PeerTwo.Features.Wallet;

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
            //share this peer's TransactionPool
            Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.PeerTwo, TransactionPool.Instance.Transactions);

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

            (bool, string) response = _blockchain.ReplaceChain(new Blockchain() { Chain = chain });

            if (response.Item1)
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.PeerTwo, $"Peer two - {response.Item2}");

            }
            else
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.PeerTwo, $"Peer two - {response.Item2}");

            }

        }

        public void UpdateTransactionPool(List<Transaction> incomingTransactions)
        {
            int initialTransactionCount = TransactionPool.Instance.Transactions.Count;
            for (int i = 0; i < incomingTransactions.Count; i++)
            {
                TransactionPool.Instance.UpdateOrAddTransaction(incomingTransactions[i]);
            }
            int finalTransactionCount = TransactionPool.Instance.Transactions.Count;
            Clients.All.SendAsync("AnnounceTransactionPoolUpdate", (int)PeerColorsEnum.PeerTwo, $"Peer two - {finalTransactionCount - initialTransactionCount} Transactions Detected and Added");
        }
    }
}
