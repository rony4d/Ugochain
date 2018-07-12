using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UgoChain.Features;

namespace UgoChainApp.Controllers
{
    [Route("api/block")]
    public class BlockController : Controller
    {
        Blockchain blockchain { get; set; } = new Blockchain();
        [HttpGet("getblocks")]
        public IActionResult GetBlocks()
        {

            return Ok(blockchain.Chain);
        }

        [HttpPost("mine")]
        public IActionResult Mine(Block block)
        {
            blockchain.AddBlock(block.Data);
            return Ok(blockchain.Chain);
        }
    }
}