using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormCategoryManagement : Form
    {
        // =========================================================
        // KẾT NỐI WEB API
        // =========================================================


        private static readonly HttpClient _client =
            ApiClientService.Client;


        //private static readonly HttpClient _client = new HttpClient
        //{
        //    BaseAddress = new Uri("https://localhost:7237/api/")
        //};  

        // =========================================================
        // CONSTRUCTOR
        // =========================================================
        public FormCategoryManagement()
        {
            InitializeComponent();
            // Gắn Token đã lưu lúc đăng nhập vào Header của HttpClient
            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }
            // Thiết lập DataGridView
            SetupDataGridView();
        }

        // =========================================================
        // THIẾT LẬP DATAGRIDVIEW
        // =========================================================
        private void SetupDataGridView()
        {
            // Không tự động tạo cột
            dgvCategories.AutoGenerateColumns = false;

            // Xóa toàn bộ cột đang có trong Designer
            dgvCategories.Columns.Clear();

            // -----------------------------
            // CỘT MÃ ID
            // -----------------------------
            DataGridViewTextBoxColumn colId =
                new DataGridViewTextBoxColumn();

            colId.Name = "CategoryId";
            colId.HeaderText = "Mã ID";
            colId.DataPropertyName = "CategoryId";
            colId.Width = 80;
            colId.ReadOnly = true;

            dgvCategories.Columns.Add(colId);

            // -----------------------------
            // CỘT TÊN NHÓM HÀNG
            // -----------------------------
            DataGridViewTextBoxColumn colName =
                new DataGridViewTextBoxColumn();

            colName.Name = "CategoryName";
            colName.HeaderText = "Tên Nhóm hàng";
            colName.DataPropertyName = "CategoryName";
            colName.Width = 150;

            dgvCategories.Columns.Add(colName);

            // -----------------------------
            // CỘT MÔ TẢ
            // -----------------------------
            DataGridViewTextBoxColumn colDescription =
                new DataGridViewTextBoxColumn();

            colDescription.Name = "Description";
            colDescription.HeaderText = "Mô tả";
            colDescription.DataPropertyName = "Description";
            colDescription.AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;

            dgvCategories.Columns.Add(colDescription);

            // Một số thiết lập giao diện
            dgvCategories.AllowUserToAddRows = false;
            dgvCategories.AllowUserToDeleteRows = false;
            dgvCategories.ReadOnly = false;
            dgvCategories.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvCategories.MultiSelect = false;
        }

        // =========================================================
        // FORM LOAD
        // =========================================================
        private async void FormCategoryManagement_Load(
            object sender,
            EventArgs e)
        {
            await LoadDataAsync();
        }

        // =========================================================
        // LOAD DỮ LIỆU TỪ API
        // =========================================================
        private async Task LoadDataAsync()
        {
            try
            {
                var categories =
                    await _client.GetFromJsonAsync<List<CategoryDto>>(
                        "categories"
                    );

                if (categories == null)
                {
                    MessageBox.Show(
                        "API không trả về dữ liệu.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dgvCategories.DataSource = null;
                    return;
                }

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    "Không kết nối được Web API!\n\n" +
                    ex.Message +
                    "\n\nKiểm tra API có đang chạy ở:\n" +
                    "https://localhost:7237",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dữ liệu:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // NÚT TẢI LẠI
        // =========================================================
        private async void btnLoad_Click(
            object sender,
            EventArgs e)
        {
            await LoadDataAsync();
        }

        // =========================================================
        // CLICK VÀO DÒNG
        // =========================================================
        private void dgvCategories_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                DataGridViewRow row =
                    dgvCategories.Rows[e.RowIndex];

                txtId.Text =
                    row.Cells["CategoryId"].Value?.ToString() ?? "";

                txtCategoryName.Text =
                    row.Cells["CategoryName"].Value?.ToString() ?? "";

                txtDescription.Text =
                    row.Cells["Description"].Value?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không đọc được dữ liệu dòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // THÊM MỚI
        // =========================================================
        private async void btnAdd_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtCategoryName.Focus();
                return;
            }

            try
            {
                var newCat = new
                {
                    CategoryName =
                        txtCategoryName.Text.Trim(),

                    Description =
                        txtDescription.Text.Trim()
                };

                var response =
                    await _client.PostAsJsonAsync(
                        "categories",
                        newCat
                    );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Thêm mới thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Thêm mới thất bại!\n\n" +
                        "HTTP: " +
                        (int)response.StatusCode +
                        "\n" +
                        error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm mới:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // CẬP NHẬT
        // =========================================================
        private async void btnUpdate_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần sửa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Mã ID không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Vui lòng nhập tên nhóm hàng!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                var updateCat = new
                {
                    CategoryId = id,

                    CategoryName =
                        txtCategoryName.Text.Trim(),

                    Description =
                        txtDescription.Text.Trim()
                };

                var response =
                    await _client.PutAsJsonAsync(
                        $"categories/{id}",
                        updateCat
                    );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Cập nhật thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Cập nhật thất bại!\n\n" +
                        "HTTP: " +
                        (int)response.StatusCode +
                        "\n" +
                        error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi cập nhật:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // XÓA
        // =========================================================
        private async void btnDelete_Click(
            object sender,
            EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtId.Text))
            {
                MessageBox.Show(
                    "Vui lòng chọn nhóm hàng cần xóa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Mã ID không hợp lệ!",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }

            DialogResult confirm =
                MessageBox.Show(
                    $"Bạn có chắc muốn xóa nhóm hàng ID = {id}?",
                    "Xác nhận xóa",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                var response =
                    await _client.DeleteAsync(
                        $"categories/{id}"
                    );

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Xóa thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    ClearInputs();
                    await LoadDataAsync();
                }
                else
                {
                    string error =
                        await response.Content.ReadAsStringAsync();

                    MessageBox.Show(
                        "Xóa thất bại!\n\n" +
                        "HTTP: " +
                        (int)response.StatusCode +
                        "\n" +
                        error,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // TÌM KIẾM
        // =========================================================
        private async void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            string keyword =
                txtKeyword.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                await LoadDataAsync();
                return;
            }

            try
            {
                string encodedKeyword =
                    Uri.EscapeDataString(keyword);

                var result =
                    await _client.GetFromJsonAsync<List<CategoryDto>>(
                        $"categories/search?keyword={encodedKeyword}"
                    );

                if (result == null)
                {
                    dgvCategories.DataSource = null;
                    return;
                }

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = result;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    "Lỗi kết nối khi tìm kiếm:\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm:\n\n" +
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // =========================================================
        // XÓA TRẮNG INPUT
        // =========================================================
        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
        }

        // =========================================================
        // EVENT KHÔNG DÙNG
        // =========================================================
        private void dgvCategories_CellContentClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
        }

        private void txtId_TextChanged(
            object sender,
            EventArgs e)
        {
        }
    }

    // =============================================================
    // CATEGORY DTO
    // =============================================================
    public class CategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }
            = string.Empty;

        public string? Description { get; set; }
    }
}