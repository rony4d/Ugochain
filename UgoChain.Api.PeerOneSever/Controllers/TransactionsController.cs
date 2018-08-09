using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerOneServer.Models;
using UgoChain.Api.PeerOneSever.Hubs;
using UgoChain.PeerOne.Features;
using UgoChain.PeerOne.Features.Wallet;

namespace UgoChain.Api.PeerOneServer.Controllers
{
    /// <summary>
    /// Ensure all peers have their own Bockchain and Transaction Pool Instance
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        readonly IHubContext<PeerOneHub> _peerOneHubContext;
        static readonly Wallet _wallet = new Wallet();
        readonly IBlockchain _blockchain;

        public TransactionsController(IHubContext<PeerOneHub> peerOneHubContext, IBlockchain blockchain)
        {
            _peerOneHubContext = peerOneHubContext;
            _blockchain = blockchain;
        }
        /// <summary>
        /// Get Transactions from local transaction pool
        /// </summary>
        /// <returns></returns>
        [HttpGet("gettransactions")]
        public IActionResult GetTransactions()
        {
            return Ok(TransactionPool.Instance.Transactions);
        }

        /// <summary>
        /// Creates a new transaction and returns all the transactions
        /// in the local transaction pool
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost("createtransaction")]
        public IActionResult CreateTransaction(TransactionViewModel viewModel)
        {
            (Transaction, string) transactionInfo = _wallet.CreateTransaction(viewModel.RecipientAddress, viewModel.AmountToSend,_blockchain as Blockchain);
            //share this peer's TransactionPool
            _peerOneHubContext.Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.PeerOne, TransactionPool.Instance.Transactions);
            return RedirectToAction("gettransactions", new { controller = "transactions" });
        }

        /// <summary>
        /// Returns the public key of the wallet
        /// </summary>
        /// <returns></returns>
        [HttpGet("publickkey")]
        public IActionResult GetPublicKey()
        {
            return Ok(_wallet.PublicKey);
        }


        [HttpGet("walletbalance")]
        public IActionResult GetWalletBalance()
        {
            decimal balance = _wallet.CalculateBalance(_blockchain as Blockchain);
            return Ok(balance);
        }
    }
}
