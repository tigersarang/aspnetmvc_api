﻿using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<HttpResponseMessage> Register(LoginDto userDto)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/register", userDto);
            return response;
        }

        // JwtVueCrudApp의 AuthController.cs의 public async Task<IActionResult> Login([FromBody] LoginModel model)과 대응
        public async Task<HttpResponseMessage> Login(string username, string password)
        {
            var model = new LoginDto { UserName = username, Password = password };
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
        public async Task<PagedResult<Product>> GetAll(int pageNumber = 1, int pageSize = 10, string search = null)
        {
            var response = await _httpClient.GetAsync($"products?pageNumber={pageNumber}&pageSize={pageSize}&search={search}");

            if (response.IsSuccessStatusCode == true)
            {
                // 성공했을 때 리턴값을 PageResult<ProdcutDto>로 받음
                var result = response.Content.ReadFromJsonAsync<PagedResult<Product>>().Result;

                if (result.Items == null)
                {
                    result.Items = new List<Product>();
                }
                return result;

            } else
            {
                throw new HttpRequestException($"product 조회 실패 :  {response.StatusCode}");
            }
        }

        //public async Task<IActionResult> GetById(int id)
        public async Task<Product> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");
            var result = await response.Content.ReadAsStringAsync();
            Product product = JsonConvert.DeserializeObject<Product>(result);
            return product;
        }

        //public async Task<IActionResult> Create([FromBody] Product product)
        public async Task<bool> Create(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("products", product);
            return response.IsSuccessStatusCode;
        }

        //public async Task<IActionResult> Update(int id, [FromBody] Product product)
        public async Task<bool> Update(int id, Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);
            return response.IsSuccessStatusCode;
        }

        //public async Task<IActionResult> Delete(int id)
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }

        // get roles
        public async Task<List<Role>> GetRoles()
        {
            var response = await _httpClient.GetAsync("auth/roles");
            return await response.Content.ReadFromJsonAsync<List<Role>>();
        }

        // get AdminAction
        public async Task<string> GetAdminActions()
        {
            var response = await _httpClient.GetAsync("auth/AdminAction");
            return await response.Content.ReadAsStringAsync();
        }

        // jwtvuecrudapp 의 RepliesController의 Create 메서드 대응
        public async Task<Reply?> CreateReply(Reply reply)
        {
            var response = await _httpClient.PostAsJsonAsync("replies", reply);

            if (response.IsSuccessStatusCode)
            {
                var addedReply = response.Content.ReadFromJsonAsync<Reply>().Result;
                return addedReply;

            }
            return null;
        }

        // jwtvuecrudapp 의 RepliesController의 Delete 메서드 대응
        public async Task<bool> DeleteReply(int id)
        {
            var response = await _httpClient.DeleteAsync($"replies/{id}");
            return response.IsSuccessStatusCode;
        }

        // jwtvuecrudapp 의 RepliesController의 Update 메서드 대응
        public async Task<bool> UpdateReply(int id, Reply reply)
        {
            var response = await _httpClient.PutAsJsonAsync($"replies/{id}", reply);
            return response.IsSuccessStatusCode;
        }








    }
}
