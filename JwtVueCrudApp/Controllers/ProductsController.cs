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
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ApplicationDbContext dbContext, ILogger<ProductsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1) return BadRequest();

                IQueryable<Product> query = _dbContext.Products;

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p => p.Name.Contains(search)).AsNoTracking();
                }

                var pagedResult = await query.Include(u => u.User).OrderByDescending(p => p.Id).ToPagedResultAsync(pageNumber, pageSize);

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "Failed to fetch the product list. " });
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _dbContext.Products.Include(u => u.User).Include(f => f.ProductFiles).FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return NotFound(new { message = "Failed to fetch product information" });
                }

                // product.Content에 저장된 파일의 내용을 읽어서 Content에 저장합니다.
                product.Content = await System.IO.File.ReadAllTextAsync(product.Content);

                var replies = await _dbContext.Replies.Include(ru => ru.User).Where(r => r.ProductId == id).OrderByDescending(o => o.Id).ToListAsync();
                product.Replies = replies;
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetById Error");
                return BadRequest(new { message = "Failed to fetch the product list. " });
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Content 값을 파일로 저장을 합니다. 그리고 저장된 파일의 경로를 Content에 저장합니다.
                    var contentPath = GlobalSettings.Instance.ProductContentPath;
                    var contentFileName = $"{Guid.NewGuid()}.txt";
                    var saveDir = Path.Combine(AppContext.BaseDirectory, contentPath);

                    //saveDir가 없으면 생성
                    if (!Directory.Exists(saveDir))
                    {
                        Directory.CreateDirectory(saveDir);
                    }

                    var contentFilePath = Path.Combine(saveDir, contentFileName);
                    await System.IO.File.WriteAllTextAsync(contentFilePath, product.Content);

                    product.Content = contentFilePath;

                    await _dbContext.Products.AddAsync(product);
                    await _dbContext.SaveChangesAsync();
                    return Ok(product);
                }
                return BadRequest(new { message = "Failed to create the product list. " });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Error");
                return BadRequest(new { message = "Failed to create the product. " });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updatedProduct)
        {
            try
            {
                string oldFilePath = string.Empty;
                if (ModelState.IsValid)
                {
                    var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                    oldFilePath = product.Content;

                    if (product == null)
                    {
                        return NotFound();
                    }

                    // Content 값을 파일로 저장을 합니다. 그리고 저장된 파일의 경로를 Content에 저장합니다.
                    var contentFileName = $"{Guid.NewGuid()}.txt";
                    var saveDir = Path.Combine(AppContext.BaseDirectory, GlobalSettings.Instance.ProductContentPath);

                    var contentFilePath = Path.Combine(saveDir, contentFileName);
                    await System.IO.File.WriteAllTextAsync(contentFilePath, updatedProduct.Content);

                    product.Content = contentFilePath;
                    product.Name = updatedProduct.Name;
                    product.Price = updatedProduct.Price;
                    product.UpdatedDate = DateTime.Now;

                    await _dbContext.SaveChangesAsync();

                    // 기존 파일 삭제
                    if (!string.IsNullOrEmpty(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    return NoContent();
                }
                return BadRequest(new { message = "Failed to update the product. " });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Update Error");
                return BadRequest(new { message = "Failed to update the product. " });
            }
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
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
