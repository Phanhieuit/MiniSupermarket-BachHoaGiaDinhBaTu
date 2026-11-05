namespace MiniSupermarket.WinForms;

public partial class FormLogin : Form
{
    public FormLogin() { InitializeComponent(); AcceptButton = btnLogin; }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrEmpty(txtPass.Text))
        {
            MessageBox.Show("Vui lòng nhập tài khoản và mật khẩu.");
            return;
        }
        btnLogin.Enabled = false;
        try
        {
            if (!await ApiClientService.LoginAsync(txtUser.Text.Trim(), txtPass.Text))
            {
                MessageBox.Show("Sai tài khoản, mật khẩu hoặc tài khoản đã bị khóa.");
                return;
            }
            Hide();
            using var shell = new FormMainShell();
            var result = shell.ShowDialog();
            SessionManager.JwtToken = SessionManager.CurrentRole = SessionManager.CurrentUsername = string.Empty;
            ApiClientService.Client.DefaultRequestHeaders.Authorization = null;
            if (result == DialogResult.Retry)
            {
                txtPass.Clear();
                Show();
                txtPass.Focus();
            }
            else Close();
        }
        catch (Exception ex) { Show(); MessageBox.Show("Lỗi kết nối máy chủ: " + ex.Message); }
        finally { if (!IsDisposed) btnLogin.Enabled = true; }
    }
    private void FormLogin_Load(object sender, EventArgs e) { }
    private void txtUser_TextChanged(object sender, EventArgs e) { }
}
