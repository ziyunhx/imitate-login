using System;

namespace ImitateLogin
{
    public class TaobaoLogin : ILogin
    {
        #region ILogin implementation

        public LoginResult DoLogin(string UserName, string Password, string UserAgent = "")
        {
            //login url
            string loginUrl = "https://login.taobao.com/member/login.jhtml";

            //need read the ua.js
            throw new NotImplementedException();
        }

        public System.Net.CookieContainer cookies
        {
            get;
            set;
        }

        #endregion
    }
}

