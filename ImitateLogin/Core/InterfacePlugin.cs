using System;
using System.Drawing;

namespace ImitateLogin
{
	#region For MEF
	public interface IMEFOperation
	{
		string Operate(string imageUrl = "", Image image = null, params string[] param);
	}

	public interface ILoginSiteData
	{
		string loginSite { get; }
	}
	#endregion
}

