using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoServer.Models;
using UgoChain.PeerTwo.Features.Wallet;
using UgoChain.Api.PeerTwoSever.Hubs;
using UgoChain.PeerTwo.Features;

namespace UgoChain.Api.PeerTwoServer.Controllers
{
    /// <summary>
    /// Ensure all peers have their own Bockchain and Transaction Pool Instance
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        readonly IHubContext<PeerTwoHub> _peerTwoHubContext;
        static readonly Wallet _wallet = new Wallet();
        readonly IBlockchain _blockchain;

        public TransactionsController(IHubContext<PeerTwoHub> peerTwoHubContext, IBlockchain blockchain)
        {
            _peerTwoHubContext = peerTwoHubContext;
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
            _peerTwoHubContext.Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.PeerTwo, TransactionPool.Instance.Transactions);
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
