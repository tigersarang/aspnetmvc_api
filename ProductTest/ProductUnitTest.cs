using CommLibs.Dto;
using CommLibs.Models;
using JwtVueCrudApp.Controllers;
using JwtVueCrudApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ProductTest
{
    [TestClass]
    public class ProductUnitTest
    {
        ProductsController _productsController;
        ApplicationDbContext _dbContext;
        private readonly ILogger<ProductsController> _logger;


        public ProductUnitTest()
        {
            _dbContext = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "tempProduct")
                .EnableSensitiveDataLogging().Options);
            _productsController = new ProductsController(_dbContext, _logger);
        }

        // Product CRUD Test
        [TestMethod]
        public async Task CreateProductTest()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Content = "Test Description",
                Price = 100,
                UserId = 1
            };
            // Act
            var result = await _productsController.Create(product);
            // Assert
            if (result is OkObjectResult okResult)
            {
                var item = okResult.Value as Product;
                Assert.AreEqual(product.Name, item.Name);

                _productsController.Delete(item.Id);
            }
            else
            {
                Assert.Fail();
            }
        }

        // Product Delete Test
        [TestMethod]
        public async Task DeleteProductTest()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Content = "Test Description",
                Price = 100,
                UserId = 1
            };
            // Act
            var result = await _productsController.Create(product);
            // Assert
            if (result is OkObjectResult okResult)
            {
                var item = okResult.Value as Product;
                Assert.AreEqual(product.Name, item.Name);
                _productsController.Delete(item.Id);

            }
            else
            {
                Assert.Fail();
            }

            // Delete Product
            await _productsController.Delete(product.Id);

            var delProduct = await _productsController.GetById(product.Id);

            Assert.AreEqual(((Microsoft.AspNetCore.Mvc.StatusCodeResult)delProduct).StatusCode, StatusCodes.Status404NotFound);
        }

        // Get All Product Test
        [TestMethod]
        public async Task GetAllProductTest()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Content = "Test Description",
                Price = 100,
                UserId = 1
            };
            // Act
            var result = await _productsController.Create(product);

            product = new Product
            {
                Name = "Test Product1",
                Content = "Test Description1",
                Price = 1000,
                UserId = 1
            };
            // Act
            result = await _productsController.Create(product);


            // Get All Product
            var allProduct = await _productsController.GetAll();
            Assert.AreEqual(((Microsoft.AspNetCore.Mvc.OkObjectResult)allProduct).StatusCode, StatusCodes.Status200OK);

            if (allProduct is OkObjectResult okResult3)
            {
                var item = okResult3.Value as PagedResult<Product>;
                Assert.AreEqual(2, item.TotalCount);
            }
            else { Assert.Fail(); }
        }

    }
}