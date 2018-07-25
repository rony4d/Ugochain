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
            return (true, "Transaction Created Successfully");
        }
    }
}
