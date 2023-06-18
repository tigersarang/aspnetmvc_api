﻿using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
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
                return RedirectToAction("Login", "Account");
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
        public async Task<IActionResult> Create(ProductDto productDto)
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

            var product = await _apiClient.Delete(id);
            return RedirectToAction("Index", "Product");
        }


    }
}
