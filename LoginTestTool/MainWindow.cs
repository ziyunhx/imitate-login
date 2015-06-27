using System;
using ImitateLogin;
using Gtk;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class MainWindow: Gtk.Window
{
	public MainWindow () : base (Gtk.WindowType.Toplevel)
	{
		Build ();
		List<string> sites = Enums.GetDescriptions<LoginSite> ();

		if (sites != null) {
			foreach (var site in sites)
				combSites.AppendText (site);
			combSites.Active = 0;
		}
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnBtnLoginClicked (object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty (txtUserName.Text) || string.IsNullOrEmpty (txtPassword.Text))
			return;

		if (string.IsNullOrEmpty (combSites.ActiveText))
			return;

		LoginHelper loginHelper = new LoginHelper ();
		txtResult.Buffer.Text = JsonConvert.SerializeObject (loginHelper.Login (txtUserName.Text, txtPassword.Text, Enums.GetEnumName<LoginSite> (combSites.ActiveText)).Cookies);
	}

	protected void OnBtnResetClicked (object sender, EventArgs e)
	{
		txtUserName.Text = "";
		txtPassword.Text = "";
		txtResult.Buffer.Clear ();
	}
}
