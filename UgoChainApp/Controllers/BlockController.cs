using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UgoChain.Features;

namespace UgoChainApp.Controllers
{
    [Produces("application/json")]
    [Route("api/block")]
    public class BlockController : Controller
    {
        [HttpGet("getblocks")]
        public IActionResult GetBlocks()
        {
            Blockchain blockchain = new Blockchain();

            return Ok(blockchain.Chain);
        }
    }
}