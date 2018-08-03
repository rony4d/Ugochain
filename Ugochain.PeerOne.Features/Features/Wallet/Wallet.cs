using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.PeerOne.Features.Wallet
{
    public class Wallet
    {
        public decimal Balance { get; set; }
        public UgoChain.Features.Wallet.PublicKey PublicKey { get; set; }
        public UgoChain.Features.Wallet.KeyPair KeyPair { get; set; }

        public Wallet()
        {
            KeyPair = ChainUtility.GenerateNewKeyPair();
            PublicKey = new UgoChain.Features.Wallet.PublicKey() { Key = KeyPair.PublicKey };
            Balance = 400;
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

        public (Transaction,string) CreateTransaction(string recipientAddress, decimal amountToSend)
        {
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

    }
}
