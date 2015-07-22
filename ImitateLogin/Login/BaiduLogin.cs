using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TNIdea.Common.Helper;

namespace ImitateLogin
{
    public class BaiduLogin : ILogin
    {
        private string rsa_pub_baidu = "";

        public CookieContainer cookies { get; set; }

        public LoginResult DoLogin(string UserName, string Password)
        {
            cookies = new CookieContainer();

            try
            {
                //1. Get the token.
                string passApiUrl = "https://passport.baidu.com/passApi/html/_blank.html";
                HttpHelper.GetHttpContent(passApiUrl, null, cookies);

                string token_url = "https://passport.baidu.com/v2/api/?getapi&tpl=mn&apiver=v3&class=login&logintype=dialogLogin";
                string prepareContent = HttpHelper.GetHttpContent(token_url, null, cookies, referer: "https://www.baidu.com/", encode: Encoding.GetEncoding("GB2312"));
                //string prepareJson = prepareContent.Split('(')[1].Split(')')[0];
                dynamic prepareJson = JsonConvert.DeserializeObject(prepareContent.Substring(0, prepareContent.LastIndexOf('}') + 1));
                string token = prepareJson.data.token;

                //2. Get public key
                string pubkey_url = "https://passport.baidu.com/v2/getpublickey?token={0}&tpl=mn&apiver=v3";
                string pubkeyContent = HttpHelper.GetHttpContent(string.Format(pubkey_url, token), null, cookies, referer: "https://www.baidu.com/", encode: Encoding.GetEncoding("GB2312"));
                //string prepareJson = prepareContent.Split('(')[1].Split(')')[0];
                dynamic pubkeyJson = JsonConvert.DeserializeObject(pubkeyContent.Substring(0, pubkeyContent.LastIndexOf('}') + 1));
                rsa_pub_baidu = pubkeyJson.pubkey;
                string KEY = pubkeyJson.key;

                //3. Build post data
                string login_data = "staticpage=https%3A%2F%2Fwww.baidu.com%2Fcache%2Fuser%2Fhtml%2Fv3Jump.html&charset=UTF-8&token={0}&tpl=mn&subpro=&apiver=v3&tt={1}&codestring=&safeflg=0&u=https%3A%2F%2Fwww.baidu.com%2F&isPhone=false&detect=1&quick_user=0&logintype=dialogLogin&logLoginType=pc_loginDialog&idc=&loginmerge=true&splogin=rate&username={2}password={3}&verifycode=&mem_pass=on&rsakey={4}&crypttype=12&ppui_logintime=14941&gid={5}";
                login_data = string.Format(login_data, token, TimeHelper.ConvertDateTimeInt(DateTime.Now), HttpUtility.UrlEncode(UserName, Encoding.UTF8), get_pwa_rsa(Password), KEY, Guid.NewGuid().ToString());

                //4. Post the login data
                string login_url = "https://passport.baidu.com/v2/api/?login";

                string Content = HttpHelper.GetHttpContent(login_url, login_data, cookies, referer: "https://www.baidu.com/");

                Match m2 = Regex.Match(Content, @"crossDomainUrlList"":\[""(?<refreshUrl>.*?)""");
                if (m2.Success)
                {
                    HttpHelper.GetHttpContent(m2.Groups["refreshUrl"].Value.Replace("\\", ""), cookies: cookies, referer: login_url);
                }

                string home_url = "https://www.baidu.com";
                string result = HttpHelper.GetHttpContent(home_url, cookies: cookies);

                //5. Verifty the login result
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

        /// <summary>
		/// 获取RSA加密后的密码密文
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private string get_pwa_rsa(string password)
        {
            RSAHelper rsa = new RSAHelper();
            rsa.SetPublic(rsa_pub_baidu, "10001");
            string data = password;
            return rsa.Encrypt(data).ToLower();
        }
    }
}
