using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ugochain.PeerOne.Features;
using UgoChain.Api.PeerOneSever.Hubs;
using UgoChain.PeerOne.Features;
using UgoChain.PeerOne.Features.Wallet;

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
        readonly Wallet _minerWallet;

        readonly Miner _miner;
        public BlockController(IHubContext<PeerOneHub> peerOneHubContext, IBlockchain blockchain)
        {
            _peerOneHubContext = peerOneHubContext;
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
            //_peerOneHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerOne, _blockchain.Chain);
            //return Ok(_blockchain.Chain);


            _miner.Mine(); // mines block and adds it to blockchain instance

            //4. Synchronize the blockchain with other peers 
            _peerOneHubContext.Clients.All.SendAsync("ReceiveCurrentBlockchain", (int)PeersEnum.PeerOne, _blockchain.Chain);

            //5. Clear Transaction Pool
            TransactionPool.Instance.Transactions.Clear();

            //6. Broadcast Clear Transaction Pool to other peers
            _peerOneHubContext.Clients.All.SendAsync("ClearTransactionPool", (int)PeersEnum.PeerOne, $"Peer one broadcasting clear pool command: {TransactionPool.Instance.Transactions.Count} Transactions");

            return RedirectToAction("getblocks", new { controller = "block" });
        }
    }

}
