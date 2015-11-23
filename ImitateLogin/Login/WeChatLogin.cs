using System;
using System.Net;
using System.Text.RegularExpressions;
using Thrinax.Helper;

namespace ImitateLogin.Login
{
    /// <summary>
    /// Wechat Login. Need heart beat sync.
    /// </summary>
    public class WeChatLogin : ILogin
    {
        private static string QRUrlFmt = "https://login.weixin.qq.com/qrcode/{0}";

        private static string SyncCheckUrl = "https://webpush.weixin.qq.com/cgi-bin/mmwebwx-bin/synccheck?r={0}&skey={1}&sid={2}&uin={3}&deviceid={4}&synckey={5}&_={6}";

        private static string WebWxSyncUrl = "https://wx.qq.com/cgi-bin/mmwebwx-bin/webwxsync?sid={0}&skey={1}&lang=zh_CN&pass_ticket={2}";

        private static Random random = new Random();

        public CookieContainer cookies { set; get; }

        /// <summary>
        /// Needn't provide username and password. You can put any string for those.
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public LoginResult DoLogin(string UserName, string Password)
        {
            string logincontent = Thrinax.Helper.HttpHelper.GetHttpContent("https://wx.qq.com/?&lang=zh_CN", cookies: cookies);

            //request the image, and send this image url to email.
            string QRLogin = Thrinax.Helper.HttpHelper.GetHttpContent("https://login.weixin.qq.com/jslogin?appid=wx782c26e4c19acffb&redirect_uri=https%3A%2F%2Fwx.qq.com%2Fcgi-bin%2Fmmwebwx-bin%2Fwebwxnewloginpage&fun=new&lang=zh_CN&_=" + TimeHelper.ConvertDateTimeInt(DateTime.Now), cookies: cookies);

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
                string result = Thrinax.Helper.HttpHelper.GetHttpContent(string.Format(strFmt, QRUid, "-595416181", TimeHelper.ConvertDateTimeInt(DateTime.Now)), cookies: cookies);

                i++;

                //If contains window.code=200, succ; If contains window.code=201, ready。
                if (result.Contains("window.code=200;"))
                {
                    Match m2 = Regex.Match(result, "redirect_uri=\"(?<redirect>.*?)\";");
                    if (m2.Success && m2.Groups["redirect"].Success)
                    {
                        string redirectContent = Thrinax.Helper.HttpHelper.GetHttpContent(m2.Groups["redirect"].Value, cookies: cookies);
                        break;
                    }
                }

            }
            while (i <= 20);

            return null;
        }

        private static string BuildDeviceId()
        {
            return "e" + random.Next(10000, 99999) + random.Next(10000, 99999) + random.Next(10000, 99999);
        }
    }
}
