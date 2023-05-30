using System.Linq;
using JwtVueCrudApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CommLibs.Extensions;
using CommLibs.Models;

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
                reply.User = _dbContext.Users.FirstOrDefault(u => u.Id == reply.UserId);
                reply.Product = _dbContext.Products.FirstOrDefault(p => p.Id == reply.ProductId);

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
