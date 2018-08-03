using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerOneServer.Models;
using UgoChain.Api.PeerOneSever.Hubs;
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
        public TransactionsController(IHubContext<PeerOneHub> peerOneHubContext)
        {
            _peerOneHubContext = peerOneHubContext;

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
            (Transaction, string) transactionInfo = _wallet.CreateTransaction(viewModel.RecipientAddress, viewModel.AmountToSend);
            //share this peer's TransactionPool
            _peerOneHubContext.Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.PeerOne, TransactionPool.Instance.Transactions);
            return RedirectToAction("gettransactions", new { controller = "transactions" });
        }
    }
}
