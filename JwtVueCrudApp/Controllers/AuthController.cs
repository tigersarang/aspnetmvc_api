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
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        public AuthController(ApplicationDbContext dbContext, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] User model)
        {
            if (_dbContext.Users.Any(u => u.UserName == model.UserName))
            {
                ModelState.AddModelError("-1", "Username already exists.");
            }

            if (model.RoleId == null)
            {
                ModelState.AddModelError("-2", "The role value is missing.");
            }

            if (ModelState.IsValid)
            {


                Role role = _dbContext.Roles.SingleOrDefault(r => r.Id == model.RoleId);

                var user = new User { UserName = model.UserName, Password = BCrypt.Net.BCrypt.HashPassword(model.Password), Role = role };

                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return Ok();
            }

            return BadRequest(ModelState);

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            _logger.LogWarning("Login Start...");

            try
            {
                if (ModelState.IsValid)
                {
                    var user = _dbContext.Users.Include(u => u.Role).SingleOrDefault(u => u.UserName == model.UserName);

                    if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                    {
                        string accessToken = GenerateAccessToken(user);

                        if (string.IsNullOrEmpty(accessToken) == true)
                        {
                            return BadRequest(new { message = "Failed to generate token." });
                        }

                        string refreshToken = GenerateRefreshToken();

                        user.RefreshToken = refreshToken;
                        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                        await _dbContext.SaveChangesAsync();
                        return Ok(new { Token = accessToken, RefreshToken = refreshToken });
                    }
                    else
                    {
                        return BadRequest(new { message = "Please check your user ID and password" });
                    }
                }
                return BadRequest(new { message = "This is an incorrect approach." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "This is an incorrect approach." });

            }
        }
        // get : /api/auth/Roles
        [AllowAnonymous]
        [HttpGet("Roles")]
        public IActionResult GetRoles()
        {
            var roles = _dbContext.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] string model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // User에 refresh token이 있는지 확인. 그리고 refresh token이 만료되지 않았는지 확인.
                    var user = _dbContext.Users.SingleOrDefault(u => u.RefreshToken == model && u.RefreshTokenExpiry > DateTime.UtcNow);

                    if (user != null)
                    {
                        string accessToken = GenerateAccessToken(user);

                        if (string.IsNullOrEmpty(accessToken) == true)
                        {
                            return BadRequest(new { message = "Failed to generate token." });
                        }

                        return Ok(new { Token = accessToken });
                    }
                    else
                    {
                        return BadRequest(new { message = "The login user information is incorrect." });
                    }
                }
                return BadRequest(new { message = "This is an incorrect approach." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = "This is an incorrect approach." });

            }
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private string GenerateAccessToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Role, user.Role.Name)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"]
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;

            }
        }
    }
}
