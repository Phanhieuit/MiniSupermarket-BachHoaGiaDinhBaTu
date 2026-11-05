using System.Net.Http.Json;
namespace MiniSupermarket.WinForms;
public partial class FormQuickReport : Form
{
    public FormQuickReport()
    {
        InitializeComponent();
        btnRunReport.Click += async (_, _) => await LoadReport();
        Load += async (_, _) => await LoadReport();
    }
    private async Task LoadReport()
    {
        btnRunReport.Enabled = false;
        try {
            var report = await ApiClientService.Client.GetFromJsonAsync<DailyReport>($"reports/daily?date={dateTimePicker1.Value:yyyy-MM-dd}");
            if (IsDisposed || report == null) return;
            lblTotalOrders.Text = report.TotalOrders.ToString();
            lblTotalRevenue.Text = $"{report.TotalRevenue:N0} đ";
            lblBestSeller.Text = report.BestSeller;
        } catch (Exception ex) { if (!IsDisposed) MessageBox.Show("Không tải được báo cáo: " + ex.Message); }
        finally { if (!IsDisposed) btnRunReport.Enabled = true; }
    }
    public record DailyReport(int TotalOrders, decimal TotalRevenue, string BestSeller);
}
