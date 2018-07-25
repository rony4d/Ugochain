using System;
using System.Collections.Generic;
using System.Text;

namespace UgoChain.Features.Wallet
{
    public class Wallet
    {
        public decimal Balance { get; set; }
        public PublicKey PublicKey { get; set; }
        public KeyPair KeyPair { get; set; }

        public Wallet()
        {
            KeyPair = ChainUtility.GenerateNewKeyPair();
            PublicKey = new PublicKey() { Key = KeyPair.PublicKey };
            Balance = 400;
        }
        public override string ToString()
        {
            return $"Wallet - " +
                $"PublicKey: {PublicKey} \n" +
                $"Balance: {Balance} \n";
        }


    }
}
