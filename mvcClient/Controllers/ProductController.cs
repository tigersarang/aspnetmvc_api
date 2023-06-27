using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Utils;
using Newtonsoft.Json;
using NuGet.Packaging;

namespace mvcClient.Controllers
{
    [Route("[controller]/[action]")]
    public class ProductController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<AccountController> _logger;

        public ProductController(ApiClient apiClient, ILogger<AccountController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        [HttpGet("/Product")]
        [HttpGet("/Product/Index")]
        //  public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string search = null)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        TempData["returnUrl"] = "/Product";
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                var response = await _apiClient.GetAll(pageNumber, pageSize, search);

                if (response.IsSuccessStatusCode == false)
                {
                    string errorJson = string.Empty;
                    errorJson = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorViewModel>(errorJson);
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = error.Message });
                }

                // 성공했을 때 리턴값을 PageResult<ProdcutDto>로 받음
                var products = response.Content.ReadFromJsonAsync<PagedResult<Product>>().Result;

                if (products?.Items == null)
                {
                    products.Items = new List<Product>();
                }

                return View(products);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to retrieve the product list." });
            }
        }

        [HttpGet("{id}")]
        // public IActionResult GetById(int id)
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                var product = await _apiClient.GetById(id);

                return View(product);
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "An error occurred while retrieving detailed information. Please contact the provider." });
            }
        }


        //  public IActionResult Create([FromBody] Product product)
        public async Task<IActionResult> Create(int? id)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                Product product = new Product();
                if (id.GetValueOrDefault() > 0)
                {
                    product = await _apiClient.GetById(id.GetValueOrDefault());
                } else
                {
                    product = new Product();
                    product.ProductFiles = new List<ProductFile>();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "An error occurred while transitioning to the project creation screen." });
            }


        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            ErrorDto errorDto = new ErrorDto();

            try
            {
                if (ModelState.IsValid == false)
                {
                    errorDto = new ErrorDto
                    {
                        Id = "-999",
                        Message = "Please enter the required information."
                    };

                    return BadRequest(errorDto);
                }

                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                productDto.UserId = int.Parse(HttpContext.Session.GetString("UserId"));

                Product product = new Product();

                // If productdto.is exists, execute the update.
                if (productDto.Id > 0)
                {
                    product = await _apiClient.GetById(productDto.Id);
                }

                // Save the modified product information.
                product.Name = productDto.Name;
                product.Content = productDto.Content;
                product.UserId = productDto.UserId;
                product.ProductFiles = new List<ProductFile>();

                string upDir = Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, GV.I.UD);

                if (Directory.Exists(upDir) == false)
                {
                    Directory.CreateDirectory(upDir);
                }

                List<ProductFile> productFiles = new List<ProductFile>();

                if (productDto.files != null)
                {

                    foreach (var file in productDto.files)
                    {
                        System.IO.File.Move(Path.Combine(Directory.GetCurrentDirectory(), GV.I.RD, file), Path.Combine(upDir, file));
                        productFiles.Add(new ProductFile { FileName = file.Split("___")[1], LInkFileName = file });
                    }
                    product.ProductFiles.AddRange(productFiles);
                }

                HttpResponseMessage? response;

                if (productDto.Id > 0)
                {
                    response = await _apiClient.UpdateAsync(productDto.Id, product);

                } else
                {
                    response = await _apiClient.Create(product);

                }

                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var productResult = JsonConvert.DeserializeObject<Product>(result);
                    return Ok("/Product/Detail/" + productResult.Id);
                }

                try
                {
                    var modelStateErrors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                    var oneError = modelStateErrors.FirstOrDefault();
                    _logger.LogError("register error : " + oneError.Key + " : " + oneError.Value);

                    errorDto = new ErrorDto
                    {
                        Id = oneError.Key,
                        Message = oneError.Value.FirstOrDefault()
                    };
                } catch(Exception ex)
                {
                    errorDto = new ErrorDto
                    {
                        Id = "-999",
                        Message = result
                    };

                    return BadRequest(errorDto);
                }



                return BadRequest(errorDto);

            }
            catch (Exception ex)
            {
                errorDto = new ErrorDto
                {
                    Id = "-999",
                    Message = ex.Message
                };

                return BadRequest(errorDto);
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Update(int id, Product product)
        //{
        //    try
        //    {
        //        if (_apiClient.IsTokenExpired())
        //        {
        //            if (!await _apiClient.RefreshToken())
        //            {
        //                return RedirectToAction("Login", "Account");
        //            }
        //        }

        //        _apiClient.SetAccessToken();

        //        var result = await _apiClient.Update(id, product);
        //        if (result)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        return View(product);
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to update the project" });
        //    }
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (_apiClient.IsTokenExpired())
                {
                    if (!await _apiClient.RefreshToken())
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                var product = await _apiClient.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "There was an error deleting the product." });
            }
        }
    }
}
