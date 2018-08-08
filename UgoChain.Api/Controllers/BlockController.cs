using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UgoChain.Api.Hubs;
using UgoChain.Features;
using UgoChain.Features.Wallet;

namespace UgoChain.Api.Controllers
{
    /// <summary>
    /// Block Controller For Main Peer Server
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
      
    public class BlockController : Controller
    {
        readonly IHubContext<PeersHub> _peerHubContext;
        readonly IBlockchain _blockchain;
        readonly  Wallet _minerWallet;

        readonly  Miner _miner;
        public BlockController(IHubContext<PeersHub> peerHubContext, IBlockchain blockchain)
        {
            _peerHubContext = peerHubContext;
            _blockchain = blockchain;
            _minerWallet = new Wallet(); // new miner instance iscreated for every request. Miner will have to signin in production
            _miner = new Miner((Blockchain)_blockchain,TransactionPool.Instance,_minerWallet); // Miner will signin in production

        }

        [HttpGet("getblocks")]
        public IActionResult GetBlocks()
        {
            return Ok(_blockchain.Chain);
        }

        ///Continuation of mine function
        /// 4. Synchronize the blockchain with other peers
        /// 5. Clear the transaction pool
        /// 6. Broadcast to everyminer to clear their transaction pools
        [HttpPost("mine")]
        public IActionResult Mine(Block block)
        {

            _miner.Mine(); // mines block and adds it to blockchain instance

            //4. Synchronize the blockchain with other peers 
            _peerHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain",(int)PeersEnum.Main, _blockchain.Chain);

            //5. Clear Transaction Pool
            TransactionPool.Instance.Transactions.Clear();

            //6. Broadcast Clear Transaction Pool to other peers
            _peerHubContext.Clients.All.SendAsync("ClearTransactionPool", (int)PeersEnum.Main, $"Main peer broadcasting clear pool command: {TransactionPool.Instance.Transactions.Count} Transactions");

            return RedirectToAction("getblocks", new { controller = "block" });
        }



    }
}