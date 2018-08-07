using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UgoChain.Features.Wallet
{
    public class Transaction
    {
        public string Id { get; set; }
        public TxInput Input { get; set; }

        public List<TxOutput> TxOutputs { get; set; } 

        public Transaction()
        {
            Guid guid = Guid.NewGuid();
            Id = guid.ToString();
            TxOutputs = new List<TxOutput>();
        }
        public (bool,string) CreateTransaction(Wallet senderWallet, string recipientAddress, decimal amount)
        {

           
            if (amount > senderWallet.Balance)
            {
                return (false,$"Amount {amount} exceeds balance");
            }

            TxOutput senderChangeBack = new TxOutput()
            {
                Address = senderWallet.PublicKey.Key,
                Amount = senderWallet.Balance - amount
            };
            TxOutput recipientOutput = new TxOutput()
            {
                Address = recipientAddress,
                Amount = amount
            };
            TxOutputs.Add(senderChangeBack);
            TxOutputs.Add(recipientOutput);

            SignTransaction(senderWallet);
            return (true, "Transaction Created Successfully");
        }

        /// <summary>
        /// Signs a transaction to be sent 
        /// 1. Build a transaction input
        /// 2. Generate the hash of the TxOutputs for the transaction
        /// 3. Sign the hash generated with the wallet's private key
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="senderWallet"></param>
        /// <returns></returns>
        public void SignTransaction(Wallet senderWallet)
        {
            TxInput txInput = new TxInput();
            byte[] hash = ChainUtility.Hash(JsonConvert.SerializeObject(TxOutputs));
            txInput.TimeStamp = Helper.ConvertToUnixTimeStamp(DateTime.Now).ToString();
            txInput.Address = senderWallet.PublicKey.Key;
            txInput.Amount = senderWallet.Balance;
            txInput.Signature = senderWallet.SignHash(hash);
            Input = txInput;
        }

        /// <summary>
        /// Veify signature using TxInput data-Note the TxInput is build from the wallet object
        /// </summary>
        /// <returns></returns>
        public bool VerifyTransaction()
        {
            byte[] publicKey = Convert.FromBase64String(Input.Address);
            byte[] dataHashToVerify = ChainUtility.Hash(JsonConvert.SerializeObject(TxOutputs));
            return ChainUtility.VerifySignature(publicKey, Input.Signature, dataHashToVerify);
        }

        /// <summary>
        /// Update current transaction by adding a new transaction output and modifying 
        /// the senders changeback output
        /// </summary>
        /// <param name="senderWallet"></param>
        /// <param name="recipientAddress"></param>
        /// <param name="amountToSend"></param>
        public (bool, string) UpdateTransaction(Wallet senderWallet, string recipientAddress, decimal amountToSend)
        {
            //get the current senders change back address and modify the amount to get back

            TxOutput changeBack = TxOutputs.Find(p => p.Address == senderWallet.PublicKey.Key);

            if (amountToSend > changeBack.Amount)
            {
                return (false, $"Amount {amountToSend} exceeds balance");

            }
            TxOutputs.Where(p => p.Address == senderWallet.PublicKey.Key).FirstOrDefault().Amount = changeBack.Amount - amountToSend;
            TxOutput newRecipientOutput = new TxOutput()
            {
                Address = recipientAddress,
                Amount = amountToSend
            };
            TxOutputs.Add(newRecipientOutput);
            SignTransaction(senderWallet);
            return (true, "Transaction Updated Successfully");

        }

        /// <summary>
        /// 1. Reward transaction has only one TxOutput which has the miners address
        /// 2. The TxInput is signed by the instance of the blockchain wallet
        /// </summary>
        /// <param name="minerWallet"></param>
        /// <param name="blockchainWallet"></param>
        /// <returns></returns>
        public (bool, string) CreateRewardTransaction(Wallet minerWallet)
        {
            Wallet blockchainWallet = Wallet.GetBlockchainWallet();

            TxOutput minerTxOutput = new TxOutput()
            {
                Address = minerWallet.PublicKey.Key,
                Amount = Block.MINER_REWARD
            };
            TxOutputs.Add(minerTxOutput);

            SignTransaction(blockchainWallet);
            return (true, "Reward Transaction Created Successfully");
        }

    }
}
