using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarket.WinForms
{
    public partial class FormUserManagement : Form
    {
        private const int MinPasswordLength = 6;

        // Tài khoản đang được chọn trong lưới (null = đang ở chế độ nhập mới)
        private UserDto? _selectedUser;

        public FormUserManagement()
        {
            InitializeComponent();
            SetupGrid();

            cboRole.Items.AddRange(new string[] { "Admin", "Cashier", "Warehouse" });
            cboRole.SelectedIndex = 1;
        }

        private void SetupGrid()
        {
            dgvUsers.AutoGenerateColumns = false;
            // Không tự co giãn các cột
            dgvUsers.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None;

            // Cho phép cuộn ngang khi bảng không đủ rộng
            dgvUsers.ScrollBars = ScrollBars.Both;

            dgvUsers.Columns.Clear();

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Id", HeaderText = "ID", Width = 60 });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Username", HeaderText = "Tên đăng nhập", Width = 160 });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FullName",
                HeaderText = "Họ và tên",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Width = 230
            });

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn
            { DataPropertyName = "Role", HeaderText = "Vai trò", Width = 110 });

            dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn
            { DataPropertyName = "IsActive", HeaderText = "Hoạt động", Width = 90 });

            // Tài khoản bị khóa hiển thị màu xám
            dgvUsers.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0 &&
                    dgvUsers.Rows[e.RowIndex].DataBoundItem is UserDto u &&
                    !u.IsActive)
                {
                    if (e.CellStyle != null) e.CellStyle.ForeColor = Color.Gray;
                }
            };
        }

        // =====================================================
        // TẢI DANH SÁCH
        // =====================================================

        private async void FormUserManagement_Load(object sender, EventArgs e)
        {
            await LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await ApiClientService.Client.GetFromJsonAsync<List<UserDto>>("users");
                dgvUsers.DataSource = users ?? new List<UserDto>();
                dgvUsers.ClearSelection();
                _selectedUser = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy danh sách tài khoản: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================
        // CHỌN DÒNG TRONG LƯỚI
        // =====================================================

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvUsers.Rows[e.RowIndex].DataBoundItem is not UserDto user) return;

            _selectedUser = user;

            txtUsername.Text = user.Username;
            txtUsername.ReadOnly = true;     // không cho đổi tên đăng nhập của tài khoản có sẵn
            txtFullName.Text = user.FullName;
            txtPassword.Clear();

            int idx = cboRole.FindStringExact(user.Role);
            if (idx >= 0) cboRole.SelectedIndex = idx;

            btnToggleLock.Text = user.IsActive ? "Khóa tài khoản" : "Mở khóa tài khoản";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedUser = null;
            dgvUsers.ClearSelection();

            txtUsername.ReadOnly = false;
            txtUsername.Clear();
            txtFullName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 1;
            btnToggleLock.Text = "Khóa / Mở khóa tài khoản";
            txtUsername.Focus();
        }

        // =====================================================
        // TẠO TÀI KHOẢN MỚI
        // =====================================================

        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            if (_selectedUser != null)
            {
                MessageBox.Show("Đang chọn một tài khoản có sẵn. Hãy nhấn \"Nhập mới / Bỏ chọn\" trước khi tạo tài khoản khác.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string fullName = txtFullName.Text.Trim();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Tên đăng nhập và mật khẩu không được trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (username.Length < 3 || username.Contains(' '))
            {
                MessageBox.Show("Tên đăng nhập tối thiểu 3 ký tự và không chứa khoảng trắng!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                MessageBox.Show("Vui lòng nhập họ và tên nhân viên!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            if (password.Length < MinPasswordLength)
            {
                MessageBox.Show($"Mật khẩu tối thiểu {MinPasswordLength} ký tự!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            var newUser = new
            {
                Username = username,
                Password = password,
                FullName = fullName,
                Role = cboRole.SelectedItem?.ToString() ?? "Cashier"
            };

            if (await SendAsync(() => ApiClientService.Client.PostAsJsonAsync("users", newUser),
                    "Tạo tài khoản mới thành công!"))
            {
                await LoadUsersAsync();
                ClearForm();
            }
        }

        // =====================================================
        // ĐẶT LẠI MẬT KHẨU
        // =====================================================

        private async void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản trong danh sách!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPassword = txtPassword.Text.Trim();

            if (newPassword.Length < MinPasswordLength)
            {
                MessageBox.Show($"Nhập mật khẩu mới (tối thiểu {MinPasswordLength} ký tự) vào ô Mật khẩu rồi bấm lại!",
                    "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            var confirm = MessageBox.Show(
                $"Đặt lại mật khẩu cho tài khoản \"{_selectedUser.Username}\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            int id = _selectedUser.Id;

            if (await SendAsync(
                    () => ApiClientService.Client.PutAsJsonAsync($"users/{id}/reset-password", new { NewPassword = newPassword }),
                    "Đặt lại mật khẩu thành công!"))
            {
                txtPassword.Clear();
            }
        }

        // =====================================================
        // KHÓA / MỞ KHÓA
        // =====================================================

        private async void btnToggleLock_Click(object sender, EventArgs e)
        {
            if (_selectedUser == null)
            {
                MessageBox.Show("Vui lòng chọn một tài khoản trong danh sách!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Không cho tự khóa chính mình
            if (string.Equals(_selectedUser.Username, SessionManager.CurrentUsername,
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Bạn không thể khóa tài khoản đang đăng nhập!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string action = _selectedUser.IsActive ? "khóa" : "mở khóa";

            var confirm = MessageBox.Show(
                $"Xác nhận {action} tài khoản \"{_selectedUser.Username}\"?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            int id = _selectedUser.Id;

            if (await SendAsync(
                    () => ApiClientService.Client.PutAsync($"users/{id}/toggle-lock", null),
                    $"Đã {action} tài khoản!"))
            {
                await LoadUsersAsync();
                ClearForm();
            }
        }

        // =====================================================
        // TIỆN ÍCH
        // =====================================================

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

                string detail = ExtractMessage(await res.Content.ReadAsStringAsync());
                if (string.IsNullOrWhiteSpace(detail))
                    detail = "Tên đăng nhập đã tồn tại hoặc dữ liệu không hợp lệ.";

                MessageBox.Show($"Thao tác thất bại (mã {(int)res.StatusCode}).\n{detail}", "Thất bại",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        /// <summary>
        /// Lấy trường "message" nếu API trả về JSON dạng { "message": "..." }.
        /// </summary>
        private static string ExtractMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body)) return string.Empty;

            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                    doc.RootElement.TryGetProperty("message", out var msg))
                {
                    return msg.GetString() ?? string.Empty;
                }
            }
            catch (JsonException)
            {
                // không phải JSON: dùng nguyên văn bên dưới
            }

            return body.Length > 300 ? body[..300] + "..." : body;
        }

        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
