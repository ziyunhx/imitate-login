using System;
using System.Net;

namespace ImitateLogin
{
	public interface ILogin
	{
		CookieContainer cookies {set; get;}
		string DoLogin(string UserName, string Password);
	}
}

