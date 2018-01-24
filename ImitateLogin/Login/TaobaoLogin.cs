using System;
using System.ComponentModel.Composition;
using System.Net;
using Thrinax.Http;

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
	[ExportMetadata("loginSite", "Taobao")]
    public class TaobaoLogin : ILogin
    {
        #region ILogin implementation

        public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
        {
			cookies = new CookieContainer ();
			//login url
			string loginUrl = "http://ui.ptlogin2.qq.com/cgi-bin/login?style=11&appid=716027613&target=self&s_url=http%3A//connect.qq.com/success.html";
			string loginContent = HttpHelper.GetHttpContent (loginUrl);

			//check the status
			string checkUrlFmt = "http://check.ptlogin2.qq.com/check?regmaster=&pt_tea=1&pt_vcode=1&uin={0}&appid=716027613&js_ver=10151&js_type=1&login_sig=XJbYoFZkShObtgmZl2UV5gWj31yYOHKVxQe2jvXNHDPa8g482pExdLczuXa-kVUM&u1=http%3A%2F%2Fconnect.qq.com%2Fsuccess.html&r=0.5678848952520639";

			string checkContent = HttpHelper.GetHttpContent (string.Format (checkUrlFmt, UserName));

			//get the request to login
			string doLoginUrl="http://ptlogin2.qq.com/login?u=46248069&verifycode=!EDL&pt_vcode_v1=0&pt_verifysession_v1=d587862962935afa82d5fe01bbd8ee5ec39315d21868ce69d5b33eb862ef0747ab1db4b43fcec93225bf24f15d0aac9ac9b5f89c83bad3f9&p=wsph-dFgYyRg-n3ubqvCh1aYLbL2mx50zOib7IXvyTd45FTlONTEaCTqxIKylvWAZgeA1bhd3jrHuXEFPQBbb*GYxMQCanqXu3jfxpv9gjeI*IYk3iwvSTrkdi9DHTo4QbmTFQLHvcSTdPgC9loqaJc8qt3nv6K*pQ8bVdyNDxancha4Ysmb6j4ddjPZ9SqWEdD9EUd94qaAgvnPOsuFEg__&pt_randsalt=0&ptredirect=0&u1=http%3A%2F%2Fconnect.qq.com%2Fsuccess.html&h=1&t=1&g=1&from_ui=1&ptlang=2052&action=3-20-1457145119893&js_ver=10151&js_type=1&login_sig=XJbYoFZkShObtgmZl2UV5gWj31yYOHKVxQe2jvXNHDPa8g482pExdLczuXa-kVUM&pt_uistyle=20&aid=716027613&";


			//refer to http://connect.qq.com to check the result.

			//need read the ua.js
            throw new NotImplementedException();
        }

		public CookieContainer cookies { set; get;}

        #endregion
    }
}