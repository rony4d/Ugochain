using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UgoChain.Features;
using UgoChain.Features.Wallet;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Wallets.Tests
{
    public class TransactionPoolTests
    {
        ITestOutputHelper _testOutputHelper;
        Transaction _transaction;
        Wallet _wallet;
        string recipientAddress = "r3c1p13entPu8liCK3y";
        //string nextRecipient = "n3x7 a44r355";
        //decimal nextAmount = 40;
        decimal amountToSend = 50;

        public TransactionPoolTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _wallet = new Wallet();
            _transaction = new Transaction();
            _transaction.CreateTransaction(_wallet, recipientAddress, amountToSend);

        }
        /*
       * Transaction Pool Tests
       */
        /// <summary>
        /// Testing TransactionPool Singleton Class
        /// </summary>
        [Fact]
        public void ShouldTestTransactionPoolStaticParameter()
        {
            Transaction transaction1 = new Transaction();
            Transaction transaction2 = new Transaction();
            TransactionPool.Instance.UpdateOrAddTransaction(transaction1);
            TransactionPool.Instance.UpdateOrAddTransaction(transaction2);
            
            Assert.Equal(2, TransactionPool.Instance.Transactions.Count);

        }
        /// <summary>
        /// Should add transaction to the transaction pool
        /// Add a transaction to the transaction pool
        /// Get the transaction and compare the Id with the Transaction object added
        /// </summary>
        [Fact]
        public void ShouldAddTransactionToPool()
        {
            TransactionPool.Instance.UpdateOrAddTransaction(_transaction);

            Transaction addedTransaction = TransactionPool.Instance.Transactions.Find(p => p.Id == _transaction.Id);
            Assert.Equal(_transaction.Id, addedTransaction.Id);
        }
        /// <summary>
        /// Should update a transaction in the pool
        /// 1. Serialize initial transaction object to get the string value
        /// 2. Add this initial transaction object to the TransactionPool
        /// 3. Update the initial transaction object by adding a new TxOutput
        /// 4. Add the updated transaction object ot the transaction pool
        /// 5. Get the string value of the updated transaction from the pool and compare with the old string value
        /// they should not be the same but should have the same Id
        /// </summary>
        [Fact]
        public void ShouldUpdateATransactionInThePool()
        {
            string oldTransactionString = JsonConvert.SerializeObject(_transaction);
            TransactionPool.Instance.UpdateOrAddTransaction(_transaction);

            _transaction.UpdateTransaction(_wallet, "r3cipi3n7 4ddr355", 50);

            TransactionPool.Instance.UpdateOrAddTransaction(_transaction);

            Transaction updatedTransactionFromPool = TransactionPool.Instance.Transactions.Find(p => p.Id == _transaction.Id);
            string updatedTransactionFromPoolString = JsonConvert.SerializeObject(updatedTransactionFromPool);

            Assert.NotEqual(oldTransactionString, updatedTransactionFromPoolString);

        }


        //Mixing Valid and Corrupt Transactions

        /// <summary>
        /// Should Invalidate a Transaction Pool with corrupt transactions
        /// Each transaction should be created by a different wallet
        /// 1. Run a loop and add a corrupt transaction every even number iteration
        /// </summary>
        [Fact]
        public void ShouldInvalidateCorruptTransactionsInPool()
        {
            //Add transactions to pool
            for (int i = 0; i < 10; i++)
            {
                _wallet = new Wallet();
                _wallet.CreateTransaction($"recipient 0x1234:{i}", 34.5m);             
            }

            //corrupt even number transactions
            for (int i = 0; i < TransactionPool.Instance.Transactions.Count; i++)
            {
                if (i%2 == 0)
                {
                    TransactionPool.Instance.Transactions[i].TxOutputs[0].Amount = 200;
                }
            }

            var response = TransactionPool.Instance.ValidTransactions();
            bool isValid = response.Item1;
            string invalidTransactionsJsonString = response.Item3;
            Assert.False(isValid);
            _testOutputHelper.WriteLine(invalidTransactionsJsonString);
        }
        /// Should Validate a Transaction Pool with valid transactions
        /// Each transaction should be created by a different wallet
        /// 1. Run a loop and add a corrupt transaction every even number iteration
        [Fact]
        public void ShouldValidateValidTransactionsInPool()
        {
            //Add transactions to pool
            for (int i = 0; i < 10; i++)
            {
                _wallet = new Wallet();
                _wallet.CreateTransaction($"recipient 0x1234:{i}", 34.5m);
            }

            bool isValid = TransactionPool.Instance.ValidTransactions().Item1;
            Assert.True(isValid);
        }

        /// <summary>
        /// Should create a miners wallet and test if the miner's reward is the 
        /// amount in the single TxOuput
        /// </summary>
        [Fact]
        public void ShouldCreateRewardTransactionForMiner()
        {
            //we will use _wallet as miners wallet
            //reinitialize transaction to empty object
            _transaction = new Transaction();
            (bool,string) response = _transaction.CreateRewardTransaction(_wallet);

            Assert.True(response.Item1);
            decimal minersReward = _transaction.TxOutputs.Find(p => p.Address == _wallet.PublicKey.Key).Amount;
            Assert.Equal(Block.MINER_REWARD, minersReward);
        }
    }
}
