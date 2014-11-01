using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImitateLogin
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.cmbWebsite.SelectedIndex = 0;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string cookies = "";
            switch (cmbWebsite.SelectedIndex)
            {
                case 0:
                    cookies = WeiboLogin.DoLogin(txtUserName.Text, txtPassWord.Text);
                    break;
                case 1:
                    cookies = SinaWapLogin.DoLogin(txtUserName.Text, txtPassWord.Text);
                    break;
                case 2:
                    cookies = WeiboWapLogin.DoLogin(txtUserName.Text, txtPassWord.Text);
                    break;
            }
            txtCookies.Text = cookies;
        }
    }
}
