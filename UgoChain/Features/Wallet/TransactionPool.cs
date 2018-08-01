using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UgoChain.Features.Wallet
{
    public sealed class TransactionPool
    {
        public static readonly TransactionPool Instance = new TransactionPool();
        public  List<Transaction> Transactions { get; set; }

        public TransactionPool()
        {
            Transactions = new List<Transaction>();
        }
        public  void UpdateOrAddTransaction(Transaction transaction)
        {
            Transaction exisitingTransaction = Transactions.Find(p => p.Id == transaction.Id);
            if (exisitingTransaction != null)
            {
                int transactionIndex = Transactions.FindIndex(p => p.Id == transaction.Id);
                Transactions[transactionIndex] = transaction;
            }
            else
            {
                Transactions.Add(transaction);
            }
        }
    }
}
