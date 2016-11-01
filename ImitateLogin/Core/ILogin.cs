using System.Net;

namespace ImitateLogin
{
    public interface ILogin
	{
		CookieContainer cookies {set; get;}
		LoginResult DoLogin(string UserName, string Password, string UserAgent = "");
	}
}

