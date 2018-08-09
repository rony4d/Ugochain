using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UgoChain.Api.Hubs;
using UgoChain.Api.Models;
using UgoChain.Features;
using UgoChain.Features.Wallet;

namespace UgoChain.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        readonly IHubContext<PeersHub> _peerHubContext;
        readonly IBlockchain _blockchain;

        static readonly Wallet _wallet = new Wallet();
        public TransactionsController(IHubContext<PeersHub> peerHubContext, IBlockchain blockchain)
        {
            _peerHubContext = peerHubContext;
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
            (Transaction,string) transactionInfo = _wallet.CreateTransaction(viewModel.RecipientAddress, viewModel.AmountToSend, _blockchain as Blockchain);
            //share this peer's TransactionPool
            _peerHubContext.Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.Main, TransactionPool.Instance.Transactions);
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