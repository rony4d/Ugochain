using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UgoChain.PeerTwo.Features.Wallet;

namespace UgoChain.PeerTwo.Features
{
    public class Miner
    {
        public Blockchain Blockchain { get; set; }
        public TransactionPool TransactionPool { get; set; }
        public Wallet.Wallet MinerWallet { get; set; }

        public List<Transaction> ValidTransactions { get; set; }
        public List<Transaction> InvalidTransactions { get; set; }

        public Miner(Blockchain blockchain, TransactionPool transactionPool, Wallet.Wallet wallet)
        {
            Blockchain = blockchain;
            TransactionPool = transactionPool;
            MinerWallet = wallet;
        }

        /// <summary>
        /// 1. Get valid transactions from transaction pool
        /// 2. Include a reward for the miner
        /// 3. Create a block consisting of the valid transactions

        /// </summary>
        public void Mine()
        {
            // 1. Get valid transactions
            var response = TransactionPool.Instance.ValidTransactions();
            ValidTransactions = response.Item2;

            if (!response.Item1)
                InvalidTransactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Item3);

            // 2. Create reward transaction for miner
            if (ValidTransactions.Count > 0)
            {
                Transaction rewardTransaction = new Transaction();
                rewardTransaction.CreateRewardTransaction(MinerWallet);
                ValidTransactions.Add(rewardTransaction);
            }
            //3. Create block consisiting of valid transactions
            Blockchain.AddBlock(JsonConvert.SerializeObject(ValidTransactions));
        }
    }
}
