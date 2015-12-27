using System;
using System.Web.UI.WebControls;

namespace ImitateLogin
{
	#region For MEF
	public interface IMEFOperation
	{
		string Operate(string imageUrl = "", Image image = null, params string[] param);
	}

	public interface IMEFOperationData
	{
		LoginSite loginSite { get; }
	}
	#endregion
}

