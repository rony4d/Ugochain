using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoClient.Models;
using UgoChain.PeerTwo.Features;

namespace UgoChain.Api.PeerTwoClient
{
    class Program
    {
        public static List<HubConnection> hubConnections;

        static void Main(string[] args)
        {
            hubConnections = new List<HubConnection>();
            Console.Title = "Ugochain Peer Two Server";
            HubConnection peerOneHubConnection = new HubConnectionBuilder()
                                           .WithUrl("https://localhost:44353/PeerOneHub")
                                           .Build();
            HubConnection peerTwoHubConnection = new HubConnectionBuilder()
                                          .WithUrl("https://localhost:44344/PeerTwoHub")
                                          .Build();

            HubConnection mainPeerConnection = new HubConnectionBuilder()
                                            .WithUrl("https://localhost:44378/PeersHub")
                                            .Build();

            ConfigureConnection(peerOneHubConnection).GetAwaiter().GetResult();
            ConfigureConnection(peerTwoHubConnection).GetAwaiter().GetResult();
            ConfigureConnection(mainPeerConnection).GetAwaiter().GetResult();

            hubConnections.Add(peerOneHubConnection);
            hubConnections.Add(peerTwoHubConnection);
            hubConnections.Add(mainPeerConnection);


            Console.ReadKey();
        }

        public static async Task ConfigureConnection(HubConnection hubConnection)
        {


            hubConnection.On<int, string, string, string>("ReceiveMessage", (peerCode, timestamp, user, message) =>
            {
                SetConsoleDefaults(peerCode);

                Console.WriteLine($"{timestamp} User: {user}, Message: {message}");
            });

            hubConnection.On<List<ConnectionData>>("ActiveConnections", (connections) =>
            {
                foreach (var connectionData in connections)
                {
                    SetConsoleDefaults(connectionData.PeerCode);

                    Console.WriteLine($"{connectionData.ConnectionTime} Connection ID: {connectionData.ConnectionId}, Payload: {connectionData.Payload}");

                }
            });

            //Announce fresh block when a block is mined from any peer
            hubConnection.On<int, string>("AnnounceFreshBlock", (peerCode, announcement) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(announcement);
            });

            //Announce sync when you receive this message
            //hubConnection.On<int, string>("AnnouncePeerSync", (peerCode, announcement) =>
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(announcement);
            //});

            // when you receive the current blockchain, update the peers with it
            hubConnection.On<int, List<Block>>("ReceiveCurrentBlockchain", (peerCode, blockchain) =>
            {
                string blockchainStr = JsonConvert.SerializeObject(blockchain);

                SetConsoleDefaults(peerCode);

                Console.WriteLine($"Current Chain JSON {blockchainStr} \n" +
                    $" Block Count: {blockchain.Count}\n " +
                    $" Newest Block Data: {blockchain.LastOrDefault().Data} \n" +
                    $" Newest Block Hash: {blockchain.LastOrDefault().Hash}");

                for (int i = 0; i < hubConnections.Count; i++)
                {
                    hubConnections[i].InvokeAsync("SyncChain", blockchain);
                }
                //hubConnections.ForEach(hubconnection => hubConnection.InvokeAsync("SyncChain", blockchain));

            });

            // when you receive the current transaction pool update other peers' transaction pools with it

            hubConnection.On<int, List<Features.Wallet.Transaction>>("ReceiveTransactions", (peerCode, transactions) =>
            {
                string transactionsStr = JsonConvert.SerializeObject(transactions);

                SetConsoleDefaults(peerCode);

                Console.WriteLine($"Current Transactions JSON {transactionsStr} \n" +
                    $" Transaction Pool Count: {transactions.Count}\n ");
                for (int i = 0; i < hubConnections.Count; i++)
                {
                    hubConnections[i].InvokeAsync("UpdateTransactionPool", transactions);
                }

            });
            //Announce transaction pool update
            hubConnection.On<int, string>("AnnounceTransactionPoolUpdate", (peerCode, announcement) =>
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(announcement);
            });

            //Tell all peers to clear transaction pools
            hubConnection.On<int, string>("ClearTransactionPool", (peerCode, announcement) =>
            {
                SetConsoleDefaults(peerCode);

                Console.WriteLine(announcement);

                for (int i = 0; i < hubConnections.Count; i++)
                {
                    hubConnections[i].InvokeAsync("ClearTransactionPoolPeerToPeer");
                }

            });

            try
            {
                await hubConnection.StartAsync();
                Console.WriteLine("Socket Connected ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured with message :{ ex.Message}");
            }

        }


        private static void SetConsoleDefaults(int peerCode)
        {
            switch (peerCode)
            {
                case (int)PeersEnum.Main:
                    Console.ForegroundColor = (ConsoleColor)PeerColorsEnum.Main;
                    break;
                case (int)PeersEnum.PeerOne:
                    Console.ForegroundColor = (ConsoleColor)PeerColorsEnum.PeerOne;
                    break;
                case (int)PeersEnum.PeerTwo:
                    Console.ForegroundColor = (ConsoleColor)PeerColorsEnum.PeerTwo;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
