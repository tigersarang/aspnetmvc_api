using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Utils;

namespace mvcClient.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiClient _apiClient;

        public AdminController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> AdminIndex()
        {
            if (_apiClient.IsTokenExpired())
            {
                if (!await _apiClient.RefreshToken())
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            _apiClient.SetAccessToken();

            var auth = await _apiClient.GetAdminActions();

            if (string.IsNullOrEmpty(auth))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}
