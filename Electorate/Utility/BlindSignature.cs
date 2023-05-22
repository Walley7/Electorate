using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    class BlindSignature {
        //================================================================================
        // https://gist.github.com/mjethani/e6d8b3e458ff59ef5b6e
        //================================================================================
        private byte[]                          mToken = null;
        private byte[]                          mBlindedToken = null;
        private byte[]                          mSignedBlindedToken = null;
        private byte[]                          mSignedToken = null;

        private BigInteger                      mBlindingFactor = null;
        private RsaBlindingParameters           mBlindingParameters = null;

        
        //================================================================================
        //--------------------------------------------------------------------------------
        public BlindSignature() {
            CreateToken();
        }

        //--------------------------------------------------------------------------------
        public BlindSignature(byte[] token, byte[] signedToken) {
            mToken = token.ToArray();
            if (signedToken != null)
                mSignedToken = signedToken.ToArray();
        }


        // PROCESS ================================================================================
        //--------------------------------------------------------------------------------
        public void CreateToken() {
            // Token - 16 random bytes
            mToken = new byte[16];
            new SecureRandom().NextBytes(mToken);
        }
        
        //--------------------------------------------------------------------------------
        public void Blind(AsymmetricKeyParameter publicKey) {
            // Blinding parameters
            RsaBlindingFactorGenerator blindingFactorGenerator = new RsaBlindingFactorGenerator();
            blindingFactorGenerator.Init(publicKey);
            mBlindingFactor = blindingFactorGenerator.GenerateBlindingFactor();
            mBlindingParameters = new RsaBlindingParameters((RsaKeyParameters)publicKey, mBlindingFactor);

            // Blind
            PssSigner pssSigner = new PssSigner(new RsaBlindingEngine(), new Sha256Digest(), 32);
            pssSigner.Init(true, mBlindingParameters);
            pssSigner.BlockUpdate(mToken, 0, mToken.Length);
            mBlindedToken = pssSigner.GenerateSignature();
        }

        //--------------------------------------------------------------------------------
        public void SetSignature(byte[] signedBlindedToken, AsymmetricKeyParameter publicKey = null, BigInteger blindingFactor = null) {
            // Signature
            mSignedBlindedToken = signedBlindedToken.ToArray(); // Copy

            // Blinding parameters
            if ((publicKey != null) && (blindingFactor != null))
                mBlindingParameters = new RsaBlindingParameters((RsaKeyParameters)publicKey, blindingFactor);

            // Unblind
            RsaBlindingEngine blindingEngine = new RsaBlindingEngine();
            blindingEngine.Init(false, mBlindingParameters);
            mSignedToken = blindingEngine.ProcessBlock(mSignedBlindedToken, 0, mSignedBlindedToken.Length);
        }
        
        //--------------------------------------------------------------------------------
        public bool VerifySignature(AsymmetricKeyParameter publicKey) {
            PssSigner pssSigner = new PssSigner(new RsaEngine(), new Sha256Digest(), 32);
            pssSigner.Init(false, publicKey);
            pssSigner.BlockUpdate(mToken, 0, mToken.Length);
            return pssSigner.VerifySignature(mSignedToken);
        }


        // TOKENS ================================================================================
        //--------------------------------------------------------------------------------
        public byte[] Token { get { return mToken; } }
        public byte[] BlindedToken { get { return mBlindedToken; } }
        public BigInteger BlindingFactor { get { return mBlindingFactor; } }
        public byte[] SignedBlindedToken { get { return mSignedBlindedToken; } }
        public byte[] SignedToken { get { return mSignedToken; } }


        // SIGNING ================================================================================
        //--------------------------------------------------------------------------------
        public static byte[] Sign(byte[] blindedToken, AsymmetricKeyParameter privateKey) {
            RsaEngine engine = new RsaEngine();
            engine.Init(false, privateKey);
            return engine.ProcessBlock(blindedToken, 0, blindedToken.Length);
        }
    }

}
