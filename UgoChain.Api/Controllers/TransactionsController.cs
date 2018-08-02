using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UgoChain.Api.Models;
using UgoChain.Features.Wallet;

namespace UgoChain.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        static readonly Wallet _wallet = new Wallet();
        public TransactionsController()
        {
            
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
            (Transaction,string) transactionInfo = _wallet.CreateTransaction(viewModel.RecipientAddress, viewModel.AmountToSend);
            return RedirectToAction("gettransactions", new { controller = "transactions" });
        }
    }
}