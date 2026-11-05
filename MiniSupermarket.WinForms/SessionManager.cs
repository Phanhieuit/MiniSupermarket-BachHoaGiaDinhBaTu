using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniSupermarket.WinForms
{
    public static class SessionManager
    {
        public static string JwtToken { get; set; } = string.Empty;
        public static string CurrentRole { get; set; } = string.Empty;
        public static string CurrentUsername { get; set; } = string.Empty;
    }

    public static class ApiClientService
    {
        private static readonly HttpClient _client = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7237/api/")  // nhớ chỉnh port phù hợp
        };

        /// <summary>
        /// HttpClient dùng chung cho các form (FormProductManagement, FormUserManagement...).
        /// Mỗi lần lấy ra sẽ tự gắn Bearer Token theo phiên đăng nhập hiện tại,
        /// và tự gỡ header khi đã đăng xuất (JwtToken rỗng).
        /// </summary>
        public static HttpClient Client
        {
            get
            {
                _client.DefaultRequestHeaders.Authorization =
                    string.IsNullOrEmpty(SessionManager.JwtToken)
                        ? null
                        : new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);

                return _client;
            }
        }

        // Hàm gọi API đăng nhập lấy Token
        public static async Task<bool> LoginAsync(string username, string password)
        {
            var loginObj = new { Username = username, Password = password };
            var response = await _client.PostAsJsonAsync("auth/login", loginObj);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                SessionManager.JwtToken = doc.RootElement.GetProperty("token").GetString() ?? string.Empty;
                SessionManager.CurrentRole = doc.RootElement.GetProperty("role").GetString() ?? string.Empty;

                // Lưu lại username để lát sau dùng hiển thị lên form
                SessionManager.CurrentUsername = username;

                return true;
            }
            return false;
        }

        // Hàm gọi API lấy dữ liệu có gắn kèm Bearer Token bảo mật
        public static async Task<string> GetDataWithTokenAsync(string endpoint)
        {
            var response = await Client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                throw new Exception("Phiên làm việc hết hạn hoặc chưa đăng nhập!");
            }
            throw new Exception("Lỗi khi gọi dữ liệu từ Server.");
        }
    }
}