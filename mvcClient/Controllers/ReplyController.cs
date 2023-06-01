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
            
            Reply result = await _apiClient.CreateReply(reply);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, Reply reply)
        {
            if (_apiClient.IsTokenExpired())
            {
                if (!await _apiClient.RefreshToken())
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            _apiClient.SetAccessToken();

            var result = await _apiClient.UpdateReply(id, reply);
            if (result)
            {
                return RedirectToAction("Detail", "Product", new { id = reply.ProductId });
            }
            return View();
        }

        [HttpDelete]
        // public IActionResult Delete(int id)
        public async Task<IActionResult> Delete(Reply reply)
        {
            if (_apiClient.IsTokenExpired())
            {
                if (!await _apiClient.RefreshToken())
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            _apiClient.SetAccessToken();

            var addedReply = await _apiClient.DeleteReply(reply.Id);
            return Ok(addedReply);

        }
    }
}
