using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Utils;

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
                        return RedirectToAction("Login", "Account");
                    }
                }

                _apiClient.SetAccessToken();

                var products = await _apiClient.GetAll(pageNumber, pageSize, search);

                //if (response.IsSuccessStatusCode)
                //{
                //    return RedirectToAction("Login");
                //}

                return View(products);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login","Account");
            }
        }

        // public IActionResult GetById(int id)
        public async Task<IActionResult> Detail(int id)
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


        //  public IActionResult Create([FromBody] Product product)
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (_apiClient.IsTokenExpired())
            {
                if (!await _apiClient.RefreshToken())
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            _apiClient.SetAccessToken();

            product.UserId = int.Parse(HttpContext.Session.GetString("UserId"));

            var result = await _apiClient.Create(product);
            if (result)
            {
                return RedirectToAction("Index");
            }
            return View(product);
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

        [HttpDelete]
        // public IActionResult Delete(int id)
        public async Task<IActionResult> Delete(int id)
        {
            if (_apiClient.IsTokenExpired())
            {
                if (!await _apiClient.RefreshToken())
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            _apiClient.SetAccessToken();

            await _apiClient.Delete(id);
            return Ok();
        }


    }
}
