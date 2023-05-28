using System.Linq;
using JwtVueCrudApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommLibs.Extensions;

namespace JwtVueCrudApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RepliesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public RepliesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // POST: api/products
        [HttpPost]
        public IActionResult Create([FromBody] Reply reply)
        {
            // Models.Reply Create. 
            if (ModelState.IsValid)
            {
                User user = _dbContext.Users.FirstOrDefault(u => u.Id == reply.UserId);
                if (user == null)
                {
                    return NotFound("로그인 사용자 정보가 없습니다.");
                }
                reply.User = user;
                _dbContext.Replies.Add(reply);
                _dbContext.SaveChanges();
                return Ok(reply);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/replies/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Reply reply)
        {
            if (ModelState.IsValid)
            {
                var existingReply = _dbContext.Replies.FirstOrDefault(p => p.Id == id);
                if (existingReply == null)
                {
                    return NotFound();
                }

                existingReply.Content = reply.Content;
                existingReply.UpdatedDate = DateTime.Now;

                _dbContext.SaveChanges();
                return Ok(existingReply);
            }

            return BadRequest(ModelState);
        }

        // DELETE: api/replies/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reply = _dbContext.Replies.FirstOrDefault(p => p.Id == id);
            if (reply == null)
            {
                return NotFound();
            }

            _dbContext.Replies.Remove(reply);
            _dbContext.SaveChanges();
            return Ok(reply);
        }

    }
}
