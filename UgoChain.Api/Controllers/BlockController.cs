using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UgoChain.Api.Hubs;
using UgoChain.Features;

namespace UgoChain.Api.Controllers
{
    /// <summary>
    /// Block Controller For Main Peer Server
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
      
    public class BlockController : Controller
    {
        IHubContext<PeersHub> _peerHubContext;
        IBlockchain _blockchain;
        public BlockController(IHubContext<PeersHub> peerHubContext, IBlockchain blockchain)
        {
            _peerHubContext = peerHubContext;
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
            _peerHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain",(int)PeersEnum.Main, _blockchain.Chain);
            //hubConnection.InvokeAsync("SyncChain", blockchain);

            return Ok(_blockchain.Chain);
        }

   
    }
}