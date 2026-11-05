namespace MiniSupermarket.WinForms
{
    partial class FormLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblTaiKhoan = new Label();
            lblMatKhau = new Label();
            label3 = new Label();
            txtUser = new TextBox();
            txtPass = new TextBox();
            btnLogin = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // lblTaiKhoan
            // 
            lblTaiKhoan.AutoSize = true;
            lblTaiKhoan.Location = new Point(210, 107);
            lblTaiKhoan.Name = "lblTaiKhoan";
            lblTaiKhoan.Size = new Size(57, 15);
            lblTaiKhoan.TabIndex = 0;
            lblTaiKhoan.Text = "Tài khoản";
            // 
            // lblMatKhau
            // 
            lblMatKhau.AutoSize = true;
            lblMatKhau.Location = new Point(210, 161);
            lblMatKhau.Name = "lblMatKhau";
            lblMatKhau.Size = new Size(57, 15);
            lblMatKhau.TabIndex = 1;
            lblMatKhau.Text = "Mật khẩu";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(290, 245);
            label3.Name = "label3";
            label3.Size = new Size(126, 15);
            label3.TabIndex = 2;
            label3.Text = "Kết nối: Chưa xác thực";
            // 
            // txtUser
            // 
            txtUser.Location = new Point(290, 107);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(150, 23);
            txtUser.TabIndex = 3;
            txtUser.TextChanged += txtUser_TextChanged;
            // 
            // txtPass
            // 
            txtPass.Location = new Point(290, 153);
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(150, 23);
            txtPass.TabIndex = 4;
            txtPass.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(290, 197);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(150, 30);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "Đăng nhập - Bách hóa Gia đình Bà Tư";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(273, 53);
            label1.Name = "label1";
            label1.Size = new Size(138, 15);
            label1.TabIndex = 6;
            label1.Text = "BÁCH HÓA GIA ĐÌNH BÀ TƯ";
            // 
            // FormLogin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label1);
            Controls.Add(btnLogin);
            Controls.Add(txtPass);
            Controls.Add(txtUser);
            Controls.Add(label3);
            Controls.Add(lblMatKhau);
            Controls.Add(lblTaiKhoan);
            Name = "FormLogin";
            Text = "Đăng nhập - Bách hóa Gia đình Bà Tư";
            Load += FormLogin_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTaiKhoan;
        private Label lblMatKhau;
        private Label label3;
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label label1;
    }
}