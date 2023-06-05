using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Utils;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace mvcClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApiClient apiClient, ILogger<AccountController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }
        // login, logout, register, etc.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password)
        {
            try
            {
                // JwtVueCrudApp에서 AuthController.cs의 public async Task<IActionResult> Login([FromBody] LoginModel model)과 대응.
                // token, refreshToken을 받아온다.그리고 session에 저장한다.
                var response = await _apiClient.Login(userName, password);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<TokenDto>(result);

                    HttpContext.Session.SetString("AccessToken", token.Token);
                    HttpContext.Session.SetString("TokenExpiration", JwtDecoder.GetExpirationDate(token.Token).ToString());
                    HttpContext.Session.SetString("refreshToken", token.RefreshToken);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "로그인에 실패하였습니다.");
                return View();
            } catch(Exception ex)
            {
                _logger.LogError(ex, "로그인에 실패하였습니다.");
                ModelState.AddModelError("", ex.Message);
                
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // getRoles()를 호출하여 role을 가져온다.
            var roles = await _apiClient.GetRoles();

            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            var response = await _apiClient.Register(user);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "회원가입에 실패하였습니다.");
            return View();
        }
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
