
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
        public WalletTests(ITestOutputHelper testOutputHelper)
        {
            _wallet = new Wallet();
            _testOutputHelper = testOutputHelper;
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

    }
}
