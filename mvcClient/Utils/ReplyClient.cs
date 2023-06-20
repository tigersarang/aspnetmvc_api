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


        // Reply 작업
        public async Task<Reply?> CreateReply(Reply reply)
        {
            var response = await _httpClient.PostAsJsonAsync("replies", reply);
            return await response.Content.ReadFromJsonAsync<Reply>();
        }
        public async Task<bool> DeleteReply(int id)
        {
            var response = await _httpClient.DeleteAsync($"replies/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
