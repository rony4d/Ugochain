
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UgoChain.Features.Wallet;
using Xunit;
using Xunit.Abstractions;

namespace UgoChain.Tests.Wallets.Tests
{
    public class WalletTests
    {
        private ITestOutputHelper _testOutputHelper;
        
        public WalletTests(ITestOutputHelper testOutputHelper)
        {
             _testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// This test was carried out with reference to Bouncy Castle ECTest (Elleptic Curve)
        /// 1. We should generate a private and public key pair
        /// 2. Sign a message with the private and  public key pair
        /// 3. Verify the msignature on the message
        /// </summary>
        //[Fact]
        //public void TestECDsaKeyGenTest()
        //{
        //    SecureRandom random = new SecureRandom();

        //    BigInteger n = new BigInteger("883423532389192164791648750360308884807550341691627752275345424702807307");

        //    FpCurve curve = new FpCurve(
        //        new BigInteger("883423532389192164791648750360308885314476597252960362792450860609699839"), // q
        //        new BigInteger("7fffffffffffffffffffffff7fffffffffff8000000000007ffffffffffc", 16), // a
        //        new BigInteger("6b016c3bdcf18941d0d654921475ca71a9db2fb27d1d37796185c2942c0a", 16), // b
        //        n, BigInteger.One);


        //    ECDomainParameters parameters = new ECDomainParameters(
        //        curve,
        //        curve.DecodePoint(Hex.Decode("020ffa963cdca8816ccc33b8642bedf905c3d358573d3f27fbbd3b3cb9aaaf")), // G
        //        n);

        //    ECKeyPairGenerator pGen = new ECKeyPairGenerator();
        //    ECKeyGenerationParameters genParam = new ECKeyGenerationParameters(
        //        parameters,
        //        random);

        //    pGen.Init(genParam);

        //    AsymmetricCipherKeyPair pair = pGen.GenerateKeyPair();

        //    ParametersWithRandom param = new ParametersWithRandom(pair.Private, random);
      
        //    ECDsaSigner ecdsa = new ECDsaSigner();

        //    ecdsa.Init(true, param);
        //    byte[] message = new BigInteger("968236873715988614170569073515315707566766479517").ToByteArray();
        //    BigInteger[] sig = ecdsa.GenerateSignature(message);

            
        //    ecdsa.Init(false, pair.Public);

        //    if (!ecdsa.VerifySignature(message, sig[0], sig[1]))
        //    {
        //        _testOutputHelper.WriteLine("signature fails");
        //    }

        //    _testOutputHelper.WriteLine("signature passes");

        //}

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

    }
}
