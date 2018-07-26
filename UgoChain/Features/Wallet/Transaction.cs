using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

            SignTransaction(this, senderWallet);
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
        public void SignTransaction(Transaction transaction, Wallet senderWallet)
        {
            TxInput txInput = new TxInput();
            byte[] hash = ChainUtility.Hash(JsonConvert.SerializeObject(transaction.TxOutputs));
            txInput.TimeStamp = Helper.ConvertToUnixTimeStamp(DateTime.Now).ToString();
            txInput.Address = senderWallet.PublicKey.Key;
            txInput.Amount = senderWallet.Balance;
            txInput.Signature = senderWallet.SignHash(hash);
            transaction.Input = txInput;
        }
    }
}
