using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ImitateLogin.Core
{
    public static class WeiboLogin
    {
        private static string servertime, nonce, rsakv, weibo_rsa_n, prelt;
        private static CookieContainer _cookies;

        /// <summary>
        /// weibo登录获取Cookies
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <returns>Cookies</returns>
        public static string DoLogin(string UserName, string Password)
        {
            _cookies = new CookieContainer();
            string cookies = null;
            try
            {
                if (GetPreloginStatus(UserName))
                {
                    string login_url = "https://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)&_=" + TimeHelper.ConvertDateTimeInt(DateTime.Now).ToString();
                    string login_data = "entry=account&gateway=1&from=&savestate=30&useticket=0&pagerefer=&vsnf=1&su=" + get_user(UserName)
                        + "&service=sso&servertime=" + servertime + "&nonce=" + nonce + "&pwencode=rsa2&rsakv=" + rsakv + "&sp=" + get_pwa_rsa(Password)
                        + "&sr=1440*900&encoding=UTF-8&cdult=3&domain=sina.com.cn&prelt=" + prelt + "&returntype=TEXT";

                    string Content = HttpHelper.GetHttpContent(login_url, login_data, _cookies);

                    Match m2 = Regex.Match(Content, @"crossDomainUrlList"":\[""(?<refreshUrl>.*?)""");
                    if (m2.Success)
                    {
                        string refreshLogin = HttpHelper.GetHttpContent(m2.Groups["refreshUrl"].Value.Replace("\\", ""), cookies: _cookies, referer: login_url);
                    }

                    string home_url = "http://weibo.com/tnidea/";
                    string result = HttpHelper.GetHttpContent(home_url, cookies: _cookies);
                    cookies = _cookies.GetCookieHeader(new Uri("http://weibo.com"));

                    if (string.IsNullOrWhiteSpace(result) || result.Contains("账号存在异常") || !result.Contains("$CONFIG['islogin']='1'"))
                    {
                        return "Fail, Msg: Login fail! Maybe you account is disable or captcha is needed.";
                    }
                }
                else
                    return "Error, Msg: The method is out of date, please update!";
            }
            catch (Exception e)
            {
                return "Error, Msg: " + e.ToString();
            }
            return cookies;
        }

        /// <summary>
        /// 获取登录前状态
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns>是否成功获取</returns>
        private static bool GetPreloginStatus(string UserName)
        {
            try
            {
                long timestart = TimeHelper.ConvertDateTimeInt(DateTime.Now);
                string prelogin_url = "http://login.sina.com.cn/sso/prelogin.php?entry=account&callback=sinaSSOController.preloginCallBack&su=" + get_user(UserName) + "&rsakt=mod&client=ssologin.js(v1.4.15)&_=" + timestart;
                string Content = HttpHelper.GetHttpContent(prelogin_url, cookies: _cookies);
                long dateTimeEndPre = TimeHelper.ConvertDateTimeInt(DateTime.Now);

                prelt = Math.Max(dateTimeEndPre - timestart, 50).ToString();
                string regex = @"""servertime"":(?<servertime>\d*).*?""nonce"":""(?<nonce>[^""]*?)"",""pubkey"":""(?<pubkey>[^""]*?)"",""rsakv"":""(?<rsakv>[^""]*?)"".*?""exectime"":(?<exectime>\d*)";
                Match m = Regex.Match(Content, regex);
                if (m.Success && m.Groups["servertime"].Success) //验证一个
                {
                    servertime = m.Groups["servertime"].Value;
                    nonce = m.Groups["nonce"].Value;
                    weibo_rsa_n = m.Groups["pubkey"].Value;
                    rsakv = m.Groups["rsakv"].Value;
                    return true;
                }
                return false;
            }
            catch // (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取Base64加密的UserName
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        private static string get_user(string UserName)
        {
            UserName = HttpUtility.UrlEncode(UserName, Encoding.UTF8);
            byte[] bytes = Encoding.Default.GetBytes(UserName);
            return HttpUtility.UrlEncode(Convert.ToBase64String(bytes), Encoding.UTF8);
        }

        /// <summary>
        /// 获取RSA加密后的密码密文
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string get_pwa_rsa(string password)
        {
            WeiboRSA rsa = new WeiboRSA();
            rsa.SetPublic(weibo_rsa_n, "10001");
            string data = servertime + "\t" + nonce + "\n" + password;
            return rsa.Encrypt(data).ToLower();

            //UTF8Encoding ue = new UTF8Encoding();
            //byte[] Exponent = { 1, 0, 1 };//65537
            //byte[] PublicKey = FromHex(weibo_rsa_n); //ue.GetBytes(weibo_rsa_n);
            //string data = servertime + "\t" + nonce + "\n" + password;
            //byte[] Transformation = ue.GetBytes(data);
            //byte[] EncryptedSymmetricData;

            //RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            //RSAParameters RSAKeyInfo = new RSAParameters();//RSA算法的标准参数的结构
            //RSAKeyInfo.Modulus = PublicKey;
            //RSAKeyInfo.Exponent = Exponent;
            //RSA.ImportParameters(RSAKeyInfo);//导入公钥
            //EncryptedSymmetricData = RSA.Encrypt(Transformation, true);//加密           

            //return GetHexString(EncryptedSymmetricData);
        }

        private const int AllocateThreshold = 256;
        private const string UpperHexChars = "0123456789ABCDEF";
        private const string LowerhexChars = "0123456789abcdef";
        private static string[] upperHexBytes;
        private static string[] lowerHexBytes;

        public static string GetHexString(this byte[] value)
        {
            return GetHexString(value, false);
        }

        public static string GetHexString(this byte[] value, bool upperCase)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length == 0)
            {
                return string.Empty;
            }

            if (upperCase)
            {
                if (upperHexBytes != null)
                {
                    return ToHexStringFast(value, upperHexBytes);
                }

                if (value.Length > AllocateThreshold)
                {
                    return ToHexStringFast(value, UpperHexBytes);
                }

                return ToHexStringSlow(value, UpperHexChars);
            }

            if (lowerHexBytes != null)
            {
                return ToHexStringFast(value, lowerHexBytes);
            }

            if (value.Length > AllocateThreshold)
            {
                return ToHexStringFast(value, LowerHexBytes);
            }

            return ToHexStringSlow(value, LowerhexChars);
        }

        private static string ToHexStringSlow(byte[] value, string hexChars)
        {
            var hex = new char[value.Length * 2];
            int j = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var b = value[i];
                hex[j++] = hexChars[b >> 4];
                hex[j++] = hexChars[b & 15];
            }

            return new string(hex);
        }

        private static string ToHexStringFast(byte[] value, string[] hexBytes)
        {
            var hex = new char[value.Length * 2];
            int j = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var s = hexBytes[value[i]];
                hex[j++] = s[0];
                hex[j++] = s[1];
            }

            return new string(hex);
        }

        private static byte[] FromHex(string hex)
        {

            if (string.IsNullOrEmpty(hex) || hex.Length % 2 != 0)
                throw new ArgumentException("not a hexidecimal string");

            List<byte> bytes = new List<byte>();
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes.Add(Convert.ToByte(hex.Substring(i, 2), 16));
            }

            return bytes.ToArray();
        }

        private static string[] UpperHexBytes
        {
            get
            {
                return (upperHexBytes ?? (upperHexBytes = new[] {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1A", "1B", "1C", "1D", "1E", "1F",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2A", "2B", "2C", "2D", "2E", "2F",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3A", "3B", "3C", "3D", "3E", "3F",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4A", "4B", "4C", "4D", "4E", "4F",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5A", "5B", "5C", "5D", "5E", "5F",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6A", "6B", "6C", "6D", "6E", "6F",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7A", "7B", "7C", "7D", "7E", "7F",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8A", "8B", "8C", "8D", "8E", "8F",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9A", "9B", "9C", "9D", "9E", "9F",
                "A0", "A1", "A2", "A3", "A4", "A5", "A6", "A7", "A8", "A9", "AA", "AB", "AC", "AD", "AE", "AF",
                "B0", "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "BA", "BB", "BC", "BD", "BE", "BF",
                "C0", "C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CA", "CB", "CC", "CD", "CE", "CF",
                "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DA", "DB", "DC", "DD", "DE", "DF",
                "E0", "E1", "E2", "E3", "E4", "E5", "E6", "E7", "E8", "E9", "EA", "EB", "EC", "ED", "EE", "EF",
                "F0", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "FA", "FB", "FC", "FD", "FE", "FF" }));
            }
        }

        private static string[] LowerHexBytes
        {
            get
            {
                return (lowerHexBytes ?? (lowerHexBytes = new[] {
                "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0a", "0b", "0c", "0d", "0e", "0f",
                "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "1a", "1b", "1c", "1d", "1e", "1f",
                "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "2a", "2b", "2c", "2d", "2e", "2f",
                "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "3a", "3b", "3c", "3d", "3e", "3f",
                "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "4a", "4b", "4c", "4d", "4e", "4f",
                "50", "51", "52", "53", "54", "55", "56", "57", "58", "59", "5a", "5b", "5c", "5d", "5e", "5f",
                "60", "61", "62", "63", "64", "65", "66", "67", "68", "69", "6a", "6b", "6c", "6d", "6e", "6f",
                "70", "71", "72", "73", "74", "75", "76", "77", "78", "79", "7a", "7b", "7c", "7d", "7e", "7f",
                "80", "81", "82", "83", "84", "85", "86", "87", "88", "89", "8a", "8b", "8c", "8d", "8e", "8f",
                "90", "91", "92", "93", "94", "95", "96", "97", "98", "99", "9a", "9b", "9c", "9d", "9e", "9f",
                "a0", "a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "aa", "ab", "ac", "ad", "ae", "af",
                "b0", "b1", "b2", "b3", "b4", "b5", "b6", "b7", "b8", "b9", "ba", "bb", "bc", "bd", "be", "bf",
                "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf",
                "d0", "d1", "d2", "d3", "d4", "d5", "d6", "d7", "d8", "d9", "da", "db", "dc", "dd", "de", "df",
                "e0", "e1", "e2", "e3", "e4", "e5", "e6", "e7", "e8", "e9", "ea", "eb", "ec", "ed", "ee", "ef",
                "f0", "f1", "f2", "f3", "f4", "f5", "f6", "f7", "f8", "f9", "fa", "fb", "fc", "fd", "fe", "ff" }));
            }
        }
    }
}
