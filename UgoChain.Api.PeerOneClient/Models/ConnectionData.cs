using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.Api.PeerOneClient.Models
{
    public class ConnectionData
    {
        public DateTime ConnectionTime { get; set; }
        public string ConnectionId { get; set; }
        public string Payload { get; set; }
        public int PeerCode { get; set; }
    }


}
