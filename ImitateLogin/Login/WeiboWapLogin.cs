using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Thrinax.Http;

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
    [ExportMetadata("loginSite", "WeiboWap")]
    public class WeiboWapLogin : ILogin
	{
		public CookieContainer cookies { set; get;}

		private string regexStr = @"form action=""(?<randUrl>[^""]+)""[\s\S]*?type=""password"" "
                + @"name=""(?<pwName>[^""]*?)""[\s\S]*?name=""backURL"" value=""(?<backURL>[^""]*?)""[\s\S]*?"
                + @"name=""backTitle"" value=""(?<backTitle>[^""]*?)""[\s\S]*?name=""vk"" value=""(?<vkValue>[^""]*?)""";

		private string postFmt = "mobile={0}&{1}={2}&remember=on&backURL={3}&backTitle={4}&vk={5}&submit=" 
			+ HttpUtility.UrlEncode("登录", Encoding.UTF8);

		/// <summary>
		/// Dos the login.
		/// </summary>
		/// <returns>The login.</returns>
		/// <param name="UserName">User name.</param>
		/// <param name="Password">Password.</param>
		public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
		{
			cookies = new CookieContainer();

			try
			{
                string preContent = HttpHelper.GetHttpContent("https://login.weibo.cn/login/", cookies: cookies, userAgent: UserAgent);

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

						string LoginContent = HttpHelper.GetHttpContent(url, postData, cookies, UserAgent, referer: "https://login.weibo.cn/login/");

						//验证是否登录成功
						if (!LoginContent.Contains("我的首页") || LoginContent.Contains("你的账号存在异常"))
						{
							return new LoginResult(){Result = ResultType.AccounntLimit, Msg = "Fail, Msg: Login fail! Maybe you account is disable or captcha is needed."};
						}
					}
					else
						return new LoginResult(){Result = ResultType.ServiceError, Msg = "Error, Msg: The method is out of date, please update!"};
				}
			}
			catch (Exception e)
			{
				return new LoginResult(){Result = ResultType.ServiceError, Msg = "Error, Msg: " + e.ToString()};
			}

			LoginResult loginResult = new LoginResult (){Result= ResultType.Success, Msg = "Success", Cookies = HttpHelper.GetAllCookies(cookies)};

			return loginResult;
		}
	}
}


