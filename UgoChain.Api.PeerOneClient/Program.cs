using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UgoChain.Api.PeerOneClient.Models;

namespace UgoChain.Api.PeerOneClient
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.Title = "Ugochain Main .NET Client";
            HubConnection peerOneHubConnection = new HubConnectionBuilder()
                                            .WithUrl("https://localhost:44353/PeerOneHub")
                                            .Build();

            HubConnection mainPeerConnection = new HubConnectionBuilder()
                                            .WithUrl("https://localhost:44378/PeersHub")
                                            .Build();

            ConfigureConnection(peerOneHubConnection).GetAwaiter().GetResult();
            ConfigureConnection(mainPeerConnection).GetAwaiter().GetResult();

            Console.ReadKey();
        }

        public static async Task ConfigureConnection(HubConnection hubConnection)
        {
            hubConnection.On<int,string, string, string>("ReceiveMessage", (peerCode,timestamp, user, message) =>
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
