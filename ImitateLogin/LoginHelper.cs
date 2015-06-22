using System;

namespace ImitateLogin
{
	public class LoginHelper
	{
		/// <summary>
		/// Login the specified userName, password and loginSite.
		/// </summary>
		/// <param name="userName">User name.</param>
		/// <param name="password">Password.</param>
		/// <param name="loginSite">Login site.</param>
		/// <returns>cookies string</returns>
		public static string Login(string userName, string password, LoginSite loginSite)
		{
			if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
				return "error, username or password can't be null.";

			ILogin LoginClass = null;

			switch (loginSite) {
			case LoginSite.Weibo:
				LoginClass = new WeiboLogin ();
				break;
			case LoginSite.WeiboWap:
				LoginClass = new WeiboWapLogin ();
				break;
			case LoginSite.SinaWap:
				LoginClass = new SinaWapLogin ();
				break;
			}

			if(LoginClass == null)
				return "error, can't find the login class.";

			return LoginClass.DoLogin (userName, password);
		}
	}
}

