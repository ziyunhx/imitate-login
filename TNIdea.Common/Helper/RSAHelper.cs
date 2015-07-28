using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TNIdea.Common.Helper
{
    public class RSAHelper
    {
        private static Random rand = new Random();
        public const string pempubheader = "-----BEGIN PUBLIC KEY-----";
        public const string pempubfooter = "-----END PUBLIC KEY-----";

        private BigInteger Modulus = 0;
        private BigInteger Exponent = 0;

        private BigInteger AE(string a, int b)
        {
            if (b < a.Length + 11)
                throw new Exception("Message too long for RSA");

            byte[] c = new byte[b];
            int d = a.Length - 1;
            while (d >= 0 && b > 0)
            {
                int e = (int)a[d--];
                if (e < 128)
                {
                    c[--b] = Convert.ToByte(e);
                }
                else if ((e > 127) && (e < 2048))
                {
                    c[--b] = Convert.ToByte(((e & 63) | 128));
                    c[--b] = Convert.ToByte((e >> 6) | 192);
                }
                else
                {
                    c[--b] = Convert.ToByte((e & 63) | 128);
                    c[--b] = Convert.ToByte(((e >> 6) & 63) | 128);
                    c[--b] = Convert.ToByte(((e >> 12) | 224));
                }
            }

            c[--b] = Convert.ToByte(0);
            byte[] temp = new byte[1];
            while (b > 2)
            {
                temp[0] = Convert.ToByte(0);
                while (temp[0] == 0)
                    rand.NextBytes(temp);
                c[--b] = temp[0];
            }
            c[--b] = 2;
            c[--b] = 0;
            return new BigInteger(c);
        }

        public void SetPublic(string pemstr)
        {
            byte[] pempublickey = DecodeOpenSSLPublicKey(pemstr);
            if (pempublickey != null)
            {
                DecodePublicKey(pempublickey);
            }
        }

        /// <summary>
        /// Parses binary asn.1 X509 SubjectPublicKeyInfo
        /// </summary>
        /// <param name="x509key"></param>
        private void DecodePublicKey(byte[] x509key)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(x509key);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return;

                seq = binr.ReadBytes(15);		//read the Sequence OID
                if (!CompareBytearrays(seq, SeqOID))	//make sure Sequence for OID is correct
                    return;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103)	//data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return;

                bt = binr.ReadByte();
                if (bt != 0x00)		//expect null byte next
                    return;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130)	//data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();	//advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();	//advance 2 bytes
                else
                    return;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102)	//data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();	// read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte();	//advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return;
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {	//if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();	//skip this null byte
                    modsize -= 1;	//reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);	//read the modulus bytes

                Modulus = new BigInteger(modulus);

                //BigInteger {0: 209999335, 1: 94837678, 2: 175986845, 3: 230135138, 4: 268304553, 5: 195320486, 6: 56589681, 7: 171621713, 8: 120215536, 9: 110044962, 10: 186213019, 11: 172508023, 12: 151535381, 13: 113516115, 14: 151621612, 15: 10796550, 16: 231157203, 17: 65670079, 18: 251299262, 19: 159535808, 20: 61312987, 21: 158963780, 22: 251665809, 23: 230017830, 24: 141990593, 25: 181559067, 26: 155478777, 27: 149700697, 28: 115985251, 29: 194627468, 30: 162285189, 31: 239411989, 32: 37571148, 33: 21890124, 34: 111156872, 35: 77748488, 36: 50903, t: 37, s: 0}

                if (binr.ReadByte() != 0x02)			//expect an Integer for the exponent data
                    return;
                int expbytes = (int)binr.ReadByte();		// should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                //65537
                Exponent = new BigInteger(exponent);
            }
            catch (Exception)
            {
                return;
            }

            finally { binr.Close(); }
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

        /// <summary>
        /// Get the binary RSA PUBLIC key
        /// </summary>
        /// <param name="instr"></param>
        /// <returns></returns>
        private byte[] DecodeOpenSSLPublicKey(string instr)
        {
            string pemstr = instr.Trim();
            byte[] binkey;

            string baiduRSARegex = @"-----BEGIN [^-]+-----([A-Za-z0-9+\/=\s]+)-----END [^-]+-----|begin-base64[^\n]+\n([A-Za-z0-9+\/=\s]+)====";

            string pubstr = "";
            Match m = Regex.Match(instr, baiduRSARegex);
            if (m.Success)
                pubstr = m.Groups[1].Value;

            try
            {
                binkey = Convert.FromBase64String(pubstr);
            }
            catch (System.FormatException)
            {
                //if can't b64 decode, data is not valid
                return null;
            }
            return binkey;
        }

        public void SetPublic(string modulus, string exponent)
        {
            if (string.IsNullOrEmpty(modulus) || string.IsNullOrEmpty(exponent))
                throw new Exception("Invalid RSA public key");
            Modulus = new BigInteger(modulus, 16);
            Exponent = new BigInteger(exponent, 16);
        }

        private BigInteger RSADoPublic(BigInteger x)
        {
            return x.modPow(Exponent, Modulus);
        }

        public string Encrypt(string a)
        {
            BigInteger tmp = AE(a, (Modulus.bitCount() + 7) >> 3);
            tmp = RSADoPublic(tmp);
            string result = tmp.ToHexString();
            return (result.Length & 1) == 0 ? result : "0" + result;
        }
    }
}
