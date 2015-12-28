using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Thrinax.Helper;

namespace ImitateLogin
{
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
        public LoginResult DoLogin(string UserName, string Password)
        {
            HttpHelper.GetHttpContent("https://wx.qq.com/?&lang=zh_CN", cookies: cookies);

            //request the image, and send this image url to email.
            string QRLogin = HttpHelper.GetHttpContent("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + TimeHelper.ConvertDateTimeInt(DateTime.Now), cookies: cookies);

            string QRUid = "";
            string qrUrl = "";
            Match m = Regex.Match(QRLogin, "\"(?<QRUid>.*?)\";");
            if (m.Success && m.Groups["QRUid"].Success)
            {
                QRUid = m.Groups["QRUid"].Value;
                qrUrl = string.Format(QRUrlFmt, QRUid);
            }

            //send QR image to other process.

            string strFmt = "https://login.weixin.qq.com/cgi-bin/mmwebwx-bin/login?loginicon=true&uuid={0}&tip=0&r={1}8&_={2}";

            int i = 0;
            do
            {
                string result = HttpHelper.GetHttpContent(string.Format(strFmt, QRUid, "-595416181", TimeHelper.ConvertDateTimeInt(DateTime.Now)), cookies: cookies);

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
                            synckey = "1_632280340%7C2_632280341%7C3_632280343%7C1000_" + TimeHelper.ConvertDateTimeInt(DateTime.Now) / 1000;

                            tickTimer.Interval = 2000;
                            tickTimer.Elapsed += new System.Timers.ElapsedEventHandler(SyncCheck);
                            tickTimer.Start();

                            break;
                        }
                    }
                }
            }
            while (i <= 20);

            return null;
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
            long timeNow = TimeHelper.ConvertDateTimeInt(DateTime.Now);
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
}
