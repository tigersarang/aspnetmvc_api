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
            if (pageNumber < 1 || pageSize < 1) return BadRequest();

            IQueryable<Product> query = _dbContext.Products;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search)).AsNoTracking();
            }

            var pagedResult = await query.Include(u => u.User).OrderByDescending(p => p.Id).ToPagedResultAsync(pageNumber, pageSize);

            return Ok(pagedResult);

        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _dbContext.Products.Include(r => r.Replies).FirstAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Error occured while getting product by id");
                return BadRequest(ex.Message);
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (ModelState.IsValid)
            {
                User user = await _dbContext.Users.FirstAsync(u => u.Id == product.UserId);
                product.User = user;
                await _dbContext.Products.AddAsync(product);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product updatedProduct)
        {
            if (ModelState.IsValid)
            {
                var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = updatedProduct.Name;
                product.Price = updatedProduct.Price;
                product.Content = updatedProduct.Content;
                product.UpdatedDate = DateTime.Now;

                _dbContext.SaveChanges();
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return NoContent();
        }
    }
}
