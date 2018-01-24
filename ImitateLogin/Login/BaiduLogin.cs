using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Web;
using Thrinax.Decrypt;
using Thrinax.Http;
using Thrinax.Utility;

/* err code
"-1":       "系统错误,请您稍后再试",
"1":        "您输入的帐号格式不正确",
"2":        "您输入的帐号不存在",
"3":        "验证码不存在或已过期,请重新输入",
"4":        "您输入的帐号或密码有误",
"5":        "请在弹出的窗口操作,或重新登录",
"6":        "您输入的验证码有误",
"16":       "您的帐号因安全问题已被限制登录",
"257":      "请输入验证码",
"100027":   "百度正在进行系统升级，暂时不能提供服务，敬请谅解",
"400031":   "请在弹出的窗口操作,或重新登录",
"401007":   "您的手机号关联了其他帐号，请选择登录",
"120021":   "登录失败,请在弹出的窗口操作,或重新登录",
"500010":   "登录过于频繁,请24小时后再试",
"200010":   "验证码不存在或已过期",
"100005":   "系统错误,请您稍后再试",
"120019":   "请在弹出的窗口操作,或重新登录",
"110024":   "此帐号暂未激活",
"100023":   "开启Cookie之后才能登录",
"17":       "您的帐号已锁定,请解锁后登录"
*/

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
	[ExportMetadata("loginSite", "Baidu")]
    /// <summary>
    /// baidu login.
    /// </summary>
    public class BaiduLogin : ILogin
    {
        private string rsa_pub_baidu = "";

        public CookieContainer cookies { get; set; }

        public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
        {
            Stopwatch stopwatch = new Stopwatch();
            cookies = new CookieContainer();

            try
            {
                stopwatch.Start();
                HttpHelper.GetHttpContent("https://passport.baidu.com/passApi/html/_blank.html", cookies: cookies, cookiesDomain: "passport.baidu.com");

                //1. Get the token.
                string token_url = string.Format("https://passport.baidu.com/v2/api/?getapi&tpl=mn&apiver=v3&tt={0}&class=login&gid={1}&logintype=dialogLogin&callback=bd__cbs__{2}", TimeUtility.ConvertDateTimeInt(DateTime.Now), Guid.NewGuid().ToString().ToUpper(), build_callback());
                string prepareContent = HttpHelper.GetHttpContent(token_url, null, cookies, referer: "https://www.baidu.com/", encode: Encoding.GetEncoding("GB2312"), cookiesDomain: "passport.baidu.com");
                //string prepareJson = prepareContent.Split('(')[1].Split(')')[0];
                dynamic prepareJson = JsonConvert.DeserializeObject(prepareContent.Split('(')[1].Split(')')[0]);
                string token = prepareJson.data.token;

                //2. Get public key
                string pubkey_url = "https://passport.baidu.com/v2/getpublickey?token={0}&tpl=mn&apiver=v3&tt={1}&gid={2}&callback=bd__cbs__{3}";
                string pubkeyContent = HttpHelper.GetHttpContent(string.Format(pubkey_url, token, TimeUtility.ConvertDateTimeInt(DateTime.Now), Guid.NewGuid().ToString().ToUpper(), build_callback()), null, cookies, referer: "https://www.baidu.com/", encode: Encoding.GetEncoding("GB2312"), cookiesDomain: "passport.baidu.com");

                dynamic pubkeyJson = JsonConvert.DeserializeObject(pubkeyContent.Split('(')[1].Split(')')[0]);
                rsa_pub_baidu = pubkeyJson.pubkey;
                string KEY = pubkeyJson.key;

                stopwatch.Stop();
                //3. Build post data
                string login_data = "staticpage=https%3A%2F%2Fwww.baidu.com%2Fcache%2Fuser%2Fhtml%2Fv3Jump.html&charset=UTF-8&token={0}&tpl=mn&subpro=&apiver=v3&tt={1}&codestring=&safeflg=0&u=https%3A%2F%2Fwww.baidu.com%2F&isPhone=&detect=1&gid={2}&quick_user=0&logintype=dialogLogin&logLoginType=pc_loginDialog&idc=&loginmerge=true&splogin=rate&username={3}&password={4}&verifycode=&mem_pass=on&rsakey={5}&crypttype=12&ppui_logintime={6}&countrycode=&callback=parent.bd__pcbs__{7}";

                login_data = string.Format(login_data, token, TimeUtility.ConvertDateTimeInt(DateTime.Now), Guid.NewGuid().ToString().ToUpper(), HttpUtility.UrlEncode(UserName), HttpUtility.UrlEncode(get_pwa_rsa(Password)), HttpUtility.UrlEncode(KEY), stopwatch.ElapsedMilliseconds, build_callback());

                //4. Post the login data
                string login_url = "https://passport.baidu.com/v2/api/?login";
                HttpHelper.GetHttpContent(login_url, login_data, cookies, referer: "https://www.baidu.com/", cookiesDomain: "passport.baidu.com");

                string home_url = "https://www.baidu.com";
                string result = HttpHelper.GetHttpContent(home_url, cookies: cookies, cookiesDomain: "passport.baidu.com");

                //5. Verifty the login result
                if (string.IsNullOrWhiteSpace(result) || result.Contains("账号存在异常") || !result.Contains("bds.comm.user=\""))
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

		private string get_pwa_rsa(string password)
        {
            RSAHelper rsaHelper = new RSAHelper();
            rsaHelper.SetPublic(rsa_pub_baidu.Substring(0, rsa_pub_baidu.LastIndexOf('-') + 1));

            return rsaHelper.Encrypt(password);
        }

        private string build_callback()
        {
            return getRandomStr(6);
        }

        private string getRandomStr(int count)
        {
            int number;
            string checkCode = String.Empty;

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                number = random.Next();
                number = number % 36;

                if (number < 10)
                    number += 48;
                else
                    number += 87;
                checkCode += ((char)number).ToString();
            }
            return checkCode;
        }
    }
}
