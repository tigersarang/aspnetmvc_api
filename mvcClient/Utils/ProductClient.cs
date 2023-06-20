using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace mvcClient.Utils
{
    public partial class ApiClient
    {

        // jwtvuecrudapp의 ProductsController의 모든 메서드 대응

        //public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string search = null)
        public async Task<HttpResponseMessage> GetAll(int pageNumber = 1, int pageSize = 10, string search = null)
        {
            var response = await _httpClient.GetAsync($"products?pageNumber={pageNumber}&pageSize={pageSize}&search={search}");
            return response;
        }

        //public async Task<IActionResult> GetById(int id)
        public async Task<Product> GetById(int id)
        {
            var response = await _httpClient.GetAsync($"products/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            } else
            {
                return null;
            }
        }

        //public async Task<IActionResult> Create([FromBody] Product product)
        public async Task<HttpResponseMessage> Create(Product product)
        {
            var response = await _httpClient.PostAsJsonAsync("products", product);
            return response;
        }

        //public async Task<IActionResult> Update(int id, [FromBody] Product product)
        public async Task<HttpResponseMessage> UpdateAsync(int id, Product product)
        {
            var response = await _httpClient.PutAsJsonAsync($"products/{id}", product);
            return response;
        }

        //public async Task<IActionResult> Delete(int id)
        public async Task<bool> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"products/{id}");
            return response.IsSuccessStatusCode;
        }

    }
}
