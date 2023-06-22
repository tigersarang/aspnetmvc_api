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
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;

        public FilesController(ApplicationDbContext dbContext, ILogger<ProductsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound(new { message = "Failed to update the product. " });
            }

            _dbContext.Products.Remove(product);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
