using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UgoChain.Features.Wallet;

namespace UgoChain.PeerTwo.Features.Wallet
{
    public sealed class TransactionPool
    {
        public static readonly TransactionPool Instance = new TransactionPool();
        public List<Transaction> Transactions { get; set; }

        public TransactionPool()
        {
            Transactions = new List<Transaction>();
        }
        public void UpdateOrAddTransaction(Transaction transaction)
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

        public (bool, Transaction) ExistingTransaction(string sendersAddress)
        {
            Transaction transaction = Transactions.Find(p => p.Input.Address == sendersAddress);

            if (transaction != null)
            {
                return (true, transaction);
            }
            return (false, null);
        }

        /// <summary>
        /// To get valid transactions
        /// 1. The total TxOutput amounts should add up to the TxInput Amount: Prevents double spending
        /// 2. Verify the signature of every transaction to ensure the TxOuputs were not modified after it was signed
        /// </summary>
        /// <returns>status(true or false),Valid Transactions as a List and Invalid Transactions as a string(serialized)</returns>
        public (bool, List<Transaction>, string) ValidTransactions()
        {
            List<Transaction> validTransactions = new List<Transaction>();
            List<Transaction> invalidTransactions = new List<Transaction>();
            string invalidTransactionsJsonString = null;
            foreach (Transaction transaction in Transactions)
            {
                decimal txInputAmount = transaction.Input.Amount;
                decimal totalTxOutputAmount = transaction.TxOutputs.Sum(p => p.Amount);

                if (txInputAmount != totalTxOutputAmount)
                {
                    invalidTransactions.Add(transaction);
                }

                if (!transaction.VerifyTransaction())
                {
                    invalidTransactions.Add(transaction);
                }
                validTransactions.Add(transaction);
            }
            if (invalidTransactions.Count > 0)
            {
                invalidTransactionsJsonString = JsonConvert.SerializeObject(invalidTransactions);
                return (false, validTransactions, invalidTransactionsJsonString);
            }

            return (true, validTransactions, invalidTransactionsJsonString);
        }
    }
}
