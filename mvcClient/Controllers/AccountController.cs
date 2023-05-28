using CommLibs.Dto;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Dto;
using mvcClient.Utils;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace mvcClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiClient _apiClient;
        public AccountController(ApiClient apiClient)
        {
            _apiClient = apiClient;
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
                });

                HttpContext.Session.SetString("AccessToken", token.Token);
                HttpContext.Session.SetString("TokenExpiration", JwtDecoder.GetExpirationDate(token.Token).ToString());
                HttpContext.Session.SetString("refreshToken", token.RefreshToken);
                HttpContext.Session.SetString("UserName", userName);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "로그인에 실패하였습니다.");
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            // getRoles()를 호출하여 role을 가져온다.
            var roles = await _apiClient.GetRoles();

            if (roles == null) roles = new List<RoleDto>();

            return View(roles);
        }
        [HttpPost]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            var response = await _apiClient.Register(userDto);


            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            var roles = await _apiClient.GetRoles();
            if (roles == null) roles = new List<RoleDto>();


            ModelState.AddModelError("", "회원가입에 실패하였습니다.");
            return View(roles);
        }
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }


    }
}
