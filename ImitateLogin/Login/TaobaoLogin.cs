using System;
using System.ComponentModel.Composition;
using System.Net;

namespace ImitateLogin
{
    [Export(typeof(ILogin))]
    [ExportMetadata("loginSite", LoginSite.Taobao)]
    public class TaobaoLogin : ILogin
    {
        #region ILogin implementation

        public LoginResult DoLogin(string UserName, string Password)
        {
			//login url
			string loginUrl = "https://login.taobao.com/member/login.jhtml";

			//need read the ua.js
            throw new NotImplementedException();
        }

		public CookieContainer cookies { set; get;}

        #endregion
    }
}