using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Web;
using Thrinax.Decrypt;
using Thrinax.Http;
using Thrinax.Utility;

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
    [ExportMetadata("loginSite", "Weibo")]
    /// <summary>
    /// Weibo Login
    /// </summary>
    public class WeiboLogin : ILogin
	{
		private string servertime, nonce, rsakv, weibo_rsa_n, prelt;
		public CookieContainer cookies { set; get;}

		/// <summary>
		/// weibo login
		/// </summary>
		/// <param name="UserName">user name</param>
		/// <param name="Password">password</param>
		/// <returns>Login result</returns>
		public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
		{
			cookies = new CookieContainer();
			try
			{
				if (GetPreloginStatus(UserName))
				{
					string login_url = "https://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)&_=" + TimeUtility.ConvertDateTimeInt(DateTime.Now).ToString();
					string login_data = "entry=account&gateway=1&from=&savestate=30&useticket=0&pagerefer=&vsnf=1&su=" + get_user(UserName)
						+ "&service=sso&servertime=" + servertime + "&nonce=" + nonce + "&pwencode=rsa2&rsakv=" + rsakv + "&sp=" + get_pwa_rsa(Password)
						+ "&sr=1440*900&encoding=UTF-8&cdult=3&domain=sina.com.cn&prelt=" + prelt + "&returntype=TEXT";

					string Content = HttpHelper.GetHttpContent(login_url, login_data, cookies);
                    dynamic refreshJson = JsonConvert.DeserializeObject(Content.Substring(0, Content.LastIndexOf('}') + 1));
                    HttpHelper.GetHttpContent(refreshJson.crossDomainUrlList[0].ToString(), cookies: cookies, referer: login_url);

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
		/// get prepare login status.
		/// </summary>
		/// <param name="UserName">user name</param>
		/// <returns>is success?</returns>
		private bool GetPreloginStatus(string UserName)
		{
			try
			{
				long timestart = TimeUtility.ConvertDateTimeInt(DateTime.Now);
				string prelogin_url = "http://login.sina.com.cn/sso/prelogin.php?entry=account&callback=sinaSSOController.preloginCallBack&su=" + get_user(UserName) + "&rsakt=mod&client=ssologin.js(v1.4.15)&_=" + timestart;
				string Content = HttpHelper.GetHttpContent(prelogin_url, cookies: cookies, encode: Encoding.GetEncoding("GB2312"));
				long dateTimeEndPre = TimeUtility.ConvertDateTimeInt(DateTime.Now);

				prelt = Math.Max(dateTimeEndPre - timestart, 50).ToString();
                dynamic prepareJson = JsonConvert.DeserializeObject(Content.Split('(')[1].Split(')')[0]);

				servertime = prepareJson.servertime;
				nonce = prepareJson.nonce;
				weibo_rsa_n = prepareJson.pubkey;
				rsakv = prepareJson.rsakv;
				return true;
			}
			catch // (Exception ex)
			{
				return false;
			}
		}

		/// <summary>
		/// Get Base64 encode UserName.
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
		/// Get RSA encrypted password.
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private string get_pwa_rsa(string password)
		{
			RSAHelper rsa = new RSAHelper ();
			rsa.SetPublic (weibo_rsa_n, "10001");
			string data = servertime + "\t" + nonce + "\n" + password;
			return rsa.Encrypt (data).ToLower ();
		}
	}
}
