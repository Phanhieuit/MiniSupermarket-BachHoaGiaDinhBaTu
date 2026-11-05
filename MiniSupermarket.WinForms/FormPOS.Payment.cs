using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MiniSupermarket.WinForms;

public partial class FormPOS
{
    private CustomerRow? _selectedCustomer;
    private readonly Button _findCustomer = new() { Text = "Tìm khách", AutoSize = true };
    private readonly Button _guestCustomer = new() { Text = "Khách vãng lai", AutoSize = true };
    private readonly CheckBox _useRewardPoints = new()
    {
        AutoSize = true, Text = "Dùng 50 điểm giảm 50.000đ (đơn > 50.000đ)",
        Enabled = false, ForeColor = Color.FromArgb(26, 92, 148)
    };
    private readonly Label _paymentStatus = new() { AutoSize = true, ForeColor = Color.DimGray, MaximumSize = new Size(340, 0) };
    private readonly List<Button> _cashButtons = new();
    private bool _findingCustomer;

    private void SetupPaymentArea()
    {
        SuspendLayout();
        Controls.Clear();
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(12), BackColor = Color.FromArgb(244, 245, 247) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        groupBoxCart.Dock = DockStyle.Fill;
        groupBoxCart.Margin = new Padding(0, 0, 12, 0);
        groupBoxCart.Controls.Clear();
        var scan = new TableLayoutPanel { Dock = DockStyle.Top, Height = 64, ColumnCount = 2, Padding = new Padding(8) };
        scan.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        scan.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        labelBarcode.Anchor = AnchorStyles.Left;
        txtBarcode.Dock = DockStyle.Fill;
        txtBarcode.Margin = new Padding(8, 4, 0, 0);
        scan.Controls.Add(labelBarcode, 0, 0); scan.Controls.Add(txtBarcode, 1, 0);
        dgvCart.Dock = DockStyle.Fill;
        groupBoxCart.Controls.Add(dgvCart); groupBoxCart.Controls.Add(scan);
        colPrice.DefaultCellStyle.Format = colTotal.DefaultCellStyle.Format = "N0";

        groupBoxCustomer.Dock = DockStyle.Fill;
        groupBoxCustomer.Margin = Padding.Empty;
        groupBoxCustomer.Text = "KHÁCH HÀNG & THANH TOÁN";
        groupBoxCustomer.Controls.Clear();
        var paymentScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        var payment = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(14) };
        payment.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        void AddRow(Control control)
        {
            var row = payment.RowCount++;
            payment.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            control.Margin = new Padding(0, 0, 0, 10);
            control.Dock = DockStyle.Fill;
            payment.Controls.Add(control, 0, row);
        }
        labelCustomerPhone.Text = "Số điện thoại thành viên";
        txtCustomerPhone.PlaceholderText = "Nhập SĐT rồi Enter hoặc bấm Tìm khách";
        var customerActions = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        customerActions.Controls.AddRange(new Control[] { _findCustomer, _guestCustomer });
        lblCustomerName.Height = 78;
        lblCustomerName.Padding = new Padding(8);
        lblCustomerName.BackColor = Color.White;
        labelTotal.Text = "TỔNG CẦN THANH TOÁN";
        lblTotalAmount.ForeColor = Color.FromArgb(21, 128, 61);
        lblTotalAmount.BackColor = Color.White;
        labelCashReceived.Text = "Tiền khách đưa (VNĐ)";
        txtCashReceived.PlaceholderText = "Ví dụ: 100000";
        txtCashReceived.Font = new Font("Segoe UI", 14);
        txtCashReceived.TextAlign = HorizontalAlignment.Right;
        var cashActions = new FlowLayoutPanel { AutoSize = true, WrapContents = true };
        foreach (var amount in new[] { 0, 50000, 100000, 200000, 500000 })
        {
            var button = new Button { AutoSize = true, Text = amount == 0 ? "Vừa đủ" : amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")), Tag = amount };
            button.Click += (_, _) => txtCashReceived.Text = (amount == 0 ? PayableTotal : amount).ToString("0", CultureInfo.InvariantCulture);
            _cashButtons.Add(button); cashActions.Controls.Add(button);
        }
        labelChange.Text = "Tiền trả lại khách";
        btnCheckout.Height = 48;
        btnCheckout.BackColor = Color.FromArgb(21, 128, 61);
        btnCheckout.ForeColor = Color.White;
        btnCheckout.FlatStyle = FlatStyle.Flat;
        btnClearCart.Height = 36;
        btnClearCart.Text = "Hủy giao dịch";
        foreach (var control in new Control[] { labelCustomerPhone, txtCustomerPhone, customerActions, lblCustomerName,
            _useRewardPoints, labelTotal, lblTotalAmount, labelCashReceived, txtCashReceived,
            cashActions, labelChange, lblChange, _paymentStatus, btnCheckout, btnClearCart }) AddRow(control);
        paymentScroll.Controls.Add(payment);
        groupBoxCustomer.Controls.Add(paymentScroll);
        layout.Controls.Add(groupBoxCart, 0, 0); layout.Controls.Add(groupBoxCustomer, 1, 0);
        Controls.Add(layout);
        _useRewardPoints.CheckedChanged += (_, _) => UpdateCart();
        _findCustomer.Click += async (_, _) => await FindCustomerAsync();
        _guestCustomer.Click += (_, _) => { txtCustomerPhone.Clear(); ShowCustomer(null); };
        txtCustomerPhone.TextChanged += (_, _) => {
            _selectedCustomer = null;
            UpdateRedeemAvailability();
            lblCustomerName.Text = string.IsNullOrWhiteSpace(txtCustomerPhone.Text)
                ? "Khách vãng lai\r\nKhông tích điểm thành viên"
                : "Chưa xác nhận khách hàng\r\nBấm Tìm khách để kiểm tra thông tin.";
        };
        ShowCustomer(null);
        ResumeLayout(true);
    }

    private void ShowCustomer(CustomerRow? customer)
    {
        _selectedCustomer = customer;
        lblCustomerName.Text = customer == null ? "Khách vãng lai\r\nKhông tích điểm thành viên"
            : $"{customer.CustomerName}\r\nHạng: {customer.MembershipRank} • Điểm: {customer.RewardPoints:N0}";
        UpdateRedeemAvailability();
    }

    // Đổi điểm chỉ bật khi tìm đúng khách và đủ điều kiện.
    private void UpdateRedeemAvailability()
    {
        var canRedeem = _selectedCustomer != null && _selectedCustomer.RewardPoints >= 50
            && Total > 50000m;
        if (!canRedeem && _useRewardPoints.Checked) _useRewardPoints.Checked = false;
        _useRewardPoints.Enabled = canRedeem && !_busy;
    }

    private async Task<bool> FindCustomerAsync()
    {
        if (_findingCustomer || _busy) return false;
        var phone = txtCustomerPhone.Text.Trim();
        if (phone.Length == 0) { ShowCustomer(null); return true; }
        _findingCustomer = true;
        SetPaymentEnabled(false);
        _paymentStatus.Text = "Đang tra cứu khách hàng…";
        try
        {
            using var response = await ApiClientService.Client.GetAsync("customers/phone/" + Uri.EscapeDataString(phone));
            if (IsDisposed) return false;
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _selectedCustomer = null;
                lblCustomerName.Text = "Chưa có khách hàng với SĐT này.";
                _paymentStatus.Text = "Thêm khách ở menu Khách hàng hoặc chọn Khách vãng lai.";
                return false;
            }
            response.EnsureSuccessStatusCode();
            var customer = await response.Content.ReadFromJsonAsync<CustomerRow>();
            if (IsDisposed || customer == null) return false;
            ShowCustomer(customer);
            _paymentStatus.Text = "Khách hàng sẽ được tích điểm khi thanh toán thành công.";
            return true;
        }
        catch (Exception ex) { if (!IsDisposed) { _selectedCustomer = null; _paymentStatus.Text = "Không tra cứu được khách hàng: " + ex.Message; } return false; }
        finally { _findingCustomer = false; if (!IsDisposed) SetPaymentEnabled(true); }
    }

    private void SetPaymentEnabled(bool enabled)
    {
        txtCustomerPhone.Enabled = _findCustomer.Enabled = _guestCustomer.Enabled = enabled;
        _useRewardPoints.Enabled = enabled && _selectedCustomer is { RewardPoints: >= 50 }
            && Total > 50000m;
        txtCashReceived.Enabled = btnCheckout.Enabled = btnClearCart.Enabled = txtBarcode.Enabled = enabled;
        foreach (var button in _cashButtons) button.Enabled = enabled;
    }

    private bool TryGetCash(out decimal cash) => decimal.TryParse(txtCashReceived.Text.Trim(),
        NumberStyles.None, CultureInfo.InvariantCulture, out cash) && cash >= 0;

    private static async Task<string> PaymentErrorAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        try {
            using var json = JsonDocument.Parse(text);
            if (json.RootElement.TryGetProperty("message", out var message)) return message.GetString() ?? "Thanh toán thất bại.";
        } catch (JsonException) { }
        return $"Thanh toán thất bại (HTTP {(int)response.StatusCode}). Vui lòng kiểm tra kết nối và phiên đăng nhập.";
    }
}
