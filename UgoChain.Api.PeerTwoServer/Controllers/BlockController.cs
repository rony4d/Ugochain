using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoSever.Hubs;
using UgoChain.PeerTwo.Features;
using UgoChain.PeerTwo.Features.Wallet;

namespace UgoChain.Api.PeerTwoServer.Controllers
{
    /// <summary>
    /// Ensure all peers have their own Bockchain and Transaction Pool Instance
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class BlockController : Controller
    {
        IHubContext<PeerTwoHub> _peerTwoHubContext;
        IBlockchain _blockchain;
        readonly Wallet _minerWallet;
        readonly Miner _miner;
        public BlockController(IHubContext<PeerTwoHub> peerTwoHubContext, IBlockchain blockchain)
        {
            _peerTwoHubContext = peerTwoHubContext;
            _blockchain = blockchain;
            _minerWallet = new Wallet(); // new miner instance iscreated for every request. Miner will have to signin in production
            _miner = new Miner((Blockchain)_blockchain, TransactionPool.Instance, _minerWallet); // Miner will signin in production
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
            //_blockchain.AddBlock(block.Data);
            //_peerTwoHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerTwo, _blockchain.Chain);
            //return Ok(_blockchain.Chain);

            _miner.Mine(); // mines block and adds it to blockchain instance

            //4. Synchronize the blockchain with other peers 
            _peerTwoHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerTwo, _blockchain.Chain);

            //5. Clear Transaction Pool
            TransactionPool.Instance.Transactions.Clear();

            //6. Broadcast Clear Transaction Pool to other peers
            _peerTwoHubContext.Clients.All.SendAsync("ClearTransactionPool", (int)PeersEnum.PeerTwo, $"Peer two broadcasting clear pool command: {TransactionPool.Instance.Transactions.Count} Transactions");

            return RedirectToAction("getblocks", new { controller = "block" });
        }

  
    }

}
