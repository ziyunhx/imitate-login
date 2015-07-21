using System;

namespace TNIdea.Common.Helper
{
    public class RSAHelper
    {
        private static Random rand = new Random();

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
