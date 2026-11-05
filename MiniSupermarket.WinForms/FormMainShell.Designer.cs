using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    partial class FormMainShell
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelSidebar;
        private Panel panelTopHeader;
        private Panel panelMainContent;
        private Panel panelLogout;

        private Label lblLogo;
        private Label lblTitle;
        private Label lblUserInfo;

        private Button btnPOS;
        private Button btnCategory;
        private Button btnProduct;
        private Button btnCustomer;
        private Button btnReports;
        private Button btnUserManage;
        private Button btnLogout;

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
            panelSidebar = new Panel();
            panelLogout = new Panel();
            btnLogout = new Button();
            btnUserManage = new Button();
            btnReports = new Button();
            btnCustomer = new Button();
            btnProduct = new Button();
            btnCategory = new Button();
            btnPOS = new Button();
            lblLogo = new Label();
            panelTopHeader = new Panel();
            lblUserInfo = new Label();
            lblTitle = new Label();
            panelMainContent = new BufferedPanel();
            panelSidebar.SuspendLayout();
            panelLogout.SuspendLayout();
            panelTopHeader.SuspendLayout();
            SuspendLayout();
            // 
            // panelSidebar
            // (thứ tự Controls.Add quan trọng với Dock = Top:
            //  control add SAU CÙNG (lblLogo) sẽ nằm trên cùng)
            // 
            panelSidebar.BackColor = Color.FromArgb(24, 30, 48);
            panelSidebar.Controls.Add(panelLogout);
            panelSidebar.Controls.Add(btnUserManage);
            panelSidebar.Controls.Add(btnReports);
            panelSidebar.Controls.Add(btnCustomer);
            panelSidebar.Controls.Add(btnProduct);
            panelSidebar.Controls.Add(btnCategory);
            panelSidebar.Controls.Add(btnPOS);
            panelSidebar.Controls.Add(lblLogo);
            panelSidebar.Dock = DockStyle.Left;
            panelSidebar.Location = new Point(0, 0);
            panelSidebar.Name = "panelSidebar";
            panelSidebar.Size = new Size(230, 720);
            panelSidebar.TabIndex = 2;
            // 
            // panelLogout
            // 
            panelLogout.BackColor = Color.FromArgb(18, 23, 38);
            panelLogout.Controls.Add(btnLogout);
            panelLogout.Dock = DockStyle.Bottom;
            panelLogout.Location = new Point(0, 660);
            panelLogout.Name = "panelLogout";
            panelLogout.Size = new Size(230, 60);
            panelLogout.TabIndex = 0;
            // 
            // btnLogout
            // 
            btnLogout.BackColor = Color.FromArgb(18, 23, 38);
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(160, 40, 50);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI Emoji", 10F);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(0, 0);
            btnLogout.Name = "btnLogout";
            btnLogout.Padding = new Padding(15, 0, 0, 0);
            btnLogout.Size = new Size(230, 60);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "🚪  Đăng xuất";
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnUserManage
            // 
            btnUserManage.BackColor = Color.FromArgb(24, 30, 48);
            btnUserManage.Cursor = Cursors.Hand;
            btnUserManage.Dock = DockStyle.Top;
            btnUserManage.FlatAppearance.BorderSize = 0;
            btnUserManage.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnUserManage.FlatStyle = FlatStyle.Flat;
            btnUserManage.Font = new Font("Segoe UI Emoji", 10F);
            btnUserManage.ForeColor = Color.White;
            btnUserManage.Location = new Point(0, 295);
            btnUserManage.Name = "btnUserManage";
            btnUserManage.Padding = new Padding(15, 0, 0, 0);
            btnUserManage.Size = new Size(230, 45);
            btnUserManage.TabIndex = 1;
            btnUserManage.Text = "🛡  Quản trị Tài khoản";
            btnUserManage.TextAlign = ContentAlignment.MiddleLeft;
            btnUserManage.UseVisualStyleBackColor = false;
            btnUserManage.Click += btnUserManage_Click;
            // 
            // btnReports
            // 
            btnReports.BackColor = Color.FromArgb(24, 30, 48);
            btnReports.Cursor = Cursors.Hand;
            btnReports.Dock = DockStyle.Top;
            btnReports.FlatAppearance.BorderSize = 0;
            btnReports.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnReports.FlatStyle = FlatStyle.Flat;
            btnReports.Font = new Font("Segoe UI Emoji", 10F);
            btnReports.ForeColor = Color.White;
            btnReports.Location = new Point(0, 250);
            btnReports.Name = "btnReports";
            btnReports.Padding = new Padding(15, 0, 0, 0);
            btnReports.Size = new Size(230, 45);
            btnReports.TabIndex = 2;
            btnReports.Text = "📊  Báo cáo Doanh thu";
            btnReports.TextAlign = ContentAlignment.MiddleLeft;
            btnReports.UseVisualStyleBackColor = false;
            btnReports.Click += btnReports_Click;
            // 
            // btnCustomer
            // 
            btnCustomer.BackColor = Color.FromArgb(24, 30, 48);
            btnCustomer.Cursor = Cursors.Hand;
            btnCustomer.Dock = DockStyle.Top;
            btnCustomer.FlatAppearance.BorderSize = 0;
            btnCustomer.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnCustomer.FlatStyle = FlatStyle.Flat;
            btnCustomer.Font = new Font("Segoe UI Emoji", 10F);
            btnCustomer.ForeColor = Color.White;
            btnCustomer.Location = new Point(0, 205);
            btnCustomer.Name = "btnCustomer";
            btnCustomer.Padding = new Padding(15, 0, 0, 0);
            btnCustomer.Size = new Size(230, 45);
            btnCustomer.TabIndex = 3;
            btnCustomer.Text = "👥  Quản lý Khách hàng";
            btnCustomer.TextAlign = ContentAlignment.MiddleLeft;
            btnCustomer.UseVisualStyleBackColor = false;
            btnCustomer.Click += btnCustomer_Click;
            // 
            // btnProduct
            // 
            btnProduct.BackColor = Color.FromArgb(24, 30, 48);
            btnProduct.Cursor = Cursors.Hand;
            btnProduct.Dock = DockStyle.Top;
            btnProduct.FlatAppearance.BorderSize = 0;
            btnProduct.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnProduct.FlatStyle = FlatStyle.Flat;
            btnProduct.Font = new Font("Segoe UI Emoji", 10F);
            btnProduct.ForeColor = Color.White;
            btnProduct.Location = new Point(0, 160);
            btnProduct.Name = "btnProduct";
            btnProduct.Padding = new Padding(15, 0, 0, 0);
            btnProduct.Size = new Size(230, 45);
            btnProduct.TabIndex = 4;
            btnProduct.Text = "📦  Quản lý Sản phẩm";
            btnProduct.TextAlign = ContentAlignment.MiddleLeft;
            btnProduct.UseVisualStyleBackColor = false;
            btnProduct.Click += btnProduct_Click;
            // 
            // btnCategory
            // 
            btnCategory.BackColor = Color.FromArgb(24, 30, 48);
            btnCategory.Cursor = Cursors.Hand;
            btnCategory.Dock = DockStyle.Top;
            btnCategory.FlatAppearance.BorderSize = 0;
            btnCategory.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnCategory.FlatStyle = FlatStyle.Flat;
            btnCategory.Font = new Font("Segoe UI Emoji", 10F);
            btnCategory.ForeColor = Color.White;
            btnCategory.Location = new Point(0, 115);
            btnCategory.Name = "btnCategory";
            btnCategory.Padding = new Padding(15, 0, 0, 0);
            btnCategory.Size = new Size(230, 45);
            btnCategory.TabIndex = 5;
            btnCategory.Text = "📁  Quản lý Danh mục";
            btnCategory.TextAlign = ContentAlignment.MiddleLeft;
            btnCategory.UseVisualStyleBackColor = false;
            btnCategory.Click += btnCategory_Click;
            // 
            // btnPOS
            // 
            btnPOS.BackColor = Color.FromArgb(24, 30, 48);
            btnPOS.Cursor = Cursors.Hand;
            btnPOS.Dock = DockStyle.Top;
            btnPOS.FlatAppearance.BorderSize = 0;
            btnPOS.FlatAppearance.MouseOverBackColor = Color.FromArgb(41, 100, 180);
            btnPOS.FlatStyle = FlatStyle.Flat;
            btnPOS.Font = new Font("Segoe UI Emoji", 10F);
            btnPOS.ForeColor = Color.White;
            btnPOS.Location = new Point(0, 70);
            btnPOS.Name = "btnPOS";
            btnPOS.Padding = new Padding(15, 0, 0, 0);
            btnPOS.Size = new Size(230, 45);
            btnPOS.TabIndex = 6;
            btnPOS.Text = "🛒  Bán hàng (POS)";
            btnPOS.TextAlign = ContentAlignment.MiddleLeft;
            btnPOS.UseVisualStyleBackColor = false;
            btnPOS.Click += btnPOS_Click;
            // 
            // lblLogo
            // 
            lblLogo.Dock = DockStyle.Top;
            lblLogo.Font = new Font("Segoe UI Emoji", 11F, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(0, 0);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(230, 70);
            lblLogo.TabIndex = 7;
            lblLogo.Text = "🛒 Bách hóa\nGia đình Bà Tư";
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panelTopHeader
            // 
            panelTopHeader.BackColor = Color.White;
            panelTopHeader.Controls.Add(lblUserInfo);
            panelTopHeader.Controls.Add(lblTitle);
            panelTopHeader.Dock = DockStyle.Top;
            panelTopHeader.Location = new Point(230, 0);
            panelTopHeader.Name = "panelTopHeader";
            panelTopHeader.Size = new Size(1050, 60);
            panelTopHeader.TabIndex = 1;
            // 
            // lblUserInfo
            // 
            lblUserInfo.AutoSize = false;
            lblUserInfo.Dock = DockStyle.Right;
            lblUserInfo.Font = new Font("Segoe UI", 10F);
            lblUserInfo.ForeColor = Color.FromArgb(80, 85, 100);
            lblUserInfo.Location = new Point(600, 0);
            lblUserInfo.Name = "lblUserInfo";
            lblUserInfo.Padding = new Padding(0, 0, 20, 0);
            lblUserInfo.Size = new Size(450, 60);
            lblUserInfo.TabIndex = 0;
            lblUserInfo.Text = "Xin chào: ...";
            lblUserInfo.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(35, 40, 55);
            lblTitle.Location = new Point(25, 16);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(248, 25);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "BÁCH HÓA GIA ĐÌNH BÀ TƯ";
            // 
            // panelMainContent
            // 
            panelMainContent.BackColor = Color.FromArgb(244, 245, 247);
            panelMainContent.Dock = DockStyle.Fill;
            panelMainContent.Location = new Point(230, 60);
            panelMainContent.Name = "panelMainContent";
            panelMainContent.Size = new Size(1050, 660);
            panelMainContent.TabIndex = 0;
            // 
            // FormMainShell
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1280, 720);
            Controls.Add(panelMainContent);
            Controls.Add(panelTopHeader);
            Controls.Add(panelSidebar);
            MinimumSize = new Size(1000, 600);
            Name = "FormMainShell";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Quản lý Bán hàng và Tồn kho - Bách hóa Gia đình Bà Tư";
            Load += FormMainShell_Load;
            panelSidebar.ResumeLayout(false);
            panelLogout.ResumeLayout(false);
            panelTopHeader.ResumeLayout(false);
            panelTopHeader.PerformLayout();
            ResumeLayout(false);
        }
    }
}