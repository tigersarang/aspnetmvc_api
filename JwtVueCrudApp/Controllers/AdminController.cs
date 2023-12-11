using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.Extensions.Logging;
using JwtVueCrudApp.Models;
using CommLibs.Models;
using Microsoft.EntityFrameworkCore;

namespace JwtVueCrudApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Admin Role만 접근이 가능합니다.
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AdminController> _logger;
        public AdminController(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<AdminController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("adminaction")]
        public async Task<IActionResult> AdminAction()
        {
            return Ok("AdminIndex");
        }

    }
}
