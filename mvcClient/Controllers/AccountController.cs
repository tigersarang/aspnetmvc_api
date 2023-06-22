using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using mvcClient.Models;
using mvcClient.Utils;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace mvcClient.Controllers
{
    [Route("[controller]/[action]")]
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
            ViewData["returnUrl"] = TempData["returnUrl"];
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl)
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

                    JwtDecoder.GetClaims(token.Token).ToList().ForEach(c =>
                    {
                        if (c.Type == "role") HttpContext.Session.SetString(c.Type, c.Value);
                        else if (c.Type == "nameid") HttpContext.Session.SetString("UserId", c.Value);
                    });

                    HttpContext.Session.SetString("AccessToken", token.Token);
                    HttpContext.Session.SetString("TokenExpiration", JwtDecoder.GetExpirationDate(token.Token).ToString());
                    HttpContext.Session.SetString("refreshToken", token.RefreshToken);
                    HttpContext.Session.SetString("UserName", userName);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        return Redirect(returnUrl);
                    }
                }
                else
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorViewModel>(result);

                    ModelState.AddModelError("", error.Message);
                    return View();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그인에 실패하였습니다.");
                ModelState.AddModelError("", "Failed to login.");

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
            try
            {
                var response = await _apiClient.Register(user);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login");
                }

                var modelStateErrors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();

                foreach (var oneError in modelStateErrors)
                {
                    _logger.LogError("register error : " + oneError.Key + " : " + oneError.Value);

                    foreach (var errorValue in oneError.Value)
                    {
                        ModelState.AddModelError(oneError.Key, errorValue);
                    }
                }

                var roles = await _apiClient.GetRoles();
                return View(roles);
            }
            catch (Exception ex)
            {
                var roles = await _apiClient.GetRoles();
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
        }
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
