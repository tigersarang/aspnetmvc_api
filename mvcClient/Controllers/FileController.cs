using CommLibs.Dto;
using CommLibs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvcClient.Models;
using mvcClient.Utils;

namespace mvcClient.Controllers
{
    [Route("[controller]/[action]")]
    public class FileController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly ILogger<FileController> _logger;

        public FileController(ApiClient apiClient, ILogger<FileController> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }



    }
}
