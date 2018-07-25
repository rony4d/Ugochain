using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;
using UgoChain.Features.Wallet;

namespace UgoChain
{
    /// <summary>
    /// NBitcoin will be used for key management for now
    /// Custom implementation using Elliptic curve to be done soon
    /// </summary>
    /// 
    public class ChainUtility
    {
        public static Network Network { get; set; } = Network.TestNet;


        public static KeyPair GenerateNewKeyPair()
        {
            Key privateKey = new Key(); // generate a random private key

            string PrivateKey = privateKey.GetBitcoinSecret(Network).ToString();
            string PublicKey = privateKey.PubKey.ToString();
            string PublicKeyHash = privateKey.PubKey.Hash.ToString();
            return new KeyPair() { PrivateKey = PrivateKey, PublicKey = PublicKey, PublicKeyHash = PublicKeyHash };
        }


    }
}
