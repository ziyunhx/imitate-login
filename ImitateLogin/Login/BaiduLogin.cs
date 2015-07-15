using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ImitateLogin
{
    public class BaiduLogin : ILogin
    {
        public CookieContainer cookies { get; set; }

        public LoginResult DoLogin(string UserName, string Password)
        {
            cookies = new CookieContainer();

            try
            {
                //1. Get the token.
				string token_url = "https://passport.baidu.com/v2/api/?getapi&tpl=mn&apiver=v3&class=login&logintype=dialogLogin";
                string prepareContent = HttpHelper.GetHttpContent(token_url, null, cookies);

                dynamic json = JsonConvert.DeserializeObject(prepareContent);
                string token = json.data.token;

                //2. Build post data
                string login_data = "";

                //3. Post the login data
                string login_url = "https://passport.baidu.com/v2/api/?login";

                string Content = HttpHelper.GetHttpContent(login_url, login_data, cookies);

                Match m2 = Regex.Match(Content, @"crossDomainUrlList"":\[""(?<refreshUrl>.*?)""");
                if (m2.Success)
                {
                    HttpHelper.GetHttpContent(m2.Groups["refreshUrl"].Value.Replace("\\", ""), cookies: cookies, referer: login_url);
                }

                string home_url = "https://www.baidu.com";
                string result = HttpHelper.GetHttpContent(home_url, cookies: cookies);

                //4. Verifty the login result
                if (string.IsNullOrWhiteSpace(result) || result.Contains("账号存在异常") || !result.Contains("$CONFIG['islogin']='1'"))
                {
                    return new LoginResult() { Result = ResultType.AccounntLimit, Msg = "Fail, Msg: Login fail! Maybe you account is disable or captcha is needed." };
                }
               
            }
            catch (Exception e)
            {
                return new LoginResult() { Result = ResultType.ServiceError, Msg = "Error, Msg: " + e.ToString() };
            }

            LoginResult loginResult = new LoginResult() { Result = ResultType.Success, Msg = "Success", Cookies = HttpHelper.GetAllCookies(cookies) };

            return loginResult;
        }
    }
}
