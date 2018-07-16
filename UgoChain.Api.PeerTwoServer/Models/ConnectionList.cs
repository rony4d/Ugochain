using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UgoChain.Api.PeerTwoSever.Models
{
    public static class ConnectionList
    {
        public static List<ConnectionData> connections { get; set; }
        public static Dictionary<string, ConnectionData> ConnectionDictionary { get; set; } = new Dictionary<string, ConnectionData>();

        public static void AddUser(ConnectionData data)
        {
            if (!ConnectionDictionary.Keys.Contains(data.ConnectionId))
            {
                ConnectionDictionary.Add(data.ConnectionId, data);

            }
        }

        public static List<ConnectionData> GetActiveConnections()
        {
            connections = new List<ConnectionData>();

            foreach (var connection in ConnectionDictionary)
            {
                connections.Add(connection.Value);

            }
            return connections;
        }
    }
}
