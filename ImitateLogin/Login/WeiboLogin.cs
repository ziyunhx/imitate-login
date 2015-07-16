using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TNIdea.Common.Helper;

namespace ImitateLogin
{
    public class WeiboLogin : ILogin
	{
		private string servertime, nonce, rsakv, weibo_rsa_n, prelt;
		public CookieContainer cookies { set; get;}

		/// <summary>
		/// weibo登录获取Cookies
		/// </summary>
		/// <param name="UserName">用户名</param>
		/// <param name="Password">密码</param>
		/// <returns>Login result</returns>
		public LoginResult DoLogin(string UserName, string Password)
		{
			cookies = new CookieContainer();
			try
			{
				if (GetPreloginStatus(UserName))
				{
					string login_url = "https://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)&_=" + TimeHelper.ConvertDateTimeInt(DateTime.Now).ToString();
					string login_data = "entry=account&gateway=1&from=&savestate=30&useticket=0&pagerefer=&vsnf=1&su=" + get_user(UserName)
						+ "&service=sso&servertime=" + servertime + "&nonce=" + nonce + "&pwencode=rsa2&rsakv=" + rsakv + "&sp=" + get_pwa_rsa(Password)
						+ "&sr=1440*900&encoding=UTF-8&cdult=3&domain=sina.com.cn&prelt=" + prelt + "&returntype=TEXT";

					string Content = HttpHelper.GetHttpContent(login_url, login_data, cookies);

					Match m2 = Regex.Match(Content, @"crossDomainUrlList"":\[""(?<refreshUrl>.*?)""");
					if (m2.Success)
					{
						HttpHelper.GetHttpContent(m2.Groups["refreshUrl"].Value.Replace("\\", ""), cookies: cookies, referer: login_url);
					}

					string home_url = "http://weibo.com/tnidea/";
					string result = HttpHelper.GetHttpContent(home_url, cookies: cookies);

					if (string.IsNullOrWhiteSpace(result) || result.Contains("账号存在异常") || !result.Contains("$CONFIG['islogin']='1'"))
					{
						return new LoginResult(){Result= ResultType.AccounntLimit,Msg= "Fail, Msg: Login fail! Maybe you account is disable or captcha is needed."};
					}
				}
				else
					return new LoginResult(){Result = ResultType.ServiceError, Msg= "Error, Msg: The method is out of date, please update!"};
			}
			catch (Exception e)
			{
				return new LoginResult(){Result = ResultType.ServiceError, Msg= "Error, Msg: " + e.ToString()};
			}

			LoginResult loginResult = new LoginResult(){ Result = ResultType.Success, Msg = "Success", Cookies = HttpHelper.GetAllCookies(cookies)};

			return loginResult;
		}

		/// <summary>
		/// 获取登录前状态
		/// </summary>
		/// <param name="UserName">用户名</param>
		/// <returns>是否成功获取</returns>
		private bool GetPreloginStatus(string UserName)
		{
			try
			{
				long timestart = TimeHelper.ConvertDateTimeInt(DateTime.Now);
				string prelogin_url = "http://login.sina.com.cn/sso/prelogin.php?entry=account&callback=sinaSSOController.preloginCallBack&su=" + get_user(UserName) + "&rsakt=mod&client=ssologin.js(v1.4.15)&_=" + timestart;
				string Content = HttpHelper.GetHttpContent(prelogin_url, cookies: cookies);
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
		private string get_user(string UserName)
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
		private string get_pwa_rsa(string password)
		{
			WeiboRSA rsa = new WeiboRSA ();
			rsa.SetPublic (weibo_rsa_n, "10001");
			string data = servertime + "\t" + nonce + "\n" + password;
			return rsa.Encrypt (data).ToLower ();
		}
	}
}
