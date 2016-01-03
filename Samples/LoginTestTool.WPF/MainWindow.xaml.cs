using ImitateLogin;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace LoginTestTool.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitCombox();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text) || string.IsNullOrEmpty(txtPassword.Password))
                return;

            if (cmbLoginSite.SelectedIndex < 0)
                return;

            LoginHelper loginHelper = new LoginHelper();
            LoginResult loginResult = loginHelper.Login(txtUserName.Text, txtPassword.Password, Enums.GetEnumName<LoginSite>(cmbLoginSite.SelectedValue.ToString()));

            txtStatus.Text = JsonConvert.SerializeObject(loginResult.Cookies);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtUserName.Text = "";
            txtPassword.Clear();
            txtStatus.Text = "";
        }

        private void InitCombox()
        {
            List<string> sites = Enums.GetDescriptions<LoginSite>();

            if (sites != null)
            {
                foreach (var site in sites)
                    cmbLoginSite.Items.Add(site);
                cmbLoginSite.SelectedIndex = 0;
            }
        }
    }
}
