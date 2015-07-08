using System;

namespace ImitateLogin
{
	public class TaobaoLogin : ILogin
	{
		#region ILogin implementation

		public LoginResult DoLogin (string UserName, string Password)
		{
			throw new NotImplementedException ();
		}

		public System.Net.CookieContainer cookies {
			get ;
			set ;
		}

		#endregion
	}
}

