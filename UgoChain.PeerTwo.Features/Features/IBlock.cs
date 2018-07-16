using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.PeerTwo.Features
{
    public interface IBlock
    {
        string TimeStamp { get; set; }
        string LastHash { get; set; }
        string Hash { get; set; }
        string Data { get; set; }

 
    }
}
