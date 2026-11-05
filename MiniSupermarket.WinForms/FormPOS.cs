using System.Net.Http.Json;
namespace MiniSupermarket.WinForms;

public partial class FormPOS : Form
{
    private readonly List<CartItem> _cart = new();
    private bool _busy;
    public FormPOS()
    {
        InitializeComponent();
        SetupPaymentArea();
        Shown += (_, _) => txtBarcode.Focus();
        dgvCart.AutoGenerateColumns = false;
        dgvCart.ReadOnly = true;
        var properties = new[] { "ProductId", "ProductName", "UnitPrice", "Quantity", "TotalPrice" };
        for (int i = 0; i < properties.Length; i++) dgvCart.Columns[i].DataPropertyName = properties[i];
    }
    private void FormPOS_Load(object sender, EventArgs e) => UpdateCart();
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F8)
        {
            txtBarcode.Focus();
            txtBarcode.SelectAll();
            return true;
        }
        if (keyData == Keys.F9) { btnCheckout.PerformClick(); return true; }
        return base.ProcessCmdKey(ref msg, keyData);
    }
    private async void txtBarcode_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        if (_busy || _findingCustomer || string.IsNullOrWhiteSpace(txtBarcode.Text)) return;
        _busy = true;
        txtBarcode.ReadOnly = true;
        SetPaymentEnabled(false);
        try
        {
            var product = await ApiClientService.Client.GetFromJsonAsync<FormProductManagement.ProductRow>(
                "products/barcode/" + Uri.EscapeDataString(txtBarcode.Text.Trim()));
            if (IsDisposed) return;
            if (product == null) return;
            var item = _cart.FirstOrDefault(i => i.ProductId == product.ProductId);
            if ((item?.Quantity ?? 0) >= product.StockQuantity) { MessageBox.Show("Không đủ tồn kho."); return; }
            if (item == null) _cart.Add(new CartItem { ProductId = product.ProductId, ProductName = product.ProductName, UnitPrice = product.Price, Quantity = 1 });
            else item.Quantity++;
            txtBarcode.Clear();
            UpdateCart();
        }
        catch (Exception ex) { if (!IsDisposed) MessageBox.Show("Không thể thêm sản phẩm: " + ex.Message); }
        finally
        {
            _busy = false;
            if (!IsDisposed)
            {
                txtBarcode.ReadOnly = false;
                SetPaymentEnabled(true);
                txtBarcode.Focus();
                txtBarcode.SelectAll();
            }
        }
    }
    private async void txtCustomerPhone_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Enter) return;
        e.SuppressKeyPress = true;
        await FindCustomerAsync();
    }
    private decimal Total => _cart.Sum(i => i.TotalPrice);
    // TC47: giảm 50.000đ nếu thành viên có >= 50 điểm và bật đổi điểm.
    private decimal PayableTotal => Total - (_useRewardPoints.Checked ? 50000m : 0m);
    private void UpdateCart()
    {
        dgvCart.DataSource = null;
        dgvCart.DataSource = _cart.ToList();
        UpdateRedeemAvailability();
        lblTotalAmount.Text = _useRewardPoints.Checked
            ? $"{PayableTotal:N0} đ  (đã trừ 50.000đ)"
            : $"{Total:N0} đ";
        CalculateChange();
    }
    private void txtCashReceived_TextChanged(object sender, EventArgs e) => CalculateChange();
    private void CalculateChange()
    {
        var valid = TryGetCash(out var cash) && cash >= PayableTotal;
        lblChange.Text = string.IsNullOrWhiteSpace(txtCashReceived.Text) ? "0 đ"
            : valid ? $"{cash - PayableTotal:N0} đ" : "Chưa đủ tiền!";
        lblChange.ForeColor = valid ? Color.Black : Color.Red;
    }
    private async void btnCheckout_Click(object sender, EventArgs e)
    {
        if (_busy || _findingCustomer) return;
        if (_cart.Count == 0) { MessageBox.Show("Giỏ hàng đang trống."); return; }
        if (!TryGetCash(out var cash) || cash < PayableTotal)
        {
            System.Media.SystemSounds.Exclamation.Play();
            MessageBox.Show("Tiền khách đưa chưa đủ.", "Cảnh báo thanh toán",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!string.IsNullOrWhiteSpace(txtCustomerPhone.Text) &&
            (_selectedCustomer == null || _selectedCustomer.PhoneNumber != txtCustomerPhone.Text.Trim()))
        {
            if (!await FindCustomerAsync()) return;
        }
        _busy = true;
        SetPaymentEnabled(false);
        _paymentStatus.Text = "Đang lưu hóa đơn…";
        try
        {
            using var response = await ApiClientService.Client.PostAsJsonAsync("orders/checkout", new {
                CustomerPhone = _selectedCustomer?.PhoneNumber,
                CashReceived = cash,
                UseRewardPoints = _useRewardPoints.Checked,
                Items = _cart.Select(i => new { i.ProductId, i.Quantity, i.UnitPrice }).ToList() });
            if (IsDisposed) return;
            if (!response.IsSuccessStatusCode) { _paymentStatus.Text = await PaymentErrorAsync(response); MessageBox.Show(_paymentStatus.Text, "Thanh toán thất bại"); return; }
            var receipt = await response.Content.ReadFromJsonAsync<Receipt>();
            MessageBox.Show($"Thanh toán thành công!\nHóa đơn: {receipt?.OrderId}\nGiảm điểm: {receipt?.DiscountAmount:N0} đ\nTổng thanh toán: {receipt?.TotalAmount:N0} đ\nTiền thừa: {receipt?.Change:N0} đ\nĐiểm đã dùng: {receipt?.PointsSpent} | Tích thêm: {receipt?.PointsEarned}");
            ClearCart();
        }
        catch (Exception ex) { if (!IsDisposed) MessageBox.Show("Không nhận được kết quả thanh toán. Kiểm tra báo cáo trước khi thử lại.\n" + ex.Message); }
        finally { _busy = false; if (!IsDisposed) SetPaymentEnabled(true); }
    }
    private void btnClearCart_Click(object sender, EventArgs e)
    {
        if (!_busy && (_cart.Count == 0 || MessageBox.Show("Hủy toàn bộ giỏ hàng?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)) ClearCart();
    }
    private void ClearCart()
    {
        _cart.Clear(); txtCashReceived.Clear(); txtCustomerPhone.Clear();
        ShowCustomer(null); _paymentStatus.Text = "Sẵn sàng cho giao dịch mới."; UpdateCart(); txtBarcode.Focus();
    }
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
    public record Receipt(int OrderId, decimal TotalAmount, decimal Change,
        decimal DiscountAmount, int PointsSpent, int PointsEarned);
}

