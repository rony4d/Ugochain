using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.Models;
using UgoChain.Features;
using UgoChain.Features.Wallet;

namespace UgoChain.Api.Hubs
{
    public class PeersHub:Hub
    {
        IBlockchain _blockchain;
        public PeersHub(IBlockchain blockchain)
        {
            _blockchain = blockchain;
        }
        /// <summary>
        /// When you connect to any peer:
        /// 1. Get the info of the peer 
        /// 2. Get the current blockchain for that peer and sync with your own blockchain
        /// 3. Get the Transaction Pool of that peer and add to your own transaction pool
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            ConnectionData connectionData = new ConnectionData();
            connectionData.ConnectionId = Context.ConnectionId;
            connectionData.ConnectionTime = DateTime.Now;
            connectionData.PeerCode = (int)PeersEnum.Main;
            var httpContext = Context.GetHttpContext();
            connectionData.Payload = $"Local Port: {httpContext.Connection.LocalPort} \n" +
                $" Local IP Address: {httpContext.Connection.LocalIpAddress} \n" +
                $" Connection Id: { httpContext.Connection.Id} \n" +
                $" Project Server Name: PeersHub";
            ConnectionList.AddUser(connectionData);

            //share this peers' info with the connected client 
            Clients.All.SendAsync("ActiveConnections", ConnectionList.GetActiveConnections());

            //share this peer's TransactionPool
            Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.Main, TransactionPool.Instance.Transactions);

            //share this peer's blockchain with the connected client
            return Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.Main, _blockchain.Chain);


        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Update this peer's blockchain with new chain from another peer and update 
        /// all other connected peers
        /// Announce that new blocks have been added
        /// </summary>
        /// <param name="chain"></param>
        /// <returns></returns>
        public void SyncChain(List<Block> chain)
        {


            (bool, string) response = _blockchain.ReplaceChain(new Blockchain() { Chain = chain });

            if (response.Item1)
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.Main, $"Main peer - {response.Item2}");
            }
            else
            {
                Clients.All.SendAsync("AnnounceFreshBlock", (int)PeerColorsEnum.Main, $"Main peer - {response.Item2}");
            }
            //if (response.Item1)
            //{
            //    Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.Main, _blockchain.Chain);

            //}
        }

        public void UpdateTransactionPool(List<Transaction> incomingTransactions)
        {
            int initialTransactionCount = TransactionPool.Instance.Transactions.Count;
            for (int i = 0; i < incomingTransactions.Count; i++)
            {
                TransactionPool.Instance.UpdateOrAddTransaction(incomingTransactions[i]);
            }
            int finalTransactionCount = TransactionPool.Instance.Transactions.Count;
            Clients.All.SendAsync("AnnounceTransactionPoolUpdate", (int)PeerColorsEnum.Main, $"Main peer - {finalTransactionCount - initialTransactionCount} Transactions Detected and Added");
        }

        /// <summary>
        /// Tells other peers to clear Transaction Pool when new block is mined
        /// </summary>
        public void ClearTransactionPoolPeerToPeer()
        {
            TransactionPool.Instance.Transactions.Clear();
        }
    }
}
