using CommLibs.Dto;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Dto;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace mvcClient.Utils
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApiClient(HttpClient httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        // JwtVueCrudApp의 AuthController.cs의 public IActionResult Register([FromBody] RegisterModel model)과 대응
        public async Task<HttpResponseMessage> Register(UserDto userDto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", userDto);
            return response;
        }

        // JwtVueCrudApp의 AuthController.cs의 public async Task<IActionResult> Login([FromBody] LoginModel model)과 대응
        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            var model = new UserDto { UserName = username, Password = password };
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

            var requestData = new Dictionary<string, string>
            {
                {"refreshToken", refreshToken }
            };

            var response = await _httpClient.PostAsJsonAsync("auth/refresh", requestData);
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

        // jwtvuecrudapp의 ProductsController의 모든 메서드 대응

        //public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        public async Task<PagedResult<ProductDto>> GetAll(int pageNumber = 1, int pageSize = 10, string search = null)
        {
            var response = await _httpClient.GetAsync($"products?pageNumber={pageNumber}&pageSize={pageSize}&search={search}");

            if (response.IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"product 조회 실패 :  {response.StatusCode}");
            }

            // 성공했을 때 리턴값을 PageResult<ProdcutDto>로 받음
            var result = response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>().Result;

            if (result.Items == null)
            {
                result.Items = new List<ProductDto>();
            }
            return result;
        }

        //public async Task<IActionResult> GetById(int id)
        public async Task<ProductDto> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }

        //public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        public async Task<bool> Create(ProductDto productDto)
        {
            var response = await _httpClient.PostAsJsonAsync("products", productDto);
            return response.IsSuccessStatusCode;
        }

        //public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        public async Task<bool> Update(int id, ProductDto productDto)
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{id}", productDto);
            return response.IsSuccessStatusCode;
        }

        //public async Task<IActionResult> Delete(int id)
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }

        // get roles
        public async Task<List<RoleDto>> GetRoles()
        {
            var response = await _httpClient.GetAsync("auth/roles");
            return await response.Content.ReadFromJsonAsync<List<RoleDto>>();
        }

        // get AdminAction
        public async Task<string> GetAdminActions()
        {
            var response = await _httpClient.GetAsync("auth/AdminAction");
            return await response.Content.ReadAsStringAsync();
        }

    }
}
