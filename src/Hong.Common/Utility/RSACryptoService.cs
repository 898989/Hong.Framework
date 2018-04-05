using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/*
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
*/

namespace Hong.Common.Tools
{
    public class RSACryptoService
    {
        const int EncryptSize = 117;
        const int DecryptSize = 128;
        const int OUTPUT_SIZE = 8 * 1024;

        private RSAEncryptionPadding _rsaEncryptionPadding;
        private RSA _privateKeyRsaProvider;
        private RSA _publicKeyRsaProvider;

        #region KEY PEM格式转XML格式
        /*
        public string PemToXml(string pem)
        {
            if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
                || pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
            {
                return GetXmlRsaKey(pem, false, true);
            }

            if (pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
            {
                string key= GetXmlRsaKey(pem, true, false);
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "pubkey.key", key, Encoding.UTF8);
                return key;
            }

            throw new InvalidKeyException("Unsupported PEM format...");
        }

        private string GetXmlRsaKey(string pem, bool isPublicKey, bool inCludPrivateCrtKeyParameters)
        {
            using (var ms = new MemoryStream())
            using (var sw = new StreamWriter(ms))
            using (var sr = new StreamReader(ms))
            {
                sw.Write(pem);
                sw.Flush();
                ms.Position = 0;
                var pr = new PemReader(sr);
                object keyPair = pr.ReadObject();
                RSA rsa = null;

                if (isPublicKey) rsa = GetPubKeyRSA(keyPair);
                else rsa = GetPriveKeyRSA(keyPair);

                var xml = GetKey(rsa, inCludPrivateCrtKeyParameters);
                rsa.Clear();
                rsa = null;

                return xml;
            }
        }

        private RSA GetPubKeyRSA(object obj)
        {
            var publicKey = (RsaKeyParameters)obj;
            return DotNetUtilities.ToRSA(publicKey);
        }

        private RSA GetPriveKeyRSA(object obj)
        {
            if ((obj as RsaPrivateCrtKeyParameters) != null)
            {
                return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)obj);
            }

            var keyPair = (AsymmetricCipherKeyPair)obj;
            return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private);
        }

        private string GetKey(RSA rsa, bool inCludPrivateCrtKeyParameters)
        {
            return rsa.ToXmlString(inCludPrivateCrtKeyParameters);
        }
        */
        #endregion

        public RSACryptoService(RSAEncryptionPadding rsaEncryptionPadding,string privateKeyPath, string publicKeyPath = null)
        {
            _rsaEncryptionPadding = rsaEncryptionPadding;

            if (!string.IsNullOrEmpty(privateKeyPath))
            {
                _privateKeyRsaProvider = CreateRsaProviderFromPrivateKey(privateKeyPath); 
            }

            if (!string.IsNullOrEmpty(publicKeyPath))
            {
                _publicKeyRsaProvider = new RSACryptoServiceProvider();
                //_publicKeyRsaProvider.FromXmlString(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "pubkey.key"));
                //_publicKeyRsaProvider.FromXmlString(PemToXml(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "apiPub.key")));
                _publicKeyRsaProvider = CreateRsaProviderFromPublicKey(File.ReadAllText(publicKeyPath));
            }
        }

        #region Decrypt

        public string Decrypt(string cipherText)
        {
            return Encoding.UTF8.GetString(_privateKeyRsaProvider.Decrypt(System.Convert.FromBase64String(cipherText), _rsaEncryptionPadding));
        }

        public byte[] Decrypt(byte[] data)
        {
            return _privateKeyRsaProvider.Decrypt(data, _rsaEncryptionPadding);
        }

        #endregion

        #region Encrypt

        public byte[] Encrypt(string text)
        {
            if (_publicKeyRsaProvider == null)
            {
                throw new Exception("_publicKeyRsaProvider is null");
            }

            return EncryptToByets(Encoding.UTF8.GetBytes(text));
        }

        public byte[] EncryptToByets(byte[] data)
        {
            return Process(_publicKeyRsaProvider, EncryptSize, data);
        }

        public byte[] EncryptToByets(string data)
        {
            return Process(_publicKeyRsaProvider, EncryptSize, Encoding.UTF8.GetBytes(data));
        }

        #endregion

        private RSA CreateRsaProviderFromPrivateKey(string privateKey)
        {
            privateKey = privateKey.Replace("\r", "").Replace("\n", "")
                .Replace("-----BEGIN RSA PRIVATE KEY-----", "")
                .Replace("-----END RSA PRIVATE KEY-----", "")
                .Replace("-----BEGIN PRIVATE KEY-----", "")
                .Replace("-----END PRIVATE KEY-----", "");

            var privateKeyBits = System.Convert.FromBase64String(privateKey);

            var rsa = RSA.Create();
            var rsaParams = new RSAParameters();

            using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
            {
                byte bt = 0;
                ushort twobytes = 0;
                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)
                    binr.ReadByte();
                else if (twobytes == 0x8230)
                    binr.ReadInt16();
                else
                    throw new Exception("Unexpected value read binr.ReadUInt16()");

                twobytes = binr.ReadUInt16();
                if (twobytes != 0x0102)
                    throw new Exception("Unexpected version");

                bt = binr.ReadByte();
                if (bt != 0x00)
                    throw new Exception("Unexpected value read binr.ReadByte()");

                rsaParams.Modulus = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.Exponent = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.D = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.P = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.Q = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.DP = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.DQ = binr.ReadBytes(GetIntegerSize(binr));
                rsaParams.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
            }

            rsa.ImportParameters(rsaParams);
            return rsa;
        }

        private int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
                count = binr.ReadByte();
            else
                if (bt == 0x82)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;
            }

            while (binr.ReadByte() == 0x00)
            {
                count -= 1;
            }
            binr.BaseStream.Seek(-1, SeekOrigin.Current);
            return count;
        }

        private RSA CreateRsaProviderFromPublicKey(string publicKeyString)
        {
            publicKeyString = publicKeyString.Replace("\r", "").Replace("\n", "")
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "");

            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] x509key;
            byte[] seq = new byte[15];
            int x509size;

            x509key = Convert.FromBase64String(publicKeyString);
            x509size = x509key.Length;

            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
                {
                    byte bt = 0;
                    ushort twobytes = 0;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    seq = binr.ReadBytes(15);       //read the Sequence OID
                    if (!CompareBytearrays(seq, SeqOID))    //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;

                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    var firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    var modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;
                    int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);

                    return rsa;
                }
            }
        }

        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;

            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }
            return true;
        }

        public byte[] Process(RSA provider, int blockSize, byte[] input)
        {
            if (input.Length <= blockSize)
            {
                return provider.Encrypt(input, _rsaEncryptionPadding);
            }

            byte[] output = new byte[OUTPUT_SIZE];
            int outputSize = 0;
            byte[] tmpArray = null;

            for (int i = 0; ; i += blockSize)
            {
                if (i + blockSize < input.Length)
                {
                    tmpArray = provider.Encrypt(Extendsion.Array.Cut(input, i, blockSize), _rsaEncryptionPadding);
                    outputSize += Extendsion.Array.Merge(tmpArray, output, outputSize);
                }
                else
                {
                    tmpArray = provider.Encrypt(Extendsion.Array.Cut(input, i, input.Length - i), _rsaEncryptionPadding);
                    outputSize += Extendsion.Array.Merge(tmpArray, output, outputSize);
                    break;
                }
            }

            return Extendsion.Array.Cut(input, 0, outputSize); // .Array.Cut(input,0, outputSize);
        }
    }
}
