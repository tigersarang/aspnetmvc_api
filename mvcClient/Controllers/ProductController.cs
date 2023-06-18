using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Utils;
using Newtonsoft.Json;

namespace mvcClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApiClient _apiClient;
        public ProductController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }


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

                var response = await _apiClient.GetById(id);

                if (response.IsSuccessStatusCode == false)
                {
                    string errorJson = string.Empty;
                    errorJson = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ErrorViewModel>(errorJson);
                    return RedirectToAction("Error", "Home", new ErrorViewModel { Message = error.Message });
                }

                var product = response.Content.ReadFromJsonAsync<Product>().Result;

                return View(product);
            }
            catch (Exception ex)
            {

                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "An error occurred while retrieving detailed information. Please contact the provider." });
            }
        }


        //  public IActionResult Create([FromBody] Product product)
        public async Task<IActionResult> Create()
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

                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "An error occurred while transitioning to the project creation screen." });
            }


        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
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

                productDto.UserId = int.Parse(HttpContext.Session.GetString("UserId"));

                Product product = new Product
                {
                    Name = productDto.Name,
                    Content = productDto.Content,
                    UserId = productDto.UserId
                };

                string upDir = Path.Combine(GV.I.RD, GV.I.UD);

                if (Directory.Exists(upDir) == false)
                {
                    Directory.CreateDirectory(upDir);
                }

                List<ProductFile> productFiles = new List<ProductFile>();

                if (productDto.files != null)
                {

                    foreach (var file in productDto.files)
                    {
                        System.IO.File.Move(Path.Combine(GV.I.RD, file), Path.Combine(upDir, file));
                        productFiles.Add(new ProductFile { FileName = file.Split("___")[1], LInkFileName = file });
                    }
                }

                product.ProductFiles = productFiles;

                var response = await _apiClient.Create(product);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var productResult = JsonConvert.DeserializeObject<Product>(result);
                    return Ok($"/Product/Detail/{productResult.Id}");
                } 

                var error = JsonConvert.DeserializeObject<ErrorViewModel>(result);
                return BadRequest(new { message = error });

            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to create the project" });
            }
        }

        // public IActionResult Update(int id, [FromBody] Product product)
        public async Task<IActionResult> Update(int id)
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
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product)
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

                var result = await _apiClient.Update(id, product);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                return View(product);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to update the project" });
            }
        }

        // public IActionResult Delete(int id)
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
                return RedirectToAction("Index", "Product");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home", new ErrorViewModel { Message = "Failed to delete the project" });
            }
        }
    }
}
