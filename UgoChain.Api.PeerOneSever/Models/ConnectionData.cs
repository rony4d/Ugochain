﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UgoChain.Api.PeerOneServer.Models
{
    public class ConnectionData
    {
        public DateTime ConnectionTime { get; set; }
        public string ConnectionId { get; set; }
        public string Payload { get; set; }
        public int PeerCode { get; set; }

    }
}
