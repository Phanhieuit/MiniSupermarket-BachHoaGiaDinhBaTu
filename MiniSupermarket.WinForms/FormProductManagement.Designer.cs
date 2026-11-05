using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    partial class FormProductManagement
    {
        private System.ComponentModel.IContainer components = null;

        private Panel panelTop;
        private Panel panelDetail;
        private DataGridView dgvProducts;

        private TextBox txtSearchBarcode;
        private ComboBox cboFilterCategory;
        private Button btnSearch;

        private Label lblId;
        private Label lblBarcode;
        private Label lblName;
        private Label lblPrice;
        private Label lblStock;
        private Label lblCategory;

        private TextBox txtId;
        private TextBox txtBarcode;
        private TextBox txtProductName;
        private NumericUpDown nudPrice;
        private NumericUpDown nudStock;
        private ComboBox cboCategory;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnLoad;

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
            panelTop = new Panel();
            txtSearchBarcode = new TextBox();
            cboFilterCategory = new ComboBox();
            btnSearch = new Button();
            panelDetail = new Panel();
            lblId = new Label();
            lblBarcode = new Label();
            lblName = new Label();
            lblPrice = new Label();
            lblStock = new Label();
            lblCategory = new Label();
            txtId = new TextBox();
            txtBarcode = new TextBox();
            txtProductName = new TextBox();
            nudPrice = new NumericUpDown();
            nudStock = new NumericUpDown();
            cboCategory = new ComboBox();
            btnAdd = new Button();
            btnUpdate = new Button();
            btnDelete = new Button();
            btnLoad = new Button();
            dgvProducts = new DataGridView();
            panelTop.SuspendLayout();
            panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudPrice).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudStock).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.BackColor = Color.White;
            panelTop.Controls.Add(txtSearchBarcode);
            panelTop.Controls.Add(cboFilterCategory);
            panelTop.Controls.Add(btnSearch);
            panelTop.Dock = DockStyle.Top;
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1050, 60);
            panelTop.TabIndex = 2;
            // 
            // txtSearchBarcode
            // 
            txtSearchBarcode.Location = new Point(15, 18);
            txtSearchBarcode.Name = "txtSearchBarcode";
            txtSearchBarcode.PlaceholderText = "Tìm theo mã vạch hoặc tên sản phẩm...";
            txtSearchBarcode.Size = new Size(300, 23);
            txtSearchBarcode.TabIndex = 0;
            txtSearchBarcode.KeyDown += txtSearchBarcode_KeyDown;
            // 
            // cboFilterCategory
            // 
            cboFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFilterCategory.Location = new Point(330, 18);
            cboFilterCategory.Name = "cboFilterCategory";
            cboFilterCategory.Size = new Size(230, 23);
            cboFilterCategory.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.BackColor = Color.FromArgb(41, 100, 180);
            btnSearch.Cursor = Cursors.Hand;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.ForeColor = Color.White;
            btnSearch.Location = new Point(575, 14);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(110, 32);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Tìm kiếm";
            btnSearch.UseVisualStyleBackColor = false;
            btnSearch.Click += btnSearch_Click;
            // 
            // panelDetail
            // 
            panelDetail.BackColor = Color.White;
            panelDetail.Controls.Add(lblId);
            panelDetail.Controls.Add(txtId);
            panelDetail.Controls.Add(lblBarcode);
            panelDetail.Controls.Add(txtBarcode);
            panelDetail.Controls.Add(lblName);
            panelDetail.Controls.Add(txtProductName);
            panelDetail.Controls.Add(lblPrice);
            panelDetail.Controls.Add(nudPrice);
            panelDetail.Controls.Add(lblStock);
            panelDetail.Controls.Add(nudStock);
            panelDetail.Controls.Add(lblCategory);
            panelDetail.Controls.Add(cboCategory);
            panelDetail.Controls.Add(btnAdd);
            panelDetail.Controls.Add(btnUpdate);
            panelDetail.Controls.Add(btnDelete);
            panelDetail.Controls.Add(btnLoad);
            panelDetail.Dock = DockStyle.Right;
            panelDetail.Name = "panelDetail";
            panelDetail.Size = new Size(330, 600);
            panelDetail.TabIndex = 1;
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(15, 15);
            lblId.Name = "lblId";
            lblId.Text = "Mã sản phẩm (tự sinh)";
            // 
            // txtId
            // 
            txtId.Location = new Point(15, 35);
            txtId.Name = "txtId";
            txtId.ReadOnly = true;
            txtId.Size = new Size(300, 23);
            txtId.TabIndex = 0;
            txtId.TabStop = false;
            // 
            // lblBarcode
            // 
            lblBarcode.AutoSize = true;
            lblBarcode.Location = new Point(15, 75);
            lblBarcode.Name = "lblBarcode";
            lblBarcode.Text = "Mã vạch (Barcode)";
            // 
            // txtBarcode
            // 
            txtBarcode.Location = new Point(15, 95);
            txtBarcode.Name = "txtBarcode";
            txtBarcode.Size = new Size(300, 23);
            txtBarcode.TabIndex = 1;
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(15, 135);
            lblName.Name = "lblName";
            lblName.Text = "Tên sản phẩm";
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(15, 155);
            txtProductName.Name = "txtProductName";
            txtProductName.Size = new Size(300, 23);
            txtProductName.TabIndex = 2;
            // 
            // lblPrice
            // 
            lblPrice.AutoSize = true;
            lblPrice.Location = new Point(15, 195);
            lblPrice.Name = "lblPrice";
            lblPrice.Text = "Đơn giá (đ)";
            // 
            // nudPrice
            // 
            nudPrice.Increment = 1000M;
            nudPrice.Location = new Point(15, 215);
            nudPrice.Maximum = 1000000000M;
            nudPrice.Name = "nudPrice";
            nudPrice.Size = new Size(300, 23);
            nudPrice.TabIndex = 3;
            nudPrice.TextAlign = HorizontalAlignment.Right;
            nudPrice.ThousandsSeparator = true;
            // 
            // lblStock
            // 
            lblStock.AutoSize = true;
            lblStock.Location = new Point(15, 255);
            lblStock.Name = "lblStock";
            lblStock.Text = "Số lượng tồn kho";
            // 
            // nudStock
            // 
            nudStock.Location = new Point(15, 275);
            nudStock.Maximum = 1000000M;
            nudStock.Name = "nudStock";
            nudStock.Size = new Size(300, 23);
            nudStock.TabIndex = 4;
            nudStock.TextAlign = HorizontalAlignment.Right;
            nudStock.ThousandsSeparator = true;
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Location = new Point(15, 315);
            lblCategory.Name = "lblCategory";
            lblCategory.Text = "Nhóm hàng";
            // 
            // cboCategory
            // 
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.Location = new Point(15, 335);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(300, 23);
            cboCategory.TabIndex = 5;
            // 
            // btnAdd
            // 
            btnAdd.BackColor = Color.FromArgb(46, 160, 67);
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(15, 385);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(145, 38);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "Thêm mới";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.BackColor = Color.FromArgb(41, 100, 180);
            btnUpdate.Cursor = Cursors.Hand;
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Location = new Point(170, 385);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(145, 38);
            btnUpdate.TabIndex = 7;
            btnUpdate.Text = "Cập nhật";
            btnUpdate.UseVisualStyleBackColor = false;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(200, 50, 60);
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(15, 433);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(145, 38);
            btnDelete.TabIndex = 8;
            btnDelete.Text = "Xóa";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnLoad
            // 
            btnLoad.BackColor = Color.FromArgb(108, 117, 125);
            btnLoad.Cursor = Cursors.Hand;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.ForeColor = Color.White;
            btnLoad.Location = new Point(170, 433);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(145, 38);
            btnLoad.TabIndex = 9;
            btnLoad.Text = "Làm mới";
            btnLoad.UseVisualStyleBackColor = false;
            btnLoad.Click += btnLoad_Click;
            // 
            // dgvProducts
            // 
            dgvProducts.AllowUserToAddRows = false;
            dgvProducts.AllowUserToDeleteRows = false;
            dgvProducts.AllowUserToResizeRows = false;
            dgvProducts.BackgroundColor = Color.FromArgb(244, 245, 247);
            dgvProducts.BorderStyle = BorderStyle.None;
            dgvProducts.ColumnHeadersHeight = 36;
            dgvProducts.Dock = DockStyle.Fill;
            dgvProducts.MultiSelect = false;
            dgvProducts.Name = "dgvProducts";
            dgvProducts.ReadOnly = true;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.TabIndex = 0;
            dgvProducts.CellClick += dgvProducts_CellClick;
            // 
            // FormProductManagement
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 245, 247);
            ClientSize = new Size(1050, 660);
            Controls.Add(dgvProducts);
            Controls.Add(panelDetail);
            Controls.Add(panelTop);
            Name = "FormProductManagement";
            Text = "Quản lý Sản phẩm";
            Load += FormProductManagement_Load;
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            panelDetail.ResumeLayout(false);
            panelDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudPrice).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudStock).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvProducts).EndInit();
            ResumeLayout(false);
        }
    }
}