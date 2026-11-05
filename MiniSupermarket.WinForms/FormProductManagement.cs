using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormProductManagement : Form
    {
        // DTO lồng bên trong form để không trùng với ProductDto/CategoryDto đã có ở nơi khác
        public class ProductRow
        {
            public int ProductId { get; set; }
            public string Barcode { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }

        public class CategoryItem
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }

        private List<ProductRow> _allProducts = new();

        public FormProductManagement()
        {
            InitializeComponent();
            SetupGrid();
            dgvProducts.CellFormatting += dgvProducts_CellFormatting;
        }

        private void SetupGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "ProductId", HeaderText = "Mã SP", Width = 70 });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Barcode", HeaderText = "Mã vạch", Width = 130 });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                HeaderText = "Tên sản phẩm",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Đơn giá",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle
                { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "StockQuantity",
                HeaderText = "Tồn kho",
                Width = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "CategoryName", HeaderText = "Nhóm hàng", Width = 150 });
        }

        // TC43: tô màu cảnh báo tự động, kể cả khi tìm kiếm/lọc/sắp xếp.

        private void dgvProducts_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvProducts.Columns[e.ColumnIndex]
                .DataPropertyName != "StockQuantity")
                return;

            if (dgvProducts.Rows[e.RowIndex]
                .DataBoundItem is ProductRow product)
            {
                if (product.StockQuantity <= 10)
                {
                    // Tô màu cảnh báo khi tồn kho <= 10
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.DarkRed;

                    e.CellStyle.SelectionBackColor =
                        Color.OrangeRed;

                    e.CellStyle.SelectionForeColor =
                        Color.White;
                }
                else
                {
                    // Tồn kho bình thường
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.Black;

                    e.CellStyle.SelectionBackColor =
                        SystemColors.Highlight;

                    e.CellStyle.SelectionForeColor =
                        SystemColors.HighlightText;
                }
            }
        }


        // =====================================================
        // TẢI DỮ LIỆU
        // =====================================================

        private async void FormProductManagement_Load(object sender, EventArgs e)
        {
            await LoadCategoriesAsync();
            await LoadProductsAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await ApiClientService.Client
                    .GetFromJsonAsync<List<CategoryItem>>("categories") ?? new List<CategoryItem>();

                // Combo nhập liệu
                cboCategory.DataSource = new List<CategoryItem>(categories);
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryId";

                // Combo lọc (có thêm mục "Tất cả")
                var filterList = new List<CategoryItem>
                {
                    new CategoryItem { CategoryId = 0, CategoryName = "-- Tất cả nhóm hàng --" }
                };
                filterList.AddRange(categories);

                cboFilterCategory.DataSource = filterList;
                cboFilterCategory.DisplayMember = "CategoryName";
                cboFilterCategory.ValueMember = "CategoryId";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp danh mục: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                _allProducts = await ApiClientService.Client
                    .GetFromJsonAsync<List<ProductRow>>("products") ?? new List<ProductRow>();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // TÌM KIẾM / LỌC (xử lý phía client trên danh sách đã tải)
        // =====================================================

        private void ApplyFilter()
        {
            string keyword = txtSearchBarcode.Text.Trim();
            int categoryId = (cboFilterCategory.SelectedItem as CategoryItem)?.CategoryId ?? 0;

            IEnumerable<ProductRow> query = _allProducts;

            if (keyword.Length > 0)
            {
                query = query.Where(p =>
                    p.Barcode.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                    p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            dgvProducts.DataSource = query.ToList();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void txtSearchBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ApplyFilter();
            }
        }

        // =====================================================
        // CHỌN DÒNG -> ĐỔ LÊN FORM CHI TIẾT
        // =====================================================

        private void dgvProducts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvProducts.Rows[e.RowIndex].DataBoundItem is not ProductRow p) return;

            txtId.Text = p.ProductId.ToString();
            txtBarcode.Text = p.Barcode;
            txtProductName.Text = p.ProductName;
            nudPrice.Value = Math.Min(p.Price, nudPrice.Maximum);
            nudStock.Value = Math.Min(p.StockQuantity, (int)nudStock.Maximum);
            cboCategory.SelectedValue = p.CategoryId;
        }

        // =====================================================
        // THÊM / SỬA / XÓA
        // =====================================================

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã vạch!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProductName.Focus();
                return false;
            }

            if (nudPrice.Value <= 0)
            {
                MessageBox.Show("Đơn giá phải lớn hơn 0!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nudPrice.Focus();
                return false;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn nhóm hàng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            var newProduct = new
            {
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = (int)cboCategory.SelectedValue!
            };

            if (await SendAsync(() => ApiClientService.Client.PostAsJsonAsync("products", newProduct),
                    "Thêm mới sản phẩm thành công!"))
            {
                ClearInputs();
                await LoadProductsAsync();
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm trong danh sách để cập nhật!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs()) return;

            var updated = new
            {
                ProductId = id,
                Barcode = txtBarcode.Text.Trim(),
                ProductName = txtProductName.Text.Trim(),
                Price = nudPrice.Value,
                StockQuantity = (int)nudStock.Value,
                CategoryId = (int)cboCategory.SelectedValue!
            };

            if (await SendAsync(() => ApiClientService.Client.PutAsJsonAsync($"products/{id}", updated),
                    "Cập nhật sản phẩm thành công!"))
            {
                await LoadProductsAsync();
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm trong danh sách để xóa!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Xác nhận xóa sản phẩm \"{txtProductName.Text}\" (ID = {id})?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            if (await SendAsync(() => ApiClientService.Client.DeleteAsync($"products/{id}"),
                    "Đã xóa sản phẩm!"))
            {
                ClearInputs();
                await LoadProductsAsync();
            }
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            ClearInputs();
            txtSearchBarcode.Clear();
            if (cboFilterCategory.Items.Count > 0) cboFilterCategory.SelectedIndex = 0;
            await LoadProductsAsync();
        }

        // =====================================================
        // TIỆN ÍCH
        // =====================================================

        private void ClearInputs()
        {
            txtId.Clear();
            txtBarcode.Clear();
            txtProductName.Clear();
            nudPrice.Value = 0;
            nudStock.Value = 0;
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            txtBarcode.Focus();
        }

        /// <summary>
        /// Gửi request, hiện thông báo thành công/thất bại. Trả về true nếu thành công.
        /// </summary>
        private async Task<bool> SendAsync(Func<Task<HttpResponseMessage>> request, string successMessage)
        {
            try
            {
                var res = await request();

                if (res.IsSuccessStatusCode)
                {
                    MessageBox.Show(successMessage, "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }

                string detail = await res.Content.ReadAsStringAsync();
                if (detail.Length > 300) detail = detail[..300] + "...";

                MessageBox.Show($"Thao tác thất bại (mã {(int)res.StatusCode}).\n{detail}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }
    }
}