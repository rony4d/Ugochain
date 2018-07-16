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
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlockController : Controller
    {
        IHubContext<PeersHub> _peerHubContext;
        public BlockController(IHubContext<PeersHub> peerHubContext)
        {
            _peerHubContext = peerHubContext;
        }

        Blockchain blockchain { get; set; } = new Blockchain();
        [HttpGet("getblocks")]
        public IActionResult GetBlocks()
        {

            return Ok(blockchain.Chain);
        }

        [HttpPost("mine")]
        public Task Mine(Block block)
        {
            blockchain.AddBlock(block.Data);
            return _peerHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain",(int)PeersEnum.Main, blockchain.Chain);
            //return Ok(blockchain.Chain);
        }
    }
}