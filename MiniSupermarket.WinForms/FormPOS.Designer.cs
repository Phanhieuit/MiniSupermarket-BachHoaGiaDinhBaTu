namespace MiniSupermarket.WinForms
{
    partial class FormPOS
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            groupBoxCart = new GroupBox();
            dgvCart = new DataGridView();

            colProductId = new DataGridViewTextBoxColumn();
            colProductName = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            colQuantity = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();

            groupBoxCustomer = new GroupBox();

            labelBarcode = new Label();
            txtBarcode = new TextBox();

            labelCustomerPhone = new Label();
            txtCustomerPhone = new TextBox();

            lblCustomerName = new Label();

            labelTotal = new Label();
            lblTotalAmount = new Label();

            labelCashReceived = new Label();
            txtCashReceived = new TextBox();

            labelChange = new Label();
            lblChange = new Label();

            btnCheckout = new Button();
            btnClearCart = new Button();

            groupBoxCart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCart).BeginInit();
            groupBoxCustomer.SuspendLayout();
            SuspendLayout();

            // 
            // groupBoxCart
            // 
            groupBoxCart.Controls.Add(dgvCart);
            groupBoxCart.Controls.Add(labelBarcode);
            groupBoxCart.Controls.Add(txtBarcode);
            groupBoxCart.Location = new Point(12, 12);
            groupBoxCart.Name = "groupBoxCart";
            groupBoxCart.Size = new Size(570, 470);
            groupBoxCart.TabIndex = 0;
            groupBoxCart.TabStop = false;
            groupBoxCart.Text = "GIỎ HÀNG THANH TOÁN";

            // 
            // dgvCart
            // 
            dgvCart.AllowUserToAddRows = false;
            dgvCart.AllowUserToDeleteRows = false;
            dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvCart.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.AutoSize;

            dgvCart.Columns.AddRange(new DataGridViewColumn[]
            {
                colProductId,
                colProductName,
                colPrice,
                colQuantity,
                colTotal
            });

            dgvCart.Location = new Point(10, 70);
            dgvCart.MultiSelect = false;
            dgvCart.Name = "dgvCart";
            dgvCart.ReadOnly = true;
            dgvCart.RowHeadersWidth = 35;
            dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCart.Size = new Size(550, 385);
            dgvCart.TabIndex = 1;

            // 
            // labelBarcode
            // 
            labelBarcode.AutoSize = true;
            labelBarcode.Location = new Point(10, 25);
            labelBarcode.Name = "labelBarcode";
            labelBarcode.Size = new Size(78, 15);
            labelBarcode.TabIndex = 0;
            labelBarcode.Text = "Mã vạch (F8):";

            // 
            // txtBarcode
            // 
            txtBarcode.Location = new Point(95, 22);
            txtBarcode.Name = "txtBarcode";
            txtBarcode.PlaceholderText = "Quét bằng máy quét USB hoặc nhập mã rồi nhấn Enter";
            txtBarcode.Size = new Size(440, 23);
            txtBarcode.TabIndex = 0;
            txtBarcode.KeyDown += txtBarcode_KeyDown;

            // 
            // groupBoxCustomer
            // 
            groupBoxCustomer.Controls.Add(labelCustomerPhone);
            groupBoxCustomer.Controls.Add(txtCustomerPhone);
            groupBoxCustomer.Controls.Add(lblCustomerName);
            groupBoxCustomer.Controls.Add(labelTotal);
            groupBoxCustomer.Controls.Add(lblTotalAmount);
            groupBoxCustomer.Controls.Add(labelCashReceived);
            groupBoxCustomer.Controls.Add(txtCashReceived);
            groupBoxCustomer.Controls.Add(labelChange);
            groupBoxCustomer.Controls.Add(lblChange);
            groupBoxCustomer.Controls.Add(btnCheckout);
            groupBoxCustomer.Controls.Add(btnClearCart);

            groupBoxCustomer.Location = new Point(600, 12);
            groupBoxCustomer.Name = "groupBoxCustomer";
            groupBoxCustomer.Size = new Size(340, 470);
            groupBoxCustomer.TabIndex = 1;
            groupBoxCustomer.TabStop = false;
            groupBoxCustomer.Text = "KHÁCH HÀNG & THANH TOÁN";

            // 
            // labelCustomerPhone
            // 
            labelCustomerPhone.AutoSize = true;
            labelCustomerPhone.Location = new Point(15, 35);
            labelCustomerPhone.Name = "labelCustomerPhone";
            labelCustomerPhone.Size = new Size(103, 15);
            labelCustomerPhone.TabIndex = 0;
            labelCustomerPhone.Text = "SĐT khách hàng:";

            // 
            // txtCustomerPhone
            // 
            txtCustomerPhone.Location = new Point(15, 55);
            txtCustomerPhone.Name = "txtCustomerPhone";
            txtCustomerPhone.PlaceholderText = "Nhập số điện thoại...";
            txtCustomerPhone.Size = new Size(305, 23);
            txtCustomerPhone.TabIndex = 1;
            txtCustomerPhone.KeyDown += txtCustomerPhone_KeyDown;

            // 
            // lblCustomerName
            // 
            lblCustomerName.BorderStyle = BorderStyle.FixedSingle;
            lblCustomerName.Location = new Point(15, 95);
            lblCustomerName.Name = "lblCustomerName";
            lblCustomerName.Size = new Size(305, 55);
            lblCustomerName.TabIndex = 2;
            lblCustomerName.Text = "Khách hàng: Khách lẻ\r\nHạng thẻ: --";
            lblCustomerName.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // labelTotal
            // 
            labelTotal.AutoSize = true;
            labelTotal.Location = new Point(15, 180);
            labelTotal.Name = "labelTotal";
            labelTotal.Size = new Size(92, 15);
            labelTotal.TabIndex = 3;
            labelTotal.Text = "TỔNG TIỀN:";

            // 
            // lblTotalAmount
            // 
            lblTotalAmount.BorderStyle = BorderStyle.FixedSingle;
            lblTotalAmount.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTotalAmount.Location = new Point(15, 205);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(305, 60);
            lblTotalAmount.TabIndex = 4;
            lblTotalAmount.Text = "0 ₫";
            lblTotalAmount.TextAlign = ContentAlignment.MiddleRight;

            // 
            // labelCashReceived
            // 
            labelCashReceived.AutoSize = true;
            labelCashReceived.Location = new Point(15, 290);
            labelCashReceived.Name = "labelCashReceived";
            labelCashReceived.Size = new Size(80, 15);
            labelCashReceived.TabIndex = 5;
            labelCashReceived.Text = "Tiền khách đưa:";

            // 
            // txtCashReceived
            // 
            txtCashReceived.Location = new Point(15, 310);
            txtCashReceived.Name = "txtCashReceived";
            txtCashReceived.PlaceholderText = "Nhập số tiền...";
            txtCashReceived.Size = new Size(305, 23);
            txtCashReceived.TabIndex = 6;
            txtCashReceived.TextChanged += txtCashReceived_TextChanged;

            // 
            // labelChange
            // 
            labelChange.AutoSize = true;
            labelChange.Location = new Point(15, 350);
            labelChange.Name = "labelChange";
            labelChange.Size = new Size(76, 15);
            labelChange.TabIndex = 7;
            labelChange.Text = "Tiền thừa:";

            // 
            // lblChange
            // 
            lblChange.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblChange.Location = new Point(100, 345);
            lblChange.Name = "lblChange";
            lblChange.Size = new Size(220, 25);
            lblChange.TabIndex = 8;
            lblChange.Text = "0 ₫";
            lblChange.TextAlign = ContentAlignment.MiddleRight;

            // 
            // btnCheckout
            // 
            btnCheckout.Location = new Point(15, 395);
            btnCheckout.Name = "btnCheckout";
            btnCheckout.Size = new Size(145, 45);
            btnCheckout.TabIndex = 9;
            btnCheckout.Text = "THANH TOÁN (F9)";
            btnCheckout.UseVisualStyleBackColor = true;
            btnCheckout.Click += btnCheckout_Click;

            // 
            // btnClearCart
            // 
            btnClearCart.Location = new Point(175, 395);
            btnClearCart.Name = "btnClearCart";
            btnClearCart.Size = new Size(145, 45);
            btnClearCart.TabIndex = 10;
            btnClearCart.Text = "HỦY GIỎ HÀNG";
            btnClearCart.UseVisualStyleBackColor = true;
            btnClearCart.Click += btnClearCart_Click;

            // 
            // colProductId
            // 
            colProductId.HeaderText = "Mã SP";
            colProductId.Name = "colProductId";
            colProductId.ReadOnly = true;

            // 
            // colProductName
            // 
            colProductName.HeaderText = "Tên SP";
            colProductName.Name = "colProductName";
            colProductName.ReadOnly = true;

            // 
            // colPrice
            // 
            colPrice.HeaderText = "Đơn giá";
            colPrice.Name = "colPrice";
            colPrice.ReadOnly = true;

            // 
            // colQuantity
            // 
            colQuantity.HeaderText = "Số lượng";
            colQuantity.Name = "colQuantity";
            colQuantity.ReadOnly = true;

            // 
            // colTotal
            // 
            colTotal.HeaderText = "Thành tiền";
            colTotal.Name = "colTotal";
            colTotal.ReadOnly = true;

            // 
            // FormPOS
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(960, 500);

            Controls.Add(groupBoxCustomer);
            Controls.Add(groupBoxCart);

            Name = "FormPOS";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Bán hàng - POS";

            Load += FormPOS_Load;

            groupBoxCart.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCart).EndInit();
            groupBoxCustomer.ResumeLayout(false);
            groupBoxCustomer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxCart;
        private GroupBox groupBoxCustomer;

        private DataGridView dgvCart;

        private DataGridViewTextBoxColumn colProductId;
        private DataGridViewTextBoxColumn colProductName;
        private DataGridViewTextBoxColumn colPrice;
        private DataGridViewTextBoxColumn colQuantity;
        private DataGridViewTextBoxColumn colTotal;

        private Label labelBarcode;
        private TextBox txtBarcode;

        private Label labelCustomerPhone;
        private TextBox txtCustomerPhone;

        private Label lblCustomerName;

        private Label labelTotal;
        private Label lblTotalAmount;

        private Label labelCashReceived;
        private TextBox txtCashReceived;

        private Label labelChange;
        private Label lblChange;

        private Button btnCheckout;
        private Button btnClearCart;
    }
}
