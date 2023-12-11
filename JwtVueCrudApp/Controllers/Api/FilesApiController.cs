
using CommLibs.Dto;
using JwtVueCrudApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtVueCrudApp.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FilesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<FilesApiController> _logger;

        public FilesApiController(ApplicationDbContext applicationDbContext, ILogger<FilesApiController> logger)
        {
            _dbContext = applicationDbContext;
            _logger = logger;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var productFile = await _dbContext.ProductFiles.FirstOrDefaultAsync(p => p.Id == id);

            if (productFile == null)
            {
                return NotFound(new { message = "Failed to update the product. " });
            }

            _dbContext.ProductFiles.Remove(productFile);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFile(int fileId)
        {
            try
            {
                var productFile = await _dbContext.ProductFiles.FirstOrDefaultAsync(p => p.Id == fileId);

                if (productFile == null)
                {
                    return NotFound(new { message = "Failed to fetch the File. " });
                }
                return Ok(productFile);
            } catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Failed to fetch the File. " });
            }
        }
    }
}
