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
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILogger<ApiClient> _logger;


        public ApiClient(HttpClient httpClient, IHttpContextAccessor contextAccessor, ILogger<ApiClient> logger)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
            _logger = logger;
        }

    }
}
