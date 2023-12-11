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
            ModelState.AddModelError("", "Some error occured");
            return BadRequest(ModelState);
        }

        // DELETE: api/replys/{id}
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
            return NoContent();
        }

    }
}
