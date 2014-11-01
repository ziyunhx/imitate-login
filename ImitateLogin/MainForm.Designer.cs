namespace ImitateLogin
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.labUsername = new System.Windows.Forms.Label();
            this.labPwd = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassWord = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.txtCookies = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWebsite = new System.Windows.Forms.ComboBox();
            this.labWebsite = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labUsername
            // 
            this.labUsername.AutoSize = true;
            this.labUsername.Location = new System.Drawing.Point(13, 24);
            this.labUsername.Name = "labUsername";
            this.labUsername.Size = new System.Drawing.Size(57, 13);
            this.labUsername.TabIndex = 0;
            this.labUsername.Text = "UserName";
            // 
            // labPwd
            // 
            this.labPwd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labPwd.AutoSize = true;
            this.labPwd.Location = new System.Drawing.Point(235, 24);
            this.labPwd.Name = "labPwd";
            this.labPwd.Size = new System.Drawing.Size(53, 13);
            this.labPwd.TabIndex = 1;
            this.labPwd.Text = "Password";
            // 
            // txtUserName
            // 
            this.txtUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserName.Location = new System.Drawing.Point(78, 21);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(127, 20);
            this.txtUserName.TabIndex = 2;
            // 
            // txtPassWord
            // 
            this.txtPassWord.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassWord.Location = new System.Drawing.Point(300, 21);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(126, 20);
            this.txtPassWord.TabIndex = 3;
            // 
            // btnLogin
            // 
            this.btnLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogin.Location = new System.Drawing.Point(300, 53);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(126, 25);
            this.btnLogin.TabIndex = 4;
            this.btnLogin.Text = "Login In";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // txtCookies
            // 
            this.txtCookies.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCookies.Location = new System.Drawing.Point(14, 112);
            this.txtCookies.Name = "txtCookies";
            this.txtCookies.Size = new System.Drawing.Size(412, 269);
            this.txtCookies.TabIndex = 5;
            this.txtCookies.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Cookies";
            // 
            // cmbWebsite
            // 
            this.cmbWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbWebsite.FormattingEnabled = true;
            this.cmbWebsite.Items.AddRange(new object[] {
            "Weibo Web",
            "Sina Wap",
            "Weibo Wap"});
            this.cmbWebsite.Location = new System.Drawing.Point(78, 56);
            this.cmbWebsite.Name = "cmbWebsite";
            this.cmbWebsite.Size = new System.Drawing.Size(127, 21);
            this.cmbWebsite.TabIndex = 3;
            // 
            // labWebsite
            // 
            this.labWebsite.AutoSize = true;
            this.labWebsite.Location = new System.Drawing.Point(13, 59);
            this.labWebsite.Name = "labWebsite";
            this.labWebsite.Size = new System.Drawing.Size(46, 13);
            this.labWebsite.TabIndex = 8;
            this.labWebsite.Text = "Website";
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnLogin;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(438, 394);
            this.Controls.Add(this.labWebsite);
            this.Controls.Add(this.cmbWebsite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCookies);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.txtPassWord);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.labPwd);
            this.Controls.Add(this.labUsername);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Imitate Login Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labUsername;
        private System.Windows.Forms.Label labPwd;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassWord;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.RichTextBox txtCookies;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWebsite;
        private System.Windows.Forms.Label labWebsite;
    }
}

