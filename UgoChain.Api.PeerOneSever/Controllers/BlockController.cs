using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerOneSever.Hubs;
using UgoChain.PeerOne.Features;

namespace UgoChain.Api.PeerOneServer.Controllers
{
    /// <summary>
    /// Ensure all peers have their own Bockchain and Transaction Pool Instance
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlockController : Controller
    {
        IHubContext<PeerOneHub> _peerOneHubContext;
        IBlockchain _blockchain;
        public BlockController(IHubContext<PeerOneHub> peerOneHubContext, IBlockchain blockchain)
        {
            _peerOneHubContext = peerOneHubContext;
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
            _peerOneHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerOne, _blockchain.Chain);
            return Ok(_blockchain.Chain);
        }
    }

}
