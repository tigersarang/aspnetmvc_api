using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace mvcClient.Utils
{
    public partial class ApiClient
    {
        // JwtVueCrudApp의 AuthController.cs의 public IActionResult Register([FromBody] RegisterModel model)과 대응
        public async Task<HttpResponseMessage> Register(User user)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", user);
            return response;
        }

        // JwtVueCrudApp의 AuthController.cs의 public async Task<IActionResult> Login([FromBody] LoginModel model)과 대응
        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            var model = new User { UserName = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("auth/login", model);
            return response;
        }

        public void SetAccessToken()
        {
            // token header 추가하기
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext.Session.GetString("AccessToken"));
        }

        // token 유효기간 체크
        public bool IsTokenExpired()
        {
            var tokenExpiration = _contextAccessor.HttpContext.Session.GetString("TokenExpiration");
            if (string.IsNullOrEmpty(tokenExpiration))
            {
                return true;
            }

            if (DateTime.TryParse(tokenExpiration, out DateTime expirationDate) == false)
            {
                return true;
            }

            return expirationDate < DateTime.UtcNow;
        }

        // refresh token으로 access token 재발급
        public async Task<bool> RefreshToken()
        {
            var refreshToken = _contextAccessor.HttpContext.Session.GetString("refreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }

            var response = await _httpClient.PostAsJsonAsync("auth/refresh", refreshToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<TokenDto>(result);
                _contextAccessor.HttpContext.Session.SetString("AccessToken", token.Token);
                _contextAccessor.HttpContext.Session.SetString("TokenExpiration", JwtDecoder.GetExpirationDate(token.Token).ToString());
                return true;
            }
            return false;
        }

        // getRoles 메서드 추가
        public async Task<List<Role>> GetRoles()
        {
            var response = await _httpClient.GetAsync("auth/roles");
            return await response.Content.ReadFromJsonAsync<List<Role>>();
        }

        public async Task<string?> GetAdminAction()
        {
            // AdminAction을 가져오는 API 호출
            var response = await _httpClient.GetAsync("Admin/adminaction");

            // 성공이면
            if (response.IsSuccessStatusCode)
            {
                // 결과를 string으로 변환해서 리턴
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }

    }
}
