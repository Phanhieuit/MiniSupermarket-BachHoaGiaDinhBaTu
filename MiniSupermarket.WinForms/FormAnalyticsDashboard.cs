
using System.Globalization;
using System.Net.Http.Json;
using System.Windows.Forms.DataVisualization.Charting;

namespace MiniSupermarket.WinForms;

// TC44-46,48: Dashboard quản trị đặt bên trong FormMainShell.
public sealed class FormAnalyticsDashboard : Form
{
    private readonly Label _todayRevenue = CreateMetric("0 đ");
    private readonly Label _totalInvoices = CreateMetric("0");
    private readonly Label _newCustomers = CreateMetric("0");
    private readonly Label _lowStock = CreateMetric("0");

    private readonly Label _status = new()
    {
        AutoSize = true,
        ForeColor = Color.DimGray,
        Text = "Đang nạp báo cáo..."
    };

    private readonly DateTimePicker _fromDate = new()
    {
        Format = DateTimePickerFormat.Short,
        Width = 128,
        Value = DateTime.Today.AddDays(-6)
    };

    private readonly DateTimePicker _toDate = new()
    {
        Format = DateTimePickerFormat.Short,
        Width = 128,
        Value = DateTime.Today
    };

    private readonly ComboBox _period = new()
    {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Width = 170
    };

    private readonly Button _refresh =
        ActionButton("Làm mới", Color.FromArgb(41, 100, 180));

    private readonly Button _exportProducts =
        ActionButton("Xuất danh mục CSV", Color.FromArgb(21, 128, 61));

    private readonly Button _exportOrders =
        ActionButton("Xuất hóa đơn CSV", Color.FromArgb(21, 128, 61));

    private readonly Chart _chart = new()
    {
        Dock = DockStyle.Fill,
        BackColor = Color.White
    };

    private readonly DataGridView _topGrid = new()
    {
        Dock = DockStyle.Fill,
        ReadOnly = true,
        AutoGenerateColumns = false,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        RowHeadersVisible = false,
        BackgroundColor = Color.White,
        BorderStyle = BorderStyle.None,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect
    };

    private bool _loading;

    public FormAnalyticsDashboard()
    {
        Text = "Dashboard - Bách hóa Gia đình Bà Tư";
        BackColor = Color.FromArgb(244, 246, 249);
        DoubleBuffered = true;

        _period.Items.AddRange(new object[]
        {
            "Theo ngày (7 ngày)",
            "Theo tuần (8 tuần)",
            "Theo tháng (12 tháng)"
        });

        _period.SelectedIndex = 0;

        // TC45 - Khởi tạo biểu đồ doanh thu
        _chart.ChartAreas.Add(new ChartArea("RevenueArea")
        {
            BackColor = Color.White
        });

        var area = _chart.ChartAreas["RevenueArea"];

        area.AxisX.Interval = 1;
        area.AxisX.MajorGrid.Enabled = false;
        area.AxisX.LabelStyle.Angle = -30;

        area.AxisY.LabelStyle.Format = "N0";
        area.AxisY.Title = "Doanh thu (VNĐ)";

        _chart.Legends.Add(new Legend("Doanh thu"));

        _chart.Series.Add(new Series("Doanh thu")
        {
            ChartType = SeriesChartType.Column,
            XValueType = ChartValueType.Int32,
            YValueType = ChartValueType.Double,
            Color = Color.FromArgb(41, 100, 180),
            BorderWidth = 1,
            IsValueShownAsLabel = false
        });

        // TC46 - Bảng Top 5 sản phẩm bán chạy
        _topGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "ProductId",
            HeaderText = "Mã SP",
            Width = 72
        });

        _topGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "ProductName",
            HeaderText = "Tên sản phẩm",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        _topGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "QuantitySold",
            HeaderText = "Đã bán",
            Width = 95
        });

        _topGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Revenue",
            HeaderText = "Doanh thu (đ)",
            Width = 150,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Format = "N0",
                Alignment = DataGridViewContentAlignment.MiddleRight
            }
        });

        BuildLayout();

        Load += async (_, _) =>
            await LoadDashboardAsync();

        _refresh.Click += async (_, _) =>
            await LoadDashboardAsync();

        _period.SelectedIndexChanged += async (_, _) =>
        {
            _fromDate.Value = _period.SelectedIndex switch
            {
                1 => DateTime.Today.AddDays(-49),
                2 => DateTime.Today.AddMonths(-11),
                _ => DateTime.Today.AddDays(-6)
            };

            _toDate.Value = DateTime.Today;

            if (IsHandleCreated && !_loading)
            {
                await LoadDashboardAsync();
            }
        };

        _exportProducts.Click += async (_, _) =>
            await ExportCsvAsync(
                "reports/export/products.csv",
                "DanhMucSanPham.csv"
            );

        _exportOrders.Click += async (_, _) =>
            await ExportCsvAsync(
                "reports/export/orders.csv",
                "BaoCaoHoaDon.csv"
            );
    }

    private static Label CreateMetric(string initial)
    {
        return new Label
        {
            Text = initial,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(20, 70, 120),
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    private static Button ActionButton(
        string text,
        Color color)
    {
        return new Button
        {
            Text = text,
            Width = 165,
            Height = 36,
            BackColor = color,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };
    }

    private static Control MetricCard(
        string title,
        Label metric)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(16, 8, 12, 8),
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(5),
            MinimumSize = new Size(150, 105)
        };

        panel.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 34));

        panel.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(new Label
        {
            Text = title,
            ForeColor = Color.FromArgb(70, 75, 87),
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Dock = DockStyle.Fill
        }, 0, 0);

        panel.Controls.Add(metric, 0, 1);

        return panel;
    }

    private void BuildLayout()
    {
        var scrolling = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        var stack = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            Padding = new Padding(16),
            BackColor = BackColor
        };

        stack.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        void Add(Control control, int height)
        {
            int row = stack.RowCount++;

            stack.RowStyles.Add(
                new RowStyle(SizeType.Absolute, height));

            control.Dock = DockStyle.Fill;
            control.Margin = new Padding(0, 0, 0, 12);

            stack.Controls.Add(control, 0, row);
        }

        Add(new Label
        {
            Text = "TỔNG QUAN KINH DOANH",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 40, 60),
            TextAlign = ContentAlignment.MiddleLeft
        }, 50);

        // TC44 - Bốn thẻ KPI
        var cards = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4
        };

        for (int i = 0; i < 4; i++)
        {
            cards.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25));
        }

        cards.Controls.Add(
            MetricCard("DOANH THU HÔM NAY", _todayRevenue),
            0, 0);

        cards.Controls.Add(
            MetricCard("TỔNG HÓA ĐƠN", _totalInvoices),
            1, 0);

        cards.Controls.Add(
            MetricCard("KHÁCH MỚI HÔM NAY", _newCustomers),
            2, 0);

        cards.Controls.Add(
            MetricCard("MẶT HÀNG SẮP HẾT (≤ 10)", _lowStock),
            3, 0);

        Add(cards, 127);

        // Thanh công cụ báo cáo
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            WrapContents = true,
            Padding = new Padding(0, 5, 0, 0)
        };

        toolbar.Controls.Add(new Label
        {
            Text = "Từ ngày:",
            AutoSize = true,
            Padding = new Padding(0, 11, 2, 0)
        });

        toolbar.Controls.Add(_fromDate);

        toolbar.Controls.Add(new Label
        {
            Text = "Đến ngày:",
            AutoSize = true,
            Padding = new Padding(0, 11, 2, 0)
        });

        toolbar.Controls.Add(_toDate);
        toolbar.Controls.Add(_period);
        toolbar.Controls.Add(_refresh);
        toolbar.Controls.Add(_exportProducts);
        toolbar.Controls.Add(_exportOrders);

        Add(toolbar, 95);
        Add(_status, 25);

        // Khu vực biểu đồ
        var chartPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = Color.White
        };

        chartPanel.Controls.Add(_chart);

        Add(chartPanel, 330);

        // Khu vực Top 5
        Add(new Label
        {
            Text = "TOP 5 SẢN PHẨM BÁN CHẠY",
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 40, 60),
            TextAlign = ContentAlignment.MiddleLeft
        }, 44);

        Add(_topGrid, 235);

        scrolling.Controls.Add(stack);
        Controls.Add(scrolling);
    }

    // TC44, TC45, TC46 - Tải dữ liệu từ Web API
    private async Task LoadDashboardAsync()
    {
        if (_loading)
            return;

        _loading = true;
        _refresh.Enabled = false;

        _status.Text = "Đang tải số liệu từ SQL Server...";

        try
        {
            string period = _period.SelectedIndex switch
            {
                1 => "week",
                2 => "month",
                _ => "day"
            };

            var client = ApiClientService.Client;

            var summaryTask =
                client.GetFromJsonAsync<DashboardSummary>(
                    "reports/dashboard-summary");

            var trendTask =
                client.GetFromJsonAsync<TrendResponse>(
                    $"reports/revenue-trend?period={period}" +
                    $"&from={_fromDate.Value:yyyy-MM-dd}" +
                    $"&to={_toDate.Value:yyyy-MM-dd}");

            var bestTask =
                client.GetFromJsonAsync<List<TopProduct>>(
                    "reports/top-products?take=5");

            await Task.WhenAll(
                summaryTask,
                trendTask,
                bestTask);

            if (IsDisposed)
                return;

            var summary = await summaryTask;
            var trend = await trendTask;
            var best = await bestTask;

            if (summary == null || trend == null)
            {
                throw new InvalidOperationException(
                    "Thiếu dữ liệu báo cáo.");
            }

            // TC44 - Cập nhật các thẻ KPI
            _todayRevenue.Text =
                $"{summary.TodayRevenue:N0} đ";

            _totalInvoices.Text =
                $"{summary.TotalInvoices:N0}";

            _newCustomers.Text =
                $"{summary.NewCustomersToday:N0}";

            _lowStock.Text =
                $"{summary.LowStockProducts:N0}";

            _lowStock.ForeColor =
                summary.LowStockProducts > 0
                    ? Color.Firebrick
                    : Color.ForestGreen;

            // ============================================
            // TC45 - SỬA LỖI CỘT DOANH THU BỊ CHỒNG NHAU
            // ============================================

            var series = _chart.Series["Doanh thu"];
            var area = _chart.ChartAreas["RevenueArea"];

            series.Points.Clear();

            series.XValueType = ChartValueType.Int32;
            series.YValueType = ChartValueType.Double;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Interval = 1;
            area.AxisX.LabelStyle.Angle = -30;

            area.AxisX.Minimum = 0.5;
            area.AxisX.Maximum =
                Math.Max(1, trend.Data.Count) + 0.5;

            for (int i = 0; i < trend.Data.Count; i++)
            {
                var row = trend.Data[i];

                var day = DateTime.ParseExact(
                    row.Date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture);

                string label = period == "month"
                    ? day.ToString("MM/yyyy")
                    : day.ToString("dd/MM");

                // Mỗi ngày ở vị trí trục X khác nhau
                int index = series.Points.AddXY(
                    i + 1,
                    (double)row.TotalRevenue);

                // Hiển thị nhãn ngày
                series.Points[index].AxisLabel = label;

                // Tooltip khi rê chuột
                series.Points[index].ToolTip =
                    $"{row.Date}: " +
                    $"{row.TotalRevenue:N0} đ " +
                    $"({row.TotalOrders} hóa đơn)";
            }

            area.RecalculateAxesScale();

            _chart.Invalidate();

            // TC46 - Top 5 sản phẩm
            _topGrid.DataSource =
                best ?? new List<TopProduct>();

            _status.Text =
                $"Đã cập nhật • {trend.Data.Count} mốc thời gian " +
                $"• {best?.Count ?? 0} mặt hàng bán chạy";
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
            {
                _status.Text =
                    "Không tải được Dashboard: " +
                    ex.Message +
                    " (Hãy kiểm tra Update-Database và đăng nhập Admin.)";
            }
        }
        finally
        {
            _loading = false;

            if (!IsDisposed)
                _refresh.Enabled = true;
        }
    }

    // TC48 - Xuất dữ liệu CSV
    private async Task ExportCsvAsync(
        string endpoint,
        string fileName)
    {
        try
        {
            using var response =
                await ApiClientService.Client.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            using var dialog = new SaveFileDialog
            {
                Filter = "CSV UTF-8 (*.csv)|*.csv",
                FileName = fileName,
                DefaultExt = "csv",
                AddExtension = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
                return;

            await File.WriteAllBytesAsync(
                dialog.FileName,
                await response.Content.ReadAsByteArrayAsync());

            if (!IsDisposed)
            {
                MessageBox.Show(
                    this,
                    "Đã xuất tệp CSV:\n" + dialog.FileName,
                    "Thành công",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
            {
                MessageBox.Show(
                    this,
                    "Xuất báo cáo thất bại: " + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }

    // DTO - Dữ liệu Dashboard
    public sealed class DashboardSummary
    {
        public decimal TodayRevenue { get; set; }
        public int TotalInvoices { get; set; }
        public int NewCustomersToday { get; set; }
        public int LowStockProducts { get; set; }
    }

    // DTO - Biểu đồ doanh thu
    public sealed class TrendResponse
    {
        public string Period { get; set; } = "";
        public List<TrendPoint> Data { get; set; } = new();
    }

    public sealed class TrendPoint
    {
        public string Date { get; set; } = "";
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // DTO - Top 5 sản phẩm bán chạy
    public sealed class TopProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}
