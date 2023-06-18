using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Utils;

namespace mvcClient.Controllers
{
    public class ReplyController : Controller
    {
        private readonly ApiClient _apiClient;
        public ReplyController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Reply reply)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                reply.UserId = int.Parse(HttpContext.Session.GetString("UserId"));
                var result = await _apiClient.CreateReply(reply);
                if (result == null)
                {
                    return BadRequest();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to create the reply" });
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                var product = await _apiClient.DeleteReply(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to delete the reply" });
            }
        }


    }
}
