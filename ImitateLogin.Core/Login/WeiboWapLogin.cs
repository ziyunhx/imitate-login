using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ImitateLogin.Core
{
    public class WeiboWapLogin
    {
        private static CookieContainer _cookies = new CookieContainer();
        private static string regexStr = @"form action=""(?<randUrl>[^""]+)""[\s\S]*?type=""password"" "
                + @"name=""(?<pwName>[^""]*?)""[\s\S]*?name=""backURL"" value=""(?<backURL>[^""]*?)""[\s\S]*?"
                + @"name=""backTitle"" value=""(?<backTitle>[^""]*?)""[\s\S]*?name=""vk"" value=""(?<vkValue>[^""]*?)""";

        private static string postFmt = "mobile={0}&{1}={2}&remember=on&backURL={3}&backTitle={4}&vk={5}&submit=" 
                + HttpUtility.UrlEncode("登录", Encoding.UTF8);

        public static string DoLogin(string UserName, string Password)
        {
            string cookies = "";
            
            try
            {
                string preContent = HttpHelper.GetHttpContent("https://login.weibo.cn/login/", cookies: _cookies);

                if(!string.IsNullOrWhiteSpace(preContent))
                {
                    Match m = Regex.Match(preContent, regexStr);
                    if (m.Success)
                    {
                        string randUrl = m.Groups["randUrl"].Value;
                        string pwName = m.Groups["pwName"].Value;
                        string vkValue = m.Groups["vkValue"].Value;
                        string backURL = m.Groups["backURL"].Value;
                        string backTitle = m.Groups["backTitle"].Value;

                        string url = "https://login.weibo.cn/login/" + randUrl;
                        string postData = string.Format(postFmt, HttpUtility.UrlEncode(UserName, Encoding.UTF8), pwName, Password
                                                     , HttpUtility.UrlEncode(backURL, Encoding.UTF8), HttpUtility.UrlEncode(backTitle, Encoding.UTF8), vkValue);

                        string LoginContent = HttpHelper.GetHttpContent(url, postData, _cookies, referer: "https://login.weibo.cn/login/");
                        cookies = _cookies.GetCookieHeader(new Uri("http://weibo.cn"));

                        string testContent = HttpHelper.GetHttpContent("http://weibo.cn/?PHPSESSID=&vt=4", cookies: _cookies, referer: "http://weibo.cn");
                    }
                }
            }
            catch (Exception e)
            {
            }

            return cookies;
        }
    }
}


