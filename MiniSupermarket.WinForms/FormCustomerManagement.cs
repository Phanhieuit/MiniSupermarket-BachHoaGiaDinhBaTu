using System.Net.Http.Json;
namespace MiniSupermarket.WinForms;

public partial class FormCustomerManagement : Form
{
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false };
    private readonly TextBox _name = new() { PlaceholderText = "Họ tên", Width = 180 };
    private readonly TextBox _phone = new() { PlaceholderText = "Số điện thoại", Width = 150 };
    private readonly TextBox _address = new() { PlaceholderText = "Địa chỉ", Width = 220 };
    private List<CustomerRow> _customers = new();
    public FormCustomerManagement()
    {
        InitializeComponent();
        var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(12), AutoScroll = true };
        var add = new Button { Text = "Thêm mới", AutoSize = true };
        var reload = new Button { Text = "Tải lại", AutoSize = true };
        var search = new TextBox { PlaceholderText = "Tìm tên / SĐT", Width = 180 };
        bar.Controls.AddRange(new Control[] { _name, _phone, _address, add, reload, search });
        Controls.Add(_grid); Controls.Add(bar);
        Load += async (_, _) => await Reload();
        reload.Click += async (_, _) => await Reload();
        search.TextChanged += (_, _) => _grid.DataSource = _customers.Where(c => c.CustomerName.Contains(search.Text, StringComparison.OrdinalIgnoreCase) || c.PhoneNumber.Contains(search.Text)).ToList();
        add.Click += async (_, _) => {
            add.Enabled = false;
            try {
                using var response = await ApiClientService.Client.PostAsJsonAsync("customers", new { CustomerName = _name.Text, PhoneNumber = _phone.Text, Address = _address.Text });
                if (!response.IsSuccessStatusCode) { MessageBox.Show(await response.Content.ReadAsStringAsync()); return; }
                _name.Clear(); _phone.Clear(); _address.Clear(); await Reload();
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
            finally { if (!IsDisposed) add.Enabled = true; }
        };
    }
    private async Task Reload()
    {
        try {
            _customers = await ApiClientService.Client.GetFromJsonAsync<List<CustomerRow>>("customers") ?? new();
            if (IsDisposed) return;
            _grid.DataSource = _customers;
            string[] titles = { "Mã KH", "Tên khách hàng", "Số điện thoại", "Địa chỉ", "Hạng thẻ", "Điểm" };
            for (int i = 0; i < titles.Length && i < _grid.Columns.Count; i++) _grid.Columns[i].HeaderText = titles[i];
        } catch (Exception ex) { if (!IsDisposed) MessageBox.Show(ex.Message); }
    }
}
public class CustomerRow
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string? Address { get; set; }
    public string MembershipRank { get; set; } = "";
    public int RewardPoints { get; set; }
}
