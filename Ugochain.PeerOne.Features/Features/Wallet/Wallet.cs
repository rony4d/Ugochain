using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UgoChain.PeerOne.Features.Wallet
{
    public class Wallet
    {
        public decimal Balance { get; set; }
        public UgoChain.Features.Wallet.PublicKey PublicKey { get; set; }
        public UgoChain.Features.Wallet.KeyPair KeyPair { get; set; }

        public const string BLOCKCHAIN_ADDRESS_PEER_ONE = "p33r0n34ddr335";
        public const decimal WALLET_INITIAL_BALANCE = 400;


        public Wallet()
        {
            KeyPair = ChainUtility.GenerateNewKeyPair();
            PublicKey = new UgoChain.Features.Wallet.PublicKey() { Key = KeyPair.PublicKey };
            Balance = WALLET_INITIAL_BALANCE;
        }
        public override string ToString()
        {
            return $"Wallet - " +
                $"PublicKey: {PublicKey} \n" +
                $"Balance: {Balance} \n";
        }

        public byte[] SignHash(byte [] Hash)
        {
            byte[] signature = ChainUtility.SignDataHash(Hash, KeyPair.PrivateKey);
            return signature;
        }

        public (Transaction, string) CreateTransaction(string recipientAddress, decimal amountToSend, Blockchain blockchain)
        {
            Balance = CalculateBalance(blockchain);

            if (amountToSend > Balance)
            {
                return (null, $"Amount {amountToSend} exceeds balance");
            }

            (bool, Transaction) response = TransactionPool.Instance.ExistingTransaction(PublicKey.Key);
            Transaction transaction = response.Item2;
            if (response.Item1)
            {
                transaction.UpdateTransaction(this, recipientAddress, amountToSend);
            }
            else
            {
                transaction = new Transaction();
                transaction.CreateTransaction(this, recipientAddress, amountToSend);
            }

            TransactionPool.Instance.UpdateOrAddTransaction(transaction);

            return (transaction, $"Transaction created successfully");
        }

        /// <summary>
        /// Modify implementation later
        /// </summary>
        public static Wallet GetBlockchainWallet()
        {
            return new Wallet()
            {
                KeyPair = ChainUtility.GenerateNewKeyPair(),
                PublicKey = new UgoChain.Features.Wallet.PublicKey() { Key = BLOCKCHAIN_ADDRESS_PEER_ONE }
            };
        }

        public decimal CalculateBalance(Blockchain blockchain)
        {
            decimal balance = Balance;
            List<Transaction> transactions = new List<Transaction>();

            foreach (Block block in blockchain.Chain)
            {
                //if block is genesis block ignore
                if (block.TimeStamp != Block.GenesisTime.ToString())
                {
                    List<Transaction> transactionsPerBlock = JsonConvert.DeserializeObject<List<Transaction>>(block.Data);
                    transactions = transactions.Concat(transactionsPerBlock).ToList();
                }

            }

            List<Transaction> walletTxInputs = transactions.Where(p => p.Input.Address == PublicKey.Key).ToList();
            string startTime = null;
            if (walletTxInputs.Count > 0)
            {
                //Get the most recent wallet TxInput Transaction

                Transaction recentwalletTxInput = walletTxInputs.OrderByDescending(p => Helper.ConvertLocalTime(double.Parse(p.Input.TimeStamp))).FirstOrDefault();

                //Assign balance to the changeBackTxOuput of the most recent wallet transaction
                balance = recentwalletTxInput.TxOutputs.Find(p => p.Address == PublicKey.Key).Amount;
                startTime = recentwalletTxInput.Input.TimeStamp;
            }


            //Adding to the balance for recent transactions received by this wallet
            foreach (var transaction in transactions)
            {
                if (startTime != null && double.Parse(transaction.Input.TimeStamp) > double.Parse(startTime))
                {
                    List<UgoChain.Features.Wallet.TxOutput> recentOutputs = transaction.TxOutputs.Where(p => p.Address == PublicKey.Key).ToList();
                    foreach (UgoChain.Features.Wallet.TxOutput output in recentOutputs)
                    {
                        balance += output.Amount;
                    }
                }

                if (startTime == null) // means this wallet has not sent out money yet, just receiving
                {
                    List<UgoChain.Features.Wallet.TxOutput> recentOutputs = transaction.TxOutputs.Where(p => p.Address == PublicKey.Key).ToList();
                    foreach (UgoChain.Features.Wallet.TxOutput output in recentOutputs)
                    {
                        balance += output.Amount;
                    }
                }
            }

            return balance;
        }


    }
}
