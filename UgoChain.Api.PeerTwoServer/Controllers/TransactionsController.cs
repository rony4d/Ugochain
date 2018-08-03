using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UgoChain.Api.PeerTwoServer.Models;
using UgoChain.PeerTwo.Features.Wallet;
using UgoChain.Api.PeerTwoSever.Hubs;

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
        public TransactionsController(IHubContext<PeerTwoHub> peerTwoHubContext)
        {
            _peerTwoHubContext = peerTwoHubContext;

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
            _peerTwoHubContext.Clients.All.SendAsync("ReceiveTransactions", (int)PeersEnum.PeerTwo, TransactionPool.Instance.Transactions);
            return RedirectToAction("gettransactions", new { controller = "transactions" });
        }
    }
}
