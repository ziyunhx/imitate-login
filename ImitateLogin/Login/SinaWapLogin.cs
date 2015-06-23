using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ImitateLogin
{
	public class SinaWapLogin : ILogin
    {
		private string regexStr = @"form action=""(?<randUrl>[^""]+)""[\s\S]*?type=""password"" "
                + @"name=""(?<pwName>[^""]*?)""[\s\S]*?name=""backURL"" value=""(?<backURL>[^""]*?)""[\s\S]*?"
                + @"name=""backTitle"" value=""(?<backTitle>[^""]*?)""[\s\S]*?name=""vk"" value=""(?<vkValue>[^""]*?)""";

		private string postFmt = "mobile={0}&{1}={2}&remember=on&backURL={3}&backTitle={4}&vk={5}&submit=" 
			+ HttpUtility.UrlEncode("登录", Encoding.UTF8);

		public string DoLogin (string UserName, string Password)
		{
			cookies = new CookieContainer();
			string _cookies = "";

			try
			{
				string preContent = HttpHelper.GetHttpContent("http://3g.sina.com.cn/prog/wapsite/sso/login.php", cookies: cookies);

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

						string url = "http://3g.sina.com.cn/prog/wapsite/sso/" + randUrl;
						string postData = string.Format(postFmt, HttpUtility.UrlEncode(UserName, Encoding.UTF8), pwName, Password
							, HttpUtility.UrlEncode(backURL, Encoding.UTF8), HttpUtility.UrlEncode(backTitle, Encoding.UTF8), vkValue);

						string LoginContent = HttpHelper.GetHttpContent(url, postData, cookies, referer: "http://weibo.com/");

						Match m2 = Regex.Match(LoginContent, "meta http-equiv=\"refresh\".*?url=(?<refreshUrl>[^\"]+)\"");
						if (m2.Success)
						{
							HttpHelper.GetHttpContent(m2.Groups["refreshUrl"].Value, cookies: cookies, referer: url);
						}

						_cookies = cookies.GetCookieHeader(new Uri("http://sina.cn"));
					}
				}
			}
			catch (Exception e)
			{
				return "Error, Msg: " + e.ToString();
			}

			return _cookies;
		}

		public CookieContainer cookies { set; get;} 
    }
}


