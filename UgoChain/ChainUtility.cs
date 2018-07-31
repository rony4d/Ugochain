using NBitcoin;
using Rebex.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
        public const string CurveOID = "1.2.840.10045.3.1.7"; // Curve object identifier
        public static Network Network { get; set; } = Network.TestNet;


        public static KeyPair GenerateNewKeyPair()
        {
           
            //Key privateKey = new Key(); // NBitcoin: generate a random private key
            EllipticCurveAlgorithm curve = EllipticCurveAlgorithm.Create(EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            byte [] pKey = curve.GetPrivateKey();
            byte [] pubKey = curve.GetPublicKey();

            string PrivateKey = Convert.ToBase64String(pKey);
            string PublicKey = Convert.ToBase64String(pubKey);
            return new KeyPair() { PrivateKey = PrivateKey, PublicKey = PublicKey };
        }

        public static byte[] SignDataHash(byte [] Hash, string privateKey)
        {
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa(CurveOID, EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            byte[] privateKeyByte = Convert.FromBase64String(privateKey);
            ellipticCurveDsa.FromPrivateKey(privateKeyByte);

            byte[] signature = ellipticCurveDsa.SignHash(Hash);

            return signature;
        }

        public static byte [] Hash(string data)
        {
            SHA256 sHA256 = SHA256.Create();
            byte [] hash = sHA256.ComputeHash(Encoding.Default.GetBytes(data));
            return hash;
        }

        public static bool VerifySignature(byte[] publickKey, byte [] signature, byte [] hashToVerify)
        {
            EllipticCurveDsa ellipticCurveDsa = new EllipticCurveDsa(CurveOID, EllipticCurveAlgorithm.EcDsaSha2Nistp256);
            ellipticCurveDsa.FromPublicKey(publickKey); // build the curve from the public key

            return ellipticCurveDsa.VerifyHash(hashToVerify, signature);
        }
    }
}
