using Microsoft.AspNetCore.Mvc;
using mvcClient.Utils;

namespace mvcClient.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly ApiClient _apiClient;

        public AdminController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        // Admin 사용자만 접근이 가능한 페이지
        public async Task<IActionResult> AdminIndex()
        {
            try
            {
                // 토큰이 만료되었는지 확인한다.
                if (_apiClient.IsTokenExpired())
                {
                    // 만료되었다면 리프레시 토큰을 이용하여 토큰을 재발급 받는다.
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                // 토큰이 만료되지 않았다면 토큰을 설정한다.
                _apiClient.SetAccessToken();

                // string값을 리턴 받는다.
                var auth = await _apiClient.GetAdminAction();

                if (string.IsNullOrEmpty(auth))
                {
                    return RedirectToAction("Login", "Account");
                }

                return View(auth);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
