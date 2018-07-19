using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoSever.Hubs;
using UgoChain.PeerTwo.Features;

namespace UgoChain.Api.PeerOneServer.Controllers
{
    /// <summary>
    /// Block Controller For Peer One Server
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlockController : Controller
    {
        IHubContext<PeerTwoHub> _peerTwoHubContext;
        IBlockchain _blockchain;
        public BlockController(IHubContext<PeerTwoHub> peerTwoHubContext, IBlockchain blockchain)
        {
            _peerTwoHubContext = peerTwoHubContext;
            _blockchain = blockchain;
        }

        [HttpGet("getblocks")]
        public IActionResult GetBlocks()
        {
            return Ok(_blockchain.Chain);
        }

        [HttpPost("mine")]
        public IActionResult Mine(Block block)
        {
            _blockchain.AddBlock(block.Data);
            _peerTwoHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerTwo, _blockchain.Chain);
            return Ok(_blockchain.Chain);
        }

  
    }

}
