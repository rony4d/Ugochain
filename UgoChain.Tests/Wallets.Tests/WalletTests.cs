﻿
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Rebex.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UgoChain.Features.Wallet;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Wallets.Tests
{
    public class WalletTests
    {
        private ITestOutputHelper _testOutputHelper;
        private readonly Wallet _wallet;
        string recipientAddress = "r4nD0m 4ddr355";
        decimal amountToSend = 100;
        private Features.Blockchain _blockchain { get; set; }

        public WalletTests(ITestOutputHelper testOutputHelper)
        {
            _wallet = new Wallet();
            _testOutputHelper = testOutputHelper;
            _blockchain = new Features.Blockchain();
        }

        /// <summary>
        /// This test was carried out with reference to Bouncy Castle ECTest(Elleptic Curve)
        /// 1. We should generate a private and public key pair
        /// 2. Sign a message with the private and public key pair
        /// 3. Verify the msignature on the message
        /// </summary>
        [Fact]
        public void TestECDsaKeyGenTest()
        {
            SecureRandom random = new SecureRandom();

            BigInteger n = new BigInteger("883423532389192164791648750360308884807550341691627752275345424702807307");

            FpCurve curve = new FpCurve(
                new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
                new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
                new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16), // b
                n, BigInteger.One);


            ECDomainParameters parameters = new ECDomainParameters(
                curve,
                curve.DecodePoint(Hex.Decode("020ffa963cdca8816ccc33b8642bedf905c3d358573d3f27fbbd3b3cb9aaaf")), // G
                n);

            ECKeyPairGenerator pGen = new ECKeyPairGenerator();
            ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
                parameters,
                random);

            pGen.Init(genParam);

            AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

            ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);

            ECDsaSigner ecdsa = new ECDsaSigner();

            ecdsa.Init(true, param);
            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
            BigInteger[] sig = ecdsa.GenerateSignature(message);


            ecdsa.Init(false, pair.Public);

            if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
            {
                _testOutputHelper.WriteLine("signature fails");
            }

            _testOutputHelper.WriteLine("signature passes");

        }

        /// <summary>
        /// Bouncy castle implementation by Rebex.Castle Api
        /// Algorithm: EcDsaSha2Nistp256
        /// </summary>
        [Fact]
        public void TestEllipticCurve()
        {
            EllipticCurveAlgorithm curve = EllipticCurveAlgorithm.Create(EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            var privateKey = curve.GetPrivateKey();
            var publicKey = curve.GetPublicKey();
            string privateKeyStr = Convert.ToBase64String(privateKey);
            string publicKeyStr = Convert.ToBase64String(publicKey);

            _testOutputHelper.WriteLine($"Private Key: {privateKeyStr} \n" +
                $"Public Key: {publicKeyStr}");
        }

        /// <summary>
        /// Should generate different private keys based on the secure random number generator
        /// 
        /// </summary>
        [Fact]
        public void ShouldGenerateDifferentPrivateKeys()
        {
            KeyPair keyPair1 = ChainUtility.GenerateNewKeyPair();
            KeyPair keyPair2 = ChainUtility.GenerateNewKeyPair();

            Assert.NotEqual(keyPair1.PrivateKey, keyPair2.PrivateKey);
                
        }

        [Fact]
        public void ShouldGenerateWalletWithPrivateAndPublicKeyPair()
        {
            Wallet wallet = new Wallet();

            _testOutputHelper.WriteLine(wallet.ToString());
        }

        /// <summary>
        /// Should Sign and Verify Message
        /// NOTE- Always Create curve from private key before signing the message
        /// If not, the other user will not be able to decrypt message
        /// </summary>
        [Fact]
        public void ShouldSignAndVerifyMessage()
        {
            EllipticCurveAlgorithm curve = EllipticCurveAlgorithm.Create(EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            var privateKey = curve.GetPrivateKey();
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsa.FromPrivateKey(privateKey);
            EllipticCurveDsa ellipticCurveDsaFake = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsaFake.FromPrivateKey(privateKey);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();

            byte[] signature = ellipticCurveDsa.SignMessage(message);

            
            bool isCorrect = ellipticCurveDsa.VerifyMessage(message, signature);

            bool isFake = ellipticCurveDsaFake.VerifyMessage(message, signature);
            Assert.Equal(isCorrect, isFake);

            //Both values are true because the curves were built from the same private key

        }
        /// <summary>
        /// Verifying without key
        /// </summary>
        [Fact]
        public void ShouldSignAndVerifyMessageWithoutKey()
        {
  
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            EllipticCurveDsa ellipticCurveDsaFake = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);

            byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();

            byte[] signature = ellipticCurveDsa.SignMessage(message);


            bool isCorrect = ellipticCurveDsa.VerifyMessage(message, signature);

            bool isFake = ellipticCurveDsaFake.VerifyMessage(message, signature);
            Assert.True(isCorrect, "Signature does not match message");
            Assert.False(isFake, "Fake Signature does not match message");

            //Both values will not be true because no common keys were used in building the curves
        }
        /// <summary>
        /// Signs and verifies a data hash
        /// Both fake curve and real curve will verify message since they are built from the same
        /// private key
        /// </summary>
        [Fact]
        public void ShouldSignAndVerifyDataHash()
        {
            EllipticCurveAlgorithm curve = EllipticCurveAlgorithm.Create(EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            var privateKey = curve.GetPrivateKey();
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            EllipticCurveDsa ellipticCurveDsaFake = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsa.FromPrivateKey(privateKey);
            ellipticCurveDsaFake.FromPrivateKey(privateKey);

            SHA256 sHA256 = SHA256.Create();
            byte[] hashBytes = sHA256.ComputeHash(Encoding.Default.GetBytes("Message to be hashed"));

            byte[] signature = ellipticCurveDsa.SignHash(hashBytes);


            bool isCorrect = ellipticCurveDsa.VerifyHash(hashBytes, signature);

            bool isFake = ellipticCurveDsaFake.VerifyHash(hashBytes, signature);
            Assert.Equal(isCorrect, isFake);

        }

        /// <summary>
        /// Signs and verifies a data hash
        /// Both fake curve and real curve will verify message since they are built from the same
        /// private key
        /// The second curve will be built from the public key and should also verify the signature.
        /// This is possible because a public key is generated by curve multiplication of the privatekey
        /// K = k * G, where G is the generator poin, k is private key, K is public key
        /// </summary>
        [Fact]
        public void ShouldVerifyTransactionUsingPublicKey()
        {
            EllipticCurveAlgorithm curve = EllipticCurveAlgorithm.Create(EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            var privateKey = curve.GetPrivateKey();
            var pubKey = curve.GetPublicKey();
            //The first curve is created from the private key
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsa.FromPrivateKey(privateKey);
            //The second curve is created from the public key of the first curve and is used to verify the transaction
            //The public key can be easily shared which makes it easy for verification
            EllipticCurveDsa ellipticCurveDsaFake = new EllipticCurveDsa("1.2.840.10045.3.1.7", EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsaFake.FromPublicKey(pubKey);

            SHA256 sHA256 = SHA256.Create();
            byte[] hashBytes = sHA256.ComputeHash(Encoding.Default.GetBytes("Message to be hashed"));

            byte[] signature = ellipticCurveDsa.SignHash(hashBytes);


            bool isCorrect = ellipticCurveDsa.VerifyHash(hashBytes, signature);

            bool isFake = ellipticCurveDsaFake.VerifyHash(hashBytes, signature);
            Assert.Equal(isCorrect, isFake);

            //Perform same operation using ChainUtility
            byte[] signature2 = ChainUtility.SignDataHash(hashBytes, Convert.ToBase64String(privateKey));

            var newKeyPair = ChainUtility.GenerateNewKeyPair();
            Assert.True(ChainUtility.VerifySignature(pubKey, signature2, hashBytes));
            Assert.False(ChainUtility.VerifySignature(Convert.FromBase64String(newKeyPair.PublicKey), signature2, hashBytes));

        }

        /// <summary>
        /// This test should create a similar transaction two times
        /// The goal is to test that the Wallet's CreateTransaction will recognize
        /// the same senders address and update the transaction from the TransactionPool
        /// instead of creating a new transaction
        /// Expected Outcome: We should have a single transaction from the Transaction Pool with 2 TxOutputs
        /// that have amount which is times 2 of original send amount
        /// We should also expect that the final ChangeBackTxOutput amount is equal to Wallet Balance minus two times
        /// the amountSent, Since we are using the same wallet
        /// </summary>
        [Fact]
        public void ShouldDoubleSendAmount()
        {
            (Transaction,string) response = _wallet.CreateTransaction(recipientAddress, amountToSend, _blockchain); //1st time
            Assert.Equal(2, response.Item1.TxOutputs.Count); //Expected 2 TxOuputs:- ChangeBackTxOutput and Recipient TxOutput
            response = _wallet.CreateTransaction(recipientAddress, amountToSend, _blockchain); //2nd time
            Assert.Equal(3, response.Item1.TxOutputs.Count); //Expected 3 TxOuputs:- ChangeBackTxOutput, 1st Recipient TxOutput and 2nd Recipient TxOutput

            decimal changeBackTxOutputAmount = response.Item1.TxOutputs.Find(p => p.Address == _wallet.PublicKey.Key).Amount;
            decimal finalBalance = _wallet.Balance - (amountToSend * 2);

            Assert.Equal(changeBackTxOutputAmount, finalBalance);

        }


        #region calculate balance test

        /// <summary>
        /// Should calculate the blockchain balance that matches the recipient
        /// </summary>
        [Fact]
        public void ShouldCalculateRecipientBalance()
        {
            Wallet senderWallet = new Wallet(); // Each wallet has initial balance of 400
            //_wallet will act as recipient wallet
            decimal addBalance = 100;
            int repeat = 3;

            for (int i = 0; i < repeat; i++)
            {
                //Recall, when creating these transactions, they are being added to the transaction pool
                senderWallet.CreateTransaction(_wallet.PublicKey.Key, addBalance, _blockchain);
            }

            //Add the above transactions to the blockchain
            _blockchain.AddBlock(JsonConvert.SerializeObject(TransactionPool.Instance.Transactions));

            //Calculate the balance of the recipient wallet

            decimal recipientBalance = _wallet.CalculateBalance(_blockchain);

            decimal expectedRecipientBalance = Wallet.WALLET_INITIAL_BALANCE + (addBalance * repeat);

            Assert.Equal(expectedRecipientBalance, recipientBalance);
        }
        
        /// <summary>
        /// Should calculate the blockchain balance that matches the sender
        /// </summary>
        [Fact]
        public void ShouldCalculateSenderBalance()
        {
            Wallet senderWallet = new Wallet(); // Each wallet has initial balance of 400
            //_wallet will act as recipient wallet
            decimal addBalance = 100;
            int repeat = 3;

            for (int i = 0; i < repeat; i++)
            {
                //Recall, when creating these transactions, they are being added to the transaction pool
                senderWallet.CreateTransaction(_wallet.PublicKey.Key, addBalance, _blockchain);
            }

            //Add the above transactions to the blockchain
            _blockchain.AddBlock(JsonConvert.SerializeObject(TransactionPool.Instance.Transactions));

            //Calculate the balance of the sender wallet

            decimal senderWalletBalance = senderWallet.CalculateBalance(_blockchain);

            decimal expectedSenderWalletBalance = Wallet.WALLET_INITIAL_BALANCE - (addBalance * repeat);

            Assert.Equal(expectedSenderWalletBalance, senderWalletBalance);
        }


        // The test below is where the recipient now sends out money, hence conducting a transaction
    
        /// <summary>
        /// In this test,
        /// 1. The recipient should create a transaction and send to the sender
        /// 2. The sender should also create a transaction and send to the recipient
        /// We want to verify that the recipient balance will be calculated from the most recent tramsactions
        /// and that the final balance will be accurate after subtracitng amount sent and adding amount received
        /// </summary>
        [Fact]
        public void ShouldCalculateBalanceOfRecipientAfterCreatingTransaction()
        {
            Wallet senderWallet = new Wallet(); // Each wallet has initial balance of 400
            //_wallet will act as recipient wallet
            decimal addBalance = 100;
            decimal subtractBalance = 50;
            int repeat = 3;

            for (int i = 0; i < repeat; i++)
            {
                //Recall, when creating these transactions, they are being added to the transaction pool
                //1. Recipient sends out subtract balance to the sender wallet, repeat times
                _wallet.CreateTransaction(senderWallet.PublicKey.Key, subtractBalance, _blockchain);
            }
            //Add the above transactions to the blockchain
            _blockchain.AddBlock(JsonConvert.SerializeObject(TransactionPool.Instance.Transactions));

            decimal firstRecipientBalance = _wallet.CalculateBalance(_blockchain); // this test has been run and found to be correct, so this holds


            //2. Sender sends out add balance to the recipient
            senderWallet.CreateTransaction(_wallet.PublicKey.Key, addBalance, _blockchain);
            
            //Add the above transactions to the blockchain
            _blockchain.AddBlock(JsonConvert.SerializeObject(TransactionPool.Instance.Transactions));

            //Calculate the balance of the recipient wallet

            decimal finalRecipientBalance = _wallet.CalculateBalance(_blockchain);

            decimal expectedRecipientBalance = firstRecipientBalance + addBalance;

            Assert.Equal(expectedRecipientBalance, finalRecipientBalance);
        }

        #endregion

    }
}
