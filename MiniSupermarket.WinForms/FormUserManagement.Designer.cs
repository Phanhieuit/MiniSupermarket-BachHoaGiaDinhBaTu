using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    partial class FormUserManagement
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView dgvUsers;
        private Panel panelDetail;

        private Label lblUsername;
        private Label lblFullName;
        private Label lblPassword;
        private Label lblRole;
        private Label lblNote;

        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtPassword;
        private ComboBox cboRole;

        private Button btnAddUser;
        private Button btnResetPassword;
        private Button btnToggleLock;
        private Button btnClear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            dgvUsers = new DataGridView();
            panelDetail = new Panel();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblFullName = new Label();
            txtFullName = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblRole = new Label();
            cboRole = new ComboBox();
            btnAddUser = new Button();
            btnResetPassword = new Button();
            btnToggleLock = new Button();
            btnClear = new Button();
            lblNote = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            panelDetail.SuspendLayout();
            SuspendLayout();
            // 
            // dgvUsers
            // 
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.AllowUserToDeleteRows = false;
            dgvUsers.AllowUserToResizeRows = false;
            dgvUsers.BackgroundColor = Color.FromArgb(244, 245, 247);
            dgvUsers.BorderStyle = BorderStyle.None;
            dgvUsers.ColumnHeadersHeight = 36;
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.Location = new Point(0, 0);
            dgvUsers.Margin = new Padding(3, 4, 3, 4);
            dgvUsers.MultiSelect = false;
            dgvUsers.Name = "dgvUsers";
            dgvUsers.ReadOnly = true;
            dgvUsers.RowHeadersVisible = false;
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(823, 880);
            dgvUsers.TabIndex = 0;
            dgvUsers.CellClick += dgvUsers_CellClick;
            dgvUsers.CellContentClick += dgvUsers_CellContentClick;
            // 
            // panelDetail
            // 
            panelDetail.BackColor = Color.White;
            panelDetail.Controls.Add(lblUsername);
            panelDetail.Controls.Add(txtUsername);
            panelDetail.Controls.Add(lblFullName);
            panelDetail.Controls.Add(txtFullName);
            panelDetail.Controls.Add(lblPassword);
            panelDetail.Controls.Add(txtPassword);
            panelDetail.Controls.Add(lblRole);
            panelDetail.Controls.Add(cboRole);
            panelDetail.Controls.Add(btnAddUser);
            panelDetail.Controls.Add(btnResetPassword);
            panelDetail.Controls.Add(btnToggleLock);
            panelDetail.Controls.Add(btnClear);
            panelDetail.Controls.Add(lblNote);
            panelDetail.Dock = DockStyle.Right;
            panelDetail.Location = new Point(823, 0);
            panelDetail.Margin = new Padding(3, 4, 3, 4);
            panelDetail.Name = "panelDetail";
            panelDetail.Size = new Size(377, 880);
            panelDetail.TabIndex = 1;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(17, 20);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(107, 20);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Tên đăng nhập";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(17, 47);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.MaxLength = 50;
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(342, 27);
            txtUsername.TabIndex = 0;
            // 
            // lblFullName
            // 
            lblFullName.AutoSize = true;
            lblFullName.Location = new Point(17, 100);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(140, 20);
            lblFullName.TabIndex = 1;
            lblFullName.Text = "Họ và tên nhân viên";
            // 
            // txtFullName
            // 
            txtFullName.Location = new Point(17, 127);
            txtFullName.Margin = new Padding(3, 4, 3, 4);
            txtFullName.MaxLength = 100;
            txtFullName.Name = "txtFullName";
            txtFullName.Size = new Size(342, 27);
            txtFullName.TabIndex = 1;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(17, 180);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(192, 20);
            lblPassword.TabIndex = 2;
            lblPassword.Text = "Mật khẩu (tạo mới / đặt lại)";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(17, 207);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.MaxLength = 100;
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(342, 27);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblRole
            // 
            lblRole.AutoSize = true;
            lblRole.Location = new Point(17, 260);
            lblRole.Name = "lblRole";
            lblRole.Size = new Size(96, 20);
            lblRole.TabIndex = 3;
            lblRole.Text = "Vai trò (Role)";
            // 
            // cboRole
            // 
            cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cboRole.Location = new Point(17, 287);
            cboRole.Margin = new Padding(3, 4, 3, 4);
            cboRole.Name = "cboRole";
            cboRole.Size = new Size(342, 28);
            cboRole.TabIndex = 3;
            // 
            // btnAddUser
            // 
            btnAddUser.BackColor = Color.FromArgb(46, 160, 67);
            btnAddUser.Cursor = Cursors.Hand;
            btnAddUser.FlatAppearance.BorderSize = 0;
            btnAddUser.FlatStyle = FlatStyle.Flat;
            btnAddUser.ForeColor = Color.White;
            btnAddUser.Location = new Point(17, 353);
            btnAddUser.Margin = new Padding(3, 4, 3, 4);
            btnAddUser.Name = "btnAddUser";
            btnAddUser.Size = new Size(343, 51);
            btnAddUser.TabIndex = 4;
            btnAddUser.Text = "Tạo tài khoản mới";
            btnAddUser.UseVisualStyleBackColor = false;
            btnAddUser.Click += btnAddUser_Click;
            // 
            // btnResetPassword
            // 
            btnResetPassword.BackColor = Color.FromArgb(41, 100, 180);
            btnResetPassword.Cursor = Cursors.Hand;
            btnResetPassword.FlatAppearance.BorderSize = 0;
            btnResetPassword.FlatStyle = FlatStyle.Flat;
            btnResetPassword.ForeColor = Color.White;
            btnResetPassword.Location = new Point(17, 415);
            btnResetPassword.Margin = new Padding(3, 4, 3, 4);
            btnResetPassword.Name = "btnResetPassword";
            btnResetPassword.Size = new Size(343, 51);
            btnResetPassword.TabIndex = 5;
            btnResetPassword.Text = "Đặt lại mật khẩu";
            btnResetPassword.UseVisualStyleBackColor = false;
            btnResetPassword.Click += btnResetPassword_Click;
            // 
            // btnToggleLock
            // 
            btnToggleLock.BackColor = Color.FromArgb(230, 126, 34);
            btnToggleLock.Cursor = Cursors.Hand;
            btnToggleLock.FlatAppearance.BorderSize = 0;
            btnToggleLock.FlatStyle = FlatStyle.Flat;
            btnToggleLock.ForeColor = Color.White;
            btnToggleLock.Location = new Point(17, 476);
            btnToggleLock.Margin = new Padding(3, 4, 3, 4);
            btnToggleLock.Name = "btnToggleLock";
            btnToggleLock.Size = new Size(343, 51);
            btnToggleLock.TabIndex = 6;
            btnToggleLock.Text = "Khóa / Mở khóa tài khoản";
            btnToggleLock.UseVisualStyleBackColor = false;
            btnToggleLock.Click += btnToggleLock_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.FromArgb(108, 117, 125);
            btnClear.Cursor = Cursors.Hand;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(17, 537);
            btnClear.Margin = new Padding(3, 4, 3, 4);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(343, 51);
            btnClear.TabIndex = 7;
            btnClear.Text = "Nhập mới / Bỏ chọn";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // lblNote
            // 
            lblNote.ForeColor = Color.FromArgb(100, 105, 120);
            lblNote.Location = new Point(17, 609);
            lblNote.Name = "lblNote";
            lblNote.Size = new Size(343, 120);
            lblNote.TabIndex = 8;
            lblNote.Text = "Chọn một dòng trong danh sách để đặt lại mật khẩu hoặc khóa/mở khóa. Nhấn \"Nhập mới\" để tạo tài khoản khác. Tài khoản bị khóa hiển thị màu xám.";
            // 
            // FormUserManagement
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1200, 880);
            Controls.Add(dgvUsers);
            Controls.Add(panelDetail);
            Margin = new Padding(3, 4, 3, 4);
            Name = "FormUserManagement";
            Text = "Quản trị Tài khoản";
            Load += FormUserManagement_Load;
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            panelDetail.ResumeLayout(false);
            panelDetail.PerformLayout();
            ResumeLayout(false);
        }
    }
}