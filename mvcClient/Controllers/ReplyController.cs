using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Delete(int id)
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


    }
}
