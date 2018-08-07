using System;
using System.Collections.Generic;
using System.Text;
using UgoChain.Features.Wallet;

namespace UgoChain.Features
{
    public class Miner
    {
        public Blockchain Blockchain { get; set; }
        public TransactionPool TransactionPool { get; set; }
        public Wallet.Wallet Wallet { get; set; }

        public Miner(Blockchain blockchain, TransactionPool transactionPool, Wallet.Wallet wallet)
        {
            Blockchain = blockchain;
            TransactionPool = transactionPool;
            Wallet = wallet;
        }

        /// <summary>
        /// 1. Get valid transactions from transaction pool
        /// 2. Include a reward for the miner
        /// 3. Create a block consisting of the valid transactions
        /// 4. Synchronize the blockchain to the peer-to-peer server
        /// 5. Clear the transaction pool
        /// 6. Broadcast to everyminer to clear their transaction pools
        /// </summary>
        public void Mine()
        {
            
        }
    }
}
