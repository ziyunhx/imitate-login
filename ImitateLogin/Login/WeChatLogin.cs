using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Thrinax.Http;
using Thrinax.Utility;

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
    [ExportMetadata("loginSite", "WeChat")]
    /// <summary>
    /// Wechat Login. Need heart beat sync.
    /// </summary>
    public class WeChatLogin : ILogin
    {
        private string WxUrl = "https://wx.qq.com/?&lang=zh_CN";

        private string QRUrlFmt = "https://login.weixin.qq.com/qrcode/{0}";

        private string SyncCheckUrl = "https://webpush.weixin.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}&_={6}";

        private string WebWxSyncUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={0}&skey={1}&lang=zh_CN&pass_ticket={2}";

        private Random random = new Random();

        public CookieContainer cookies { set; get; }

        private string skey = "", wxsid = "", wxuin = "", pass_ticket = "", synckey = "";

        private System.Timers.Timer tickTimer = new System.Timers.Timer();

        /// <summary>
        /// Needn't provide username and password. You can put any string for those.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
        {
            LoginResult loginResult = new LoginResult();

            HttpHelper.GetHttpContent("https://wx.qq.com/?&lang=zh_CN", cookies: cookies);

            //request the image, and send this image url to email.
            string QRLogin = HttpHelper.GetHttpContent("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + TimeUtility.ConvertDateTimeInt(DateTime.Now), cookies: cookies);

            string QRUid = "";
            string qrUrl = "";
            Match m = Regex.Match(QRLogin, "\"(?<QRUid>.*?)\";");
            if (m.Success && m.Groups["QRUid"].Success)
            {
                QRUid = m.Groups["QRUid"].Value;
                qrUrl = string.Format(QRUrlFmt, QRUid);
            }

            //send QR image to other process.
			string qrResult = PluginHelper.Operation(LoginSite.WeChat.ToString(), qrUrl, null);

            if (!string.IsNullOrEmpty(qrResult))
            {
                string strFmt = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}8&_={2}";

                int i = 0;
                do
                {
                    string result = HttpHelper.GetHttpContent(string.Format(strFmt, QRUid, "-595416181", TimeUtility.ConvertDateTimeInt(DateTime.Now)), cookies: cookies);

                    i++;

                    //If contains window.code=200, succ; If contains window.code=201, ready。
                    if (result.Contains("window.code=200;"))
                    {
                        Match m2 = Regex.Match(result, "redirect_uri=\"(?<redirect>.*?)\";");
                        if (m2.Success && m2.Groups["redirect"].Success)
                        {
                            string redirectContent = HttpHelper.GetHttpContent(m2.Groups["redirect"].Value + "&fun=new&version=v2&lang=zh_CN", cookies: cookies, referer: WxUrl);
                            Match m3 = Regex.Match(redirectContent, "<skey>(?<skey>.*?)</skey><wxsid>(?<wxsid>.*?)</wxsid><wxuin>(?<wxuin>.*?)</wxuin><pass_ticket>(?<pass_ticket>.*?)</pass_ticket>");
                            if (m3.Success && m3.Groups["skey"].Success && m3.Groups["wxsid"].Success && m3.Groups["wxuin"].Success && m3.Groups["pass_ticket"].Success)
                            {
                                skey = m3.Groups["skey"].Value;
                                wxsid = m3.Groups["wxsid"].Value;
                                wxuin = m3.Groups["wxuin"].Value;
                                pass_ticket = m3.Groups["pass_ticket"].Value;

                                //wxInit, Get the synckey.
                                string wxInitUrl = string.Format("https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxinit?r={0}&lang=zh_CN&pass_ticket={1}", TimeUtility.ConvertDateTimeInt(DateTime.Now) / 1000, pass_ticket);
                                string wxInitPost = "{\"BaseRequest\":{\"Uin\":\"" + wxuin + "\",\"Sid\":\"" + wxsid + "\",\"Skey\":\"" + skey + "\",\"DeviceID\":\"" + BuildDeviceId() + "\"}}";
                                do
                                {
                                    string wxInitContent = HttpHelper.GetHttpContent(wxInitUrl, wxInitPost,cookies, referer: "https://wx.qq.com/?&lang=zh_CN");
                                    if (!string.IsNullOrEmpty(wxInitContent))
                                    {
                                        synckey = GetSyncKeyStr(wxInitContent);
                                        if (!string.IsNullOrEmpty(synckey))
                                            break;
                                    }
                                }
                                while (true);


                                tickTimer.Interval = 2000;
                                tickTimer.Elapsed += new System.Timers.ElapsedEventHandler(SyncCheck);
                                tickTimer.Start();

                                loginResult.Result = ResultType.Success;
                                loginResult.Msg = "Success";
                                loginResult.Cookies = HttpHelper.GetAllCookies(cookies);
                                return loginResult;
                            }
                        }
                    }
                }
                while (i <= 20);
            }

            loginResult.Result = ResultType.Timeout;
            loginResult.Msg = "error, no body scan the QR code in limited time.";

            return loginResult;
        }

        /// <summary>
        /// Get sync key string from http content.
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private string GetSyncKeyStr(string httpContent)
        {
            MatchCollection matchs = Regex.Matches(httpContent, @"{\s""Key"": (?<key>\d*),\s""Val"": (?<val>\d*)\s}");
            if (matchs != null && matchs.Count > 0)
            {
                List<string> syncKeys = new List<string>();
                foreach (Match match in matchs)
                {
                    if (match.Success && match.Groups["key"].Success && match.Groups["val"].Success)
                    {
                        syncKeys.Add(match.Groups["key"].Value + "_" + match.Groups["val"].Value);
                    }
                }
                return HttpUtility.UrlEncode(string.Join("|", syncKeys), Encoding.UTF8);
            }
            return "";
        }

        private string BuildSyncKey()
        {
            if (string.IsNullOrEmpty(synckey))
                return "";

            List<SyncKey> _syncKey = new List<SyncKey>();
            string temp = HttpUtility.UrlDecode(synckey);
            foreach (var str in temp.Split('|'))
            {
                SyncKey _key = new SyncKey();
                _key.Key = str.Split('_')[0];
                _key.val = str.Split('_')[1];
                _syncKey.Add(_key);
            }
            return "{\"Count\":" + _syncKey.Count + ",\"List\":" + JsonConvert.SerializeObject(_syncKey) + "}";
        }


        /// <summary>
        /// build a random device id.
        /// </summary>
        /// <returns></returns>
        private string BuildDeviceId()
        {
            return "e" + random.Next(10000, 99999) + random.Next(10000, 99999) + random.Next(10000, 99999);
        }
        
        /// <summary>
        /// do sync check to keep alive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncCheck(object sender, System.Timers.ElapsedEventArgs e)
        {
            long timeNow = TimeUtility.ConvertDateTimeInt(DateTime.Now);
            string result = HttpHelper.GetHttpContent(string.Format(SyncCheckUrl, timeNow + 89075, skey, wxsid, wxuin, BuildDeviceId(), synckey, timeNow), cookies:cookies, referer: WxUrl);

            //needn't do it!
            if (!result.Contains("selector:\"0\""))
            {
                string temp = HttpHelper.GetHttpContent(string.Format(WebWxSyncUrl, wxsid, skey, pass_ticket), cookies: cookies, referer: WxUrl);
                MatchCollection matchs = Regex.Matches(temp, @"{\s""Key"": (?<key>\d*),\s""Val"": (?<val>\d*)\s}");
                if (matchs != null && matchs.Count > 0)
                {
                    List<string> syncKeys = new List<string>();
                    foreach (Match match in matchs)
                    {
                        if (match.Success && match.Groups["Key"].Success && match.Groups["val"].Success)
                        {
                            syncKeys.Add(match.Groups["Key"].Value + "_" + match.Groups["val"].Value);
                        }
                    }
                    synckey = HttpUtility.UrlEncode(string.Join("|", syncKeys), Encoding.UTF8);
                }
            }
        }
    }

    public class SyncKey
    {
        public string Key;
        public string val;
    }
}
